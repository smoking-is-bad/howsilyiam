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
namespace TabletApp.Content.Commissioning
{
   partial class ProbeParams
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProbeParams));
         this.label6 = new System.Windows.Forms.Label();
         this.velocity = new TabletApp.Views.FloatTextBox();
         this.warnThicknessUnits = new System.Windows.Forms.Label();
         this.minThicknessUnits = new System.Windows.Forms.Label();
         this.nomThicknessUnits = new System.Windows.Forms.Label();
         this.velocityUnits = new System.Windows.Forms.Label();
         this.warnThickness = new TabletApp.Views.FloatTextBox();
         this.label25 = new System.Windows.Forms.Label();
         this.thickness = new System.Windows.Forms.Label();
         this.label7 = new System.Windows.Forms.Label();
         this.probeLabel = new System.Windows.Forms.Label();
         this.ascan = new TabletApp.Views.AscanWaveform();
         this.label5 = new System.Windows.Forms.Label();
         this.calZeroOffset = new TabletApp.Views.FloatTextBox();
         this.minThickness = new TabletApp.Views.FloatTextBox();
         this.nomThickness = new TabletApp.Views.FloatTextBox();
         this.label9 = new System.Windows.Forms.Label();
         this.label1 = new System.Windows.Forms.Label();
         this.label3 = new System.Windows.Forms.Label();
         this.tabControl = new System.Windows.Forms.TabControl();
         this.model = new System.Windows.Forms.Label();
         this.label2 = new System.Windows.Forms.Label();
         this.description = new System.Windows.Forms.TextBox();
         this.label4 = new System.Windows.Forms.Label();
         this.latLongView = new TabletApp.Views.LatLongView();
         this.copyToAllButton = new System.Windows.Forms.Button();
         this.type = new System.Windows.Forms.Label();
         this.label10 = new System.Windows.Forms.Label();
         this.SuspendLayout();
         // 
         // label6
         // 
         resources.ApplyResources(this.label6, "label6");
         this.label6.Name = "label6";
         // 
         // velocity
         // 
         resources.ApplyResources(this.velocity, "velocity");
         this.velocity.FloatValue = 0F;
         this.velocity.Increment = 1F;
         this.velocity.IntValue = 0;
         this.velocity.MaxValue = 32767F;
         this.velocity.MinValue = -32768F;
         this.velocity.Name = "velocity";
         this.velocity.NumPlaces = 2;
         this.velocity.Value = 0F;
         // 
         // warnThicknessUnits
         // 
         resources.ApplyResources(this.warnThicknessUnits, "warnThicknessUnits");
         this.warnThicknessUnits.Name = "warnThicknessUnits";
         // 
         // minThicknessUnits
         // 
         resources.ApplyResources(this.minThicknessUnits, "minThicknessUnits");
         this.minThicknessUnits.Name = "minThicknessUnits";
         // 
         // nomThicknessUnits
         // 
         resources.ApplyResources(this.nomThicknessUnits, "nomThicknessUnits");
         this.nomThicknessUnits.Name = "nomThicknessUnits";
         // 
         // velocityUnits
         // 
         resources.ApplyResources(this.velocityUnits, "velocityUnits");
         this.velocityUnits.Name = "velocityUnits";
         // 
         // warnThickness
         // 
         resources.ApplyResources(this.warnThickness, "warnThickness");
         this.warnThickness.FloatValue = 0F;
         this.warnThickness.Increment = 1F;
         this.warnThickness.IntValue = 0;
         this.warnThickness.MaxValue = 32767F;
         this.warnThickness.MinValue = -32768F;
         this.warnThickness.Name = "warnThickness";
         this.warnThickness.NumPlaces = 2;
         this.warnThickness.Value = 0F;
         // 
         // label25
         // 
         resources.ApplyResources(this.label25, "label25");
         this.label25.Name = "label25";
         // 
         // thickness
         // 
         resources.ApplyResources(this.thickness, "thickness");
         this.thickness.Name = "thickness";
         // 
         // label7
         // 
         resources.ApplyResources(this.label7, "label7");
         this.label7.Name = "label7";
         // 
         // probeLabel
         // 
         resources.ApplyResources(this.probeLabel, "probeLabel");
         this.probeLabel.Name = "probeLabel";
         // 
         // ascan
         // 
         resources.ApplyResources(this.ascan, "ascan");
         this.ascan.BackColor = System.Drawing.Color.Transparent;
         this.ascan.Name = "ascan";
         this.ascan.TabStop = false;
         // 
         // label5
         // 
         resources.ApplyResources(this.label5, "label5");
         this.label5.Name = "label5";
         // 
         // calZeroOffset
         // 
         resources.ApplyResources(this.calZeroOffset, "calZeroOffset");
         this.calZeroOffset.FloatValue = 50F;
         this.calZeroOffset.Increment = 0.001F;
         this.calZeroOffset.IntValue = 50;
         this.calZeroOffset.MaxValue = 50F;
         this.calZeroOffset.MinValue = 0.001F;
         this.calZeroOffset.Name = "calZeroOffset";
         this.calZeroOffset.NumPlaces = 3;
         this.calZeroOffset.Value = 50F;
         // 
         // minThickness
         // 
         resources.ApplyResources(this.minThickness, "minThickness");
         this.minThickness.FloatValue = 0F;
         this.minThickness.Increment = 1F;
         this.minThickness.IntValue = 0;
         this.minThickness.MaxValue = 100F;
         this.minThickness.MinValue = 0F;
         this.minThickness.Name = "minThickness";
         this.minThickness.NumPlaces = 2;
         this.minThickness.Value = 0F;
         // 
         // nomThickness
         // 
         resources.ApplyResources(this.nomThickness, "nomThickness");
         this.nomThickness.FloatValue = 0F;
         this.nomThickness.Increment = 1F;
         this.nomThickness.IntValue = 0;
         this.nomThickness.MaxValue = 100F;
         this.nomThickness.MinValue = 0F;
         this.nomThickness.Name = "nomThickness";
         this.nomThickness.NumPlaces = 2;
         this.nomThickness.Value = 0F;
         // 
         // label9
         // 
         resources.ApplyResources(this.label9, "label9");
         this.label9.Name = "label9";
         // 
         // label1
         // 
         resources.ApplyResources(this.label1, "label1");
         this.label1.Name = "label1";
         // 
         // label3
         // 
         resources.ApplyResources(this.label3, "label3");
         this.label3.Name = "label3";
         // 
         // tabControl
         // 
         resources.ApplyResources(this.tabControl, "tabControl");
         this.tabControl.Name = "tabControl";
         this.tabControl.SelectedIndex = 0;
         // 
         // model
         // 
         resources.ApplyResources(this.model, "model");
         this.model.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.model.Name = "model";
         // 
         // label2
         // 
         resources.ApplyResources(this.label2, "label2");
         this.label2.Name = "label2";
         // 
         // description
         // 
         resources.ApplyResources(this.description, "description");
         this.description.Name = "description";
         // 
         // label4
         // 
         resources.ApplyResources(this.label4, "label4");
         this.label4.Name = "label4";
         // 
         // latLongView
         // 
         resources.ApplyResources(this.latLongView, "latLongView");
         this.latLongView.BackColor = System.Drawing.Color.Transparent;
         this.latLongView.Latitude = "0.0000";
         this.latLongView.Longitude = "0.0000";
         this.latLongView.Name = "latLongView";
         // 
         // copyToAllButton
         // 
         resources.ApplyResources(this.copyToAllButton, "copyToAllButton");
         this.copyToAllButton.BackColor = System.Drawing.Color.Transparent;
         this.copyToAllButton.BackgroundImage = global::TabletApp.Properties.Resources.blue_button_big;
         this.copyToAllButton.FlatAppearance.BorderSize = 0;
         this.copyToAllButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
         this.copyToAllButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
         this.copyToAllButton.ForeColor = System.Drawing.Color.White;
         this.copyToAllButton.Name = "copyToAllButton";
         this.copyToAllButton.TabStop = false;
         this.copyToAllButton.UseVisualStyleBackColor = false;
         this.copyToAllButton.Click += new System.EventHandler(this.copyToAllButton_Click);
         // 
         // type
         // 
         resources.ApplyResources(this.type, "type");
         this.type.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.type.Name = "type";
         // 
         // label10
         // 
         resources.ApplyResources(this.label10, "label10");
         this.label10.Name = "label10";
         // 
         // AProbeParams
         // 
         resources.ApplyResources(this, "$this");
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.type);
         this.Controls.Add(this.label10);
         this.Controls.Add(this.copyToAllButton);
         this.Controls.Add(this.label6);
         this.Controls.Add(this.velocity);
         this.Controls.Add(this.warnThicknessUnits);
         this.Controls.Add(this.minThicknessUnits);
         this.Controls.Add(this.nomThicknessUnits);
         this.Controls.Add(this.velocityUnits);
         this.Controls.Add(this.warnThickness);
         this.Controls.Add(this.label25);
         this.Controls.Add(this.thickness);
         this.Controls.Add(this.label7);
         this.Controls.Add(this.probeLabel);
         this.Controls.Add(this.ascan);
         this.Controls.Add(this.label5);
         this.Controls.Add(this.calZeroOffset);
         this.Controls.Add(this.minThickness);
         this.Controls.Add(this.nomThickness);
         this.Controls.Add(this.label9);
         this.Controls.Add(this.label1);
         this.Controls.Add(this.label3);
         this.Controls.Add(this.tabControl);
         this.Controls.Add(this.model);
         this.Controls.Add(this.label2);
         this.Controls.Add(this.description);
         this.Controls.Add(this.label4);
         this.Controls.Add(this.latLongView);
         this.Name = "AProbeParams";
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.TextBox description;
      private System.Windows.Forms.Label model;
      private System.Windows.Forms.TabControl tabControl;
      private Views.LatLongView latLongView;
      private Views.FloatTextBox minThickness;
      private Views.FloatTextBox nomThickness;
      private System.Windows.Forms.Label label9;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Label label3;
      private Views.FloatTextBox calZeroOffset;
      private System.Windows.Forms.Label label5;
      private Views.AscanWaveform ascan;
      private System.Windows.Forms.Label probeLabel;
      private System.Windows.Forms.Label label7;
      private System.Windows.Forms.Label thickness;
      private Views.FloatTextBox warnThickness;
      private System.Windows.Forms.Label label25;
      private System.Windows.Forms.Label warnThicknessUnits;
      private System.Windows.Forms.Label minThicknessUnits;
      private System.Windows.Forms.Label nomThicknessUnits;
      private System.Windows.Forms.Label velocityUnits;
      private Views.FloatTextBox velocity;
      private System.Windows.Forms.Label label6;
      private System.Windows.Forms.Button copyToAllButton;
      private System.Windows.Forms.Label type;
      private System.Windows.Forms.Label label10;
   }
}

