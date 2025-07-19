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
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabletApp.Api;
using TabletApp.Properties;
using TabletApp.State;
using TabletApp.Utils;
using DsiExtensions;

namespace TabletApp.Content.Commissioning
{
   /// <summary>
   /// User control for the Collect Data screen, where we test the connection to the DSI.
   /// </summary>
   public class CollectData : Message
   {
      private AMasterDsiInfo fMasterDsiInfo;

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="parameters">Parameters from the state def file</param>
      public CollectData(Dictionary<string, string> parameters)
         : base(parameters)
      {
         AStateController.Instance.StateWillChangeEvent += HandleStateWillChangeEvent;
         Debug.Assert(AStateController.Instance.GlobalData.ContainsKey("masterdsi"));
         fMasterDsiInfo = (AMasterDsiInfo)AStateController.Instance.GlobalData["masterdsi"];
         Debug.Assert(fMasterDsiInfo.DsiItems.Count > 0);
      }

      /// <summary>
      /// About to disappear - clean up.
      /// </summary>
      public override void WillDisappear()
      {
         AStateController.Instance.StateWillChangeEvent -= HandleStateWillChangeEvent;
      }

      /// <summary>
      /// Handle state change.  Read from the DSI and store in our master info.
      /// </summary>
      /// <param name="newState"></param>
      /// <param name="args"></param>
      private void HandleStateWillChangeEvent(AState newState, string actionName, CancelEventArgs args)
      {
         if ("collectdata" == actionName)
         {
            try
            {
               ANanoSense nano = fMasterDsiInfo.DsiItems[fMasterDsiInfo.CurrentDsi];
               AMarshaledDsiInfo dsiInfo = fMasterDsiInfo.Dsi.ReadDsiInfo((byte)nano.Dsi.modbusAddress);
               nano.Dsi.CopyFrom(dsiInfo);      // !!! TODO: is this necessary since we already have the dsi info from previous screens
               
               AStateController.Instance.PostPerformActionEvent("showspinnerprogress", Resources.PerformMeasurementProgress);
               Task task = Task.Run(
                   async () =>
                   {
                      await this.PerformMeasurements();
                   }
                );
               task.Wait();
               AStateController.Instance.PostPerformActionEvent("hidespinnerprogress", null);
            }
            catch (Exception e)
            {
               AOutput.DisplayError(Resources.ErrorCommReadDsiAndProbes + " " + e.AggregateMessage());
               args.Cancel = true;
            }
         }
      }

      /// <summary>
      /// Run a measurement on all of the probes on our current DSI.
      /// </summary>
      /// <returns></returns>
      private async Task PerformMeasurements()
      {
         ANanoSense nano = fMasterDsiInfo.DsiItems[fMasterDsiInfo.CurrentDsi];
         await AApiManager.Instance.PerformMeasurementsAsync(fMasterDsiInfo.Dsi, nano, DateTime.UtcNow, null,
            (AMeasurementParam param, AProbe probe, String errorMessage) =>
            {
            });
      }
   }
}

