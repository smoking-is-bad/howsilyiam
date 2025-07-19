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
   partial class UpDownControl
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpDownControl));
         this.upButton = new System.Windows.Forms.Button();
         this.downButton = new System.Windows.Forms.Button();
         this.numberPad = new TabletApp.Views.NumberPad();
         this.SuspendLayout();
         // 
         // upButton
         // 
         this.upButton.BackColor = System.Drawing.Color.Transparent;
         this.upButton.BackgroundImage = global::TabletApp.Properties.Resources.blue_button_up;
         resources.ApplyResources(this.upButton, "upButton");
         this.upButton.FlatAppearance.BorderSize = 0;
         this.upButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
         this.upButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
         this.upButton.ForeColor = System.Drawing.Color.White;
         this.upButton.Name = "upButton";
         this.upButton.UseVisualStyleBackColor = false;
         // 
         // downButton
         // 
         this.downButton.BackColor = System.Drawing.Color.Transparent;
         this.downButton.BackgroundImage = global::TabletApp.Properties.Resources.blue_button_down;
         resources.ApplyResources(this.downButton, "downButton");
         this.downButton.FlatAppearance.BorderSize = 0;
         this.downButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
         this.downButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
         this.downButton.ForeColor = System.Drawing.Color.White;
         this.downButton.Name = "downButton";
         this.downButton.UseVisualStyleBackColor = false;
         // 
         // numberPad
         // 
         resources.ApplyResources(this.numberPad, "numberPad");
         this.numberPad.Name = "numberPad";
         // 
         // AUpDownControl
         // 
         resources.ApplyResources(this, "$this");
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.Controls.Add(this.numberPad);
         this.Controls.Add(this.downButton);
         this.Controls.Add(this.upButton);
         this.Name = "AUpDownControl";
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.Button upButton;
      private System.Windows.Forms.Button downButton;
      private NumberPad numberPad;
   }
}

