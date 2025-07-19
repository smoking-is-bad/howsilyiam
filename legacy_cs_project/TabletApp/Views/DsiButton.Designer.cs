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
   partial class DsiButton
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DsiButton));
         this.button = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // button
         // 
         this.button.BackgroundImage = global::TabletApp.Properties.Resources.dsi_active;
         resources.ApplyResources(this.button, "button");
         this.button.FlatAppearance.BorderSize = 0;
         this.button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
         this.button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
         this.button.Name = "button";
         this.button.TabStop = false;
         this.button.UseVisualStyleBackColor = true;
         this.button.Click += new System.EventHandler(this.button_Click);
         // 
         // ADsiButton
         // 
         resources.ApplyResources(this, "$this");
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.button);
         this.Name = "ADsiButton";
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.Button button;
   }
}

