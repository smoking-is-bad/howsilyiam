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
using System.Data;
using System.Linq;
using System.Windows.Forms;
using TabletApp.State;
using System.Diagnostics;
using TabletApp.Api;
using TabletApp.Api.DsiApi; // for AUtf8String16
using Model;
using TabletApp.Utils;
using TabletApp.Properties;
using TabletApp.Autofill;
using DsiExtensions;
using DsiApi;

namespace TabletApp.Content.Commissioning
{
   /// <summary>
   /// User control for the 
   /// </summary>
   public partial class DsiData : BaseContent
   {
      private bool fEditMode;
      private int fOrigNumProbes;
      private int fOrigProbeModelIndex;
      private AMasterDsiInfo fMasterDsiInfo;
      private Timer fRtcTimer = new Timer();

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="parameters">Parameters from the state def file</param>
      public DsiData(Dictionary<string, string> parameters)
         : base(parameters)
      {
         InitializeComponent();

         AStateController.Instance.StateWillChangeEvent += HandleStateWillChangeEvent;
         Debug.Assert(AStateController.Instance.GlobalData.ContainsKey("masterdsi"));
         fMasterDsiInfo = (AMasterDsiInfo)AStateController.Instance.GlobalData["masterdsi"];
         Debug.Assert(fMasterDsiInfo.DsiItems.Count > 0);

         this.ConfigureUnits();

         // populate the probe model combo
         this.PopulateProbeModels();
         this.probeModelComboBox.SelectedIndex = 0;      // first by default
         this.uProbeModelComboBox.SelectedIndex = 0;      // first by default

         bool editPath = AStateController.Instance.GlobalData.ContainsKey("editdsi") && "true" == (string)AStateController.Instance.GlobalData["editdsi"];
         fEditMode = (fParams.ContainsKey("mode") && fParams["mode"] == "edit") || editPath;

         // only allow changing the num DSI attribute when editing a DSI
         this.numDsis.Enabled = editPath;

         this.testWebButton.EnabledChanged += testWebButton_EnabledChanged;
         this.testWebButton.Click += testWebButton_Click;
         this.testWebButton.Enabled = false;
         this.webAppUrl.TextChanged += webAppInfo_TextChanged;
         this.webUser.TextChanged += webAppInfo_TextChanged;
         this.webPass.TextChanged += webAppInfo_TextChanged;
         this.testWebSuccessImage.Visible = false;

         this.LoadDsiData();
         this.HandleLoggerDsi();
         this.HandleUpimDsi();
      }

      /// <summary>
      /// Respond to our control being shown - validate the DSI model.
      /// </summary>
      public override void DidShow()
      {
         this.ValidateDsiModel();
      }

      /// <summary>
      /// About to disappear - clean up.
      /// </summary>
      public override void WillDisappear()
      {
         AStateController.Instance.StateWillChangeEvent -= HandleStateWillChangeEvent;
         this.StopRtcTimer();
      }

      /// <summary>
      /// Start the timer that refreshes our real-time clock.
      /// </summary>
      private void StartRtcTimer()
      {
         fRtcTimer.Interval = 1000;
         fRtcTimer.Tick += RtcTimer_Tick;
         fRtcTimer.Start();
      }

      /// <summary>
      /// Stop the timer that refreshes our real-time clock.
      /// </summary>
      private void StopRtcTimer()
      {
         fRtcTimer.Stop();
      }

      /// <summary>
      /// Real-time clock timer proc.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void RtcTimer_Tick(object sender, EventArgs e)
      {
         rtcLabel.Text = DateTime.Now.ToString("hh:mm:ss tt");
      }

      /// <summary>
      /// Make sure the DSI model is a known model.  If not, alert the user.
      /// </summary>
      private void ValidateDsiModel()
      {
         ANanoSense nano = fMasterDsiInfo.DsiItems[fMasterDsiInfo.CurrentDsi];
         ADsiInfo dsiInfo = nano.Dsi;
         string dsiModel = dsiInfo.Model;
         if ("modbus" != dsiModel && "wihart" != dsiModel && "cell" != dsiModel && "logger" != dsiModel && "upim" != dsiModel)
         {
            AOutput.DisplayError(Resources.ErrorInvalidDsiModel);
         }
      }

      /// <summary>
      /// Setup our unit labels and text box params (num decimal places, min/max, etc)
      /// </summary>
      private void ConfigureUnits()
      {
         AUnitUtils.ConfigureVelocityUnits(this.velocity, this.velocityUnits);
         AUnitUtils.ConfigureVelocityUnits(this.uVelocity, this.uVelocityUnits);
         AUnitUtils.ConfigureThicknessUnits(this.nomThickness, this.nomThicknessUnits);
         AUnitUtils.ConfigureThicknessUnits(this.minThickness, this.minThicknessUnits);
         AUnitUtils.ConfigureThicknessUnits(this.warnThickness, this.warnThicknessUnits);
      }

