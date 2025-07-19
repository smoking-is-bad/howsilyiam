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
   partial class ProbeList
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProbeList));
         this.probePanel = new System.Windows.Forms.TableLayoutPanel();
         this.vertSeparator = new System.Windows.Forms.Panel();
         this.SuspendLayout();
         // 
         // probePanel
         // 
         resources.ApplyResources(this.probePanel, "probePanel");
         this.probePanel.Name = "probePanel";
         // 
         // vertSeparator
         // 
         this.vertSeparator.BackColor = System.Drawing.Color.DarkGray;
         resources.ApplyResources(this.vertSeparator, "vertSeparator");
         this.vertSeparator.Name = "vertSeparator";
         // 
         // AProbeList
         // 
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
         resources.ApplyResources(this, "$this");
         this.BackColor = System.Drawing.Color.Transparent;
         this.Controls.Add(this.vertSeparator);
         this.Controls.Add(this.probePanel);
         this.Name = "AProbeList";
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.TableLayoutPanel probePanel;
      private System.Windows.Forms.Panel vertSeparator;
   }
}

