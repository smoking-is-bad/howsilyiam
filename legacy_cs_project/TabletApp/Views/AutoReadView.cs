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
using TabletApp.Properties;
using TabletApp.State;
using TabletApp.Api;
using TabletApp.Utils;
using TabletApp.AutoRead;
using System.Threading;

namespace TabletApp.Views
{
   /// <summary>
   /// Controller class for the auto-read view, consisting of the auto-read indicator, the countdown timer,
   /// and the Stop button.
   /// </summary>
   public partial class AutoReadView : UserControl
   {
      private bool fStopped;
      private int fSecondsElapsed;

      /// <summary>
      /// Default constructor
      /// </summary>
      public AutoReadView()
      {
         InitializeComponent();
         AStateController.Instance.PerformActionEvent += new PerformAction(HandlePerformAction);

         Color clearColor = Color.FromArgb(0, 255, 255, 255);
         this.stopButton.FlatAppearance.BorderColor = clearColor;

         this.autoReadLabel.Visible = false;
         this.autoReadTimerText.Visible = false;
         this.stopButton.Visible = false;
         fStopped = false;
      }

      /// <summary>
      /// Handle any actions that update our state.
      /// </summary>
      /// <param name="actionName"></param>
      /// <param name="data"></param>
      private void HandlePerformAction(string actionName, object data)
      {
         if (this.InvokeRequired)
         {
            this.Invoke(new Action(() => this.HandlePerformAction(actionName, data)));
            return;
         }

         // show the auto-read indicator
         if ("autoreadstate" == actionName)
         {
            this.autoReadLabel.Visible = (bool)data;
         }
         // start the auto-read timer
         else if ("starttimer" == actionName)
         {
            this.autoReadTimerText.Visible = true;
            this.stopButton.Visible = true;
            fStopped = false;
            this.StartTimer();
         }
         // stop the auto-read timer
         else if ("stoptimer" == actionName)
         {
            this.autoReadTimerText.Visible = false;
            this.stopButton.Visible = false;
            fStopped = true;
         }
      }

      /// <summary>
      /// Start the auto-read timer
      /// </summary>
      private void StartTimer()
      {
         fSecondsElapsed = 0;
         this.UpdateAutoReadTimer(0);
         System.Timers.Timer theTimer = new System.Timers.Timer(1000);
         theTimer.Elapsed += TimerPing;
         theTimer.Start();
      }

      /// <summary>
      /// Ping from the timer - update our auto-read countdown
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void TimerPing(object sender, EventArgs e)
      {
         this.UpdateAutoReadTimer(++fSecondsElapsed);
         bool timeElapsed = fSecondsElapsed >= Settings.Default.AutoReadDelay;
         if (fStopped || timeElapsed)
         {
            ((System.Timers.Timer)sender).Stop();
            if (timeElapsed)
            {
               AStateController.Instance.PostPerformActionEvent("timerelapsed", null);
            }
         }
      }

      /// <summary>
      /// Update the auto-read timer per the given seconds ellapsed.
      /// </summary>
      /// <param name="secsEllapsed">The number of seconds that have ellapsed in the current wait period</param>
      private void UpdateAutoReadTimer(int secsEllapsed)
      {
         if (this.InvokeRequired)
         {
            this.Invoke(new Action(() => this.UpdateAutoReadTimer(secsEllapsed)));
            return;
         }

         TimeSpan span = TimeSpan.FromSeconds(Settings.Default.AutoReadDelay - secsEllapsed);
         this.autoReadTimerText.Text = String.Format("{0:00}:{1:00}:{2:00}", span.Hours, span.Minutes, span.Seconds);
      }

      /// <summary>
      /// User hit the Stop button - stop the auto-read session, after confirmation.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void stopButton_Click(object sender, EventArgs e)
      {
         if (DialogResult.Yes == AOutput.DisplayYesNo(Resources.AutoReadCancelMessage, Resources.AutoReadCancelCaption))
         {
            AAutoReadManager.Instance.StopAutoRead();
         }
      }
   }
}

