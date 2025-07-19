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
using System.IO.Ports;

using DsiApi;
using Logging;
using TabletApp.Utils;


namespace TabletApp.Api.Network
{
   class ADataLoggerChecker
   {
      /// <param name="endLoggingOnHeartbeat">If true, sends acknowledgement message when heartbeat is received.</param>
      public ADataLoggerChecker(bool endLoggingOnHeartbeat)
      {
         fSendAck = endLoggingOnHeartbeat;
         this.TimeoutMs = 150 * 1000;
      }

      public int TimeoutMs { get; set; }

      public void Reset()
      {
         fCancel = false;
      }

      /// <summary>
      /// Listen for the heartbeat from the DSI, timing out eventually if none is received.
      /// If fSendAck is true, sends the acknowledgement message when heartbeat is received.
      /// </summary>
      /// <returns>true if heartbeat is received, false if canceled or timed out</returns>
      public bool Listen()
      {
         var startTime = DateTime.Now;
         var timeout = new TimeSpan(0, 0, 0, 0, this.TimeoutMs);
         bool received = false;

         var comPort = APorts.Instance().Open(Properties.Settings.Default.ComPort,
                                              Properties.Settings.Default.ComBaudRate,
                                              Parity.Even, 8, StopBits.One);

         ALog.Info("DsiNetwork", "Waiting for data logger heartbeat message, timing out at {0}", startTime + timeout);
         while (!fCancel)
         {
            try
            {
               if ('D' == comPort.ReadByte())
               {
                  if ('L' == comPort.ReadByte())
                  {
                     if (fSendAck)
                     {
                        this.SendAck(comPort);
                     }
                     received = true;
                     ALog.Info("DsiNetwork", "Received data logger heartbeat message.");
                     break;
                  }
               }
            }
            catch (TimeoutException)
            {
               if (timeout < (DateTime.Now - startTime))
               {
                  var msg = ApiResources.ErrorFailedToReadLoggerHeartbeat;
                  ALog.Error("DsiNetwork", msg);
                  AOutput.DisplayError(msg, forceShow: true, logMessage: false);
                  break;
               }
            }
            catch (Exception ex)
            {
               ALog.Info("DsiNetwork", "Unexpected exception while waiting for data logger heartbeat message: "
                + ex.Message);
               if (AOutput.DisplayYesNo("Unexpected exception - " + ex.Message, "Try again?") != System.Windows.Forms.DialogResult.Yes)
               {
                  break;
               }
            }
         }

         return received && !fCancel;
      }

      private void SendAck(SerialPort com)
      {
         com.WriteLine("OK\r\n");
      }

      public void Cancel()
      {
         fCancel = true;
      }

      public bool Canceled()
      {
         return fCancel;
      }

      private bool fSendAck = false;
      private bool fCancel = false;
   }
}
