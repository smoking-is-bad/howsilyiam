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
   partial class Camera
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Camera));
         this.pictureBox = new System.Windows.Forms.PictureBox();
         this.takePictureButton = new System.Windows.Forms.Button();
         this.deviceComboBox = new System.Windows.Forms.ComboBox();
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
         this.SuspendLayout();
         // 
         // pictureBox
         // 
         resources.ApplyResources(this.pictureBox, "pictureBox");
         this.pictureBox.Name = "pictureBox";
         this.pictureBox.TabStop = false;
         // 
         // takePictureButton
         // 
         this.takePictureButton.BackColor = System.Drawing.Color.Transparent;
         this.takePictureButton.BackgroundImage = global::TabletApp.Properties.Resources.blue_button_big;
         resources.ApplyResources(this.takePictureButton, "takePictureButton");
         this.takePictureButton.FlatAppearance.BorderSize = 0;
         this.takePictureButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
         this.takePictureButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
         this.takePictureButton.ForeColor = System.Drawing.Color.White;
         this.takePictureButton.Name = "takePictureButton";
         this.takePictureButton.UseVisualStyleBackColor = false;
         this.takePictureButton.EnabledChanged += new System.EventHandler(this.takePictureButton_EnabledChanged);
         this.takePictureButton.Click += new System.EventHandler(this.takePictureButton_Click);
         // 
         // deviceComboBox
         // 
         this.deviceComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         resources.ApplyResources(this.deviceComboBox, "deviceComboBox");
         this.deviceComboBox.FormattingEnabled = true;
         this.deviceComboBox.Name = "deviceComboBox";
         this.deviceComboBox.SelectedIndexChanged += new System.EventHandler(this.deviceComboBox_SelectedIndexChanged);
         // 
         // ACamera
         // 
         resources.ApplyResources(this, "$this");
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.deviceComboBox);
         this.Controls.Add(this.takePictureButton);
         this.Controls.Add(this.pictureBox);
         this.Name = "ACamera";
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.PictureBox pictureBox;
      private System.Windows.Forms.Button takePictureButton;
      private System.Windows.Forms.ComboBox deviceComboBox;
   }
}

