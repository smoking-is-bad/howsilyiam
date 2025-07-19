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
using System.Linq;
using Model;
using TabletApp.Properties;
using DsiApi;
using System.Drawing;

namespace TabletApp.Utils
{
   public enum AlertState
   {
      Normal,
      Warning,
      Error
   }

   /// <summary>
   /// Extension methods for various model objects
   /// </summary>
   public static class ADataExtensions
   {
      public const string kDefaultNumberString = "NaN";
      public const string kDefaultDateString = "";
      private const string kNaString = "N/A";

      /// <summary>
      /// Interpret the string as a double and convert into a displayable measurement string for
      /// the UI.
      /// </summary>
      /// <param name="measurement">The measurement string</param>
      /// <returns>The formatted string</returns>
      static public string FormatAsMeasurmentString(this string measurement, bool addUnits = true)
      {
         if (measurement == null || "" == measurement)
         {
            return "";
         }
         if (measurement == "NaN")
         {
            return kNaString;
         }

         String formattedString;
         // determine the format string based on the precision settings
         string formatString = "{0:0.";
         for (int i = 0; i < (Settings.Default.ThicknessIsMetric ? Settings.Default.PrecisionMm : Settings.Default.PrecisionInches); ++i)
         {
            formatString += "0";
         }
         formatString += "}";
         if (Settings.Default.ThicknessIsMetric)
         {
            formattedString = String.Format(formatString + (addUnits ? "mm" : ""), Convert.ToDouble(measurement) * 1000.0);
         }
         else
         {
            double thicknessMeters = Convert.ToDouble(measurement);     // assumes thickness stored in meters
            double thicknessInches = thicknessMeters / 0.0254;
            formattedString = String.Format(formatString + (addUnits ? "in" : ""), thicknessInches);
         }
         return formattedString;
      }

      /// <summary>
      /// Format the given DSI timestamp string into something more user friendly,
      /// and convert to the given timezone (local time if null).
      /// </summary>
      /// <param name="timestamp">UTC timestamp string</param>
      /// <param name="timezone">Timezone (local time if null)</param>
      /// <returns>Friendly local time string</returns>
      static public string FormatAsTimestampString(this string timestamp, TimeZoneInfo timezone = null)
      {
         string formattedString = "";

         try
         {
            DateTime convertedDate = DateTime.SpecifyKind(DateTime.Parse(timestamp), DateTimeKind.Utc);
            DateTime dt;
            if (null != timezone)
            {
               dt = TimeZoneInfo.ConvertTimeFromUtc(convertedDate, timezone);
            }
            else
            {
               dt = convertedDate.ToLocalTime();
            }
            formattedString = dt.ToString();
         }
         catch (Exception)
         {
            // fall through
         }

         return formattedString;
      }

      /// <summary>
      /// Convert a string to a DateTime object.
      /// </summary>
      /// <param name="dateTimeString">String that has a commond date/time format (eg ISO)</param>
      /// <returns>The DateTime object</returns>
      static public DateTime ToDateTime(this string dateTimeString)
      {
         DateTime dateTime = new DateTime();
         DateTime.TryParse(dateTimeString, out dateTime);
         return dateTime;
      }

      /// <summary>
      /// Get the minimum calculated thickness value from all the probes in the list.
      /// </summary>
      /// <param name="probes">List of AProbe objects</param>
      /// <returns>String representing the minimum calculated thickness</returns>
      static public string MinThickness(this IList<AProbe> probes)
      {
         try
         {
            List<AProbe> sortedProbes = new List<AProbe>(probes);
            sortedProbes.Sort(new Comparison<AProbe>(
               (AProbe probe1, AProbe probe2) =>
               {
                  double thickness1 = Convert.ToDouble(probe1.Thickness());
                  double thickness2 = Convert.ToDouble(probe2.Thickness());
                  return thickness1.CompareTo(thickness2);
               }
               ));
            return sortedProbes.First().Thickness();
         }
         catch (Exception)
         {
            return kDefaultNumberString;
         }
      }

