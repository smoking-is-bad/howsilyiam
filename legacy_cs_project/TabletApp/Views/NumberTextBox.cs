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
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabletApp.Content;
using TabletApp.Utils;


namespace TabletApp.Views
{
   public delegate void EntryComplete(NumberTextBox numberTextBox);

   /// <summary>
   /// Number-only text field (positive integers only).
   /// </summary>
   public class NumberTextBox : TextBox
   {
      [DllImport("user32.dll")]
      private static extern IntPtr GetFocus();

      private UpDownControl fUpDownControl;
      private bool fMouseUp = false;      // indicates mouse up on the up/down control after a tap/hold
      private float fAccelParam = 1f;     // acceleration factor for tap/hold
      protected float fPrevValue = 0f;    // most recent valid value
      private int fUpDownOffset = 1;
      private bool fUsePrevValue = true;

      public event EntryComplete EntryCompleteEvent;

      public float Increment { get; set; }
      public float MinValue { get; set; }
      public float MaxValue { get; set; }

      public int IntValue 
      {
         set
         {
            value = Math.Min(value, Convert.ToInt32(this.MaxValue));
            value = Math.Max(value, Convert.ToInt32(this.MinValue));
            this.Text = Convert.ToString(value);
            fPrevValue = value;
         }
         get
         {
            int val;
            try
            {
               double dval = this.Text.Length > 0 ? Convert.ToDouble(this.Text) : 0;
               val = Convert.ToInt32(dval);
            }
            catch (Exception)
            {
               val = 0;
            }
            return (int)this.GetValidValue((float)val);
         }
      }

      public virtual float Value
      {
         set
         {
            this.IntValue = (int)value;
         }
         get
         {
            return (float)this.IntValue;
         }
      }

      /// <summary>
      /// Constructor
      /// </summary>
      public NumberTextBox()
      {
         this.Increment = 1;
         this.MinValue = Int16.MinValue;
         this.MaxValue = Int16.MaxValue;
         this.Value = 0;
         this.KeyPress += NumberTextBox_KeyPress;
         this.GotFocus += NumberTextBox_GotFocus;
         this.LostFocus += NumberTextBox_LostFocus;
         this.Click += NumberTextBox_Click;
      }

      /// <summary>
      /// Set the horizontal offset for the up/down control position.
      /// </summary>
      /// <param name="offset"></param>
      public void SetUpDownOffset(int offset)
      {
         fUpDownOffset = offset;
      }

      /// <summary>
      /// Get a valid value for the given value.  Use the most recent valid value
      /// if the given value is out of range.
      /// </summary>
      /// <param name="value"></param>
      /// <returns></returns>
      protected float GetValidValue(float value)
      {
         if (this.Focused && fUsePrevValue && (value < this.MinValue || value > this.MaxValue))
         {
            return fPrevValue;
         }
         return value;
      }

      /// <summary>
      /// Timer handler for click/hold on the up/down buttons
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      void timer_Tick(object sender, EventArgs e)
      {
         Timer timer = (Timer)sender;
         if (fMouseUp)
         {
            timer.Stop();
            return;
         }
         this.Value += (float)timer.Tag;
         timer.Interval = (int)(500f / fAccelParam);
         if (fAccelParam <= 10)
         {
            fAccelParam += 0.1f;
         }
      }

      /// <summary>
      /// Start the timer so we can handle click/hold after a mouse down.
      /// </summary>
      /// <param name="increment"></param>
      void StartMouseDownTimer(float increment)
      {
         fMouseUp = false;
         fAccelParam = 1f;
         Timer timer = new Timer();
         timer.Interval = 700;
         timer.Tag = increment;
         timer.Tick += timer_Tick;
         timer.Start();
      }

      void UpButton_MouseDown(object sender, EventArgs e)
      {
         this.Value += this.Increment;
         // restore focus to ourselves
         this.Focus();
         this.StartMouseDownTimer(this.Increment);
      }

      void DownButton_MouseDown(object sender, EventArgs e)
      {
         this.Value -= this.Increment;
         // restore focus to ourselves
         this.Focus();
         this.StartMouseDownTimer(-this.Increment);
      }

      private void DownButton_MouseUp(object sender, MouseEventArgs e)
      {
         fMouseUp = true;
         this.PostEntryCompleteEvent();
      }

      private void UpButton_MouseUp(object sender, MouseEventArgs e)
      {
         fMouseUp = true;
         this.PostEntryCompleteEvent();
      }

      private void NumberTextBox_GotFocus(object sender, EventArgs e)
      {
         this.AddUpDownControl();
         fUpDownControl.Visible = true;
         AUtilities.HideKeyboard();
         this.PositionUpDownControl();
      }

