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
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabletApp.Views;
using TabletApp.Properties;
using TabletApp.State;
using System.Diagnostics;
using TabletApp.Api;
using Model;
using DsiExtensions;
using TabletApp.Utils;
using DsiApi;
using TabletApp.Autofill;

namespace TabletApp.Content.Commissioning
{
   /// <summary>
   /// User control for gathering the probe params.
   /// </summary>
   public partial class ProbeParams : BaseContent
   {
      private int fNumSetups = 0;
      private AMasterDsiInfo fMasterDsiInfo;
      private object fLockObject = new object();
      private volatile bool fAutoRefresh = true;
      private volatile bool fPerformRefresh = true;
      private Task fAutoRefreshTask = null;
      private bool fDirty = false;
      private volatile bool fCopyToAll = false;
      private bool fFirstRefresh = true;

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="parameters">Parameters from the state def file</param>
      public ProbeParams(Dictionary<string, string> parameters)
         : base(parameters)
      {
         InitializeComponent();
         Debug.Assert(AStateController.Instance.GlobalData.ContainsKey("masterdsi"));
         fMasterDsiInfo = (AMasterDsiInfo)AStateController.Instance.GlobalData["masterdsi"];
         Debug.Assert(fMasterDsiInfo.DsiItems.Count > 0);

         AStateController.Instance.PerformActionEvent += HandlePerformAction;
         AStateController.Instance.StateWillChangeEvent += HandleStateWillChangeEvent;

         this.velocity.EntryCompleteEvent += probeParam_EntryComplete;
         this.nomThickness.EntryCompleteEvent += probeParam_EntryComplete;
         this.minThickness.EntryCompleteEvent += probeParam_EntryComplete;
         this.warnThickness.EntryCompleteEvent += probeParam_EntryComplete;
         this.calZeroOffset.EntryCompleteEvent += probeParam_EntryComplete;

         this.ConfigureUnits();
         this.PopulateProbeData();
         this.InitScrollZoom();

         fAutoRefreshTask = Task.Run(
             async () =>
             {
                await Task.Delay(300);
                while (fAutoRefresh)
                {
                   while (!fPerformRefresh)
                   {
                      await Task.Delay(100);
                   }
                   fPerformRefresh = false;
                   this.SaveProbeData();
                   await this.UpdateAscan();
                }
             }
          );
      }

      /// <summary>
      /// About to disappear - clean up.
      /// </summary>
      public override void WillDisappear()
      {
         AStateController.Instance.PerformActionEvent -= HandlePerformAction;
         AStateController.Instance.StateWillChangeEvent -= HandleStateWillChangeEvent;
         fAutoRefresh = false;
      }

      /// <summary>
      /// Initialize our scroll/zoom to use in this view, only if it hasn't yet been defined.
      /// Upon first visit to ProbeParams, it should default to the settings values.
      /// Thereafter, when visiting other probes via next/previous, it should carry over
      /// the scroll/zoom if the LockAscan setting is "on".
      /// </summary>
      private void InitScrollZoom()
      {
         if (!AStateController.Instance.GlobalData.ContainsKey("ProbeParamsScroll") ||
             !AStateController.Instance.GlobalData.ContainsKey("ProbeParamsZoom"))
         {
            AStateController.Instance.GlobalData["ProbeParamsScroll"] = TabletApp.Properties.Settings.Default.AscanScroll;
            AStateController.Instance.GlobalData["ProbeParamsZoom"] = TabletApp.Properties.Settings.Default.AscanZoom;
         }
      }

      /// <summary>
      /// Clear the scroll/zoom for this view from the global state data.
      /// </summary>
      private void ClearScrollZoom()
      {
         AStateController.Instance.GlobalData.Remove("ProbeParamsScroll");
         AStateController.Instance.GlobalData.Remove("ProbeParamsZoom");
      }

      /// <summary>
      /// Save the current scroll/zoom to the global state data.
      /// </summary>
      private void SaveScrollZoom()
      {
         AStateController.Instance.GlobalData["ProbeParamsScroll"] = this.ascan.GraphScrollPercent;
         AStateController.Instance.GlobalData["ProbeParamsZoom"] = this.ascan.GraphZoomPercent;
      }

