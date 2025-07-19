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
   /// TrackBar with floating point values.
   /// </summary>
   public class FloatTrackBar : TrackBar
   {
      private float fPrecision = 0.01f;

      public float Precision
      {
         get { return fPrecision; }
         set
         {
            fPrecision = value;
         }
      }
      public new float LargeChange
      { get { return base.LargeChange * fPrecision; } set { base.LargeChange = (int)(value / fPrecision); } }
      public new float Maximum
      { get { return base.Maximum * fPrecision; } set { base.Maximum = (int)(value / fPrecision); } }
      public new float Minimum
      { get { return base.Minimum * fPrecision; } set { base.Minimum = (int)(value / fPrecision); } }
      public new float SmallChange
      { get { return base.SmallChange * fPrecision; } set { base.SmallChange = (int)(value / fPrecision); } }
      public new float TickFrequency
      { get { return base.TickFrequency * fPrecision; } set { base.TickFrequency = (int)(value / fPrecision); } }
      
      public new float Value
      {
         get { return base.Value * fPrecision; }
         set
         {
            if (value < this.Minimum)
            {
               value = this.Minimum;
            }
            if (value > this.Maximum)
            {
               value = this.Maximum;
            }
            base.Value = (int)(value / fPrecision);
         }
      }
   }
}

