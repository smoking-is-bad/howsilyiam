// Copyright (c) 2017 Sensor Networks, Inc.
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
using TabletApp.Utils;
using System;
using System.Collections.Generic;
using TabletApp.Properties;

namespace TabletApp.Persist
{
   /// <summary>
   /// SNI CSV file.
   /// </summary>
   class ASniCsvFile : ACsvFile
   {
      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="path">The fully qualified path to the CSV file (will be created or appended)</param>
      public ASniCsvFile(String path) : base(path)
      {
      }

      /// <summary>
      /// Get the alarm state as a string (R = error, Y = warning, G = normal) for the given probe.
      /// </summary>
      /// <param name="probe">The AProbe</param>
      /// <returns>Alarm string</returns>
      protected string GetProbeAlarmState(AProbe probe)
      {
         switch (probe.CurrentAlertState())
         {
            case AlertState.Error:
               return "R";
            case AlertState.Warning:
               return "Y";
            case AlertState.Normal:
               return "G";
         }
         return "G";
      }

      internal static string Units(string metric, string imperial)
      {
         return (Settings.Default.CsvIsMetric ? $"({metric})" : $"({imperial})");
      }

      /// <summary>
      /// Write out the probe list header
      /// </summary>
      protected void WriteHeader()
      {
         TimeZoneInfo timezone = TimeZoneMapping.TimeZoneForId(Settings.Default.CsvTimezoneId);
         var rm = ACsvFile.ResourceManager;
         this.AddLine(new List<string> {
            string.Format(rm.GetString("TimeDateTitle"), timezone.DisplayName),
            rm.GetString("ThicknessAlarmStateTitle"),
            rm.GetString("StateTitleThickness")    + " " + Units("mm", "in"),
            rm.GetString("MaterialTempTitle")      + " " + Units("C", "F"),
            rm.GetString("DsiTempTitle")           + " " + Units("C", "F"),
            rm.GetString("ReferenceVelocityTitle") + " " + Units("m/sec", "in/µsec"),
            rm.GetString("ThicknessAlarmThresholdTitle"),
            rm.GetString("ThicknessWarningThresholdTitle"),
            rm.GetString("CompanyTitle"),
            rm.GetString("SiteTitle"),
            rm.GetString("PlantTitle"),
            rm.GetString("AssetTitle"),
            rm.GetString("CollectionPointTitle"),
            rm.GetString("SerialNumberTitle"),
            rm.GetString("ProbeNumberTitle")
         });
      }

      /// <summary>
      /// Format the given string for saving to a CSV file, respecting the CSV settings.
      /// </summary>
      /// <param name="measString"></param>
      /// <returns></returns>
      protected string FormatMeasurementString(string measString)
      {
         bool metric = Settings.Default.ThicknessIsMetric;
         int mmPrec = Settings.Default.PrecisionMm;
         int inPrec = Settings.Default.PrecisionInches;
         
         // use csv prefs for thickness values
         Settings.Default.ThicknessIsMetric = Settings.Default.CsvIsMetric;
         Settings.Default.PrecisionMm = 5;
         Settings.Default.PrecisionInches = 5;

         string formattedString = measString.FormatAsMeasurmentString(false);

         Settings.Default.ThicknessIsMetric = metric;
         Settings.Default.PrecisionMm = mmPrec;
         Settings.Default.PrecisionInches = inPrec;

         return formattedString;
      }

      /// <summary>
      /// Format the given timestamp string for CSV output, using the preferred timezone.
      /// </summary>
      /// <param name="tsString"></param>
      /// <returns></returns>
      protected string FormatTimestampString(string tsString)
      {
         TimeZoneInfo timezone = TimeZoneMapping.TimeZoneForId(Settings.Default.CsvTimezoneId);

         string formattedString = tsString.FormatAsTimestampString(timezone);

         return formattedString;
      }

      /// <summary>
      /// Write out the probe information for the given probe.
      /// </summary>
      public void AppendProbe(AProbe probe, ANanoSense nano)
      {
         const String naString = "N/A";
         String materialTemp = naString;
         String dsiTemp = naString;
         if (null != probe.setups[0])
         {
            try
            {
               float cTemp = (float)Convert.ToDouble(probe.setups[0].materialTemp);
               if (cTemp < 900)
               {
                  float floatTemp = AUnitUtils.CelsiusToFahrenheit(cTemp);
                  materialTemp = Settings.Default.CsvIsMetric ? probe.setups[0].materialTemp : String.Format("{0:0.##}", floatTemp);
               }
               cTemp = (float)Convert.ToDouble(probe.setups[0].dsiTemp);
               if (cTemp < 900)
               {
                  float floatTemp = AUnitUtils.CelsiusToFahrenheit(cTemp);
                  dsiTemp = Settings.Default.CsvIsMetric ? probe.setups[0].dsiTemp : String.Format("{0:0.##}", floatTemp);
               }
            }
            catch (Exception)
            {
               // ignore and use defaults
            }
         }
         double velocity = Convert.ToDouble(probe.velocity);
         string velString = Settings.Default.CsvIsMetric ? String.Format("{0:0}", velocity) : String.Format("{0:0.00000}", velocity * AUnitUtils.kMetersPerSecToInPerUsec);
         this.AddLine(new List<string> {
               this.FormatTimestampString(probe.Timestamp()),
               this.GetProbeAlarmState(probe),
               this.FormatMeasurementString(probe.Thickness()),
               materialTemp,
               dsiTemp,
               velString,
               null != probe.minimumThickness ? this.FormatMeasurementString(probe.minimumThickness) : naString,
               null != probe.warningThickness ? this.FormatMeasurementString(probe.warningThickness) : naString,
               null != nano.company ? (string)nano.company.name : naString,
               null != nano.site ? (string)nano.site.name : naString,
               null != nano.plant ? (string)nano.plant.name : naString,
               null != nano.asset ? (string)nano.asset.name : naString,
               null != nano.collectionPoint ? (string)nano.collectionPoint.name : naString,
               nano.Dsi.serialNumber,
               Convert.ToString(probe.num) });
      }
   }
}

