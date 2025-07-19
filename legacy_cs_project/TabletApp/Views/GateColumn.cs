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
using TabletApp.Utils;

namespace TabletApp.Views
{
   public partial class GateColumn : UserControl
   {
      public bool Dirty { get; set; }
      public FloatTextBox StartTextBox { get { return this.start; } }
      public FloatTextBox WidthTextBox { get { return this.width; } }
      public FloatTextBox ThresholdTextBox { get { return this.threshold; } }

      public GateColumn()
      {
         InitializeComponent();

         this.start.EntryCompleteEvent += gateParam_EntryComplete;
         this.width.EntryCompleteEvent += gateParam_EntryComplete;
         this.threshold.EntryCompleteEvent += gateParam_EntryComplete;
      }

      public void SetEnabled(bool enabled)
      {
         this.start.Enabled = enabled;
         this.width.Enabled = enabled;
         this.threshold.Enabled = enabled;
      }

      public void PopulateWithGate(AGate gate, AProbe probe)
      {
         AUnitUtils.LoadGateStart(probe, gate, this.start);
         AUnitUtils.LoadGateWidth(probe, gate, this.width);
         AUnitUtils.LoadGateThreshold(gate, this.threshold);
      }

      public void SaveToGate(AGate gate, AProbe probe)
      {
         AUnitUtils.StoreGateStart(probe, gate, this.start);
         AUnitUtils.StoreGateWidth(probe, gate, this.width);
         AUnitUtils.StoreGateThreshold(gate, this.threshold);
         this.Dirty = false;
      }

      public void ClearGate()
      {
         this.width.Value = 0;
      }

      private void gateParam_EntryComplete(NumberTextBox textBox)
      {
         this.Dirty = true;
      }
   }
}

