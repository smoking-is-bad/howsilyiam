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

using Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabletApp.Content.Commissioning;
using TabletApp.Properties;
using TabletApp.Views;

namespace TabletApp.Utils
{
   /// <summary>
   /// Utility class for handling unit display within the app.
   /// </summary>
   public sealed class AUnitUtils
   {
      public static readonly ushort[] kMuxSingleMode = new ushort[16] { 128, 64, 32, 16, 8, 4, 2, 1, 32768, 16384, 8192, 4096, 2048, 1024, 512, 256 };
      public static readonly ushort[] kMuxDualMode = new ushort[8] { 32896, 16448, 8224, 4112, 2056, 1028, 514, 257 };
      public static readonly ushort[,] kSwitchSingleMode = new ushort[5, 16] 
         { { 266, 261, 266, 261, 266, 261, 266, 261, 266, 261, 266, 261, 266, 261, 266, 261 },
           { 522, 517, 522, 517, 522, 517, 522, 517, 522, 517, 522, 517, 522, 517, 522, 517 },
           { 1034, 1029, 1034, 1029, 1034, 1029, 1034, 1029, 1034, 1029, 1034, 1029, 1034, 1029, 1034, 1029 },
           { 2058, 2053, 2058, 2053, 2058, 2053, 2058, 2053, 2058, 2053, 2058, 2053, 2058, 2053, 2058, 2053 },
           { 4106, 4101, 4106, 4101, 4106, 4101, 4106, 4101, 4106, 4101, 4106, 4101, 4106, 4101, 4106, 4101 } };
      public static readonly ushort[,] kSwitchDualMode = new ushort[5, 8] 
         { { 265, 265, 265, 265, 265, 265, 265, 265 },
           { 521, 521, 521, 521, 521, 521, 521, 521 }, 
           { 1033, 1033, 1033, 1033, 1033, 1033, 1033, 1033 },
           { 2057, 2057, 2057, 2057, 2057, 2057, 2057, 2057 },
           { 4105, 4105, 4105, 4105, 4105, 4105, 4105, 4105 } };

      public static readonly float kMetersPerSecToInPerUsec = 0.00003937f;
      public static readonly float kInPerSecToMetersPerSec = 25400f;
      public static readonly float kMetersToInches = 39.3701f;
      public static readonly float kInchesToMeters = 0.0254f;
      public static readonly float kPsecCountDuration = 6.103515625f;
      public static readonly float kNanoSecondsOneSecond = 1000000000f;
      public static readonly float kMinVelocityMetersPerSec = 3500f;
      public static readonly float kMaxVelocityMetersPerSec = 6500f;
      public static readonly float kMinVelocityInchesPerUsec = 0.13f;
      public static readonly float kMaxVelocityInchesPerUsec = 0.26f;
      public static readonly float kMinAbsoluteThreshold = 1250;
      public static readonly float kMaxAbsoluteThreshold = 25000;

      public static void ConfigureVelocityUnits(FloatTextBox textBox, Label unitsLabel)
      {
         bool isMetric = Settings.Default.ThicknessIsMetric;
         unitsLabel.Text = (isMetric ? "m/sec" : "inches/µsec");
         textBox.NumPlaces = (isMetric ? 0 : 4);
         textBox.MinValue = (isMetric ? kMinVelocityMetersPerSec : kMinVelocityInchesPerUsec);
         textBox.MaxValue = (isMetric ? kMaxVelocityMetersPerSec : kMaxVelocityInchesPerUsec);
         textBox.Increment = (isMetric ? 1f : 0.0001f);
      }

      public static void ConfigureThicknessUnits(FloatTextBox textBox, Label unitsLabel)
      {
         bool isMetric = Settings.Default.ThicknessIsMetric;
         unitsLabel.Text = (isMetric ? "mm" : "inches");
         textBox.NumPlaces = (isMetric ? Settings.Default.PrecisionMm : Settings.Default.PrecisionInches);
         textBox.MinValue = 0f;
         textBox.MaxValue = 1000f;
      }

