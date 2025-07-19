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

namespace TabletApp.Utils
{
   /// <summary>
   /// Base class for any class wishing to implement itself as a singleton.
   /// </summary>
   /// <typeparam name="T">Type of singleton class</typeparam>
   public abstract class ASingleton<T>
   {
      // singleton vars
      private static T sThis = default(T);
      private static readonly object sSyncRoot = new Object();

      /// <summary>
      /// Thread-safe accessor of singleton
      /// </summary>
      public static T Instance
      {
         get
         {
            if (null == sThis)
            {
               lock (sSyncRoot)
               {
                  if (null == sThis)
                  {
                     sThis = Activator.CreateInstance<T>();
                  }
               }
            }

            return sThis;
         }
      }

      /// <summary>
      /// Protected constructor for singleton
      /// </summary>
      protected ASingleton()
      {
      }
   }
}

