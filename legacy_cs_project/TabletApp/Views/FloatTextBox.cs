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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TabletApp.Views
{
   /// <summary>
   /// Text box for floating point numbers, no negative.
   /// </summary>
   public class FloatTextBox : NumberTextBox
   {
      public int NumPlaces { get; set; }

      public float FloatValue
      {
         set
         {
            value = Math.Min(value, this.MaxValue);
            value = Math.Max(value, this.MinValue);
            this.Text = value.ToString("F" + this.NumPlaces);
            fPrevValue = value;
         }
         get
         {
            float val;
            try
            {
               val = this.Text.Length > 0 ? (float)Convert.ToDouble(this.Text) : 0;
            }
            catch (Exception)
            {
               val = 0f;
            }
            return this.GetValidValue(val);
         }
      }

      public override float Value
      {
         set
         {
            this.FloatValue = value;
         }
         get
         {
            return this.FloatValue;
         }
      }

      public FloatTextBox()
      {
         this.NumPlaces = 2;
      }

      override protected bool ValidateText(char keyChar)
      {
         bool valid = base.ValidateText(keyChar);
         
         valid = valid || (keyChar == '.' && (!this.Text.Contains('.') || this.SelectedText.Contains('.')));

         return valid;
      }
   }
}

