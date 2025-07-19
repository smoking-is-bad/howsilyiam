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
   partial class Settings
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings));
         this.groupBox6 = new System.Windows.Forms.GroupBox();
         this.csvSaveCheckbox = new System.Windows.Forms.CheckBox();
         this.groupBox5 = new System.Windows.Forms.GroupBox();
         this.autoReadMinsTextBox = new TabletApp.Views.NumberTextBox();
         this.autoReadHoursTextBox = new TabletApp.Views.NumberTextBox();
         this.label11 = new System.Windows.Forms.Label();
         this.panel3 = new System.Windows.Forms.Panel();
         this.autoUploadOffRadioButton = new System.Windows.Forms.RadioButton();
         this.autoUploadOnRadioButton = new System.Windows.Forms.RadioButton();
         this.label10 = new System.Windows.Forms.Label();
         this.label9 = new System.Windows.Forms.Label();
         this.label8 = new System.Windows.Forms.Label();
         this.panel2 = new System.Windows.Forms.Panel();
         this.autoReadOffRadioButton = new System.Windows.Forms.RadioButton();
         this.autoReadOnRadioButton = new System.Windows.Forms.RadioButton();
         this.label14 = new System.Windows.Forms.Label();
         this.groupBox4 = new System.Windows.Forms.GroupBox();
         this.label13 = new System.Windows.Forms.Label();
         this.inchesPrecisionCombo = new System.Windows.Forms.ComboBox();
         this.label12 = new System.Windows.Forms.Label();
         this.mmPrecisionCombo = new System.Windows.Forms.ComboBox();
         this.label7 = new System.Windows.Forms.Label();
         this.panel1 = new System.Windows.Forms.Panel();
         this.inchesRadioButton = new System.Windows.Forms.RadioButton();
         this.mmRadioButton = new System.Windows.Forms.RadioButton();
         this.groupBox3 = new System.Windows.Forms.GroupBox();
         this.label19 = new System.Windows.Forms.Label();
         this.smtpServerPort = new TabletApp.Views.NumberTextBox();
         this.label6 = new System.Windows.Forms.Label();
         this.password = new System.Windows.Forms.TextBox();
         this.label5 = new System.Windows.Forms.Label();
         this.username = new System.Windows.Forms.TextBox();
         this.label4 = new System.Windows.Forms.Label();
         this.fromAddress = new System.Windows.Forms.TextBox();
         this.label3 = new System.Windows.Forms.Label();
         this.smtpServer = new System.Windows.Forms.TextBox();
         this.groupBox1 = new System.Windows.Forms.GroupBox();
         this.baudRateCombo = new System.Windows.Forms.ComboBox();
         this.label15 = new System.Windows.Forms.Label();
         this.comPortCombo = new System.Windows.Forms.ComboBox();
         this.label2 = new System.Windows.Forms.Label();
         this.groupBox2 = new System.Windows.Forms.GroupBox();
         this.label18 = new System.Windows.Forms.Label();
         this.label16 = new System.Windows.Forms.Label();
         this.ascanScroll = new TabletApp.Views.NumberTextBox();
         this.label1 = new System.Windows.Forms.Label();
         this.ascanZoom = new TabletApp.Views.NumberTextBox();
         this.lockAscans = new System.Windows.Forms.CheckBox();
         this.label17 = new System.Windows.Forms.Label();
         this.groupBox7 = new System.Windows.Forms.GroupBox();
         this.languageCombo = new System.Windows.Forms.ComboBox();
         this.label20 = new System.Windows.Forms.Label();
         this.groupBox6.SuspendLayout();
         this.groupBox5.SuspendLayout();
         this.panel3.SuspendLayout();
         this.panel2.SuspendLayout();
         this.groupBox4.SuspendLayout();
         this.panel1.SuspendLayout();
         this.groupBox3.SuspendLayout();
         this.groupBox1.SuspendLayout();
         this.groupBox2.SuspendLayout();
         this.groupBox7.SuspendLayout();
         this.SuspendLayout();
         // 
         // groupBox6
         // 
         resources.ApplyResources(this.groupBox6, "groupBox6");
         this.groupBox6.Controls.Add(this.csvSaveCheckbox);
         this.groupBox6.Name = "groupBox6";
         this.groupBox6.TabStop = false;
         // 
         // csvSaveCheckbox
         // 
         resources.ApplyResources(this.csvSaveCheckbox, "csvSaveCheckbox");
         this.csvSaveCheckbox.Name = "csvSaveCheckbox";
         this.csvSaveCheckbox.UseVisualStyleBackColor = true;
         // 
         // groupBox5
         // 
         resources.ApplyResources(this.groupBox5, "groupBox5");
         this.groupBox5.Controls.Add(this.autoReadMinsTextBox);
         this.groupBox5.Controls.Add(this.autoReadHoursTextBox);
         this.groupBox5.Controls.Add(this.label11);
         this.groupBox5.Controls.Add(this.panel3);
         this.groupBox5.Controls.Add(this.label10);
         this.groupBox5.Controls.Add(this.label9);
         this.groupBox5.Controls.Add(this.label8);
         this.groupBox5.Controls.Add(this.panel2);
         this.groupBox5.Controls.Add(this.label14);
         this.groupBox5.Name = "groupBox5";
         this.groupBox5.TabStop = false;
         // 
         // autoReadMinsTextBox
         // 
         this.autoReadMinsTextBox.Increment = 1F;
         this.autoReadMinsTextBox.IntValue = 1;
         resources.ApplyResources(this.autoReadMinsTextBox, "autoReadMinsTextBox");
         this.autoReadMinsTextBox.MaxValue = 59F;
         this.autoReadMinsTextBox.MinValue = 0F;
         this.autoReadMinsTextBox.Name = "autoReadMinsTextBox";
         this.autoReadMinsTextBox.Value = 1F;
         // 
         // autoReadHoursTextBox
         // 
         this.autoReadHoursTextBox.Increment = 1F;
         this.autoReadHoursTextBox.IntValue = 0;
         resources.ApplyResources(this.autoReadHoursTextBox, "autoReadHoursTextBox");
         this.autoReadHoursTextBox.MaxValue = 99F;
         this.autoReadHoursTextBox.MinValue = 0F;
         this.autoReadHoursTextBox.Name = "autoReadHoursTextBox";
         this.autoReadHoursTextBox.Value = 0F;
         // 
         // label11
         // 
         resources.ApplyResources(this.label11, "label11");
         this.label11.Name = "label11";
         // 
         // panel3
         // 
         this.panel3.Controls.Add(this.autoUploadOffRadioButton);
         this.panel3.Controls.Add(this.autoUploadOnRadioButton);
         resources.ApplyResources(this.panel3, "panel3");
         this.panel3.Name = "panel3";
         // 
         // autoUploadOffRadioButton
         // 
         resources.ApplyResources(this.autoUploadOffRadioButton, "autoUploadOffRadioButton");
         this.autoUploadOffRadioButton.Name = "autoUploadOffRadioButton";
         this.autoUploadOffRadioButton.TabStop = true;
         this.autoUploadOffRadioButton.UseVisualStyleBackColor = true;
         // 
         // autoUploadOnRadioButton
         // 
         resources.ApplyResources(this.autoUploadOnRadioButton, "autoUploadOnRadioButton");
         this.autoUploadOnRadioButton.Name = "autoUploadOnRadioButton";
         this.autoUploadOnRadioButton.TabStop = true;
         this.autoUploadOnRadioButton.UseVisualStyleBackColor = true;
         // 
         // label10
         // 
         resources.ApplyResources(this.label10, "label10");
         this.label10.Name = "label10";
         // 
         // label9
         // 
         resources.ApplyResources(this.label9, "label9");
         this.label9.Name = "label9";
         // 
         // label8
         // 
         resources.ApplyResources(this.label8, "label8");
         this.label8.Name = "label8";
         // 
         // panel2
         // 
         this.panel2.Controls.Add(this.autoReadOffRadioButton);
         this.panel2.Controls.Add(this.autoReadOnRadioButton);
         resources.ApplyResources(this.panel2, "panel2");
         this.panel2.Name = "panel2";
         // 
         // autoReadOffRadioButton
         // 
         resources.ApplyResources(this.autoReadOffRadioButton, "autoReadOffRadioButton");
         this.autoReadOffRadioButton.Name = "autoReadOffRadioButton";
         this.autoReadOffRadioButton.TabStop = true;
         this.autoReadOffRadioButton.UseVisualStyleBackColor = true;
         // 
         // autoReadOnRadioButton
         // 
         resources.ApplyResources(this.autoReadOnRadioButton, "autoReadOnRadioButton");
         this.autoReadOnRadioButton.Name = "autoReadOnRadioButton";
         this.autoReadOnRadioButton.TabStop = true;
         this.autoReadOnRadioButton.UseVisualStyleBackColor = true;
         // 
         // label14
         // 
         resources.ApplyResources(this.label14, "label14");
         this.label14.Name = "label14";
         // 
         // groupBox4
         // 
         resources.ApplyResources(this.groupBox4, "groupBox4");
         this.groupBox4.Controls.Add(this.label13);
         this.groupBox4.Controls.Add(this.inchesPrecisionCombo);
         this.groupBox4.Controls.Add(this.label12);
         this.groupBox4.Controls.Add(this.mmPrecisionCombo);
         this.groupBox4.Controls.Add(this.label7);
         this.groupBox4.Controls.Add(this.panel1);
         this.groupBox4.Name = "groupBox4";
         this.groupBox4.TabStop = false;
         // 
         // label13
         // 
         resources.ApplyResources(this.label13, "label13");
         this.label13.Name = "label13";
         // 
         // inchesPrecisionCombo
         // 
         this.inchesPrecisionCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.inchesPrecisionCombo.FormattingEnabled = true;
         this.inchesPrecisionCombo.Items.AddRange(new object[] {
            resources.GetString("inchesPrecisionCombo.Items"),
            resources.GetString("inchesPrecisionCombo.Items1"),
            resources.GetString("inchesPrecisionCombo.Items2"),
            resources.GetString("inchesPrecisionCombo.Items3")});
         resources.ApplyResources(this.inchesPrecisionCombo, "inchesPrecisionCombo");
         this.inchesPrecisionCombo.Name = "inchesPrecisionCombo";
         // 
         // label12
         // 
         resources.ApplyResources(this.label12, "label12");
         this.label12.Name = "label12";
         // 
         // mmPrecisionCombo
         // 
         this.mmPrecisionCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         resources.ApplyResources(this.mmPrecisionCombo, "mmPrecisionCombo");
         this.mmPrecisionCombo.FormattingEnabled = true;
         this.mmPrecisionCombo.Items.AddRange(new object[] {
            resources.GetString("mmPrecisionCombo.Items"),
            resources.GetString("mmPrecisionCombo.Items1"),
            resources.GetString("mmPrecisionCombo.Items2")});
         this.mmPrecisionCombo.Name = "mmPrecisionCombo";
         // 
         // label7
         // 
         resources.ApplyResources(this.label7, "label7");
         this.label7.Name = "label7";
         // 
         // panel1
         // 
         this.panel1.Controls.Add(this.inchesRadioButton);
         this.panel1.Controls.Add(this.mmRadioButton);
         resources.ApplyResources(this.panel1, "panel1");
         this.panel1.Name = "panel1";
         // 
         // inchesRadioButton
         // 
         resources.ApplyResources(this.inchesRadioButton, "inchesRadioButton");
         this.inchesRadioButton.Name = "inchesRadioButton";
         this.inchesRadioButton.TabStop = true;
         this.inchesRadioButton.UseVisualStyleBackColor = true;
         // 
         // mmRadioButton
         // 
         resources.ApplyResources(this.mmRadioButton, "mmRadioButton");
         this.mmRadioButton.Name = "mmRadioButton";
         this.mmRadioButton.TabStop = true;
         this.mmRadioButton.UseVisualStyleBackColor = true;
         // 
         // groupBox3
         // 
         resources.ApplyResources(this.groupBox3, "groupBox3");
         this.groupBox3.Controls.Add(this.label19);
         this.groupBox3.Controls.Add(this.smtpServerPort);
         this.groupBox3.Controls.Add(this.label6);
         this.groupBox3.Controls.Add(this.password);
         this.groupBox3.Controls.Add(this.label5);
         this.groupBox3.Controls.Add(this.username);
         this.groupBox3.Controls.Add(this.label4);
         this.groupBox3.Controls.Add(this.fromAddress);
         this.groupBox3.Controls.Add(this.label3);
         this.groupBox3.Controls.Add(this.smtpServer);
         this.groupBox3.Name = "groupBox3";
         this.groupBox3.TabStop = false;
         // 
         // label19
         // 
         resources.ApplyResources(this.label19, "label19");
         this.label19.Name = "label19";
         // 
         // smtpServerPort
         // 
         this.smtpServerPort.Increment = 1F;
         this.smtpServerPort.IntValue = 25;
         resources.ApplyResources(this.smtpServerPort, "smtpServerPort");
         this.smtpServerPort.MaxValue = 999F;
         this.smtpServerPort.MinValue = 1F;
         this.smtpServerPort.Name = "smtpServerPort";
         this.smtpServerPort.Value = 25F;
         // 
         // label6
         // 
         resources.ApplyResources(this.label6, "label6");
         this.label6.Name = "label6";
         // 
         // password
         // 
         resources.ApplyResources(this.password, "password");
         this.password.Name = "password";
         this.password.UseSystemPasswordChar = true;
         // 
         // label5
         // 
         resources.ApplyResources(this.label5, "label5");
         this.label5.Name = "label5";
         // 
         // username
         // 
         resources.ApplyResources(this.username, "username");
         this.username.Name = "username";
         // 
         // label4
         // 
         resources.ApplyResources(this.label4, "label4");
         this.label4.Name = "label4";
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
         // smtpServer
         // 
         resources.ApplyResources(this.smtpServer, "smtpServer");
         this.smtpServer.Name = "smtpServer";
         // 
         // groupBox1
         // 
         resources.ApplyResources(this.groupBox1, "groupBox1");
         this.groupBox1.Controls.Add(this.baudRateCombo);
         this.groupBox1.Controls.Add(this.label15);
         this.groupBox1.Controls.Add(this.comPortCombo);
         this.groupBox1.Controls.Add(this.label2);
         this.groupBox1.Name = "groupBox1";
         this.groupBox1.TabStop = false;
         // 
         // baudRateCombo
         // 
         this.baudRateCombo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
         this.baudRateCombo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
         this.baudRateCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         resources.ApplyResources(this.baudRateCombo, "baudRateCombo");
         this.baudRateCombo.FormattingEnabled = true;
         this.baudRateCombo.Name = "baudRateCombo";
         // 
         // label15
         // 
         resources.ApplyResources(this.label15, "label15");
         this.label15.Name = "label15";
         // 
         // comPortCombo
         // 
         this.comPortCombo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
         this.comPortCombo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
         this.comPortCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         resources.ApplyResources(this.comPortCombo, "comPortCombo");
         this.comPortCombo.FormattingEnabled = true;
         this.comPortCombo.Name = "comPortCombo";
         // 
         // label2
         // 
         resources.ApplyResources(this.label2, "label2");
         this.label2.Name = "label2";
         // 
         // groupBox2
         // 
         resources.ApplyResources(this.groupBox2, "groupBox2");
         this.groupBox2.Controls.Add(this.label18);
         this.groupBox2.Controls.Add(this.label16);
         this.groupBox2.Controls.Add(this.ascanScroll);
         this.groupBox2.Controls.Add(this.label1);
         this.groupBox2.Controls.Add(this.ascanZoom);
         this.groupBox2.Controls.Add(this.lockAscans);
         this.groupBox2.Controls.Add(this.label17);
         this.groupBox2.Name = "groupBox2";
         this.groupBox2.TabStop = false;
         // 
         // label18
         // 
         resources.ApplyResources(this.label18, "label18");
         this.label18.Name = "label18";
         // 
         // label16
         // 
         resources.ApplyResources(this.label16, "label16");
         this.label16.Name = "label16";
         // 
         // ascanScroll
         // 
         this.ascanScroll.Increment = 1F;
         this.ascanScroll.IntValue = 1;
         resources.ApplyResources(this.ascanScroll, "ascanScroll");
         this.ascanScroll.MaxValue = 100F;
         this.ascanScroll.MinValue = 0F;
         this.ascanScroll.Name = "ascanScroll";
         this.ascanScroll.Value = 1F;
         // 
         // label1
         // 
         resources.ApplyResources(this.label1, "label1");
         this.label1.Name = "label1";
         // 
         // ascanZoom
         // 
         this.ascanZoom.Increment = 1F;
         this.ascanZoom.IntValue = 1;
         resources.ApplyResources(this.ascanZoom, "ascanZoom");
         this.ascanZoom.MaxValue = 100F;
         this.ascanZoom.MinValue = 0F;
         this.ascanZoom.Name = "ascanZoom";
         this.ascanZoom.Value = 1F;
         // 
         // lockAscans
         // 
         resources.ApplyResources(this.lockAscans, "lockAscans");
         this.lockAscans.Name = "lockAscans";
         this.lockAscans.UseVisualStyleBackColor = true;
         // 
         // label17
         // 
         resources.ApplyResources(this.label17, "label17");
         this.label17.Name = "label17";
         // 
         // groupBox7
         // 
         resources.ApplyResources(this.groupBox7, "groupBox7");
         this.groupBox7.Controls.Add(this.languageCombo);
         this.groupBox7.Controls.Add(this.label20);
         this.groupBox7.Name = "groupBox7";
         this.groupBox7.TabStop = false;
         // 
         // languageCombo
         // 
         this.languageCombo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
         this.languageCombo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
         this.languageCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         resources.ApplyResources(this.languageCombo, "languageCombo");
         this.languageCombo.FormattingEnabled = true;
         this.languageCombo.Name = "languageCombo";
         // 
         // label20
         // 
         resources.ApplyResources(this.label20, "label20");
         this.label20.Name = "label20";
         // 
         // Settings
         // 
         resources.ApplyResources(this, "$this");
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.BackColor = System.Drawing.Color.Transparent;
         this.Controls.Add(this.groupBox7);
         this.Controls.Add(this.groupBox2);
         this.Controls.Add(this.groupBox6);
         this.Controls.Add(this.groupBox5);
         this.Controls.Add(this.groupBox4);
         this.Controls.Add(this.groupBox3);
         this.Controls.Add(this.groupBox1);
         this.Name = "Settings";
         this.groupBox6.ResumeLayout(false);
         this.groupBox6.PerformLayout();
         this.groupBox5.ResumeLayout(false);
         this.groupBox5.PerformLayout();
         this.panel3.ResumeLayout(false);
         this.panel3.PerformLayout();
         this.panel2.ResumeLayout(false);
         this.panel2.PerformLayout();
         this.groupBox4.ResumeLayout(false);
         this.groupBox4.PerformLayout();
         this.panel1.ResumeLayout(false);
         this.panel1.PerformLayout();
         this.groupBox3.ResumeLayout(false);
         this.groupBox3.PerformLayout();
         this.groupBox1.ResumeLayout(false);
         this.groupBox2.ResumeLayout(false);
         this.groupBox2.PerformLayout();
         this.groupBox7.ResumeLayout(false);
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.ComboBox comPortCombo;
      private System.Windows.Forms.GroupBox groupBox1;
      private System.Windows.Forms.GroupBox groupBox3;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.TextBox fromAddress;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.TextBox smtpServer;
      private System.Windows.Forms.Label label6;
      private System.Windows.Forms.TextBox password;
      private System.Windows.Forms.Label label5;
      private System.Windows.Forms.TextBox username;
      private System.Windows.Forms.GroupBox groupBox4;
      private System.Windows.Forms.Label label7;
      private System.Windows.Forms.Panel panel1;
      private System.Windows.Forms.RadioButton inchesRadioButton;
      private System.Windows.Forms.RadioButton mmRadioButton;
      private System.Windows.Forms.GroupBox groupBox5;
      private System.Windows.Forms.Label label11;
      private System.Windows.Forms.Panel panel3;
      private System.Windows.Forms.RadioButton autoUploadOffRadioButton;
      private System.Windows.Forms.RadioButton autoUploadOnRadioButton;
      private System.Windows.Forms.Label label10;
      private System.Windows.Forms.Label label9;
      private System.Windows.Forms.Label label8;
      private System.Windows.Forms.Panel panel2;
      private System.Windows.Forms.RadioButton autoReadOffRadioButton;
      private System.Windows.Forms.RadioButton autoReadOnRadioButton;
      private System.Windows.Forms.Label label14;
      private System.Windows.Forms.Label label13;
      private System.Windows.Forms.ComboBox inchesPrecisionCombo;
      private System.Windows.Forms.Label label12;
      private System.Windows.Forms.ComboBox mmPrecisionCombo;
      private System.Windows.Forms.GroupBox groupBox6;
      private System.Windows.Forms.GroupBox groupBox2;
      private System.Windows.Forms.Label label17;
      private System.Windows.Forms.CheckBox lockAscans;
      private Views.NumberTextBox ascanZoom;
      private System.Windows.Forms.Label label18;
      private System.Windows.Forms.Label label16;
      private Views.NumberTextBox ascanScroll;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Label label19;
      private Views.NumberTextBox smtpServerPort;
      private Views.NumberTextBox autoReadMinsTextBox;
      private Views.NumberTextBox autoReadHoursTextBox;
      private System.Windows.Forms.CheckBox csvSaveCheckbox;
      private System.Windows.Forms.GroupBox groupBox7;
      private System.Windows.Forms.ComboBox languageCombo;
      private System.Windows.Forms.Label label20;
      private System.Windows.Forms.ComboBox baudRateCombo;
      private System.Windows.Forms.Label label15;
   }
}

