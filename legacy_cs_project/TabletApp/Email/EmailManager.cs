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
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Security;
using System.Threading.Tasks;
using TabletApp.Properties;
using TabletApp.Utils;

namespace TabletApp.Email
{
   /// <summary>
   /// Utility class for sending an email.
   /// </summary>
   class AEmailManager : ASingleton<AEmailManager>
   {
      /// <summary>
      /// Send an email message asynchronously with the given parameters.
      /// </summary>
      /// <param name="server"></param>
      /// <param name="username"></param>
      /// <param name="password"></param>
      /// <param name="from"></param>
      /// <param name="to"></param>
      /// <param name="subject"></param>
      /// <param name="body"></param>
      /// <param name="attachments"></param>
      /// <returns></returns>
      public async Task SendMessageAsync(string server, int port, string username, SecureString password, string from, string to, string subject, string body, List<string> attachments)
      {
         MailMessage message = new MailMessage(from, to, subject, body);

         foreach (string attachment in attachments)
         {
            // Create  the file attachment for this e-mail message.
            Attachment data = new Attachment(attachment, MediaTypeNames.Application.Octet);
            // Add time stamp information for the file.
            ContentDisposition disposition = data.ContentDisposition;
            disposition.CreationDate = System.IO.File.GetCreationTime(attachment);
            disposition.ModificationDate = System.IO.File.GetLastWriteTime(attachment);
            disposition.ReadDate = System.IO.File.GetLastAccessTime(attachment);
            // Add the file attachment to this e-mail message.
            message.Attachments.Add(data);
         }

         // Send the message.
         SmtpClient client = new SmtpClient(server, port);
         // Add credentials if the SMTP server requires them.
         client.EnableSsl = true;
         client.Timeout = 10000;
         client.DeliveryMethod = SmtpDeliveryMethod.Network;
         client.UseDefaultCredentials = false;
         //using (WindowsIdentity.GetCurrent().Impersonate())
         //{
         //   client.Credentials = CredentialCache.DefaultNetworkCredentials;
         //}
         client.Credentials = new NetworkCredential(username, password);

         try
         {
            const int timeout = 30000;
            Task task = Task.Run(() =>
               {
                  client.Send(message);
               });
            if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
            {
               // check for task exception
               if (task.IsFaulted)
               {
                  throw task.Exception;
               }
            }
            else
            {
               // task timed out
               throw new Exception(Resources.ErrorSendEmailTimeout);
            }
         }
         catch (Exception e)
         {
            // log our exception and rethrow
            AOutput.LogException(e);
            throw e;
         }
      }
   }
}

