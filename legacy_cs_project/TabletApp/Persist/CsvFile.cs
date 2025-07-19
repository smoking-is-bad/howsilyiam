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
using System.Resources;

using TabletApp.Properties;

namespace TabletApp.Persist
{
   /// <summary>
   /// Class representing a newly created comma-separated-values file to which
   /// new values can be written.
   /// </summary>
   class ACsvFile : IDisposable
   {
      private StreamWriter fWriter;

      public ACsvFile(string path)
      {
         bool needsNewline = File.Exists(path) && !this.HasTerminatingNewline(path);
         fWriter = new StreamWriter(path, true);
         if (needsNewline)
         {
            this.AddLine();
         }
      }

      public void Dispose()
      {
         fWriter.Close();
      }

      /// <summary>
      /// Check to see if the given file is terminated by a newline sequence.
      /// </summary>
      /// <param name="path"></param>
      /// <returns>true if it ends in a newline</returns>
      private bool HasTerminatingNewline(string path)
      {
         bool hasIt = true;
         string newline = Environment.NewLine;
         using (FileStream stream = File.OpenRead(path))
         {
            stream.Seek(-1, SeekOrigin.End);
            int theByte = stream.ReadByte();
            int lastNewline = newline.Last();
            hasIt = theByte == lastNewline;
         }

         return hasIt;
      }

      /// <summary>
      /// Add a blank line.
      /// </summary>
      public void AddLine()
      {
         fWriter.WriteLine();
      }

      /// <summary>
      /// Add a line with a single item.
      /// </summary>
      /// <param name="item">The item string</param>
      /// <param name="quoteItem">Whether or not to put double quotes around the item (true by default)</param>
      public void AddLine(string item, bool quoteItem = true)
      {
         this.AddLine(new List<string> { item }, quoteItem);
      }

      /// <summary>
      /// Add a line with a series of items.  Items can be null, which will result in a zero-length item in the entry.
      /// </summary>
      /// <param name="items">List of string items to add (single items can be null to skip columns)</param>
      /// <param name="quoteItem">Whether or not to put double quotes around the item (true by default)</param>
      public void AddLine(List<string> items, bool quoteItems = true)
      {
         Debug.Assert(0 != items.Count);

         if (quoteItems)
         {
            items = items.ConvertAll(i => (null != i ? "\"" + i + "\"" : i));
         }

         string line;
         if (items.Count > 1)
         {
            line = String.Join(",", items);
         }
         else
         {
            line = items[0];
         }

         fWriter.WriteLine(line);
      }

      private static ResourceManager resourceMan;
      protected static ResourceManager ResourceManager
      {
         get
         {
            if (object.ReferenceEquals(resourceMan, null))
            {
               ResourceManager temp = new ResourceManager("TabletApp.Properties.Resources", typeof(Resources).Assembly);
               resourceMan = temp;
            }
            return resourceMan;
         }
      }
   }
}

