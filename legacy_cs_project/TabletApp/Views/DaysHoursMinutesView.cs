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

namespace TabletApp.Views
{
   /// <summary>
   /// User control for day/hour/minute text boxes (DD:HH:MM).
   /// </summary>
   public partial class DaysHoursMinutesView : UserControl
   {
      public string Days
      {
         get
         {
            return this.days.Text;
         }
         set
         {
            this.days.Text = value;
         }
      }

      public string Hours
      {
         get
         {
            return this.hours.Text;
         }
         set
         {
            this.hours.Text = value;
         }
      }

      public string Minutes
      {
         get
         {
            return this.minutes.Text;
         }
         set
         {
            this.minutes.Text = value;
         }
      }

      public int TotalMinutes
      {
         get
         {
            int days = Convert.ToInt32(this.days.Text);
            int hours = Convert.ToInt32(this.hours.Text);
            int minutes = Convert.ToInt32(this.minutes.Text);
            return days * 24 * 60 + hours * 60 + minutes;
         }
         set
         {
            int minutes = value % 60;
            int hours = (value / 60) % 24;
            int days = (value / (60 * 24));
            this.Minutes = Convert.ToString(minutes);
            this.Hours = Convert.ToString(hours);
            this.Days = Convert.ToString(days);
         }
      }

      public DaysHoursMinutesView()
      {
         InitializeComponent();
      }
   }
}

