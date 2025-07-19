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
   partial class ProbeListItem
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProbeListItem));
         this.button = new System.Windows.Forms.Button();
         this.thickness = new System.Windows.Forms.Label();
         this.panel1 = new System.Windows.Forms.Panel();
         this.timestamp = new System.Windows.Forms.Label();
         this.SuspendLayout();
         // 
         // button
         // 
         this.button.BackgroundImage = global::TabletApp.Properties.Resources.sensor_inactive;
         resources.ApplyResources(this.button, "button");
         this.button.FlatAppearance.BorderSize = 0;
         this.button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
         this.button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
         this.button.Name = "button";
         this.button.UseVisualStyleBackColor = true;
         this.button.Click += new System.EventHandler(this.button_Click);
         // 
         // thickness
         // 
         resources.ApplyResources(this.thickness, "thickness");
         this.thickness.Name = "thickness";
         // 
         // panel1
         // 
         this.panel1.BackColor = System.Drawing.Color.DarkGray;
         resources.ApplyResources(this.panel1, "panel1");
         this.panel1.Name = "panel1";
         // 
         // timestamp
         // 
         resources.ApplyResources(this.timestamp, "timestamp");
         this.timestamp.Name = "timestamp";
         // 
         // AProbeListItem
         // 
         resources.ApplyResources(this, "$this");
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.BackColor = System.Drawing.Color.Transparent;
         this.Controls.Add(this.timestamp);
         this.Controls.Add(this.thickness);
         this.Controls.Add(this.button);
         this.Controls.Add(this.panel1);
         this.Name = "AProbeListItem";
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Button button;
      private System.Windows.Forms.Label thickness;
      private System.Windows.Forms.Panel panel1;
      private System.Windows.Forms.Label timestamp;
   }
}

