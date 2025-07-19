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
using System.Diagnostics;
using System.Linq;

using Model;


namespace DsiApi
{

   /// <summary>
   /// Handles calculation of thicknesses and the reverse in one of two modes
   /// </summary>
   /// 
   /// <remarks>
   /// kBangToFirstEcho - absolute thickness for a signal traveling to that distance and returning
   /// kDelayLineToFirstWall - thickness from one sample to another, assuming round-trip
   /// 
   /// The calculator is configured for measuring values for a specific probe. It takes ascan
   /// data and upsamples it to a higher resolution space, searches for zero crossings constrained
   /// by gates, and calculates a thickness given that zero crossing.
   /// 
   /// The sample ticks are assumed to be in that higher resolution space, but samples at other
   /// rates are supported as multiples of the probe's sample rate. See AbsoluteThicknessAtTime().
   /// </remarks>
   public class AThicknessCalculator
   {
      public enum DetectionMode
      {
         kBangToFirstEcho,
         kDelayLineToFirstWall,
         kMultiEcho
      }

      const double kNoCrossing = double.NaN; // Helps ensure we get NaN results for thickness.

      /// <summary>
      ///  Create configured thickness calculator and assign to setup. 
      /// </summary>
      public static void PrepareSetupForThicknessCalculation(AProbe probe, ASetup setup)
      {
         // Older data files didn't have this field. 
         // 460e-09  default used for first show
         double calZeroOffset = probe.calZeroOffset; /* seconds */;
         var calc = new AThicknessCalculator(ADsiNetwork.kAscanUpsampleScale *
          ADsiInfo.kSampleRate, Convert.ToDouble(probe.velocity), calZeroOffset);
         AFilter filter = new AAscanUpsampler(ADsiNetwork.kAscanUpsampleScale);

         // Calculate a translation to apply to crossings to compensate for phase shift caused by 
         // the upsampling. The thickness calculation won't need it, but display does.
         // Units:
         // samples = nanoseconds / (nonoseconds per upsample) / (upsamples per sample)
         // which was filter.Delay * 1e09 / 25.0 / ADsiNetwork.kAscanUpsampleScale;
         setup.crossingDelayCompensation = 7; // !!! This is Jim's magic number for the new firmware.
         setup.fCalculator = calc;
      }


      /// <param name="upscaledSampleRate">The sample rate of the data (including scale of any upsampling done)</param>
      /// <param name="velocity"></param>
      /// <param name="calZeroOffset"></param>
      public AThicknessCalculator(double upscaledSampleRate, double velocity, double calZeroOffset)
      {
         fSampleRate = upscaledSampleRate;
         fSamplePeriod = 1.0 / fSampleRate; // time per sample
         fVelocity = velocity;
         fCalZeroOffset = calZeroOffset;
      }


      public static DetectionMode DetectionModeForGates(IList<AMarshaledGate> gates)
      {
         const UInt16 kModeDelayLineBit = 0x8000;
         var mode = DetectionMode.kBangToFirstEcho;
         if (0x0000 != (gates[0].mode & kModeDelayLineBit))
         {
            mode = DetectionMode.kDelayLineToFirstWall;
         }
         return mode;
      }

      /// <summary>
      /// Calculate thickness in meters at a given number of samples after time_0, assumes the signal made a round trip to the distance and back.
      /// </summary>
      /// IMPORTANT:When modifying this, you must also modify TimeAtThickness to perform the inverse operation.
      /// <param name="rateScale">If tickCount is at a different sample rate, use this to scale the rate</param>
      public double ThicknessAtAbsoluteTime(double tickCount, double rateScale = 1.0)
      {
         return (tickCount * fSamplePeriod * rateScale - fCalZeroOffset) * fVelocity / 2.0;
      }

      /// <summary>
      /// Calculate thickness given two times, each measured the same way, so other
      /// constants like fCalZeroOffset are cancelled.
      /// </summary>
      public double ThicknessAtRelativeTime(double tickCount, double relativeTo, double rateScale = 1.0)
      {
         return (tickCount - relativeTo) * fSamplePeriod * rateScale * fVelocity * 0.5;
      }

      /// UNUSED!!!
      /// <summary>
      /// Calculate the sample tick which produces the given thickness.
      /// </summary>
      /// <param name="thickness">The thickness in meters</param>
      /// <param name="rateScale">If a time at a different multiple of the sample rate is desired, use this to scale the resolution</param>
      public double TimeAtRelativeThickness(double thickness, double relativeTick, double rateScale = 1.0)
      {
         return (thickness * 2.0 / fVelocity ) * fSampleRate / rateScale + relativeTick;
      }