      /// <summary>
      /// Apply the current scroll/zoom from the global state data to the given probe.
      /// </summary>
      /// <param name="probe"></param>
      private void ApplyScrollZoomToProbe(AProbe probe)
      {
         probe.Scroll = (int)AStateController.Instance.GlobalData["ProbeParamsScroll"];
         probe.Zoom = (int)AStateController.Instance.GlobalData["ProbeParamsZoom"];
      }

      /// <summary>
      /// Display the error and revert back to main commissioning screen.
      /// </summary>
      /// <param name="error">Error string</param>
      private void HandleCriticalError(string error)
      {
         if (this.InvokeRequired)
         {
            this.Invoke(new Action(() => this.HandleCriticalError(error)));
            return;
         }

         AOutput.DisplayError(error);
         fAutoRefresh = false;
         AStateController.Instance.ChangeToState("comm-main");
      }

      /// <summary>
      /// Update the ascan graph by performing a measurement.
      /// </summary>
      /// <returns></returns>
      private async Task UpdateAscan()
      {
         int probeIndex = (int)AStateController.Instance.GlobalData["probeindex"];
         ANanoSense nano = fMasterDsiInfo.DsiItems[fMasterDsiInfo.CurrentDsi];
         Debug.Assert(null != nano.Dsi.probes && nano.Dsi.probes.Length > probeIndex);
         AProbe theProbe = new AProbe(nano.Dsi.probes[probeIndex], probeIndex);
         await AApiManager.Instance.PerformMeasurementsAsync(fMasterDsiInfo.Dsi, nano, DateTime.UtcNow, theProbe,
            (AMeasurementParam param, AProbe probe, String errorMessage) =>
            {
               if (fAutoRefresh)
               {
                  if (null != errorMessage && errorMessage.Length > 0)
                  {
                     this.HandleCriticalError(Resources.ErrorCommReadProbeFailed + " " + errorMessage);
                  }
                  else
                  {
                     this.UpdateAscanWithProbe(probe);
                  }
               }
            });
      }

      /// <summary>
      /// Update the ascan graph with the given probe.
      /// </summary>
      /// <param name="probe"></param>
      private void UpdateAscanWithProbe(AProbe probe)
      {
         if (this.InvokeRequired)
         {
            this.Invoke(new Action(() => this.UpdateAscanWithProbe(probe)));
            return;
         }

         this.thickness.Text = probe.Thickness().FormatAsMeasurmentString();
         if (fFirstRefresh)
         {
            this.ApplyScrollZoomToProbe(probe);
         }
         this.ascan.PopulateWithProbe(probe, fFirstRefresh);
         this.ascan.Invalidate();
         this.ascan.Update();
         this.ascan.Refresh();

         // refresh the ascan as fast as we can
         if (fAutoRefresh)
         {
            fPerformRefresh = true;
         }
         fFirstRefresh = false;
      }

      /// <summary>
      /// Setup our unit labels and text box params (num decimal places, min/max, etc)
      /// </summary>
      private void ConfigureUnits()
      {
         AUnitUtils.ConfigureVelocityUnits(this.velocity, this.velocityUnits);
         AUnitUtils.ConfigureThicknessUnits(this.nomThickness, this.nomThicknessUnits);
         AUnitUtils.ConfigureThicknessUnits(this.minThickness, this.minThicknessUnits);
         AUnitUtils.ConfigureThicknessUnits(this.warnThickness, this.warnThicknessUnits);
      }

