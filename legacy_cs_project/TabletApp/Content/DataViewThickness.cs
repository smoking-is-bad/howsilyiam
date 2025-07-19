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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabletApp.State;
using DsiApi;
using Logging;
using Model;
using TabletApp.Utils;
using TabletApp.Properties;
using System.Diagnostics;
using TabletApp.Api;
using TabletApp.Api.Network;
using System.Threading;
using TabletApp.AutoRead;
using DsiExtensions;
using TabletApp.Views;

namespace TabletApp.Content
{
   /// <summary>
   /// User control for the thickness data view.  This has 2 modes - one for selecting
   /// the DSIs and probes, and the other for normal data viewing, where the mode
   /// is determined by the parameters.
   /// </summary>
   public partial class DataViewThickness : BaseContent
   {
      /// <summary>
      /// The main struct will our DSI info is kept
      /// </summary>
      private AMasterDsiInfo fMasterDsiInfo = new AMasterDsiInfo();

      /// <summary>
      /// determine whether we're using "last" values (eg thickness) or current values
      /// </summary>
      private bool fScanMode = false;

      /// <summary>
      /// Are we in the commissioning path?
      /// </summary>
      private bool fCommissioning = false;

      /// <summary>
      /// Are we working with a data logger DSI?  If so, switch to logger mode.
      /// </summary>
      private bool fLoggerMode = false;

      /// <summary>
      /// Is logging currently enabled on the logger DSI?
      /// </summary>
      private bool fLoggingEnabled = true;

      /// <summary>
      /// Track the currently selected DSI
      /// </summary>
      private int fCurrentDsiIndex = 0;

      /// <summary>
      /// Handle cancellation of our async ops
      /// </summary>
      private CancellationTokenSource fCancelTokenSource;

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="parameters">Parameters from the state def file</param>
      public DataViewThickness(Dictionary<string, string> parameters)
         : base(parameters)
      {
         InitializeComponent();

         // register for our events
         this.dsiGrid.DsiButtonClickedEvent += HandleCurrentDsiChanged;
         this.probeList.ProbeSelectionStateChangedEvent += HandleProbeSelectionChanged;
         AStateController.Instance.StateWillChangeEvent += HandleStateWillChangeEventAsync;

         // state params determine if we're in "scan" mode or in the thickness data view
         fScanMode = (fParams.ContainsKey("mode") && fParams["mode"] == "scan");
         fCommissioning = (fParams.ContainsKey("commissioning") && fParams["commissioning"] == "true");

         // disable the "read" buttons in the wizard - user can't read until all DSIs have been scanned
         if (fCommissioning)
         {
            AStateController.Instance.PostPerformActionEvent("disablebutton", "add");
            AStateController.Instance.PostPerformActionEvent("disablebutton", "edit");
            AStateController.Instance.PostPerformActionEvent("disablebutton", "replace");
            AStateController.Instance.PostPerformActionEvent("disablebutton", "editstring");
         }
         else
         {
            AStateController.Instance.PostPerformActionEvent("disablebutton", "readall");
            AStateController.Instance.PostPerformActionEvent("disablebutton", "readsel");
         }

         this.InitializeData();
      }

      /// <summary>
      /// Initialize our data according to our current mode (scan vs data view).
      /// </summary>
      private void InitializeData()
      {
         this.EnableProbeList(false);
         this.ShowProgress(null);
         // if we're in "scan" mode, start the scan or use existing data
         if (fScanMode)
         {
            this.InitForScanMode();
         }
         // thickness data view mode - just display the availabe info
         else
         {
            this.InitForDataViewMode();
         }
      }