      /// <summary>
      /// Get the maximum calculated thickness value from all the probes in the list.
      /// </summary>
      /// <param name="probes">List of AProbe objects</param>
      /// <returns>String representing the minimum calculated thickness</returns>
      static public string MaxThickness(this IList<AProbe> probes)
      {
         try
         {
            List<AProbe> sortedProbes = new List<AProbe>(probes);
            sortedProbes.Sort(new Comparison<AProbe>(
               (AProbe probe1, AProbe probe2) =>
               {
                  double thickness1 = Convert.ToDouble(probe1.Thickness());
                  double thickness2 = Convert.ToDouble(probe2.Thickness());
                  return thickness1.CompareTo(thickness2);
               }
               ));
            return sortedProbes.Last().Thickness();
         }
         catch (Exception)
         {
            return kDefaultNumberString;
         }
      }

      /// <summary>
      /// Get the mean calculated thickness value from all the probes in the list.
      /// </summary>
      /// <param name="probes">List of AProbe objects</param>
      /// <returns>String representing the minimum calculated thickness</returns>
      static public string MeanThickness(this IList<AProbe> probes)
      {
         try
         {
            double total = 0;
            foreach (AProbe probe in probes)
            {
               double thickness = Convert.ToDouble(probe.Thickness());
               total += thickness;
            }

            return Convert.ToString(total / (double)probes.Count);
         }
         catch (Exception)
         {
            return kDefaultNumberString;
         }
      }

      /// <summary>
      /// Get the calculated thickness from the first setup in the probe.
      /// </summary>
      /// <param name="probe">The AProbe object</param>
      /// <returns>String representing the minimum calculated thickness</returns>
      static public string Thickness(this AProbe probe)
      {
         if (probe.setups.Length > 0 && null != probe.setups[0])
         {
            return probe.setups[0].calculatedThickness;
         }
         else
         {
            return kDefaultNumberString;
         }
      }

      static public string LastThickness(this AProbe probe)
      {
         if (probe.setups.Length > 0 && null != probe.setups[0])
         {
            return probe.setups[0].lastThickness;
         }
         else
         {
            return kDefaultNumberString;
         }
      }

      /// <summary>
      /// Get the timestamp from probe with the min thickness.
      /// </summary>
      /// <param name="probes">List of AProbe objects</param>
      /// <returns>String representing the timestamp associated with the probe with the minimum thickness</returns>
      static public string Timestamp(this IList<AProbe> probes)
      {
         try
         {
            List<AProbe> sortedProbes = new List<AProbe>(probes);
            sortedProbes.Sort(new Comparison<AProbe>(
               (AProbe probe1, AProbe probe2) =>
               {
                  double thickness1 = Convert.ToDouble(probe1.Thickness());
                  double thickness2 = Convert.ToDouble(probe2.Thickness());
                  return thickness1.CompareTo(thickness2);
               }
               ));
            return sortedProbes.First().Timestamp();
         }
         catch (Exception)
         {
            return kDefaultDateString;
         }
      }

      /// <summary>
      /// Get the timestamp from the first setup in the probe.
      /// </summary>
      /// <param name="probe">The AProbe object</param>
      /// <returns>String representing the most recent timestamp</returns>
      static public string Timestamp(this AProbe probe)
      {
         if (probe.setups.Length > 0 && null != probe.setups[0])
         {
            return probe.setups[0].timestamp;
         }
         else
         {
            return kDefaultDateString;
         }
      }

      static public string LastTimestamp(this AProbe probe)
      {
         if (probe.setups.Length > 0 && null != probe.setups[0])
         {
            return probe.setups[0].lastTimestamp;
         }
         else
         {
            return kDefaultDateString;
         }
      }

