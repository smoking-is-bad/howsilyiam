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
   partial class Email
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Email));
         this.label5 = new System.Windows.Forms.Label();
         this.message = new System.Windows.Forms.TextBox();
         this.label4 = new System.Windows.Forms.Label();
         this.label2 = new System.Windows.Forms.Label();
         this.subject = new System.Windows.Forms.TextBox();
         this.label1 = new System.Windows.Forms.Label();
         this.fromAddress = new System.Windows.Forms.TextBox();
         this.label3 = new System.Windows.Forms.Label();
         this.toAddress = new System.Windows.Forms.TextBox();
         this.fileList = new TabletApp.Views.FileList();
         this.SuspendLayout();
         // 
         // label5
         // 
         resources.ApplyResources(this.label5, "label5");
         this.label5.Name = "label5";
         // 
         // message
         // 
         resources.ApplyResources(this.message, "message");
         this.message.Name = "message";
         // 
         // label4
         // 
         resources.ApplyResources(this.label4, "label4");
         this.label4.Name = "label4";
         // 
         // label2
         // 
         resources.ApplyResources(this.label2, "label2");
         this.label2.Name = "label2";
         // 
         // subject
         // 
         resources.ApplyResources(this.subject, "subject");
         this.subject.Name = "subject";
         // 
         // label1
         // 
         resources.ApplyResources(this.label1, "label1");
         this.label1.Name = "label1";
         // 
         // fromAddress
         // 
         resources.ApplyResources(this.fromAddress, "fromAddress");
         this.fromAddress.Name = "fromAddress";
         // 
         // label3
         // 
         resources.ApplyResources(this.label3, "label3");
         this.label3.Name = "label3";
         // 
         // toAddress
         // 
         resources.ApplyResources(this.toAddress, "toAddress");
         this.toAddress.Name = "toAddress";
         // 
         // fileList
         // 
         resources.ApplyResources(this.fileList, "fileList");
         this.fileList.BackColor = System.Drawing.Color.Transparent;
         this.fileList.Mapper = null;
         this.fileList.Name = "fileList";
         this.fileList.TabStop = false;
         // 
         // AEmail
         // 
         resources.ApplyResources(this, "$this");
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.fileList);
         this.Controls.Add(this.label5);
         this.Controls.Add(this.message);
         this.Controls.Add(this.label4);
         this.Controls.Add(this.label2);
         this.Controls.Add(this.subject);
         this.Controls.Add(this.label1);
         this.Controls.Add(this.fromAddress);
         this.Controls.Add(this.label3);
         this.Controls.Add(this.toAddress);
         this.Name = "AEmail";
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.TextBox toAddress;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.TextBox fromAddress;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.TextBox subject;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.TextBox message;
      private System.Windows.Forms.Label label5;
      private Views.FileList fileList;
   }
}

