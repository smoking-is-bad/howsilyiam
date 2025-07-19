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
using System.Collections.Generic;
using System.IO;
using System.Linq; // for Array.Count
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using DsiApi;
using MockData;
using ModBus;
using Model;
using DsiUtil;

namespace UnitTestDsiSim
{
   [TestClass]
   public class DsiTests
   {
      [TestInitialize]
      public void Initialize()
      {
         string root = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
         fNetworkPath = Path.Combine(root, "DsiTest");
         if (!Directory.Exists(fNetworkPath))
         {
            Directory.CreateDirectory(fNetworkPath);
         }

         fNumDsis = 12;
         fDsiNet = new ADsiNetwork(fNumDsis, fNetworkPath, 0);

         Tuple<List<byte>, UInt16> scan = fDsiNet.ScanNetwork();

         // The DSIs haven't been configured so they won't know how many
         // DSIs are in the chain and scan.Item2 will be zero.
         fDefaultCompany = new ACompany();
         fDefaultCompany.id = "01234";
         fDefaultCompany.name = "Art & Logic";
         fDefaultCompany.location = new APostalAddress
         {
            address1 = "2 Lake Boulevard",
            address2 = "Suite 1000",
            city = "Pasadena",
            state = "CA",
            postalCode = "91101",
            country = "United States"
         };
         fDefaultCompany.phone = "";

         AMarshaledDsiInfo dsiInfo = new AMarshaledDsiInfo();
         dsiInfo.dsiCount = fNumDsis;
         dsiInfo.probeCount = 1;
         AMarshaledProbe probe = new AMockData().Probe();

         foreach (byte address in scan.Item1)
         {
            fDsiNet.WriteCompany(address, fDefaultCompany);
            dsiInfo.modbusAddress = address;
            fDsiNet.WriteDsiInfo(address, dsiInfo);
            fDsiNet.WriteProbe(address, 0, probe);
         }
      }


      [TestMethod]
      public void TestScanNetwork()
      {
         // Dependent on getting the number of DSIs, so make sure it is configured.
         Tuple<List<byte>, UInt16> scan = fDsiNet.ScanNetwork();
         Assert.AreEqual(fNumDsis, scan.Item2);
         Assert.AreEqual(fNumDsis, scan.Item1.Count);
      }


      [TestMethod]
      public void TestReadModBusId()
      {
         // Depending on whether the DSI is in a factory state,
         // we may or may not get back the factory id, but it should
         // be within the range of allowed ids or that indicates
         // an error.
         int id = (int)fDsiNet.ReadModBusId(ADsiNetwork.kFactoryId);
         Assert.IsTrue(id <= (int)Address.kMaxAddress);
         Assert.IsTrue(id >= (int)Address.kFirstAddress);
      }


      [TestMethod]
      public void TestWriteModBusId()
      {
         byte id = (byte)fDsiNet.ReadModBusId(ADsiNetwork.kFactoryId);
         const byte kIdToWrite = 0x1a;
         Assert.IsTrue(fDsiNet.WriteModBusId(id, kIdToWrite));
         byte newId = (byte)fDsiNet.ReadModBusId(kIdToWrite);
         Assert.AreNotEqual(id, newId);
         Assert.AreEqual(kIdToWrite, newId);
      }


      [TestMethod]
      public void TestReadCompany()
      {
         byte netAddress = 0x08;
         ACompany readBack = fDsiNet.ReadCompany(netAddress);
         Assert.AreEqual(fDefaultCompany, readBack);
      }

      [TestMethod]
      public void TestWriteCompany()
      {
         // This test is already an exercise of WriteBasicHeader because
         // that's how the DSI's header was written in the initialization.
         byte netAddress = 0x01;
    
         bool success = fDsiNet.WriteCompany(netAddress, fDefaultCompany);
         Assert.IsTrue(success);

         ACompany readBack = fDsiNet.ReadCompany(netAddress);
         Assert.AreEqual(fDefaultCompany, readBack);
      }

      [TestMethod]
      public void TestReadWriteCollectionPoint()
      {
         var collectionPoint = new AMockData().CollectionPoint();

         byte netAddress = 0x02;
         fDsiNet.WriteCollectionPoint(netAddress, collectionPoint);
         ACollectionPoint readBack = fDsiNet.ReadCollectionPoint(netAddress);

         Assert.AreEqual(collectionPoint, readBack);
      }

      [TestMethod]
      public void TestReadWriteGate()
      {
         var gate = new AGate()
         {
            start = 100,
            width = 1000,
            threshold = -10000,
            tof = 1,
            amplitude = 1,
            mode = 2
         };

         UInt16 probe = 1;
         UInt16 setup = 7;
         byte netAddress = 0x02;
         for (UInt16 i = 0; i < 3; ++i)
         {
            fDsiNet.WriteGate(netAddress, probe, setup, i, gate);
         }

         for (UInt16 i = 0; i < 3; ++i)
         {
            AGate readBack = fDsiNet.ReadGate(netAddress, probe, setup, i);
            Assert.AreEqual(gate, readBack);
         }
      }


