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
using TabletApp.State;

namespace TabletApp.Content
{
   /// <summary>
   /// Base class for all content user controls.
   /// </summary>
   public class BaseContent : UserControl
   {
      /// <summary>
      /// Parameters passed in from the state definition file
      /// </summary>
      public Dictionary<string, string> fParams;

      /// <summary>
      /// Default constructor
      /// </summary>
      public BaseContent()
      {
      }

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="parameters">Parameters from the state def file</param>
      public BaseContent(Dictionary<string, string> parameters)
      {
         fParams = parameters;
      }

      /// <summary>
      /// Control was just shown.
      /// </summary>
      public virtual void DidShow()
      {
         // no op - subclasses override
      }

      /// <summary>
      /// Control is about to disappear - give an opportunity to clean up.
      /// </summary>
      public virtual void WillDisappear()
      {
         // no op - subclasses override
      }

      /// <summary>
      /// The state controller sets this based on the mode showing the content.
      /// Use it to determine specifics about content view configuration.
      /// Can be in {  "sensor", "logger", "file", "commissioning" }
      /// This list may not be exhaustive.
      /// </summary>
      protected string ReadMode
      {
         get
         {
            return AStateController.Instance.GlobalData.ContainsKey("readmode")
            ? (string)AStateController.Instance.GlobalData["readmode"]
            : null;
         }
      }
   }
}

