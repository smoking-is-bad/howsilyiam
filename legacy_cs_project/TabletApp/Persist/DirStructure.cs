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

namespace TabletApp.Persist
{
   /// <summary>
   /// Class to handle loading and creating the save directory structure.
   /// 
   /// The directory hierarchy is maintained according to this spec, where the numbers indicate the maximum length of the given path component:
   /// 
   ///   Base path	   25	c:\Users\Public\Documents
   ///   backslash	   1	
   ///   app id         3	"SNI"
   ///   backslash	   1	
   ///   Company	      48	
   ///   backslash	   1	
   ///   Site	         24	
   ///   backslash	   1	
   ///   Plant	         24	
   ///   backslash	   1	
   ///   Asset	         24	
   ///   backslash	   1	
   ///   Collection pt  24	
   ///   backslash	   1	
   ///   DSI SN/TAG	   24	
   ///   backslash	   1	
   ///   Date	         19	yyyy-mm-ddThh:mm:ss
   ///   backslash      1	
   ///   filename	      31	"XX+" + sn/tn + ".sni"
   ///   Trailing null	1	
   /// 
   /// for a total of 256 max characters.
   /// 
   /// Since all of the components of a given path (company, site, plant, asset, collection point) are not required, we need to store some
   /// metadata information that relates the corresponding level for that path component.  This is stored in a hidden .meta file inside each directory.
   ///
   /// Example:
   ///
   /// \National Fuel Company  
   ///    .meta = 1
   ///    \Erie Site
   ///       .meta = 2
   ///       \Millcreek Plant
   ///          .meta = 3
   ///          \Presque Isle Asset
   ///             .meta = 4
   ///             \Test Board Collection Point
   ///                .meta = 5
   ///                \DSI_addr+DSI_Tag_or_SN
   ///                   alltmls.csv
   ///                   tml1.csv
   ///                   tml2.csv
   ///                   \YYYY-MM-DD_hh-mm-ss
   ///                      summary.csv
   ///                      DSI_addr+DSI_Tag_or_SN.sni
   ///                   \YYYY-MM-DD_hh-mm-ss
   ///                      summary.csv
   ///                      DSI_addr+DSI_Tag_or_SN.sni
   /// 
   /// where the full path to an SNI file would look like this:
   /// 
   /// C:\Users\Public\Documents\SNI\National Fuel Company\Erie Site\Millcreek Plant\Presque Isle Asset\Test Board Collection Point\1+PI-DSI00003\2015-04-24_19-43-50\1+PI-DSI00003.sni
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
         this.RootDirectory = (null != rootDir ? rootDir : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), Resources.RootXmlFolderName));
         Directory.CreateDirectory(this.RootDirectory);
         // create our root node and load the hierarchy
         this.RootNode = new ADirNode(null, NodeLevel.Root, "root");
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
                  levelNames[(int)level] = ((string)nano.company.name).Truncate(48);
                  break;
               case NodeLevel.Site:
                  levelNames[(int)level] = ((string)nano.site.name).Truncate(24);
                  break;
               case NodeLevel.Plant:
                  levelNames[(int)level] = ((string)nano.plant.name).Truncate(24);
                  break;
               case NodeLevel.Asset:
                  levelNames[(int)level] = ((string)nano.asset.name).Truncate(24);
                  break;
               case NodeLevel.CollectionPoint:
                  levelNames[(int)level] = ((string)nano.collectionPoint.name).Truncate(24);
                  break;
               case NodeLevel.Data:
                  levelNames[(int)level] = "data";
                  break;
            }
         }

         return levelNames;
      }

      /// <summary>
      /// Get the filename for the given ANanoSense object.  See <code>BaseNameForNano</code>.
      /// </summary>
      /// <param name="nano">The ANanoSense object</param>
      /// <returns>The filename string</returns>
      private string FilenameForNano(ANanoSense nano)
      {
         return this.BaseNameForNano(nano).Truncate(28) + ".sni";
      }

      /// <summary>
      /// Get the base name for a DSI, that is comprised of its address and tag or serial number (with preference to tag).
      /// Format is [address]+[tag_or_serialnumber].sni
      /// </summary>
      /// <param name="nano"></param>
      /// <returns></returns>
      private string BaseNameForNano(ANanoSense nano)
      {
         string tagSn = ((null != nano.Dsi.tag) && (nano.Dsi.tag.Length > 0) ? (string)nano.Dsi.tag : (string)nano.Dsi.serialNumber);
         return (nano.Dsi.modbusAddress + "+" + tagSn);
      }

      /// <summary>
      /// Get the base directory for the given ANanonSense object.s
      /// </summary>
      /// <param name="nano"></param>
      /// <returns></returns>
      public string BaseDirForNano(ANanoSense nano)
      {
         string[] levelNames = this.GetLevelNamesForNano(nano);
         // create the directory structure, if needed, according to the level names associated with the ANanoSense object
         string newDir = this.RootNode.CreateStructureIfNeeded(this.RootDirectory, levelNames, NodeLevel.Company);
         newDir = Path.Combine(newDir, this.BaseNameForNano(nano).Truncate(24));
         Directory.CreateDirectory(newDir);

         return newDir;
      }

      /// <summary>
      /// Get the XML file path for the ANanoSense object given.
      /// </summary>
      /// <param name="nano">The ANanoSense object</param>
      /// <returns>The fully qualified path to the .sni file</returns>
      public string PathForNano(ANanoSense nano)
      {
         string baseDir = this.BaseDirForNano(nano);
         string newDir = Path.Combine(baseDir, nano.Dsi.probes.Timestamp().SafeFilename());
         Directory.CreateDirectory(newDir);
         string filename = this.FilenameForNano(nano);
         string path = Path.Combine(newDir, filename);

         return path;
      }

      /// <summary>
      /// Given a file system path that is part of our dir structure, return the friendly path
      /// that starts at our base node.  
      /// EG "C:\Users\Public\Documents\SNI\CompanyName\SiteName\PlantName\AssetName\CollectionPointName\1+PI-DSI00003\2015-04-24_19-43-50\1+PI-DSI00003.sni"
      /// might map to "CompanyName:SiteName:PlantName:AssetName:CollectionPointName:1+PI-DSI00003:2015-04-24_19-43-50:1+PI-DSI00003.sni"
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
            child = node.GetChild(prefix);
            if (null != child)
            {
               node = child;
            }
         } while (null != child);

         friendlyPath = node.RelativePathFromRoot() + @"\" + subPath;
         if (null != separator)
         {
            friendlyPath = friendlyPath.Replace(@"\", separator);
         }

         return friendlyPath;
      }
   }
}

