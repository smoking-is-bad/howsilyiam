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
using DsiSim;
using Model;


namespace UnitTestDsiSim
{
   [TestClass]
   public class CalculationTests
   {
      public CalculationTests()
      {
          fUpsampler = new AAscanUpsampler(fScale);
      }


      [TestMethod]
      public void TestUpsampling()
      {
         Int16[] jimData = MockData.AAscan.JimMockData();
         double[] upsampledData =fUpsampler.Upsample(jimData);
         Assert.IsNotNull(upsampledData);
         Assert.AreEqual(jimData.Length * fScale, upsampledData.Length);

         // ??? Test interpolation on a small data set?
      }

      [TestMethod]
      public void TestThicknessCalculation()
      {
         Int16[] data = MockData.AAscan.JimMockData();
         double[] upsampledData = fUpsampler.Upsample(data);
         Assert.IsNotNull(upsampledData);
         Assert.AreEqual(data.Length * fScale, upsampledData.Length);

         // These are the values Jim used in his sample code.
         double sampleRate = 40e6 * fScale;
         double velocity = 5900.0;
         double calZeroOffset = 0;
         var calculator = new AThicknessCalculator(sampleRate, velocity, calZeroOffset);

         // We won't test zero-crossing detection directly but the thickness will not
         // be correct unless all crossings are off by a fixed amount.
         int ascanStart = 0;
         var gates = new[] 
         {
            new AGate() 
            { 
               start = 100,
               width = 1000,
               threshold = -10000,
               mode = (UInt16)AThicknessCalculator.DetectionMode.kDelayLineToFirstWall
            },
            new AGate()
            {
               start = 1100,
               width = 1000,
               threshold = -7000,
               mode = (UInt16)AThicknessCalculator.DetectionMode.kDelayLineToFirstWall
            }
         };
         var thickness = calculator.MeasureThicknessZeroCross(upsampledData, ascanStart, fScale, 
            gates);

         Assert.AreEqual(0.022125, thickness, 9E-08);
      }

      private AAscanUpsampler fUpsampler;
      private int fScale = 8;
   }
}

