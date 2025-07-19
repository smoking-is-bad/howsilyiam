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
using System.Windows.Forms;

namespace TabletApp.Views
{
   /// <summary>
   /// Key button enums - these correspond to the Tag of the 
   /// buttons (see <code>key_Click</code>)
   /// </summary>
   public enum NumberKey
   {
      Key0 = 0,
      Key1 = 1,
      Key2 = 2,
      Key3 = 3,
      Key4 = 4,
      Key5 = 5,
      Key6 = 6,
      Key7 = 7,
      Key8 = 8,
      Key9 = 9,
      KeyPoint = 10,
      KeyNeg = 11,
      KeyDel = 12,
      KeyNext = 13,
      KeyPrev = 14,
      KeyEnter = 15,
   }
   /// <summary>
   /// Delegate for receiving "key" hits on the number pad
   /// </summary>
   /// <param name="buttonIndex"></param>
   public delegate void KeyHit(NumberKey key);

   /// <summary>
   /// User control for a number pad control.
   /// </summary>
   public partial class NumberPad : UserControl
   {
      public event KeyHit KeyHitEvent;

      public NumberPad()
      {
         InitializeComponent();
      }

      /// <summary>
      /// Is the given control a direct descendent of this.
      /// </summary>
      /// <param name="control"></param>
      /// <returns></returns>
      public bool IsChild(Control control)
      {
         return this.Controls.Contains(control);
      }

      /// <summary>
      /// Allow/disallow the decimal point button (show/hide).
      /// </summary>
      /// <param name="allow"></param>
      public void AllowDecimal(bool allow)
      {
         decimalButton.Visible = allow;
      }

      /// <summary>
      /// Allow/disallow the negative button (show/hide).
      /// </summary>
      /// <param name="allow"></param>
      public void AllowNegative(bool allow)
      {
         negativeButton.Visible = allow;
      }

      /// <summary>
      /// Propagate key button clicks to any listeners.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void key_Click(object sender, EventArgs e)
      {
         if (null != this.KeyHitEvent)
         {
            this.KeyHitEvent((NumberKey)Convert.ToInt16(((Button)sender).Tag));
         }
      }
   }
}

