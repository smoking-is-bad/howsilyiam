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
using TabletApp.State;
using TabletApp.Api;
using System.Threading;
using TabletApp.Utils;
using TabletApp.Properties;
using System.IO;
using Model;
using DsiExtensions;
using TabletApp.Persist;

namespace TabletApp.Content
{
   /// <summary>
   /// User control for the upload screen.  This will retrieve the current list of ANanoSense objects from the state globals
   /// and perform the upload on those objects.  It assumes the objects have already been saved (ie have their <code>backingFilePath</code>
   /// ivar set).
   /// </summary>
   public partial class Upload : BaseContent
   {
      /// <summary>
      /// Handle cancellation of our async ops
      /// </summary>
      private CancellationTokenSource fCancelTokenSource;

      private List<ANanoSense> fNanoList;

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="parameters">Parameters from the state def file</param>
      public Upload(Dictionary<string, string> parameters)
         : base(parameters)
      {
         InitializeComponent();

         AStateController.Instance.StateWillChangeEvent += HandleStateWillChangeEvent;
         AStateController.Instance.PerformActionEvent += HandlePerformAction;
         this.fileList.FileRemovedEvent += HandleFileRemoved;

         this.fileList.Mapper = (path) =>
         {
            return AFileManager.Instance.DirStructure.FriendlyPathForPath(path, ":");
         };

         AMasterDsiInfo info = (AMasterDsiInfo)AStateController.Instance.GlobalData["masterdsi"];
         fNanoList = info.DsiItemsSansLogger.ToSaveList();
         List<string> files = fNanoList.ToFileList();
         this.fileList.PopulateFileTable(files.ToArray());
         AStateController.Instance.PostPerformActionEvent("disablebutton", "done");
         Task.Run(
            () =>
            {
               this.DoUpload();
            }
         );
      }

      /// <summary>
      /// Remove items from our nano list as needed.
      /// </summary>
      /// <param name="fileIndex"></param>
      private void HandleFileRemoved(int fileIndex)
      {
         fNanoList.RemoveAt(fileIndex);
      }

      /// <summary>
      /// About to disappear - clean up.
      /// </summary>
      public override void WillDisappear()
      {
         // dispose of our progress bar so the containing user control can get disposed
         this.progressBar.Dispose();

         // unlisten for our events
         AStateController.Instance.PerformActionEvent -= HandlePerformAction;
         AStateController.Instance.StateWillChangeEvent -= HandleStateWillChangeEvent;
         if (null != fCancelTokenSource)
         {
            // cancel out of any async ops
            fCancelTokenSource.Cancel();
         }
      }

      /// <summary>
      /// Respond to Cloud and Email button clicks.
      /// </summary>
      /// <param name="actionName"></param>
      /// <param name="data"></param>
      private void HandlePerformAction(string actionName, object data)
      {
         if ("done" == actionName)
         {
            AStateController.Instance.ChangeToState("home");
         }
      }

      /// <summary>
      /// Perform the upload.
      /// </summary>
      private async void DoUpload()
      {
         if (null != fCancelTokenSource)
         {
            // already uploading - return
            return;
         }

         try
         {
            // setup our cancelation token so we can cancel during the upload
            fCancelTokenSource = new CancellationTokenSource();
            await AApiManager.Instance.UploadAsync(fNanoList, this.HandleProgress);
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
            fCancelTokenSource.Dispose();
            fCancelTokenSource = null;
            AStateController.Instance.PostPerformActionEvent("enablebutton", "done");
         }
      }

      /// <summary>
      /// Handle progress information from the async upload.
      /// </summary>
      /// <param name="currentFile"></param>
      /// <param name="totalFiles"></param>
      /// <param name="currentBytes"></param>
      /// <param name="totalBytes"></param>
      /// <param name="fileComplete"></param>
      /// <param name="errorString"></param>
      private void HandleProgress(int currentFile, int totalFiles, long currentBytes, long totalBytes, bool fileComplete, string errorString)
      {
         if (this.InvokeRequired)
         {
            // handle our async cancel here from our callback
            if (null != fCancelTokenSource && fCancelTokenSource.Token.IsCancellationRequested)
            {
               fCancelTokenSource.Token.ThrowIfCancellationRequested();
            }

            this.Invoke(new Action(() => this.HandleProgress(currentFile, totalFiles, currentBytes, totalBytes, fileComplete, errorString)));
            return;
         }

         // handle our async cancel here from our callback
         if (null != fCancelTokenSource && fCancelTokenSource.Token.IsCancellationRequested)
         {
            fCancelTokenSource.Token.ThrowIfCancellationRequested();
         }

         AOutput.DebugString("currentFile = " + currentFile);
         AOutput.DebugString("currentBytes = " + currentBytes);
         AOutput.DebugString("totalBytes = " + totalBytes);
         int perc = Math.Min((int)(((float)currentBytes / (float)totalBytes) * 100.0), 100);
         this.progressBar.Value = perc;

         if (fileComplete && currentFile == totalFiles)
         {
            this.fileProgressText.Text = Resources.UploadComplete;
         }
         else
         {
            this.fileProgressText.Text = String.Format(Resources.UploadProgressString, currentFile, totalFiles);
         }

         if (fileComplete)
         {
            this.fileList.MarkFileAtIndex(currentFile - 1, success: (null == errorString), toolTip: errorString);
            if (null != errorString)
            {
               AOutput.LogMessage("Error uploading file: " + errorString);
            }
         }
      }

      /// <summary>
      /// If our async op is still in action, confirm exit with the user.
      /// </summary>
      /// <param name="newState"></param>
      /// <param name="args"></param>
      private void HandleStateWillChangeEvent(AState newState, string actionName, CancelEventArgs args)
      {
         if (null != fCancelTokenSource && DialogResult.No == AOutput.DisplayYesNo(Resources.CancelMessage, Resources.CancelCaption))
         {
            args.Cancel = true;
         }
      }
   }
}

