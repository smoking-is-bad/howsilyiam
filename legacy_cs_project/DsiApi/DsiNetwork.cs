// Copyright (c) 2015 Sensor Networks, Inc.
// 
// All rights reserved. No part of this publication may be reproduced,
// distributed, or transmitted in any form or by any means, including
// photocopying, recording, or other electronic or mechanical methods, without
// the prior written permission of Sensor Networks, Inc., except in the case of
// brief quotations embodied in critical reviews and certain other noncommercial
// uses permitted by copyright law.
// 
// 
// Copyright 2014, Sensor Network, Inc. All rights reserved.
// $Id: $

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Logging;
using ModBus;
using Model;

#if qSimEnabled
using DsiSim;
#endif


namespace DsiApi
{
   using ScanNetworkCallback = Action<ADsiInfo, List<AProbe>>;

   // Callback returns modifed ASetup which fired during the measurement.
   // !!!:Currently always the first setup.
   using MeasurementCallback = Action<AMeasurementParam, AProbe, String>;


   /// <summary>
   /// ADsiNetwork represents an interface to the Digital Sensor Interface hardware. 
   /// </summary>
   /// Communication with the device is handled with the IModBusDevice and IPacketHandler interfaces,
   /// the concrete implementation of which determines the actual results of using this class.
   /// See AModBusSim, ADsiNetworkSim
   public class ADsiNetwork
   {
      /// <summary>
      /// Initialize the DSI to talk to a network of physical DSI or a single one for commissioning
      /// </summary>
      public ADsiNetwork(string portName, int baudRate = 115200, int timeout = 1000, int retries = 3, Action<byte[]> receiveCallback = null,
         Action<byte[]> sendCallback = null, string initialSettingsXmlPath = null)
      : this(new AModBusNetwork(new ASerialPacketHandler(portName, baudRate, timeout, retries, receiveCallback, sendCallback)))
      {
         if (null != initialSettingsXmlPath)
         {
            this.InitializeNetworkWithSettings(initialSettingsXmlPath);
         }
      }


#if qSimEnabled

      /// <summary>
      /// Initialize the network from a set of DSI simulation (*.dsi) files at simPath. 
      /// See ADsiSim.CreateSimFiles(...) for creating the files.
      /// </summary>
      /// <param name="simPath">The directory to hold the files created</param>
      /// <param name="latencyMs">A delay to apply after reading a result packet to simulate device response times</param>
      /// <param name="createDsis">If > 0, makes new DSIs from address 1 to createDsis.</param> 
      /// <param name="receiveCallback">A hook to get the bytes received.</param>
      /// <param name="sendCallback">A hook to get the bytes about to be sent.</param>
      /// <param name="initialSettingsXmlPath">Settings to use to populate the XML file. The DSI address is ignored,
      /// and instead the index of the file is used.</param>
      public ADsiNetwork(UInt16 createDsis, string simPath, int latencyMs,
         Action<byte[]> receiveCallback = null,
         Action<byte[]> sendCallback = null, string initialSettingsXmlPath = null)
         : this(new AModBusNetwork(new ASimPacketHandler(simPath, latencyMs)))
      {
         if (createDsis > 0)
         {
            ADsiSim.CreateSimFiles(simPath, createDsis, reset: true);
         }

         if (null != initialSettingsXmlPath)
         {
            this.InitializeNetworkWithSettings(initialSettingsXmlPath);
         }
      }

#endif

      /// <summary>
      /// Prefer above constructors unless the modBusDevice is not one of those used
      /// above.
      ///
      /// All other constructors should call this one.
      /// </summary>
      /// <param name="modBus">The ModBus implementation</param>
      public ADsiNetwork(IModBusDevice modBus)
      {
         fAddress = (byte)Address.kBroadcast; // illegal address for a device
         fModBus = modBus;
         fUpsampler = new AAscanUpsampler(kAscanUpsampleScale, ADsiInfo.kSampleRate);
      }


      public async Task<Tuple<List<byte>, UInt16>> ScanNetworkASync(bool stopIfFactory, ScanNetworkCallback callback = default(ScanNetworkCallback))
      {
         return await Task<Tuple<List<byte>, UInt16>>.Run(() =>
          {
             return this.ScanNetwork(stopIfFactory, callback);
          });
      }

      /// <summary>
      /// Scan all modbus addresses looking for valid DSIs. Validate that number by reading
      /// the number of devices from any of the returned DSI addresses.
      /// </summary>
      /// <returns>The addresses of DSIs found, and the first device's kNumDsisInChain
      /// value which should match the list's count if the DSI was properly configured.</returns>
      /// 
      public Tuple<List<byte>, UInt16> ScanNetwork(bool stopIfFactory, ScanNetworkCallback callback = null)
      {
         var validAddresses = new List<byte>();

         UInt16 numDsisInChain = 0;
         byte address = kFactoryId;
         if (this.ReadModBusId(address, searchIfNoReply: false) == address)
         {
            Console.WriteLine("Adding factory address {0}", address);
            validAddresses.Add(address);

            var marshaledDsiInfo = this.ReadDsiInfo(address);
            if (null == marshaledDsiInfo)
            {
               ALog.Error("ScanNetwork", "Could not read DSI information from modbus id {0}", address);
            }
            else
            {
               // Don't validate the factory address.
               var dsiInfo = new ADsiInfo(marshaledDsiInfo);
               var probes = this.ReadProbesAndSetups(address, dsiInfo.probeCount);
               this.ValidateProbes(probes, requireGates: false);
               if (null != callback)
               {
                  callback(dsiInfo, probes);
               }
            }

            numDsisInChain += 1;

            if (stopIfFactory)
            {
               // stop if factory found
               return new Tuple<List<byte>, UInt16>(validAddresses, numDsisInChain);
            }
         }

         // Scan for the fist address which should actually be kFirstAddress in a valid
         // configuration.
         address = (byte)Address.kFirstAddress;
         byte end = (byte)Address.kMaxAddress;
         while (address <= end)
         {
            UInt16 readId = this.ReadModBusId(address, searchIfNoReply: false);
            if (readId == address)
            {
               break;
            }
            address += 1;
         }

         int dsiCountdown = 0;
         if (address <= end)
         {
            // Get the first device's dsiCount.
            UInt16[] result = fModBus.ReadUserRegisters(address,
             (UInt16)AMemoryLayout.UserRegisters.kNumDsisInChain, 0x0001);
            dsiCountdown += (UInt16)((null != result) ? result[0] : (UInt16)0);
         }
         numDsisInChain += (ushort)dsiCountdown;

#if qTime
         TimeSpan timeSum = new TimeSpan();
#endif

         while (0 < dsiCountdown--)
         {
#if qTime
            Stopwatch timer = Stopwatch.StartNew();
#endif

            //Console.WriteLine("Reached {0}", address);
            var marshaledDsiInfo = this.ReadDsiInfo(address);
            if (null == marshaledDsiInfo)
            {
               ALog.Error("ScanNetwork", "Could not read DSI information from modbus id {0}", address);
               address += 1;
               continue;
            }

            if (marshaledDsiInfo.modbusAddress == address)
            {
               //Console.WriteLine("Adding valid address {0}", address);
               validAddresses.Add(address);

               var dsiInfo = new ADsiInfo(marshaledDsiInfo);
               var probes = this.ReadProbesAndSetups(address, dsiInfo.probeCount);
               this.ValidateProbes(probes, requireGates: false);
               if (null != callback)
               {
                  callback(dsiInfo, probes);
               }
            }
            else
            {
               ALog.Warning("ScanNetwork", "DSIInfo and ReadModBusId returned different id values.");
            }

            address += 1;

#if qTime
            timer.Stop(); 
            TimeSpan timespan = timer.Elapsed;
            timeSum += timespan;

            Console.WriteLine(String.Format("Address scan {0:00}:{1:00}:{2:00}", timespan.Minutes, 
             timespan.Seconds, timespan.Milliseconds));
#endif
         }

#if qTime
         TimeSpan timeAvg = new TimeSpan(timeSum.Ticks / (long)(address - Address.kFirstAddress));
         Console.WriteLine(String.Format("Avg Address scan {0:00}:{1:00}:{2:00}", timeAvg.Minutes, 
          timeAvg.Seconds, timeAvg.Milliseconds));
#endif

         return new Tuple<List<byte>, UInt16>(validAddresses, numDsisInChain);
      }

