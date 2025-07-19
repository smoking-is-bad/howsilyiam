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
   partial class DataViewThickness
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataViewThickness));
         this.selectInfo = new System.Windows.Forms.Label();
         this.shotNavigator = new TabletApp.Views.ShotNavigator();
         this.probeList = new TabletApp.Views.ProbeList();
         this.dsiInfo = new TabletApp.Views.DsiInfo();
         this.locationInfo = new TabletApp.Views.LocationInfo();
         this.dsiGrid = new TabletApp.Views.DsiGrid();
         this.SuspendLayout();
         // 
         // selectInfo
         // 
         resources.ApplyResources(this.selectInfo, "selectInfo");
         this.selectInfo.Name = "selectInfo";
         // 
         // shotNavigator
         // 
         resources.ApplyResources(this.shotNavigator, "shotNavigator");
         this.shotNavigator.Name = "shotNavigator";
         // 
         // probeList
         // 
         resources.ApplyResources(this.probeList, "probeList");
         this.probeList.BackColor = System.Drawing.Color.Transparent;
         this.probeList.Name = "probeList";
         this.probeList.Selectable = false;
         // 
         // dsiInfo
         // 
         this.dsiInfo.BackColor = System.Drawing.Color.Transparent;
         resources.ApplyResources(this.dsiInfo, "dsiInfo");
         this.dsiInfo.Name = "dsiInfo";
         // 
         // locationInfo
         // 
         this.locationInfo.BackColor = System.Drawing.Color.Transparent;
         resources.ApplyResources(this.locationInfo, "locationInfo");
         this.locationInfo.Name = "locationInfo";
         // 
         // dsiGrid
         // 
         resources.ApplyResources(this.dsiGrid, "dsiGrid");
         this.dsiGrid.Name = "dsiGrid";
         this.dsiGrid.Selectable = true;
         // 
         // DataViewThickness
         // 
         resources.ApplyResources(this, "$this");
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.BackColor = System.Drawing.Color.Transparent;
         this.Controls.Add(this.shotNavigator);
         this.Controls.Add(this.selectInfo);
         this.Controls.Add(this.probeList);
         this.Controls.Add(this.dsiInfo);
         this.Controls.Add(this.locationInfo);
         this.Controls.Add(this.dsiGrid);
         this.Name = "DataViewThickness";
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private Views.DsiGrid dsiGrid;
      private Views.LocationInfo locationInfo;
      private Views.DsiInfo dsiInfo;
      private Views.ProbeList probeList;
      private System.Windows.Forms.Label selectInfo;
      private Views.ShotNavigator shotNavigator;
   }
}