      /// <summary>
      /// Initialize our data for scan mode.  If it's the first scan, we need to perform
      /// the async scan, otherwise we can use the existing scan data.
      /// </summary>
      private void InitForScanMode()
      {
         // want to listen for "read selected" action
         AStateController.Instance.PerformActionEvent += HandlePerformAction;

         this.selectInfo.Visible = false;
         // use the existing scan data if it's available
         if (AStateController.Instance.GlobalData.ContainsKey("masterdsi-full"))
         {
            fMasterDsiInfo = (AMasterDsiInfo)AStateController.Instance.GlobalData["masterdsi-full"];
            if (fCommissioning)
            {
               AStateController.Instance.PostPerformActionEvent("enablebutton", "add");
               AStateController.Instance.PostPerformActionEvent("enablebutton", "edit");
               AStateController.Instance.PostPerformActionEvent("enablebutton", "replace");
               AStateController.Instance.PostPerformActionEvent("enablebutton", "editstring");
            }
            else
            {
               AStateController.Instance.PostPerformActionEvent("enablebutton", "readall");
               AStateController.Instance.PostPerformActionEvent("enablebutton", "reset");
            }
            this.UpdateReadSelectedButtonState();
            if (fMasterDsiInfo.DsiItems.Count > 0)
            {
               this.UpdateSiteInfo(fMasterDsiInfo.DsiItems[0]);
               fLoggerMode = fMasterDsiInfo.DsiItems[0].Dsi.IsLoggerDsi();
            }
            fMasterDsiInfo.CurrentDsi = 0;
            fCurrentDsiIndex = fMasterDsiInfo.CurrentDsi;
            if (fLoggerMode)
            {
               this.UpdateForDataLoggerMode();
               // remove all of the "shot" DSIs if in logger mode
               fMasterDsiInfo.DsiItems.RemoveRange(1, fMasterDsiInfo.DsiItems.Count - 1);
            }
            else
            {
               this.UpdateDsiGrid(fMasterDsiInfo.DsiItems);
               this.dsiGrid.SelectDsi(fMasterDsiInfo.CurrentDsi);
               this.EnableProbeList(true);
            }
            this.UpdateCurrentDsi(fMasterDsiInfo.CurrentDsi);
         }
         // otherwise perform the scan
         else
         {
            this.StartScan();
         }
      }

      /// <summary>
      /// Initialize our data for the thickness data view mode.
      /// </summary>
      private void InitForDataViewMode()
      {
         fMasterDsiInfo = (AMasterDsiInfo)AStateController.Instance.GlobalData["masterdsi"];
         if (fMasterDsiInfo.DsiItems.Count > 0)
         {
            this.UpdateSiteInfo(fMasterDsiInfo.DsiItems[0]);
            fLoggerMode = fMasterDsiInfo.DsiItems[0].Dsi.IsLoggerDsi();
         }
         fCurrentDsiIndex = fMasterDsiInfo.CurrentDsi;
         if (fLoggerMode && !fCommissioning)
         {
            this.UpdateForDataLoggerMode();
         }
         else
         {
            this.UpdateDsiGrid(fMasterDsiInfo.DsiItems);
            this.dsiGrid.SelectDsi(fMasterDsiInfo.CurrentDsi);
         }
         this.UpdateCurrentDsi(fMasterDsiInfo.CurrentDsi);
         this.EnableProbeList(fCommissioning);
         this.selectInfo.Visible = fCommissioning;
      }

      /// <summary>
      /// We've discovered we're working with a data-logger DSI - switch modes.
      /// </summary>
      private void UpdateForDataLoggerMode()
      {
         if (this.InvokeRequired)
         {
            this.Invoke(new Action(() => this.UpdateForDataLoggerMode()));
            return;
         }

         this.EnableProbeList(false);
         this.dsiGrid.Visible = false;
         this.shotNavigator.Visible = true;
         this.shotNavigator.SetInfoMode(fScanMode);
         this.shotNavigator.SetEditMode(!fScanMode);
         bool reviewMode = (AStateController.Instance.GlobalData.ContainsKey("datamode") && (string)AStateController.Instance.GlobalData["datamode"] == "review");
         if (!reviewMode)
         {
            // in review mode, we show the datalogger UI, but don't want it to show as data logger
            AStateController.Instance.PostPerformActionEvent("setinfo", Resources.LoggerOnString);
         }
         else
         {
            this.shotNavigator.SetTitleText("");
         }

         if (fLoggerMode && !reviewMode)
         {
            this.UpdateLoggingButtonAndSetInfo();
         }
        
         if (fScanMode)
         {
            this.shotNavigator.SetNumShots(fMasterDsiInfo.DsiItems[0].Dsi.numShots);
         }
         else
         {
            this.shotNavigator.SetShotInfo(fMasterDsiInfo.CurrentDsi, fMasterDsiInfo.DsiItems[0].Dsi.numShots);
         }
         this.shotNavigator.ShotChangedEvent += ShotNavigator_ShotChangedEvent;
         if (fCommissioning)
         {
            AStateController.Instance.PostPerformActionEvent("disablebutton", "add");
            AStateController.Instance.PostPerformActionEvent("disablebutton", "replace");
         }
      }

