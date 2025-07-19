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
using Microsoft.VisualStudio.TestTools.UnitTesting;

using DsiExtensions;
using MockData;
using Model;

namespace UnitTestDsiSim
{
   [TestClass]
   public class BitCopyTests
   {
      [TestMethod]
      public void CopySingleByteTests()
      {  
         // Cheat sheet for 0x6F
         // 76543210    <- address
         // 01101111    <- value
         byte[] src = { 0x6F };
         byte[] dst = { 0x00 };

         dst[0] = 0x00;
         ABufferExtensions.BitCopy(src, 0, dst, 0, 8);
         Assert.AreEqual(src[0], dst[0]);

         dst[0] = 0x00;
         ABufferExtensions.BitCopy(src, 0, dst, 0,  1);
         Assert.AreEqual(0x01, dst[0]);

         dst[0] = 0x00;
         ABufferExtensions.BitCopy(src, 7, dst, 0, 1);
         Assert.AreEqual(0x00, dst[0]);

         dst[0] = 0x00;
         ABufferExtensions.BitCopy(src, 1, dst, 0, 2);
         Assert.AreEqual(0x03, dst[0]);

         dst[0] = 0x00;
         ABufferExtensions.BitCopy(src, 4, dst, 0, 2);
         Assert.AreEqual(0x02, dst[0]);

         dst[0] = 0x00;
         ABufferExtensions.BitCopy(src, 6, dst, 0, 2);
         Assert.AreEqual(0x01, dst[0]);

         dst[0] = 0x00;
         ABufferExtensions.BitCopy(src, 3, dst, 0, 4);
         Assert.AreEqual(0x0D, dst[0]);

         dst[0] = 0x00;
         ABufferExtensions.BitCopy(src, 2, dst, 0, 4);
         Assert.AreEqual(0x0B, dst[0]);

         dst[0] = 0x00;
         ABufferExtensions.BitCopy(src, 5, dst, 0, 3);
         Assert.AreEqual(0x03, dst[0]);
      }

      [TestMethod]
      public void CopyFromTwoBytesTests()
      {
         // Cheat sheet
         // 0       |1       |    <- byte
         // 76543210|fedcba98|    <- address/offset
         // 01011111|01101111|    <- value
         byte[] src = { 0x5F, 0x6F };
         byte[] dst = { 0x00, 0x00 };

         // Single Byte or less from 1 of 2 bytes
         ABufferExtensions.BitCopy(src, 0x0B, dst, 0, 4); // b1101
         Assert.AreEqual(0x0D, dst[0]);

         dst[0] = 0x00;
         ABufferExtensions.BitCopy(src, 0x0C, dst, 0, 4); // b0110
         Assert.AreEqual(0x06, dst[0]);

         // Span a byte boundary.
         dst[0] = 0x00;
         ABufferExtensions.BitCopy(src, 0x06, dst, 0, 4); //b1101
         Assert.AreEqual(0x0D, dst[0]);

         dst[0] = 0x00;
         ABufferExtensions.BitCopy(src, 0x04, dst, 0, 7); // b0111|0101
         Assert.AreEqual(0x75, dst[0]);

         dst[0] = 0x00;
         ABufferExtensions.BitCopy(src, 0x04, dst, 0, 12); // b0000|0110|1111|0101
         Assert.AreEqual(0xF5, dst[0]);
         Assert.AreEqual(0x06, dst[1]);

         // Full copy
         dst[0] = 0x00;
         dst[1] = 0x00;
         ABufferExtensions.BitCopy(src, 0x00, dst, 0, 16);
         CollectionAssert.AreEqual(src, dst);

         // Non-aligned write
         dst[0] = 0x00;
         dst[1] = 0x00;
         ABufferExtensions.BitCopy(src, 0x02, dst, 0x03, 5); // b1011|1000
         Assert.AreEqual(0xB8, dst[0]);

         dst[0] = 0x00;
         dst[1] = 0x00;
         ABufferExtensions.BitCopy(src, 0x02, dst, 0x0A, 5); // b0101|1100|0000|0000
         Assert.AreEqual(0x00, dst[0]);
         Assert.AreEqual(0x5C, dst[1]);

         // Aligned single byte to not first byte of destination.
         dst[0] = 0x00;
         dst[1] = 0x00;
         ABufferExtensions.BitCopy(src, 0x00, dst, 0x08, 8);
         Assert.AreEqual(src[0], dst[1]);
      }

      [TestMethod]
      public void TestStuctToUInt16()
      {
         var mock = new AMockData();
         ACompany company = mock.Company();

         UInt16[] buffer = ABufferExtensions.CopyToUInt16Array(company);
         Assert.IsNotNull(buffer);

         ACompany readBackCompany = new ACompany();
         Assert.IsTrue(ABufferExtensions.MarshalUInt16ArrayToObject(buffer, readBackCompany));
         Assert.AreEqual(company, readBackCompany);
      }
   }
}

