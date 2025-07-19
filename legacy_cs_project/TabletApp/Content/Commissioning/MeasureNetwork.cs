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

using DsiApi;
using Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabletApp.Api;
using TabletApp.Properties;
using TabletApp.State;
using TabletApp.Utils;
using DsiExtensions;
using System.Windows.Forms;

namespace TabletApp.Content.Commissioning
{
   /// <summary>
   /// User control for the Measure Network? screen, prompting the user whether they
   /// wish to do a normal measurement on the newly commissioned network.
   /// </summary>
   public class MeasureNetwork : Message
   {
      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="parameters">Parameters from the state def file</param>
      public MeasureNetwork(Dictionary<string, string> parameters)
         : base(parameters)
      {
         AStateController.Instance.StateWillChangeEvent += HandleStateWillChangeEvent;
      }

      /// <summary>
      /// About to disappear - clean up.
      /// </summary>
      public override void WillDisappear()
      {
         AStateController.Instance.StateWillChangeEvent -= HandleStateWillChangeEvent;
      }

      /// <summary>
      /// Handle state change.  Read from the DSI and store in our master info.
      /// </summary>
      /// <param name="newState"></param>
      /// <param name="args"></param>
      private void HandleStateWillChangeEvent(AState newState, string actionName, CancelEventArgs args)
      {
         if ("readall" == actionName)
         {
            if (DialogResult.No == AOutput.DisplayYesNo(Resources.ConnectStringMessage, Resources.ConnectStringCaption))
            {
               args.Cancel = true;
            }
         }
      }
   }
}

