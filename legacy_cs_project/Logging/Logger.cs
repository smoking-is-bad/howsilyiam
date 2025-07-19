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
using System.Diagnostics;
using DsiExtensions;

namespace Logging
{
   // From http://www.daveoncsharp.com/2009/09/create-a-logger-using-the-trace-listener-in-csharp/

   /// <summary>
   /// Class to manage formatting trace entries. 
   /// </summary>
   public static class ALog
   {
      private static TraceSource sTraceSource;

      public static void Initialize(string sourceName, string logPath)
      {
         sTraceSource = new TraceSource(sourceName);
         sTraceSource.Switch = new SourceSwitch(sourceName);
         sTraceSource.Listeners.Remove("Default");
         TextWriterTraceListener textListener = new TextWriterTraceListener(logPath);
         sTraceSource.Listeners.Add(textListener);
         sTraceSource.Switch.Level = SourceLevels.All;
      }

      public static void Error(string module, string message, params object[] args)
      {
         WriteEntry(TraceEventType.Error, module, message, args);
      }

      public static void Error(string module, Exception ex)
      {
         WriteEntry(TraceEventType.Critical, module, ex.AggregateMessage());
      }

      public static void Warning(string module, string message, params object[] args)
      {
         WriteEntry(TraceEventType.Warning, module, message, args);
      }

      public static void Info(string module, string message, params object[] args)
      {
         WriteEntry(TraceEventType.Information, module, message, args);
      }

      private static void WriteEntry(TraceEventType type, string module, string message, params object[] args)
      {
         if (TraceEventType.Critical == type)
         {
            sTraceSource.Listeners[0].TraceOutputOptions |= TraceOptions.Callstack;
         }
         else
         {
            sTraceSource.Listeners[0].TraceOutputOptions &= ~TraceOptions.Callstack;
         }
         sTraceSource.TraceEvent(type, 1, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Module: " + module + " - " + message, args);
         sTraceSource.Flush();
      }
   }
}

