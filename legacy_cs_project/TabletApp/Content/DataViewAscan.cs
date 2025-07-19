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
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabletApp.State;
using DsiApi;
using Model;
using TabletApp.Properties;
using TabletApp.Utils;
using TabletApp.Views;
using System.Diagnostics;
using TabletApp.Api;
using TabletApp.Api.Network;
using System.Threading;
using TabletApp.Persist;
using TabletApp.AutoRead;
using DsiExtensions;

namespace TabletApp.Content
{
   /// <summary>
   /// User control for the AScan data view.  Has 2 modes - progress mode and
   /// normal mode, determined by the "progress" parameter.
   /// </summary>
   public partial class DataViewAscan : BaseContent
   {
      private ReadProgressInfo fProgressInfo = new ReadProgressInfo();
      private AMasterDsiInfo fMasterDsiInfo;
      private CancellationTokenSource fReadCancelTokenSource;
      private int fCurrentDsiIndex;      // 0-31
      private bool fProgressMode;
      private bool fCommissioning = false;
      private bool fLoggerMode = false;

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="parameters">Parameters from the state def file</param>
      public DataViewAscan(Dictionary<string, string> parameters)
         : base(parameters)
      {
         InitializeComponent();

         // get the DSI info from the state globals
         Debug.Assert(AStateController.Instance.GlobalData.ContainsKey("masterdsi"));
         fMasterDsiInfo = (AMasterDsiInfo)AStateController.Instance.GlobalData["masterdsi"];
         Debug.Assert(fMasterDsiInfo.DsiItems.Count > 0);

         // register for our events
         this.dsiGrid.DsiButtonClickedEvent += HandleCurrentDsiChanged;
         AStateController.Instance.StateWillChangeEvent += HandleStateWillChangeEvent;

         fProgressMode = fParams.ContainsKey("mode") && fParams["mode"] == "progress";
         fCommissioning = (fParams.ContainsKey("commissioning") && fParams["commissioning"] == "true");
         fLoggerMode = fMasterDsiInfo.DsiItems[0].Dsi.IsLoggerDsi() && !fCommissioning;
         fCurrentDsiIndex = fMasterDsiInfo.CurrentDsi;

         this.InitializeData();
      }

      /// <summary>
      /// Initialize our data according to our current mode (progress vs data view)
      /// </summary>
      private void InitializeData()
      {
         // update DSI info
         this.UpdateSiteInfo(fMasterDsiInfo.DsiItems[0]);
         dsiStats.PopulateWithDsi(fMasterDsiInfo.DsiItems[0].Dsi);

         if (fLoggerMode)
         {
            this.InitForLoggerMode();
         }
         else
         {
            this.UpdateDsiGrid(fMasterDsiInfo.DsiItems);
            this.dsiGrid.LineVisible = false;
         }

         // if in progress mode, kick off our measurements
         if (fProgressMode)
         {
            // in progress mode, and upon first visit in data mode, we want to initialize
            // our scroll/zoom from settings (ie not from the data model)
            AStateController.Instance.GlobalData["ZoomScrollFromSettings"] = true;
            this.InitProbeDrawingInfo();
            this.ascanList.HandleGraphChange = false;
            this.dsiGrid.Selectable = false;
            this.dsiProgress.Visible = true;
            if (Properties.Settings.Default.AutoRead && !fCommissioning && !fLoggerMode)
            {
               AAutoReadManager.Instance.StartAutoRead();
               AAutoReadManager.Instance.StartTimer();
            }
            this.InitProgress();
            fReadCancelTokenSource = new CancellationTokenSource();
            Task.Run(
                async () =>
                {
                   await Task.Delay(300);
                   await this.PerformMeasurementsAsync();
                }
             );
         }
         // otherwise, just display the data we have
         else
         {
            this.InitProbeDrawingInfo();
            this.ascanList.HandleGraphChange = true;
            if (!fLoggerMode)
            {
               this.dsiGrid.Selectable = true;
               this.dsiGrid.Top = 141;
               // populate with the current dsi by default
               this.dsiGrid.SelectDsi(fMasterDsiInfo.CurrentDsi);
            }
            this.locationInfo.Visible = true;
            this.UpdateProbeList(fMasterDsiInfo.CurrentDsi);
            // after first visit to the data view mode (ie not the progress view), we want
            // to persist the scroll/zoom for the ascan graphs in the data model
            // and NOT get them from settings
            AStateController.Instance.GlobalData["ZoomScrollFromSettings"] = false;
         }
      }