      /// <summary>
      /// Check the probes/setups/gates for error conditions.  Throw DsiStateException if
      /// any are found.
      /// </summary>
      /// <param name="probes"></param>
      private void ValidateProbes(IList<AProbe> probes, bool requireGates = true)
      {
         if (null == probes || 0 == probes.Count)
         {
            throw new DsiStateException(DsiApi.ApiResources.ErrorProbeListEmpty);
         }
         foreach (AProbe probe in probes)
         {
            if (null == probe.setups || 0 == probe.setups.Length)
            {
               throw new DsiStateException(DsiApi.ApiResources.ErrorSetupListEmpty);
            }
            if (requireGates)
            {
               foreach (ASetup setup in probe.setups)
               {
                  if (null == setup.gates || 0 == setup.gates.Length)
                  {
                     throw new DsiStateException(DsiApi.ApiResources.ErrorGateListEmpty);
                  }
               }
            }
         }
      }

      /// <summary>
      /// Note that company contains the site name. collectionPoint contains a coordinate coord.
      /// </summary>
      /// <param name="netAddress">The DSI to read</param>
      /// <param name="company">On output, the DSI's configured company information</param>
      /// <param name="site">On output, the DSI's configured collection point information</param>
      public void GetSiteInfo(ANanoSense ns)
      {
         byte addr = (byte)ns.Dsi.modbusAddress;
         ns.company = this.ReadCompany(addr);
         ns.site = this.ReadSite(addr);
         ns.plant = this.ReadPlant(addr);
         ns.asset = this.ReadAsset(addr);
         ns.collectionPoint = this.ReadCollectionPoint(addr);
      }


      /// <summary>
      /// Asynchronously fire shots for all DSIs,probes defined in measurementParams.
      /// </summary>
      /// <param name="measurementParams">Defines which DSI and which of its probes are fired.</param>
      /// <param name="timestamp">The time to assign to all results. If null, DateTime.UtcNow is used. </param>
      /// <param name="callback">Called with results of each shot fired.</param>
      /// <returns>The task representing the promised result.</returns>
      public async Task PerformMeasurementsAsync(IList<AMeasurementParam> measurementParams,
       DateTime timestamp, MeasurementCallback callback = default(MeasurementCallback))
      {
         try
         {
            await Task.Run(() =>
            {
               try
               {
                  this.PerformMeasurements(measurementParams, timestamp, callback);
               }
               catch (Exception)
               {
                  throw;
               }
            }
            );

         }
         catch (Exception)
         {
            throw;
         }
      }


      /// <summary>
      /// Synchronously fire shots for all DSIs,probes defined in measurementParams.
      /// </summary>
      /// <param name="measurementParams">Defines which DSI and which of its probes are fired.</param>
      /// <param name="timestamp">The time to assign to all results. If null, DateTime.UtcNow is used. </param>
      /// <param name="callback">Called with results of each shot fired.</param>
      public void PerformMeasurements(IList<AMeasurementParam> measurementParams, DateTime timestamp,
       MeasurementCallback callback = default(MeasurementCallback))
      {
         /*if (null == timestamp)
         {
            timestamp = DateTime.UtcNow;
         }*/

         string isoTimestamp = timestamp.ToIsoTimestamp();

#if qTime

         var sumTime = new TimeSpan();
         var callbackSumTime = new TimeSpan();
         int shots = 0;
         if (null == sTimes) { sTimes = new long[32 * 16]; }
         if (null == sTimes2) { sTimes2 = new long[32 * 16]; }

#endif

         foreach (AMeasurementParam param in measurementParams)
         {
            foreach (AProbe probe in param.fProbes)
            {
               //WARNING: Currently UI doesn't have setup concept. Always fire first one until it does.
               UInt16 numSetups = 1;// probe.numSetups;
               for (UInt16 setupIndex = 0; setupIndex < numSetups; ++setupIndex)
               {
#if qTime

                  Stopwatch timer = Stopwatch.StartNew();

#endif

                  String errorMessage = null;
                  ASetup setup = this.FireShot(param.fDsiAddress, (byte)(probe.num - 1),
                   (byte)setupIndex, isoTimestamp, out errorMessage);
                  probe.setups[0] = setup;

                  //this.ErrorMessage = errorMessage;
#if qTime
                  // Don't time the callback (or time separately)
             
                  timer.Stop();
                  TimeSpan timespan = timer.Elapsed;
                  sumTime += timespan;
                  sTimes[sShot] = timespan.Ticks;
                  //Console.WriteLine(String.Format("Fire shot {0:00}:{1:00}:{2:00}", timespan.Minutes,
                  // timespan.Seconds, timespan.Milliseconds));

#endif

                  if (null != callback)
                  {
#if qTime

                     timer = Stopwatch.StartNew();

#endif

                     callback(param, probe, errorMessage);

#if qTime
                     TimeSpan timespan2 = timer.Elapsed;
                     callbackSumTime += timespan2;
                     sTimes2[sShot] = timespan2.Ticks;
                    // Console.WriteLine(String.Format("Shot callback {0:00}:{1:00}:{2:00}",
                    //  timespan2.Minutes, timespan2.Seconds, timespan2.Milliseconds));

#endif
                  }

#if qTime

                  ++shots;
                  ++sShot;

#endif
               }
            }
            this.ValidateProbes(param.fProbes);
         }

#if qTime

         Console.WriteLine("{0} shots", shots);
         Console.WriteLine(String.Format("Total shot time {0:00}:{1:00}:{2:00}", sumTime.Minutes,
                  sumTime.Seconds, sumTime.Milliseconds));
         this.ReportTimes(sTimes, sShot, "shot");
        
         Console.WriteLine(String.Format("Total callback time {0:00}:{1:00}:{2:00}", callbackSumTime.Minutes,
                  callbackSumTime.Seconds, callbackSumTime.Milliseconds));
         this.ReportTimes(sTimes2, sShot, "callback");

#endif

      }


      /// <summary>
      /// Asynchronously reads all logged shot results from the DSI specified. 
      /// Throws a DsiStateException if the data logger mode cannot be enabled after reading
      /// the last shot.
      /// </summary>
      /// <param name="param">Address of DSI and list of probes to read. (Only the probe count is used.)</param>
      /// <param callback="callback">Called for each read of a shot result. The probe or the errorMessage may be null.</param>
      public async Task ReadDataLoggerShotsAsync(AMeasurementParam measurementParam,
       MeasurementCallback callback = default(MeasurementCallback))
      {
         try
         {
            await Task.Run(() =>
            {
               try
               {
                  this.ReadDataLoggerShots(measurementParam, callback);
               }
               catch (Exception)
               {
                  throw;
               }
            }
            );

         }
         catch (Exception)
         {
            throw;
         }
      }