      public static void ConfigureStartWidthUnits(FloatTextBox textBox, Label unitsLabel)
      {
         bool isMetric = Settings.Default.ThicknessIsMetric;
         unitsLabel.Text = (isMetric ? "mm" : "inches");
         textBox.NumPlaces = (isMetric ? Settings.Default.PrecisionMm : Settings.Default.PrecisionInches);
         textBox.MinValue = 0f;
         textBox.MaxValue = 1000f;
      }

      public static void LoadDsiVelocity(AProbe probe, FloatTextBox textBox)
      {
         bool isMetric = Settings.Default.ThicknessIsMetric;
         float mPerSec = (float)Convert.ToDouble(probe.velocity);
         textBox.Value = (isMetric ? mPerSec : kMetersPerSecToInPerUsec * mPerSec);
      }

      public static void StoreDsiVelocity(AProbe probe, FloatTextBox textBox)
      {
         bool isMetric = Settings.Default.ThicknessIsMetric;
         if (isMetric)
         {
            probe.velocity = Convert.ToString(textBox.Value);
         }
         else
         {
            float inPerUsec = textBox.Value;
            probe.velocity = (inPerUsec * kInPerSecToMetersPerSec).ToString();
         }
      }

      private static void SetThicknessIncrement(AProbe probe, FloatTextBox textBox)
      {
         bool isMetric = Settings.Default.ThicknessIsMetric;
         float velocity = (float)Convert.ToDouble(probe.velocity);
         float incMeters = velocity * 25f / kNanoSecondsOneSecond;
         textBox.Increment = (isMetric ? incMeters * 1000f : kMetersToInches * incMeters);
      }

      public static void LoadNomThickness(AProbe probe, FloatTextBox textBox)
      {
         bool isMetric = Settings.Default.ThicknessIsMetric;
         float meters = (float)Convert.ToDouble(probe.nominalThickness);
         textBox.Value = (isMetric ? meters * 1000f : kMetersToInches * meters);
         AUnitUtils.SetThicknessIncrement(probe, textBox);
      }

      public static void StoreNomThickness(AProbe probe, FloatTextBox textBox)
      {
         bool isMetric = Settings.Default.ThicknessIsMetric;
         if (isMetric)
         {
            float mm = textBox.Value;
            probe.nominalThickness = (mm / 1000f).ToString();
         }
         else
         {
            float inches = textBox.Value;
            probe.nominalThickness = (inches * kInchesToMeters).ToString();
         }
      }

      public static void LoadMinThickness(AProbe probe, FloatTextBox textBox)
      {
         bool isMetric = Settings.Default.ThicknessIsMetric;
         float meters = (float)Convert.ToDouble(probe.minimumThickness);
         textBox.Value = (isMetric ? meters * 1000f : kMetersToInches * meters);
         AUnitUtils.SetThicknessIncrement(probe, textBox);
      }

      public static void StoreMinThickness(AProbe probe, FloatTextBox textBox)
      {
         bool isMetric = Settings.Default.ThicknessIsMetric;
         if (isMetric)
         {
            float mm = textBox.Value;
            probe.minimumThickness = (mm / 1000f).ToString();
         }
         else
         {
            float inches = textBox.Value;
            probe.minimumThickness = (inches * kInchesToMeters).ToString();
         }
      }

      public static void LoadWarningThickness(AProbe probe, FloatTextBox textBox)
      {
         bool isMetric = Settings.Default.ThicknessIsMetric;
         float meters = (float)Convert.ToDouble(probe.warningThickness);
         textBox.Value = (isMetric ? meters * 1000f : kMetersToInches * meters);
         AUnitUtils.SetThicknessIncrement(probe, textBox);
      }

      public static void StoreWarningThickness(AProbe probe, FloatTextBox textBox)
      {
         bool isMetric = Settings.Default.ThicknessIsMetric;
         if (isMetric)
         {
            float mm = textBox.Value;
            probe.warningThickness = (mm / 1000f).ToString();
         }
         else
         {
            float inches = textBox.Value;
            probe.warningThickness = (inches * kInchesToMeters).ToString();
         }
      }