      /// <summary>
      /// Update the enable/disable logging button
      /// </summary>
      private void UpdateLoggingButtonAndSetInfo()
      {
         fMasterDsiInfo = (AMasterDsiInfo)AStateController.Instance.GlobalData["masterdsi"];
         bool readFailed = true;
         bool enabled = fMasterDsiInfo.Dsi.ReadDataLoggerEnabled((byte)fMasterDsiInfo.DsiItems[0].Dsi.modbusAddress, out readFailed);
         if (!readFailed)
         {
            fLoggingEnabled = enabled;
         }
         AStateController.Instance.PostPerformActionEvent("enablebutton", "readsel");
         AStateController.Instance.PostPerformActionEvent("setbuttontitle", new List<string>() { "readsel", fLoggingEnabled ? Resources.ButtonDisableLogging : Resources.ButtonEnableLogging });
         AStateController.Instance.PostPerformActionEvent("setinfo",
            fLoggingEnabled ? Resources.LoggerOnString : Resources.LoggerOffString);
         var modeMatches = new Regex(@"logger|commissioning").Matches(this.ReadMode ?? "");
         if (modeMatches.Count != 0)
         {
            AStateController.Instance.PostPerformActionEvent("disablebutton", "back");
         }
      }

      /// <summary>
      /// Respond to shot change from the shot navigator - update the probe list.
      /// </summary>
      /// <param name="currentShot"></param>
      private void ShotNavigator_ShotChangedEvent(int currentShot)
      {
         this.UpdateCurrentDsi(currentShot);  // shot number is 1-based, we want +1 to skip over the actual DSI, but are passing a 0-based index
      }

      /// <summary>
      /// About to disappear - clean up.
      /// </summary>
      public override void WillDisappear()
      {
         // clean up our event listening
         AStateController.Instance.PerformActionEvent -= HandlePerformAction;
         AStateController.Instance.StateWillChangeEvent -= HandleStateWillChangeEventAsync;
         if (null != fCancelTokenSource)
         {
            // cancel out of any async ops
            fCancelTokenSource.Cancel();
         }
      }

