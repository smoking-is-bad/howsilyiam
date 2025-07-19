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
   partial class DataViewSummary
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataViewSummary));
         this.locationInfo = new TabletApp.Views.LocationInfo();
         this.summaryGrid = new TabletApp.Views.SummaryGrid();
         this.SuspendLayout();
         // 
         // locationInfo
         // 
         resources.ApplyResources(this.locationInfo, "locationInfo");
         this.locationInfo.BackColor = System.Drawing.Color.Transparent;
         this.locationInfo.Name = "locationInfo";
         // 
         // summaryGrid
         // 
         resources.ApplyResources(this.summaryGrid, "summaryGrid");
         this.summaryGrid.Name = "summaryGrid";
         // 
         // DataViewSummary
         // 
         resources.ApplyResources(this, "$this");
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.locationInfo);
         this.Controls.Add(this.summaryGrid);
         this.Name = "DataViewSummary";
         this.ResumeLayout(false);

      }

      #endregion

      private Views.SummaryGrid summaryGrid;
      private Views.LocationInfo locationInfo;


   }
}

