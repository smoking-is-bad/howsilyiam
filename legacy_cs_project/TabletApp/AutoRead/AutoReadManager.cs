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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabletApp.Api;
using TabletApp.Properties;
using TabletApp.State;
using TabletApp.Utils;
using DsiExtensions;

namespace TabletApp.AutoRead
{
   /// <summary>
   /// Manager class for handling auto-read state management.
   /// </summary>
   public class AAutoReadManager : ASingleton<AAutoReadManager>
   {
      public bool AutoReadActive { get; set; }

      /// <summary>
      /// Default constructor
      /// </summary>
      public AAutoReadManager()
      {
         AStateController.Instance.PerformActionEvent += new PerformAction(HandlePerformAction);
      }

      /// <summary>
      /// Handle any actions
      /// </summary>
      /// <param name="actionName"></param>
      /// <param name="data"></param>
      private void HandlePerformAction(string actionName, object data)
      {
         // timer elapsed, force a change of state back to "progress"
         if ("timerelapsed" == actionName && this.AutoReadActive)
         {
            if (!AStateController.Instance.ChangeToState("progress"))
            {
               // restart the timer if we couldn't change states
               this.StartTimer();
            }
         }
      }

      /// <summary>
      /// Set the auto-read state to active
      /// </summary>
      public void StartAutoRead()
      {
         // action listeners will handle the UI updates
         AStateController.Instance.PostPerformActionEvent("autoreadstate", true);
         this.AutoReadActive = true;

         // suppress error dialogs while in auto-read mode
         AOutput.SuppressErrorDialog = true;
      }

      /// <summary>
      /// Set the auto-read state to inactive.
      /// </summary>
      public void StopAutoRead()
      {
         // action listeners will handle the UI updates
         AStateController.Instance.PostPerformActionEvent("autoreadstate", false);
         this.AutoReadActive = false;
         AOutput.SuppressErrorDialog = false;
         this.StopTimer();
      }

      /// <summary>
      /// Start the auto-read timer
      /// </summary>
      public void StartTimer()
      {
         // action listeners will handle the UI updates
         AStateController.Instance.PostPerformActionEvent("starttimer", null);
      }

      /// <summary>
      /// Stop the auto-read timer
      /// </summary>
      public void StopTimer()
      {
         // action listeners will handle the UI updates
         AStateController.Instance.PostPerformActionEvent("stoptimer", null);
      }

      /// <summary>
      /// Perform an auto-read upload in the background.  We simply show the global spinner progress for this upload.
      /// </summary>
      public async Task PerformAutoUploadAsync(List<ANanoSense> dsis)
      {
         try
         {
            AStateController.Instance.PostPerformActionEvent("showspinnerprogress", Resources.AutoReadUploadProgress);
            // setup our cancelation token so we can cancel during the upload
            await AApiManager.Instance.UploadAsync(dsis);
         }
         catch (OperationCanceledException)
         {
            // silent fail
         }
         catch (Exception e)
         {
            AOutput.DisplayError(Resources.ErrorUpload + " " + e.AggregateMessage());
         }
         finally
         {
            AStateController.Instance.PostPerformActionEvent("hidespinnerprogress", null);
         }
      }
   }
}

