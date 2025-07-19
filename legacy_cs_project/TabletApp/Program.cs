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

using Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabletApp.Autofill;
using TabletApp.Persist;
using TabletApp.Properties;
using TabletApp.State;
using TabletApp.Utils;

namespace TabletApp
{
   static class Program
   {
      /// <summary>
      /// The main entry point for the application.
      /// </summary>
      [STAThread]
      static void Main(string[] args)
      {
         if (3 == args.Length && args[0].Equals("-s"))
         {
            string setting = args[1];
            int value = int.Parse(args[2]);
            AUtilities.SetSettingValue(setting, value);
         }
         else if (args.Length > 0 && args[0].Equals("-multiport"))
         {
            // Start in multi-port mode
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(GlobalExceptionHandler);

            ALog.Initialize(Resources.AppName, AUtilities.GetLogFilePath());
            AUtilities.PreventSleep();
            AUtilities.SetLanguageFromSettings();
            AUtilities.SaveCsvSettings();
            AAdminChecker.Instance.StartChecking();
            AStateController.Instance.Initialize();
            AFileManager.Instance.Initialize();
            AAutofillManager.Instance.Initialize();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Start with multi-port scan state
            AStateController.Instance.GlobalData["multiportmode"] = true;
            Application.Run(new WizardForm("multiportscan"));
         }
         else
         {
            // already running - just bring the current instance to front
            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
            {
               AUtilities.BringProccessToFront(Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Single(p => p.Id != Process.GetCurrentProcess().Id));
               return;
            }

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(GlobalExceptionHandler);

            ALog.Initialize(Resources.AppName, AUtilities.GetLogFilePath());
            AUtilities.PreventSleep();
            AUtilities.SetLanguageFromSettings();
            AUtilities.SaveCsvSettings();
            AAdminChecker.Instance.StartChecking();
            AStateController.Instance.Initialize();
            AFileManager.Instance.Initialize();
            AAutofillManager.Instance.Initialize();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new WizardForm("home"));
         }
      }

      private static void GlobalExceptionHandler(object sender, UnhandledExceptionEventArgs args)
      {
         Exception e = (Exception)args.ExceptionObject;
         AOutput.DisplayError(Resources.ErrorUnhandledException + " " + e.Message + e.StackTrace);
      }
   }
}