      public static void LoadCalZeroOffset(AProbe probe, FloatTextBox textBox)
      {
         ulong psecCounts = probe.calZeroCount;
         float psec = kPsecCountDuration * (float)psecCounts;
         float usec = psec / 1000000f;
         textBox.Value = usec;
      }

      public static void StoreCalZeroOffset(AProbe probe, FloatTextBox textBox)
      {
         float usec = textBox.Value;
         float psec = usec * 1000000f;
         uint psecCounts = (uint)(psec / kPsecCountDuration);
         probe.calZeroCount = psecCounts;
      }

      public static void LoadPulserWidth(ASetup setup, NumberTextBox textBox)
      {
         ushort nsecCounts = setup.pulserWidth;
         float nsec = nsecCounts * 25;
         textBox.Value = nsec;
      }

      public static void StorePulserWidth(ASetup setup, NumberTextBox textBox)
      {
         float nsec = (float)textBox.Value;
         ushort nsecCounts = (ushort)(nsec / 25f);
         setup.pulserWidth = nsecCounts;
      }

      public static void LoadGain(ASetup setup, FloatTextBox textBox)
      {
         textBox.Value = (float)setup.gain / 10F;
      }

      public static void StoreGain(ASetup setup, FloatTextBox textBox)
      {
         setup.gain = (ushort)(textBox.FloatValue * 10F);
      }

      public static void LoadProbeNumber(AProbe probe, ASetup setup, NumberTextBox textBox)
      {
         ushort[] muxNums = ("dual" == probe.type.ToLower() ? AUnitUtils.kMuxDualMode : AUnitUtils.kMuxSingleMode);
         int probeNumber = 1;
         foreach (ushort mux in muxNums)
         {
            if (setup.muxSwitchSettings == mux)
            {
               break;
            }
            ++probeNumber;
         }
         textBox.Value = probeNumber;
      }

      public static void LoadAverages(AProbe probe, ASetup setup, ComboBox combo)
      {
         int probeIndex = probe.num - 1;
         for (int i = 0; i < AUnitUtils.kSwitchSingleMode.GetLength(0); ++i)
         {
            if (("dual" == probe.type.ToLower() && probeIndex < AUnitUtils.kSwitchDualMode.Length && setup.switchSettings == AUnitUtils.kSwitchDualMode[i, probeIndex]) ||
               ("single" == probe.type.ToLower() && setup.switchSettings == AUnitUtils.kSwitchSingleMode[i, probeIndex]))
            {
               combo.SelectedIndex = i;
               break;
            }
         }
      }

      public static void StoreAverages(AProbe probe, ASetup setup, ComboBox combo)
      {
         int probeIndex = probe.num - 1;
         if ("dual" == probe.type.ToLower() && probeIndex < AUnitUtils.kMuxDualMode.Length)
         {
            setup.switchSettings = AUnitUtils.kSwitchDualMode[combo.SelectedIndex, probeIndex];
         }
         else
         {
            setup.switchSettings = AUnitUtils.kSwitchSingleMode[combo.SelectedIndex, probeIndex];
         }
      }

      private static void SetStartWidthIncrementMinMax(AProbe probe, FloatTextBox textBox, uint maxCount)
      {
         bool isMetric = Settings.Default.ThicknessIsMetric;
         float meters = kMinVelocityMetersPerSec * 25 / kNanoSecondsOneSecond;
         textBox.MinValue = (isMetric ? meters * 1000f : kMetersToInches * meters);
         meters = kMaxVelocityMetersPerSec * maxCount * 25 / kNanoSecondsOneSecond;
         textBox.MaxValue = (isMetric ? meters * 1000f : kMetersToInches * meters);
         textBox.Increment = textBox.MinValue;
      }

