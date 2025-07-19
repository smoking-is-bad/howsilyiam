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
   partial class AscanWaveform
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AscanWaveform));
         this.SuspendLayout();
         // 
         // AAscanWaveform
         // 
         resources.ApplyResources(this, "$this");
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.BackColor = System.Drawing.Color.Transparent;
         this.DoubleBuffered = true;
         this.Name = "AAscanWaveform";
         this.Paint += new System.Windows.Forms.PaintEventHandler(this.AAscanWaveform_Paint);
         this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.AAscanWaveform_MouseDown);
         this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.AAscanWaveform_MouseMoveZoom);
         this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.AAscanWaveform_MouseUp);
         this.ResumeLayout(false);

      }

      #endregion


   }
}

