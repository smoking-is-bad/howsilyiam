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
using System.Device.Location;
using TabletApp.State;
using TabletApp.Properties;
using TabletApp.Utils;
using System.Threading;

namespace TabletApp.Views
{
   /// <summary>
   /// User control for displaying latitude and longitude text boxes along with a Get Location button
   /// for using location services to auto-fill those boxes.
   /// </summary>
   public partial class LatLongView : UserControl
   {
      private GeoCoordinateWatcher fWatcher = new GeoCoordinateWatcher();

      public string Latitude 
      { 
         get
         {
            return this.latTextBox.Text;
         }
         set
         {
            try
            {
               this.latTextBox.Value = (float)Convert.ToDouble(value);
            }
            catch (Exception)
            {
               this.latTextBox.Value = 0f;
            }
         }
      }

      public string Longitude
      {
         get
         {
            return this.longTextBox.Text;
         }
         set
         {
            try
            {
               this.longTextBox.Value = (float)Convert.ToDouble(value);
            }
            catch (Exception)
            {
               this.longTextBox.Value = 0f;
            }
         }
      }

      public LatLongView()
      {
         InitializeComponent();
      }

      /// <summary>
      /// Get the user's location via .NET location services.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void getLocationButton_Click(object sender, EventArgs e)
      {
         fWatcher.StatusChanged += watcher_StatusChanged;
         fWatcher.PositionChanged += watcher_PositionChanged;
         fWatcher.Start();
      }

      /// <summary>
      /// Display the found location in the UI.
      /// </summary>
      /// <param name="coord"></param>
      private void SetLocation(GeoCoordinate coord)
      {
         if (coord.IsUnknown != true)
         {
            this.latTextBox.Text = Convert.ToString(coord.Latitude);
            this.longTextBox.Text = Convert.ToString(coord.Longitude);
         }
         else
         {
            AOutput.DisplayError(Resources.ErrorNoLocation);
         }

         fWatcher.StatusChanged -= watcher_StatusChanged;
         fWatcher.PositionChanged -= watcher_PositionChanged;
         fWatcher.Stop();
      }

      private void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
      {
         this.SetLocation(e.Position.Location);
      }

      private void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
      {
         if (e.Status == GeoPositionStatus.Ready)
         {
            this.SetLocation(fWatcher.Position.Location);
         }
      }
   }
}

