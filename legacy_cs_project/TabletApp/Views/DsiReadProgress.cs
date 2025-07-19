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
   public struct ReadProgressInfo
   {
      public int NumDsis { get; set; }
      public int CurrDsi { get; set; }
      public int NumProbes { get; set; }
      public int CurrProbe { get; set; }
      public bool Waiting { get; set; }
   }

   /// <summary>
   /// User control for the DSI progress
   /// </summary>
   public partial class DsiReadProgress : UserControl
   {
      /// <summary>
      /// Constructor
      /// </summary>
      public DsiReadProgress()
      {
         InitializeComponent();
         this.probeProgressBar.Minimum = 0;
         this.probeProgressBar.Maximum = 100;
      }

      /// <summary>
      /// Hack to displose of the progress bar to allow the enclosing form to dispose properly
      /// during GC.
      /// </summary>
      public void CleanUp()
      {
         this.probeProgressBar.Dispose();
      }

      /// <summary>
      /// Update our progress according to the given progress info.
      /// </summary>
      /// <param name="info"></param>
      public void UpdateProgress(ReadProgressInfo info)
      {
         if (info.Waiting)
         {
            this.dsiProgress.Text = Resources.ReadProgressWaiting;
         }
         else
         {
            this.dsiProgress.Text = String.Format(Resources.ReadProgressDsiString, info.CurrDsi + 1, info.NumDsis);
         }
         if (info.NumProbes > 0)
         {
            this.probeProgressBar.Value = Math.Min((int)(((float)info.CurrProbe / (float)info.NumProbes) * 100.0), 100);
         }
      }
   }
}