      /// <summary>
      /// Based on the current velocity value, populate our velocity-dependent controls.
      /// </summary>
      private void PopulateVelocityDependentData()
      {
         if (this.InvokeRequired)
         {
            this.Invoke(new Action(() => this.PopulateVelocityDependentData()));
            return;
         }

         int probeIndex = (int)AStateController.Instance.GlobalData["probeindex"];
         ANanoSense nano = fMasterDsiInfo.DsiItems[fMasterDsiInfo.CurrentDsi];
         Debug.Assert(null != nano.Dsi.probes && nano.Dsi.probes.Length > probeIndex);
         AProbe probe = nano.Dsi.probes[probeIndex];

         // store the latest velocity value
         AUnitUtils.StoreDsiVelocity(probe, this.velocity);

         // then update all the relevant UI fields
         AUnitUtils.LoadMinThickness(probe, this.minThickness);
         AUnitUtils.LoadNomThickness(probe, this.nomThickness);
         AUnitUtils.LoadWarningThickness(probe, this.warnThickness);
         int tabIndex = 0;
         foreach (ASetup setup in probe.setups)
         {
            TabPage page = this.tabControl.TabPages[tabIndex];
            SetupTabContents contents = page.Controls[0] as SetupTabContents;
            contents.PopulateWithSetup(setup, probe);
         }
      }

      /// <summary>
      /// Populate our controls with the current state of the probe and all setups/gates.
      /// </summary>
      private void PopulateProbeData()
      {
         int probeIndex = (int)AStateController.Instance.GlobalData["probeindex"];
         ANanoSense nano = fMasterDsiInfo.DsiItems[fMasterDsiInfo.CurrentDsi];
         Debug.Assert(null != nano.Dsi.probes && nano.Dsi.probes.Length > probeIndex);
         AStateController.Instance.PostPerformActionEvent((probeIndex > 0 ? "enablebutton" : "disablebutton"), "previous");
         AStateController.Instance.PostPerformActionEvent((probeIndex < nano.Dsi.probes.Length - 1 ? "enablebutton" : "disablebutton"), "next");
         AProbe probe = nano.Dsi.probes[probeIndex];
         this.probeLabel.Text = String.Format(this.probeLabel.Text, probeIndex + 1);
         this.thickness.Text = probe.Thickness().FormatAsMeasurmentString();
         this.model.Text = probe.model;
         this.type.Text = probe.type;
         this.description.Text = probe.description;
         this.latLongView.Latitude = probe.location.Latitude;
         this.latLongView.Longitude = probe.location.Longitude;
         AUnitUtils.LoadDsiVelocity(probe, this.velocity);
         AUnitUtils.LoadMinThickness(probe, this.minThickness);
         AUnitUtils.LoadNomThickness(probe, this.nomThickness);
         AUnitUtils.LoadWarningThickness(probe, this.warnThickness);
         AUnitUtils.LoadCalZeroOffset(probe, this.calZeroOffset);

         this.tabControl.TabPages.Clear();
         fNumSetups = 0;
         foreach (ASetup setup in probe.setups)
         {
            this.AddSetup(setup, probe);
         }
      }

