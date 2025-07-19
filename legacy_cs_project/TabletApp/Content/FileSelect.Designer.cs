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
namespace TabletApp.Content
{
   partial class FileSelect
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileSelect));
         this.fileList = new TabletApp.Views.FileList();
         this.contentsList = new System.Windows.Forms.ListBox();
         this.addButton = new System.Windows.Forms.Button();
         this.label2 = new System.Windows.Forms.Label();
         this.label1 = new System.Windows.Forms.Label();
         this.addAllButton = new System.Windows.Forms.Button();
         this.treeView = new CodersLab.Windows.Controls.TreeView();
         this.multiSelectLabel = new System.Windows.Forms.Label();
         this.SuspendLayout();
         // 
         // fileList
         // 
         resources.ApplyResources(this.fileList, "fileList");
         this.fileList.BackColor = System.Drawing.Color.Transparent;
         this.fileList.Mapper = null;
         this.fileList.Name = "fileList";
         this.fileList.TabStop = false;
         // 
         // contentsList
         // 
         resources.ApplyResources(this.contentsList, "contentsList");
         this.contentsList.FormattingEnabled = true;
         this.contentsList.Name = "contentsList";
         this.contentsList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
         this.contentsList.SelectedIndexChanged += new System.EventHandler(this.contentsList_SelectedIndexChanged);
         this.contentsList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.contentsList_MouseDoubleClick);
         // 
         // addButton
         // 
         resources.ApplyResources(this.addButton, "addButton");
         this.addButton.BackColor = System.Drawing.Color.Transparent;
         this.addButton.FlatAppearance.BorderSize = 0;
         this.addButton.ForeColor = System.Drawing.Color.White;
         this.addButton.Name = "addButton";
         this.addButton.UseVisualStyleBackColor = false;
         this.addButton.EnabledChanged += new System.EventHandler(this.addButton_EnabledChanged);
         this.addButton.Click += new System.EventHandler(this.addButton_Click);
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
         // addAllButton
         // 
         resources.ApplyResources(this.addAllButton, "addAllButton");
         this.addAllButton.BackColor = System.Drawing.Color.Transparent;
         this.addAllButton.FlatAppearance.BorderSize = 0;
         this.addAllButton.ForeColor = System.Drawing.Color.White;
         this.addAllButton.Name = "addAllButton";
         this.addAllButton.UseVisualStyleBackColor = false;
         this.addAllButton.EnabledChanged += new System.EventHandler(this.addButton_EnabledChanged);
         this.addAllButton.Click += new System.EventHandler(this.addAllButton_Click);
         // 
         // treeView
         // 
         resources.ApplyResources(this.treeView, "treeView");
         this.treeView.Name = "treeView";
         this.treeView.SelectionBackColor = System.Drawing.SystemColors.Highlight;
         this.treeView.SelectionMode = CodersLab.Windows.Controls.TreeViewSelectionMode.MultiSelectSameLevelAndRootBranch;
         this.treeView.SelectionsChanged += new System.EventHandler(this.treeView_SelectionsChanged);
         this.treeView.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView_BeforeSelect);
         // 
         // multiSelectLabel
         // 
         this.multiSelectLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         resources.ApplyResources(this.multiSelectLabel, "multiSelectLabel");
         this.multiSelectLabel.Name = "multiSelectLabel";
         // 
         // FileSelect
         // 
         resources.ApplyResources(this, "$this");
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.BackColor = System.Drawing.Color.Transparent;
         this.Controls.Add(this.multiSelectLabel);
         this.Controls.Add(this.treeView);
         this.Controls.Add(this.addAllButton);
         this.Controls.Add(this.label1);
         this.Controls.Add(this.label2);
         this.Controls.Add(this.addButton);
         this.Controls.Add(this.contentsList);
         this.Controls.Add(this.fileList);
         this.Name = "FileSelect";
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private Views.FileList fileList;
      private System.Windows.Forms.ListBox contentsList;
      private System.Windows.Forms.Button addButton;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Button addAllButton;
      private CodersLab.Windows.Controls.TreeView treeView;
      private System.Windows.Forms.Label multiSelectLabel;
   }
}

