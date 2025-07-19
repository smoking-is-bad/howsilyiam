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

using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabletApp.Content;
using TabletApp.Properties;
using TabletApp.State;
using TabletApp.Utils;
using TabletApp.Views;

namespace TabletApp
{
   /// <summary>
   /// The main form for the app
   /// </summary>
   public partial class WizardForm : Form
   {
      private Image fBackgroundImage = null;
      private MenuStrip toolsMenu;

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="initialState">The initial state id</param>
      public WizardForm(string initialState)
      {
         InitializeComponent();

         // listen for display settings changes so can adjust for orientation changes
         SystemEvents.DisplaySettingsChanged += HandleDisplaySettingsChanged;
         
         // don't want our buttons to draw a border when they're in the background
         Color clearColor = Color.FromArgb(0, 255, 255, 255);
         this.homeButton.FlatAppearance.BorderColor = clearColor;
         this.aboutButton.FlatAppearance.BorderColor = clearColor;
         this.helpButton.FlatAppearance.BorderColor = clearColor;
         this.settingsButton.FlatAppearance.BorderColor = clearColor;
         this.quitButton.FlatAppearance.BorderColor = clearColor;
         this.button1.FlatAppearance.BorderColor = clearColor;
         this.button2.FlatAppearance.BorderColor = clearColor;
         this.button3.FlatAppearance.BorderColor = clearColor;
         this.button4.FlatAppearance.BorderColor = clearColor;
         this.button5.FlatAppearance.BorderColor = clearColor;

         // listen for state change events
         AStateController.Instance.StateChangeEvent += new StateChange(HandleStateChange);
         // listen for butt enable/disable events
         AStateController.Instance.PerformActionEvent += new PerformAction(HandlePerformAction);
         AStateController.Instance.ChangeToState(initialState);

         // error icon for telling the user about errors during app use - click displays the app log file
         this.errorPictureBox.Image = SystemIcons.Warning.ToBitmap();
         this.errorPictureBox.Visible = false;

         this.spinnerProgress.Visible = false;

         // Add Multi-Port menu item
         AddMultiPortMenuItem();
      }

      private void InitializeComponent()
      {
         this.toolsMenu = new MenuStrip();
         this.SuspendLayout();
         
         // Configure the toolsMenu properties
         this.toolsMenu.Name = "toolsMenu";
         this.toolsMenu.Text = "Tools";
         this.toolsMenu.Dock = DockStyle.Top;

         // Add the toolsMenu to the form's controls
         this.Controls.Add(this.toolsMenu);

         this.ResumeLayout(false);
         this.PerformLayout();
      }

      protected override void OnLoad(EventArgs e)
      {
         if (!AScaleUtils.ScaleMainForm(this))
         {
            this.Height = Screen.PrimaryScreen.WorkingArea.Height;
            this.Width = Screen.PrimaryScreen.WorkingArea.Width;
         }
         this.Location = Point.Empty;
         this.navButtonPanel.Top = this.Height - this.navButtonPanel.Height;
         //this.contentContainer.BorderStyle = BorderStyle.FixedSingle;       // debug

         base.OnLoad(e);
      }

      /// <summary>
      /// Prevent flickering when drawing the background image
      /// </summary>
      protected override CreateParams CreateParams
      {
         get
         {
            CreateParams cp = base.CreateParams;
            cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
            return cp;
         }
      }

      /// <summary>
      /// Handle orientation changes by adjusting our size
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void HandleDisplaySettingsChanged(object sender, EventArgs e)
      {
         this.Height = Screen.PrimaryScreen.WorkingArea.Height;
         this.Width = Screen.PrimaryScreen.WorkingArea.Width;
         this.PerformLayout();
      }

      /// <summary>
      /// Handle painting the background image so we can draw it with an offset.
      /// </summary>
      /// <param name="e"></param>
      protected override void OnPaintBackground(PaintEventArgs e)
      {
         base.OnPaintBackground(e);
         if (null != fBackgroundImage)
         {
            int xOffset = this.sidebarPanel.Width;
            int yOffset = this.headerPanel.Height;
            int width = this.Width - xOffset;
            int height = this.Height - yOffset;
            bool portraitImage = fBackgroundImage.Height > fBackgroundImage.Width;
            Rectangle dstRect = new Rectangle(xOffset, yOffset, width, height);
            Rectangle srcRect = Rectangle.Empty;
            if (portraitImage)
            {
               float srcRatio = (float)fBackgroundImage.Width / (float)this.Width;
               float srcHeight = (float)this.Height * srcRatio;
               float srcYOffset = ((float)fBackgroundImage.Height - srcHeight) / 2f;
               if (srcYOffset < 0)
               {
                  portraitImage = false;
               }
               srcRect = new Rectangle(0, (int)srcYOffset, fBackgroundImage.Width, (int)srcHeight);
            }
            if (!portraitImage)
            {
               float srcRatio = (float)fBackgroundImage.Height / (float)this.Height;
               float srcWidth = (float)this.Width * srcRatio;
               float srcXOffset = ((float)fBackgroundImage.Width - srcWidth) / 2f;
               srcRect = new Rectangle((int)srcXOffset, 0, (int)srcWidth, fBackgroundImage.Height);
            }
            e.Graphics.DrawImage(fBackgroundImage, dstRect, srcRect, GraphicsUnit.Pixel);
         }
         else
         {
            e.Graphics.Clear(Color.White);
         }
      }
      