      public static bool ValidateVelocityDependentParameters(AProbe probe)
      {
         // (1)  Ascan Start
         //
         //       0 < (Ascan Start) < 37,951
         //       if Gate mode =0, (Ascan Start) < (Gate1 Start) AND (Gate 2 Start)
         //       if Gate mode =1, (Ascan Start) < (Gate 1 Start)
         //
         // (2)  Gate 1
         //
         //       0 ≤ (Gate 1 Start) < 39,900
         //       (Gate 1 Start) ≧ (Ascan Start)
         //       (Gate 1 Start) - (Ascan Start) + (Gate 1 Width) ≤ 2048 
         //
         // (2)  Gate 2
         //
         //      if Gate mode = 1
         //       0 ≤ (Gate 2 Start) < 39,900
         //       (Gate 2 Start) + (Gate 2 Width) + (Gate 1 Start) + (Gate 1 Width) - (Ascan Start) ≤ 2048 

         foreach (ASetup setup in probe.setups)
         {
            Debug.Assert(setup.gates.Length >= 2);
            AGate gate1 = setup.gates[0];
            AGate gate2 = setup.gates[1];

            uint ascanStart = setup.ascanStart;
            uint gate1Start = gate1.start;
            uint gate1Width = gate1.width;
            uint gate2Start = gate2.start;
            uint gate2Width = gate2.width;
            int gateMode = (0 != (0x8000 & gate1.mode) ? 1 : 0);

            //       0 < (Ascan Start) < 37,951
            if (ascanStart <= 0 || ascanStart >= 37951)
            {
               return false;
            }
            //       0 ≤ (Gate 1 Start) < 39,900
            //       (Gate 1 Start) ≧ (Ascan Start)
            //       (Gate 1 Start) - (Ascan Start) + (Gate 1 Width) ≤ 2048 
            if (gate1Start <= 0 || gate1Start >= 39900)
            {
               return false;
            }
            if (gate1Start < ascanStart)
            {
               return false;
            }
            if (gate1Start - ascanStart + gate1Width > 2048)
            {
               return false;
            }

            //      if Gate mode = 1
            //       0 ≤ (Gate 2 Start) < 39,900
            //       (Gate 2 Start) + (Gate 2 Width) + (Gate 1 Start) + (Gate 1 Width) - (Ascan Start) ≤ 2048 
            if (1 == gateMode)
            {
               if (gate2Start < 0 || gate2Start >= 39900)
               {
                  return false;
               }
               if (gate2Start + gate2Width + gate1Start + gate1Width - ascanStart > 2048)
               {
                  return false;
               }
            }
         }

         return true;
      }

      public static void LoadAscanStart(AProbe probe, ASetup setup, FloatTextBox textBox)
      {
         bool isMetric = Settings.Default.ThicknessIsMetric;
         float velocity = (float)Convert.ToDouble(probe.velocity);      // m/sec
         uint nsecCounts = setup.ascanStart;
         float nsec = nsecCounts * 25;
         float meters = velocity * nsec / kNanoSecondsOneSecond;
         textBox.Value = (isMetric ? meters * 1000f : kMetersToInches * meters) / 2f;     // div 2 to account for round trip
         AUnitUtils.SetStartWidthIncrementMinMax(probe, textBox, 37951);
      }

      public static void StoreAscanStart(AProbe probe, ASetup setup, FloatTextBox textBox)
      {
         bool isMetric = Settings.Default.ThicknessIsMetric;
         float velocity = (float)Convert.ToDouble(probe.velocity);      // m/sec
         float meters = (isMetric ? textBox.Value / 1000f : kInchesToMeters * textBox.Value);
         float nsec = meters * kNanoSecondsOneSecond / velocity;
         setup.ascanStart = Math.Max((uint)((nsec / 25f) * 2f), 1);    // * 2 to account for round trip
      }

      public static void LoadGateStart(AProbe probe, AGate gate, FloatTextBox textBox)
      {
         bool isMetric = Settings.Default.ThicknessIsMetric;
         float velocity = (float)Convert.ToDouble(probe.velocity);      // m/sec
         uint nsecCounts = gate.start;
         // for gate 1, we need to subtract off the calzero offset for the dispayed value
         if (1 == gate.num)
         {
            nsecCounts -= AUnitUtils.DsiGateStartOffset(probe);
         }
         float nsec = nsecCounts * 25;
         float meters = velocity * nsec / kNanoSecondsOneSecond;
         float displayValue = (isMetric ? meters * 1000f : kMetersToInches * meters) / 2f;     // div 2 to account for round trip
         textBox.Value = displayValue;
         AUnitUtils.SetStartWidthIncrementMinMax(probe, textBox, 39900);
      }

