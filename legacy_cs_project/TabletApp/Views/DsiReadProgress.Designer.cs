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
   partial class DsiReadProgress
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DsiReadProgress));
         this.probeProgressBar = new System.Windows.Forms.ProgressBar();
         this.dsiProgress = new System.Windows.Forms.Label();
         this.SuspendLayout();
         // 
         // probeProgressBar
         // 
         resources.ApplyResources(this.probeProgressBar, "probeProgressBar");
         this.probeProgressBar.Name = "probeProgressBar";
         this.probeProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
         // 
         // dsiProgress
         // 
         resources.ApplyResources(this.dsiProgress, "dsiProgress");
         this.dsiProgress.Name = "dsiProgress";
         // 
         // ADsiReadProgress
         // 
         resources.ApplyResources(this, "$this");
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.dsiProgress);
         this.Controls.Add(this.probeProgressBar);
         this.Name = "ADsiReadProgress";
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.ProgressBar probeProgressBar;
      private System.Windows.Forms.Label dsiProgress;
   }
}

