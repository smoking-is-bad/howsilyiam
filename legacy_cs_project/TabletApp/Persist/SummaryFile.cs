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
using TabletApp.Utils;
using System;
using System.Collections.Generic;

namespace TabletApp.Persist
{
   /// <summary>
   /// Domain-specific CSV file for the measurement summary file.  Write out all the pertinent info
   /// from the given list of ANanoSense objects.
   /// </summary>
   class ASummaryFile : ASniCsvFile
   {
      private List<ANanoSense> fNanos;

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="nanos">The list of ANanoSense objects to write out</param>
      /// <param name="path">The fully qualified path to the new CSV file (will be created)</param>
      public ASummaryFile(List<ANanoSense> nanos, string path) : base(path)
      {
         fNanos = nanos;
      }

      /// <summary>
      /// Write out all the info from the ANanoSense objects
      /// </summary>
      public void Save()
      {
         this.WriteCompany();
         this.AddLine();
         this.WriteDsiInfo();
         this.AddLine();
         this.WriteProbes();
      }

      /// <summary>
      /// Write out the company info, based off of the first ANanoSense object.
      /// </summary>
      private void WriteCompany()
      {
         this.AddLine(fNanos[0].company.name);
         this.AddLine(fNanos[0].company.location.address1);
         if (null != fNanos[0].company.location.address2 && fNanos[0].company.location.address2.Length > 0)
         {
            this.AddLine(fNanos[0].company.location.address2);
         }
         this.AddLine(fNanos[0].company.location.city + ", " + fNanos[0].company.location.state + " " + fNanos[0].company.location.postalCode);
         this.AddLine(fNanos[0].company.location.country);
         this.AddLine(fNanos[0].company.phone);
      }

      /// <summary>
      /// Get the alarm state as a string for all of the DSIs.
      /// </summary>
      /// <returns>"Yes" for alarmed, "No" for normal</returns>
      private string GetAlarmString()
      {
         AlertState state = AlertState.Normal;
         foreach (ANanoSense nano in fNanos)
         {
            state = nano.Dsi.probes.CurrentAlertState();
            if (AlertState.Normal != state)
            {
               break;
            }
         }

         return (AlertState.Normal != state 
            ? ResourceManager.GetString("ButtonYes")
            : ResourceManager.GetString("ButtonNo"));
      }

      /// <summary>
      /// Get the minimum thickness (as a string) for all of the DSIs.
      /// </summary>
      /// <returns>Minimum thickness string</returns>
      private string GetMinThickness()
      {
         double minThickness = Double.MaxValue;

         foreach (ANanoSense nano in fNanos)
         {
            double nanoMin = Convert.ToDouble(nano.Dsi.probes.MinThickness());
            // ignore invalid thickness values
            if (!Double.IsNaN(nanoMin))
            {
               minThickness = Math.Min(nanoMin, minThickness);
            }
         }

         return this.FormatMeasurementString(Convert.ToString(Double.MaxValue == minThickness ? Double.NaN : minThickness));
      }

      /// <summary>
      /// Get the maximum thickness (as a string) for all of the DSIs.
      /// </summary>
      /// <returns>Maximum thickness string</returns>
      private string GetMaxThickness()
      {
         double maxThickness = Double.MinValue;

         foreach (ANanoSense nano in fNanos)
         {
            double nanoMax = Convert.ToDouble(nano.Dsi.probes.MaxThickness());
            // ignore invalid thickness values
            if (!Double.IsNaN(nanoMax))
            {
               maxThickness = Math.Max(nanoMax, maxThickness);
            }
         }

         return this.FormatMeasurementString(Convert.ToString(Double.MinValue == maxThickness ? Double.NaN : maxThickness));
      }

      /// <summary>
      /// Get the mean thickness (as a string) for all of the DSIs.
      /// </summary>
      /// <returns>Mean thickness string</returns>
      private string GetMeanThickness()
      {
         double meanThickness = 0;
         int validNanos = 0;

         foreach (ANanoSense nano in fNanos)
         {
            double nanoMean = Convert.ToDouble(nano.Dsi.probes.MeanThickness());
            // ignore invalid thickness values
            if (!Double.IsNaN(nanoMean))
            {
               meanThickness += nanoMean;
               ++validNanos;
            }
         }

         return this.FormatMeasurementString(Convert.ToString(meanThickness / (double)validNanos));
      }

      /// <summary>
      /// Get the total probe count for all of the DSIs.
      /// </summary>
      /// <returns>Probe count string</returns>
      private string GetProbeCount()
      {
         int numProbes = 0;
         fNanos.ForEach(p => numProbes = numProbes + p.Dsi.probes.Length);

         return Convert.ToString(numProbes);
      }

      /// <summary>
      /// Format a translated string for use as a heading in the preample of the summary.
      /// </summary>
      private static string Heading(string resourceKey)
      {
         return ResourceManager.GetString(resourceKey) + ":";
      }

      /// <summary>
      /// Write out the DSI info.  Take it from the first ANanoSense object.
      /// </summary>
      private void WriteDsiInfo()
      {
         var rm = ResourceManager;
         this.AddLine(new List<string>
         {
            null,
            Heading("SiteTitle"), fNanos[0].site.name,
            Heading("ThicknessAlarmTitle"), this.GetAlarmString()
         });
         this.AddLine(new List<string>
         {
            null,
            Heading("PlantTitle"), fNanos[0].plant.name,
            Heading("SystemAlarmTitle"),  // TODO: display the system alarm (this was here when I did the translation work JDB 2019-03-08)
         });     
         this.AddLine(new List<string>
         {
            null,
            Heading("AssetTitle"), fNanos[0].asset.name
         });
         this.AddLine(new List<string>
         {
            null,
            Heading("CollectionPointIDTitle"), fNanos[0].collectionPoint.name,
            null, ResourceManager.GetString("StatisticsTitle")
         });
         this.AddLine(new List<string>
         {
            null,
            Heading("DescriptionTitle"), fNanos[0].collectionPoint.description,
            null, Heading("MinimumTitle"), this.GetMinThickness() 
         });
         this.AddLine(new List<string>
         {
            null,
            "GPS:", fNanos[0].collectionPoint.location.coordinates,
            null, Heading("MaximumTitle"), this.GetMaxThickness()
         });
         this.AddLine(new List<string>
         {
            null,
            Heading("DsiCountTitle"), Convert.ToString(fNanos[0].Dsi.dsiCount),
            null, Heading("MeanTitle"), this.GetMeanThickness()
         });
         this.AddLine(new List<string>
         {
            null,
            Heading("ProbeCountTitle"), this.GetProbeCount()
         });
      }

      /// <summary>
      /// Write out the probe information for all probes in the DSIs.
      /// </summary>
      private void WriteProbes()
      {
         // header
         this.WriteHeader();
         foreach (ANanoSense nano in fNanos)
         {
            foreach (AProbe probe in nano.Dsi.probes)
            {
               this.AppendProbe(probe, nano);
            }
         }
      }
   }
}

