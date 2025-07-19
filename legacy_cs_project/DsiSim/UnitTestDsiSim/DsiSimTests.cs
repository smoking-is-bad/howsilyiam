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
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using DsiSim;

namespace UnitTestDsiSim
{
   [TestClass]
   public class DsiSimTests
   {
      [TestInitialize]
      public void Initialize()
      {
         string root = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
         string[] components = { root, "DsiSimTest.dsi" };
         fNetworkPath = Path.Combine(components);

         fDsiSim = new ADsiSim(fNetworkPath);
         fDsiSim.Reset();
         // Tests depend on some of these values like having enough coils and inputs.
         fDsiSim.MakeMemoryLayout(200, 200, 0x2048, 0x258, 0x7c59);
         fDsiSim.EnableProbeFiring = false;
      }

      [TestCleanup]
      public void Cleanup()
      {
         // Comment this out to manually inspect after testing.
         fDsiSim.Reset();
      }

      [TestMethod]
      public void TestCreation()
      {
         Assert.IsNotNull(fDsiSim);
      }

      [TestMethod]
      public void TestWriteSingleRegister()
      {;
         UInt16 address = 0x0000;
         UInt16 value = 0xFFFF;
         while (value > 0)
         {
            var result = fDsiSim.WriteSingleRegister(address, value);
            Assert.AreEqual(result.Item1, address);
            Assert.AreEqual(result.Item2, value);

            ++address;
            value -= 0x1111;
         }
      }

      [TestMethod]
      public void TestWriteMultipleRegisters()
      {
         UInt16 address = 0x0000;
         UInt16 quantity = 16;
         UInt16[] output = { 0x0102, 0x0304, 0x0506, 0x0708, 0x090a,
                             0x0102, 0x0304, 0x0506, 0x0708, 0x090a,
                             0x0102, 0x0304, 0x0506, 0x0708, 0x090a,
                             0x0102 };

         var result = fDsiSim.WriteMultipleRegisters(address, quantity, output);
         Assert.AreEqual(result.Item1, address);
         Assert.AreEqual(result.Item2, quantity);

         UInt16[] readBackValues = fDsiSim.ReadHoldingRegisters(address, quantity);
         int i = 0;
         foreach (UInt16 value in output)
         {
            Assert.AreEqual(value, readBackValues[i++]);
         }
      }

      [TestMethod]
      public void TestWriteUserRegisters()
      {
         UInt16 address = 0x0000;
         UInt16 quantity = 16;
         UInt16[] output = { 0x0102, 0x0304, 0x0506, 0x0708, 0x090a,
                             0x0102, 0x0304, 0x0506, 0x0708, 0x090a,
                             0x0102, 0x0304, 0x0506, 0x0708, 0x090a,
                             0x0102 };

         var result = fDsiSim.WriteUserRegisters(address, quantity, output);
         Assert.AreEqual(result.Item1, address);
         Assert.AreEqual(result.Item2, quantity);

         UInt16[] readBackValues = fDsiSim.ReadUserRegisters(address, quantity);
         int i = 0;
         foreach (UInt16 value in output)
         {
            Assert.AreEqual(value, readBackValues[i++]);
         }
      }


      [TestMethod]
      public void TestReadHoldingRegisters()
      {
         // !!! Todo: Point to file with known data instead of relying on write functionality.
        
         UInt16 quantity = 15;

         UInt16[] testOutput = { 0x7F7F, 0x7E7E, 0x7D7D, 0x7C7C,
                               0x7B7B, 0x7A7A, 0x7979, 0x7878,
                               0x7777, 0x7676, 0x7575, 0x7474,
                               0x7373, 0x7272, 0x7171 };

         fDsiSim.WriteMultipleRegisters(0x0000, quantity, testOutput);
  
         UInt16[] registerValues = fDsiSim.ReadHoldingRegisters(0x0000, quantity);
         Assert.AreEqual(registerValues.Length, quantity);

         int i = 0;
         foreach (var value in registerValues)
         {
            Assert.AreEqual(testOutput[i], value);
            i += 1;
         }
      }


