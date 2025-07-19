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
   partial class SpinnerProgress
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpinnerProgress));
         this.spinner = new System.Windows.Forms.PictureBox();
         this.progressText = new System.Windows.Forms.Label();
         ((System.ComponentModel.ISupportInitialize)(this.spinner)).BeginInit();
         this.SuspendLayout();
         // 
         // spinner
         // 
         this.spinner.Image = global::TabletApp.Properties.Resources.spinner;
         resources.ApplyResources(this.spinner, "spinner");
         this.spinner.Name = "spinner";
         this.spinner.TabStop = false;
         // 
         // progressText
         // 
         resources.ApplyResources(this.progressText, "progressText");
         this.progressText.Name = "progressText";
         // 
         // ASpinnerProgress
         // 
         resources.ApplyResources(this, "$this");
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.progressText);
         this.Controls.Add(this.spinner);
         this.Name = "ASpinnerProgress";
         ((System.ComponentModel.ISupportInitialize)(this.spinner)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.PictureBox spinner;
      private System.Windows.Forms.Label progressText;
   }
}