      /// <summary>
      /// Populate the probe model dropdown.
      /// </summary>
      private void PopulateProbeModels()
      {
         IList<AProbe> probes = AAutofillManager.Instance.Source.GetProbeList();
         if (null != probes)
         {
            this.probeModelComboBox.Items.AddRange(probes.Select(probe => probe.model).ToArray());
            this.uProbeModelComboBox.Items.AddRange(probes.Select(probe => probe.model).ToArray());
         }
      }

      /// <summary>
      /// Special handling for logger DSI
      /// </summary>
      private void HandleLoggerDsi()
      {
         ANanoSense nano = fMasterDsiInfo.DsiItems[fMasterDsiInfo.CurrentDsi];
         ADsiInfo dsiInfo = nano.Dsi;
         if (dsiInfo.IsLoggerDsi())
         {
            AStateController.Instance.PostPerformActionEvent("setinfo", Resources.LoggerOffString);
            this.StartRtcTimer();
         }
      }

      /// <summary>
      /// Special handling for micropim DSI
      /// </summary>
      private void HandleUpimDsi()
      {
         ANanoSense nano = fMasterDsiInfo.DsiItems[fMasterDsiInfo.CurrentDsi];
         ADsiInfo dsiInfo = nano.Dsi;
         if (dsiInfo.IsUpimDsi())
         {
            foreach (Control control in this.Controls)
            {
               control.Visible = false;
            }
            this.uPanel.Visible = true;
         }
      }

      /// <summary>
      /// Set the real-time clock on the DSI if it's a logger DSI
      /// </summary>
      private void SetLoggerRtc()
      {
         ANanoSense nano = fMasterDsiInfo.DsiItems[fMasterDsiInfo.CurrentDsi];
         ADsiInfo dsiInfo = nano.Dsi;
         if (dsiInfo.IsLoggerDsi())
         {
            fMasterDsiInfo.Dsi.SetRealTimeClock((byte)dsiInfo.modbusAddress, DateTime.UtcNow);
         }
      }

      /// <summary>
      /// Load the controls with DSI data.
      /// </summary>
      private void LoadDsiData()
      {
         ANanoSense nano = fMasterDsiInfo.DsiItems[fMasterDsiInfo.CurrentDsi];
         ADsiInfo dsiInfo = nano.Dsi;
         bool hideWebLogin = AStateController.Instance.GlobalData.ContainsKey("hideweblogin") && (bool)AStateController.Instance.GlobalData["hideweblogin"];
         
         AStateController.Instance.GlobalData["hideweblogin"] = false;

         this.dsiAddress.Text = Convert.ToString(nano.Dsi.modbusAddress);
         this.serialNumber.Text = dsiInfo.serialNumber;
         this.uSerialNumber.Text = dsiInfo.serialNumber;
         this.tagNumber.Text = dsiInfo.tag;
         this.numDsis.IntValue = dsiInfo.dsiCount;
         this.numProbes.IntValue = dsiInfo.probeCount;
         this.uNumProbes.IntValue = dsiInfo.probeCount;
         if (null != dsiInfo.probes && dsiInfo.probes.Length > 0)
         {
            // Doing simple assignment of the string would fail to find a match because
            // ComboBox must do exact object matching.
            this.probeModelComboBox.SelectedIndex = this.probeModelComboBox.FindStringExact(dsiInfo.probes[0].model);
            this.uProbeModelComboBox.SelectedIndex = this.uProbeModelComboBox.FindStringExact(dsiInfo.probes[0].model);
            AUnitUtils.LoadDsiVelocity(dsiInfo.probes[0], this.velocity);
            AUnitUtils.LoadDsiVelocity(dsiInfo.probes[0], this.uVelocity);
            AUnitUtils.LoadMinThickness(dsiInfo.probes[0], this.minThickness);
            AUnitUtils.LoadNomThickness(dsiInfo.probes[0], this.nomThickness);
            AUnitUtils.LoadWarningThickness(dsiInfo.probes[0], this.warnThickness);
         }
         this.description.Text = (string)dsiInfo.description;
         this.latLongView.Latitude = dsiInfo.Latitude;
         this.latLongView.Longitude = dsiInfo.Longitude;
         //dsiInfo.dsiModel = "cell";     // debug
         string model = dsiInfo.Model;
         this.dsiModel.Text = model;
         this.uDsiModel.Text = model;
         this.dsiWihartData.Visible = ("wihart" == model);
         this.dsiCellData.Visible = ("cell" == model);
         this.dsiLoggerData.Visible = ("logger" == model);
         this.ascanTxInterval.IntValue = dsiInfo.ascanTxInterval;
         this.shotTimeInterval.TotalMinutes = dsiInfo.shotTimeInterval;
         this.uShotInterval.TotalMinutes = dsiInfo.shotTimeInterval;
         this.loggerShotInterval.TotalMinutes = dsiInfo.shotTimeInterval;
         this.transmitTimeInterval.TotalMinutes = dsiInfo.transmitTimeInterval;
         this.webAppUrl.Text = (null != dsiInfo.cloudAppUrl && dsiInfo.cloudAppUrl.Length > 0 ? dsiInfo.cloudAppUrl.FullyQualifiedUri() : Resources.DefaultWebUrl);
         this.webUser.Text = (null != dsiInfo.cloudAppUserName && !hideWebLogin ? dsiInfo.cloudAppUserName : "");
         this.webPass.Text = (null != dsiInfo.cloudAppPassword && !hideWebLogin ? dsiInfo.cloudAppPassword : "");
         this.cellProvider.Text = dsiInfo.cellProvider;
         this.cellUser.Text = dsiInfo.cellNetworkUserName;
         this.cellPass.Text = dsiInfo.cellNetworkPassword;
         this.gsmAccessPoint.Text = dsiInfo.gsmAccessPoint;

         // if editing, track the original number of probes and probe model so we know how to update the probe objects when saving
         fOrigNumProbes = dsiInfo.probeCount;
         fOrigProbeModelIndex = ("upim" == model ? this.uProbeModelComboBox.SelectedIndex: this.probeModelComboBox.SelectedIndex);
      }

