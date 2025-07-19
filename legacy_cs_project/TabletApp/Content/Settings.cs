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

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using TabletApp.State;
using System.Security;
using TabletApp.Utils;
using System.Globalization;
using System.IO.Ports;
using System.IO;
using System.Drawing;

namespace TabletApp.Content
{
   /// <summary>
   /// User control for the settings screen
   /// </summary>
   public partial class Settings : BaseContent
   {
      private Timer fPortRefreshTimer = new Timer();
      private string[] fPortNames;

      // Add these controls to the Settings form
      private CheckBox enableMultiPortCheckBox;
      private NumericUpDown portScanTimeoutUpDown;

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="parameters">Parameters from the state def file</param>
      public Settings(Dictionary<string, string> parameters)
         : base(parameters)
      {
         InitializeComponent();
         AStateController.Instance.PerformActionEvent += HandlePerformAction;

         Properties.Settings.Default.Reload();

         this.InitializeMultiPortSettings();
         this.LoadSettings();

         this.StartPortTimer();
      }

      /// <summary>
      /// Start the timer that refreshes our port name list.
      /// </summary>
      private void StartPortTimer()
      {
         fPortRefreshTimer.Interval = 1000;
         fPortRefreshTimer.Tick += PortRefreshTimer_Tick;
         fPortRefreshTimer.Start();
      }

      /// <summary>
      /// Stop the timer that refreshes our port name list.
      /// </summary>
      private void StopPortTimer()
      {
         fPortRefreshTimer.Stop();
      }

      /// <summary>
      /// Port name list timer proc.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void PortRefreshTimer_Tick(object sender, EventArgs e)
      {
         var portNames = SerialPort.GetPortNames();
         // refresh only if different from current
         if (!Enumerable.SequenceEqual(portNames, fPortNames))
         {
            fPortNames = portNames;
            this.comPortCombo.Items.Clear();
            this.comPortCombo.Items.AddRange(fPortNames);
         }
      }

      /// <summary>
      /// Load all user settings into our controls.
      /// </summary>
      private void LoadSettings()
      {
         fPortNames = SerialPort.GetPortNames();
         this.comPortCombo.Items.AddRange(fPortNames);
         this.comPortCombo.SelectedItem = Properties.Settings.Default.ComPort;
         this.smtpServer.Text = Properties.Settings.Default.EmailSmtpServer;
         this.smtpServerPort.Text = Properties.Settings.Default.EmailSmtpServerPort.ToString();
         this.fromAddress.Text = Properties.Settings.Default.EmailFromAddress;
         this.username.Text = Properties.Settings.Default.EmailUsername;
         SecureString password = Properties.Settings.Default.EmailPassword.DecryptString();
         this.password.Text = password.ToInsecureString();
         this.mmRadioButton.Checked = Properties.Settings.Default.ThicknessIsMetric;
         this.inchesRadioButton.Checked = !Properties.Settings.Default.ThicknessIsMetric;
         this.mmPrecisionCombo.SelectedIndex = this.mmPrecisionCombo.Items.IndexOf(Properties.Settings.Default.PrecisionMm.ToString());
         this.inchesPrecisionCombo.SelectedIndex = this.inchesPrecisionCombo.Items.IndexOf(Properties.Settings.Default.PrecisionInches.ToString());
         this.autoReadOnRadioButton.Checked = Properties.Settings.Default.AutoRead;
         this.autoReadOffRadioButton.Checked = !Properties.Settings.Default.AutoRead;
         this.autoUploadOnRadioButton.Checked = Properties.Settings.Default.AutoUpload;
         this.autoUploadOffRadioButton.Checked = !Properties.Settings.Default.AutoUpload;
         this.ascanZoom.IntValue = Properties.Settings.Default.AscanZoom;
         this.ascanScroll.IntValue = Properties.Settings.Default.AscanScroll;
         this.lockAscans.Checked = Properties.Settings.Default.LockAscans;
         this.csvSaveCheckbox.Checked = Properties.Settings.Default.CsvLogSave;
         this.enableMultiPortCheckBox.Checked = Properties.Settings.Default.EnableMultiPort;
         this.portScanTimeoutUpDown.Value = Properties.Settings.Default.PortScanTimeout;
         this.LoadAutoReadDelay();
         this.LoadLanguage();
         this.LoadBaudRate();
      }

      /// <summary>
      /// Save all our settings.
      /// </summary>
      private void SaveSettings()
      {
         Properties.Settings.Default.ComPort = this.comPortCombo.SelectedItem as string;
         Properties.Settings.Default.EmailSmtpServer = this.smtpServer.Text;
         Properties.Settings.Default.EmailSmtpServerPort = int.Parse(this.smtpServerPort.Text);
         Properties.Settings.Default.EmailFromAddress = this.fromAddress.Text;
         Properties.Settings.Default.EmailUsername = this.username.Text;
         SecureString secure = this.password.Text.ToSecureString();
         Properties.Settings.Default.EmailPassword = secure.EncryptString();
         Properties.Settings.Default.ThicknessIsMetric = this.mmRadioButton.Checked;
         Properties.Settings.Default.PrecisionInches = Convert.ToInt32(this.inchesPrecisionCombo.SelectedItem as string);
         Properties.Settings.Default.PrecisionMm = Convert.ToInt32(this.mmPrecisionCombo.SelectedItem as string);
         Properties.Settings.Default.AutoRead = this.autoReadOnRadioButton.Checked;
         Properties.Settings.Default.AutoUpload = this.autoUploadOnRadioButton.Checked;
         Properties.Settings.Default.AscanZoom = this.ascanZoom.IntValue;
         Properties.Settings.Default.AscanScroll = this.ascanScroll.IntValue;
         Properties.Settings.Default.LockAscans = this.lockAscans.Checked;
         Properties.Settings.Default.CsvLogSave = this.csvSaveCheckbox.Checked;
         Properties.Settings.Default.EnableMultiPort = this.enableMultiPortCheckBox.Checked;
         Properties.Settings.Default.PortScanTimeout = (int)this.portScanTimeoutUpDown.Value;
         this.SaveAutoReadDelay();
         this.SaveLanguage();
         this.SaveBaudRate();
         Properties.Settings.Default.Save();
      }

