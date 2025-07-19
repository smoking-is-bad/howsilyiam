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
using System.Windows.Forms;
using TabletApp.Properties;

using Logging;
using TabletApp.State;

namespace TabletApp.Utils
{
   /// <summary>
   /// Generic output utilities
   /// </summary>
   public sealed class AOutput
   {
      public static bool SuppressErrorDialog { get; set; }

      /// <summary>
      /// Display a string if in debug mode
      /// </summary>
      /// <param name="aString"></param>
      static public void DebugString(String aString)
      {
#if DEBUG
         Console.WriteLine(aString);
#endif
      }

      /// <summary>
      /// Error/message logging routines
      /// </summary>

      static public void LogException(Exception exception)
      {
         ALog.Error("App", exception);
      }

      static public void LogException(Exception exception, String message)
      {
         ALog.Error("App", "{0}\r\n{1}", message, exception.ToString());
      }

      static public void LogMessage(String message)
      {
         ALog.Info("App", message);
      }


      static private MessageBoxOptions kTopMost = (MessageBoxOptions)0x00040000L;

      /// <summary>
      /// Display the given error string
      /// </summary>
      /// <param name="errorString">Error string to display</param>
      /// <param name="forceShow">Set to true to ignore the current suppress setting</param>
      static public void DisplayError(String errorString, bool forceShow = false, bool logMessage = true)
      {
         if (logMessage)
         {
            AOutput.LogMessage(errorString);
         }

         if (AOutput.SuppressErrorDialog && !forceShow)
         {
            AStateController.Instance.PostPerformActionEvent("showerror", null);
         }
         else
         {
            MessageBox.Show(errorString, Resources.ErrorCaption, MessageBoxButtons.OK,
               MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, kTopMost);
         }
      }

      /// <summary>
      /// Display the given message string in a dialog
      /// </summary>
      /// <param name="messageString">Message string to display</param>
      static public void DisplayMessage(String messageString)
      {
         MessageBox.Show(messageString, Resources.GeneralMessageCaption, MessageBoxButtons.OK,
            MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, kTopMost);
      }

      /// <summary>
      /// Display a yes/no dialog box with the given message and caption
      /// </summary>
      /// <param name="message">Message to display</param>
      /// <param name="caption">Caption</param>
      /// <returns>DialogResult</returns>
      static public DialogResult DisplayYesNo(String message, String caption)
      {
         return MessageBox.Show(message, caption, MessageBoxButtons.YesNo,
            MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, kTopMost);
      }

      /// <summary>
      /// Display a dialog with text input and the given prompt.  Return the text entered.
      /// </summary>
      /// <param name="prompt"></param>
      /// <returns>Entered text</returns>
      static public string DisplayTextInput(string prompt, bool textIsPassword = false)
      {
         TextInputDialog dialog = new TextInputDialog();
         string result = null;

         dialog.Text = Resources.GeneralMessageCaption;
         dialog.Prompt.Text = prompt;
         dialog.TextBox.UseSystemPasswordChar = textIsPassword;
         dialog.TopMost = true;
         if (dialog.ShowDialog() == DialogResult.OK)
         {
            result = dialog.TextBox.Text;
         }
         dialog.Dispose();

         return result;
      }
   }
}

