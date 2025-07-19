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
namespace TabletApp
{
   partial class WizardForm
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

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizardForm));
         this.headerPanel = new System.Windows.Forms.Panel();
         this.infoLabel = new System.Windows.Forms.Label();
         this.location = new System.Windows.Forms.Label();
         this.viewTitle = new System.Windows.Forms.Label();
         this.logoImage = new System.Windows.Forms.PictureBox();
         this.navButtonPanel = new System.Windows.Forms.TableLayoutPanel();
         this.button5 = new System.Windows.Forms.Button();
         this.button4 = new System.Windows.Forms.Button();
         this.button3 = new System.Windows.Forms.Button();
         this.button2 = new System.Windows.Forms.Button();
         this.button1 = new System.Windows.Forms.Button();
         this.contentContainer = new System.Windows.Forms.Panel();
         this.sidebarPanel = new System.Windows.Forms.Panel();
         this.errorPictureBox = new System.Windows.Forms.PictureBox();
         this.quitButton = new System.Windows.Forms.Button();
         this.settingsButton = new System.Windows.Forms.Button();
         this.helpButton = new System.Windows.Forms.Button();
         this.aboutButton = new System.Windows.Forms.Button();
         this.homeButton = new System.Windows.Forms.Button();
         this.aAutoReadView1 = new TabletApp.Views.AutoReadView();
         this.spinnerProgress = new TabletApp.Views.SpinnerProgress();
         this.headerPanel.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.logoImage)).BeginInit();
         this.navButtonPanel.SuspendLayout();
         this.sidebarPanel.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.errorPictureBox)).BeginInit();
         this.SuspendLayout();
         // 
         // headerPanel
         // 
         resources.ApplyResources(this.headerPanel, "headerPanel");
         this.headerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
         this.headerPanel.BackgroundImage = global::TabletApp.Properties.Resources.header_bg;
         this.headerPanel.Controls.Add(this.infoLabel);
         this.headerPanel.Controls.Add(this.aAutoReadView1);
         this.headerPanel.Controls.Add(this.spinnerProgress);
         this.headerPanel.Controls.Add(this.location);
         this.headerPanel.Controls.Add(this.viewTitle);
         this.headerPanel.Controls.Add(this.logoImage);
         this.headerPanel.Name = "headerPanel";
         // 
         // infoLabel
         // 
         this.infoLabel.BackColor = System.Drawing.Color.Transparent;
         resources.ApplyResources(this.infoLabel, "infoLabel");
         this.infoLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
         this.infoLabel.Name = "infoLabel";
         // 
         // location
         // 
         resources.ApplyResources(this.location, "location");
         this.location.BackColor = System.Drawing.Color.Transparent;
         this.location.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
         this.location.Name = "location";
         // 
         // viewTitle
         // 
         resources.ApplyResources(this.viewTitle, "viewTitle");
         this.viewTitle.BackColor = System.Drawing.Color.Transparent;
         this.viewTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
         this.viewTitle.Name = "viewTitle";
         // 
         // logoImage
         // 
         this.logoImage.BackColor = System.Drawing.Color.Transparent;
         this.logoImage.Image = global::TabletApp.Properties.Resources.logo;
         resources.ApplyResources(this.logoImage, "logoImage");
         this.logoImage.Name = "logoImage";
         this.logoImage.TabStop = false;
         this.logoImage.Click += new System.EventHandler(this.logoImage_Click);
         // 
         // navButtonPanel
         // 
         resources.ApplyResources(this.navButtonPanel, "navButtonPanel");
         this.navButtonPanel.BackColor = System.Drawing.Color.Transparent;
         this.navButtonPanel.Controls.Add(this.button5, 5, 0);
         this.navButtonPanel.Controls.Add(this.button4, 4, 0);
         this.navButtonPanel.Controls.Add(this.button3, 2, 0);
         this.navButtonPanel.Controls.Add(this.button2, 1, 0);
         this.navButtonPanel.Controls.Add(this.button1, 0, 0);
         this.navButtonPanel.Name = "navButtonPanel";
         // 
         // button5
         // 
         resources.ApplyResources(this.button5, "button5");
         this.button5.BackColor = System.Drawing.Color.Transparent;
         this.button5.FlatAppearance.BorderSize = 0;
         this.button5.ForeColor = System.Drawing.Color.White;
         this.button5.Name = "button5";
         this.button5.UseVisualStyleBackColor = false;
         this.button5.EnabledChanged += new System.EventHandler(this.button1_EnabledChanged);
         this.button5.Click += new System.EventHandler(this.navButton_Click);
         // 
         // button4
         // 
         resources.ApplyResources(this.button4, "button4");
         this.button4.BackColor = System.Drawing.Color.Transparent;
         this.button4.FlatAppearance.BorderSize = 0;
         this.button4.ForeColor = System.Drawing.Color.White;
         this.button4.Name = "button4";
         this.button4.UseVisualStyleBackColor = false;
         this.button4.EnabledChanged += new System.EventHandler(this.button1_EnabledChanged);
         this.button4.Click += new System.EventHandler(this.navButton_Click);
         // 
         // button3
         // 
         resources.ApplyResources(this.button3, "button3");
         this.button3.BackColor = System.Drawing.Color.Transparent;
         this.button3.FlatAppearance.BorderSize = 0;
         this.button3.ForeColor = System.Drawing.Color.White;
         this.button3.Name = "button3";
         this.button3.UseVisualStyleBackColor = false;
         this.button3.EnabledChanged += new System.EventHandler(this.button1_EnabledChanged);
         this.button3.Click += new System.EventHandler(this.navButton_Click);
         // 
         // button2
         // 
         resources.ApplyResources(this.button2, "button2");
         this.button2.BackColor = System.Drawing.Color.Transparent;
         this.button2.FlatAppearance.BorderSize = 0;
         this.button2.ForeColor = System.Drawing.Color.White;
         this.button2.Name = "button2";
         this.button2.UseVisualStyleBackColor = false;
         this.button2.EnabledChanged += new System.EventHandler(this.button1_EnabledChanged);
         this.button2.Click += new System.EventHandler(this.navButton_Click);
         // 
         // button1
         // 
         resources.ApplyResources(this.button1, "button1");
         this.button1.BackColor = System.Drawing.Color.Transparent;
         this.button1.FlatAppearance.BorderSize = 0;
         this.button1.ForeColor = System.Drawing.Color.White;
         this.button1.Name = "button1";
         this.button1.Tag = "";
         this.button1.UseVisualStyleBackColor = false;
         this.button1.EnabledChanged += new System.EventHandler(this.button1_EnabledChanged);
         this.button1.Click += new System.EventHandler(this.navButton_Click);
         // 
         // contentContainer
         // 
         resources.ApplyResources(this.contentContainer, "contentContainer");
         this.contentContainer.BackColor = System.Drawing.Color.Transparent;
         this.contentContainer.Name = "contentContainer";
         // 
         // sidebarPanel
         // 
         resources.ApplyResources(this.sidebarPanel, "sidebarPanel");
         this.sidebarPanel.BackgroundImage = global::TabletApp.Properties.Resources.sidebar;
         this.sidebarPanel.Controls.Add(this.errorPictureBox);
         this.sidebarPanel.Controls.Add(this.quitButton);
         this.sidebarPanel.Controls.Add(this.settingsButton);
         this.sidebarPanel.Controls.Add(this.helpButton);
         this.sidebarPanel.Controls.Add(this.aboutButton);
         this.sidebarPanel.Controls.Add(this.homeButton);
         this.sidebarPanel.Name = "sidebarPanel";
         // 
         // errorPictureBox
         // 
         resources.ApplyResources(this.errorPictureBox, "errorPictureBox");
         this.errorPictureBox.BackColor = System.Drawing.Color.Transparent;
         this.errorPictureBox.Name = "errorPictureBox";
         this.errorPictureBox.TabStop = false;
         this.errorPictureBox.Click += new System.EventHandler(this.errorPictureBox_Click);
         // 
         // quitButton
         // 
         resources.ApplyResources(this.quitButton, "quitButton");
         this.quitButton.BackColor = System.Drawing.Color.Transparent;
         this.quitButton.FlatAppearance.BorderSize = 0;
         this.quitButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
         this.quitButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
         this.quitButton.ForeColor = System.Drawing.Color.White;
         this.quitButton.Image = global::TabletApp.Properties.Resources.quit;
         this.quitButton.Name = "quitButton";
         this.quitButton.TabStop = false;
         this.quitButton.Tag = "";
         this.quitButton.UseVisualStyleBackColor = false;
         this.quitButton.Click += new System.EventHandler(this.quitButton_Click);
         // 
         // settingsButton
         // 
         resources.ApplyResources(this.settingsButton, "settingsButton");
         this.settingsButton.BackColor = System.Drawing.Color.Transparent;
         this.settingsButton.FlatAppearance.BorderSize = 0;
         this.settingsButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
         this.settingsButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
         this.settingsButton.ForeColor = System.Drawing.Color.White;
         this.settingsButton.Image = global::TabletApp.Properties.Resources.settings;
         this.settingsButton.Name = "settingsButton";
         this.settingsButton.TabStop = false;
         this.settingsButton.Tag = "";
         this.settingsButton.UseVisualStyleBackColor = false;
         this.settingsButton.Click += new System.EventHandler(this.settingsButton_Click);
         // 
         // helpButton
         // 
         resources.ApplyResources(this.helpButton, "helpButton");
         this.helpButton.BackColor = System.Drawing.Color.Transparent;
         this.helpButton.FlatAppearance.BorderSize = 0;
         this.helpButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
         this.helpButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
         this.helpButton.ForeColor = System.Drawing.Color.White;
         this.helpButton.Image = global::TabletApp.Properties.Resources.help;
         this.helpButton.Name = "helpButton";
         this.helpButton.TabStop = false;
         this.helpButton.Tag = "";
         this.helpButton.UseVisualStyleBackColor = false;
         this.helpButton.Click += new System.EventHandler(this.helpButton_Click);
         // 
         // aboutButton
         // 
         resources.ApplyResources(this.aboutButton, "aboutButton");
         this.aboutButton.BackColor = System.Drawing.Color.Transparent;
         this.aboutButton.FlatAppearance.BorderSize = 0;
         this.aboutButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
         this.aboutButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
         this.aboutButton.ForeColor = System.Drawing.Color.White;
         this.aboutButton.Image = global::TabletApp.Properties.Resources.about;
         this.aboutButton.Name = "aboutButton";
         this.aboutButton.TabStop = false;
         this.aboutButton.Tag = "";
         this.aboutButton.UseVisualStyleBackColor = false;
         this.aboutButton.Click += new System.EventHandler(this.aboutButton_Click);
         // 
         // homeButton
         // 
         resources.ApplyResources(this.homeButton, "homeButton");
         this.homeButton.BackColor = System.Drawing.Color.Transparent;
         this.homeButton.BackgroundImage = global::TabletApp.Properties.Resources.sidebar_active;
         this.homeButton.FlatAppearance.BorderSize = 0;
         this.homeButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
         this.homeButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
         this.homeButton.ForeColor = System.Drawing.Color.White;
         this.homeButton.Image = global::TabletApp.Properties.Resources.home;
         this.homeButton.Name = "homeButton";
         this.homeButton.TabStop = false;
         this.homeButton.Tag = "";
         this.homeButton.UseVisualStyleBackColor = false;
         this.homeButton.Click += new System.EventHandler(this.homeButton_Click);
         // 
         // aAutoReadView1
         // 
         this.aAutoReadView1.BackColor = System.Drawing.Color.Transparent;
         resources.ApplyResources(this.aAutoReadView1, "aAutoReadView1");
         this.aAutoReadView1.Name = "aAutoReadView1";
         this.aAutoReadView1.TabStop = false;
         // 
         // spinnerProgress
         // 
         this.spinnerProgress.BackColor = System.Drawing.Color.Transparent;
         resources.ApplyResources(this.spinnerProgress, "spinnerProgress");
         this.spinnerProgress.Name = "spinnerProgress";
         this.spinnerProgress.ProgressText = "Progress...";
         this.spinnerProgress.TabStop = false;
         // 
         // WizardForm
         // 
         resources.ApplyResources(this, "$this");
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.BackColor = System.Drawing.Color.White;
         this.Controls.Add(this.sidebarPanel);
         this.Controls.Add(this.contentContainer);
         this.Controls.Add(this.navButtonPanel);
         this.Controls.Add(this.headerPanel);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "WizardForm";
         this.ShowIcon = false;
         this.headerPanel.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.logoImage)).EndInit();
         this.navButtonPanel.ResumeLayout(false);
         this.sidebarPanel.ResumeLayout(false);
         this.sidebarPanel.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this.errorPictureBox)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.Panel headerPanel;
      private System.Windows.Forms.Label viewTitle;
      private System.Windows.Forms.PictureBox logoImage;
      private System.Windows.Forms.TableLayoutPanel navButtonPanel;
      private System.Windows.Forms.Button button5;
      private System.Windows.Forms.Button button4;
      private System.Windows.Forms.Button button3;
      private System.Windows.Forms.Button button2;
      private System.Windows.Forms.Button button1;
      private System.Windows.Forms.Panel contentContainer;
      private System.Windows.Forms.Panel sidebarPanel;
      private System.Windows.Forms.Button homeButton;
      private System.Windows.Forms.Button settingsButton;
      private System.Windows.Forms.Button helpButton;
      private System.Windows.Forms.Button aboutButton;
      private System.Windows.Forms.Button quitButton;
      private System.Windows.Forms.Label location;
      private Views.SpinnerProgress spinnerProgress;
      private Views.AutoReadView aAutoReadView1;
      private System.Windows.Forms.PictureBox errorPictureBox;
      private System.Windows.Forms.Label infoLabel;
   }
}