      /// <summary>
      /// Initialize the view for logger mode.
      /// </summary>
      private void InitForLoggerMode()
      {
         this.dsiGrid.Visible = false;
         this.shotNavigator.Visible = true;
         this.shotNavigator.SetInfoMode(fProgressMode);
         this.shotNavigator.SetEditMode(!fProgressMode);
         this.shotNavigator.SetShotInfo(fMasterDsiInfo.CurrentDsi, fMasterDsiInfo.DsiItems[0].Dsi.numShots);
         this.shotNavigator.ShotChangedEvent += ShotNavigator_ShotChangedEvent;
         bool reviewMode = (AStateController.Instance.GlobalData.ContainsKey("datamode") && (string)AStateController.Instance.GlobalData["datamode"] == "review");

         // in review mode, we show the datalogger UI, but don't want it to show as data logger
         if (fProgressMode && !reviewMode)
         {
            AStateController.Instance.PostPerformActionEvent("setinfo", Resources.LoggerOffString);
         }
         if (reviewMode)
         {
            this.shotNavigator.SetTitleText("");
         }

         var modeMatches = new Regex(@"logger|commissioning").Matches(this.ReadMode ?? "");
         if (modeMatches.Count != 0)
         {
            AStateController.Instance.PostPerformActionEvent("disablebutton", "back");
         }
      }

      /// <summary>
      /// Respond to shot change from the shot navigator - update the ascan list.
      /// </summary>
      /// <param name="currentShot"></param>
      private void ShotNavigator_ShotChangedEvent(int currentShot)
      {
         fCurrentDsiIndex = currentShot;
         this.UpdateProbeList(currentShot);  // shot number is 1-based, we want +1 to skip over the actual DSI, but are passing a 0-based index
      }

      /// <summary>
      /// About to disappear - clean up.
      /// </summary>
      public override void WillDisappear()
      {
         this.dsiProgress.CleanUp();
         this.shotNavigator.ShotChangedEvent -= ShotNavigator_ShotChangedEvent;
         AStateController.Instance.StateWillChangeEvent -= HandleStateWillChangeEvent;
         if (null != fReadCancelTokenSource)
         {
            // cancel any async reads
            fReadCancelTokenSource.Cancel();
         }
         this.SaveProbeDrawingInfo();
#if false // Disabled for 0243(nanosense2) TODO: Remove when testing done
         if (fLoggerMode && fProgressMode)
         {
            bool readFailed;
            bool enabled = fMasterDsiInfo.Dsi.ReadDataLoggerEnabled((byte)fMasterDsiInfo.DsiItems[0].Dsi.modbusAddress, out readFailed);
            AStateController.Instance.PostPerformActionEvent("setinfo", enabled ? Resources.LoggerOnString : Resources.LoggerOffString);
         }
#endif
      }

