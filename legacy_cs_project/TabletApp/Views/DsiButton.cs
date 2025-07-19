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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabletApp.Properties;

namespace TabletApp.Views
{
   public enum DsiButtonState
   {
      NotConnected,
      Connected,
      Empty,
      Warning,
      Error
   }

   /// <summary>
   /// Delegate method for receiving DSI button click notifications
   /// </summary>
   /// <param name="button">The button that was clicked</param>
   public delegate void ButtonClicked(DsiButton button);

   /// <summary>
   /// A button in the DSI grid.  Can be selected and also has multiple
   /// possible states defined by <code>DsiButtonState</code>.  The button
   /// is actually embedded within a user control.
   /// </summary>
   public partial class DsiButton : UserControl
   {
      public event ButtonClicked ButtonClickedEvent;

      public bool Selectable { get; set; }

      private bool fSelected;
      public bool Selected 
      { 
         get
         {
            return fSelected;
         }

         set
         {
            fSelected = value;
            this.UpdateButton();
         }
      }

      private DsiButtonState fState;
      public DsiButtonState State 
      {
         get
         {
            return fState;
         }

         set
         {
            fState = value;
            this.UpdateButton();
         }
      }

      /// <summary>
      /// The index of the button within the grid.  This is 0-based and has a range
      /// of 0-31.
      /// </summary>
      public int Index
      {
         get
         {
            int index = -1;
            if (this.Parent is TableLayoutPanel)
            {
               TableLayoutPanel parent = (TableLayoutPanel)this.Parent;
               TableLayoutPanelCellPosition pos = parent.GetPositionFromControl(this);
               index = (Convert.ToInt16(parent.Tag) * parent.ColumnCount) + pos.Column;
            }

            return index;
         }
      }

      /// <summary>
      /// Constructor
      /// </summary>
      public DsiButton()
      {
         InitializeComponent();
         this.Selectable = false;
         this.State = DsiButtonState.Empty;
      }

      /// <summary>
      /// Update the button contents
      /// </summary>
      private void UpdateButton()
      {
         if (this.InvokeRequired)
         {
            this.Invoke(new Action(() => this.UpdateButton()));
            return;
         }

         this.button.Text = this.GetButtonText();
         this.button.BackgroundImage = this.GetButtonImage();
         this.Selectable = (DsiButtonState.Connected == this.State || DsiButtonState.Warning == this.State || DsiButtonState.Error == this.State);
         this.button.Update();
      }

      /// <summary>
      /// Get the image that the button should show according
      /// to its current state.
      /// </summary>
      /// <returns>The image for the button</returns>
      private Image GetButtonImage()
      {
         Image image = null;

         switch (this.State)
         {
            case DsiButtonState.Connected:
               image = (this.Selected ? Resources.dsi_active : Resources.dsi_blank);
               break;
            case DsiButtonState.NotConnected:
               image = Resources.dsi;
               break;
            case DsiButtonState.Empty:
               image = Resources.dsi_blank;
               break;
            case DsiButtonState.Warning:
               image = (this.Selected ? Resources.dsi_warning_active : Resources.dsi_warning);
               break;
            case DsiButtonState.Error:
               image = (this.Selected ? Resources.dsi_error_active : Resources.dsi_error);
               break;
         }

         return image;
      }

      /// <summary>
      /// Get the text that the button should display according to
      /// its current state.
      /// </summary>
      /// <returns>String to display</returns>
      private string GetButtonText()
      {
         int index = this.Index;
         string text = (fSelected || DsiButtonState.NotConnected == this.State || DsiButtonState.Empty == this.State ? "" : Convert.ToString(index + 1));

         return text;
      }

      /// <summary>
      /// Handle a button click event by sending out the higher level
      /// event to any listeners.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void button_Click(object sender, EventArgs e)
      {
         if (!this.Selectable)
         {
            return;
         }

         if (null != this.ButtonClickedEvent)
         {
            this.ButtonClickedEvent(this);
         }
      }
   }
}

