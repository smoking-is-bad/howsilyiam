// Copyright (c) 2017 Sensor Networks, Inc.
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
using System.Windows.Forms;
using TabletApp.Utils;
using Model;

namespace TabletApp.Views
{
   /// <summary>
   /// User control that contains the DSI stats.
   /// </summary>
   public partial class DsiStats : UserControl
   {
      /// <summary>
      /// Constructor
      /// </summary>
      public DsiStats()
      {
         InitializeComponent();
      }

      /// <summary>
      /// Populate our UI according to the new dsi.
      /// </summary>
      /// <param name="dsi">The ADsiInfo object</param>
      public void PopulateWithDsi(ADsiInfo dsi)
      {
         if (this.InvokeRequired)
         {
            this.Invoke(new Action(() => this.PopulateWithDsi(dsi)));
            return;
         }

         this.batteryLevel.Text = dsi.BatteryLevelString();
         this.batteryLevel.ForeColor = dsi.BatteryLevelColor();
         this.dsiTemp.Text = dsi.DsiTemperatureString();
         this.materialTemp.Text = dsi.MaterialTemperatureString();
      }
   }
}