      /// <summary>
      /// If our async op is still in action, confirm exit with the user.
      /// </summary>
      /// <param name="newState"></param>
      /// <param name="args"></param>
      private void HandleStateWillChangeEvent(AState newState, string actionName, CancelEventArgs args)
      {
         if (fCommissioning)
         {
            return;
         }

         // cannot interrupt logger reads
         if (null != fReadCancelTokenSource && fProgressMode && fLoggerMode)
         {
            AOutput.DisplayMessage(Resources.ErrorLoggerReadInterrupt);
            args.Cancel = true;
            return;
         }

         bool newStateOk = (null != newState && ("progress" == newState.Id || "dvsummary" == newState.Id || "dvthickness" == newState.Id));
         bool newStateIsScan = (null != newState && "scan" == newState.Id);

         // first check if we will be stopping an auto-read by changing states
         if ((!fProgressMode && !newStateOk && AAutoReadManager.Instance.AutoReadActive) &&
             DialogResult.No == AOutput.DisplayYesNo(Resources.AutoReadCancelMessage, Resources.AutoReadCancelCaption))
         {
            args.Cancel = true;
         }

         // now check if we would be canceling an active read
         if (null != fReadCancelTokenSource && DialogResult.No == AOutput.DisplayYesNo(Resources.CancelMessage, Resources.CancelCaption))
         {
            args.Cancel = true;
         }

         // if we are stopping an auto-read due to the state change, force-stop the auto-read
         if (!args.Cancel && !newStateOk && AAutoReadManager.Instance.AutoReadActive && (!fProgressMode || newStateIsScan))
         {
            AAutoReadManager.Instance.StopAutoRead();
         }

         // if we're in the data view, share the current dsi setting with the other data views
         if (!fProgressMode)
         {
            fMasterDsiInfo.CurrentDsi = fCurrentDsiIndex;
         }
      }

      /// <summary>
      /// The current DSI changed on the grid - update our probe list.
      /// </summary>
      /// <param name="dsiIndex">index of the new DSI</param>
      private void HandleCurrentDsiChanged(int dsiIndex)
      {
         // if commissioning, change to the dsiparams state when a DSI is clicked
         if (fCommissioning)
         {
            AStateController.Instance.ChangeToState("comm-dsiparams");
         }
         else
         {
            fCurrentDsiIndex = dsiIndex;
            UpdateDsiInfo(dsiIndex);
         }
      }