      /// <summary>
      /// Reads all logged shot results from the DSI specified. Throws an exception if the data logger mode cannot
      /// be enabled on finishing reading the shots.
      /// </summary>
      /// <param name="param">Address of DSI and list of probes to read. (Only the probe count is used.)</param>
      /// <param callback="callback">Called for each read of a shot result. The probe or the errorMessage may be null. 
      /// If probe is null, this indicates the reading process has encountered an error and will be stopped.</param>
      /// <param name="errorMessage">On failure, contains a description of the failing command, null otherwise.</param>
      public void ReadDataLoggerShots(AMeasurementParam param, MeasurementCallback callback = default(MeasurementCallback))
      {
         byte netAddress = param.fDsiAddress;

         string errorMessage = null;

         int shotCountTotal = 0;

         var readCount = this.ReadDataLoggerNumShots(netAddress);
         if (readCount != 0xFFFF)
         {
            shotCountTotal = readCount;
         }
         else
         {
            errorMessage = ApiResources.ErrorReadLoggedShotsReadShotCount;
         }

         if (null != errorMessage)
         {
            if (null != callback)
            {
               callback(param, null, errorMessage);
            }
            return;                                             // EARLY EXIT
         }

         int probeCount = param.fProbes.Length;
         int shotsPerProbe = (int)Math.Ceiling((float)shotCountTotal / (float)param.fProbes.Length);
         int numBlankShots = (probeCount - shotCountTotal % probeCount) % probeCount;
         for (int shotIndex = 0; shotIndex < shotsPerProbe; ++shotIndex)
         {
            for (UInt16 probeIndex = 0; probeIndex < probeCount; ++probeIndex)
            {
               AProbe probe;
               // if we don't have a multiple of probeCount for number of shots
               // and its the first shot, then pad with blank probes
               if (0 == shotIndex && numBlankShots > 0)
               {
                  probe = new AProbe();
                  probe.num = probeIndex + 1;
                  probe.numSetups = 1;
                  probe.setups = new ASetup[1];
                  probe.setups[0] = new ASetup();
                  --numBlankShots;
               }
               else
               {
                  try
                  {
                     probe = this.ReadNextLoggedShot(netAddress, shotIndex, probeIndex, out errorMessage);
                  }
                  catch (Exception e)
                  {
                     callback?.Invoke(param, null, e.Message);
                     throw;
                  }
               }
               callback?.Invoke(param, probe, errorMessage);
            }
         }

         if (null != errorMessage)
         {
            throw new DsiStateException(errorMessage);
         }
      }

      /// <summary>
      /// Assumes the DSI has disabled data logging. 
      /// 
      /// Reads the next logged shot. 
      /// 
      /// shotIndex and probeIndex are only for error message construction. The DSI keeps 
      /// track of the next shot internally. 
      /// 
      /// After each shot is read, the DSI decreases the number of shots that it stores. 
      /// See ReadDataLoggerNumShots().
      /// </summary>
      /// <returns>A probe with the logged ascan in setup[0]. The probes information is read from the DSI and includes a timestamp.</returns>
      AProbe ReadNextLoggedShot(byte netAddress, int shotIndex, UInt16 probeIndex,
       out string errorMessage)
      {
         errorMessage = null;

         ALog.Info("ReadDataLoggerShots", "Preparing to read shot {0} of probe {1}.",
          shotIndex, probeIndex);

         if (!this.PrepareNextDataLoggerShot(netAddress))
         {
            errorMessage = String.Format(ApiResources.ErrorReadLoggedShotsPrepareNextShotSetBit,
             shotIndex, probeIndex);
            return null;
         }

         if (!this.PollDataReadyBit(netAddress, timeout: 200, bitSet: true))
         {
            ALog.Error("ReadNextLoggedShot", ApiResources.ErrorFireShotDataReadyBitTimeout);
         }

         ALog.Info("ReadNextLoggedShot", "Reading next logged shot result.");

         var readProbe = ReadProbe(netAddress, probeIndex);
         if (null != readProbe)
         {
            // I don't recall why we read this first, but I'm going to keep it that way.
            Int16[] ascanData = this.ReadAscanData(netAddress, probeIndex);
            if (null == readProbe.setups)
            {
               readProbe.setups = new ASetup[1];
            }

            readProbe.setups[0] = ReadSetup(netAddress, probeIndex, 0);
            readProbe.setups[0].ascanData = ascanData;

            var gates = ReadGates(netAddress, probeIndex, 0);
            readProbe.setups[0].gates = gates;
         }

         ALog.Info("ReadNextLoggedShot", "Read logged shot result.");

         return readProbe;
      }

      /// <summary>
      /// When data logging is enabled, this causes "the next save shot to be loaded
      ///  into RAM for reading via Modbus commands. See EnableDataLogger.
      /// </summary>
      public bool PrepareNextDataLoggerShot(byte netAddress)
      {
         return fModBus.WriteSingleCoil(netAddress, AMemoryLayout.kReadShotCoilAddress,
          value: 0xFF00) == AMemoryLayout.kReadShotCoilAddress;
      }


      public AAsset ReadAsset(byte netAddress)
      {
         AAsset asset = null;
         int maxRead = AReadUserRegistersCommand.MaxPayload / sizeof(UInt16);
         UInt16 initialAddress = (UInt16)AMemoryLayout.UserRegisters.kAssetOffset;

         this.ReadObject(netAddress, initialAddress, out asset, maxRead,
          (UInt16 subAddr, UInt16 quantity) =>
          {
             return fModBus.ReadUserRegisters(netAddress, subAddr, quantity);
          });

         return asset;
      }


      public ACollectionPoint ReadCollectionPoint(byte netAddress)
      {
         ACollectionPoint collectionPoint = null;
         int maxRead = AReadUserRegistersCommand.MaxPayload / sizeof(UInt16);
         UInt16 initialAddress = (UInt16)AMemoryLayout.UserRegisters.kCollectionPointOffset;

         this.ReadObject(netAddress, initialAddress, out collectionPoint, maxRead,
          (UInt16 subAddr, UInt16 quantity) =>
          {
             return fModBus.ReadUserRegisters(netAddress, subAddr, quantity);
          });

         return collectionPoint;
      }


      public ACompany ReadCompany(byte netAddress)
      {
         ACompany company = null;
         int maxRead = AReadUserRegistersCommand.MaxPayload / sizeof(UInt16);
         UInt16 initialAddress = (UInt16)AMemoryLayout.UserRegisters.kCompanyOffset;

         this.ReadObject(netAddress, initialAddress, out company, maxRead,
          (UInt16 subAddr, UInt16 quantity) =>
          {
             return fModBus.ReadUserRegisters(netAddress, subAddr, quantity);
          });

         return company;
      }


      public AMarshaledDsiInfo ReadDsiInfo(byte netAddress)
      {
         AMarshaledDsiInfo dsiInfo = null;
         int maxRead = AReadUserRegistersCommand.MaxPayload / sizeof(UInt16);

         UInt16 initialAddress = (UInt16)AMemoryLayout.UserRegisters.kDsiInfoOffset;

         this.ReadObject(netAddress, initialAddress, out dsiInfo, maxRead,
          (UInt16 subAddr, UInt16 quantity) =>
          {
             return fModBus.ReadUserRegisters(netAddress, subAddr, quantity);
          });

         return dsiInfo;
      }


      public AGate ReadGate(byte netAddress, UInt16 probeIndex, UInt16 setupIndex, UInt16 gateIndex)
      {
         AMarshaledGate marshaledGate = null;
         int maxRead = AReadHoldingRegistersCommand.MaxPayload / sizeof(UInt16);
         UInt16 initialAddress = AMemoryLayout.GateAddress(probeIndex, setupIndex, gateIndex);
         this.ReadObject(netAddress, initialAddress, out marshaledGate, maxRead,
          (UInt16 subAddr, UInt16 quantity) =>
          {
             return fModBus.ReadHoldingRegisters(netAddress, subAddr, quantity);
          });

         AGate gate = null;
         if (null != marshaledGate)
         {
            gate = new AGate(marshaledGate);
            gate.num = gateIndex + 1;
         }

         return gate;
      }


      public AGate[] ReadGates(byte netAddress, UInt16 probeIndex, UInt16 setupIndex)
      {
         var gates = new AGate[AMemoryLayout.kProbeSetupNumGates];
         for (UInt16 g = 0; g < AMemoryLayout.kProbeSetupNumGates; ++g)
         {
            gates[g] = this.ReadGate(netAddress, probeIndex, setupIndex, g);
         }
         return gates;
      }


      public APlant ReadPlant(byte netAddress)
      {
         APlant plant = null;
         int maxRead = AReadUserRegistersCommand.MaxPayload / sizeof(UInt16);
         UInt16 initialAddress = (UInt16)AMemoryLayout.UserRegisters.kPlantOffset;

         this.ReadObject(netAddress, initialAddress, out plant, maxRead,
          (UInt16 subAddr, UInt16 quantity) =>
          {
             return fModBus.ReadUserRegisters(netAddress, subAddr, quantity);
          });

         return plant;
      }