      /// <summary>
      /// If our async op is still in action, confirm exit with the user.
      /// </summary>
      /// <param name="newState"></param>
      /// <param name="args"></param>
      private void HandleStateWillChangeEventAsync(AState newState, string actionName, CancelEventArgs args)
      {
         bool newStateOk = (null != newState && ("progress" == newState.Id || "dvascan" == newState.Id || "dvsummary" == newState.Id));

         // Read Selected button masquerades as the enable/disable logger button in logger mode
         if (fLoggerMode && "readsel" == actionName)
         {
            fLoggingEnabled = !fLoggingEnabled;
            fMasterDsiInfo.Dsi.SetRealTimeClockAndEnableDataLogger((byte)fMasterDsiInfo.DsiItems[0].Dsi.modbusAddress, fLoggingEnabled);

            // 2085: Check that logging was enabled.
            AStateController.Instance.PostPerformActionEvent("showspinnerprogress", Resources.CheckingDataLoggerEnabled);
            var checker = new ADataLoggerChecker(endLoggingOnHeartbeat: false);
            checker.Listen();

            // 0243(nanosense2): Don't update displayed logger state until we have confirmed the change.
            this.UpdateForDataLoggerMode();

            // Don't update the info with "setinfo" because home will hide it anyway.
            AStateController.Instance.PostPerformActionEvent("hidespinnerprogress", null);

            AStateController.Instance.ChangeToState("home");

            args.Cancel = true;
            return;
         }

         // first check if we will be stopping an auto-read by changing states
         if ((!fScanMode && !newStateOk && AAutoReadManager.Instance.AutoReadActive) &&
             DialogResult.No == AOutput.DisplayYesNo(Resources.AutoReadCancelMessage, Resources.AutoReadCancelCaption))
         {
            args.Cancel = true;
         }

         // now check if we would be canceling an active scan
         if (null != fCancelTokenSource && DialogResult.No == AOutput.DisplayYesNo(Resources.CancelMessage, Resources.CancelCaption))
         {
            args.Cancel = true;
         }

         // check for hitting Done button while in commissioning with no DSIs found - just go back
         if ("done" == actionName)
         {
            if (0 == fMasterDsiInfo.DsiItems.Count)
            {
               AStateController.Instance.ChangeToPreviousState();
               args.Cancel = true;
            }
         }

         // if we are stopping an auto-read due to the state change, force-stop the auto-read
         if (!args.Cancel && !newStateOk && AAutoReadManager.Instance.AutoReadActive && !fScanMode)
         {
            AAutoReadManager.Instance.StopAutoRead();
         }

         // if we're in the data view, share the current dsi setting with the other data views
         if (!fScanMode && !fCommissioning)
         {
            fMasterDsiInfo.CurrentDsi = fCurrentDsiIndex;
         }
      }

      /// <summary>
      /// Show/hide our progress spinner with associated text.
      /// </summary>
      /// <param name="progressText">Text to show - if null, hide the spinner</param>
      private void ShowProgress(string progressText)
      {
         if (this.InvokeRequired)
         {
            this.Invoke(new Action(() => this.ShowProgress(progressText)));
            return;
         }

         if (null != progressText)
         {
            AStateController.Instance.PostPerformActionEvent("showspinnerprogress", progressText);
         }
         else
         {
            AStateController.Instance.PostPerformActionEvent("hidespinnerprogress", null);
         }
      }

      /// <summary>
      /// Change selectability of the probe list.
      /// </summary>
      /// <param name="selectable"></param>
      private void EnableProbeList(bool selectable)
      {
         if (this.InvokeRequired)
         {
            this.Invoke(new Action(() => this.EnableProbeList(selectable)));
            return;
         }

         this.probeList.Selectable = selectable;
      }

      /// <summary>
      /// Start our async scan.
      /// </summary>
      private void StartScan()
      {
         fCancelTokenSource = new CancellationTokenSource();
         Task.Run(
            async () =>
            {
               // allow the UI to update before starting the scan
               await Task.Delay(300);
               this.ScanNetworkAsync();
            }
         );
      }

