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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TabletApp.Properties;

namespace TabletApp.Persist
{
   /// <summary>
   /// Measurement CSV file.
   /// </summary>
   class AMeasurementFile : ASniCsvFile
   {
      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="path">The fully qualified path to the CSV file (will be created or appended)</param>
      public AMeasurementFile(String path) : base(path)
      {
         if (0 == new FileInfo(path).Length)
         {
            this.WriteHeader();
         }
         else
         {
            // create the .bak file
            this.CreateBackup(path);
         }
      }

      /// <summary>
      /// Create a backup file prior to writing a new one.
      /// </summary>
      /// <param name="path"></param>
      private void CreateBackup(String path)
      {
         // create a backup
         String backup = path + ".bak";
         File.Delete(backup);
         File.Copy(path, backup);
      }

      /// <summary>
      /// Write out the DSI information for the given DSI object.
      /// </summary>
      public void AppendDsi(ANanoSense nano)
      {
         foreach (AProbe probe in nano.Dsi.probes)
         {
            this.AppendProbe(probe, nano);
         }
      }
   }
}