      /// <summary>
      /// Read a given probe's information from the DSI. 
      /// It does not allocate the setups array and does not read in arrays.
      /// </summary>
      /// <param name="netAddress">The DSI to from which to read the probe.</param>
      /// <param name="probeIndex">The index of the probe on the DSI.</param>
      /// <returns></returns>
      public AProbe ReadProbe(byte netAddress, UInt16 probeIndex)
      {
         AMarshaledProbe probe = null;

         int maxRead = AReadHoldingRegistersCommand.MaxPayload / sizeof(UInt16);
         UInt16 initialAddress = AMemoryLayout.ProbeAddress(probeIndex);
         this.ReadObject(netAddress, initialAddress, out probe, maxRead,
          (UInt16 subAddr, UInt16 quantity) =>
          {
             return fModBus.ReadHoldingRegisters(netAddress, subAddr, quantity);
          });

         AProbe probeSetups = null;
         if (null != probe)
         {
            probeSetups = new AProbe(probe, probeIndex);
         }

         return probeSetups;
      }


      public List<AProbe> ReadProbesAndSetups(byte netAddress, UInt16 probeCount)
      {
         var probes = new List<AProbe>();
         for (UInt16 p = 0; p < probeCount; ++p)
         {
            AMarshaledProbe probe = this.ReadProbe(netAddress, p);
            AProbe probeAndSetups = new AProbe(probe, p);
            probes.Add(probeAndSetups);

            int setupCount = probe.numSetups;

            for (UInt16 s = 0; s < setupCount; ++s)
            {
               ASetup setup = this.ReadSetup(netAddress, p, s);
               probeAndSetups.setups[s] = setup;
            }
         }
         return probes;
      }


      public ASetup ReadSetup(byte netAddress, UInt16 probeIndex, UInt16 setupIndex)
      {
         AMarshaledSetup setup = null;
         int maxRead = AReadHoldingRegistersCommand.MaxPayload / sizeof(UInt16);
         UInt16 initialAddress = AMemoryLayout.ProbeSetupAddress(probeIndex, setupIndex);
         this.ReadObject(netAddress, initialAddress, out setup, maxRead,
          (UInt16 subAddr, UInt16 quantity) =>
          {
             return fModBus.ReadHoldingRegisters(netAddress, subAddr, quantity);
          });

         ASetup setupGates = null;
         if (null != setup)
         {
            setupGates = new ASetup(setup, setupIndex);
         }

         // The zero crossings were tacked on to the memory block of the DSI after the gates.
         // We have to cherry pick these values.
         AMarshaledZeroCrossings crossings = null;
         UInt16 crossingAddress = (UInt16)(initialAddress + AMemoryLayout.kProbeSetupCrossingStartOffset);
         this.ReadObject(netAddress, crossingAddress, out crossings, maxRead,
          (UInt16 subAddr, UInt16 quantity) =>
          {
             return fModBus.ReadHoldingRegisters(netAddress, subAddr, quantity);
          });

         if (null == setupGates.zeroCrossings)
         {
            setupGates.zeroCrossings = new double[AMemoryLayout.kProbeSetupNumGates];
         }
         crossings.CopyTo(setupGates.zeroCrossings);

         return setupGates;
      }


      public ASite ReadSite(byte netAddress)
      {
         ASite site = null;
         int maxRead = AReadUserRegistersCommand.MaxPayload / sizeof(UInt16);
         UInt16 initialAddress = (UInt16)AMemoryLayout.UserRegisters.kSiteOffset;

         this.ReadObject(netAddress, initialAddress, out site, maxRead,
          (UInt16 subAddr, UInt16 quantity) =>
          {
             return fModBus.ReadUserRegisters(netAddress, subAddr, quantity);
          });

         return site;
      }


      public bool WriteAsset(byte netAddress, AAsset asset)
      {
         if (null == asset)
         {
            return true;
         }

         UInt16 initialAddress = (UInt16)AMemoryLayout.UserRegisters.kAssetOffset;

         return this.WriteObject(netAddress, initialAddress, asset,
          (UInt16[] subRegisters, UInt16 subAddress) =>
          {
             Tuple<UInt16, UInt16> result = fModBus.WriteUserRegisters(netAddress, subAddress,
              (UInt16)subRegisters.Length, subRegisters);
             return (result != null) && (result.Item1 == subAddress);
          });
      }


      public bool WriteCollectionPoint(byte netAddress, ACollectionPoint collectionPoint)
      {
         UInt16 initialAddress = (UInt16)AMemoryLayout.UserRegisters.kCollectionPointOffset;

         return this.WriteObject(netAddress, initialAddress, collectionPoint,
          (UInt16[] subRegisters, UInt16 subAddress) =>
          {
             Tuple<UInt16, UInt16> result = fModBus.WriteUserRegisters(netAddress, subAddress,
              (UInt16)subRegisters.Length, subRegisters);
             return (result != null) && (result.Item1 == subAddress);
          });
      }


      public bool WriteCompany(byte netAddress, ACompany company)
      {
         UInt16 initialAddress = (UInt16)AMemoryLayout.UserRegisters.kCompanyOffset;

         return this.WriteObject(netAddress, initialAddress, company,
          (UInt16[] subRegisters, UInt16 subAddress) =>
          {
             Tuple<UInt16, UInt16> result = fModBus.WriteUserRegisters(netAddress, subAddress,
              (UInt16)subRegisters.Length, subRegisters);
             return (result != null) && (result.Item1 == subAddress);
          });
      }


      public bool WriteDsiCount(byte netAddress, ushort count)
      {
         const UInt16 kQuantity = 0x0001;
         const UInt16 kAddress = (UInt16)AMemoryLayout.UserRegisters.kNumDsisInChain;
         UInt16[] values = { count };

         Tuple<UInt16, UInt16> result = fModBus.WriteUserRegisters(netAddress, kAddress,
          kQuantity, values);

         return (null != result && kAddress == result.Item1 && result.Item2 == 0x0001);
      }


      public bool WriteDsiInfo(byte netAddress, ADsiInfo dsiAndProbes)
      {
         AMarshaledDsiInfo dsiInfo = new AMarshaledDsiInfo(dsiAndProbes);
         return this.WriteDsiInfo(netAddress, dsiInfo);
      }

      public bool WriteDsiInfo(byte netAddress, AMarshaledDsiInfo dsiInfo)
      {
         UInt16 initialAddress = (UInt16)AMemoryLayout.UserRegisters.kDsiInfoOffset;
         return this.WriteObject(netAddress, initialAddress, dsiInfo,
          (UInt16[] subRegisters, UInt16 subAddress) =>
          {
             Tuple<UInt16, UInt16> result = fModBus.WriteUserRegisters(netAddress, subAddress,
              (UInt16)subRegisters.Length, subRegisters);
             return (result != null) && (result.Item1 == subAddress);
          });
      }


      public bool WriteGate(byte netAddress, UInt16 probeIndex, UInt16 setupIndex,
         UInt16 gateIndex, AGate gate)
      {
         AMarshaledGate marshaledGate = new AMarshaledGate(gate);
         return this.WriteGate(netAddress, probeIndex, setupIndex, gateIndex, marshaledGate);
      }


      public bool WriteGate(byte netAddress, UInt16 probeIndex, UInt16 setupIndex,
         UInt16 gateIndex, AMarshaledGate gate)
      {
         UInt16 initialAddress = AMemoryLayout.GateAddress(probeIndex, setupIndex, gateIndex);
         return this.WriteObject(netAddress, initialAddress, gate,
          (UInt16[] subRegisters, UInt16 subAddress) =>
          {
             Tuple<UInt16, UInt16> result = fModBus.WriteMultipleRegisters(netAddress, subAddress,
              (UInt16)subRegisters.Length, subRegisters);
             return (result != null) && (result.Item1 == subAddress);
          });
      }


      public bool WritePlant(byte netAddress, APlant plant)
      {
         if (null == plant)
         {
            return true;
         }

         UInt16 initialAddress = (UInt16)AMemoryLayout.UserRegisters.kPlantOffset;

         return this.WriteObject(netAddress, initialAddress, plant,
          (UInt16[] subRegisters, UInt16 subAddress) =>
          {
             Tuple<UInt16, UInt16> result = fModBus.WriteUserRegisters(netAddress, subAddress,
              (UInt16)subRegisters.Length, subRegisters);
             return (result != null) && (result.Item1 == subAddress);
          });
      }