      /// <summary>
      /// Perform measurement of all our available DSIs/probes.
      /// If in logger mode, the read shots are saved as they are read, otherwise all files are 
      /// written after reading all DSIs on the network.
      /// </summary>
      private async Task PerformMeasurementsAsync()
      {
         bool success = false;

         // For summary CSV file
         var nanoGroups = new Dictionary<string, List<ANanoSense>>();

         try
         {

#if qTimeShots

            Stopwatch timer = Stopwatch.StartNew();
            long expectedProbes = 32 * 16;
            long shot = 0;

#endif

            fCurrentDsiIndex = 0;
            DateTime timestamp = DateTime.UtcNow;
            List<ANanoSense> nanos = fMasterDsiInfo.DsiItems.ToList();
            foreach (ANanoSense dsiItem in nanos)
            {
               // only operate on the current DSI for commissioning
               if (fCommissioning && fCurrentDsiIndex != fMasterDsiInfo.CurrentDsi)
               {
                  fCurrentDsiIndex++;
                  continue;
               }
               if (dsiItem.Dsi.probes.Length > 0)
               {
                  int maxRetries = Properties.Settings.Default.HighLevelRetryCount;
                  bool doRetry = true;
                  int retryCount = 0;
                  int startProbe = fProgressInfo.CurrProbe;

                  while (doRetry)
                  {
                     this.ascanList.Clear();
                     fProgressInfo.CurrProbe = startProbe;
                     this.UpdateProgress();

                     try
                     {
                        var callback = new Action<AMeasurementParam, AProbe, string>(
                           (AMeasurementParam param, AProbe probe, String errorMessage) =>
                           {
                              // handle async cancel here from our callback
                              if (fReadCancelTokenSource.Token.IsCancellationRequested)
                              {
                                 fReadCancelTokenSource.Token.ThrowIfCancellationRequested();
                              }

                              if (fLoggerMode)
                              {
                                 var nano = fMasterDsiInfo.DsiItems.Last();
   
                                 if (probe != null)
                                 {
                                    if (1 == probe.num)
                                    {
                                       nano = dsiItem.ShallowCopy();
                                       nano.Dsi.Model = "shot";
                                       fMasterDsiInfo.DsiItems.Add(nano);
                                       this.ascanList.Clear();
                                       if (fProgressInfo.CurrDsi > 0)
                                       {
                                          this.shotNavigator.IncrementCurrentShot();
                                       }
                                       fProgressInfo.CurrDsi++;
                                    }

                                    Array.Resize(ref nano.Dsi.probes, nano.Dsi.probes.Count() + 1);
                                    if ((bool)AStateController.Instance.GlobalData["ZoomScrollFromSettings"])
                                    {
                                       probe.Scroll = Properties.Settings.Default.AscanScroll;
                                       probe.Zoom = Properties.Settings.Default.AscanZoom;
                                    }
                                    nano.Dsi.probes[nano.Dsi.probes.Count() - 1] = probe;
                                 }
                                 
                                 // Write out shots when all probes for the DSI have been read
                                 // or some error interrupted reading.
                                 if (null == probe || nano.Dsi.probeCount == probe.num)
                                 {
                                    this.SaveDsiFiles(nano, nanoGroups);

                                    // save the measurement CSV file if the setting is on
                                    if (!AFileManager.Instance.SaveMeasurementCsvFiles(nano))
                                    {
                                        AOutput.LogMessage(Resources.ErrorSaveMeasurementCsv);
                                    }
                                 }
                              }

                              if (null != probe)
                              {
                                 // update our UI
                                 this.ascanList.AddProbe(probe);
                                 dsiStats.PopulateWithDsi(dsiItem.Dsi);
                              }

                              if (null != errorMessage)
                              {
                                 AOutput.LogMessage(errorMessage);
                                 AStateController.Instance.PostPerformActionEvent("showerror", null);
                                 throw new DsiStateException(errorMessage);
                              }

                              fProgressInfo.CurrProbe++;
                              this.UpdateProgress();

#if qTimeShots

                              if (++shot == expectedProbes)
                              {
                                 timer.Stop();
                                 TimeSpan elapsed = timer.Elapsed;
                                 Console.WriteLine(String.Format("***Total read time {0:00}:{1:00}:{2:00}", elapsed.Minutes,
                                    elapsed.Seconds, elapsed.Milliseconds));
                                 TimeSpan perProbe = new TimeSpan(elapsed.Ticks / (32 * 16)); // Assumed maxed out string and probes.
                                 Console.WriteLine(String.Format("***Per probe time {0:00}:{1:00}:{2:00}", perProbe.Minutes,
                                    perProbe.Seconds, perProbe.Milliseconds));
                              }

#endif

                        });
                        if (fLoggerMode)
                        {
                           await AApiManager.Instance.ReadDataLoggerShotsAsync(fMasterDsiInfo.Dsi, dsiItem, callback);

                           if ("commissioning" != (this.ReadMode ?? ""))
                           {
                              // 2088 - The dialog needs to be invoked on the GUI thread to make sure it
                              // appears over the main form.
                              this.Invoke(new Action(() =>
                               {
                                  if (DialogResult.Yes == AOutput.DisplayYesNo(Resources.LoggerEnableQuestion,
                                   Resources.LoggerEnableCaption))
                                  {
                                     AStateController.Instance.PostPerformActionEvent("showspinnerprogress", Resources.CheckingDataLoggerEnabled);
                                     fMasterDsiInfo.Dsi.SetRealTimeClockAndEnableDataLogger((byte)fMasterDsiInfo.DsiItems[0].Dsi.modbusAddress, true);
                                  // Listen for data logger heartbeat before changing state of logger enabled button
                                  try
                                     {
                                        var checker = new ADataLoggerChecker(endLoggingOnHeartbeat: false);
                                        checker.Listen();
                                        if (!checker.Canceled())
                                        {
                                        // Do not checker.SendAck() here because we don't want to end LoggingMode
                                        AStateController.Instance.PostPerformActionEvent("setinfo", Resources.LoggerOnString);
                                        }
                                     }
                                     catch (Exception e)
                                     {
                                        AOutput.LogException(e);
                                     //!!!this.MessageText = Resources.MessageErrorLoggerOpen;
                                  }
                                     AStateController.Instance.PostPerformActionEvent("hidespinnerprogress", null);
                                  }
                               }));
                           }
                        }
                        else
                        {
                           await AApiManager.Instance.PerformMeasurementsAsync(fMasterDsiInfo.Dsi, dsiItem, timestamp, null, callback);
                        }
                        doRetry = false;
                     }
                     catch (Exception ex)
                     {
                        // retry on exception
                        if (ex is DsiStateException || (null != ex.InnerException && ex.InnerException is DsiStateException))
                        {
                           if (retryCount < maxRetries)
                           {
                              ++retryCount;
                              AOutput.LogMessage("High level retry, number " + retryCount);
                              doRetry = true;
                              Thread.Sleep(200 * retryCount);
                           }
                           else
                           {
                              AOutput.LogMessage("Max high level retries reached, aborting");
                              throw ex;
                           }
                        }
                        else
                        {
                           throw ex;
                        }
                     }
                  }
                  fProgressInfo.CurrDsi++;
               }
               fCurrentDsiIndex++;
            }

            success = true;
         }
         catch (OperationCanceledException)
         {
            // silent fail
         }
         catch (Exception e)
         {
            AOutput.DisplayError(Resources.ErrorReadProbes + " " + e.AggregateMessage());
         }
         finally
         {
            fReadCancelTokenSource.Dispose();
            fReadCancelTokenSource = null;
         }

         if (success)
         {
            if (!fCommissioning)
            {
               if (fLoggerMode)
               {
                  String errorString = null;
                  this.SaveDsiSummary(nanoGroups, out errorString);
                  if (null != errorString)
                  {
                    AOutput.LogMessage(errorString);
                    // TODO: Log/report
                  }
               }
               else
               {
                  this.SaveDsis();
               }

               if (Properties.Settings.Default.AutoRead && !fLoggerMode)
               {
                  // for auto-read, do the async upload if enabled
                  if (Properties.Settings.Default.AutoUpload)
                  {
                     await AAutoReadManager.Instance.PerformAutoUploadAsync(fMasterDsiInfo.DsiItems.ToSaveList());
                  }
               }
            }
            // successfully finished - progress to our next state
            AStateController.Instance.ChangeToState(fParams["nextstate"]);
         }
         else if (Properties.Settings.Default.AutoRead && !fLoggerMode)
         {
            AOutput.LogMessage("Failure during auto-read operation.  Doing a rescan and continuing...");
            // rescan and go to next state if in autoread mode
            // so that autoread will continue with the next one
            await this.Rescan();
            AStateController.Instance.ChangeToState(fParams["nextstate"]);
         }
      }

