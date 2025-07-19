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
using Model;
using TabletApp.Utils;

namespace TabletApp.Views
{
   /// <summary>
   /// User control that represents an item in the probe list.
   /// </summary>
   public partial class ProbeListItem : UserControl
   {
      private AlertState fAlertState;

      private bool fSelected;
      public bool Selected 
      { 
         get
         {
            return fSelected;
         }
         set
         {
            fSelected = value;
            this.UpdateButtonState();
         }
      }

      public bool Selectable { get; set; }

      /// <summary>
      /// Get the actual probe button object.
      /// </summary>
      public Button ProbeButton
      {
         get
         {
            return this.button;
         }
      }

      /// <summary>
      /// Constructor
      /// </summary>
      public ProbeListItem()
      {
         InitializeComponent();
         this.Selectable = true;
         this.Selected = false;
      }

      /// <summary>
      /// Handle click on the button by changing its selected state.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void button_Click(object sender, EventArgs e)
      {
         if (this.Selectable)
         {
            this.Selected = !this.Selected;
            this.UpdateButtonState();
         }
      }

      /// <summary>
      /// Update the button image per its selected and alert state.
      /// </summary>
      private void UpdateButtonState()
      {
         switch (fAlertState)
         {
            case AlertState.Normal:
               this.button.BackgroundImage = (this.Selected ? Resources.sensor_active : Resources.sensor_inactive);
               this.button.ForeColor = (this.Selected ? Color.White : Color.Black);
               break;
            case AlertState.Warning:
               this.button.BackgroundImage = (this.Selected ? Resources.sensor_yellow : Resources.sensor_yellow_inactive);
               break;
            case AlertState.Error:
               this.button.BackgroundImage = (this.Selected ? Resources.sensor_red : Resources.sensor_red_inactive);
               this.button.ForeColor = (this.Selected ? Color.White : Color.Black);
               break;
         }
      }

      /// <summary>
      /// Populate our UI according to the new AProbe.
      /// </summary>
      /// <param name="probe"></param>
      public void PopulateWithProbe(AProbe probe)
      {
         this.thickness.Text = probe.Thickness().FormatAsMeasurmentString();
         this.timestamp.Text = probe.Timestamp().FormatAsTimestampString();
         this.button.Text = Convert.ToString(probe.num);
         fAlertState = probe.CurrentAlertState();
         this.UpdateButtonState();
      }
   }
}

