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
   partial class Upload
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Upload));
         this.progressBar = new System.Windows.Forms.ProgressBar();
         this.fileProgressText = new System.Windows.Forms.Label();
         this.fileList = new TabletApp.Views.FileList();
         this.SuspendLayout();
         // 
         // progressBar
         // 
         resources.ApplyResources(this.progressBar, "progressBar");
         this.progressBar.Name = "progressBar";
         this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
         // 
         // fileProgressText
         // 
         resources.ApplyResources(this.fileProgressText, "fileProgressText");
         this.fileProgressText.Name = "fileProgressText";
         // 
         // fileList
         // 
         resources.ApplyResources(this.fileList, "fileList");
         this.fileList.BackColor = System.Drawing.Color.Transparent;
         this.fileList.Mapper = null;
         this.fileList.Name = "fileList";
         // 
         // AUpload
         // 
         resources.ApplyResources(this, "$this");
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.fileList);
         this.Controls.Add(this.fileProgressText);
         this.Controls.Add(this.progressBar);
         this.Name = "AUpload";
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.ProgressBar progressBar;
      private System.Windows.Forms.Label fileProgressText;
      private Views.FileList fileList;

   }
}