      /// <summary>
      /// Do a rescan of the DSI network.  This is done in a failure state during an auto-read
      /// so that the auto-read has a better chance of continuing.
      /// </summary>
      private async Task<bool> Rescan()
      {
         bool success;

         try
         {
            Thread.Sleep(10000);       // give DSIs some time to recover

            List<ANanoSense> newDsis = new List<ANanoSense>();
            fMasterDsiInfo.Dsi = AApiManager.Instance.InitializeDsiNetwork();

            // first do a scan
            await AApiManager.Instance.ScanNetworkAsync(fMasterDsiInfo.Dsi, false,
               (Model.ADsiInfo dsiInfo, List<AProbe> probes) =>
               {
                  if (ADsiNetwork.kFactoryId == dsiInfo.modbusAddress)
                  {
                     return;
                  }

                  ANanoSense newItem = new ANanoSense();
                  newItem.Dsi = dsiInfo;
                  newItem.Dsi.probes = probes.ToArray();

                  fMasterDsiInfo.Dsi.GetSiteInfo(newItem);
                  // store the dsiinfo and probes in our ivar
                  newDsis.Add(newItem);
               });

            // now filter the found dsis/probes in case the user did a Read Selected
            List<ANanoSense> keepDsis = new List<ANanoSense>();
            foreach (ANanoSense newNano in newDsis)
            {
               ANanoSense foundDsi = fMasterDsiInfo.DsiItems.Find(d => d.Dsi.serialNumber == newNano.Dsi.serialNumber);
               if (null != foundDsi)
               {
                  // dsi was found in our current master list - keep it
                  keepDsis.Add(newNano);
                  List<AProbe> keepProbes = new List<AProbe>();
                  // go through the dsi's probes and make sure they were active
                  foreach (AProbe newProbe in newNano.Dsi.probes)
                  {
                     if (null != foundDsi.Dsi.probes.ToList().Find(p => p.num == newProbe.num))
                     {
                        // probe found in our current master list - keep it
                        keepProbes.Add(newProbe);
                     }
                  }
                  // update the probes of the new dsi object
                  newNano.Dsi.probes = keepProbes.ToArray();
               }
            }
            // update our master dsi list
            fMasterDsiInfo.DsiItems = keepDsis;
            success = true;
         }
         catch (Exception e)
         {
            AOutput.LogException(e, "Error during rescan");
            success = false;
         }

         return success;
      }

