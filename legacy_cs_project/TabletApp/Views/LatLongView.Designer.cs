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
   partial class LatLongView
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LatLongView));
         this.label8 = new System.Windows.Forms.Label();
         this.label9 = new System.Windows.Forms.Label();
         this.getLocationButton = new System.Windows.Forms.Button();
         this.longTextBox = new TabletApp.Views.FloatNegTextBox();
         this.latTextBox = new TabletApp.Views.FloatNegTextBox();
         this.SuspendLayout();
         // 
         // label8
         // 
         resources.ApplyResources(this.label8, "label8");
         this.label8.Name = "label8";
         // 
         // label9
         // 
         resources.ApplyResources(this.label9, "label9");
         this.label9.Name = "label9";
         // 
         // getLocationButton
         // 
         this.getLocationButton.BackColor = System.Drawing.Color.Transparent;
         this.getLocationButton.BackgroundImage = global::TabletApp.Properties.Resources.blue_button_big;
         resources.ApplyResources(this.getLocationButton, "getLocationButton");
         this.getLocationButton.FlatAppearance.BorderSize = 0;
         this.getLocationButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
         this.getLocationButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
         this.getLocationButton.ForeColor = System.Drawing.Color.White;
         this.getLocationButton.Name = "getLocationButton";
         this.getLocationButton.UseVisualStyleBackColor = false;
         this.getLocationButton.Click += new System.EventHandler(this.getLocationButton_Click);
         // 
         // longTextBox
         // 
         this.longTextBox.FloatValue = 0F;
         resources.ApplyResources(this.longTextBox, "longTextBox");
         this.longTextBox.Increment = 0.0001F;
         this.longTextBox.IntValue = 0;
         this.longTextBox.MaxValue = 180F;
         this.longTextBox.MinValue = -180F;
         this.longTextBox.Name = "longTextBox";
         this.longTextBox.NumPlaces = 4;
         this.longTextBox.Value = 0F;
         // 
         // latTextBox
         // 
         this.latTextBox.FloatValue = 0F;
         resources.ApplyResources(this.latTextBox, "latTextBox");
         this.latTextBox.Increment = 0.0001F;
         this.latTextBox.IntValue = 0;
         this.latTextBox.MaxValue = 90F;
         this.latTextBox.MinValue = -90F;
         this.latTextBox.Name = "latTextBox";
         this.latTextBox.NumPlaces = 4;
         this.latTextBox.Value = 0F;
         // 
         // ALatLongView
         // 
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
         this.BackColor = System.Drawing.Color.Transparent;
         this.Controls.Add(this.label8);
         this.Controls.Add(this.label9);
         this.Controls.Add(this.longTextBox);
         this.Controls.Add(this.latTextBox);
         this.Controls.Add(this.getLocationButton);
         this.Name = "ALatLongView";
         resources.ApplyResources(this, "$this");
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Label label8;
      private System.Windows.Forms.Label label9;
      private FloatNegTextBox longTextBox;
      private FloatNegTextBox latTextBox;
      private System.Windows.Forms.Button getLocationButton;
   }
}