      public bool WriteProbe(byte netAddress, UInt16 probeIndex, AProbe probeAndSetups)
      {
         AMarshaledProbe probe = new AMarshaledProbe(probeAndSetups);
         return this.WriteProbe(netAddress, probeIndex, probe);
      }

      public bool WriteProbe(byte netAddress, UInt16 probeIndex, AMarshaledProbe probe)
      {
         UInt16 initialAddress = AMemoryLayout.ProbeAddress(probeIndex);
         return this.WriteObject(netAddress, initialAddress, probe,
          (UInt16[] subRegisters, UInt16 subAddress) =>
          {
             Tuple<UInt16, UInt16> result = fModBus.WriteMultipleRegisters(netAddress, subAddress,
              (UInt16)subRegisters.Length, subRegisters);
             return (result != null) && (result.Item1 == subAddress);
          });
      }


      public bool WriteSetup(byte netAddress, UInt16 probeIndex, UInt16 setupIndex, ASetup setupAndGates)
      {
         AMarshaledSetup setup = new AMarshaledSetup(setupAndGates);
         bool success = this.WriteSetup(netAddress, probeIndex, setupIndex, setup);

         if (!success)
         {
            return false;
         }

         if (null != setupAndGates.zeroCrossings)
         {
            var crossings = new AMarshaledZeroCrossings(setupAndGates.zeroCrossings);

            UInt16 initialAddress = AMemoryLayout.ProbeSetupAddress(probeIndex, setupIndex);
            UInt16 crossingsAddress = (UInt16)(initialAddress + AMemoryLayout.kProbeSetupCrossingStartOffset);
            success = this.WriteObject(netAddress, initialAddress, setup,
             (UInt16[] subRegisters, UInt16 subAddress) =>
             {
                Tuple<UInt16, UInt16> result = fModBus.WriteMultipleRegisters(netAddress, subAddress,
                 (UInt16)subRegisters.Length, subRegisters);
                return (result != null) && (result.Item1 == subAddress);
             });
         }

         return success;
      }


      public bool WriteSetup(byte netAddress, UInt16 probeIndex, UInt16 setupIndex, AMarshaledSetup setup)
      {
         UInt16 initialAddress = AMemoryLayout.ProbeSetupAddress(probeIndex, setupIndex);
         return this.WriteObject(netAddress, initialAddress, setup,
          (UInt16[] subRegisters, UInt16 subAddress) =>
          {
             Tuple<UInt16, UInt16> result = fModBus.WriteMultipleRegisters(netAddress, subAddress,
              (UInt16)subRegisters.Length, subRegisters);
             return (result != null) && (result.Item1 == subAddress);
          });
      }


      public bool WriteSite(byte netAddress, ASite site)
      {
         if (null == site)
         {
            return true;
         }

         UInt16 initialAddress = (UInt16)AMemoryLayout.UserRegisters.kSiteOffset;

         return this.WriteObject(netAddress, initialAddress, site,
          (UInt16[] subRegisters, UInt16 subAddress) =>
          {
             Tuple<UInt16, UInt16> result = fModBus.WriteUserRegisters(netAddress, subAddress,
              (UInt16)subRegisters.Length, subRegisters);
             return (result != null) && (result.Item1 == subAddress);
          });
      }

      public bool EnableDataLogger(byte netAddress, bool doEnable)
      {
         /* Console.WriteLine("Setting probe {0} to {1}able data logging.", probeIndex, (doEnable ? "en" : "dis"));*/
         UInt16 commandCoil = AMemoryLayout.kDisableDataLoggerCoilAddress;
         if (doEnable)
         {
            commandCoil = AMemoryLayout.kEnableDataLoggerCoilAddress;
         }
         // Write Single Coil takes either 0xFF00 (ON) or 0x0000 (OFF) data value.
         // The hardware implements on/off as two coils which both take 0xFF00 (ON)
         UInt16 result = fModBus.WriteSingleCoil(netAddress, commandCoil, value: 0xFF00);
         bool opSucceeded = (commandCoil == result);

#if false // Disable this failure condition rather than fixing firmware bad behavior, per client.
         ALog.Info("DsiNetwork", "{0} Data Logger Mode - {1}",
            doEnable ? "Enable" : "Disable", 
            opSucceeded ? "succeeded" : "failed");
#endif

         return opSucceeded;
      }

      /// <summary>
      /// Before enabling the data logger, we are supposed to set the realtime clock. These commands
      /// should always be done together when enabling. If disabling, the clock is not set.
      /// </summary>
      /// <returns>true if both commands succeed, false if either or both fail</returns>
      public bool SetRealTimeClockAndEnableDataLogger(byte netAddress, bool doEnable)
      {
         bool success = true;
         if (doEnable)
         {
            success = this.SetRealTimeClock(netAddress, DateTime.UtcNow);
            if (!success)
            {
               ALog.Warning("DsiNetwork", ApiResources.ErrorReadLoggedShotsSetRealTimeClock);
            }
         }
         ALog.Info("DsiNetwork", "Sending command to enable data logger.");
         bool enableSuccess = this.EnableDataLogger(netAddress, doEnable);
         if (!enableSuccess)
         {
#if false  // Disable this failure condition rather than fixing firmware bad behavior, per client.
            ALog.Warning("DsiNetwork", ApiResources.ErrorReadLoggedShotsEnableLogging);
#endif
         }
         return success && enableSuccess;
      }

      /// <summary>
      /// Read the state of data logger mode. For non data logger DSI models behavior is undefined.
      /// </summary>
      /// <param name="netAddress">Address of DSI to read.</param>
      /// <param name="readFailed">true if the input failed to be read, false otherwise. Check this if the method return false.</param>
      /// <returns>true if data logger mode is enabled, false if disabled or readFailed is true</returns>
      public bool ReadDataLoggerEnabled(byte netAddress, out bool readFailed)
      {
         byte[] discreteInputs = fModBus.ReadDiscreteInputs(netAddress,
          AMemoryLayout.kDataLoggerModeInputAddress, 0x0001);
         readFailed = (null == discreteInputs);
         return (!readFailed && 0x01 == discreteInputs[0]);
      }


      /// <summary>
      /// Read the number of shots compiled when in data logger mode.
      /// </summary>
      /// <returns>The number of shots or 0xFFFF if the command failed</returns>
      public UInt16 ReadDataLoggerNumShots(byte netAddress)
      {
         UInt16 numShots = 0xFFFF;

         const UInt16 kQuantity = 0x0001;
         UInt16[] registers = fModBus.ReadUserRegisters(netAddress,
          (UInt16)AMemoryLayout.UserRegisters.kDataLoggerNumShots, kQuantity);

         if (null != registers)
         {
            numShots = registers[0];
         }

         return numShots;
      }

