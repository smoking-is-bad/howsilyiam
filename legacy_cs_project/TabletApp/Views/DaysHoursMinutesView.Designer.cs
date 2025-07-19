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
   partial class DaysHoursMinutesView
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DaysHoursMinutesView));
         this.label10 = new System.Windows.Forms.Label();
         this.label14 = new System.Windows.Forms.Label();
         this.label1 = new System.Windows.Forms.Label();
         this.minutes = new TabletApp.Views.NumberTextBox();
         this.hours = new TabletApp.Views.NumberTextBox();
         this.days = new TabletApp.Views.NumberTextBox();
         this.SuspendLayout();
         // 
         // label10
         // 
         resources.ApplyResources(this.label10, "label10");
         this.label10.Name = "label10";
         // 
         // label14
         // 
         resources.ApplyResources(this.label14, "label14");
         this.label14.Name = "label14";
         // 
         // label1
         // 
         resources.ApplyResources(this.label1, "label1");
         this.label1.Name = "label1";
         // 
         // minutes
         // 
         resources.ApplyResources(this.minutes, "minutes");
         this.minutes.Increment = 1F;
         this.minutes.IntValue = 0;
         this.minutes.MaxValue = 59F;
         this.minutes.MinValue = 0F;
         this.minutes.Name = "minutes";
         this.minutes.Value = 0F;
         // 
         // hours
         // 
         resources.ApplyResources(this.hours, "hours");
         this.hours.Increment = 1F;
         this.hours.IntValue = 0;
         this.hours.MaxValue = 23F;
         this.hours.MinValue = 0F;
         this.hours.Name = "hours";
         this.hours.Value = 0F;
         // 
         // days
         // 
         resources.ApplyResources(this.days, "days");
         this.days.Increment = 1F;
         this.days.IntValue = 0;
         this.days.MaxValue = 99F;
         this.days.MinValue = 0F;
         this.days.Name = "days";
         this.days.Value = 0F;
         // 
         // ADaysHoursMinutesView
         // 
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
         this.BackColor = System.Drawing.Color.Transparent;
         this.Controls.Add(this.minutes);
         this.Controls.Add(this.hours);
         this.Controls.Add(this.days);
         this.Controls.Add(this.label10);
         this.Controls.Add(this.label14);
         this.Controls.Add(this.label1);
         this.Name = "ADaysHoursMinutesView";
         resources.ApplyResources(this, "$this");
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Label label10;
      private System.Windows.Forms.Label label14;
      private System.Windows.Forms.Label label1;
      private NumberTextBox days;
      private NumberTextBox hours;
      private NumberTextBox minutes;
   }
}