      /// <summary>
      /// Write the probe data to the device and save it to our master data structure.
      /// </summary>
      /// <returns>true on success</returns>
      private bool SaveProbeData()
      {
         if (this.InvokeRequired)
         {
            return (bool)this.Invoke(new Func<bool>(() => this.SaveProbeData()));
         }

         if (!this.IsDirty() && !fCopyToAll)
         {
            return true;
         }

         bool success = true;
         bool copyToAllProbes = fCopyToAll;

         fCopyToAll = false;

         if (copyToAllProbes)
         {
            AStateController.Instance.PostPerformActionEvent("showspinnerprogress", Resources.CopyToProbesProgress);
         }

         try
         {
            ushort selectedProbeIndex = (ushort)(int)AStateController.Instance.GlobalData["probeindex"];
            ANanoSense nano = fMasterDsiInfo.DsiItems[fMasterDsiInfo.CurrentDsi];
            Debug.Assert(null != nano.Dsi.probes && nano.Dsi.probes.Length > selectedProbeIndex);

            ushort probeIndex = 0;
            foreach (AProbe probe in nano.Dsi.probes)
            {
               // handle the "copy to all probes" setting
               if (!copyToAllProbes && probeIndex != selectedProbeIndex)
               {
                  ++probeIndex;
                  continue;
               }
               if (copyToAllProbes)
               {
                  AStateController.Instance.PostPerformActionEvent("updatespinnerprogress", Resources.CopyToProbesProgress);
               }
               probe.description = this.description.Text;
               probe.location.Latitude = this.latLongView.Latitude;
               probe.location.Longitude = this.latLongView.Longitude;
               AUnitUtils.StoreDsiVelocity(probe, this.velocity);
               AUnitUtils.StoreNomThickness(probe, this.nomThickness);
               AUnitUtils.StoreMinThickness(probe, this.minThickness);
               AUnitUtils.StoreWarningThickness(probe, this.warnThickness);
               if (!copyToAllProbes || (copyToAllProbes && probeIndex == selectedProbeIndex))
               {
                  // only store calzero to current probe - don't copy to others
                  AUnitUtils.StoreCalZeroOffset(probe, this.calZeroOffset);
               }

               success = fMasterDsiInfo.Dsi.WriteProbe((byte)nano.Dsi.modbusAddress, probeIndex, probe);

               if (success)
               {
                  int tabIndex = 0;
                  ushort setupIndex = 0;
                  Debug.Assert(probe.setups.Length == this.tabControl.TabPages.Count);
                  foreach (ASetup setup in probe.setups)
                  {
                     TabPage page = this.tabControl.TabPages[tabIndex];
                     SetupTabContents contents = page.Controls[0] as SetupTabContents;
                     contents.SaveToSetup(setup, probe);
                     success = fMasterDsiInfo.Dsi.WriteSetup((byte)nano.Dsi.modbusAddress, probeIndex, setupIndex, setup);
                     if (!success)
                     {
                        break;
                     }
                     ushort gateIndex = 0;
                     foreach (AGate gate in setup.gates)
                     {
                        success = fMasterDsiInfo.Dsi.WriteGate((byte)nano.Dsi.modbusAddress, probeIndex, setupIndex, gateIndex, gate);
                        if (!success)
                        {
                           break;
                        }
                        ++gateIndex;
                     }
                     ++setupIndex;
                     ++tabIndex;
                  }
               }
               ++probeIndex;
            }
            if (success)
            {
               fDirty = false;
            }
            else
            {
               this.HandleCriticalError(Resources.ErrorCommWriteProbeDataFailed);
            }
         }
         catch (Exception e)
         {
            this.HandleCriticalError(Resources.ErrorCommWriteProbeDataException + " " + e.AggregateMessage());
            success = false;
         }

         if (copyToAllProbes)
         {
            AStateController.Instance.PostPerformActionEvent("hidespinnerprogress", null);
         }
         return success;
      }

      /// <summary>
      /// Add a new setup tab.
      /// </summary>
      private void AddSetup(ASetup setup, AProbe probe)
      {
         ++fNumSetups;
         SetupTabContents contents = new SetupTabContents();
         contents.PopulateWithSetup(setup, probe);
         contents.Location = Point.Empty;
         contents.BackColor = Color.Transparent;
         TabPage tp = new TabPage();
         tp.BackColor = Color.Transparent;
         tp.UseVisualStyleBackColor = true;
         tp.Text = String.Format(Resources.SetupTabTitle, fNumSetups);
         if (null != this.FindForm())
         {
            AScaleUtils.ScaleControl(contents);
         }
         tp.Controls.Add(contents);
         this.tabControl.TabPages.Add(tp);
      }

      /// <summary>
      /// Handle state change.  Save the probe data and handle the canceled state.
      /// </summary>
      /// <param name="newState"></param>
      /// <param name="args"></param>
      private void HandleStateWillChangeEvent(AState newState, string actionName, CancelEventArgs args)
      {
         if ("saveprobe" == actionName || "previous" == actionName || "next" == actionName)
         {
            int probeIndex = (int)AStateController.Instance.GlobalData["probeindex"];
            ANanoSense nano = fMasterDsiInfo.DsiItems[fMasterDsiInfo.CurrentDsi];
            AProbe probe = nano.Dsi.probes[probeIndex];

            // if params not valid, alert the user and allow them to fix
            if (!AUnitUtils.ValidateVelocityDependentParameters(probe))
            {
               AOutput.DisplayError(Resources.ErrorVelocityParams, true);
               args.Cancel = true;
               return;
            }

            fAutoRefresh = false;
            // wait up to 5 seconds for auto-refresh to complete
            if (!fAutoRefreshTask.IsCompleted)
            {
               fAutoRefreshTask.Wait(5000);
            }

            fDirty = true;
            bool success = this.SaveProbeData();

            if ("previous" == actionName)
            {
               AStateController.Instance.GlobalData["probeindex"] = --probeIndex;
            }
            else if ("next" == actionName)
            {
               AStateController.Instance.GlobalData["probeindex"] = ++probeIndex;
            }

            // for next/previous, if LockAscans setting is off, clear out our saved scroll/zoom so it gets reset
            // if we're saving (done), also clear it out
            if ((("previous" == actionName || "next" == actionName) && !TabletApp.Properties.Settings.Default.LockAscans) ||
                "saveprobe" == actionName)
            {
               this.ClearScrollZoom();
            }
            // for next/previous, if LockAscans setting is on, save the scroll/zoom so it gets propagated to the next ProbeParams
            if (("previous" == actionName || "next" == actionName) && TabletApp.Properties.Settings.Default.LockAscans)
            {
               this.SaveScrollZoom();
            }
         }
      }

