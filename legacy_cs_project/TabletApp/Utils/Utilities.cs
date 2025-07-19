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
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Security.Claims;
using System.DirectoryServices.AccountManagement;
using System.Media;
using TabletApp.Properties;
using System.Globalization;
using System.Resources;
using System.Runtime.InteropServices;

namespace TabletApp.Utils
{
   public sealed class AUtilities
   {
      internal static class NativeMethods
      {
         [DllImport("USER32.DLL")]
         public static extern bool SetForegroundWindow(IntPtr hWnd);

         [DllImport("kernel32.dll")]
         public static extern uint SetThreadExecutionState(uint esFlags);
         public const uint ES_CONTINUOUS = 0x80000000;
         public const uint ES_SYSTEM_REQUIRED = 0x00000001;
         public const uint ES_DISPLAY_REQUIRED = 0x00000002;
      }

      /// <summary>
      /// Bring a process to front by making its main window frontmost.
      /// </summary>
      /// <param name="process"></param>
      public static void BringProccessToFront(Process process)
      {
         NativeMethods.SetForegroundWindow(process.MainWindowHandle);
      }
      
      /// <summary>
      /// Get the application version string from the assembly info.
      /// </summary>
      /// <returns>Version string</returns>
      static public string GetAppVersion()
      {
         Assembly assembly = Assembly.GetExecutingAssembly();
         FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
         string version = fileVersionInfo.ProductVersion;
         return version;
      }

      /// <summary>
      /// Get the path for a file with the given name that resides in the same directory are the running application.
      /// </summary>
      /// <param name="filename">Name of the file</param>
      /// <returns>Full path to that file</returns>
      public static String PathForAppSiblingFile(String filename)
      {
         String dir = System.Reflection.Assembly.GetExecutingAssembly().Location;
         String parent = Path.GetDirectoryName(dir);
         String siblingPath = Path.Combine(parent, filename);

         return siblingPath;
      }

      /// <summary>
      /// Show the keyboard.  HACK - looks for the keyboard exe and executes that.
      /// </summary>
      public static void ShowKeyboard()
      {
         string progFiles = @"C:\Program Files\Common Files\Microsoft Shared\ink";
         string keyboardPath = Path.Combine(progFiles, "TabTip.exe");

         Process.Start(keyboardPath);
      }

      /// <summary>
      /// Hide the keyboard.  HACK - looks for the keyboard exe and kills that.
      /// </summary>
      public static void HideKeyboard()
      {
         try
         {
            var processlist = Process.GetProcesses();

            foreach (var process in processlist.Where(process => process.ProcessName == "TabTip"))
            {
               process.Kill();
               break;
            }
         }
         catch (Exception)
         {
            // silent fail
         }
      }

      /// <summary>
      /// Play the sound at the given resource path (eg "TabletApp.Resources.mysound.wav").
      /// </summary>
      /// <param name="resourcePath"></param>
      public static void PlaySound(string resourcePath)
      {
         Assembly assembly = Assembly.GetExecutingAssembly();
         Stream stream = assembly.GetManifestResourceStream(resourcePath);
         SoundPlayer player = new SoundPlayer(stream);
         player.Play();
      }

      /// <summary>
      /// Get the path for the application log file.
      /// </summary>
      /// <returns></returns>
      public static string GetLogFilePath()
      {
         string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), Resources.RootDataFolderName);
         logPath = Path.Combine(logPath, Resources.LogFilename);
         return logPath;
      }

      /// <summary>
      /// Open the app's log file (assumed to be "log.txt" in C:\ProgramData\SensorNetworks) using
      /// the default viewer.
      /// </summary>
      public static void OpenLogFile()
      {
         System.Diagnostics.Process.Start(AUtilities.GetLogFilePath());
      }

      /// <summary>
      /// Get all of the languages supported by our app (according to our Resources localizations).
      /// </summary>
      /// <returns>List of CultureInfo</returns>
      public static List<CultureInfo> GetSupportedLanguages()
      {
         List<CultureInfo> languages = new List<CultureInfo>();
         ResourceManager rm = new ResourceManager(typeof(Resources));

         CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
         foreach (CultureInfo culture in cultures)
         {
            try
            {
               ResourceSet rs = rm.GetResourceSet(culture, true, false);
               // or ResourceSet rs = rm.GetResourceSet(new CultureInfo(culture.TwoLetterISOLanguageName), true, false);
               if (null != rs && CultureInfo.InvariantCulture.TwoLetterISOLanguageName != culture.TwoLetterISOLanguageName)
               {
                  languages.Add(culture);
               }
            }
            catch (CultureNotFoundException e)
            {
               AOutput.LogException(e, "Exception while getting supported languages");
            }
         }

         return languages;
      }

      /// <summary>
      /// Set the current language (culture) according to the language set in settings.
      /// </summary>
      public static void SetLanguageFromSettings()
      {
         List<CultureInfo> languages = AUtilities.GetSupportedLanguages();
         CultureInfo currCulture = languages.Find(l => l.TwoLetterISOLanguageName == Settings.Default.Language);
         if (null != currCulture)
         {
            currCulture.NumberFormat.NaNSymbol = ADataExtensions.kDefaultNumberString;
            CultureInfo.DefaultThreadCurrentUICulture = currCulture;
            CultureInfo.DefaultThreadCurrentCulture = currCulture;
         }
      }

      /// <summary>
      /// Prevent or allow sleep for the application.
      /// </summary>
      public static void PreventSleep()
      {
         NativeMethods.SetThreadExecutionState(NativeMethods.ES_CONTINUOUS | NativeMethods.ES_SYSTEM_REQUIRED | NativeMethods.ES_DISPLAY_REQUIRED);
      }

      public static void AllowSleep()
      {
         NativeMethods.SetThreadExecutionState(NativeMethods.ES_CONTINUOUS);
      }

      /// <summary>
      /// Force-save our CSV settings so the user can change them.
      /// </summary>
      public static void SaveCsvSettings()
      {
         Settings.Default.CsvIsMetric = Settings.Default.CsvIsMetric;
         Settings.Default.CsvTimezoneId = Settings.Default.CsvTimezoneId;
         Settings.Default.Save();
      }

      /// <summary>
      /// Set a named setting value in our Settings.
      /// </summary>
      /// <param name="settingName"></param>
      /// <param name="value"></param>
      public static void SetSettingValue(string settingName, object value)
      {
         switch (settingName)
         {
            case nameof(Settings.Default.LowLevelRetryCount):
               Settings.Default.LowLevelRetryCount = (int)value;
               break;
            case nameof(Settings.Default.HighLevelRetryCount):
               Settings.Default.HighLevelRetryCount = (int)value;
               break;
            case nameof(Settings.Default.LowLevelTimeout):
               Settings.Default.LowLevelTimeout = (int)value;
               break;
         }
         Settings.Default.Save();
      }

      /// <summary>
      /// Base64 encode given string.
      /// </summary>
      /// <param name="plainText"></param>
      /// <returns></returns>
      public static string Base64Encode(string plainText)
      {
         var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
         return Convert.ToBase64String(plainTextBytes);
      }

      /// <summary>
      /// Base64 decode given string.
      /// </summary>
      /// <param name="base64EncodedData"></param>
      /// <returns></returns>
      public static string Base64Decode(string base64EncodedData)
      {
         var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
         return Encoding.UTF8.GetString(base64EncodedBytes);
      }
   }
}

