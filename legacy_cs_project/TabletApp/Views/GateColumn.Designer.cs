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
   partial class GateColumn
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GateColumn));
         this.start = new TabletApp.Views.FloatTextBox();
         this.width = new TabletApp.Views.FloatTextBox();
         this.threshold = new TabletApp.Views.FloatNegTextBox();
         this.SuspendLayout();
         // 
         // start
         // 
         this.start.FloatValue = 0F;
         resources.ApplyResources(this.start, "start");
         this.start.Increment = 0.1F;
         this.start.IntValue = 0;
         this.start.MaxValue = 80F;
         this.start.MinValue = 0F;
         this.start.Name = "start";
         this.start.NumPlaces = 1;
         this.start.Value = 0F;
         // 
         // width
         // 
         this.width.FloatValue = 0F;
         resources.ApplyResources(this.width, "width");
         this.width.Increment = 0.1F;
         this.width.IntValue = 0;
         this.width.MaxValue = 80F;
         this.width.MinValue = 0F;
         this.width.Name = "width";
         this.width.NumPlaces = 1;
         this.width.Value = 0F;
         // 
         // threshold
         // 
         this.threshold.FloatValue = 0F;
         resources.ApplyResources(this.threshold, "threshold");
         this.threshold.Increment = 1F;
         this.threshold.IntValue = 0;
         this.threshold.MaxValue = 90F;
         this.threshold.MinValue = -90F;
         this.threshold.Name = "threshold";
         this.threshold.NumPlaces = 0;
         this.threshold.Value = 0F;
         // 
         // AGateColumn
         // 
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
         this.BackColor = System.Drawing.Color.Transparent;
         this.Controls.Add(this.threshold);
         this.Controls.Add(this.width);
         this.Controls.Add(this.start);
         this.Name = "AGateColumn";
         resources.ApplyResources(this, "$this");
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private FloatTextBox start;
      private FloatTextBox width;
      private FloatNegTextBox threshold;
   }
}