      /// <summary>
      /// About to disappear - clean up.
      /// </summary>
      public override void WillDisappear()
      {
         // unlisten for our action events
         AStateController.Instance.PerformActionEvent -= HandlePerformAction;
         this.StopPortTimer();
      }

      /// <summary>
      /// Handle the "save" action from the wizard button.
      /// </summary>
      /// <param name="actionName"></param>
      /// <param name="data"></param>
      private void HandlePerformAction(string actionName, object data)
      {
         if ("save" == actionName)
         {
            this.SaveSettings();
         }
      }

      /// <summary>
      /// Load the language dropdown contents and set to the current language.
      /// </summary>
      private void LoadLanguage()
      {
         List<CultureInfo> languages = AUtilities.GetSupportedLanguages();
         string currLanguage = Properties.Settings.Default.Language;

         // populate our language combo
         this.languageCombo.Items.AddRange(languages.Select(l => l.DisplayName).ToArray());

         // now find the language to select in the combo according to our current language setting
         CultureInfo info = languages.Find(l => currLanguage.ToLower() == l.TwoLetterISOLanguageName.ToLower());
         this.languageCombo.SelectedIndex = (null != info ? languages.IndexOf(info) : 0);
      }

      /// <summary>
      /// Save the current language setting and set it to the current language for the app.
      /// </summary>
      private void SaveLanguage()
      {
         List<CultureInfo> languages = AUtilities.GetSupportedLanguages();
         CultureInfo currCulture = languages[this.languageCombo.SelectedIndex];
         Properties.Settings.Default.Language = currCulture.TwoLetterISOLanguageName;
         AUtilities.SetLanguageFromSettings();
         AStateController.Instance.PostPerformActionEvent("changelanguage", null);
      }

      /// <summary>
      /// Load the auto-read delay setting into the UI.
      /// </summary>
      private void LoadAutoReadDelay()
      {
         // delay stored in seconds
         TimeSpan span = TimeSpan.FromSeconds(Properties.Settings.Default.AutoReadDelay);
         if (0 == span.Minutes && 0 == span.Hours)
         {
            // default to 1 hour (handles old default of 60 seconds)
            span = TimeSpan.FromSeconds(3600);
         }
         this.autoReadMinsTextBox.Text = span.Minutes.ToString("00");
         this.autoReadHoursTextBox.Text = span.Hours.ToString("00");
      }

      /// <summary>
      /// Store the auto-delay value to settings.
      /// </summary>
      private void SaveAutoReadDelay()
      {
         int hours = this.autoReadHoursTextBox.IntValue;
         int mins = this.autoReadMinsTextBox.IntValue;
         //Settings.Default.AutoReadDelay = 15;    //debug
         Properties.Settings.Default.AutoReadDelay = hours * 3600 + mins * 60;
      }

      /// <summary>
      /// Populate and load the baud rate setting.
      /// </summary>
      private void LoadBaudRate()
      {
         List<int> rates = new List<int>() { 110, 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 28800, 38400, 56000, 57600, 115200, 230400, 460800, 921600 };

         foreach (int rate in rates)
         {
            this.baudRateCombo.Items.Add(rate);
         }
         this.baudRateCombo.SelectedItem = Properties.Settings.Default.ComBaudRate;
      }

      /// <summary>
      /// Save the baud rate setting.
      /// </summary>
      private void SaveBaudRate()
      {
         Properties.Settings.Default.ComBaudRate = (int)this.baudRateCombo.SelectedItem;
      }

      /// <summary>
      /// Respond to the useSim checkbox changing by updating our XML config file options.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void useSimCheckbox_CheckedChanged(object sender, EventArgs e)
      {
         //this.UpdateXmlConfig(((CheckBox)sender).Checked);
      }

      /// <summary>
      /// Limit auto-read delay values to numbers.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void autoReadDelay_KeyPress(object sender, KeyPressEventArgs e)
      {
         if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
         {
            e.Handled = true;
         }
      }

      /// <summary>
      /// Initialize multi-port settings controls.
      /// </summary>
      private void InitializeMultiPortSettings()
      {
         this.enableMultiPortCheckBox = new CheckBox
         {
            Text = "Enable Multi-Port Scanning",
            Location = new Point(20, 400),
            Size = new Size(200, 20)
         };

         this.portScanTimeoutUpDown = new NumericUpDown
         {
            Location = new Point(20, 430),
            Size = new Size(100, 20),
            Minimum = 10,
            Maximum = 300,
            Value = 30,
            Increment = 10
         };

         var timeoutLabel = new Label
         {
            Text = "Port Scan Timeout (seconds):",
            Location = new Point(130, 432),
            Size = new Size(150, 20)
         };

         this.Controls.Add(this.enableMultiPortCheckBox);
         this.Controls.Add(this.portScanTimeoutUpDown);
         this.Controls.Add(timeoutLabel);
      }
   }
}