      /// <summary>
      /// Tell the DSI to fire a given probe with the settings from the given setup, returning
      /// the probe results.
      /// </summary>
      /// <remarks>
      /// Once the shot has fired, the data ready bit is polled. If it is not set before
      /// a timeout threshold, no data is returned.
      /// </remarks>
      /// <param name="netAddress">The DSI whose probes are to be fired</param>
      /// <param name="probeIndex">Which probe to fire</param>
      /// <param name="setupIndex">Which setup to use for the shot</param>
      /// <returns>The setup with measured thickness and timestamp written, with the previous
      /// thickness and timestamp preserved in its lastThickness and lastTimestamp fields,
      /// as well as the ascan data.</returns>
      public ASetup FireShot(byte netAddress, byte probeIndex, byte setupIndex, string timestamp, out string errorMessage)
      {
         Debug.Assert(probeIndex >= 0 && AMemoryLayout.kProbeSetupNumProbes > probeIndex);
         Debug.Assert(setupIndex >= 0 && AMemoryLayout.kProbeSetupNumSetups > setupIndex);

         ASetup emptySetup = new ASetup(); // returned in error conditions
         emptySetup.lastTimestamp = timestamp;

         errorMessage = null;
         ASetup setup = null;

         try
         {
            // Remove this check for now as it doesn't work with 2nd gen boards - only with 3rd gen boards
            // that clear their data ready bit after a read
            //// Poll the data ready bit to make sure DSI is ready to receive fire commands.
            //if (!this.PollDataReadyBit(netAddress, timeout: 100, bitSet: false))
            //{
            //   errorMessage = DsiApi.ApiResources.ErrorFireShotClearDataReadyBitTimeout;
            //   return emptySetup; // EARLY EXIT
            //}


            // Set the probe to fire, the DSI description doc seems to put it in the high nibble.
            if (!this.WriteProbeToFire(netAddress, probeIndex))
            {
               errorMessage = DsiApi.ApiResources.ErrorFireShotSetProbeNumber;
               return emptySetup; // EARLY EXIT
            }

            if (!this.WriteSetupToFire(netAddress, setupIndex))
            {
               errorMessage = DsiApi.ApiResources.ErrorFireShotSetSetupNumber;
               return emptySetup; // EARLY EXIT
            }

            ALog.Info("FireShot", "Firing DSI {0}, probe {1}, setup {2}", netAddress, probeIndex, setupIndex);

            // Set the fire register, triggering the probe to fire.
            if (!this.SetFireBit(netAddress))
            {
               errorMessage = DsiApi.ApiResources.ErrorFireShotSetFireRegister;
               return emptySetup;
            }

            if (!this.PollDataReadyBit(netAddress, timeout: 200, bitSet: true))
            {
               errorMessage = DsiApi.ApiResources.ErrorFireShotDataReadyBitTimeout;
               return emptySetup;
            }

            ALog.Info("FireShot", "Reading shot results.");

            AProbe probe = this.ReadProbe(netAddress, probeIndex);
            if (null == probe)
            {
               errorMessage = DsiApi.ApiResources.ErrorFireShotReadProbeInfo;
               return emptySetup;
            }

            setup = this.ReadSetup(netAddress, probeIndex, setupIndex);
            if (null == setup)
            {
               errorMessage = DsiApi.ApiResources.ErrorFireShotReadSetupInfo;
               return emptySetup;
            }

            setup.gates = this.ReadGates(netAddress, probeIndex, setupIndex);
            Int16[] ascanData = this.ReadAscanData(netAddress, probeIndex);
            emptySetup.ascanData = ascanData; // in case we fail below, still show a graph and have something to search.
            setup.ascanData = ascanData;

            // 1477 - Model 1 DSIs have no dsiModel in firmware and do not calculate thickness on board.
            // All other models do. In either case, we still write the timestamp, and I've
            // used the existing API for writing the thickness to do that.
            AMarshaledDsiInfo dsiInfo = this.ReadDsiInfo(netAddress);
            double thickness;
            if (0 != dsiInfo.Model.Length)
            {
               thickness = Convert.ToDouble(setup.calculatedThickness);
            }
            else
            {
               thickness = this.CalculateThickness(ascanData, probe, setup);
               this.WriteThickness(netAddress, probeIndex, setupIndex, thickness.ToString(), timestamp);
            }

            setup.calculatedThickness = thickness.ToString();

            // It turns out the thickness and timestamp fields are only for the DSIs use and
            // thickness is only valid right after the shot. We are supposed to write back the
            // new shot information with the timestamp into the 'last-' registers of the DSI.
            setup.lastThickness = thickness.ToString();
            setup.lastTimestamp = timestamp;
            this.WriteLastThickness(netAddress, probeIndex, setupIndex,
             setup.lastThickness, setup.lastTimestamp);

            // Update setup object to return results.

            setup.timestamp = timestamp;
            setup.ascanData = ascanData;
         }
         catch (Exception ex)
         {
            ALog.Error("FireShot", ex);
            errorMessage = ex.Message;
            setup = emptySetup;
         }

         return setup;
      }


      public bool SetRealTimeClock(byte netAddress, DateTime time)
      {
         ALog.Info("RealtimeClock", "Setting time to {0}", time.ToLongTimeString());

         // The year is masked so only 00-99 is sent
         byte shortYear = (byte)((uint)time.Year % 100);

         return this.SetRealTimeClock(netAddress, shortYear, (byte)(time.Month - 1), (byte)(time.Day - 1),
            (byte)time.Hour, (byte)time.Minute, (byte)time.Second);
      }


      public bool SetRealTimeClock(byte netAddress, byte shortYear, byte month, byte day,
       byte hour, byte minute, byte second)
      {
         UInt16 address = AMemoryLayout.kRealTimeClockAddress;

         byte[] byteValues = { month, shortYear, hour, day, second, minute };

         int byteLength = byteValues.Length * sizeof(byte);
         int registerLength = byteLength / sizeof(UInt16);

         UInt16[] values = new UInt16[registerLength];
         Buffer.BlockCopy(byteValues, 0, values, 0, byteLength);

         Tuple<UInt16, UInt16> result = fModBus.WriteFunctionRegisters(netAddress,
          address, (UInt16)values.Length, values);

         return (null != result && result.Item1 == address && result.Item2 == (UInt16)values.Length);
      }


      /// <summary>
      /// Write the thickness and a timestamp into the probe section data for either the current value
      /// or the previous values.
      /// </summary>
      /// <param name="netAddress">The address of the dsi to be written to</param>
      /// <param name="probeIndex">The probe whose shot fired to obtain the thickness</param>
      /// <param name="setupIndex">The setup whose settings are used to fire the probe and measure thickness</param>
      /// <param name="thickness">The previous measured thickness</param>
      /// <param name="timestamp">The previous timestamp to give this measurement</param>
      /// <param name="isCurrent">True if writing the current value, false if writing the previous value.</param>
      /// <returns>True on success, false otherwise</returns>
      public bool WriteThickness(byte netAddress, UInt16 probeIndex, UInt16 setupIndex,
         string thickness, string timestamp, bool isCurrent = true)
      {
         UInt16 address = AMemoryLayout.ProbeSetupAddress(probeIndex, setupIndex);
         address += (isCurrent ?
          AMemoryLayout.kProbeSetupThicknessOffset :
          AMemoryLayout.kProbeSetupLastThicknessOffset);

         int numRegisters = AMemoryLayout.kProbeSetupThicknessQuantity +
          AMemoryLayout.kProbeSetupTimestampQuantity;
         var values = new UInt16[numRegisters];

         byte[] thicknessBytes = Encoding.UTF8.GetBytes(thickness);
         int thicknessMaxLen = AMemoryLayout.kProbeSetupThicknessQuantity * sizeof(UInt16);
         int thicknessLen = Math.Min(thicknessBytes.Length, thicknessMaxLen);
         Buffer.BlockCopy(thicknessBytes, 0, values, 0, thicknessLen);

         byte[] timestampBytes = Encoding.UTF8.GetBytes(timestamp);
         int timestampMaxLen = AMemoryLayout.kProbeSetupTimestampQuantity * sizeof(UInt16);
         int timestampLen = Math.Min(timestampBytes.Length, timestampMaxLen);
         Buffer.BlockCopy(timestampBytes, 0, values, thicknessMaxLen, timestampLen);

         Tuple<UInt16, UInt16> result = fModBus.WriteMultipleRegisters(netAddress,
          address, (UInt16)values.Length, values);

         return (result.Item1 == address && result.Item2 == (UInt16)values.Length);
      }

      /// <summary>
      /// Write the thickness and a timestamp into the probe section data.
      /// </summary>
      /// <param name="netAddress">The address of the dsi to be written to</param>
      /// <param name="probeIndex">The probe whose shot fired to obtain the thickness</param>
      /// <param name="setupIndex">The setup whose settings are used to fire the probe and measure thickness</param>
      /// <param name="thickness">The previous measured thickness</param>
      /// <param name="timestamp">The previous timestamp to give this measurement</param>
      /// <returns>True on success, false otherwise</returns>
      public bool WriteLastThickness(byte netAddress, UInt16 probeIndex, UInt16 setupIndex,
         string thickness, string timestamp)
      {
         return this.WriteThickness(netAddress, probeIndex, setupIndex, thickness, timestamp, isCurrent: false);
      }