      /// <summary>
      /// Write the location data to the DSL per what's stored in our master info (this was all entered
      /// on a previous screen, but we need to save it to each DSI).
      /// </summary>
      private void SaveLocationData()
      {
         ANanoSense nano = fMasterDsiInfo.DsiItems[fMasterDsiInfo.CurrentDsi];
         fMasterDsiInfo.Dsi.WriteCompany((Byte)nano.Dsi.modbusAddress, nano.company);
         fMasterDsiInfo.Dsi.WriteSite((Byte)nano.Dsi.modbusAddress, nano.site);
         fMasterDsiInfo.Dsi.WritePlant((Byte)nano.Dsi.modbusAddress, nano.plant);
         fMasterDsiInfo.Dsi.WriteAsset((Byte)nano.Dsi.modbusAddress, nano.asset);
         fMasterDsiInfo.Dsi.WriteCollectionPoint((Byte)nano.Dsi.modbusAddress, nano.collectionPoint);
      }

      /// <summary>
      /// Save the probe data to the DSI per probe auto-fill info according to what probe model
      /// is selected.
      /// </summary>
      private void SaveProbeData()
      {
         ANanoSense nano = fMasterDsiInfo.DsiItems[fMasterDsiInfo.CurrentDsi];
         ADsiInfo dsiInfo = nano.Dsi;
         ushort startIndex = 0;

         dsiInfo.probeCount = (ushort)this.numProbes.IntValue;
         var newProbeArray = new AProbe[dsiInfo.probeCount];
         ComboBox combo = dsiInfo.IsUpimDsi() ? this.uProbeModelComboBox : this.probeModelComboBox;

         // if we're in the edit mode and we didn't change the probe model, adjust our starting probe index
         if (fEditMode && fOrigProbeModelIndex == combo.SelectedIndex)
         {
            // if we are defining less probes than originally specified, we just use the existing ones
            // so return
            if (dsiInfo.probeCount <= fOrigNumProbes)
            {
               // resize the probes array
               Array.Copy(dsiInfo.probes, newProbeArray, dsiInfo.probeCount);
               dsiInfo.probes = newProbeArray;
               return;
            }
            // if we're defining more probes than originally specified, use the existing probes
            // and define the rest according to the probe autofill file
            else if (dsiInfo.probeCount > fOrigNumProbes)
            {
               startIndex = (ushort)fOrigNumProbes;
               // copy the orig probes to the new array
               Array.Copy(dsiInfo.probes, newProbeArray, fOrigNumProbes);
            }
         }

         dsiInfo.probes = newProbeArray;
         IList<AProbe> autofillProbes = AAutofillManager.Instance.Source.GetProbeList();
         AProbe autofillProbe = autofillProbes[combo.SelectedIndex];

         for (ushort probeIndex = startIndex; probeIndex < dsiInfo.probeCount; ++probeIndex)
         {
            AProbe newProbe = new AProbe(autofillProbe, probeIndex);
            newProbe.setups = new ASetup[autofillProbe.setups.Length];
            newProbe.numSetups = (ushort)autofillProbe.setups.Length;
            AUnitUtils.StoreDsiVelocity(newProbe, this.velocity);
            AUnitUtils.StoreDsiVelocity(newProbe, this.uVelocity);
            AUnitUtils.StoreNomThickness(newProbe, this.nomThickness);
            AUnitUtils.StoreMinThickness(newProbe, this.minThickness);
            AUnitUtils.StoreWarningThickness(newProbe, this.warnThickness);
            newProbe.model = (AUtf8String16)combo.SelectedItem;
            newProbe.location = new AGpsCoordinate(dsiInfo.location.coordinates);
            ushort setupIndex = 0;
            foreach (ASetup autofillSetup in autofillProbe.setups)
            {
               ASetup newSetup = new ASetup(autofillSetup, setupIndex);
               if ("dual" == newProbe.type.ToLower() && probeIndex < AUnitUtils.kMuxDualMode.Length)
               {
                  newSetup.muxSwitchSettings = AUnitUtils.kMuxDualMode[probeIndex];
                  newSetup.switchSettings = AUnitUtils.kSwitchDualMode[4, probeIndex];
               }
               else if ("single" == newProbe.type.ToLower())
               {
                  newSetup.muxSwitchSettings = AUnitUtils.kMuxSingleMode[probeIndex];
                  newSetup.switchSettings = AUnitUtils.kSwitchSingleMode[4, probeIndex];
               }
               newSetup.gates = new AGate[3];      // always add 3 gates
               ushort gateIndex = 0;
               foreach (AGate gate in newSetup.gates)
               {
                  AGate newGate;
                  if (gateIndex < autofillSetup.gates.Length)
                  {
                     newGate = new AGate(autofillSetup.gates[gateIndex]);
                  }
                  else
                  {
                     newGate = new AGate();
                  }
                  newSetup.gates[gateIndex] = newGate;
                  fMasterDsiInfo.Dsi.WriteGate((byte)nano.Dsi.modbusAddress, probeIndex, setupIndex, gateIndex, newGate);
                  ++gateIndex;
               }
               newProbe.setups[setupIndex] = newSetup;
               fMasterDsiInfo.Dsi.WriteSetup((byte)nano.Dsi.modbusAddress, probeIndex, setupIndex, newSetup);
               ++setupIndex;
            }
            fMasterDsiInfo.Dsi.WriteProbe((byte)nano.Dsi.modbusAddress, probeIndex, newProbe);
            dsiInfo.probes[probeIndex] = newProbe;
         }
      }

