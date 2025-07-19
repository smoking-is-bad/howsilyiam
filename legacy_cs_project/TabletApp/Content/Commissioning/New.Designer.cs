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
   partial class New
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(New));
         this.numDsisTextBox = new TabletApp.Views.NumberTextBox();
         this.newCompanyButton = new System.Windows.Forms.Button();
         this.groupBox1 = new System.Windows.Forms.GroupBox();
         this.cpNameTextBox = new System.Windows.Forms.TextBox();
         this.cpDescTextBox = new System.Windows.Forms.TextBox();
         this.label6 = new System.Windows.Forms.Label();
         this.label7 = new System.Windows.Forms.Label();
         this.latLongView = new TabletApp.Views.LatLongView();
         this.label10 = new System.Windows.Forms.Label();
         this.plantComboBox = new System.Windows.Forms.ComboBox();
         this.assetComboBox = new System.Windows.Forms.ComboBox();
         this.siteComboBox = new System.Windows.Forms.ComboBox();
         this.companyComboBox = new System.Windows.Forms.ComboBox();
         this.label3 = new System.Windows.Forms.Label();
         this.label2 = new System.Windows.Forms.Label();
         this.label1 = new System.Windows.Forms.Label();
         this.label4 = new System.Windows.Forms.Label();
         this.groupBox1.SuspendLayout();
         this.SuspendLayout();
         // 
         // numDsisTextBox
         // 
         resources.ApplyResources(this.numDsisTextBox, "numDsisTextBox");
         this.numDsisTextBox.Increment = 1F;
         this.numDsisTextBox.IntValue = 1;
         this.numDsisTextBox.MaxValue = 32F;
         this.numDsisTextBox.MinValue = 1F;
         this.numDsisTextBox.Name = "numDsisTextBox";
         this.numDsisTextBox.Value = 1F;
         // 
         // newCompanyButton
         // 
         this.newCompanyButton.BackColor = System.Drawing.Color.Transparent;
         this.newCompanyButton.BackgroundImage = global::TabletApp.Properties.Resources.blue_button_big;
         resources.ApplyResources(this.newCompanyButton, "newCompanyButton");
         this.newCompanyButton.FlatAppearance.BorderSize = 0;
         this.newCompanyButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
         this.newCompanyButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
         this.newCompanyButton.ForeColor = System.Drawing.Color.White;
         this.newCompanyButton.Name = "newCompanyButton";
         this.newCompanyButton.TabStop = false;
         this.newCompanyButton.UseVisualStyleBackColor = false;
         this.newCompanyButton.Click += new System.EventHandler(this.newCompanyButton_Click);
         // 
         // groupBox1
         // 
         this.groupBox1.Controls.Add(this.cpNameTextBox);
         this.groupBox1.Controls.Add(this.cpDescTextBox);
         this.groupBox1.Controls.Add(this.label6);
         this.groupBox1.Controls.Add(this.label7);
         this.groupBox1.Controls.Add(this.latLongView);
         resources.ApplyResources(this.groupBox1, "groupBox1");
         this.groupBox1.Name = "groupBox1";
         this.groupBox1.TabStop = false;
         // 
         // cpNameTextBox
         // 
         resources.ApplyResources(this.cpNameTextBox, "cpNameTextBox");
         this.cpNameTextBox.Name = "cpNameTextBox";
         // 
         // cpDescTextBox
         // 
         resources.ApplyResources(this.cpDescTextBox, "cpDescTextBox");
         this.cpDescTextBox.Name = "cpDescTextBox";
         // 
         // label6
         // 
         resources.ApplyResources(this.label6, "label6");
         this.label6.Name = "label6";
         // 
         // label7
         // 
         resources.ApplyResources(this.label7, "label7");
         this.label7.Name = "label7";
         // 
         // latLongView
         // 
         this.latLongView.BackColor = System.Drawing.Color.Transparent;
         this.latLongView.Latitude = "0.0000";
         resources.ApplyResources(this.latLongView, "latLongView");
         this.latLongView.Longitude = "0.0000";
         this.latLongView.Name = "latLongView";
         // 
         // label10
         // 
         resources.ApplyResources(this.label10, "label10");
         this.label10.Name = "label10";
         // 
         // plantComboBox
         // 
         this.plantComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
         this.plantComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
         resources.ApplyResources(this.plantComboBox, "plantComboBox");
         this.plantComboBox.FormattingEnabled = true;
         this.plantComboBox.Name = "plantComboBox";
         // 
         // assetComboBox
         // 
         this.assetComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
         this.assetComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
         resources.ApplyResources(this.assetComboBox, "assetComboBox");
         this.assetComboBox.FormattingEnabled = true;
         this.assetComboBox.Name = "assetComboBox";
         // 
         // siteComboBox
         // 
         this.siteComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
         this.siteComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
         resources.ApplyResources(this.siteComboBox, "siteComboBox");
         this.siteComboBox.FormattingEnabled = true;
         this.siteComboBox.Name = "siteComboBox";
         // 
         // companyComboBox
         // 
         this.companyComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
         this.companyComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
         this.companyComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         resources.ApplyResources(this.companyComboBox, "companyComboBox");
         this.companyComboBox.FormattingEnabled = true;
         this.companyComboBox.Name = "companyComboBox";
         this.companyComboBox.SelectedIndexChanged += new System.EventHandler(this.companyComboBox_SelectedIndexChanged);
         // 
         // label3
         // 
         resources.ApplyResources(this.label3, "label3");
         this.label3.Name = "label3";
         // 
         // label2
         // 
         resources.ApplyResources(this.label2, "label2");
         this.label2.Name = "label2";
         // 
         // label1
         // 
         resources.ApplyResources(this.label1, "label1");
         this.label1.Name = "label1";
         // 
         // label4
         // 
         resources.ApplyResources(this.label4, "label4");
         this.label4.Name = "label4";
         // 
         // ANew
         // 
         resources.ApplyResources(this, "$this");
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.BackColor = System.Drawing.Color.Transparent;
         this.Controls.Add(this.numDsisTextBox);
         this.Controls.Add(this.newCompanyButton);
         this.Controls.Add(this.groupBox1);
         this.Controls.Add(this.label10);
         this.Controls.Add(this.plantComboBox);
         this.Controls.Add(this.assetComboBox);
         this.Controls.Add(this.siteComboBox);
         this.Controls.Add(this.companyComboBox);
         this.Controls.Add(this.label3);
         this.Controls.Add(this.label2);
         this.Controls.Add(this.label1);
         this.Controls.Add(this.label4);
         this.Name = "ANew";
         this.groupBox1.ResumeLayout(false);
         this.groupBox1.PerformLayout();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.Label label6;
      private System.Windows.Forms.Label label7;
      private System.Windows.Forms.ComboBox companyComboBox;
      private System.Windows.Forms.ComboBox siteComboBox;
      private System.Windows.Forms.ComboBox assetComboBox;
      private System.Windows.Forms.ComboBox plantComboBox;
      private System.Windows.Forms.Label label10;
      private System.Windows.Forms.GroupBox groupBox1;
      private System.Windows.Forms.TextBox cpDescTextBox;
      private System.Windows.Forms.Button newCompanyButton;
      private System.Windows.Forms.TextBox cpNameTextBox;
      private Views.NumberTextBox numDsisTextBox;
      private Views.LatLongView latLongView;
   }
}

