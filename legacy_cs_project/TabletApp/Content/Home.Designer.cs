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
   partial class Home
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Home));
         this.uploadButton = new System.Windows.Forms.Button();
         this.reviewButton = new System.Windows.Forms.Button();
         this.readSensorsButton = new System.Windows.Forms.Button();
         this.commissioningButton = new System.Windows.Forms.Button();
         this.emailButton = new System.Windows.Forms.Button();
         this.readLoggerButton = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // uploadButton
         // 
         resources.ApplyResources(this.uploadButton, "uploadButton");
         this.uploadButton.BackColor = System.Drawing.Color.Transparent;
         this.uploadButton.BackgroundImage = global::TabletApp.Properties.Resources.blue_button_big;
         this.uploadButton.FlatAppearance.BorderSize = 0;
         this.uploadButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
         this.uploadButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
         this.uploadButton.ForeColor = System.Drawing.Color.White;
         this.uploadButton.Name = "uploadButton";
         this.uploadButton.UseVisualStyleBackColor = false;
         this.uploadButton.Click += new System.EventHandler(this.uploadButton_Click);
         // 
         // reviewButton
         // 
         resources.ApplyResources(this.reviewButton, "reviewButton");
         this.reviewButton.BackColor = System.Drawing.Color.Transparent;
         this.reviewButton.BackgroundImage = global::TabletApp.Properties.Resources.blue_button_big;
         this.reviewButton.FlatAppearance.BorderSize = 0;
         this.reviewButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
         this.reviewButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
         this.reviewButton.ForeColor = System.Drawing.Color.White;
         this.reviewButton.Name = "reviewButton";
         this.reviewButton.UseVisualStyleBackColor = false;
         this.reviewButton.Click += new System.EventHandler(this.reviewButton_Click);
         // 
         // readSensorsButton
         // 
         resources.ApplyResources(this.readSensorsButton, "readSensorsButton");
         this.readSensorsButton.BackColor = System.Drawing.Color.Transparent;
         this.readSensorsButton.BackgroundImage = global::TabletApp.Properties.Resources.blue_button_big;
         this.readSensorsButton.FlatAppearance.BorderSize = 0;
         this.readSensorsButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
         this.readSensorsButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
         this.readSensorsButton.ForeColor = System.Drawing.Color.White;
         this.readSensorsButton.Name = "readSensorsButton";
         this.readSensorsButton.UseVisualStyleBackColor = false;
         this.readSensorsButton.Click += new System.EventHandler(this.readSensorsButton_Click);
         // 
         // commissioningButton
         // 
         resources.ApplyResources(this.commissioningButton, "commissioningButton");
         this.commissioningButton.BackColor = System.Drawing.Color.Transparent;
         this.commissioningButton.BackgroundImage = global::TabletApp.Properties.Resources.blue_button_big;
         this.commissioningButton.FlatAppearance.BorderSize = 0;
         this.commissioningButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
         this.commissioningButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
         this.commissioningButton.ForeColor = System.Drawing.Color.White;
         this.commissioningButton.Name = "commissioningButton";
         this.commissioningButton.UseVisualStyleBackColor = false;
         this.commissioningButton.Click += new System.EventHandler(this.commissioningButton_Click);
         // 
         // emailButton
         // 
         resources.ApplyResources(this.emailButton, "emailButton");
         this.emailButton.BackColor = System.Drawing.Color.Transparent;
         this.emailButton.BackgroundImage = global::TabletApp.Properties.Resources.blue_button_big;
         this.emailButton.FlatAppearance.BorderSize = 0;
         this.emailButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
         this.emailButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
         this.emailButton.ForeColor = System.Drawing.Color.White;
         this.emailButton.Name = "emailButton";
         this.emailButton.UseVisualStyleBackColor = false;
         this.emailButton.Click += new System.EventHandler(this.emailButton_Click);
         // 
         // readLoggerButton
         // 
         resources.ApplyResources(this.readLoggerButton, "readLoggerButton");
         this.readLoggerButton.BackColor = System.Drawing.Color.Transparent;
         this.readLoggerButton.BackgroundImage = global::TabletApp.Properties.Resources.blue_button_big;
         this.readLoggerButton.FlatAppearance.BorderSize = 0;
         this.readLoggerButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
         this.readLoggerButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
         this.readLoggerButton.ForeColor = System.Drawing.Color.White;
         this.readLoggerButton.Name = "readLoggerButton";
         this.readLoggerButton.UseVisualStyleBackColor = false;
         this.readLoggerButton.Click += new System.EventHandler(this.readLoggerButton_Click);
         // 
         // Home
         // 
         resources.ApplyResources(this, "$this");
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.BackColor = System.Drawing.Color.Transparent;
         this.Controls.Add(this.readLoggerButton);
         this.Controls.Add(this.emailButton);
         this.Controls.Add(this.commissioningButton);
         this.Controls.Add(this.uploadButton);
         this.Controls.Add(this.reviewButton);
         this.Controls.Add(this.readSensorsButton);
         this.Name = "Home";
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.Button readSensorsButton;
      private System.Windows.Forms.Button reviewButton;
      private System.Windows.Forms.Button uploadButton;
      private System.Windows.Forms.Button commissioningButton;
      private System.Windows.Forms.Button emailButton;
      private System.Windows.Forms.Button readLoggerButton;
   }
}

