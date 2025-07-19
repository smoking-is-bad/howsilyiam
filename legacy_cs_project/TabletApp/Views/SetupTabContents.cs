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
using System.Diagnostics;
using TabletApp.State;
using TabletApp.Content.Commissioning;
using TabletApp.Utils;

namespace TabletApp.Views
{
   /// <summary>
   /// Contents of a setup tab in the Probe Params screen.
   /// </summary>
   public partial class SetupTabContents : UserControl
   {
      public bool Dirty { get; set; }

      public SetupTabContents()
      {
         InitializeComponent();

         UpDownControl tempControl = new UpDownControl();
         // move the gate 1 up/down controls to the left of the edit boxes
         int offset = (AScaleUtils.NeedsScale ? -20 : 1);
         this.gateColumn1.StartTextBox.SetUpDownOffset(-(this.gateColumn1.StartTextBox.Width + tempControl.Width + offset));
         this.gateColumn1.WidthTextBox.SetUpDownOffset(-(this.gateColumn1.WidthTextBox.Width + tempControl.Width + offset));
         this.gateColumn1.ThresholdTextBox.SetUpDownOffset(-(this.gateColumn1.ThresholdTextBox.Width + tempControl.Width + offset));
         this.pulserWidth.EntryCompleteEvent += setupParam_EntryComplete;
         this.gain.EntryCompleteEvent += setupParam_EntryComplete;
         this.averages.SelectedIndexChanged += averages_SelectedIndexChanged;
         this.ascanStart.EntryCompleteEvent += setupParam_EntryComplete;
         this.agcCheckbox.CheckedChanged += agcCheckbox_CheckedChanged;
         this.averages.SelectedIndex = 0;
         this.Dirty = false;
         this.ConfigureUnits();
      }

      /// <summary>
      /// Setup our unit labels and text box params (num decimal places, min/max, etc)
      /// </summary>
      private void ConfigureUnits()
      {
         AUnitUtils.ConfigureStartWidthUnits(this.ascanStart, this.ascanStartUnits);
         AUnitUtils.ConfigureStartWidthUnits(this.gateColumn1.StartTextBox, this.gateStartUnits);
         AUnitUtils.ConfigureStartWidthUnits(this.gateColumn1.WidthTextBox, this.gateWidthUnits);
         AUnitUtils.ConfigureStartWidthUnits(this.gateColumn2.StartTextBox, this.gateStartUnits);
         AUnitUtils.ConfigureStartWidthUnits(this.gateColumn2.WidthTextBox, this.gateWidthUnits);
      }

      /// <summary>
      /// Has any param been modified since last save.
      /// </summary>
      /// <returns>true if dirty</returns>
      public bool IsDirty()
      {
         return this.Dirty || this.gateColumn1.Dirty || this.gateColumn2.Dirty;
      }

      /// <summary>
      /// Populate the contents with the given setup, including the gate data.
      /// </summary>
      /// <param name="setup"></param>
      /// <param name="probe"></param>
      public void PopulateWithSetup(ASetup setup, AProbe probe)
      {
         AUnitUtils.LoadPulserWidth(setup, this.pulserWidth);
         AUnitUtils.LoadGain(setup, this.gain);
         AUnitUtils.LoadProbeNumber(probe, setup, this.probeNumber);
         AUnitUtils.LoadAverages(probe, setup, this.averages);
         AUnitUtils.LoadAscanStart(probe, setup, this.ascanStart);
         // mode dictates measurement mode (0x80 in upper byte) and AGC setting (0x101 in lower 24 bits)
         this.measurementModeZero.Checked = (0 == (0x8000 & setup.gates[0].mode));
         this.agcCheckbox.Checked = (0x101 == (0x101 & setup.gates[0].mode));

         GateColumn[] columns = { this.gateColumn1, this.gateColumn2 };
         Debug.Assert(setup.gates.Length <= 3);
         int columnIndex = 0;
         foreach (AGate gate in setup.gates)
         {
            if (columnIndex > 1)
            {
               // only allow 2 gates for now
               break;
            }
            columns[columnIndex].PopulateWithGate(gate, probe);
            ++columnIndex;
         }
      }

      /// <summary>
      /// Save the current state of the controls to the given setup
      /// </summary>
      /// <param name="setup"></param>
      public void SaveToSetup(ASetup setup, AProbe probe)
      {
         AUnitUtils.StorePulserWidth(setup, this.pulserWidth);
         AUnitUtils.StoreGain(setup, this.gain);
         AUnitUtils.StoreAverages(probe, setup, this.averages);
         AUnitUtils.StoreAscanStart(probe, setup, this.ascanStart);

         GateColumn[] columns = { this.gateColumn1, this.gateColumn2 };
         Debug.Assert(setup.gates.Length <= 3);
         int columnIndex = 0;
         foreach (AGate gate in setup.gates)
         {
            // mode dictates measurement mode (0x80 in upper byte) and AGC setting (0x101 in lower 24 bits)
            // note that AGC "off" for mode 1 is 1, while it's 0 for mode 0
            bool modeZero = this.measurementModeZero.Checked;
            bool agc = this.agcCheckbox.Checked;
            gate.mode = (ushort)((modeZero ? 0 : 0x8000) | (agc ? 0x101 : (modeZero ? 0 : 1)));
            if (columnIndex > 1)
            {
               // only allow 2 gates for now
               break;
            }
            columns[columnIndex].SaveToGate(gate, probe);
            ++columnIndex;
         }
         this.Dirty = false;
      }

      /// <summary>
      /// Handle changes to the Mode 0 radio button
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void measurementModeZero_CheckedChanged(object sender, EventArgs e)
      {
         RadioButton radio = (RadioButton)sender;
         this.gateColumn2.SetEnabled(!radio.Checked);
         // disable by setting width to 0
         AGate gate = new AGate();
         if (radio.Checked)
         {
            this.gateColumn2.ClearGate();
         }
         this.Dirty = true;
      }

      void agcCheckbox_CheckedChanged(object sender, EventArgs e)
      {
         this.Dirty = true;
      }

      void averages_SelectedIndexChanged(object sender, EventArgs e)
      {
         this.Dirty = true;
      }

      private void setupParam_EntryComplete(NumberTextBox textBox)
      {
         this.Dirty = true;
      }
   }
}

