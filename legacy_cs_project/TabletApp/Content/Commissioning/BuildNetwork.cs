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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Model;
using TabletApp.Api;
using TabletApp.State;
using TabletApp.Utils;
using System.Diagnostics;
using TabletApp.Views;
using TabletApp.Properties;

namespace TabletApp.Content.Commissioning
{
   /// <summary>
   /// User control for the Build Network commissioning screen.
   /// </summary>
   public partial class BuildNetwork : BaseContent
   {
      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="parameters">Parameters from the state def file</param>
      public BuildNetwork(Dictionary<string, string> parameters)
         : base(parameters)
      {
         InitializeComponent();

         AStateController.Instance.StateWillChangeEvent += HandleStateWillChangeEvent;

         this.dsiGrid.LineVisible = false;
         this.dsiGrid.Selectable = false;
         Debug.Assert(AStateController.Instance.GlobalData.ContainsKey("masterdsi"));
         AMasterDsiInfo master = (AMasterDsiInfo)AStateController.Instance.GlobalData["masterdsi"];
         Debug.Assert(master.DsiItems.Count > 0);
         foreach (DsiButton dsiButton in this.dsiGrid.AllButtons())
         {
            if (dsiButton.Index >= master.DsiItems.Count)
            {
               break;
            }
            dsiButton.Selected = false;
            dsiButton.State = (dsiButton.Index > master.CurrentDsi ? DsiButtonState.Empty : DsiButtonState.Connected);
         }
         this.locationInfo.PopulateWithDsi(master.DsiItems[0]);
         // set our location info in the app header
         AStateController.Instance.PostPerformActionEvent("setlocation", master.DsiItems[0].LocationString());

         // don't allow add if we've commissioned all
         if (master.DsiItems.Count == master.CurrentDsi + 1)
         {
            AStateController.Instance.PostPerformActionEvent("disablebutton", "add");
         }
         else
         {
            AStateController.Instance.PostPerformActionEvent("disablebutton", "done");
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
      /// Handle state change.  Check for cancel action when changing state, giving user a chance to cancel that action.
      /// </summary>
      /// <param name="newState"></param>
      /// <param name="actionName"></param>
      /// <param name="args"></param>
      private void HandleStateWillChangeEvent(AState newState, string actionName, CancelEventArgs args)
      {
         if ("cancel" == actionName)
         {
            if (DialogResult.No == AOutput.DisplayYesNo(Resources.BuildNetworkCancelMessage, Resources.BuildNetworkCancelCaption))
            {
               args.Cancel = true;
            }
         }
      }
   }
}