      /// <summary>
      /// Save all the DSIs represented by our list of ANanoSense objects.
      /// </summary>
      private void SaveDsis()
      {
         String errorString;
         List<ANanoSense> dsis = fMasterDsiInfo.DsiItemsSansLogger;

         // save our dsis now that we've gathered them
         if (fLoggerMode)
         {
            // need to save the "shot" dsis as "logger"
            dsis.ForEach(d => d.Dsi.Model = "logger");
         }
         AFileManager.Instance.Save(dsis, out errorString);
         if (fLoggerMode)
         {
            dsis.ForEach(d => d.Dsi.Model = "shot");
         }
         if (null != errorString)
         {
            AOutput.DisplayError(Resources.ErrorSave + " " + errorString);
         }
      }

      /// <summary>
      /// Save the SNI and measurement file for the given DSI.
      /// </summary>
      /// <param name="nanoGroups">On output, the dsiItem is added to the list for the item's CSV path. Used for writing the summary CSV file.</param>
      private void SaveDsiFiles(ANanoSense dsiItem, Dictionary<string, List<ANanoSense>> nanoGroups)
      {
         if (fLoggerMode)
         {
            dsiItem.Dsi.Model = "logger";
         }
         String csvPath = null;
         String saveErrorMessage = null;
         if (AFileManager.Instance.Save(dsiItem, out csvPath, out saveErrorMessage))
         {
            if (null != csvPath && !nanoGroups.Keys.Contains(csvPath))
            {
               nanoGroups[csvPath] = new List<ANanoSense>();
            }
            nanoGroups[csvPath].Add(dsiItem);
         }
         if (fLoggerMode)
         {
            dsiItem.Dsi.Model = "shot";
         }

         if (null != saveErrorMessage)
         {
            AOutput.DisplayError(saveErrorMessage);
         }
      }

      private void SaveDsiSummary(Dictionary<string, List<ANanoSense>> nanoGroups, out String errorString)
      {
         AFileManager.Instance.SaveSummaryFile(nanoGroups, out errorString);
      }


      /// <summary>
      /// Initialize our progress information and update the progress UI
      /// with that initial state.
      /// </summary>
      private void InitProgress()
      {
         AMasterDsiInfo info = (AMasterDsiInfo)AStateController.Instance.GlobalData["masterdsi"];
         fProgressInfo.NumDsis = 0;
         fProgressInfo.NumProbes = 0;
         if (fLoggerMode)
         {
            fProgressInfo.NumDsis = info.DsiItems[0].Dsi.numShots;
            fProgressInfo.NumProbes = info.DsiItems[0].Dsi.numShots * info.DsiItems[0].Dsi.probeCount;
         }
         else
         {
            foreach (ANanoSense dsiItem in info.DsiItems)
            {
               if (dsiItem.Dsi.probes.Length > 0)
               {
                  ++fProgressInfo.NumDsis;
               }
               fProgressInfo.NumProbes += dsiItem.Dsi.probes.Length;
            }
         }
         fProgressInfo.CurrDsi = 0;
         fProgressInfo.CurrProbe = 0;
         this.UpdateProgress();
      }

      /// <summary>
      /// Update the progress UI with the latest state.
      /// </summary>
      private void UpdateProgress(bool waiting = false)
      {
         if (this.InvokeRequired)
         {
            this.Invoke(new Action(() => this.UpdateProgress(waiting)));
            return;
         }

         fProgressInfo.Waiting = waiting;
         this.dsiGrid.SelectDsi(fCurrentDsiIndex);
         this.dsiProgress.UpdateProgress(fProgressInfo);
      }