      public Int16[] ReadAscanData(byte netAddress, UInt16 probeIndex)
      {
         var ascanData = new Int16[AMemoryLayout.kAscanSectionOffset];
         int offset = 0;
         int numRegisters = AMemoryLayout.kAscanSectionOffset;
         int address = AMemoryLayout.AscanSectionAddress(probeIndex);

         // There should be 17 sections with the final section returning 0x30 registers
         while (numRegisters > 0)
         {
            int quantity = Math.Min(numRegisters, AMemoryLayout.kAscanSubsectionNumRegisters);
            UInt16[] sectionData = fModBus.ReadInputRegisters(netAddress, (UInt16)address,
             (UInt16)quantity);
            numRegisters -= quantity;
            address += quantity;

            // Block copy produces all zeroes when converting UInt16 to Int16.
            for (int i = 0; i < sectionData.Length; ++i)
            {
               var bytes = BitConverter.GetBytes(sectionData[i]);
               ascanData[offset + i] = BitConverter.ToInt16(bytes, 0);
            }
            offset += quantity;
         }

         return ascanData;
      }

      /// <summary>
      /// Read the address of the device. The default address is 0xD4, and can
      /// be used to check connectivity on a new DSI. 
      /// </summary>
      /// <param name="netAddress"></param>
      /// <param name="searchIfNoReply">If true and the device at netAddress does not respond, search for 
      /// the device by pinging each valid address.</param>
      /// <returns>The address or 0xFFFF if no device could be reached</returns>
      public UInt16 ReadModBusId(byte netAddress, bool searchIfNoReply = true)
      {
         UInt16 id = 0xFFFF;

         const UInt16 kQuantity = 0x0001;
         UInt16[] registers = fModBus.ReadUserRegisters(netAddress,
          (UInt16)AMemoryLayout.UserRegisters.kModBusAddress, kQuantity);

         // Private function to test if we received a good address back.
         Func<bool> GoodAddress = () =>
         {
            return ((null != registers)
             && ((UInt16)Address.kBroadcast != registers[0])
             /*!!!:&& ((UInt16)Address.kMaxAddress >= registers[0])*/);
         };

         if (!GoodAddress() && searchIfNoReply)
         {
            for (netAddress = (byte)Address.kFirstAddress; netAddress <= (byte)Address.kMaxAddress; ++netAddress)
            {
               registers = fModBus.ReadUserRegisters(netAddress,
                 (UInt16)AMemoryLayout.UserRegisters.kModBusAddress, kQuantity);
               if (GoodAddress())
               {
                  break;
               }
            }
         }

         if (GoodAddress())
         {
            id = registers[0];

            // Cache in the object
            fAddress = (byte)(id & 0x00FF);
         }

         return id;
      }


      public bool WriteModBusId(byte netAddress, byte modBusAddress)
      {
         const UInt16 kQuantity = 0x0001;
         const UInt16 kAddress = (UInt16)AMemoryLayout.UserRegisters.kModBusAddress;
         UInt16[] values = { (UInt16)modBusAddress };

         Tuple<UInt16, UInt16> result = fModBus.WriteUserRegisters(netAddress, kAddress,
          kQuantity, values);

         return (null != result && kAddress == result.Item1 && result.Item2 == 0x0001);
      }


      /// <summary>
      /// Calculate the thickness based on the ascanData and setup parameters. 
      /// </summary>
      /// <param name="ascanData"></param>
      /// <param name="probe">The probe's velocity value is used in the calculation.</param>
      /// <param name="setup">As input the ascanStart and gates are used. On return, it contains the zero crossings.</param>
      /// <returns>The thickness, but also sets the setup's calculator and zeroCrossings array with the crossings giving the index used in the calculation.</returns>
      private double CalculateThickness(Int16[] ascanData, AProbe probe, ASetup setup)
      {
         //Console.WriteLine("Calculating thickness.");
         double[] upsampledData = fUpsampler.Upsample(ascanData);

         if (null == setup.fCalculator)
         {
            AThicknessCalculator.PrepareSetupForThicknessCalculation(probe, setup);
         }

         double[] upZeroCrossings = setup.fCalculator.FindZeroCrossings(upsampledData, (int)setup.ascanStart,
          kAscanUpsampleScale, setup.gates, setup.crossingDelayCompensation);

         // Scale crossings back to original data rate for storage and transmission.
         setup.zeroCrossings = Array.ConvertAll<double, double>(upZeroCrossings, c => c / kAscanUpsampleScale);

         // Prepare crossing for thickness calculator by making crossing relative to time_0 and
         // correcting the phase shift introduced in the upsampler. These need to be
         // scaled to the higher sample rate of the crossings.
         AThicknessCalculator.DetectionMode mode = AThicknessCalculator.DetectionModeForGates(setup.gates);
         double correction = 0;
         if (AThicknessCalculator.DetectionMode.kBangToFirstEcho == mode)
         {
            correction = (setup.ascanStart - setup.crossingDelayCompensation) * kAscanUpsampleScale;
         }
         double[] translatedCrossings = Array.ConvertAll(upZeroCrossings, c => c + correction);

         double thickness = setup.fCalculator.MeasureThicknessZeroCross(translatedCrossings, mode);
         return thickness;
      }


      ///
      /// <param name="timeout">The number of milleseconds to wait for the bit to be set.
      /// This is not exact as it doesn't account for the time it takes for the port read
      /// to timeout</param>
      /// <param name="bitSet">The desired value of the data ready bit: false for clear, true for set.</param>
      /// <returns>true if data ready bit was read as desired, false if timeout exceeded</returns>
      private bool PollDataReadyBit(byte netAddress, int timeout, bool bitSet = true)
      {
         const int kSleepTimeMs = 20;
         byte[] dataReadyBit = { (byte)(bitSet ? 0x00 : 0x01) };
         byte testValue = (byte)(bitSet ? 0x01 : 0x00);
         do
         {
            byte[] discreteInputs = fModBus.ReadDiscreteInputs(netAddress,
                AMemoryLayout.kAcquisitionCompleteInputAddress, 0x0001);
            dataReadyBit = discreteInputs ?? dataReadyBit;
            if (testValue == (dataReadyBit[0]))
            {
               break;
            }

            Thread.Sleep(kSleepTimeMs);
            timeout -= kSleepTimeMs;

         } while (timeout > 0);

         return (timeout > 0);
      }

      private bool SetFireBit(byte netAddress)
      {
         return (fModBus.WriteSingleCoil(netAddress, AMemoryLayout.kFireProbeCoilAddress, 0xFF00)
          == AMemoryLayout.kFireProbeCoilAddress);
      }


      private bool WriteProbeToFire(byte netAddress, UInt16 probeIndex)
      {
         //Console.WriteLine("Setting probe {0} to fire.", probeIndex);
         byte[] values = { (byte)probeIndex };
         Tuple<UInt16, UInt16> result = fModBus.WriteMultipleCoils(netAddress,
          AMemoryLayout.kFireCoilsAddress, AMemoryLayout.kFireCoilsQuantity, values);
         bool opFailed = (null == result || (AMemoryLayout.kFireCoilsAddress != result.Item1 &&
          AMemoryLayout.kFireCoilsQuantity != result.Item2));
         return !opFailed;
      }


      private bool WriteSetupToFire(byte netAddress, byte setupIndex)
      {
         //Console.WriteLine("Setting setup {0} to fire.", setupIndex);

         byte[] values = { (byte)setupIndex };
         Tuple<UInt16, UInt16> result = fModBus.WriteMultipleCoils(netAddress,
          AMemoryLayout.kSetupNumberCoilAddress, AMemoryLayout.kSetupNumberCoilsQuantity, values);

         bool opFailed = (null == result || (AMemoryLayout.kSetupNumberCoilAddress != result.Item1 &&
          AMemoryLayout.kSetupNumberCoilsQuantity != result.Item2));
         return !opFailed;
      }


      public IModBusDevice ModBus { get { return fModBus; } }

      /// <summary>
      /// DSIs should come with this ModBus address 
      /// </summary>
      public static byte kFactoryId = 212;

      public const int kAscanUpsampleScale = 8;

      private byte fAddress;
      private IModBusDevice fModBus;
      private AAscanUpsampler fUpsampler;

#if qTime

      static long[] sTimes;
      static long[] sTimes2;
      static long sShot = 0;