      /// <summary>
      /// Save all of the DSI data
      /// </summary>
      /// <returns>true on success</returns>
      private bool SaveDsiData()
      {
         bool success = true;

         try
         {
            if (!fEditMode)
            {
               this.SaveLocationData();
            }
            this.SaveProbeData();
            // set the new address and then validate that by reading the dsi info
            ANanoSense nano = fMasterDsiInfo.DsiItems[fMasterDsiInfo.CurrentDsi];
            ADsiInfo dsiInfo = nano.Dsi;
            dsiInfo.tag = this.tagNumber.Text;
            dsiInfo.dsiCount = (ushort)this.numDsis.IntValue;
            dsiInfo.description = this.description.Text;
            dsiInfo.Latitude = this.latLongView.Latitude;
            dsiInfo.Longitude = this.latLongView.Longitude;
            dsiInfo.ascanTxInterval = (ushort)this.ascanTxInterval.IntValue;
            dsiInfo.shotTimeInterval = (ushort)(dsiInfo.IsLoggerDsi() ? this.loggerShotInterval.TotalMinutes : 
               (dsiInfo.IsUpimDsi() ? this.uShotInterval.TotalMinutes : this.shotTimeInterval.TotalMinutes));
            dsiInfo.transmitTimeInterval = (ushort)this.transmitTimeInterval.TotalMinutes;
            dsiInfo.cloudAppUrl = this.webAppUrl.Text.FullyQualifiedUri();
            dsiInfo.cloudAppUserName = this.webUser.Text;
            dsiInfo.cloudAppPassword = this.webPass.Text;
            dsiInfo.cellProvider = this.cellProvider.Text;
            dsiInfo.cellNetworkUserName = this.cellUser.Text;
            dsiInfo.cellNetworkPassword = this.cellPass.Text;
            dsiInfo.gsmAccessPoint = this.gsmAccessPoint.Text;

            this.SetLoggerRtc();

            // !!! TODO: validate the data???
            success = fMasterDsiInfo.Dsi.WriteDsiInfo((Byte)nano.Dsi.modbusAddress, dsiInfo);
            if (!success)
            {
               AOutput.DisplayError(Resources.ErrorCommWriteDsiInfoFailed);
            }
         }
         catch (Exception e)
         {
            AOutput.DisplayError(Resources.ErrorCommWriteDsiInfoException + " " + e.AggregateMessage());
            success = false;
         }

         return success;
      }

