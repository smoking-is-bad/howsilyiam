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
   partial class AutoReadView
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AutoReadView));
         this.autoReadTimerText = new System.Windows.Forms.Label();
         this.autoReadLabel = new System.Windows.Forms.Label();
         this.stopButton = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // autoReadTimerText
         // 
         resources.ApplyResources(this.autoReadTimerText, "autoReadTimerText");
         this.autoReadTimerText.Name = "autoReadTimerText";
         // 
         // autoReadLabel
         // 
         resources.ApplyResources(this.autoReadLabel, "autoReadLabel");
         this.autoReadLabel.Name = "autoReadLabel";
         // 
         // stopButton
         // 
         this.stopButton.BackColor = System.Drawing.Color.Transparent;
         resources.ApplyResources(this.stopButton, "stopButton");
         this.stopButton.FlatAppearance.BorderSize = 0;
         this.stopButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
         this.stopButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
         this.stopButton.ForeColor = System.Drawing.Color.White;
         this.stopButton.Name = "stopButton";
         this.stopButton.UseVisualStyleBackColor = false;
         this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
         // 
         // AAutoReadView
         // 
         resources.ApplyResources(this, "$this");
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.stopButton);
         this.Controls.Add(this.autoReadTimerText);
         this.Controls.Add(this.autoReadLabel);
         this.Name = "AAutoReadView";
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Label autoReadTimerText;
      private System.Windows.Forms.Label autoReadLabel;
      private System.Windows.Forms.Button stopButton;
   }
}

