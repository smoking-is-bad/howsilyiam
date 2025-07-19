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
using System.Linq; // for text dump of samples
using Microsoft.VisualStudio.TestTools.UnitTesting;

using DsiApi;
using Model;

namespace UnitTestDsiSim
{
   [TestClass]
   public class ZeroCrossingTests
   {
      [TestMethod]
      public void TestNoZeroWhenThresholdNotExceeded()
      {
         double[] upsamples = fUpsampler.Upsample(fAllPositivesAboveThreshold);

#if false // Dump samples
         System.IO.File.WriteAllLines("upsamples.txt",
          upsamples.Select(sample => (sample.ToString() + ",")));
#endif

         AGate[] gates = new AGate[] { fGate };
         double[] crossings = fCalculator.FindZeroCrossings(upsamples, 0, kScale, gates);

         Assert.AreNotEqual(null, crossings);

         Assert.AreEqual(gates.Length, crossings.Length);

         Assert.IsTrue(Double.IsNaN(crossings[0]));
      }

      [TestMethod]
      public void TestZeroOnSharpCrossing()
      {
         double[] upsamples = fUpsampler.Upsample(fSquare);

#if true // Dump samples
         System.IO.File.WriteAllLines("square-upsamples.txt",
          upsamples.Select(sample => sample.ToString()));
#endif

         AGate[] gates = new AGate[] { fGate };
         double[] crossings = fCalculator.FindZeroCrossings(upsamples, 0, kScale, gates);

         Assert.AreNotEqual(null, crossings);

         Assert.AreEqual(gates.Length, crossings.Length);

         Assert.IsFalse(Double.IsNaN(crossings[0]));
      }

      const int kScale = 8; // 1:8 upsampling
      const double kSampleRate = 40e6;

      private AThicknessCalculator fCalculator = new AThicknessCalculator(kSampleRate * kScale,
       velocity:5900.0, calZeroOffset:0.0);

      private AAscanUpsampler fUpsampler = new AAscanUpsampler(kScale, kSampleRate);

      private AGate fGate = new AGate() { start = 1, width = 8, threshold = -10000 };

      private Int16[] fAllPositivesAboveThreshold =  { 
       0, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000, 0 };

      private Int16[] fSquare = { 
       0, 15000, 15000, 15000, 15000, -15000, -15000, -15000, -15000, 0 };

   }
}
