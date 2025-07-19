// Copyright (c) 2017 Sensor Networks, Inc.
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
using System.Threading.Tasks;
using TabletApp.Api.Network;
using TabletApp.Properties;
using TabletApp.State;

namespace TabletApp.Content
{
   /// <summary>
   /// Unused? 
   /// Control for reading a data logger DSI.
   /// </summary>
   public partial class ReadLogger : Message
   {
      private ADataLoggerChecker fChecker;

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="parameters">Parameters from the state def file</param>
      public ReadLogger(Dictionary<string, string> parameters)
         : base(parameters)
      {
         this.MessageLabel.TextAlign = ContentAlignment.MiddleCenter;
         try
         {
            this.MessageText = Resources.MessageWaitLogger;
            this.DoPortRead();
         }
         catch (Exception)
         {
            this.MessageText = Resources.MessageErrorLoggerOpen;
         }
         AStateController.Instance.PerformActionEvent += HandlePerformAction;
      }

      /// <summary>
      /// About to disappear - clean up.
      /// </summary>
      public override void WillDisappear()
      {
         if (null != fChecker && !fChecker.Canceled())
         {
            fChecker.Cancel();
         }
         // clean up our event listening
         AStateController.Instance.PerformActionEvent -= HandlePerformAction;
      }

      /// <summary>
      /// Start the port read loop
      /// </summary>
      private void DoPortRead()
      {
         fChecker = new ADataLoggerChecker(endLoggingOnHeartbeat: true);
         AStateController.Instance.PostPerformActionEvent("showspinnerprogress", Resources.ProgressReadLogger);
         Task.Run(() => this.PortReadLoop());
      }

      /// <summary>
      /// Read until we get DL from our com port or until cancel
      /// </summary>
      private void PortReadLoop()
      {
         fChecker.Listen();
         if (!fChecker.Canceled())
         {
            AStateController.Instance.ChangeToState("scan");
         }
         fChecker = null;

         AStateController.Instance.PostPerformActionEvent("hidespinnerprogress", null);
      }

      /// <summary>
      /// Handle any actions.
      /// </summary>
      /// <param name="actionName"></param>
      /// <param name="data"></param>
      private void HandlePerformAction(string actionName, object data)
      {
         if ("cancel" == actionName && null != fChecker)
         {
            fChecker.Cancel();
         }
      }
   }
}

