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
namespace TabletApp.Content
{
   partial class DataViewAscan
   {
      /// <summary> 
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary> 
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing && (components != null))
         {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataViewAscan));
         this.dsiGrid = new TabletApp.Views.DsiGrid();
         this.dsiProgress = new TabletApp.Views.DsiReadProgress();
         this.ascanList = new TabletApp.Views.AscanList();
         this.locationInfo = new TabletApp.Views.LocationInfo();
         this.shotNavigator = new TabletApp.Views.ShotNavigator();
         this.dsiStats = new TabletApp.Views.DsiStats();
         this.SuspendLayout();
         // 
         // dsiGrid
         // 
         resources.ApplyResources(this.dsiGrid, "dsiGrid");
         this.dsiGrid.Name = "dsiGrid";
         this.dsiGrid.Selectable = true;
         // 
         // dsiProgress
         // 
         resources.ApplyResources(this.dsiProgress, "dsiProgress");
         this.dsiProgress.Name = "dsiProgress";
         // 
         // ascanList
         // 
         resources.ApplyResources(this.ascanList, "ascanList");
         this.ascanList.BackColor = System.Drawing.Color.Transparent;
         this.ascanList.HandleGraphChange = false;
         this.ascanList.Name = "ascanList";
         // 
         // locationInfo
         // 
         resources.ApplyResources(this.locationInfo, "locationInfo");
         this.locationInfo.BackColor = System.Drawing.Color.Transparent;
         this.locationInfo.Name = "locationInfo";
         // 
         // shotNavigator
         // 
         resources.ApplyResources(this.shotNavigator, "shotNavigator");
         this.shotNavigator.Name = "shotNavigator";
         // 
         // dsiStats
         // 
         resources.ApplyResources(this.dsiStats, "dsiStats");
         this.dsiStats.BackColor = System.Drawing.Color.Transparent;
         this.dsiStats.Name = "dsiStats";
         // 
         // DataViewAscan
         // 
         resources.ApplyResources(this, "$this");
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.dsiStats);
         this.Controls.Add(this.shotNavigator);
         this.Controls.Add(this.locationInfo);
         this.Controls.Add(this.ascanList);
         this.Controls.Add(this.dsiGrid);
         this.Controls.Add(this.dsiProgress);
         this.Name = "DataViewAscan";
         this.ResumeLayout(false);

      }

      #endregion

      private Views.DsiGrid dsiGrid;
      private Views.DsiReadProgress dsiProgress;
      private Views.AscanList ascanList;
      private Views.LocationInfo locationInfo;
      private Views.ShotNavigator shotNavigator;
      private Views.DsiStats dsiStats;
   }
}

