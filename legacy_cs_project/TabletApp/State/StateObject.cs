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
using System.Diagnostics;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using TabletApp.Properties;

namespace TabletApp.State
{
   /// <summary>
   /// Base data object for any state-related objects.
   /// </summary>
   public class AStateObject
   {
      private const string kGlobalPrefix = "global#";
      private const string kResourcePrefix = "resource#";

      /// <summary>
      /// Expand the given string by resolving any key values embedded in the string.
      /// </summary>
      /// <param name="aString"></param>
      /// <returns></returns>
      protected string ExpandString(string aString)
      {
         string expandedString = aString;

         if (aString.StartsWith(kGlobalPrefix))
         {
            Dictionary<string, object> globals = AStateController.Instance.GlobalData;
            string key = aString.Substring(kGlobalPrefix.Length);
            Debug.Assert(globals.ContainsKey(key));
            expandedString = globals[key] as string;
         }
         else if (aString.StartsWith(kResourcePrefix))
         {
           string key = aString.Substring(kResourcePrefix.Length);
           expandedString = Resources.ResourceManager.GetString(key, Resources.Culture);
         }

         return expandedString;
      }

      /// <summary>
      /// Get the expanded value for the property with the given name.  Expansions are
      /// keyed according to the prefixes defined above.
      /// </summary>
      /// <param name="propName"></param>
      /// <returns>The expanded string</returns>
      public string ExpandedValue(string propName)
      {
         string value = (string)GetType().GetProperty(propName).GetValue(this, null);
         return this.ExpandString(value);
      }
   }
}