      /// <summary>
      /// Async method that scans the DSI network
      /// </summary>
      private async void ScanNetworkAsync()
      {
         bool onlyFactory = true;
         bool foundLogger = false;

         try
         {
            AStateController.Instance.PostPerformActionEvent("disablebutton", "rescan");
            this.ShowProgress(Resources.InitializeNetworkProgress);
            fMasterDsiInfo.Dsi = AApiManager.Instance.InitializeDsiNetwork();
            this.ShowProgress(Resources.ScanProgress);
            await AApiManager.Instance.ScanNetworkAsync(fMasterDsiInfo.Dsi, false, 
               (Model.ADsiInfo dsiInfo, List<AProbe> probes) =>
               {
                  // handle our async cancel here from our callback
                  if (fCancelTokenSource.Token.IsCancellationRequested)
                  {
                     fCancelTokenSource.Token.ThrowIfCancellationRequested();
                  }

                  if (ADsiNetwork.kFactoryId == dsiInfo.modbusAddress)
                  {
                     return;
                  }

                  if (foundLogger)
                  {
                     return;
                  }

                  onlyFactory = false;
                  ANanoSense newItem = new ANanoSense();
                  newItem.Dsi = dsiInfo;
                  newItem.Dsi.probes = probes.ToArray();

                  // if it's the first one (selected by default), update the dsiinfo and probe list
                  if (0 == this.dsiGrid.ActiveDsiCount)
                  {
                     this.RetrieveSiteInfo(newItem);
                     // if it's the first one, update the site and dsi info
                     this.UpdateSiteInfo(newItem);
                     this.UpdateDsiInfo(newItem);
                     this.UpdateProbeList(probes);
                  }
                  else
                  {
                     fMasterDsiInfo.Dsi.GetSiteInfo(newItem);
                  }
                  foundLogger = dsiInfo.IsLoggerDsi();
                  if (foundLogger)
                  {
                     // we don't care about other DSIs if there's a logger DSI
                     fMasterDsiInfo.DsiItems.Clear();
                  }
                  // update the grid to reflect a new dsi
                  this.AddDsiToGrid(newItem);
                  // store the dsiinfo and probes in our ivar
                  fMasterDsiInfo.DsiItems.Add(newItem);
                  //Thread.Sleep(5000);         // debug to simulate delay
               });

            if (!onlyFactory)
            {
               // re-enable our "read" buttons now that the entire DSI network has been scanned
               if (fCommissioning)
               {
                  AStateController.Instance.PostPerformActionEvent("enablebutton", "add");
                  AStateController.Instance.PostPerformActionEvent("enablebutton", "edit");
                  AStateController.Instance.PostPerformActionEvent("enablebutton", "replace");
                  AStateController.Instance.PostPerformActionEvent("enablebutton", "editstring");
               }
               else
               {
                  if (!foundLogger || fMasterDsiInfo.DsiItems[0].Dsi.numShots > 0)
                  {
                     AStateController.Instance.PostPerformActionEvent("enablebutton", "readall");
                  }
                  else
                  {
                     AStateController.Instance.PostPerformActionEvent("disablebutton", "readall");
                  }
               }

               AStateController.Instance.GlobalData["masterdsi"] = fMasterDsiInfo;
               // store a dup of the master info in case we do a read selected, which alters "masterdsi"
               AStateController.Instance.GlobalData["masterdsi-full"] = fMasterDsiInfo;
            }
         }
         catch (OperationCanceledException)
         {
            // silent fail
         }
         catch (Exception e)
         {
            onlyFactory = false;
            String msg = Resources.ErrorScanNetwork + " " + e.AggregateMessage();
            ALog.Error("Scan Network", msg);
            AOutput.DisplayError(msg);
         }
         finally
         {
            if (onlyFactory)
            {
               AOutput.DisplayError(Resources.ErrorOnlyFactoryDsiFound);
            }
            fCancelTokenSource.Dispose();
            fCancelTokenSource = null;
            this.EnableProbeList(true);
            this.ShowProgress(null);
            AStateController.Instance.PostPerformActionEvent("enablebutton", "rescan");
            if (foundLogger)
            {
               fLoggerMode = true;
               this.UpdateForDataLoggerMode();
               ANanoSense nano = fMasterDsiInfo.DsiItems[0];
               if (fCommissioning && nano.Dsi.IsLoggerDsi() && nano.Dsi.numShots > 0)
               {
                  // 2088 - display dialog on GUI thread so that it will remain on top
                  // of this form.
                  this.Invoke(new Action(() =>
                   {
                      var choice = AOutput.DisplayYesNo(Resources.ErrorLoggerHasShotsMessage, Resources.ErrorLoggerHasShotsTitle);
                      AStateController.Instance.ChangeToState(DialogResult.Yes == choice ? "scan" : "home");
                   }));
               }
            }
         }
      }

