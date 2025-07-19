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
using System.IO;
using TabletApp.Properties;

namespace TabletApp.Utils
{
   /// <summary>
   /// Manages the admin password for commissioning.
   /// </summary>
   public class APasswordManager : ASingleton<APasswordManager>
   {
      static private bool sValidPassword = false;

      /// <summary>
      /// Get the path for the application password file.
      /// </summary>
      /// <returns></returns>
      public string GetPasswordFilePath()
      {
         string passPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), Resources.RootDataFolderName);
         passPath = Path.Combine(passPath, Resources.PasswordFilename);
         return passPath;
      }

      /// <summary>
      /// Get the current password, either stored or via prompting from user.
      /// </summary>
      /// <returns>Current password or null</returns>
      public string GetCurrentPassword()
      {
         string currPwd = ReadPasswordFile();
         // if no current password, prompt for one
         if (null == currPwd || 0 == currPwd.Length)
         {
            string first = "first";
            string second = "second";
            while (first != second)
            {
               first = AOutput.DisplayTextInput(Resources.NewPasswordPrompt, true);
               if (null == first)
               {
                  break;
               }
               if (first.Length < 6)
               {
                  AOutput.DisplayError(Resources.ErrorInvalidNewPassword);
                  continue;
               }
               second = AOutput.DisplayTextInput(Resources.NewPasswordSecondPrompt, true);
               if (null == second)
               {
                  break;
               }
               if (first != second)
               {
                  AOutput.DisplayError(Resources.ErrorPasswordMatch);
               }
               else
               {
                  currPwd = second;
                  CreatePasswordFile(currPwd);
               }
            }
         }

         return currPwd;
      }

      /// <summary>
      /// Validate the given password by prompting for the password from the user.
      /// </summary>
      /// <param name="currPwd"></param>
      /// <returns>True if valid</returns>
      public bool ValidatePassword(string currPwd)
      {
         if (null == currPwd)
         {
            return false;
         }

         while (!sValidPassword)
         {
            string password = AOutput.DisplayTextInput(Resources.PasswordPrompt, true);

            if (null == password)
            {
               break;
            }

            if (currPwd.Equals(password))
            {
               sValidPassword = true;
            }
            else
            {
               AOutput.DisplayError(Resources.ErrorInvalidPassword);
            }
         }

         return sValidPassword;
      }

      /// <summary>
      /// Create our password file with the base64 version of the given password.
      /// </summary>
      /// <param name="pwd"></param>
      public void CreatePasswordFile(string pwd)
      {
         string path = GetPasswordFilePath();
         string encPwd = AUtilities.Base64Encode(pwd);
         File.WriteAllText(path, encPwd);
         File.SetAttributes(path, FileAttributes.Hidden);
      }

      /// <summary>
      /// Get the current password from the password file, if any.
      /// </summary>
      /// <returns></returns>
      public string ReadPasswordFile()
      {
         string pwd = null;
         string path = GetPasswordFilePath();

         try
         {
            string encPwd = File.ReadAllText(path);
            pwd = AUtilities.Base64Decode(encPwd);
         }
         catch (Exception)
         {
            // no-op
         }

         return pwd;
      }
   }
}
