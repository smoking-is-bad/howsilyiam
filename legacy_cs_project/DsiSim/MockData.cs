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

using System;

using DsiApi;
using Model;
using DsiUtil;


namespace MockData
{
   /// <summary>
   /// Factory for, sometimes random, model data.
   /// </summary>
   public class AMockData
   {
      public AMockData()
      : this(new Random())
      {
      }

      public AMockData(Random rng)
      {
         fRng = rng;
      }

      /// <summary>
      /// Generate dsi with numProbes probes and random number of setups.
      /// </summary>
      public ANanoSense NanoSense(int numProbes)
      {
         return new ANanoSense()
         {
            testUser = AIdentity.LoggedInUsername(),
            testId = Guid.NewGuid().ToString(),
            company = this.Company(),
            collectionPoint = this.CollectionPoint(),
            dsis = this.DsiAndProbes(numProbes, fRng.Next(1, 7))
         };
      }


      public ACompany Company()
      {
         return new ACompany()
         {
            id = "1",
            name = "Boalsburg Gas Company",
            location = new APostalAddress()
            {
               address1 = "1540 Martin Street",
               address2 = "",
               city = "State College",
               state = "PA",
               postalCode = "16801",
               country = "USA",
            },
            phone = "8142374859",
         };
      }

      public ACollectionPoint CollectionPoint()
      {
         return new ACollectionPoint()
         {
            description = "Main Line West",
            name = "123456",
            location = new AGpsCoordinate()
             {
                coordinates = "40.805289;-77.892777"
             }
         };
      }

      public ADsiInfo[] DsiAndProbes(int numProbes, int numSetups)
      {
         var dsiInfo = new ADsiInfo()
         {
            probes = new AProbe[numProbes]
         };

         for (int p = 0; p < numProbes; ++p)
         {
            dsiInfo.probes[p] = this.ProbeSetups(p, numSetups);
         }

         return new ADsiInfo[] { dsiInfo };
      }


      public AMarshaledDsiInfo DsiInfo(int numProbes)
      {
         var dsiInfo = new AMarshaledDsiInfo()
         {
            serialNumber = fRng.Next(1, 123456789).ToString(),
            tag = Guid.NewGuid().ToString(),
            firmware = this.FirmwareVersion(),
            location = new AGpsCoordinate()
            {
               coordinates ="40.805299;-77.892776"
            },
            dsiCount = 1,
            probeCount = (UInt16)numProbes,
            packetVersion = "0.1", //???
            baudRate = "115200",
            parityAndStopBits = (1 << 8) | 0,
         };

         return dsiInfo;
      }


      public AFirmwareVersion FirmwareVersion()
      {
         return new AFirmwareVersion()
         {
            micro = "0.1",
            fpga = "0.1a"
         };
      }


      public AMarshaledProbe Probe()
      {
         return new AMarshaledProbe()
         {
            model = "231234 oh niner",
            type = "offset",
            location = new AGpsCoordinate 
             { 
                coordinates = "40.805299;-77.892776"
             },
            nominalThickness = "1.0",
            minimumThickness = "0.5667",
            velocity = "5900.0",
            description = "Mock probe.",
            calZeroCount = 0x13999
         };
      }


      public AProbe ProbeSetups(int probeNum, int numSetups)
      {
         var probe = new AProbe(this.Probe(), probeNum);

         var setups = this.Setups(numSetups);
         probe.setups = new ASetup[numSetups];
         for (int s = 0; s < numSetups; ++s)
         {
            var setup = new ASetup(setups[s], s);
            setup.ascanData = MockData.AAscan.PresetData();
            probe.setups[s] = setup;
         }

         return probe;
      }


      public AMarshaledProbe[] Probes(int numProbes)
      {
         var probes = new AMarshaledProbe[numProbes];
         for (Int32 i = 1; i <= probes.Length; ++i)
         {
            probes[i - 1] = this.Probe();
         }
         return probes;
      }


      public AMarshaledSetup[] Setups(int numSetups)
      {
         var setups = new AMarshaledSetup[numSetups];
         for (int s = 0; s < setups.Length; ++s)
         {
            setups[s] = this.Setup(s + 1);
         }
         return setups;
      }


      public AMarshaledSetup Setup(int setupNum)
      {
         AMarshaledSetup setup = new AMarshaledSetup();
         setup.dsiTemp = this.MakeTemperature();
         setup.materialTemp = this.MakeTemperature();
         setup.status = 0;
         setup.calculatedThickness = this.MakeThickness().ToString();
         setup.timestamp = DateTime.UtcNow.ToIsoTimestamp();
         setup.lastThickness = this.MakeThickness().ToString();
         setup.lastTimestamp = this.MakeRandomDate();
         setup.pulserWidth = Convert.ToUInt16(this.MakeByte());
         setup.gain = Convert.ToUInt16(this.MakeByte());
         setup.muxSwitchSettings = 0x0080;
         setup.switchSettings = 0x040a;
         setup.ascanStart = 0;
         return setup;
      }


      public byte[] AscanData(int numSections)
      {
         const UInt16 kSectionQuantity = AMemoryLayout.kAscanSectionOffset;
         var ascanData = new byte[kSectionQuantity * numSections * sizeof(UInt16)];
         int offset = 0;
         for (int a = 0; a < numSections; ++a, offset += kSectionQuantity)
         {
            Buffer.BlockCopy(AAscan.GeneratedMockData(kSectionQuantity), 0, ascanData,
               offset * sizeof(UInt16), kSectionQuantity * sizeof(UInt16));
         }

         return ascanData;
      }


      private AMarshaledGate Gate(int gateNum, UInt16 start = 100, UInt16 width = 1000,
       Int16 threshold = -10000)
      {
         AMarshaledGate gate = new AMarshaledGate();
         gate.start = start;
         gate.width = width;
         gate.threshold = threshold;
         gate.mode = 1;
         gate.tof = 1;
         gate.amplitude = 1;
         return gate;
      }


      /// <returns>Random temperature in Celsius in range of those found on Earth.</returns>
      private string MakeTemperature()
      {
         return fRng.Next(-89, 50).ToString();
      }


      private double MakeThickness()
      {
         const double kMaxThickness = 24.0;
         return fRng.NextDouble() * kMaxThickness;
      }


      private string MakeByte(int len = 1, string format = null)
      {
         string output = "";
         while (0 < len--)
         {
            output += fRng.Next(0x00, 0xFF).ToString(format);
         }
         return output;
      }

      private string MakeRandomDate()
      {
         long tick = ((long)fRng.Next(Int32.MaxValue - 1) << 29) +
          (long)fRng.Next(Int32.MaxValue - 1);
         return (new DateTime(tick)).ToIsoTimestamp();
      }


      private Random fRng;
   }
}