      /// Given a list of zero crossings calculate the distance from probe to that crossing.
      public double MeasureThicknessZeroCross(double[] crossings, DetectionMode mode = DetectionMode.kBangToFirstEcho)
      {
         // Crossings may be NaN, but that will return a thickness of NaN, which is what we want.
         double thickness = double.NaN;
         switch (mode)
         {
            case DetectionMode.kBangToFirstEcho:
            Debug.Assert(crossings.Length >= 1);
            thickness = this.ThicknessAtAbsoluteTime(crossings[0]);
            break;

            case DetectionMode.kDelayLineToFirstWall:
            Debug.Assert(crossings.Length > 1);
            if (crossings.Length > 1)
            {
               //R.MeasuredThickness = (ZeroCrossing2-ZeroCrossing1)*Acq.UpSamplePeriod*C.Velocity / 2.0;
               thickness = this.ThicknessAtRelativeTime(crossings[1], crossings[0]);
            }
            break;

            // !!!: What's different from above case?
            case DetectionMode.kMultiEcho:
            Debug.Assert(crossings.Length > 1);
            if (crossings.Length > 1)
            {
               //R.MeasuredThickness = (ZeroCrossing2 - ZeroCrossing1)*Acq.UpSamplePeriod*C.Velocity / 2.0;
               thickness = (crossings[1] - crossings[0]) * fSamplePeriod * fVelocity / 2.0;
            }
            break;
         }

         return thickness;
      }

      /// <param name="gateDelayCompensation">When the ascan is upsampled, the FIR filter shifts time by some amount. This value is applied to gate times to compensate.</param>
      public double[] FindZeroCrossings(double[] ascanData, int ascanStart, 
       double upsampleScale, IList<AMarshaledGate> gates, double gateDelayCompensation)
      {
         var mode = DetectionModeForGates(gates);

         var crossings = new double[gates.Count];
         int crossingIndex = 0;
         double crossing = kNoCrossing;
         foreach (AMarshaledGate gate in gates)
         {
            int gateWidth = (int)gate.width;
            bool noPreviousCrossing = ((crossingIndex > 0) && (Double.IsNaN(crossing)));
            if ((0 == gateWidth) || noPreviousCrossing)
            {
               crossings[crossingIndex++] = kNoCrossing;
               continue;
            }

            crossing = kNoCrossing;

            int gateStart = (int)(gate.start + Math.Ceiling(gateDelayCompensation));

            // Translate the gate which was defined from time_0 to sample index which starts at ascanStart.
            // But gate's are relative to the zero crossing of the first gate, and
            // doesn't need this compensation.
            if (crossingIndex == 0)
            {
               gateStart -= ascanStart;
            }

            if (gateStart < 0)
            {
               gateWidth += gateStart;
               gateStart = 0;
            }

            //Console.WriteLine("Find crossing in {0} to {1} compensated with {2}", gateStart, gateStart + gateWidth, gateDelayCompensation);

            // Translate gate values to upsampled space.
            gateStart = (int)(gateStart * upsampleScale);

            // This mode defines gate 2 relative to previous zero crossing.
            if (DetectionMode.kDelayLineToFirstWall == mode && crossingIndex == 1) // $$$ crossingIndex != 0   all gates beyond first will be relaive to previous zero crossing.
            {
               double prevExactCrossing = crossings[crossingIndex - 1];

               // kNoCrossing is NaN, but equality doesn't work with it. In case 
               // we ever change kNoCrossing to something else, I am leaving both tests.
               if (!Double.IsNaN(prevExactCrossing) && kNoCrossing != prevExactCrossing)
               {
                  // The crossing was interpolated, make sure to start at next sample.
                  int prevCrossing = (int)Math.Ceiling(prevExactCrossing);
                  gateStart += prevCrossing;
               }
            }

            int gateStop = gateStart + (int)(gateWidth * upsampleScale);
  
            // Limit width to ascan array.
            int diff = gateStop - ascanData.Length;
            gateStop -= (diff > 0 ? diff : 0);
 
            double threshold = (double)gate.threshold;

            // Negated predicates from Jim's code because we are using
            // FindIndex which returns index when the test passes. 
            Predicate<double> thresholdTest = (double sample) => (sample < threshold);
            if (threshold >= 0)
            {
               thresholdTest = (double sample) => (sample >= threshold);
            }

            // Find the index of the first sample passing test or end of ascan
            int width = gateStop - gateStart;
            int i = -1;
            if (width > 0)
            {
               i = Array.FindIndex<double>(ascanData, gateStart, width, thresholdTest);
            }
            if (i < 0)
            {
               i = gateStart + width;
            }

            // --- find zero crossing stepping forward from the last value of "i" 
            // that was detected as the point exceeding the threshold
            // becomes false when the product of sample m * sample m+1 becomes 
            // negative or one of the values becomes zero;
            int lastIndex = gateStop - 1;// ascanData.Length - 1;
            while (i < lastIndex) 
            {
               if ((ascanData[i] * ascanData[i + 1]) <= 0)
               {
                  break;
               }
               ++i;
            }

            //Console.WriteLine("findIndex({0}, {1}) = {2}", gateStart, gateStop, i);

            if (i < lastIndex)
            {
               double slope = ascanData[i + 1] - ascanData[i];
               //crossing = i - ascanData[i + 1] / slope + 1;
               crossing = i - ascanData[i] / slope;
            }

            crossings[crossingIndex++] = crossing;
         }

         return crossings;
      }

