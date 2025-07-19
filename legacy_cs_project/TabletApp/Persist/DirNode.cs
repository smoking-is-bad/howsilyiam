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
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TabletApp.Properties;

namespace TabletApp.Persist
{
   /// <summary>
   /// Enum for our different node levels, including Root.
   /// </summary>
   public enum NodeLevel
	{
	   Root,
      Company,
      Site,
      Plant,
      Asset,
      CollectionPoint,
      Data,
      NumLevels
	}

   /// <summary>
   /// Class for building our directory tree structure in memory.  A mapping file is maintained
   /// in order to persist its level enumeration (Company, Site, etc).
   /// See <code>ADirStructure</code> for details.
   /// </summary>
   public class ADirNode
   {
      public NodeLevel Level { get; private set; }
      public string Name { get; private set; }
      public List<ADirNode> Children { get; private set; }
      public ADirNode Parent { get; private set; }

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="parent"></param>
      /// <param name="level"></param>
      /// <param name="name"></param>
      /// <param name="mapping"></param>
      public ADirNode(ADirNode parent, NodeLevel level, string name)
      {
         this.Parent = parent;
         this.Level = level;
         this.Name = name;
         this.Children = new List<ADirNode>();
      }

      /// <summary>
      /// Check if a child node exists with the given name.
      /// </summary>
      /// <param name="name"></param>
      /// <returns>True if the child exists</returns>
      public bool ChildExists(string name)
      {
         return (null != this.Children.FirstOrDefault(s => s.Name == name));
      }

      /// <summary>
      /// Get the child node with the given name.
      /// </summary>
      /// <param name="name"></param>
      /// <returns>ADirNode child</returns>
      public ADirNode GetChild(string name)
      {
         return this.Children.FirstOrDefault(s => s.Name == name);
      }

      /// <summary>
      /// Add a new child
      /// </summary>
      /// <param name="name">Mapped name</param>
      /// <param name="level">Level</param>
      /// <returns>The new ADirNode</returns>
      public ADirNode AddChild(string name, NodeLevel? level = null)
      {
         // must specify either mapping or level
         Debug.Assert(null != level);
         ADirNode newNode = new ADirNode(this, (NodeLevel)level, name);
         this.Children.Add(newNode);

         return newNode;
      }

      /// <summary>
      /// Get the nodel level for the given data directory, based on the .meta file therein.
      /// </summary>
      /// <param name="baseDir">Data directory fully qualified path</param>
      /// <returns>NodeLevel</returns>
      private NodeLevel GetLevelForPath(string baseDir)
      {
         NodeLevel level = NodeLevel.Data;
         string filePath = Path.Combine(baseDir, Resources.DirMetaFilename);
         
         if (File.Exists(filePath))
         {
            try
            {
               level = (NodeLevel)int.Parse(File.ReadAllText(filePath));
            }
            catch (Exception)
            {
            }
         }

         return level;
      }

      /// <summary>
      /// Create the metadata file for the given directory.  This file contains the
      /// corresponding level for this directory.
      /// </summary>
      /// <param name="baseDir"></param>
      private void CreateMetadataFile(string baseDir)
      {
         string filePath = Path.Combine(baseDir, Resources.DirMetaFilename);

         if (!File.Exists(filePath))
         {
            File.WriteAllText(filePath, ((int)this.Level).ToString());
         }
      }

      /// <summary>
      /// Recusively load the mapping information based on the directory structure
      /// at the given directory.
      /// </summary>
      /// <param name="baseDir">The base directory</param>
      public void Load(string baseDir)
      {
         IEnumerable<string> subdirs = Directory.EnumerateDirectories(baseDir);

         foreach (string dir in subdirs)
         {
            NodeLevel level = this.GetLevelForPath(dir);
            ADirNode newNode = this.AddChild(Path.GetFileName(dir), level);
            newNode.Load(dir);
         }
      }

      /// <summary>
      /// Recursively create the needed directory structure (if it doesn't already exist) based on
      /// the given base directory, the NodeLevel.NumLevels-length list of level names, and the
      /// intended level for this node.
      /// </summary>
      /// <param name="baseDir">The base directory in which to create the structure</param>
      /// <param name="levelNames">List of level names for the mapping - must contain NodeLevel.NumLevels number of entries (some may be null)</param>
      /// <param name="level">The intended level for this node</param>
      /// <returns>The newly created (or existing), fully qualified directory path</returns>
      public string CreateStructureIfNeeded(string baseDir, IList<string> levelNames, NodeLevel level)
      {
         // if we've gone beyond our non-data levels, just return the base directory
         if (level >= NodeLevel.NumLevels - 1)
         {
            return baseDir;
         }

         // function to get the next non-null level name from the given list
         Func<string> NextLevelName = () =>
         {
            for (int i = (int)level; i < levelNames.Count; ++i)
            {
               if (null != levelNames[i] && levelNames[i].Length > 0)
               {
                  return levelNames[i];
               }
            }
            return null;
         };

         // levelNames must contain NodeLevel.NumLevels of entries
         Debug.Assert((int)NodeLevel.NumLevels == levelNames.Count);

         string childName = NextLevelName();
         string newBase = baseDir;
         ADirNode child = this.GetChild(childName);
         // if child does not yet exist, create it and its associated directory
         if (null == child)
         {
            List<string> tempList = new List<string>(levelNames);
            NodeLevel newLevel = (NodeLevel)tempList.IndexOf(childName, (int)level);
            child = this.AddChild(childName, newLevel);
            newBase = Path.Combine(baseDir, child.Name);
            Directory.CreateDirectory(newBase);
            child.CreateMetadataFile(newBase);
         }
         // child already exists, so just get the directory path
         else
         {
            newBase = Path.Combine(baseDir, child.Name);
         }
         // create the structure for the next level
         newBase = child.CreateStructureIfNeeded(newBase, levelNames, child.Level + 1);

         return newBase;
      }

      /// <summary>
      /// Trace up the hierarchy and build the relative file system path starting
      /// from this node, stopping at the root.
      /// </summary>
      /// <returns>The relative path starting from the root node</returns>
      public string RelativePathFromRoot()
      {
         string path = this.Name;
         ADirNode node = this;
         while (null != (node = node.Parent) && NodeLevel.Root != node.Level)
         {
            path = Path.Combine(node.Name, path);
         }

         return path;
      }
   }
}