      /// <summary>
      /// Handle the Read Selected action by populating our master DSI info
      /// state object with only the selected probes.
      /// </summary>
      /// <param name="actionName"></param>
      /// <param name="data"></param>
      private void HandlePerformAction(string actionName, object data)
      {
         if ("readsel" == actionName)
         {
            if (!fLoggerMode)
            {
               AStateController.Instance.GlobalData["masterdsi"] = fMasterDsiInfo.CloneForSelectedProbes(fMasterDsiInfo.SelectedProbes);
            }
         }
         else if ("readall" == actionName)
         {
            AStateController.Instance.GlobalData["masterdsi"] = fMasterDsiInfo;
            if (fLoggerMode)
            {
               fMasterDsiInfo.CurrentDsi = 1;      // skip over the logger DSI
            }
         }
         else if ("rescan" == actionName)
         {
            AStateController.Instance.PostPerformActionEvent("disablebutton", "rescan");
            AStateController.Instance.PostPerformActionEvent("disablebutton", "readall");
            AMasterDsiInfo.ClearGlobalData();
            fMasterDsiInfo = new AMasterDsiInfo();
            fCurrentDsiIndex = 0;
            this.dsiGrid.Clear();
            this.probeList.Clear();
            this.EnableProbeList(true);
            this.StartScan();
         }
         else if ("add" == actionName)
         {
            // for add, we want CurrentDsi to be the current last one, as it will be incremented later in the
            // commissioning path, and then just duplicate our last DSI as the template for the new one
            fMasterDsiInfo.CurrentDsi = fMasterDsiInfo.DsiItems.Count - 1;
            ANanoSense newNano = fMasterDsiInfo.DsiItems[fMasterDsiInfo.DsiItems.Count - 1].ShallowCopy();
            fMasterDsiInfo.DsiItems.Add(newNano);
         }
         else if ("edit" == actionName)
         {
            // edit operates on the currently selected dsi
            fMasterDsiInfo.CurrentDsi = fCurrentDsiIndex;
         }
         else if ("replace" == actionName)
         {
            // replace operates on the currently selected dsi
            fMasterDsiInfo.CurrentDsi = fCurrentDsiIndex;
         }
      }

      /// <summary>
      /// Handle a probe selection change, which means the user selected or unselected
      /// a probe for reading.  We need to keep track of which probes are selected for
      /// Read Selected.
      /// </summary>
      /// <param name="probeIndex"></param>
      /// <param name="selected"></param>
      private void HandleProbeSelectionChanged(int probeIndex, bool selected)
      {
         // if commissioning, change to probeparams screen when a probe is clicked,
         // recording the index of the selected probe
         if (fCommissioning && !fScanMode)
         {
            AStateController.Instance.GlobalData["probeindex"] = probeIndex;
            AStateController.Instance.ChangeToState("comm-probeparams");
         }
         else if (!fCommissioning)
         {
            // update our selection list as needed
            if (selected)
            {
               if (!fMasterDsiInfo.SelectedProbes.ContainsKey(fCurrentDsiIndex))
               {
                  fMasterDsiInfo.SelectedProbes[fCurrentDsiIndex] = new List<int>();
               }
               fMasterDsiInfo.SelectedProbes[fCurrentDsiIndex].Add(probeIndex);
            }
            else
            {
               fMasterDsiInfo.SelectedProbes[fCurrentDsiIndex].Remove(probeIndex);
            }
            this.UpdateReadSelectedButtonState();
         }
      }

      /// <summary>
      /// Update the enabled state of the Read Selected button according to the selection
      /// count from all the probe lists.
      /// </summary>
      private void UpdateReadSelectedButtonState()
      {
         // enable/disable the Read Selected button according to the number of probes selected
         int totalCount = 0;
         foreach (List<int> selList in fMasterDsiInfo.SelectedProbes.Values)
         {
            totalCount += selList.Count;
         }
         AStateController.Instance.PostPerformActionEvent((totalCount > 0 ? "enablebutton" : "disablebutton"), "readsel");
      }

      /// <summary>
      /// Update our current DSI info in response to a DSI button click
      /// </summary>
      /// <param name="newIndex"></param>
      private void HandleCurrentDsiChanged(int newIndex)
      {
         if (fCommissioning && !fScanMode)
         {
            AStateController.Instance.ChangeToState("comm-dsiparams");
         }
         else
         {
            this.UpdateCurrentDsi(newIndex);
         }
      }