      /// <summary>
      /// Reset our current probe to the values from the autofill file.
      /// </summary>
      private void ResetProbe()
      {
         int probeIndex = (int)AStateController.Instance.GlobalData["probeindex"];
         ANanoSense nano = fMasterDsiInfo.DsiItems[fMasterDsiInfo.CurrentDsi];
         AProbe probe = nano.Dsi.probes[probeIndex];
         AProbe cleanProbe = null;
         try
         {
            cleanProbe = AAutofillManager.Instance.Source.GetProbeList().First(p => p.model == probe.model);
         }
         catch (Exception)
         {
            // probe model not found in autofill file
            AOutput.DisplayError(Resources.ErrorResetProbe, true);
            return;
         } 
         cleanProbe.numSetups = probe.numSetups;
         probe.CopyFrom(cleanProbe);
         int setupIndex = 0;
         foreach (ASetup setup in probe.setups)
         {
            ASetup cleanSetup = (setupIndex < cleanProbe.setups.Length ? cleanProbe.setups[setupIndex] : cleanProbe.setups[0]);
            setup.CopyFrom(cleanSetup);
            if ("dual" == cleanProbe.type.ToLower() && probeIndex < AUnitUtils.kMuxDualMode.Length)
            {
               setup.muxSwitchSettings = AUnitUtils.kMuxDualMode[probeIndex];
               setup.switchSettings = AUnitUtils.kSwitchDualMode[4, probeIndex];
            }
            else if ("single" == cleanProbe.type.ToLower())
            {
               setup.muxSwitchSettings = AUnitUtils.kMuxSingleMode[probeIndex];
               setup.switchSettings = AUnitUtils.kSwitchSingleMode[4, probeIndex];
            }
            int gateIndex = 0;
            foreach (AGate gate in setup.gates)
            {
               AGate cleanGate = (gateIndex < cleanSetup.gates.Length ? cleanSetup.gates[gateIndex] : cleanSetup.gates[0]);
               gate.CopyValuesFrom(cleanGate);
               ++gateIndex;
            }
            ++setupIndex;
         }
      }

      /// <summary>
      /// Handle our associated actions as needed.
      /// </summary>
      /// <param name="actionName"></param>
      /// <param name="data"></param>
      private void HandlePerformAction(string actionName, object data)
      {
         // reset the probe to its original data from the probe autofill file
         if ("resetprobe" == actionName)
         {
            this.ResetProbe();
            this.PopulateProbeData();
            this.AutoScrollPosition = new Point(0, 0);
         }
      }

      private bool IsDirty()
      {
         bool setupDirty = false;

         foreach (TabPage page in this.tabControl.TabPages)
         {
            SetupTabContents contents = page.Controls[0] as SetupTabContents;
            setupDirty = contents.IsDirty();
         }
         return fDirty || setupDirty;
      }

      private void probeParam_EntryComplete(NumberTextBox textBox)
      {
         fDirty = true;
      }

      private void copyToAllButton_Click(object sender, EventArgs e)
      {
         if (DialogResult.Yes == AOutput.DisplayYesNo(Resources.CopyProbesMessage, Resources.CopyProbesCaption))
         {
            fCopyToAll = true;
         }
      }
   }
}

