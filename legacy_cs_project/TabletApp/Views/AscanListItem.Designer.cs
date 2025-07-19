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
namespace TabletApp.Views
{
   partial class AscanListItem
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AscanListItem));
         this.thickness = new System.Windows.Forms.Label();
         this.probeNum = new System.Windows.Forms.Label();
         this.panel1 = new System.Windows.Forms.Panel();
         this.timestamp = new System.Windows.Forms.Label();
         this.setAsDefaultButton = new System.Windows.Forms.Button();
         this.waveform = new TabletApp.Views.AscanWaveform();
         this.SuspendLayout();
         // 
         // thickness
         // 
         resources.ApplyResources(this.thickness, "thickness");
         this.thickness.Name = "thickness";
         // 
         // probeNum
         // 
         resources.ApplyResources(this.probeNum, "probeNum");
         this.probeNum.Name = "probeNum";
         // 
         // panel1
         // 
         resources.ApplyResources(this.panel1, "panel1");
         this.panel1.BackColor = System.Drawing.Color.LightGray;
         this.panel1.Name = "panel1";
         // 
         // timestamp
         // 
         resources.ApplyResources(this.timestamp, "timestamp");
         this.timestamp.Name = "timestamp";
         // 
         // setAsDefaultButton
         // 
         resources.ApplyResources(this.setAsDefaultButton, "setAsDefaultButton");
         this.setAsDefaultButton.BackColor = System.Drawing.Color.Transparent;
         this.setAsDefaultButton.BackgroundImage = global::TabletApp.Properties.Resources.blue_button_small;
         this.setAsDefaultButton.FlatAppearance.BorderSize = 0;
         this.setAsDefaultButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
         this.setAsDefaultButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
         this.setAsDefaultButton.ForeColor = System.Drawing.Color.White;
         this.setAsDefaultButton.Name = "setAsDefaultButton";
         this.setAsDefaultButton.UseVisualStyleBackColor = false;
         this.setAsDefaultButton.Click += new System.EventHandler(this.setAsDefaultButton_Click);
         // 
         // waveform
         // 
         resources.ApplyResources(this.waveform, "waveform");
         this.waveform.BackColor = System.Drawing.Color.Transparent;
         this.waveform.GraphScroll = 0F;
         this.waveform.GraphScrollPercent = -2147483648;
         this.waveform.GraphZoom = 1F;
         this.waveform.GraphZoomPercent = 100;
         this.waveform.Name = "waveform";
         // 
         // AscanListItem
         // 
         resources.ApplyResources(this, "$this");
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.BackColor = System.Drawing.Color.Transparent;
         this.Controls.Add(this.setAsDefaultButton);
         this.Controls.Add(this.timestamp);
         this.Controls.Add(this.panel1);
         this.Controls.Add(this.probeNum);
         this.Controls.Add(this.thickness);
         this.Controls.Add(this.waveform);
         this.Name = "AscanListItem";
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private AscanWaveform waveform;
      private System.Windows.Forms.Label thickness;
      private System.Windows.Forms.Label probeNum;
      private System.Windows.Forms.Panel panel1;
      private System.Windows.Forms.Label timestamp;
      private System.Windows.Forms.Button setAsDefaultButton;
   }
}