      /// <summary>
      /// Respond to a current DSI change by updating the relevant DSI info.
      /// </summary>
      /// <param name="dsiIndex">index of the DSI</param>
      private void UpdateCurrentDsi(int dsiIndex)
      {
         if (this.InvokeRequired)
         {
            this.Invoke(new Action(() => this.UpdateCurrentDsi(dsiIndex)));
            return;
         }

         Debug.Assert(dsiIndex < fMasterDsiInfo.DsiItems.Count);
         fCurrentDsiIndex = dsiIndex;
         ANanoSense dsiItem = fMasterDsiInfo.DsiItems[dsiIndex];
         this.UpdateDsiInfo(dsiItem);
         this.UpdateProbeList(dsiItem.Dsi.probes);
         // don't show the selector line if there are no probes
         this.dsiGrid.LineVisible = (null != dsiItem.Dsi.probes && dsiItem.Dsi.probes.Length > 0);
      }

      /// <summary>
      /// Update the site info.  Should only need to happen once per visit to this screen.
      /// </summary>
      /// <param name="dsi">ANanoSense object</param>
      private void RetrieveSiteInfo(ANanoSense dsi)
      {
         fMasterDsiInfo.Dsi.GetSiteInfo(dsi);
         this.UpdateSiteInfo(dsi);
      }

      /// <summary>
      /// Perform the actual update of the site info, according to the params.
      /// </summary>
      /// <param name="dsi"></param>
      private void UpdateSiteInfo(ANanoSense dsi)
      {
         if (this.InvokeRequired)
         {
            this.Invoke(new Action(() => this.UpdateSiteInfo(dsi)));
            return;
         }

         this.locationInfo.PopulateWithDsi(dsi);
         // set our location info in the app header
         AStateController.Instance.PostPerformActionEvent("setlocation", dsi.LocationString());
      }

      /// <summary>
      /// Update the DSI info view
      /// </summary>
      /// <param name="dsiItem"></param>
      private void UpdateDsiInfo(ANanoSense dsiItem)
      {
         if (this.InvokeRequired)
         {
            this.Invoke(new Action(() => this.UpdateDsiInfo(dsiItem)));
            return;
         }

         this.dsiInfo.PopulateWithDsi(dsiItem);
      }

      /// <summary>
      /// Add the new DSI to the grid.  Gets called as the DSIs are read, so just needs to add the
      /// new DSI with the proper state.
      /// </summary>
      /// <param name="dsiItem"></param>
      private void AddDsiToGrid(ANanoSense dsiItem)
      {
         if (this.InvokeRequired)
         {
            this.Invoke(new Action(() => this.AddDsiToGrid(dsiItem)));
            return;
         }

         this.dsiGrid.AddDsi(dsiItem.Dsi, dsiItem.Dsi.probes.CurrentAlertState());
      }

      /// <summary>
      /// Update all the DSI states on the grid.
      /// </summary>
      /// <param name="items"></param>
      private void UpdateDsiGrid(List<ANanoSense> items)
      {
         if (this.InvokeRequired)
         {
            this.Invoke(new Action(() => this.UpdateDsiGrid(items)));
            return;
         }

         foreach (ANanoSense item in items)
         {
            this.AddDsiToGrid(item);
         }

         if (fCommissioning && !fScanMode)
         {
            List<DsiButton> buttons = this.dsiGrid.AllButtons();
            for (int i = 0; i < fMasterDsiInfo.DsiItems.Count; ++i)
            {
               if (i != fMasterDsiInfo.CurrentDsi)
               {
                  buttons[i].State = DsiButtonState.Empty;
               }
            }
         }
      }

      /// <summary>
      /// Update our probe list per the new probes provided.  This is a response to the first DSI being added
      /// or user changing the current DSI.
      /// </summary>
      /// <param name="probes"></param>
      private void UpdateProbeList(IList<AProbe> probes)
      {
         if (this.InvokeRequired)
         {
            this.Invoke(new Action(() => this.UpdateProbeList(probes)));
            return;
         }

         this.probeList.PopulateWithProbes(probes);
         this.probeList.SetSelected(fMasterDsiInfo.SelectedProbes.ContainsKey(fCurrentDsiIndex) ? fMasterDsiInfo.SelectedProbes[fCurrentDsiIndex] : null);
      }
   }
}

