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

using DsiApi;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabletApp.State;
using TabletApp.Utils;

namespace TabletApp.Api
{
   /// <summary>
   /// Class representing all DSIs in the network 
   /// </summary>
   public class AMasterDsiInfo
   {
      public ADsiNetwork Dsi { get; set; }
      public List<ANanoSense> DsiItems { get; set; }
      public List<ANanoSense> DsiItemsSansLogger
      {
         get
         {
            return this.DsiItems.Where(d => !d.Dsi.IsLoggerDsi()).ToList();
         }
      }
      public int CurrentDsi { get; set; }
      public Dictionary<int, List<int>> SelectedProbes;


      /// <summary>
      /// Default constructor
      /// </summary>
      public AMasterDsiInfo()
      {
         this.DsiItems = new List<ANanoSense>();
         this.SelectedProbes = new Dictionary<int,List<int>>();
         this.CurrentDsi = 0;
      }

      /// <summary>
      /// Clear our persistant global data stored in our state globals.
      /// </summary>
      public static void ClearGlobalData()
      {
         if (AStateController.Instance.GlobalData.ContainsKey("masterdsi-full"))
         {
            AStateController.Instance.GlobalData.Remove("masterdsi-full");
         }
         if (AStateController.Instance.GlobalData.ContainsKey("masterdsi"))
         {
            AStateController.Instance.GlobalData.Remove("masterdsi");
         }
      }

      /// <summary>
      /// Clone this for the given list of probe indexes, organized by DSI index and probe index.
      /// </summary>
      /// <param name="probeIndexes"></param>
      /// <returns>New AMasterDsiInfo object</returns>
      public AMasterDsiInfo CloneForSelectedProbes(Dictionary<int, List<int>> probeIndexes)
      {
         AMasterDsiInfo newMasterInfo = new AMasterDsiInfo();
         bool firstSet = false;

         newMasterInfo.Dsi = this.Dsi;
         for (int i = 0; i < this.DsiItems.Count; ++i)
         {
            ANanoSense newItem = this.DsiItems[i].ShallowCopy();
            newMasterInfo.DsiItems.Add(newItem);
            // if the dsi is included..
            if (probeIndexes.ContainsKey(i))
            {
               // set the "current" dsi for future states to the first one with probes
               if (!firstSet)
               {
                  newMasterInfo.CurrentDsi = i;
                  firstSet = true;
               }
               List<AProbe> probes = new List<AProbe>();
               for (int j = 0; j < this.DsiItems[i].Dsi.probes.Length; ++j)
               {
                  // if the probe is included..
                  if (probeIndexes[i].Contains(j))
                  {
                     probes.Add(this.DsiItems[i].Dsi.probes[j]);
                  }
               }
               newItem.Dsi.probes = probes.ToArray();
            }
         }

         return newMasterInfo;
      }
   }
}

