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

//#define qDesktopDebug      // debug scaling on the desktop

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace TabletApp.Utils
{

   /// <summary>
   /// Utility class for handling form and control scaling within the app.
   /// </summary>
   public static class AScaleUtils
   {
      public static float ScaleFactor { get; set; }

      public static bool NeedsScale { get; set; }

      private const int kMinWidth = 1080;
#if qDesktopDebug
      private const int kTargetHeight = 1518;
#else
      private const int kTargetHeight = 1920;
#endif
      
      /// <summary>
      /// Apply a scale to the main form if constraints are not met.  Currently, this means
      /// that if the main form width is less than 1080 in portrait (height in landscape)
      /// then all controls and fonts within the app will be scaled by a factor of
      /// actualwidth/minwidth.
      /// </summary>
      /// <param name="form"></param>
      /// <returns></returns>
      public static bool ScaleMainForm(Form form)
      {
         bool scaled = false;
         int screenWidth = Screen.PrimaryScreen.Bounds.Width;
         int screenHeight = Screen.PrimaryScreen.Bounds.Height;
#if qDesktopDebug
         screenWidth = 768;
         screenHeight = 1080;
#endif
         bool portrait = screenWidth <= screenHeight;
         int actualWidth = (portrait ? screenWidth : screenHeight);
         int actualHeight = (portrait ? screenHeight : screenWidth);
         AScaleUtils.NeedsScale = actualWidth < kMinWidth;
         if (AScaleUtils.NeedsScale)
         {
            AScaleUtils.ScaleFactor = (float)actualWidth / (float)kMinWidth;
            form.Height = Screen.PrimaryScreen.WorkingArea.Height;
#if qDesktopDebug
            form.Width = 768;
#else
            form.Width = Screen.PrimaryScreen.WorkingArea.Width;
#endif
            form.ScaleControl(AScaleUtils.ScaleFactor);
            form.ScaleFontDeep(AScaleUtils.ScaleFactor);
            form.Height = Screen.PrimaryScreen.WorkingArea.Height;
#if qDesktopDebug
            form.Width = 768;
#else
            form.Width = Screen.PrimaryScreen.WorkingArea.Width;
#endif
            scaled = true;
         }
         else
         {
            AScaleUtils.ScaleFactor = 1.0f;
         }

         return scaled;
      }

      /// <summary>
      /// Scale the given control if the app is currently scaling.  Scales both the
      /// control bounds and font.
      /// </summary>
      /// <param name="control"></param>
      public static void ScaleControl(Control control)
      {
         if (AScaleUtils.NeedsScale)
         {
            control.ScaleControl(AScaleUtils.ScaleFactor);
            control.ScaleFontDeep(AScaleUtils.ScaleFactor);
         }
      }
   }
}
