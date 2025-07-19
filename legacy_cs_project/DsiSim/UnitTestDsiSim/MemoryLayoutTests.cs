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

using DsiApi;
using DsiUtil;


namespace UnitTestDsiSim
{
   // These tests aren't spectacular, but at least they document assumptions about memory
   // layout in two different places.
   //
   // If tests here start to fail make sure the assumptions in the address arrays still hold.
   //

   [TestClass]
   public class MemoryLayoutTests
   {
      [TestMethod]
      public void TestProbeAddresses()
      {
         for (UInt16 p = 0; p < AMemoryLayout.kProbeSetupNumProbes; ++p)
         {
            try
            {
               UInt16 expected = sProbeAddresses[p];
               UInt16 calculated = AMemoryLayout.ProbeAddress(p);
               Assert.AreEqual(expected, calculated, "{0} != {1}",
                expected.ToString("X"), calculated.ToString("X"));
            }
            catch (IndexOutOfRangeException ex)
            {
               Assert.IsTrue(false, "Attempted to get probe address which exceeded expected number of probes. {0}", ex.Message);
            }

         }
      }


      [TestMethod]
      public void TestSetupAddresses()
      {
         for (UInt16 p = 0; p < AMemoryLayout.kProbeSetupNumProbes; ++p)
         {
            for (UInt16 s = 0; s < AMemoryLayout.kProbeSetupNumSetups; ++s)
            {
               try
               {
                  UInt16 expected = (UInt16)(sProbeAddresses[p] + sSetupOffsets[s]);
                  UInt16 calculated = AMemoryLayout.ProbeSetupAddress(p, s);
                  Assert.AreEqual(expected, calculated, "{0} != {1}",
                   expected.ToString("X"), calculated.ToString("X"));
               }
               catch (IndexOutOfRangeException ex)
               {
                  Assert.IsTrue(false, "Attempted to get setup address which exceeded expected number of probes. {0}", ex.Message);
               }
            }
         }
      }


      [TestMethod]
      public void TestGateAddresses()
      {
         for (UInt16 p = 0; p < AMemoryLayout.kProbeSetupNumProbes; ++p)
         {
            for (UInt16 s = 0; s < AMemoryLayout.kProbeSetupNumSetups; ++s)
            {
               for (UInt16 g = 0; g < AMemoryLayout.kProbeSetupNumGates; ++g)
               {
                  try
                  {
                     UInt16 expected = (UInt16)(sProbeAddresses[p] + sSetupOffsets[s] + sGateOffsets[g]);
                     UInt16 calculated = AMemoryLayout.GateAddress(p, s, g);
                     Assert.AreEqual(expected, calculated, "Gate address 0x{0} != 0x{1}",
                      expected.ToString("X"), calculated.ToString("X"));
                  }
                  catch (IndexOutOfRangeException ex)
                  {
                     Assert.IsTrue(false, "Attempted to get setup address which exceeded expected number of probes. {0}", ex.Message);
                  }
               }
            }
         }
      }

      [TestMethod]
      public void TestAscanSectionAddresses()
      {
         for (UInt16 p = 0; p < AMemoryLayout.kProbeSetupNumProbes; ++p)
         {
            UInt16 ascanExpected = sProbeAscanAddresses[p];
            UInt16 ascanActual = AMemoryLayout.AscanSectionAddress(p);
            Assert.AreEqual(ascanExpected, ascanActual, "Ascan data address 0x{0} != 0x{1}",
             ascanExpected.ToString("X"), ascanActual.ToString("X"));

            // Validate subsection address code.
            UInt16 numRegisters = AMemoryLayout.kAscanSectionOffset;

            // There should be 17 sections with the final section returning 0x30 registers
            // This is the same loop used as when actually reading, but I haven't factored it
            // out in a way we can test other than duplicating it here.
            int section = 0;
            UInt16 sectionActual = ascanActual;
            while (numRegisters > 0)
            {
               UInt16 quantity = Math.Min(numRegisters, AMemoryLayout.kAscanSubsectionNumRegisters);
               UInt16 sectionExpected = (UInt16)(ascanExpected + sAscanSectionAddresses[section++]);
               Assert.AreEqual(sectionExpected, sectionActual, "Ascan section address 0x{0} != 0x{1}",
                sectionExpected.ToString("X"), sectionActual.ToString("X"));

               sectionActual += quantity;
               numRegisters -= quantity;
            }

            // Don't overcount.
            Assert.IsTrue(0 >= numRegisters);
         }
      }


      private static UInt16[] sProbeAddresses = new UInt16[]
      { 
         0x0000, 0x0800, 0x1000, 0x1800,
         0x2000, 0x2800, 0x3000, 0x3800,
         0x4000, 0x4800, 0x5000, 0x5800,
         0x6000, 0x6800, 0x7000, 0x7800
      };


      private static UInt16[] sAscanSectionAddresses =
      {
         0x0000, 0x007D, 0x00FA, 0x0177, 0x01F4, 0x0271,
         0x02EE, 0x036B, 0x03E8, 0x0465, 0x04E2, 0x055F,
         0x05DC, 0x0659, 0x06D6, 0x0753, 0x07D0
      };

      // Coincidentally, the ascan data equals the probe data in size. 
      // Not coincidentally, there are the same number of them.
      private static UInt16[] sProbeAscanAddresses = sProbeAddresses;


      private static UInt16[] sSetupOffsets = new UInt16[]
      {
         0x80, 0x100, 0x180, 0x200, 0x280, 0x300, 0x380, 0x400, 0x480, 0x500, 0x580
      };


      // Adjust start of gate to be setup relative.
      public static UInt16[] sGateOffsets = new UInt16[]
      {
         // Calculate offset from start of setup data.
         0xBF - 0x80, 0xC8 - 0x80, 0xD1 - 0x80
      };
   }
}