      private double fSampleRate;

      private double fSamplePeriod;
      private double fVelocity;
      private double fCalZeroOffset;
   }


   public class AFilter
   {
      /// <summary>
      /// Convolve fir filter to generate upsampled signal 
      //  Algorithm:  y(j+nL)=sum x(n-k)*h(j+kL), sum k=0:K, j=0:L-1, n=0:length(x)-1
      //  L = upsample factor, K = fir filter length / upsample factor
      /// </summary>
      /// <param name="data">The data to expand by the upsampling factor.</param>
      public double[] Upsample(Int16[] data, int scale)
      {
         var upsamples = new double[scale * data.Length];

         int K = this.KernelLength / scale; // convolution period

         for (int n = 0; n < data.Length; ++n)
         {
            for (int j = 0; j < scale; ++j)
            {
               int c1 = j + n * scale;
               upsamples[c1] = 0.0;

               for (int k = 0; k < K; ++k)
               {
                  int c2 = j + k * scale;
                  if (((n - k) >= 0) && (c2 < fKernel.Length)) 
                  {
                     upsamples[c1] += data[n - k] * fKernel[c2];
                  }
                  else
                  {
                     break;
                  }
               }

               upsamples[c1] *= this.Gain;
            }
         }

         return upsamples;
      }
         
      /// <summary>
      /// The convolution kernel for the filter.
      /// </summary>
      public int[] Kernel 
      { 
         get { return fKernel ?? new int[128]; }
         set { fKernel = value; }
      }

      public int KernelLength { get { return fKernel.Length; }}

      /** Phase offset introduced by the filter, in seconds */
      public double Delay { get; set; }

      public double Gain { get; set; }

      private int[] fKernel;
   }

   public class AAscanUpsampler : AFilter
   {
      // Probably should not be parameters, kernel must depend on scale == 8.
      public AAscanUpsampler(int scale, double sampleRate = 40e6)
      {
         this.Kernel = new[] { 0, 1, 2, 1, -3, -12, -28, -49, -72, -93, -105,
            -102, -79, -32, 38, 129, 232, 335, 422, 477, 484, 429, 307, 117, -130,
            -415, -710, -981, -1188, -1296, -1273, -1097, -764, -285, 309, 968,
            1628, 2213, 2646, 2853, 2774, 2373, 1643, 611, -659, -2069, -3488, 
            -4765, -5741, -6256, -6171, -5377, -3809, -1458, 1631, 5351, 9540,
            13990, 18464, 22707, 26466, 29514, 31659, 32767, 32767, 31659, 29514,
            26466, 22707, 18464, 13990, 9540, 5351, 1631, -1458, -3809, -5377,
            -6171, -6256, -5741, -4765, -3488, -2069, -659, 611, 1643, 2373, 2774,
            2853, 2646, 2213, 1628, 968, 309, -285, -764, -1097, -1273, -1296,
            -1188, -981, -710, -415, -130, 117, 307, 429, 484, 477, 422, 335,
            232, 129, 38, -32, -79, -102, -105, -93, -72, -49, -28, -12, -3, 1,
            2, 1, 0
         };

         //formula for delay of N tap fir filter with linear phase Delay=(N-1)/(2*Fs)
         this.Delay = (this.Kernel.Length - 1) / (2 * sampleRate);

         // fir_filter_gain = upsampleN / sum(fir_filter);  
         // formula for FIR filter gain at DC:  Gain = sum(filter coefficients),
         // use inverse so we have a multiplication, also adjust for upsampleing
         this.Gain = scale / (double)Enumerable.Sum(this.Kernel);

         fScale = scale;
      }

      public double[] Upsample(Int16[] data)
      {
         return base.Upsample(data, fScale);
      }

      private int fScale;
   }
}