      private void ReportTimes(long[] times, long numTimes, String task)
      {
         double variance = 0;
         double avg = 0;
         for (int s = 0; s < numTimes; ++s)
         {
            avg += times[s];
         }
         avg /= (double)numTimes;
         var avgTime = new TimeSpan((long)avg);
         Console.WriteLine(String.Format("Avg {0} time {1:00}:{2:00}:{3:00}", task, avgTime.Minutes,
                  avgTime.Seconds, avgTime.Milliseconds));
         for (int s = 0; s < sShot; ++s)
         {
            variance += Math.Pow((double)(sTimes[s]) - avg, 2.0);
         }
         double sigma = Math.Sqrt(variance / (double)sShot);
         TimeSpan sigmaTime = new TimeSpan((long)sigma);
         Console.WriteLine(String.Format("Sigma {0} {1:00}:{2:00}:{3:00}", task, sigmaTime.Minutes,
                  sigmaTime.Seconds, sigmaTime.Milliseconds));
      }

#endif

   }


   /// <summary>
   /// Various constants used in addressing memory location on DSI devices.
   /// </summary>
   public static class AMemoryLayout
   {
      /// <returns>The given probe's address relative to start of holding registers.</returns>
      static public UInt16 ProbeAddress(UInt16 probeIndex)
      {
         Debug.Assert(probeIndex < kProbeSetupNumProbes);
         return (UInt16)(kProbeInfoRegisterAddress + probeIndex * kProbeSetupSize);
      }

      /// <returns>The given probe's setup's address relative to start of holding registers.</returns>
      static public UInt16 ProbeSetupAddress(UInt16 probeIndex, UInt16 setupIndex)
      {
         Debug.Assert(probeIndex < AMemoryLayout.kProbeSetupNumProbes);
         Debug.Assert(setupIndex < AMemoryLayout.kProbeSetupNumSetups);

         return (UInt16)(ProbeAddress(probeIndex) + kProbeSetupStartOffset +
          kProbeSetupOffset * setupIndex);
      }

      /// <returns>The given probe's setup's gate's address relative to start of holding registers.</returns>
      static public UInt16 GateAddress(UInt16 probeIndex, UInt16 setupIndex, UInt16 gateIndex)
      {
         Debug.Assert(probeIndex < kProbeSetupNumProbes);
         Debug.Assert(setupIndex < kProbeSetupNumSetups);
         Debug.Assert(gateIndex < kProbeSetupNumGates);
         return (UInt16)(ProbeSetupAddress(probeIndex, setupIndex) + kProbeSetupGateStartOffset +
          gateIndex * kProbeSetupGateSize);
      }

      /// <returns>The start of ascan sections for the given probe relative to the start of input registers.</returns>
      static public UInt16 AscanSectionAddress(UInt16 probeIndex)
      {
         Debug.Assert(probeIndex < kProbeSetupNumProbes);
         return (UInt16)(kAscanAddressStart + probeIndex * kAscanSectionOffset);
      }

      // Discrete Input address (Read 0x02)
      public const UInt16 kAcquisitionCompleteInputAddress = 0x0000;
      public const UInt16 kDataLoggerModeInputAddress = 0x0001; // 0 = OFF, 1 = ON

      // Discrete Coil addresses
      public const UInt16 kFireProbeCoilAddress = 0x0000;
      public const UInt16 kReadShotCoilAddress = 0x0001;
      public const UInt16 kDisableDataLoggerCoilAddress = 0x0002;
      public const UInt16 kEnableDataLoggerCoilAddress = 0x0003;
      public const UInt16 kFireCoilsAddress = 0x0004; // Probe number, 4-bits
      public const UInt16 kFireCoilsQuantity = 0x0004;
      public const UInt16 kSetupNumberCoilAddress = 0x0008; // Setup number, 3-bits
      public const UInt16 kSetupNumberCoilsQuantity = 0x0003;

      // Holding Register Offsets and Sizes
      // In holding registers.
      public const UInt16 kProbeInfoRegisterAddress = 0x0000;
      public const UInt16 kProbeInfoRegisterQuantity = 0x0008;

      public const UInt16 kProbeSetupStartOffset = 0x0080; // Where setup information starts relative to the start of the probe
      public const UInt16 kProbeSetupSize = 0x0800;              // probe 1 info starts at 0x0000, next is at 0x0800. 
      public const UInt16 kProbeSetupNumProbes = 16;
      public const UInt16 kProbeSetupOffset = 0x0080;       // 0x80, 0x100, 0x180, 0x200, ...
      public const UInt16 kProbeSetupNumSetups = 8;

      public const UInt16 kProbeSetupGateStartOffset = 0x00BF - kProbeSetupStartOffset; // Number of bytes after setup start that gates start.
      public const UInt16 kProbeSetupNumGates = 3;
      public const UInt16 kProbeSetupGateSize = 0x09; // registers

      public const UInt16 kProbeSetupCrossingStartOffset = 0x00DA - kProbeSetupStartOffset;
      public const UInt16 kProbeSetupNumCrossings = 3;
      public const UInt16 kProbeSetupCrossingSize = 0x0001; // registers


      // Public for testing.
      public const UInt16 kProbeSetupThicknessOffset = 0x0089 - kProbeSetupStartOffset; // offset from start of probe section
      public const UInt16 kProbeSetupLastThicknessOffset = 0x00A1 - kProbeSetupStartOffset; // offset from start of probe section
      // Current and previous/last areas have same quantities.
      public const int kProbeSetupThicknessQuantity = 8;
      public const int kProbeSetupTimestampQuantity = 16;

      // Ascan data is read from Input Registers.
      public const UInt16 kAscanAddressStart = 0x0000;
      public const UInt16 kAscanSectionOffset = 0x0800;
      public const UInt16 kAscanSubsectionNumRegisters = 0x007D;

      public const UInt32 kProbeResultMuxSwitchMask = 0xFFFFFF00;
      public const UInt32 kProbeResultNumAvgsMask = ~kProbeResultMuxSwitchMask;

      public const UInt16 kDsiModelNumRegisters = 0x0004;

      // Function Registers (Write 0x44)
      public const UInt16 kRealTimeClockAddress = 0x0000;
      public const UInt16 kRealTimeClockNumRegisters = 0x0003;

      /// <summary>
      /// Offsets in user registers for information about site and device.
      /// All values are ASCII right padded with '0' to the next offset
      /// unless noted.
      /// </summary>
      public enum UserRegisters : ushort
      {
         kCompanyOffset = 0x0000,
         /* We won't directly access these fields by offset. See ACompany
         kCompanyName = 0x0010,
         kCompanyAddressLine1 = 0x0030,
         kCompanyAddressLine2 = 0x0050,
         kCity = 0x0070,
         kState = 0x0080,
         kPostalCode = 0x0090,
         kCountry = 0x0098,
         kPhoneNumber = 0x00A8,
         kSiteOrDivision = 0x00B8,
         kPlant = 0x00D8,
         kAsset = 0x00F8,
          */
         kSiteOffset = 0x00B8,
         kPlantOffset = 0x00D8,
         kAssetOffset = 0x00F8,
         kCollectionPointOffset = 0x0118,
         /*
         kCollectionPointId = 0x0118,
         kCollectionPointDescription = 0x0138,
         kCollectionPointGpsCoordinate = 0x0178,
          */
         kDsiInfoOffset = 0x0188,
         kModBusAddress = 0x0188,                // Hex value
         /*
         kDsiSerialNumber = 0x0189,
         kDsiTagNumber = 0x0199,
         kDsiFirmwareVersionMicro = 0x01B9,
         kDsiFirmwareVersionFpga = 0x01C9,
         kDsiGpsCoordinate = 0x01D9,
          */
         kNumDsisInChain = 0x01E9,               // Hex value
         /*
         kNumProbesAttached = 0x01EA,            // Hex value
         kResultPacketVersion = 0x01EB,
         kBaudRate = 0x01FB,
         kParityAndStopBits = 0x0203,             // Bit field
         kDsiDescription = 0x0204
         */
         kDsiModel = 0x0247,                      // Hex value
         kDataLoggerNumShots = 0x2E3,
      }
   }
}
