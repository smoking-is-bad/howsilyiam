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
   /// User control for showing a progress spinner with associated progress text.
   /// </summary>
   public partial class SpinnerProgress : UserControl
   {
      /// <summary>
      /// Get/set the progress text
      /// </summary>
      public string ProgressText 
      { 
         get
         {
            return this.progressText.Text;
         }
         set
         {
            this.progressText.Text = value;
         }
      }

      /// <summary>
      /// Constructor
      /// </summary>
      public SpinnerProgress()
      {
         InitializeComponent();
      }

      /// <summary>
      /// Hack method to clear out the spinner image.  It seems that an animated gif
      /// prevents proper garbage collection of the enclosing user control, even though
      /// that user control has been released from its parent.  Clearing out the animated
      /// gif alleviates that problem and allows the main user control to be properly disposed.
      /// </summary>
      public void ClearSpinner()
      {
         this.spinner.Image = null;
      }

      /// <summary>
      /// Show/hide the spinner graphic.
      /// </summary>
      /// <param name="visible"></param>
      public void SetSpinnerVisible(bool visible)
      {
         this.spinner.Visible = visible;
      }

      /// <summary>
      /// Update the spinner graphic.
      /// </summary>
      public void UpdateSpinner()
      {
         this.spinner.Refresh();
      }
   }
}