      /// <summary>
      /// Set the data model scroll/zoom from our settings defaults
      /// if we're told to do so.
      /// </summary>
      private void InitProbeDrawingInfo()
      {
         if ((bool)AStateController.Instance.GlobalData["ZoomScrollFromSettings"])
         {
            foreach (var dsi in (fLoggerMode ? fMasterDsiInfo.DsiItemsSansLogger : fMasterDsiInfo.DsiItems))
            {
               foreach (AProbe probe in dsi.Dsi.probes)
               {
                  probe.Scroll = Properties.Settings.Default.AscanScroll;
                  probe.Zoom = Properties.Settings.Default.AscanZoom;
               }
            }
         }
      }

      /// <summary>
      /// Save the scroll/zoom info for ascan graphs to the data model.
      /// Note that for each DSI
      /// </summary>
      private void SaveProbeDrawingInfo()
      {
         List<AscanWaveform> graphs = this.ascanList.Ascans;
         if (null != graphs)
         {
            foreach (var dsi in (fLoggerMode ? fMasterDsiInfo.DsiItemsSansLogger : fMasterDsiInfo.DsiItems))
            {
               int i = 0;
               foreach (AProbe probe in dsi.Dsi.probes)
               {
                  probe.Scroll = i < graphs.Count ? graphs[i].GraphScrollPercent : Properties.Settings.Default.AscanScroll;
                  probe.Zoom = i < graphs.Count ? graphs[i].GraphZoomPercent : Properties.Settings.Default.AscanScroll;
                  ++i;
               }
            }
         }
      }

      /// <summary>
      /// Update the probe list for the given DSI index.
      /// </summary>
      /// <param name="dsiIndex">index of the DSI</param>
      private void UpdateProbeList(int dsiIndex)
      {
         if (this.InvokeRequired)
         {
            this.Invoke(new Action(() => this.UpdateProbeList(dsiIndex)));
            return;
         }

         Debug.Assert(dsiIndex <= fMasterDsiInfo.DsiItems.Count);
         this.ascanList.PopulateWithProbes(fMasterDsiInfo.DsiItems[dsiIndex].Dsi.probes);
      }

      /// <summary>
      /// Update all the DSI states on the grid.
      /// </summary>
      /// <param name="dsiInfo"></param>
      /// <param name="probes"></param>
      private void UpdateDsiGrid(List<ANanoSense> items)
      {
         if (this.InvokeRequired)
         {
            this.Invoke(new Action(() => this.UpdateDsiGrid(items)));
            return;
         }

         foreach (ANanoSense item in items)
         {
            this.dsiGrid.AddDsi(item.Dsi, item.Dsi.probes.CurrentAlertState());
         }

         // if commissioning, any DSI that's in our list that has not yet been commissioned should be displayed as empty
         if (fCommissioning)
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
      /// Perform the actual update of the site info, according to the params.
      /// </summary>
      /// <param name="company"></param>
      /// <param name="collectionPoint"></param>
      private void UpdateSiteInfo(ANanoSense dsi)
      {
         if (this.InvokeRequired)
         {
            this.Invoke(new Action(() => this.UpdateSiteInfo(dsi)));
            return;
         }

         this.locationInfo.PopulateWithDsi(dsi);
      }

      /// <summary>
      /// Update DSI info to that of the given index
      /// </summary>
      /// <param name="dsiIndex"></param>
      private void UpdateDsiInfo(int dsiIndex)
      {
         if (this.InvokeRequired)
         {
            this.Invoke(new Action(() => this.UpdateDsiInfo(dsiIndex)));
            return;
         }

         ANanoSense nano = fMasterDsiInfo.DsiItems[dsiIndex];
         this.UpdateSiteInfo(nano);
         this.SaveProbeDrawingInfo();
         this.UpdateProbeList(dsiIndex);
         dsiStats.PopulateWithDsi(nano.Dsi);
      }
   }
}

