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
using Model;
using System.IO;
using System.Security;
using System.Windows.Forms;
using System.Drawing;
using TabletApp.Views;

namespace TabletApp.Utils
{
   /// <summary>
   /// Extension methods for various model objects
   /// </summary>
   public static class AExtensions
   {
      /// <summary>
      /// Get a Windows-safe filename, replacing invalid characters.
      /// </summary>
      /// <param name="filename">A filename</param>
      /// <returns>The safe version of the filename</returns>
      static public string SafeFilename(this string filename)
      {
         string safeFilename = filename;

         foreach (var c in Path.GetInvalidFileNameChars()) 
         {
            safeFilename = safeFilename.Replace(c, '-'); 
         }

         return safeFilename;
      }

      static public List<string> FilenameList(this List<string> filePaths)
      {
         return filePaths.Select(c => { c = Path.GetFileName(c); return c; }).ToList();
      }

      // following taken from http://weblogs.asp.net/jongalloway//encrypting-passwords-in-a-net-app-config-file

      static byte[] entropy = System.Text.Encoding.Unicode.GetBytes("Salt Is Not A Password");

      static public string EncryptString(this System.Security.SecureString input)
      {
         byte[] encryptedData = System.Security.Cryptography.ProtectedData.Protect(
             System.Text.Encoding.Unicode.GetBytes(input.ToInsecureString()),
             entropy,
             System.Security.Cryptography.DataProtectionScope.CurrentUser);
         return Convert.ToBase64String(encryptedData);
      }

      public static SecureString DecryptString(this string encryptedData)
      {
         try
         {
            byte[] decryptedData = System.Security.Cryptography.ProtectedData.Unprotect(
                Convert.FromBase64String(encryptedData),
                entropy,
                System.Security.Cryptography.DataProtectionScope.CurrentUser);
            return ToSecureString(System.Text.Encoding.Unicode.GetString(decryptedData));
         }
         catch
         {
            return new SecureString();
         }
      }

      public static SecureString ToSecureString(this string input)
      {
         SecureString secure = new SecureString();
         foreach (char c in input)
         {
            secure.AppendChar(c);
         }
         secure.MakeReadOnly();
         return secure;
      }

      public static string ToInsecureString(this SecureString input)
      {
         string returnValue = string.Empty;
         IntPtr ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(input);
         try
         {
            returnValue = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(ptr);
         }
         finally
         {
            System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(ptr);
         }
         return returnValue;
      }

      public static string GetRootFolder(this string path)
      {
         while (true)
         {
            string temp = Path.GetDirectoryName(path);
            if (String.IsNullOrEmpty(temp))
            {
               break;
            }
            path = temp;
         }
         return path;
      }

      public static string FullyQualifiedUri(this string s)
      {
         try
         {
            return new UriBuilder(s).Uri.AbsoluteUri;
         }
         catch (Exception)
         {
            return "";
         }
      }

      public static string Truncate(this string value, int maxLength)
      {
         if (string.IsNullOrEmpty(value))
         {
            return value;
         }
         return value.Length <= maxLength ? value : value.Substring(0, maxLength);
      }

      #region Control extensions

      public static void ScaleControl(this Control control, float scale)
      {
         float widthRatio = scale;
         float heightRatio = scale;
         SizeF scaleSize = new SizeF(widthRatio, heightRatio);
         control.Scale(scaleSize);
      }

      public static void ScaleFont(this Control control, float scale)
      {
         if (control is ContainerControl)
         {
            ((ContainerControl)control).AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
         }
         if (!(control is TextBox) && !(control is Button) && !(control is Label) &&
                !(control is RadioButton) && !(control is ComboBox) && !(control is CheckBox) &&
                !(control is SpinnerProgress) && !(control is TreeView) && !(control is ListBox) &&
                !(control is TabControl) && !(control is GroupBox))
         {
            return;
         }
         Font font = control.Font;
         control.Font = new Font(font.FontFamily, control.Font.SizeInPoints * scale, font.Style);
      }

      public static void ScaleFontDeep(this Control topControl, float scale)
      {
         topControl.ScaleFont(scale);
         foreach (Control control in topControl.Controls)
         {
            control.ScaleFontDeep(scale);
         }
      }

      #endregion Control extensions
   }
}