      [TestMethod]
      public void TestAlignedWriteAndReadMultipleCoils()
      {
         byte[] outputValues = { 0xFF };
         UInt16 address = 0;
         for (UInt16 quantity = 1; quantity <= 8; ++quantity, address += 8)
         {
            fDsiSim.WriteMultipleCoils(address, quantity, outputValues);
         }

         var coils = fDsiSim.ReadCoils(0x0000, 8 * 8);
         byte[] expected = { 0x01, 0x03, 0x07, 0x0F, 0x1F, 0x3F, 0x7F, 0xFF };
         CollectionAssert.AreEqual(expected, coils);
      }

      [TestMethod]
      public void TestUnalignedWriteAndReadMultipleCoils()
      {
         byte[] outputValues = { 0xFF };
         UInt16 address = 0x02;
         for (UInt16 quantity = 1; quantity <= 8; ++quantity, address += 8)
         {
            fDsiSim.WriteMultipleCoils(address, quantity, outputValues);
         }

         // When writing we overflow into the ninth byte due to being offset by 2,
         // but  this will read them byte aligned by starting at the same offset
         // we started writing to.
         var coils = fDsiSim.ReadCoils(0x02, 8 * 8);
         byte[] expected = { 0x01, 0x03, 0x07, 0x0F, 0x1F, 0x3F, 0x7F, 0xFF };
         CollectionAssert.AreEqual(expected, coils);
      }


      [TestMethod]
      public void TestWriteSingleCoil()
      {
         fDsiSim.WriteSingleCoil(0x0000, 0xFF00);
         byte[] readBack = fDsiSim.ReadCoils(0x0000, 0x0001);
         Assert.AreEqual(0x01, readBack[0] & 0x01);
         fDsiSim.WriteSingleCoil(0x0000, 0x0000);
         readBack = fDsiSim.ReadCoils(0x0000, 0x0001);
         Assert.AreEqual(0x00, readBack[0] & 0x01);

         // Test writing somewhere in the middle of a byte.
         int byteOffset = 4;
         int bitOffset = 3;
         UInt16 bitAddress = (UInt16)(byteOffset * 8 + bitOffset);
         fDsiSim.WriteSingleCoil(bitAddress, 0xFF00);
         readBack = fDsiSim.ReadCoils(bitAddress, 0x0008);
         int mask = 0x01 << bitOffset;
         Assert.AreEqual(0x01, readBack[0] & 0x01);
         fDsiSim.WriteSingleCoil(bitAddress, 0x0000);
         readBack = fDsiSim.ReadCoils(bitAddress, 0x0008);
         Assert.AreEqual(0, readBack[0] & 0x01);
      }


      [TestMethod]
      public void TestReadDiscreteInputs()
      {
         // Inputs are not writable, so not much to test.
         int numBytes = 8;
         var inputs = fDsiSim.ReadDiscreteInputs(0x0000, (UInt16)(numBytes * 8));
         Assert.AreEqual(numBytes, inputs.Length);
      }


      [TestMethod]
      public void TestReadInputRegisters()
      {
         // Corresponds to second probe's ascan data.
         UInt16 numRegisters = 2048;
         var inputRegisters = fDsiSim.ReadInputRegisters(0x0800, numRegisters);
         Assert.AreEqual(numRegisters, inputRegisters.Length);
      }


      [TestMethod]
      public void TestProbeDataReady()
      {
         fDsiSim.EnableProbeFiring = true;

         // Fire the probe.
         Stopwatch sw = Stopwatch.StartNew();
         fDsiSim.WriteSingleCoil(0x0000, 0xFF00);

         const int kDelay = 50;
         UInt16 dataReadyAddress = 0x0000;
         var inputs = fDsiSim.ReadDiscreteInputs(dataReadyAddress, 0x0001);
         sw.Stop();
         if (sw.ElapsedMilliseconds < kDelay)
         {
            Assert.AreEqual(0x00, inputs[0]);

            // DsiSim waits 100ms to set the data ready bit.
            Thread.Sleep(kDelay * 2);
         }

         // Now it should be set.
         inputs = fDsiSim.ReadDiscreteInputs(dataReadyAddress, 0x0001);
         Assert.AreEqual(0x01, inputs[0]);
      }

      private ADsiSim fDsiSim;
      private string fNetworkPath;
   }
}

