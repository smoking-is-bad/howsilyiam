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
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TabletApp.State;
using TabletApp.Api;
using TabletApp.Utils;
using TabletApp.Properties;
using TabletApp.AutoRead;

namespace TabletApp.Content
{
   /// <summary>
   /// User control for the summary data view
   /// </summary>
   public partial class DataViewSummary : BaseContent
   {
      private AMasterDsiInfo fMasterDsiInfo;

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="parameters">Parameters from the state def file</param>
      public DataViewSummary(Dictionary<string, string> parameters)
         : base(parameters)
      {
         InitializeComponent();
         fMasterDsiInfo = (AMasterDsiInfo)AStateController.Instance.GlobalData["masterdsi"];
         this.summaryGrid.PopulateWithDsiList(fMasterDsiInfo.DsiItemsSansLogger);
         this.locationInfo.PopulateWithDsi(fMasterDsiInfo.DsiItems[0]);
         AStateController.Instance.StateWillChangeEvent += HandleStateWillChangeEvent;
         this.summaryGrid.SetLoggerState(fMasterDsiInfo.DsiItems[0].Dsi.IsLoggerDsi());


         var modeMatches = new Regex(@"logger|commissioning").Matches(this.ReadMode ?? "");
         if (modeMatches.Count != 0)
         {
            AStateController.Instance.PostPerformActionEvent("disablebutton", "back");
         }
      }

      /// <summary>
      /// About to disappear - clean up.
      /// </summary>
      public override void WillDisappear()
      {
         AStateController.Instance.StateWillChangeEvent -= HandleStateWillChangeEvent;
      }

      /// <summary>
      /// Handle state change during an auto-read.
      /// </summary>
      /// <param name="newState"></param>
      /// <param name="args"></param>
      private void HandleStateWillChangeEvent(AState newState, string actionName, CancelEventArgs args)
      {
         bool newStateOk = (null != newState && ("progress" == newState.Id || "dvascan" == newState.Id || "dvthickness" == newState.Id));
         
         // check if we are stopping an auto-read, giving the user a chance to cancel
         if ((!newStateOk && AAutoReadManager.Instance.AutoReadActive) && 
             DialogResult.No == AOutput.DisplayYesNo(Resources.AutoReadCancelMessage, Resources.AutoReadCancelCaption))
         {
            args.Cancel = true;
         }

         // if we are indeed changing states and stopping an auto-read as a result, force-stop the auto-read
         if (!args.Cancel && !newStateOk && AAutoReadManager.Instance.AutoReadActive)
         {
            AAutoReadManager.Instance.StopAutoRead();
         }
      }
   }
}