      [TestMethod]
      public void TestReadWriteProbe()
      {
         byte netAddress = (byte)(fNumDsis - 1);
         AMockData mock = new AMockData();
         AMarshaledProbe probe = mock.Probe();

         // Write then read, to test for overlapping writes.
         for (UInt16 i = 0; i < AMemoryLayout.kProbeSetupNumProbes; ++i)
         {
            Assert.IsTrue(fDsiNet.WriteProbe(netAddress, i, probe));
         }

         for (UInt16 i = 0; i < AMemoryLayout.kProbeSetupNumProbes; ++i)
         {
            AProbe readBack = fDsiNet.ReadProbe(netAddress, i);
            Assert.AreEqual(i + 1, readBack.num);
            Assert.AreEqual(probe, readBack);
         }
      }

      [TestMethod]
      public void TestWriteThickness()
      {
         byte netAddress = 0x08;
         double thickness = 0.0223;
         UInt16 probeNumber = 0x0E;
         UInt16 shotNumber = 0x07;
         string timestamp = DateTime.UtcNow.ToIsoTimestamp();
         Assert.IsTrue(fDsiNet.WriteThickness(netAddress, probeNumber, shotNumber,
          thickness, timestamp));

         AMarshaledSetup setup = fDsiNet.ReadSetup(netAddress, probeNumber, shotNumber);
         Assert.AreEqual(thickness.ToString(), setup.lastThickness);
         Assert.AreEqual(timestamp, setup.lastTimestamp);
      }

      [TestMethod]
      public void TestFireShot()
      {
         // This exercises firing the shot, reading the probe result and
         // reading the ascan data.

         string timestamp = DateTime.UtcNow.ToIsoTimestamp();

         byte netAddress = 0x03;

         byte probe = 0x00;
         // while (probe < 0x0F)
         {
            byte setup = 0x02;
            //while (setup < 0x08)
            {
               ASetup original = fDsiNet.ReadSetup(netAddress, probe, setup);

               ASetup results = fDsiNet.FireShot(netAddress, probe, setup,
                DateTime.UtcNow.ToIsoTimestamp());

               Assert.IsNotNull(results);
               Assert.IsNotNull(results.ascanData);

               // Test for any overwriting of setup values during shot firing.
               // This was seen with an early version.
               ASetup readBack = fDsiNet.ReadSetup(netAddress, probe, setup);
               Assert.IsTrue(results.ConstFieldsEquals(readBack as AMarshaledSetup));
               Assert.IsTrue(results.ascanData.Count() == 2048);

               Assert.AreEqual(results.lastThickness, original.calculatedThickness);
               Assert.AreEqual(results.lastTimestamp, original.timestamp);
            }
         }
      }

      [TestMethod]
      public void TestAscanReadsViaSerial()
      {
         var dsiNet = new ADsiNetwork("COM3", 115200, (byte[] buffer) =>
         {
            buffer.Print("Sending");
         });

         byte netAddress = 0x01;
         dsiNet.WriteModBusId(ADsiNetwork.kFactoryId, netAddress);

         for (UInt16 p = 0x0000; p < AMemoryLayout.kProbeSetupNumProbes; ++p)
         {
            try
            {
               Assert.AreNotEqual(null, dsiNet.ReadAscanData(netAddress, p));
            }
            catch (Exception ex)
            {
               Assert.Fail("Exception reading ascan data for probe {0}.\r\n{1}", p, ex.Message);
            }
         }
      }

      [TestMethod]
      public void TestReadInputRegisterViaSerial()
      {
#if false // Enable test when needed. At thousands of commands it is very slow (63 minutes)
         // unless the quantity is adjusted.
         var dsiNet = new ADsiNetwork("COM3");//, 115200, (byte[] buffer) =>
         /*
         {
            buffer.Print("Sending");
         });
         */

         byte netAddress = 0x01;

         UInt16 addr = 0x0000;
         UInt16 maxAddr =  (UInt16)(AMemoryLayout.AscanSectionAddress(AMemoryLayout.kProbeSetupNumProbes - 1)
          + AMemoryLayout.kAscanSubsectionNumRegisters);
         UInt16 quantity = 0x0001; // step for reads
         try
         {
            while (addr < maxAddr)
            {
               Assert.AreNotEqual(null, dsiNet.ModBus.ReadInputRegisters(netAddress, addr, quantity));
               addr += quantity;
            }
         }
         catch (Exception ex)
         {
            Assert.Fail("Input register read failed at 0x{0}:0x{1}.\r\n{2}", addr.ToString("X4"),
               quantity.ToString("X4"), ex.Message);
         }
#endif
      }


      private byte fNumDsis;
      private string fNetworkPath;
      private ADsiNetwork fDsiNet;

      ACompany fDefaultCompany;
   }
}