      public static void StoreGateStart(AProbe probe, AGate gate, FloatTextBox textBox)
      {
         bool isMetric = Settings.Default.ThicknessIsMetric;
         float velocity = (float)Convert.ToDouble(probe.velocity);      // m/sec
         float meters = (isMetric ? textBox.Value / 1000f : kInchesToMeters * textBox.Value);
         float nsec = meters * kNanoSecondsOneSecond / velocity;
         uint gateStart = (uint)Math.Round(((double)nsec / 25.0) * 2.0);    // * 2 to account for round trip
         // for gate 1, we need to add the calzero offset to the stored gate start value
         if (1 == gate.num)
         {
            gateStart += AUnitUtils.DsiGateStartOffset(probe);
         }
         gate.start = gateStart;
      }

      public static void LoadGateWidth(AProbe probe, AGate gate, FloatTextBox textBox)
      {
         bool isMetric = Settings.Default.ThicknessIsMetric;
         float velocity = (float)Convert.ToDouble(probe.velocity);      // m/sec
         uint nsecCounts = gate.width;
         float nsec = nsecCounts * 25;
         float meters = velocity * nsec / kNanoSecondsOneSecond;
         textBox.Value = (isMetric ? meters * 1000f : kMetersToInches * meters) / 2f;     // div 2 to account for round trip
         AUnitUtils.SetStartWidthIncrementMinMax(probe, textBox, 39900);
         textBox.MinValue = 0;   // override the min value to 0
      }

      public static void StoreGateWidth(AProbe probe, AGate gate, FloatTextBox textBox)
      {
         bool isMetric = Settings.Default.ThicknessIsMetric;
         float velocity = (float)Convert.ToDouble(probe.velocity);      // m/sec
         float meters = (isMetric ? textBox.Value / 1000f : kInchesToMeters * textBox.Value);
         float nsec = meters * kNanoSecondsOneSecond / velocity;
         gate.width = (uint)Math.Round(((double)nsec / 25.0) * 2.0);    // * 2 to account for round trip
      }

      public static void LoadGateThreshold(AGate gate, NumberTextBox textBox)
      {
         textBox.Value = (int)((float)gate.threshold / kMaxAbsoluteThreshold * 100f);
      }

      public static void StoreGateThreshold(AGate gate, NumberTextBox textBox)
      {
         gate.threshold = (short)(((float)textBox.Value / 100f) * kMaxAbsoluteThreshold);

         // apply -5 +5 (-1250 +1250) limits to gate threshold - don't report as invalid if so
         if (gate.threshold < kMinAbsoluteThreshold && gate.threshold >= 0)
         {
            gate.threshold = (short)kMinAbsoluteThreshold;
         }
         else if (gate.threshold > -kMinAbsoluteThreshold && gate.threshold < 0)
         {
            gate.threshold = (short)-kMinAbsoluteThreshold;
         }
      }

      public static float DisplayGateStartOffset(AProbe probe)
      {
         bool isMetric = Settings.Default.ThicknessIsMetric;
         float velocity = (float)Convert.ToDouble(probe.velocity);      // m/sec
         ulong psecCounts = probe.calZeroCount;
         float psec = kPsecCountDuration * (float)psecCounts;
         float nsecCalZero = psec / 1000f;
         float meters = velocity * nsecCalZero / kNanoSecondsOneSecond;
         return (isMetric ? meters * 1000f : kMetersToInches * meters);
      }

      /// <returns>picosecond count value converted to nanosecond count value</returns>
      public static uint DsiGateStartOffset(AProbe probe)
      {
         ulong psecCounts = probe.calZeroCount;
         float psec = kPsecCountDuration * (float)psecCounts;
         float nsecCalZero = psec / 1000f;
         return (uint)Math.Round((double)nsecCalZero / 25.0);
      }

      public static float CelsiusToFahrenheit(float celsius)
      {
         float fahrenheit = celsius * (9f / 5f) + 32;
         return fahrenheit;
      }
   }
}

