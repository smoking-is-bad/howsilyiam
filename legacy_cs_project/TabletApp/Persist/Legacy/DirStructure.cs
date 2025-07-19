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
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TabletApp.Properties;
using TabletApp.Utils;
using DsiUtil;

namespace TabletApp.Persist.Legacy
{
   /// <summary>
   /// Class to handle loading and creating the save directory structure.
   /// 
   /// A simple 2 or 3 character mapping is used based on Company, Site, Plant, Asset, CollectionPoint for the ANanoSense object.
   /// All of these are not guarenteed to exist for a given nano object, so the hierarchy is at most 5 levels deep from the root.
   /// For each measurement (represented by the ANanoSense object), a final directory is created based on that measurement's
   /// timestamp. Each directory contains a .contents file that defines the mapped name for that directory.
   /// 
   /// Mapping ids:
   ///
   /// C: Company
   /// S: Site
   /// P: Plant
   /// A: Asset
   /// CP: Collection Point
   ///
   /// Example:
   ///
   /// \C1   
   ///    .contents = "National Fuel"
   ///    \S1
   ///       .contents = "Erie Site"
   ///       \P1
   ///          .contents = "Millcreek Plant"
   ///          \A1
   ///             .contents = "Presque Isle"
   ///             \CP1
   ///                .contents = "Test Board"
   ///                \YYYY-MM-DD_hh-mm-ss
   ///                   summary.csv
   ///                   DSI_addr+DSI_SN.sni
   ///                   DSI_addr+DSI_SN.sni
   ///                   (etc)
   ///                \YYYY-MM-DD_hh-mm-ss
   ///                   (a different set of test results)
   ///                   
   /// </summary>
   public class ADirStructure
   {
      public string RootDirectory { get; private set; }

      public ADirNode RootNode { get; private set; }

      /// <summary>
      /// Load the directory structure into our ADirNode objects.
      /// </summary>
      public ADirStructure(string rootDir = null)
      {
         this.RootDirectory = (null != rootDir ? rootDir : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), Resources.RootXmlFolderName_Legacy));
         Directory.CreateDirectory(this.RootDirectory);
         // create our root node and load the hierarchy
         this.RootNode = new ADirNode(null, NodeLevel.Root, "root", "root");
         this.RootNode.Load(this.RootDirectory);
      }

      /// <summary>
      /// Get an array of names for the needed directory structure.  This will be a fixed size array, fixed
      /// at the maximum number of levels, per Root/Company/Site/Plant/Asset/CollectionPoint.
      /// </summary>
      /// <param name="nano">The ANanoSense object</param>
      /// <returns></returns>
      private string[] GetLevelNamesForNano(ANanoSense nano)
      {
         string[] levelNames = new string[(int)NodeLevel.NumLevels];
         
         foreach (NodeLevel level in Enum.GetValues(typeof(NodeLevel)))
         {
            // get the associated data for each level
            switch (level)
            {
               case NodeLevel.Root:
                  levelNames[(int)level] = "root";    // need to include the root so that ADirNode can index into this array properly
                  break;
               case NodeLevel.Company:
                  levelNames[(int)level] = nano.company.name;
                  break;
               case NodeLevel.Site:
                  levelNames[(int)level] = nano.site.name;
                  break;
               case NodeLevel.Plant:
                  levelNames[(int)level] = nano.plant.name;
                  break;
               case NodeLevel.Asset:
                  levelNames[(int)level] = nano.asset.name;
                  break;
               case NodeLevel.CollectionPoint:
                  levelNames[(int)level] = nano.collectionPoint.name;
                  break;
            }
         }

         return levelNames;
      }

      /// <summary>
      /// Get the filename for the given ANanoSense object.
      /// Format is [address]+[serialnumber].sni
      /// </summary>
      /// <param name="nano">The ANanoSense object</param>
      /// <returns>The filename string</returns>
      private string FilenameForNano(ANanoSense nano)
      {
         return nano.Dsi.modbusAddress + "+" + nano.Dsi.serialNumber + ".sni";
      }

      /// <summary>
      /// Get the XML file path for the ANanoSense object given.
      /// </summary>
      /// <param name="nano">The ANanoSense object</param>
      /// <returns>The fully qualified path to the .sni file</returns>
      public string PathForNano(ANanoSense nano)
      {
         string[] levelNames = this.GetLevelNamesForNano(nano);
         // create the directory structure, if needed, according to the level names associated with the ANanoSense object
         string newDir = this.RootNode.CreateStructureIfNeeded(this.RootDirectory, levelNames, NodeLevel.Company);
         newDir = Path.Combine(newDir, nano.Dsi.probes.Timestamp().SafeFilename());
         Directory.CreateDirectory(newDir);
         string filename = this.FilenameForNano(nano);
         string path = Path.Combine(newDir, filename);

         return path;
      }

      /// <summary>
      /// Given a file system path that is part of our dir structure, return the friendly path
      /// that starts at our base node.  EG "C:\ProgramData\SensortNetworks\C1\S1\P1\A1\SP1\2015-04-24T19-43-50\1+PI-DSI00003.sni"
      /// might map to "National Fuel:Millcreek Plant:Presque Isle:Test Board:2015-04-24T19-43-50:1+PI-DSI00003.sni"
      /// </summary>
      /// <param name="path">File system path</param>
      /// <returns>Friendly path</returns>
      public string FriendlyPathForPath(string path, string separator = null)
      {
         string friendlyPath = "";

         if (!path.StartsWith(this.RootDirectory))
         {
            return friendlyPath;
         }

         string prefix = this.RootDirectory;
         ADirNode node = this.RootNode;
         ADirNode child = null;
         string subPath = path;
         do
         {
            subPath = subPath.Remove(0, subPath.IndexOf(prefix) + prefix.Length + 1);
            prefix = subPath.GetRootFolder();
            child = node.GetChildByMapping(prefix);
            if (null != child)
            {
               node = child;
            }
         } while (null != child);

         friendlyPath = node.FriendlyPath() + @"\" + subPath;
         if (null != separator)
         {
            friendlyPath = friendlyPath.Replace(@"\", separator);
         }

         return friendlyPath;
      }
   }
}

