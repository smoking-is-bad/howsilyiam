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
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using TabletApp.Properties;
using TabletApp.Utils;
using DsiUtil;

namespace TabletApp.Persist
{
   /// <summary>
   /// Class for handling loading and saving of our data files.
   /// </summary>
   public class AFileManager : ASingleton<AFileManager>
   {
      public ADirStructure DirStructure { get; private set; }

      /// <summary>
      /// Initialization.
      /// </summary>
      /// <param name="rootDir"></param>
      public void Initialize(string rootDir = null)
      {
         this.DirStructure = new ADirStructure(rootDir);
         this.MigrateLegacyStructure();
      }

      /// <summary>
      /// Migrate the old directory structure to the new.
      /// </summary>
      private void MigrateLegacyStructure()
      {
         String root = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), Resources.RootXmlFolderName_Legacy);

         if (Directory.Exists(root) && !Settings.Default.Migrated)
         {
            String errorString;

            // gather all SNI files
            var filePaths = new DirectoryInfo(root).GetFiles("*.sni", SearchOption.AllDirectories).Select(fi => fi.FullName).ToList();
            var nanos = this.Load(filePaths);
            this.Save(nanos, out errorString);
            Settings.Default.Migrated = true;
            Settings.Default.Save();
         }
      }

      /// <summary>
      /// Refresh the directory structure.
      /// </summary>
      public void Refresh()
      {
         string rootDir = this.DirStructure.RootDirectory;
         this.DirStructure = new ADirStructure(rootDir);
      }

      /// <summary>
      /// Save the list of DSIs to separate XML files.
      /// </summary>
      /// <param name="dsis">List of ANanoSense objects</param>
      /// <param name="errorString">An error, if one occurred</param>
      /// <returns>True if no errors</returns>
      public bool Save(List<ANanoSense> dsis, out String errorString)
      {
         var nanoGroups = new Dictionary<string, List<ANanoSense>>();
         errorString = null;
       
         foreach (ANanoSense nano in dsis)
         {
            String csvPath = null;
            if (this.Save(nano, out csvPath, out errorString))
            {
               if (!nanoGroups.Keys.Contains(csvPath))
               {
                  nanoGroups[csvPath] = new List<ANanoSense>();
               }
               nanoGroups[csvPath].Add(nano);
            }
         }

         this.SaveSummaryFile(nanoGroups, out errorString);

         // save the measurement CSV file if the setting is on
         if (Settings.Default.CsvLogSave)
         {
            this.SaveMeasurementCsvFiles(dsis, out errorString);
         }

         // return true if error free
         return null == errorString;
      }

      public bool Save(ANanoSense nano, out String csvPath, out String errorString)
      {
         errorString = null;
         csvPath = null;
         try
         {
            if (null != nano.Dsi.probes && nano.Dsi.probes.Length > 0)
            {
               nano.fVersion = ANanoSense.kMaxSupportedVersion;
               nano.testId = Guid.NewGuid().ToString();
               nano.testUser = AIdentity.LoggedInUsername();
               XmlSerializer writer = new XmlSerializer(typeof(ANanoSense));

               string nanoPath = this.DirStructure.PathForNano(nano);
               csvPath = Path.Combine(Path.GetDirectoryName(nanoPath), Resources.SummaryCsvFilename);
               nano.backingFilePath = nanoPath;
               nano.csvFilePath = csvPath;
               using (StreamWriter file = new StreamWriter(nanoPath))
               {
                  writer.Serialize(file, nano);
                  file.Close();
               }
            }
         }
         catch (Exception e)
         {
            AOutput.LogException(e);
            errorString = Resources.ErrorSaveXml;
         }
         return null == errorString;
      }

      /// <summary>
      /// Save an ASummaryFile for each key in nanoGroups containing the summary information for that set of DSIs.
      /// </summary>
      /// <param name="nanoGroups">maps CSV paths to a list of DSIs, each group is written as a summary file</param>
      public void SaveSummaryFile(Dictionary<string, List<ANanoSense>> nanoGroups, out String errorString)
      {
         errorString = null;
         foreach (string csvPath in nanoGroups.Keys)
         {
            try
            {
               var nanos = nanoGroups[csvPath];
               // save the CSV file - lives next to the other nano files
               using (ASummaryFile summaryFile = new ASummaryFile(nanos, csvPath))
               {
                  summaryFile.Save();
               }
            }
            catch (Exception e)
            {
               AOutput.LogException(e);
               errorString = Resources.ErrorSaveSummary;
            }
         }
      }

      /// <summary>
      /// Load the files into ANanoSense object and return the new list.
      /// </summary>
      /// <param name="filePaths">List of fully qualified file paths</param>
      /// <returns>List of ANanoSense objects</returns>
      public List<ANanoSense> Load(List<string> filePaths)
      {
         List<ANanoSense> nanos = new List<ANanoSense>();

         foreach (string filePath in filePaths)
         {
            try
            {
               var serializer = new XmlSerializer(typeof(ANanoSense));
               serializer.UnknownNode += (sender, args) =>
                  Console.WriteLine($"Unknown Node:{args.Name}\t{args.Text}");
               serializer.UnknownElement += (sender, args) =>
                  Console.WriteLine($"Unknown Element:{args.Element.Name}\t{args.Element.InnerText}");

               using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
               {
                  var nano = (ANanoSense)serializer.Deserialize(stream);
                  nano.Dsi.probeCount = (UInt16)nano.Dsi.probes.Length;
                  foreach (AProbe probe in nano.Dsi.probes)
                  {
                     probe.numSetups = (UInt16)probe.setups.Length;
                     foreach (ASetup setup in probe.setups)
                     {
                        setup.lastThickness = setup.calculatedThickness;
                        setup.lastTimestamp = setup.timestamp;
                     }
                  }
                  nano.backingFilePath = filePath;
                  nanos.Add(nano);
               }
            }
            catch (Exception e)
            {
               AOutput.LogException(e);
               // continue to next file
            }
         }

         return nanos;
      }

      /// <summary>
      /// Save the measurement CSV files for the given set of DSIs.
      /// </summary>
      /// <param name="nanos">The DSIs to save</param>
      /// <param name="errorString">null if successful, otherwise an error message</param>
      /// <returns>True if no errors while saving</returns>
      public bool SaveMeasurementCsvFiles(List<ANanoSense> nanos, out string errorString)
      {
         errorString = null;
         if (Settings.Default.CsvLogSave)
         {
            foreach (ANanoSense nano in nanos)
            {
               if (!this.SaveMeasurementCsvFiles(nano))
               {
                  errorString = Resources.ErrorSaveMeasurementCsv;
                  return false;
               }
            }
         }
         return true;
      }

      public bool SaveMeasurementCsvFiles(ANanoSense nano)
      {
         bool success = true;
         try
         {
            String baseDir = this.DirStructure.BaseDirForNano(nano);
            String dsiFilePath = Path.Combine(baseDir, Resources.MeasurementDsiFileName);
            using (AMeasurementFile dsiFile = new AMeasurementFile(dsiFilePath))
            {
               dsiFile.AppendDsi(nano);
            }
            foreach (AProbe probe in nano.Dsi.probes)
            {
               String probeFileName = String.Format(Resources.MeasurementProbeFileName, probe.num);
               String probeFilePath = Path.Combine(baseDir, probeFileName);
               using (AMeasurementFile probeFile = new AMeasurementFile(probeFilePath))
               {
                  probeFile.AppendProbe(probe, nano);
               }
            }
         }
         catch (Exception e)
         {
            AOutput.LogException(e);
            success = false;
         }
         return success;
      }
   }
}