      /// <summary>
      /// Get the alert state based on the calculated thickness value of all probes in the list.
      /// </summary>
      /// <param name="probes">The list of AProbes</param>
      /// <returns>Utils.AlertState</returns>
      static public AlertState CurrentAlertState(this IList<AProbe> probes)
      {
         AlertState state = AlertState.Normal;
         foreach (AProbe probe in probes)
         {
            AlertState newState = probe.CurrentAlertState();
            if (newState > state)
            {
               state = newState;
            }
         }
         return state;
      }

      /// <summary>
      /// Get the alert state based on the calculated thickness value of the probe.
      /// </summary>
      /// <param name="probes">The AProbe object</param>
      /// <returns>Utils.AlertState</returns>
      static public AlertState CurrentAlertState(this AProbe probe)
      {
         AlertState state = AlertState.Normal;
         string thicknessString = probe.Thickness();
         string minThicknessString = probe.minimumThickness;
         string nomThicknessString = probe.nominalThickness;
         string warnThicknessString = probe.warningThickness;
         if ("" != thicknessString && "" != minThicknessString && "" != nomThicknessString && "" != warnThicknessString)
         {
            double thickness = Convert.ToDouble(thicknessString);
            if (double.IsNaN(thickness))
            {
               thickness = -1;
            }
            double minThickness = Convert.ToDouble(minThicknessString);
            double nomThickness = Convert.ToDouble(nomThicknessString);
            double warnThickness = Convert.ToDouble(warnThicknessString);
            // less than min allowable thickness is an error
            if (thickness < minThickness)
            {
               state = AlertState.Error;
            }
            // less than warning thickness is a, well, warning
            else if (thickness < warnThickness)
            {
               state = AlertState.Warning;
            }
         }
         return state;
      }

      /// <summary>
      /// Get the location as a string for the given ACompany.
      /// </summary>
      /// <param name="ns">The ANanoSense object</param>
      /// <returns></returns>
      static public string LocationString(this ANanoSense ns)
      {
         return ns.company.name + ", " + ns.site.name;
      }

      /// <summary>
      /// Get the battery level for the given DSI as a string.
      /// </summary>
      /// <param name="dsi"></param>
      /// <returns></returns>
      static public string BatteryLevelString(this ADsiInfo dsi)
      {
         String batteryString = kNaString;

         if (null != dsi && null != dsi.probes[0] && null != dsi.probes[0].setups[0])
         {
            try
            {
               ASetup setup = dsi.probes[0].setups[0];
               int perc = (int)((float)(setup.status >> 12) / 15f * 100f);
               batteryString = "" + perc + "%";
            }
            catch (Exception)
            {
               // ignore
            }
         }

         return batteryString;
      }

      /// <summary>
      /// Get the battery level color for the given DSI.
      /// </summary>
      /// <param name="dsi"></param>
      /// <returns></returns>
      static public Color BatteryLevelColor(this ADsiInfo dsi)
      {
         Color batteryColor = Color.Green;

         if (null != dsi && null != dsi.probes[0] && null != dsi.probes[0].setups[0])
         {
            try
            {
               ASetup setup = dsi.probes[0].setups[0];
               int perc = (int)((float)(setup.status >> 12) / 15f * 100f);
               if (perc < 24)
               {
                  batteryColor = Color.Red;
               }
               else if (perc > 24 && perc < 50)
               {
                  batteryColor = Color.DarkGoldenrod;
               }
               else
               {
                  batteryColor = Color.Green;
               }
            }
            catch (Exception)
            {
               // ignore
            }
         }

         return batteryColor;
      }

      /// <summary>
      /// Get the DSI temperature for the given DSI as a string.
      /// </summary>
      /// <param name="dsi"></param>
      /// <returns></returns>
      static public string DsiTemperatureString(this ADsiInfo dsi)
      {
         String dsiTemp = kNaString;

         if (null != dsi && null != dsi.probes[0] && null != dsi.probes[0].setups[0])
         {
            try
            {
               ASetup setup = dsi.probes[0].setups[0];
               float cTemp = (float)Convert.ToDouble(setup.dsiTemp);
               if (cTemp < 900)
               {
                  float floatTemp = AUnitUtils.CelsiusToFahrenheit((float)Convert.ToDouble(setup.dsiTemp));
                  dsiTemp = Settings.Default.ThicknessIsMetric ? setup.dsiTemp + " °C" : String.Format("{0:0.##} °F", floatTemp);
               }
            }
            catch (Exception)
            {
               // ignore and use defaults
            }
         }

         return dsiTemp;
      }

