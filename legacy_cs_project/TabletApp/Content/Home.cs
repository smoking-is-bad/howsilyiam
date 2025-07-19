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
using System.Drawing;
using System.Windows.Forms;
using TabletApp.State;
using TabletApp.Utils;

namespace TabletApp.Content
{
   /// <summary>
   /// User control for the Home screen
   /// </summary>
   public partial class Home : BaseContent
   {
      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="parameters">Parameters from the state def file</param>
      public Home(Dictionary<string, string> parameters)
         : base(parameters)
      {
         InitializeComponent();
         // don't want our buttons to draw a border when in the background
         Color clearColor = Color.FromArgb(0, 255, 255, 255);
         this.reviewButton.FlatAppearance.BorderColor = clearColor;
         this.readSensorsButton.FlatAppearance.BorderColor = clearColor;
         this.uploadButton.FlatAppearance.BorderColor = clearColor;
         this.commissioningButton.FlatAppearance.BorderColor = clearColor;
         // clear out any location info
         AStateController.Instance.PostPerformActionEvent("setlocation", "");
         AStateController.Instance.PostPerformActionEvent("setinfo", "");
         // clear out any existing state vars
         AStateController.Instance.GlobalData.Clear();
         AStateController.Instance.GlobalData["ZoomScrollFromSettings"] = true;

         AddMultiPortScanButton();
      }

      /// <summary>
      /// Handle a click on the review button - change our state.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void reviewButton_Click(object sender, EventArgs e)
      {
         AStateController.Instance.GlobalData["readmode"] = "file";
         AStateController.Instance.ChangeToState("review");
      }

      /// <summary>
      /// Handle a click on the read sensors button - change our state.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void readSensorsButton_Click(object sender, EventArgs e)
      {
         AStateController.Instance.GlobalData["readmode"] = "sensor";
         AStateController.Instance.ChangeToState("hookup");
      }

      /// <summary>
      /// Handle a click on the upload button - change our state.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void uploadButton_Click(object sender, EventArgs e)
      {
         AStateController.Instance.ChangeToState("uploadselect");
      }

      private void emailButton_Click(object sender, EventArgs e)
      {
         AStateController.Instance.ChangeToState("emailselect");
      }

      /// <summary>
      /// Go to the main commissioning page.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void commissioningButton_Click(object sender, EventArgs e)
      {
         string currPwd = APasswordManager.Instance.GetCurrentPassword();

         // if we have a password, prompt for it and compare
         if (null != currPwd)
         {
            if (APasswordManager.Instance.ValidatePassword(currPwd))
            {
               AStateController.Instance.GlobalData["readmode"] = "commissioning";

               // if password is valid, move on to commissioning
               AStateController.Instance.ChangeToState("comm-main");
            }
         }
      }

      /// <summary>
      /// Handle Read Data Logger button click
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void readLoggerButton_Click(object sender, EventArgs e)
      {
         AStateController.Instance.GlobalData["readmode"] = "logger";
         AStateController.Instance.ChangeToState("readlogger");
      }

      /// <summary>
      /// Add a button for multi-port scanning.
      /// </summary>
      private void AddMultiPortScanButton()
      {
         // Create a new button for multi-port scanning
         var multiPortButton = new Button
         {
            Text = "Multi-Port Scan",
            Size = new Size(200, 40),
            Location = new Point(50, 300), // Adjust based on your layout
            Font = new Font("Arial", 10, FontStyle.Bold)
         };

         multiPortButton.Click += (sender, e) =>
         {
            AStateController.Instance.ChangeToState("multiportscan");
         };

         this.Controls.Add(multiPortButton);
      }
   }
}

