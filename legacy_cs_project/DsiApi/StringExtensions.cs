// Copyright (c) 2015-2019 Sensor Networks, Inc.
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
using System.Text.RegularExpressions;

namespace DsiExtensions
{
   /// <summary>
   /// Extension to string to convert a string to a Uri, using https if Settings.ForceSsl is true
   /// and the Url hostname is not localhost.
   /// </summary>
   public static class AStringExtensions
   {
      public static Uri AsUriForcingSecureHttp(this string baseUrl)
      {
         var isLocalhost = Regex.IsMatch(baseUrl.ToLower(), @"http(s)?://localhost");
         if (TabletApp.Properties.Settings.Default.ForceSsl && !isLocalhost)
         {
            baseUrl = Regex.Replace(baseUrl, "http://", "https://", RegexOptions.IgnoreCase);
         }
         return new Uri(baseUrl);
      }
   }
}