      /// <summary>
      /// Get the material temperature for the given DSI as a string.
      /// </summary>
      /// <param name="dsi"></param>
      /// <returns></returns>
      static public string MaterialTemperatureString(this ADsiInfo dsi)
      {
         String materialTemp = kNaString;

         if (null != dsi && null != dsi.probes[0] && null != dsi.probes[0].setups[0])
         {
            try
            {
               ASetup setup = dsi.probes[0].setups[0];
               float cTemp = (float)Convert.ToDouble(setup.materialTemp);
               if (cTemp < 900)
               {
                  float floatTemp = AUnitUtils.CelsiusToFahrenheit(cTemp);
                  materialTemp = Settings.Default.ThicknessIsMetric ? setup.materialTemp + " °C" : String.Format("{0:0.##} °F", floatTemp);
               }
            }
            catch (Exception)
            {
               // ignore and use defaults
            }
         }

         return materialTemp;
      }

      /// <summary>
      /// Extract the list of ANanoSense objects that need to be saved - ie they have
      /// one or more probes attached to them.
      /// </summary>
      /// <param name="nanoList">The source ANanoSense list</param>
      /// <returns>New ANanoSense list</returns>
      static public List<ANanoSense> ToSaveList(this List<ANanoSense> nanoList)
      {
         List<ANanoSense> newNanos = new List<ANanoSense>();

         foreach (ANanoSense nano in nanoList)
         {
            if (null != nano.backingFilePath)
            {
               newNanos.Add(nano);
            }
         }

         return newNanos;
      }

      /// <summary>
      /// Convert the list of ANanoSense objects into its respective list of
      /// file paths (backingFilePath).
      /// </summary>
      /// <param name="nanoList">The source ANanoSense list</param>
      /// <returns>List of file paths (strings)</returns>
      static public List<string> ToFileList(this List<ANanoSense> nanoList)
      {
         List<string> fileList = new List<string>();

         foreach (ANanoSense nano in nanoList)
         {
            if (nano.Dsi.IsLoggerDsi())
            {
               continue;
            }
            fileList.Add(nano.backingFilePath);
         }

         return fileList;
      }

      /// <summary>
      /// Get the maximum address from the given list of nanos.
      /// </summary>
      /// <param name="nanoList"></param>
      /// <param name="maxIndex">Maximum index to check</param>
      /// <returns></returns>
      static public ushort MaxAddress(this List<ANanoSense> nanoList, int maxIndex=999)
      {
         ushort max = 0;
         int index = 0;

         foreach (ANanoSense nano in nanoList)
         {
            if (index > maxIndex)
            {
               break;
            }
            if (ADsiNetwork.kFactoryId == nano.Dsi.modbusAddress)
            {
               continue;
            }
            max = Math.Max(max, nano.Dsi.modbusAddress);
            ++index;
         }

         return max;
      }

      /// <summary>
      /// Is the given DSI (via ADsiInfo) a data logger DSI.
      /// </summary>
      /// <param name="dsi">ADsiInfo</param>
      /// <returns>true if a data logger DSI</returns>
      static public bool IsLoggerDsi(this ADsiInfo dsi)
      {
         return dsi.Model == "logger";
      }

      /// <summary>
      /// Is the given DSI (via ADsiInfo) a micropim DSI.
      /// </summary>
      /// <param name="dsi">ADsiInfo</param>
      /// <returns>true if a upim DSI</returns>
      static public bool IsUpimDsi(this ADsiInfo dsi)
      {
         return dsi.Model == "upim";
      }
   }
}

