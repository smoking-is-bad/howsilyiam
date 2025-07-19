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
using System.Diagnostics;

namespace TabletApp.Views
{
   /// <summary>
   /// User control that represents an item in the Ascan list.
   /// Shows probe info and the Ascan waveform.
   /// </summary>
   public partial class AscanListItem : UserControl
   {
      public AscanWaveform Graph
      {
         get
         {
            return this.waveform;
         }
      }

      public Button SetAsDefaultButton
      {
         get
         {
            return this.setAsDefaultButton;
         }
      }

      /// <summary>
      /// Constructor
      /// </summary>
      public AscanListItem()
      {
         InitializeComponent();
      }

      /// <summary>
      /// Populate our UI according to the given AProbe.
      /// </summary>
      /// <param name="probe"></param>
      public void PopulateWithProbe(AProbe probe)
      {
         this.thickness.Text = probe.Thickness().FormatAsMeasurmentString();
         this.timestamp.Text = probe.Timestamp().FormatAsTimestampString();
         this.probeNum.Text = String.Format(Resources.ProbeNumber, probe.num);
         this.waveform.PopulateWithProbe(probe);
      }

      private void setAsDefaultButton_Click(object sender, EventArgs e)
      {
         // set the Settings values
         Settings.Default.AscanScroll = this.waveform.GraphScrollPercent;
         Settings.Default.AscanZoom = this.waveform.GraphZoomPercent;
         Settings.Default.Save();
      }
   }
}

