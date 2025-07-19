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
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ModBus;
using DsiUtil;


namespace UnitTestDsiSim
{
   /// <summary>
   /// Test the checksum calculation and verification functions.
   /// </summary>
   [TestClass]
   public class CrcTests
   {

      [TestInitialize]
      public void Initialize()
      {
         byte payloadSize = 0; // Changing this will change the checksum which will cause tests to fail
         fPacket = new APacket(0x02, 0x07, payloadSize);
         fPacket.CalculateErrorChecks();
         fChecksumOffset = APacket.kAddressAndCommandSize + payloadSize;
      }


      [TestMethod]
      public void TestCrc16()
      {
         // Example from the Modbus serial line protocol doc.
         // Note that the result shown there is byte swapped.
         byte[][] input = { new byte[] { 0x02, 0x07 },
                            new byte[] { 0xFF, 0xFF, 0xFF },
                          };
         UInt16[] expected = { 0x1241, 0x4040 };

         int c = 0;
         foreach (byte[] values in input)
         {
            UInt16 crc = Checksum.Crc16(values, values.Length);
            Assert.AreEqual(expected[c++], crc);
         }
      }


      [TestMethod]
      public void TestPacketChecksum()
      {
         UInt16 crc = fPacket.ValueAtOffset(fChecksumOffset);
         Assert.AreEqual(0x4112, crc);
      }


      [TestMethod]
      public void TestChecksumValidation()
      {
         Assert.IsTrue(fPacket.ValidChecksum());

         fPacket.Data()[fChecksumOffset] = 0xFF;
         Assert.IsFalse(fPacket.ValidChecksum());
      }

      APacket fPacket;
      int fChecksumOffset;
   }
}

