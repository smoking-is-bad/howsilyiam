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
using TabletApp.Utils;
using Model;

namespace TabletApp.Views
{
   /// <summary>
   /// Delegate for receiving DSI button clicks to know when the current
   /// DSI has changed.
   /// </summary>
   /// <param name="buttonIndex"></param>
   public delegate void DsiButtonClicked(int buttonIndex);

   /// <summary>
   /// A user control that defines a TableLayoutPanel representing
   /// a grid of 32 DSIs, each with different states.
   /// </summary>
   public partial class DsiGrid : UserControl
   {
      public event DsiButtonClicked DsiButtonClickedEvent;

      public bool Selectable { get; set; }

      /// <summary>
      /// The starting position of the selector line
      /// </summary>
      private Point fSelectorStartPos;

      /// <summary>
      /// The vertical end of the horizontal line
      /// </summary>
      private int fHorizEnd;

      private int fSelectedIndex = 0;

      /// <summary>
      /// Determine the "active" DSI account according the button states.
      /// </summary>
      public int ActiveDsiCount
      {
         get
         {
            int count = 0;
            foreach (DsiButton button in this.AllButtons())
            {
               if (DsiButtonState.NotConnected != button.State && DsiButtonState.Empty != button.State)
               {
                  ++count;
               }
            }

            return count;
         }
      }

      /// <summary>
      /// Set the visibility of the selector line that gets drawn from the current DSI.
      /// </summary>
      public bool LineVisible
      {
         set
         {
            this.horizLine.Visible = value;
            this.selectorLine.Visible = value;
         }
      }

      /// <summary>
      /// Constructor
      /// </summary>
      public DsiGrid()
      {
         InitializeComponent();

         fSelectorStartPos = this.selectorLine.Location;
         fHorizEnd = this.horizLine.Right;

         // set the initial state of all the buttons
         foreach (DsiButton button in this.AllButtons())
         {
            button.ButtonClickedEvent += button_ButtonClickedEvent;
         }

         // make selectable and select the first dsi by default
         this.Selectable = true;
         this.SelectDsi(0);
      }
      
      /// <summary>
      /// Reset the grid and set all DSIs to "not connected".
      /// </summary>
      public void Clear()
      {
         if (this.InvokeRequired)
         {
            this.Invoke(new Action(() => this.Clear()));
            return;
         }

         foreach (DsiButton button in this.AllButtons())
         {
            button.State = DsiButtonState.NotConnected;
         }
         this.SelectDsi(0);
      }

      /// <summary>
      /// Add a new DSI.  New DSIs are added sequentially and by default are assumed to in the
      /// Connected state, unless otherwise specified (eg Warning state).
      /// </summary>
      /// <param name="dsiInfo"></param>
      /// <param name="alertState"></param>
      public void AddDsi(Model.ADsiInfo dsiInfo, AlertState alertState = AlertState.Normal)
      {
         if (this.InvokeRequired)
         {
            this.Invoke(new Action(() => this.AddDsi(dsiInfo, alertState)));
            return;
         }

         DsiButton button = this.GetNextAvailableButton();
         if (null != button)
         {
            button.State = this.ButtonStateForAlertState(alertState);
         }
         this.dsiButtonGrid1.Update();
         this.dsiButtonGrid2.Update();
      }

      /// <summary>
      /// Select the DSI button at the given index.
      /// </summary>
      /// <param name="dsiIndex">index of the DSI button to select</param>
      public void SelectDsi(int dsiIndex)
      {
         fSelectedIndex = dsiIndex;

         foreach (DsiButton button in this.AllButtons())
         {
            if (dsiIndex == button.Index)
            {
               button.Selected = true;
            }
            else
            {
               button.Selected = false;
            }
         }

         this.PositionSelectorLine();
      }

      /// <summary>
      /// Get the next "empty" button that we can populate.
      /// </summary>
      /// <returns></returns>
      private DsiButton GetNextAvailableButton()
      {
         DsiButton availButton = null;

         foreach (DsiButton button in this.AllButtons())
         {
            // find the the first non-connect buttn and assign that to the new DSI by setting its state
            if (DsiButtonState.NotConnected == button.State || DsiButtonState.Empty == button.State)
            {
               availButton = button;
               break;
            }
         }

         return availButton;
      }

      /// <summary>
      /// Since we store the buttons in 2 (staggered) panels, we need to combine
      /// all the buttons from those two panels in a list.
      /// </summary>
      /// <returns>List of buttons</returns>
      public List<DsiButton> AllButtons()
      {
         List<DsiButton> buttons = new List<DsiButton>();
         buttons.AddRange(this.dsiButtonGrid1.Controls.OfType<DsiButton>());
         buttons.AddRange(this.dsiButtonGrid2.Controls.OfType<DsiButton>());

         return buttons;
      }

      /// <summary>
      /// Get the button state for the given AlertState.
      /// </summary>
      /// <param name="alertState">The alert state</param>
      /// <returns>DsiButtonState</returns>
      private DsiButtonState ButtonStateForAlertState(AlertState alertState)
      {
         switch (alertState)
         {
            case AlertState.Normal:
               return DsiButtonState.Connected;
            case AlertState.Warning:
               return DsiButtonState.Warning;
            case AlertState.Error:
               return DsiButtonState.Error;
            default:
               return DsiButtonState.Connected;
         }
      }

      /// <summary>
      /// Set the position of the selector line according to the selected DSI.
      /// </summary>
      private void PositionSelectorLine()
      {
         // !!! HACK - unscale and then rescale when we are positioning our selector line
         // Any abolute positioning/sizing needs to happen in unscaled coords (except if we haven't 
         // been added to the form yet, since scaling isn't yet applied at that time).
         if (null != this.FindForm() && AScaleUtils.NeedsScale)
         {
            this.Scale(new SizeF(1f / AScaleUtils.ScaleFactor, 1f / AScaleUtils.ScaleFactor));
         }
         bool isScaled = AScaleUtils.NeedsScale;
         bool isLow = (fSelectedIndex >= this.dsiButtonGrid1.ColumnCount);
         int fudge = (AScaleUtils.NeedsScale ? (int)Math.Ceiling((double)(isLow ? (fSelectedIndex % this.dsiButtonGrid1.ColumnCount) : fSelectedIndex) / 3.0) : 1);
         int xOffset = fSelectorStartPos.X + (isLow ? 29 + (fSelectedIndex % this.dsiButtonGrid1.ColumnCount) * 2 * 29 - fudge : fSelectedIndex * 2 * 29 - fudge);
         int yOffset = (isLow ? fSelectorStartPos.Y + 62 : fSelectorStartPos.Y);
         this.selectorLine.Location = new Point(xOffset, yOffset);
         this.horizLine.Left = (xOffset > fHorizEnd ? fHorizEnd - 1 : xOffset);
         this.horizLine.Width = (xOffset > fHorizEnd ? xOffset - (fHorizEnd - 1) : fHorizEnd - xOffset);
         if (null != this.FindForm() && AScaleUtils.NeedsScale)
         {
            this.Scale(new SizeF(AScaleUtils.ScaleFactor, AScaleUtils.ScaleFactor));
         }
      }

      /// <summary>
      /// Handle a DSI button click.  Only one can be selected at a time, so select
      /// the new one and deselect the others.
      /// </summary>
      /// <param name="button"></param>
      private void button_ButtonClickedEvent(DsiButton button)
      {
         if (!this.Selectable)
         {
            return;
         }

         if (null != this.DsiButtonClickedEvent)
         {
            this.DsiButtonClickedEvent(button.Index);
         }

         this.SelectDsi(button.Index);
      }
   }
}

