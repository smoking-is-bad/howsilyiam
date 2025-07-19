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
using System.Threading;
using Model;
using TabletApp.State;
using TabletApp.Api;
using TabletApp.Utils;
using System.IO;
using TabletApp.Properties;
using TabletApp.Email;
using DsiExtensions;
using TabletApp.Persist;

namespace TabletApp.Content
{
   public partial class Email : BaseContent
   {
      private List<string> fFileList;

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="parameters">Parameters from the state def file</param>
      public Email(Dictionary<string, string> parameters)
         : base(parameters)
      {
         InitializeComponent();

         AStateController.Instance.PerformActionEvent += HandlePerformAction;
         this.fileList.FileRemovedEvent += HandleFileRemoved;

         this.fileList.Mapper = (path) =>
         {
            return AFileManager.Instance.DirStructure.FriendlyPathForPath(path, ":");
         };

         AMasterDsiInfo info = (AMasterDsiInfo)AStateController.Instance.GlobalData["masterdsi"];
         fFileList = info.DsiItemsSansLogger.ToSaveList().ToFileList();
         // FileSelect will add csvs to globals, otherwise get the csv from the nano
         List<string> csvs = (AStateController.Instance.GlobalData.ContainsKey("emailcsvs") ? (List<string>)AStateController.Instance.GlobalData["emailcsvs"] : null);
         if (null != csvs)
         {
            fFileList.AddRange(csvs);
         }
         else if (null != info.DsiItems[0].csvFilePath)
         {
            fFileList.Add(info.DsiItems[0].csvFilePath);
         }
         this.fileList.PopulateFileTable(fFileList.ToArray());

         this.fromAddress.Text = Properties.Settings.Default.EmailFromAddress;
         this.subject.Text = Resources.EmailDefaultSubject;
         this.message.Text = Resources.EmailDefaultMessage;

         this.ValidateEmailSettings();
      }

      /// <summary>
      /// Validate the email settings from the Settings screen.  Display error if any are
      /// not set.
      /// </summary>
      /// <returns>True if valid</returns>
      private bool ValidateEmailSettings()
      {
         bool valid = true;

         if (null == Properties.Settings.Default.EmailFromAddress || 0 == Properties.Settings.Default.EmailFromAddress.Length ||
             null == Properties.Settings.Default.EmailSmtpServer || 0 == Properties.Settings.Default.EmailSmtpServer.Length ||
             0 == Properties.Settings.Default.EmailSmtpServerPort.ToString().Length ||
             null == Properties.Settings.Default.EmailUsername || 0 == Properties.Settings.Default.EmailUsername.Length ||
             null == Properties.Settings.Default.EmailPassword || 0 == Properties.Settings.Default.EmailPassword.Length)
         {
            AOutput.DisplayError(Resources.ErrorNoEmailSettings);
            valid = false;
         }
         return valid;
      }

      /// <summary>
      /// Validate the message properties.  Display an error if any are empty.
      /// </summary>
      /// <returns>True if valid</returns>
      private bool ValidateEmailProperties()
      {
         bool valid = true;

         if (0 == this.toAddress.Text.Length)
         {
            AOutput.DisplayError(Resources.ErrorEmailToMissing);
            valid = false;
         }
         if (0 == this.fromAddress.Text.Length)
         {
            AOutput.DisplayError(Resources.ErrorEmailFromMissing);
            valid = false;
         }
         return valid;
      }

      /// <summary>
      /// Remove items from our nano list as needed.
      /// </summary>
      /// <param name="fileIndex"></param>
      private void HandleFileRemoved(int fileIndex)
      {
         fFileList.RemoveAt(fileIndex);
      }

      /// <summary>
      /// About to disappear - clean up.
      /// </summary>
      public override void WillDisappear()
      {
         // unlisten for our events
         AStateController.Instance.PerformActionEvent -= HandlePerformAction;
      }

      /// <summary>
      /// Respond to Send button click.
      /// </summary>
      /// <param name="actionName"></param>
      /// <param name="data"></param>
      private void HandlePerformAction(string actionName, object data)
      {
         if ("send" == actionName)
         {
            if (this.ValidateEmailSettings() && this.ValidateEmailProperties())
            {
               Properties.Settings.Default.EmailFromAddress = this.fromAddress.Text;
               Task.Run(
                  () =>
                  {
                     this.DoSendEmail();
                  }
               );
            }
         }
      }

      /// <summary>
      /// Show/hide our progress spinner with associated text.
      /// </summary>
      /// <param name="complete">Did we complete the email send?</param>
      /// <param name="success">Was it a success?</param>
      private void ShowProgress(bool complete, bool success = false)
      {
         if (this.InvokeRequired)
         {
            this.Invoke(new Action(() => this.ShowProgress(complete, success)));
            return;
         }

         if (complete)
         {
            AStateController.Instance.PostPerformActionEvent("hidespinnerprogress", success ? Resources.EmailProgressSuccess : Resources.EmailProgressFail);
         }
         else
         {
            AStateController.Instance.PostPerformActionEvent("showspinnerprogress", Resources.EmailProgressSending);
         }
      }

      /// <summary>
      /// Send the email.
      /// </summary>
      private async void DoSendEmail()
      {
         try
         {
            this.ShowProgress(false);
            await AEmailManager.Instance.SendMessageAsync(
               Properties.Settings.Default.EmailSmtpServer,
               Properties.Settings.Default.EmailSmtpServerPort,
               Properties.Settings.Default.EmailUsername,
               Properties.Settings.Default.EmailPassword.DecryptString(),
               this.fromAddress.Text, 
               this.toAddress.Text, 
               this.subject.Text, 
               this.message.Text,
               fFileList);
            this.ShowProgress(true, true);
         }
         catch (Exception e)
         {
            this.ShowProgress(true, false);
            AOutput.DisplayError(Resources.ErrorSendEmail + " " + e.AggregateMessage());
         }
      }
   }
}

