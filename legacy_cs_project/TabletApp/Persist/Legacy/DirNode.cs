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

namespace TabletApp.Persist.Legacy
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
      NumLevels
	}

   /// <summary>
   /// Class for building our directory tree structure in memory and maintaining the
   /// name-id mapping for each directory.  See <code>ADirStructure</code> for details.
   /// </summary>
   public class ADirNode
   {
      /// <summary>
      /// The base string for the mapped directory names.
      /// </summary>
      public readonly string[] MappingBase = {"Root", "C", "S", "P", "A", "CP"};

      public NodeLevel Level { get; private set; }
      public string Name { get; private set; }
      public string Mapping { get; private set; }
      public List<ADirNode> Children { get; private set; }
      public ADirNode Parent { get; private set; }

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="parent"></param>
      /// <param name="level"></param>
      /// <param name="name"></param>
      /// <param name="mapping"></param>
      public ADirNode(ADirNode parent, NodeLevel level, string name, string mapping)
      {
         this.Parent = parent;
         this.Level = level;
         this.Name = name;
         this.Mapping = mapping;
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
      /// Get the child node with the given mapped name.
      /// </summary>
      /// <param name="name"></param>
      /// <returns>ADirNode child</returns>
      public ADirNode GetChildByMapping(string mapping)
      {
         return this.Children.FirstOrDefault(s => s.Mapping == mapping);
      }

      /// <summary>
      /// Add a new child
      /// </summary>
      /// <param name="name">Mapped name</param>
      /// <param name="mapping">Dir name (must be non-null if level is null</param>
      /// <param name="level">Level (must be non-null if mapping is null)</param>
      /// <returns>The new ADirNode</returns>
      public ADirNode AddChild(string name, string mapping = null, NodeLevel? level = null)
      {
         // must specify either mapping or level
         Debug.Assert(null != mapping || null != level);
         ADirNode newNode = new ADirNode(
            this,
            (null == level ? this.LevelForMapping(mapping) : (NodeLevel)level), 
            name, 
            (null == mapping ? this.NextMapping((NodeLevel)level) : mapping));
         this.Children.Add(newNode);

         return newNode;
      }

      /// <summary>
      /// Get the corresponding level for the given mapping (C1 = NodeLevel.Company, S22 = NodeLevel.Site, etc).
      /// </summary>
      /// <param name="mapping">A full mapping</param>
      /// <returns>The corresponding NodeLevel</returns>
      private NodeLevel LevelForMapping(string mapping)
      {
         string baseMapping = this.BaseForMapping(mapping);
         int mappingIndex = Array.FindIndex(MappingBase, s => s == baseMapping);
         return (NodeLevel)mappingIndex;
      }

      /// <summary>
      /// Get the base (eg "CP") for the given mapping (eg "CP12").
      /// </summary>
      /// <param name="mapping">A full mapping</param>
      /// <returns>The mapping base</returns>
      private string BaseForMapping(string mapping)
      {
         // strip off the numbers
         return Regex.Replace(mapping, @"[\d-]", string.Empty);
      }

      /// <summary>
      /// Get the next child mapping according to the current structure.  The next mapping is based on the current
      /// count and makes the assumption that their are already Children.Count directories, numbered
      /// sequentially, which should be true barring any external meddling.
      /// </summary>
      /// <param name="level">The level of the children</param>
      /// <returns>The mapping string (dir name, eg "C1", "S22", etc)</returns>
      private string NextMapping(NodeLevel level)
      {
         if (this.Children.Count > 0)
         {
            string lastMapping = MappingBase[(int)level] + Convert.ToString(this.Children.Count);
            Debug.Assert(null != this.Children.FirstOrDefault(s => s.Mapping == lastMapping));
         }
         string newMapping = MappingBase[(int)level] + Convert.ToString(this.Children.Count + 1);
         Debug.Assert(null == this.Children.FirstOrDefault(s => s.Mapping == newMapping));
         return newMapping;
      }

      /// <summary>
      /// Get the mapped name for the given data directory, based on the .contents file therein.
      /// </summary>
      /// <param name="baseDir">Data directory fully qualified path</param>
      /// <returns>Mapped name (eg "My Company", "My Site", etc)</returns>
      private string GetNameForPath(string baseDir)
      {
         string mapping = Path.GetFileName(baseDir);
         string filePath = Path.Combine(baseDir, Resources.DirMappingFilename);
         
         if (File.Exists(filePath))
         {
            mapping = File.ReadAllText(filePath);
         }

         return mapping;
      }

      /// <summary>
      /// Create the .contents mapping file for the given directory.  This file contains the
      /// mapped name.
      /// </summary>
      /// <param name="baseDir"></param>
      private void CreateMappingFile(string baseDir)
      {
         string filePath = Path.Combine(baseDir, Resources.DirMappingFilename);

         if (!File.Exists(filePath))
         {
            File.WriteAllText(filePath, this.Name);
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
            string newName = this.GetNameForPath(dir);
            ADirNode newNode = this.AddChild(newName, Path.GetFileName(dir));
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
         // if we've gone beyond our max levels, just return the base directory
         if (level >= NodeLevel.NumLevels)
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
            child = this.AddChild(childName, null, newLevel);
            newBase = Path.Combine(baseDir, child.Mapping);
            Directory.CreateDirectory(newBase);
            child.CreateMappingFile(newBase);
         }
         // child already exists, so just get the directory path
         else
         {
            newBase = Path.Combine(baseDir, child.Mapping);
         }
         // create the structure for the next level
         newBase = child.CreateStructureIfNeeded(newBase, levelNames, child.Level + 1);

         return newBase;
      }

      /// <summary>
      /// Trace up the hierarchy and build the mapped (file system name) path starting
      /// from this node.
      /// </summary>
      /// <returns>The full path starting from the root node</returns>
      public string MappedPath()
      {
         string path = this.Mapping;
         ADirNode node = this;
         while (null != (node = node.Parent) && NodeLevel.Root != node.Level)
         {
            path = Path.Combine(node.Mapping, path);
         }

         return path;
      }

      /// <summary>
      /// Trace up the hierarchy and build the friendly (real name) path starting
      /// from this node.
      /// </summary>
      /// <returns>The friendly path starting from the root node</returns>
      public string FriendlyPath()
      {
         string path = this.Name;
         ADirNode node = this;
         while (null != (node = node.Parent) && NodeLevel.Root != node.Level)
         {
            path = node.Name + @"\" + path;
         }

         return path;
      }
   }
}

