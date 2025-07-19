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
using TabletApp.Content;
using TabletApp.Utils;
using TabletApp.Api;
using Model;

namespace TabletApp.Views
{
   /// <summary>
   /// User control that contains the DSI info.
   /// </summary>
   public partial class DsiInfo : UserControl
   {
      /// <summary>
      /// Constructor
      /// </summary>
      public DsiInfo()
      {
         InitializeComponent();
      }

      /// <summary>
      /// Populate our UI according to the new dsi.
      /// </summary>
      /// <param name="dsi">The ANanoSense object</param>
      public void PopulateWithDsi(ANanoSense dsi)
      {
         this.dsData.Text = Convert.ToString(dsi.Dsi.modbusAddress);
         this.tnData.Text = dsi.Dsi.tag;
         this.snData.Text = dsi.Dsi.serialNumber;
         this.gpsData.Text = dsi.Dsi.location.coordinates;
         this.batteryLevel.Text = dsi.Dsi.BatteryLevelString();
         this.batteryLevel.ForeColor = dsi.Dsi.BatteryLevelColor();
         this.dsiTemp.Text = dsi.Dsi.DsiTemperatureString();
         this.materialTemp.Text = dsi.Dsi.MaterialTemperatureString();
         this.lastReading.Text = dsi.Dsi.probes.Timestamp().FormatAsTimestampString();
         this.lastThickness.Text = dsi.Dsi.probes.MinThickness().FormatAsMeasurmentString();
      }
   }
}