      /// <summary>
      /// Handle state change.  Save the DSI data and handle the canceled state.
      /// </summary>
      /// <param name="newState"></param>
      /// <param name="args"></param>
      private void HandleStateWillChangeEvent(AState newState, string actionName, CancelEventArgs args)
      {
         if ("savedsi" == actionName)
         {
            bool success = this.SaveDsiData();
            args.Cancel = !success;
         }
         else if ("cancel" == actionName && !fEditMode)
         {
            if (DialogResult.Yes == AOutput.DisplayYesNo(Resources.CancelDsiDataMessage, Resources.CancelDsiCaption))
            {
               bool replace = (AStateController.Instance.GlobalData.ContainsKey("replacedsi") && "true" == (string)AStateController.Instance.GlobalData["replacedsi"]);
               bool add = (AStateController.Instance.GlobalData.ContainsKey("adddsi") && "true" == (string)AStateController.Instance.GlobalData["adddsi"]);

               // reset the address
               ANanoSense nano = fMasterDsiInfo.DsiItems[fMasterDsiInfo.CurrentDsi];
               fMasterDsiInfo.Dsi.WriteModBusId((Byte)nano.Dsi.modbusAddress, ADsiNetwork.kFactoryId);

               // decrement our dsi count to erase the added one
               if (!replace)
               {
                  --fMasterDsiInfo.CurrentDsi;
               }
               if (add)
               {
                  fMasterDsiInfo.DsiItems.RemoveAt(fMasterDsiInfo.DsiItems.Count - 1);
               }
            }
            else
            {
               args.Cancel = true;
            }
         }
      }

      private void probeModelComboBox_SelectedIndexChanged(object sender, EventArgs e)
      {
         ComboBox combo = (ComboBox)sender;
         IList<AProbe> autofillProbes = AAutofillManager.Instance.Source.GetProbeList();
         AProbe autofillProbe = autofillProbes[combo.SelectedIndex];
         AUnitUtils.LoadDsiVelocity(autofillProbe, this.velocity);
         AUnitUtils.LoadDsiVelocity(autofillProbe, this.uVelocity);
         AUnitUtils.LoadMinThickness(autofillProbe, this.minThickness);
         AUnitUtils.LoadNomThickness(autofillProbe, this.nomThickness);
         AUnitUtils.LoadWarningThickness(autofillProbe, this.warnThickness);
         this.numProbes.MaxValue = ("dual" == autofillProbe.type.ToLower() ? 8 : 16);
         this.uNumProbes.MaxValue = this.numProbes.MaxValue;
         this.numProbes.Value = ("dual" == autofillProbe.type.ToLower() && this.numProbes.Value > 8 ? 8 : this.numProbes.Value);
         this.uNumProbes.Value = this.numProbes.Value;
      }

      private void testWebButton_EnabledChanged(object sender, EventArgs e)
      {
         Button button = sender as Button;
         button.BackgroundImage = (button.Enabled ? Resources.blue_button_small : Resources.gray_button);
      }

      async void testWebButton_Click(object sender, EventArgs e)
      {
         this.testWebSuccessImage.Visible = false;

         this.webAppUrl.Text = this.webAppUrl.Text.FullyQualifiedUri();

         try
         {
            await AApiManager.Instance.LoginAsync(this.webAppUrl.Text, this.webUser.Text, this.webPass.Text).ConfigureAwait(true);
            this.testWebSuccessImage.Image = Resources.upload_success;
         }
         catch (Exception ex)
         {
            this.testWebSuccessImage.Image = Resources.upload_fail;
            AOutput.DisplayError(ex.AggregateMessage());
         }

         try
         {
            await AApiManager.Instance.LogoutAsync().ConfigureAwait(true);
         }
         catch (Exception)
         {
            // silent fail on logout
         } 

         this.testWebSuccessImage.Visible = true;
      }

      void webAppInfo_TextChanged(object sender, EventArgs e)
      {
         this.testWebButton.Enabled = this.webAppUrl.Text.Length > 0 && this.webUser.Text.Length > 0 && this.webPass.Text.Length > 0;
      }
   }
}

