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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabletApp.Utils
{
   /// <summary>
   /// Mapping to allow the specification of a timezone via an integer id.
   /// </summary>
   public static class TimeZoneMapping
   {
      /// <summary>
      /// Dictionary containing an id and the official timezone name as used by Windows.
      /// </summary>
      public static Dictionary<int, string> TimeZoneMap = new Dictionary<int, string>()
      {
         { 1, "Dateline Standard Time" },
         { 2, "UTC-11" },
         { 3, "Hawaiian Standard Time" },
         { 4, "Alaskan Standard Time" },
         { 5, "Pacific Standard Time (Mexico)" },
         { 6, "Pacific Standard Time" },
         { 7, "US Mountain Standard Time" },
         { 8, "Mountain Standard Time (Mexico)" },
         { 9, "Mountain Standard Time" },
         { 10, "Central America Standard Time" },
         { 11, "Central Standard Time" },
         { 12, "Central Standard Time (Mexico)" },
         { 13, "Canada Central Standard Time" },
         { 14, "SA Pacific Standard Time" },
         { 15, "Eastern Standard Time" },
         { 16, "US Eastern Standard Time" },
         { 17, "Venezuela Standard Time" },
         { 18, "Paraguay Standard Time" },
         { 19, "Atlantic Standard Time" },
         { 20, "Central Brazilian Standard Time" },
         { 21, "SA Western Standard Time" },
         { 22, "Pacific SA Standard Time" },
         { 23, "Newfoundland Standard Time" },
         { 24, "E. South America Standard Time" },
         { 25, "Argentina Standard Time" },
         { 26, "SA Eastern Standard Time" },
         { 27, "Greenland Standard Time" },
         { 28, "Montevideo Standard Time" },
         { 29, "Bahia Standard Time" },
         { 30, "UTC-02" },
         { 31, "Mid-Atlantic Standard Time" },
         { 32, "Azores Standard Time" },
         { 33, "Cape Verde Standard Time" },
         { 34, "Morocco Standard Time" },
         { 35, "UTC" },
         { 36, "GMT Standard Time" },
         { 37, "Greenwich Standard Time" },
         { 38, "W. Europe Standard Time" },
         { 39, "Central Europe Standard Time" },
         { 40, "Romance Standard Time" },
         { 41, "Central European Standard Time" },
         { 42, "W. Central Africa Standard Time" },
         { 43, "Namibia Standard Time" },
         { 44, "Jordan Standard Time" },
         { 45, "GTB Standard Time" },
         { 46, "Middle East Standard Time" },
         { 47, "Egypt Standard Time" },
         { 48, "Syria Standard Time" },
         { 49, "E. Europe Standard Time" },
         { 50, "South Africa Standard Time" },
         { 51, "FLE Standard Time" },
         { 52, "Turkey Standard Time" },
         { 53, "Israel Standard Time" },
         { 54, "Kaliningrad Standard Time" },
         { 55, "Libya Standard Time" },
         { 56, "Arabic Standard Time" },
         { 57, "Arab Standard Time" },
         { 58, "Belarus Standard Time" },
         { 59, "Russian Standard Time" },
         { 60, "E. Africa Standard Time" },
         { 61, "Iran Standard Time" },
         { 62, "Arabian Standard Time" },
         { 63, "Azerbaijan Standard Time" },
         { 64, "Russia Time Zone 3" },
         { 65, "Mauritius Standard Time" },
         { 66, "Georgian Standard Time" },
         { 67, "Caucasus Standard Time" },
         { 68, "Afghanistan Standard Time" },
         { 69, "West Asia Standard Time" },
         { 70, "Ekaterinburg Standard Time" },
         { 71, "Pakistan Standard Time" },
         { 72, "India Standard Time" },
         { 73, "Sri Lanka Standard Time" },
         { 74, "Nepal Standard Time" },
         { 75, "Central Asia Standard Time" },
         { 76, "Bangladesh Standard Time" },
         { 77, "N. Central Asia Standard Time" },
         { 78, "Myanmar Standard Time" },
         { 79, "SE Asia Standard Time" },
         { 80, "North Asia Standard Time" },
         { 81, "China Standard Time" },
         { 82, "North Asia East Standard Time" },
         { 83, "Singapore Standard Time" },
         { 84, "W. Australia Standard Time" },
         { 85, "Taipei Standard Time" },
         { 86, "Ulaanbaatar Standard Time" },
         { 87, "Tokyo Standard Time" },
         { 88, "Korea Standard Time" },
         { 89, "Yakutsk Standard Time" },
         { 90, "Cen. Australia Standard Time" },
         { 91, "AUS Central Standard Time" },
         { 92, "E. Australia Standard Time" },
         { 93, "AUS Eastern Standard Time" },
         { 94, "West Pacific Standard Time" },
         { 95, "Tasmania Standard Time" },
         { 96, "Magadan Standard Time" },
         { 97, "Vladivostok Standard Time" },
         { 98, "Russia Time Zone 10" },
         { 99, "Central Pacific Standard Time" },
         { 100, "Russia Time Zone 11" },
         { 101, "New Zealand Standard Time" },
         { 102, "UTC+12" },
         { 103, "Fiji Standard Time" },
         { 104, "Kamchatka Standard Time" },
         { 105, "Tonga Standard Time" },
         { 106, "Samoa Standard Time" },
         { 107, "Line Islands Standard Time" },
      };

      /// <summary>
      /// Get the TimeZoneInfo for the given integer id.
      /// The integer id represents the keys in our TimeZoneMap.
      /// </summary>
      /// <param name="timeZoneId"></param>
      /// <returns></returns>
      public static TimeZoneInfo TimeZoneForId(int timeZoneId)
      {
         string stringId;

         if (TimeZoneMap.ContainsKey(timeZoneId))
         {
            stringId = TimeZoneMap[timeZoneId];
         }
         else
         {
            stringId = TimeZoneMap[35];
         }

         TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(stringId);

         return timeZone;
      }
   }
}
