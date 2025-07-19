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
   partial class Main
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
         this.addEditButton = new System.Windows.Forms.Button();
         this.newButton = new System.Windows.Forms.Button();
         this.resetButton = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // addEditButton
         // 
         resources.ApplyResources(this.addEditButton, "addEditButton");
         this.addEditButton.BackColor = System.Drawing.Color.Transparent;
         this.addEditButton.BackgroundImage = global::TabletApp.Properties.Resources.blue_button_big;
         this.addEditButton.FlatAppearance.BorderSize = 0;
         this.addEditButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
         this.addEditButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
         this.addEditButton.ForeColor = System.Drawing.Color.White;
         this.addEditButton.Name = "addEditButton";
         this.addEditButton.UseVisualStyleBackColor = false;
         this.addEditButton.Click += new System.EventHandler(this.addEditButton_Click);
         // 
         // newButton
         // 
         resources.ApplyResources(this.newButton, "newButton");
         this.newButton.BackColor = System.Drawing.Color.Transparent;
         this.newButton.BackgroundImage = global::TabletApp.Properties.Resources.blue_button_big;
         this.newButton.FlatAppearance.BorderSize = 0;
         this.newButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
         this.newButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
         this.newButton.ForeColor = System.Drawing.Color.White;
         this.newButton.Name = "newButton";
         this.newButton.UseVisualStyleBackColor = false;
         this.newButton.Click += new System.EventHandler(this.newButton_Click);
         // 
         // resetButton
         // 
         resources.ApplyResources(this.resetButton, "resetButton");
         this.resetButton.BackColor = System.Drawing.Color.Transparent;
         this.resetButton.BackgroundImage = global::TabletApp.Properties.Resources.blue_button_big;
         this.resetButton.FlatAppearance.BorderSize = 0;
         this.resetButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
         this.resetButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
         this.resetButton.ForeColor = System.Drawing.Color.White;
         this.resetButton.Name = "resetButton";
         this.resetButton.UseVisualStyleBackColor = false;
         this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
         // 
         // AMain
         // 
         resources.ApplyResources(this, "$this");
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.BackColor = System.Drawing.Color.Transparent;
         this.Controls.Add(this.resetButton);
         this.Controls.Add(this.addEditButton);
         this.Controls.Add(this.newButton);
         this.Name = "AMain";
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.Button newButton;
      private System.Windows.Forms.Button addEditButton;
      private System.Windows.Forms.Button resetButton;
   }
}