      private void NumberTextBox_LostFocus(object sender, EventArgs e)
      {
         bool hideIt = true;
         IntPtr focus = GetFocus();
         if (null != focus)
         {
            Control focusControl = Control.FromHandle(focus);
            if (focusControl == fUpDownControl || focusControl == fUpDownControl.UpButton || focusControl == fUpDownControl.DownButton || fUpDownControl.NumberPad.IsChild(focusControl))
            {
               hideIt = false;
            }
         }
         if (hideIt)
         {
            fUpDownControl.Visible = false;
            this.Value = this.Value;
            this.PostEntryCompleteEvent();
         }
      }

      private void NumberTextBox_KeyPress(object sender, KeyPressEventArgs e)
      {
         if ((char)Keys.Enter == e.KeyChar && fUpDownControl.Visible)
         {
            //Control form = this.FindForm();
            //bool success = form.SelectNextControl(this, true, true, true, true);
            fUpDownControl.Visible = false;
            fUsePrevValue = false;
            this.Value = this.Value;
            fUsePrevValue = true;
            this.SelectAll();
            this.PostEntryCompleteEvent();
            e.Handled = true;
         }
         else if ((char)Keys.Enter == e.KeyChar)
         {
            fUpDownControl.Visible = true;
            this.SelectAll();
            AUtilities.HideKeyboard();
            e.Handled = true;
         }
         else
         {
            e.Handled = !this.ValidateText(e.KeyChar);
         }
      }

      private void NumberTextBox_Click(object sender, EventArgs e)
      {
         fUpDownControl.Visible = true;
         this.SelectAll();
         AUtilities.HideKeyboard();
      }

      private void NumberPad_KeyHitEvent(NumberKey key)
      {
         if (key >= NumberKey.Key0 && key <= NumberKey.Key9)
         {
            this.Focus();
            SendKeys.Send("{" + (int)key + "}");
         }
         else if (NumberKey.KeyPoint == key)
         {
            this.Focus();
            SendKeys.Send(".");
         }
         else if (NumberKey.KeyNeg == key)
         {
            this.Focus();
            SendKeys.Send("-");
         }
         else if (NumberKey.KeyDel == key)
         {
            this.Focus();
            SendKeys.Send("{BKSP}");
         }
         else if (NumberKey.KeyNext == key)
         {
            this.Focus();
            SendKeys.Send("{TAB}");
         }
         else if (NumberKey.KeyPrev == key)
         {
            this.Focus();
            SendKeys.Send("+{TAB}");
         }
         else if (NumberKey.KeyEnter == key)
         {
            this.Focus();
            SendKeys.Send("{ENTER}");
         }
      }

      /// <summary>
      /// Add the up/down control for inc/dec the value.
      /// </summary>
      private void AddUpDownControl()
      {
         if (null != fUpDownControl)
         {
            return;
         }

         if (this.InvokeRequired)
         {
            this.Invoke(new Action(() => this.AddUpDownControl()));
            return;
         }

         this.CreateControl();
         this.Parent.CreateControl();
         fUpDownControl = new UpDownControl();
         fUpDownControl.Visible = false;
         fUpDownControl.UpButton.MouseDown += UpButton_MouseDown;
         fUpDownControl.DownButton.MouseDown += DownButton_MouseDown;
         fUpDownControl.UpButton.MouseUp += UpButton_MouseUp;
         fUpDownControl.DownButton.MouseUp += DownButton_MouseUp;
         fUpDownControl.NumberPad.KeyHitEvent += NumberPad_KeyHitEvent;
         fUpDownControl.NumberPad.AllowDecimal(this is FloatTextBox || this is FloatNegTextBox);
         fUpDownControl.NumberPad.AllowNegative(this is FloatNegTextBox);
         float diff = this.MaxValue - this.MinValue;
         // add to the form itself so the up/down sits atop everything else
         Control form = this.FindForm();
         this.PositionUpDownControl();
         form.Controls.Add(fUpDownControl);
         form.Controls.SetChildIndex(fUpDownControl, 0);
      }

      /// <summary>
      /// Position the up/down control such that it comes up to the right of its associated text box.
      /// </summary>
      private void PositionUpDownControl()
      {
         fUpDownControl.Left = this.Right + fUpDownOffset;
         fUpDownControl.Top = this.Top - this.AutoScrollOffset.Y - (fUpDownControl.Height - this.Height) / 2;
         Point topLeft = this.Parent.PointToScreen(fUpDownControl.Location);
         topLeft = this.FindForm().PointToClient(topLeft);
         fUpDownControl.Location = topLeft;
      }
      
      /// <summary>
      /// Post our EntryComplete event, signaling that the user completed numerical entry.
      /// </summary>
      private void PostEntryCompleteEvent()
      {
         if (null != this.EntryCompleteEvent)
         {
            this.EntryCompleteEvent(this);
         }
      }

      virtual protected bool ValidateText(char keyChar)
      {
         bool valid = true;

         if (!char.IsControl(keyChar) && !char.IsDigit(keyChar))
         {
            valid = false;
         }

         return valid;
      }
   }
}

