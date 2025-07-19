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
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabletApp.Utils
{
   /// <summary>
   /// Class for asynchronously checking the current user's admin status.
   /// </summary>
   public class AAdminChecker : ASingleton<AAdminChecker>
   {
      private bool fChecked = false;
      private bool fIsAdmin = false;
      private Task fCheckerTask;
      public Task CheckerTask
      {
         get { return fCheckerTask; }
      }

      /// <summary>
      /// Kick off the check in a separate thread.
      /// </summary>
      /// <returns></returns>
      public Task StartChecking()
      {
         fCheckerTask = Task.Run(() => this.IsCurrentUserAdmin());
         return fCheckerTask;
      }

      /// <summary>
      /// Check if the current user has admin rights.
      /// </summary>
      /// <returns>True of admin</returns>
      public void IsCurrentUserAdmin()
      {
         if (!fChecked)
         {
            using (PrincipalContext pc = new PrincipalContext(ContextType.Machine, null))
            {
               UserPrincipal up = UserPrincipal.Current;
               GroupPrincipal gp = GroupPrincipal.FindByIdentity(pc, "S-1-5-32-544");     // well-known admin security identifier
               // is the user a member of the admin group?
               fIsAdmin = up.IsMemberOf(gp);
               fChecked = true;
            }
         }
      }

      public bool IsComplete()
      {
         return fChecked;
      }

      public bool IsAdmin()
      {
         return fIsAdmin;
      }
   }
}