      /// <summary>
      /// Update the global spinner progress
      /// </summary>
      /// <param name="visible">Shown or not</param>
      /// <param name="text">Progress text</param>
      private void UpdateSpinnerProgress(bool visible, string text)
      {
         if (visible)
         {
            this.spinnerProgress.Visible = true;
            this.spinnerProgress.SetSpinnerVisible(true);
            this.spinnerProgress.ProgressText = text;
            this.spinnerProgress.UpdateSpinner();
         }
         else
         {
            // if not visible, but text is specified, hide the spinner and show the text
            if (null != text)
            {
               this.spinnerProgress.Visible = true;
               this.spinnerProgress.SetSpinnerVisible(false);
               this.spinnerProgress.ProgressText = text;
            }
            else
            {
               this.spinnerProgress.Visible = false;
            }
         }
      }

      /// <summary>
      /// Handle actions.  These would be called by current state objects
      /// (user controls, etc) to eg handle enable/disable of the wizard buttons, etc.
      /// </summary>
      /// <param name="action"></param>
      /// <param name="data"></param>
      private void HandlePerformAction(string action, object data)
      {
         if (this.InvokeRequired)
         {
            this.Invoke(new Action(() => this.HandlePerformAction(action, data)));
            return;
         }

         // enable/disable wizard buttons
         if ("enablebutton" == action || "disablebutton" == action)
         {
            string buttonId = data as string;
            foreach (Button button in this.navButtonPanel.Controls)
            {
               if (buttonId == (string)button.Tag)
               {
                  button.Enabled = ("enablebutton" == action ? true : false);
               }
            }
         }
         // hide/show wizard buttons
         else if ("hidebutton" == action || "showbutton" == action)
         {
            string buttonId = data as string;
            foreach (Button button in this.navButtonPanel.Controls)
            {
               if (buttonId == (string)button.Tag)
               {
                  button.Visible = ("showbutton" == action ? true : false);
               }
            }
         }
         else if ("setbuttontitle" == action)
         {
            List<string> info = data as List<string>;
            if (null != info && 2 == info.Count)
            {
               string buttonId = info[0];
               string title = info[1];
               foreach (Button button in this.navButtonPanel.Controls)
               {
                  if (buttonId == (string)button.Tag)
                  {
                     button.Text = title;
                  }
               }
            }
         }
         // set the location label value
         else if ("setlocation" == action)
         {
            string locationString = data as string;
            this.location.Text = locationString;
         }
         // set the info label value
         else if ("setinfo" == action)
         {
            string infoString = data as string;
            this.infoLabel.Visible = infoString.Length > 0;
            this.infoLabel.Text = infoString;
         }
         else if ("showspinnerprogress" == action)
         {
            this.UpdateSpinnerProgress(true, (string)data);
         }
         else if ("updatespinnerprogress" == action)
         {
            this.UpdateSpinnerProgress(true, (string)data);
         }
         else if ("hidespinnerprogress" == action)
         {
            this.UpdateSpinnerProgress(false, (string)data);
         }
         else if ("keyboard" == action)
         {
            AUtilities.ShowKeyboard();
         }
         else if ("showerror" == action)
         {
            this.errorPictureBox.Visible = true;
         }
         else if ("changelanguage" == action)
         {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizardForm));
            this.helpButton.Text = resources.GetString("helpButton.Text");
            this.quitButton.Text = resources.GetString("quitButton.Text");
            this.homeButton.Text = resources.GetString("homeButton.Text");
            this.settingsButton.Text = resources.GetString("settingsButton.Text");
            this.aboutButton.Text = resources.GetString("aboutButton.Text");
         }
      }

      /// <summary>
      /// Respond to a state change by loading the new user control and populating
      /// the navigation buttons accordingly
      /// </summary>
      /// <param name="newState">The new state</param>
      private void HandleStateChange(AState newState)
      {
         if (this.InvokeRequired)
         {
            this.Invoke(new Action(() => this.HandleStateChange(newState)));
            return;
         }

         if (null == newState.Content)
         {
            return;
         }

         this.UpdateSpinnerProgress(false, null);
         this.UpdateNavButtons(newState);
         this.UpdateUserControl(newState);
         this.UpdateSidebarButtons();
         this.viewTitle.Text = newState.ExpandedValue("Title");
         if (null != newState.Background)
         {
            fBackgroundImage = Image.FromFile(Path.Combine("Background", newState.Background));
         }
         else
         {
            fBackgroundImage = null;
         }
         this.Invalidate();
         if (this.contentContainer.Controls.Count > 0)
         {
            // send out our DidShow notif
            BaseContent newContent = this.contentContainer.Controls[0] as BaseContent;
            newContent.DidShow();
         }

         // Configure Multi-Port button if applicable
         ConfigureMultiPortButton(newState);
      }

      /// <summary>
      /// Update the buttons on the left sidebar, mainly handling the background image per the current state.
      /// </summary>
      private void UpdateSidebarButtons()
      {
         if ("home" == AStateController.Instance.CurrentState.Id)
         {
            this.homeButton.BackgroundImage = Resources.sidebar_active;
         }
         else
         {
            this.homeButton.BackgroundImage = null;
         }

         if ("about" == AStateController.Instance.CurrentState.Id)
         {
            this.aboutButton.BackgroundImage = Resources.sidebar_active;
         }
         else
         {
            this.aboutButton.BackgroundImage = null;
         }

         if ("settings" == AStateController.Instance.CurrentState.Id)
         {
            this.settingsButton.BackgroundImage = Resources.sidebar_active;
         }
         else
         {
            this.settingsButton.BackgroundImage = null;
         }
      }

      /// <summary>
      /// Update the user control according to the given state.
      /// The content id defines the user control class to instantiate.
      /// </summary>
      /// <param name="state">New state</param>
      private void UpdateUserControl(AState state)
      {
         Type contentType = Type.GetType(state.Content);
         if (null == contentType)
         {
            throw new Exception("Invalid content type specified: " + state.Content);
         }
         BaseContent newContent = (BaseContent)Activator.CreateInstance(contentType, new object[] { state.ParamsAsDict });
         AScaleUtils.ScaleControl(newContent);
         // if the align param is "span" then force the content to span the entire area
         if ("span" == state.Align)
         {
            newContent.Left = 0;
            newContent.Width = this.contentContainer.Width;
            newContent.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
         }
         // otherwise just center it
         else 
         {
            newContent.Left = this.contentContainer.Width / 2 - newContent.Width / 2;
            newContent.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
         }
         newContent.Top = 0;
         newContent.Height = this.contentContainer.Height;
         //newContent.BorderStyle = BorderStyle.FixedSingle;      // debug
         // make sure the scroll bars will show when needed - disabled this since we apply scrolling inside the content views now
         //this.contentContainer.AutoScrollMinSize = newContent.Size;
         // remove any existing content and add the new one
         if (this.contentContainer.Controls.Count > 0)
         {
            BaseContent oldContent = this.contentContainer.Controls[0] as BaseContent;
            this.contentContainer.Controls.RemoveAt(0);
            oldContent.WillDisappear();
            // don't explicitly dispose here - allow GC to handle it
            //oldContent.Dispose();
         }
         this.RemoveUpDownControls();
         this.AddEventHandlers(newContent);
         this.contentContainer.Controls.Add(newContent);
         // select the first selectable control in the new content
         Control control = newContent.GetNextControl(null, true);
         control.Select();
      }

      /// <summary>
      /// Remove any lingering up/down controls that were added to the form (see ANumberTextBox)
      /// </summary>
      private void RemoveUpDownControls()
      {
         foreach (Control control in this.Controls)
         {
            if (control is UpDownControl)
            {
               this.Controls.Remove(control);
            }
         }
      }

      /// <summary>
      /// Add event handlers for listening to text box clicks so we can track
      /// when to show the keyboard (recursive function).
      /// </summary>
      /// <param name="control"></param>
      private void AddEventHandlers(Control control)
      {
         if ((control is TextBox && !(control is NumberTextBox)) || 
             (control is ComboBox && ComboBoxStyle.DropDown == ((ComboBox)control).DropDownStyle))
         {
            control.Click += control_Click;
         }
         foreach (Control child in control.Controls)
         {
            this.AddEventHandlers(child);
         }
      }

      /// <summary>
      /// Show the keyboard when user clicks a non-numeric text box.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      void control_Click(object sender, EventArgs e)
      {
         AUtilities.ShowKeyboard();
      }

      /// <summary>
      /// Update the nav buttons per the given state.
      /// </summary>
      /// <param name="state">New state</param>
      private void UpdateNavButtons(AState state)
      {
         if (state.Buttons.Count > navButtonPanel.ColumnCount)
         {
            throw new Exception("Invalid button definition for state");
         }

         List<int> unusedPositions = new List<int> { 0, 1, 2, 4, 5 };      // position 3 is a spacer column

         // show each valid button and set the title and id (tag)
         foreach (AButton button in state.Buttons)
         {
            if (button.Position < 1 || button.Position > navButtonPanel.ColumnCount)
            {
               throw new Exception("Invalid button position for state: id = " + button.Id + ", position = " + button.Position);
            }
            int position = this.TablePositionFromButtonPosition(button.Position);
            Button buttonControl = (Button)navButtonPanel.GetControlFromPosition(position, 0);
            buttonControl.Enabled = true;
            buttonControl.Visible = true;
            buttonControl.Text = button.ExpandedValue("Title");
            buttonControl.Tag = button.Id;
            buttonControl.Image = (null != button.Image ? (Image)Resources.ResourceManager.GetObject(button.Image) : null);
            buttonControl.TabStop = button.TabStop;
            unusedPositions.Remove(position);
         }

         // hide the remaining buttons
         foreach (int position in unusedPositions)
         {
            Button buttonControl = (Button)navButtonPanel.GetControlFromPosition(position, 0);
            if (null != buttonControl)
            {
               buttonControl.Visible = false;
            }
         }
      }

      /// <summary>
      /// Convert from a button position to a position (column) in our nav button table.
      /// Button position is 1-based and we also have the spacer column in the table.
      /// </summary>
      /// <param name="buttonPosition">1-based button position</param>
      /// <returns></returns>
      private int TablePositionFromButtonPosition(int buttonPosition)
      {
         if (buttonPosition <= 3)
         {
            return buttonPosition - 1;
         }
         return buttonPosition;
      }

      /// <summary>
      /// Handle a click of one of the nav buttons.  This causes a state change.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void navButton_Click(object sender, EventArgs e)
      {
         AStateController.Instance.ChangeToStateForControlId((string)((Button)sender).Tag);
      }

      private void logoImage_Click(object sender, EventArgs e)
      {
         AStateController.Instance.ChangeToState("home");
      }

      private void homeButton_Click(object sender, EventArgs e)
      {
         AStateController.Instance.ChangeToState("home");
      }

      private void aboutButton_Click(object sender, EventArgs e)
      {
         AStateController.Instance.ChangeToState("about");
      }

      private void helpButton_Click(object sender, EventArgs e)
      {
         Process.Start("Help\\manual.pdf");
      }

      private void settingsButton_Click(object sender, EventArgs e)
      {
         AStateController.Instance.ChangeToState("settings");
      }

      private void quitButton_Click(object sender, EventArgs e)
      {
         if (DialogResult.Yes == AOutput.DisplayYesNo(Resources.QuitMessage, Resources.QuitCaption))
         {
            Application.Exit();
         }
      }

      private void button1_EnabledChanged(object sender, EventArgs e)
      {
         Button button = sender as Button;
         button.BackgroundImage = (button.Enabled ? Resources.blue_button_small : Resources.gray_button);
      }

      private void errorPictureBox_Click(object sender, EventArgs e)
      {
         AUtilities.OpenLogFile();
         this.errorPictureBox.Visible = false;
      }

      /// <summary>
      /// Add a menu item for Multi-Port Scan.
      /// </summary>
      private void AddMultiPortMenuItem()
      {
         var multiPortMenuItem = new ToolStripMenuItem("Multi-Port Scan");
         multiPortMenuItem.Click += (sender, e) =>
         {
            AStateController.Instance.ChangeToState("multiportscan");
         };

         this.toolsMenu.DropDownItems.Add(multiPortMenuItem);
      }

      /// <summary>
      /// Configure Multi-Port button for applicable states.
      /// </summary>
      /// <param name="currentState">Current state</param>
      private void ConfigureMultiPortButton(AState currentState)
      {
         if (currentState.Id == "home" || currentState.Id == "scan")
         {
            var button = new AButton
            {
               Id = "multiport",
               Title = "Multi-Port",
               Position = 3,
               Target = "multiportscan"
            };
            currentState.Buttons.Add(button);
         }
      }
   }
}

