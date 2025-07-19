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
   partial class SummaryGrid
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SummaryGrid));
         this.gridPanel = new System.Windows.Forms.TableLayoutPanel();
         this.thickness = new System.Windows.Forms.Label();
         this.dsiLabel = new System.Windows.Forms.Panel();
         this.label3 = new System.Windows.Forms.Label();
         this.label2 = new System.Windows.Forms.Label();
         this.label1 = new System.Windows.Forms.Label();
         this.loggerLabel = new System.Windows.Forms.Panel();
         this.label4 = new System.Windows.Forms.Label();
         this.label5 = new System.Windows.Forms.Label();
         this.label6 = new System.Windows.Forms.Label();
         this.label7 = new System.Windows.Forms.Label();
         this.label8 = new System.Windows.Forms.Label();
         this.label9 = new System.Windows.Forms.Label();
         this.label10 = new System.Windows.Forms.Label();
         this.dsiLabel.SuspendLayout();
         this.loggerLabel.SuspendLayout();
         this.SuspendLayout();
         // 
         // gridPanel
         // 
         resources.ApplyResources(this.gridPanel, "gridPanel");
         this.gridPanel.Name = "gridPanel";
         this.gridPanel.CellPaint += new System.Windows.Forms.TableLayoutCellPaintEventHandler(this.gridPanel_CellPaint);
         // 
         // thickness
         // 
         resources.ApplyResources(this.thickness, "thickness");
         this.thickness.Name = "thickness";
         // 
         // dsiLabel
         // 
         this.dsiLabel.Controls.Add(this.label3);
         this.dsiLabel.Controls.Add(this.label2);
         this.dsiLabel.Controls.Add(this.label1);
         resources.ApplyResources(this.dsiLabel, "dsiLabel");
         this.dsiLabel.Name = "dsiLabel";
         // 
         // label3
         // 
         resources.ApplyResources(this.label3, "label3");
         this.label3.Name = "label3";
         // 
         // label2
         // 
         resources.ApplyResources(this.label2, "label2");
         this.label2.Name = "label2";
         // 
         // label1
         // 
         resources.ApplyResources(this.label1, "label1");
         this.label1.Name = "label1";
         // 
         // loggerLabel
         // 
         this.loggerLabel.Controls.Add(this.label10);
         this.loggerLabel.Controls.Add(this.label9);
         this.loggerLabel.Controls.Add(this.label8);
         this.loggerLabel.Controls.Add(this.label7);
         this.loggerLabel.Controls.Add(this.label4);
         this.loggerLabel.Controls.Add(this.label5);
         this.loggerLabel.Controls.Add(this.label6);
         resources.ApplyResources(this.loggerLabel, "loggerLabel");
         this.loggerLabel.Name = "loggerLabel";
         // 
         // label4
         // 
         resources.ApplyResources(this.label4, "label4");
         this.label4.Name = "label4";
         // 
         // label5
         // 
         resources.ApplyResources(this.label5, "label5");
         this.label5.Name = "label5";
         // 
         // label6
         // 
         resources.ApplyResources(this.label6, "label6");
         this.label6.Name = "label6";
         // 
         // label7
         // 
         resources.ApplyResources(this.label7, "label7");
         this.label7.Name = "label7";
         // 
         // label8
         // 
         resources.ApplyResources(this.label8, "label8");
         this.label8.Name = "label8";
         // 
         // label9
         // 
         resources.ApplyResources(this.label9, "label9");
         this.label9.Name = "label9";
         // 
         // label10
         // 
         resources.ApplyResources(this.label10, "label10");
         this.label10.Name = "label10";
         // 
         // SummaryGrid
         // 
         resources.ApplyResources(this, "$this");
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.loggerLabel);
         this.Controls.Add(this.dsiLabel);
         this.Controls.Add(this.thickness);
         this.Controls.Add(this.gridPanel);
         this.Name = "SummaryGrid";
         this.dsiLabel.ResumeLayout(false);
         this.loggerLabel.ResumeLayout(false);
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.TableLayoutPanel gridPanel;
      private System.Windows.Forms.Label thickness;
      private System.Windows.Forms.Panel dsiLabel;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Panel loggerLabel;
      private System.Windows.Forms.Label label10;
      private System.Windows.Forms.Label label9;
      private System.Windows.Forms.Label label8;
      private System.Windows.Forms.Label label7;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.Label label5;
      private System.Windows.Forms.Label label6;
   }
}

