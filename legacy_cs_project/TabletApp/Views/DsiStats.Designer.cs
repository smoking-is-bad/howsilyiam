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
namespace TabletApp.Views
{
   partial class DsiStats
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DsiStats));
         this.label5 = new System.Windows.Forms.Label();
         this.label6 = new System.Windows.Forms.Label();
         this.lastThickness = new System.Windows.Forms.Label();
         this.batteryLevel = new System.Windows.Forms.Label();
         this.dsiTemp = new System.Windows.Forms.Label();
         this.materialTemp = new System.Windows.Forms.Label();
         this.label3 = new System.Windows.Forms.Label();
         this.SuspendLayout();
         // 
         // label5
         // 
         resources.ApplyResources(this.label5, "label5");
         this.label5.Name = "label5";
         // 
         // label6
         // 
         resources.ApplyResources(this.label6, "label6");
         this.label6.Name = "label6";
         // 
         // lastThickness
         // 
         resources.ApplyResources(this.lastThickness, "lastThickness");
         this.lastThickness.Name = "lastThickness";
         // 
         // batteryLevel
         // 
         resources.ApplyResources(this.batteryLevel, "batteryLevel");
         this.batteryLevel.Name = "batteryLevel";
         // 
         // dsiTemp
         // 
         resources.ApplyResources(this.dsiTemp, "dsiTemp");
         this.dsiTemp.Name = "dsiTemp";
         // 
         // materialTemp
         // 
         resources.ApplyResources(this.materialTemp, "materialTemp");
         this.materialTemp.Name = "materialTemp";
         // 
         // label3
         // 
         resources.ApplyResources(this.label3, "label3");
         this.label3.Name = "label3";
         // 
         // DsiStats
         // 
         resources.ApplyResources(this, "$this");
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.BackColor = System.Drawing.Color.Transparent;
         this.Controls.Add(this.materialTemp);
         this.Controls.Add(this.label3);
         this.Controls.Add(this.dsiTemp);
         this.Controls.Add(this.batteryLevel);
         this.Controls.Add(this.lastThickness);
         this.Controls.Add(this.label6);
         this.Controls.Add(this.label5);
         this.Name = "DsiStats";
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion
      private System.Windows.Forms.Label label5;
      private System.Windows.Forms.Label label6;
      private System.Windows.Forms.Label lastThickness;
      private System.Windows.Forms.Label batteryLevel;
      private System.Windows.Forms.Label dsiTemp;
      private System.Windows.Forms.Label materialTemp;
      private System.Windows.Forms.Label label3;
   }
}

