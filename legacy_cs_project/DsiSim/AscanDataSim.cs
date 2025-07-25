﻿// Copyright (c) 2015 Sensor Networks, Inc.
// 
// All rights reserved. No part of this publication may be reproduced,
// distributed, or transmitted in any form or by any means, including
// photocopying, recording, or other electronic or mechanical methods, without
// the prior written permission of Sensor Networks, Inc., except in the case of
// brief quotations embodied in critical reviews and certain other noncommercial
// uses permitted by copyright law.
// 
// 
using Model;
using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace MockData
{
   public class AAscan
   {
      /// <summary>Prefer PresetData(). This could be tuned to be useful in the future.</summary>
      /// <param name="quantity">The number of registers of ascan data to create (1/2 the number of bytes)</param>
      /// <returns>Sparse data simulating an ascan section with a ping about half the time this is called.</returns>
      public static Int16[] GeneratedMockData(UInt16 quantity)
      {
         Random rng = new Random();

         // We'll simulate the data by generating sparse data at a random position.
         var ascanData = new Int16[quantity];

         // There are 17 sections of ascan data and we want about 2 pings, so we'll increase the
         // chances of it happening.
         if (rng.NextDouble() < (8.0 / 17.0))
         {
            // The first ping in Jim's data
            Int16[] ping =
            {  
               -0, -1, -3, -7, 0, 52, 181, 291, -0, -1216, -3156, -3814, 0, 8988, 17531,
               15918, -0, -21183, -31046, -21183, -0, 15918, 17531, 8988, 0, -3814, -3156,
               -1216, -0, 291, 181, 52, 0, -7, -3, -1
            };

            int offset = (int)(rng.NextDouble() * quantity);
            offset = Math.Min(offset, quantity - ping.Length);

            Buffer.BlockCopy(ping, 0, ascanData, offset, ping.Length);
         }

         return ascanData;
      }

      public static Int16[] PresetData()
      {
         Int16[] data = FileData();

         if (null == data)
         {
            data = RecordedMockData();
            //data =  JimMockData();
            //data = new Int16[2048]; // flat line.
         }

         return data;
         
      }

      public static Int16[] FileData()
      {
         Int16[] data = null;
         ANanoSense nano = null;
         string file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"SensorNetworks\ascan.xml");

         if (File.Exists(file))
         {
            using (var xmlStream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
               var serializer = new XmlSerializer(typeof(ANanoSense));

               nano = (ANanoSense)serializer.Deserialize(xmlStream);
            }
            data = nano.Dsi.probes[0].setups[0].ascanData;
         }

         return data;
      }

      // Expected thickness is 0.0225125 based on Jim's sample code output using mode
      // kDelayLineToFirstWall
      // Crossings at 8263.5 and 10663.5
      public static Int16[] JimMockData()
      {
         Int16[] data = { 0, 0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, -0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, -0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, -0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, -0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0,
            -0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -0, 0,
            0, 0, -0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, -0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            -0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, -0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -0, -0, 0, 0, 0, 0, -0, -0, -0, -0, 0, 0, 0, 0, -0, -0, -0, -0, -0, 0, 0, 0,
            -0, -0, -0, -0, 0, 0, 0, 0, -0, -0, -0, -0, -0, 0, 0, 0, -0, -0, -0, -0, 0, 0, 0, 0, -0, -0, -0, -0, -0, 0, 0, 0, -0, -0, -0, -0, 0, 0, 0, 0, -0, -0, -0, -0, -0, 0, 0, 0,
            -0, -0, -0, -0, -0, 0, 0, 0, -0, -0, -0, -0, -0, 0, 0, 0, 0, -0, -0, -0, -0, 0, 0, 0, 0, -0, -0, -0, -0, 0, 0, 0, 0, -0, -0, -0, -0, 0, 0, 0, -0, -0, -0, -0, -0, 0, 0, 0,
            -0, -0, -0, -0, -0, 0, 0, 0, -0, -1, -3, -7, 0, 52, 181, 291, -0, -1216, -3156, -3814, 0, 8988, 17531, 15918, -0, -21183, -31046, -21183, -0, 15918, 17531, 8988, 0, -3814,
            -3156, -1216, -0, 291, 181, 52, 0, -7, -3, -1, -0, 0, 0, 0, -0, -0, -0, -0, -0, 0, 0, 0, -0, -0, -0, -0, -0, 0, 0, 0, -0, -0, -0, -0, 0, 0, 0, 0, -0, -0, -0, -0, 0, 0, 0,
            0, -0, -0, -0, -0, 0, 0, 0, 0, -0, -0, -0, -0, -0, 0, 0, 0, -0, -0, -0, -0, -0, 0, 0, 0, -0, -0, -0, -0, -0, 0, 0, 0, 0, -0, -0, -0, -0, 0, 0, 0, -0, -0, -0, -0, -0, 0, 0,
            0, 0, -0, -0, -0, -0, 0, 0, 0, -0, -0, -0, -0, -0, 0, 0, 0, 0, -0, -0, -0, -0, 0, 0, 0, -0, -0, -0, -0, -0, 0, 0, 0, 0, -0, -0, -0, -0, 0, 0, 0, 0, -0, -0, 0, -0, 0, 0, 0, 
            0, 0, 0, 0, -0, 0, -0, -0, 0, 0, 0, 0, -0, -0, -0, -0, 0, 0, 0, 0, -0, -0, -0, -0, -0, 0, 0, 0, -0, -0, -0, -0, 0, 0, 0, 0, -0, -0, -0, -0, -0, 0, 0, 0, -0, -0, -0, -0, 0,
            0, 0, 0, -0, -0, -0, -0, -0, 0, 0, 0, -0, -0, -0, -0, 0, 0, 0, 0, -0, -0, -0, -0, -0, 0, 0, 0, -0, -0, -0, -0, -0, 0, 0, 0, -0, -0, -0, -0, -0, 0, 0, 0, 0, -0, -0, -0, -0,
            0, 0, 0, 0, -0, -0, -0, -0, 0, 0, 0, 0, -0, -0, -0, -0, 0, 0, 0, -0, -0, -0, -0, -0, 0, 0, 0, -0, -0, -0, -0, -0, 0, 0, 0, -0, -1, -2, -5, 0, 37, 127, 204, -0, -851, -2209,
            -2670, 0, 6292, 12272, 11142, -0, -14828, -21732, -14828, -0, 11142, 12272, 6292, 0, -2670, -2209, -851, -0, 204, 127, 37, 0, -5, -2, -1, -0, 0, 0, 0, -0, -0, -0, -0, -0, 0,
            0, 0, -0, -0, -0, -0, -0, 0, 0, 0, -0, -0, -0, -0, 0, 0, 0, 0, -0, -0, -0, -0, 0, 0, 0, 0, -0, -0, -0, -0, 0, 0, 0, 0, -0, -0, -0, -0, -0, 0, 0, 0, -0, -0, -0, -0, -0, 0, 0,
            0, -0, -0, -0, -0, -0, 0, 0, 0, 0, -0, -0, -0, -0, 0, 0, 0, -0, -0, -0, -0, -0, 0, 0, 0, 0, -0, -0, -0, -0, 0, 0, 0, -0, -0, -0, -0, -0, 0, 0, 0, 0, -0, -0, -0, -0, 0, 0, 0,
            -0, -0, -0, -0, -0, 0, 0, 0, 0, -0, -0, -0, -0, 0, 0, 0, 0, -0, -0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
            -0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
            0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, -0, 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, 0, 0,
            0, 0, -0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            -0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0 };
         return data;
      }

      public static Int16[] RecordedMockData()
      {
       Int16[] data = { -32768, -32768, -32768, -32768, -32768, -32768, -32768, -32768,
         -32768, -32551, -29241, -25089, -22091, -17422, -12748, -9878, -8433,
         -8168, -8064, -7395, -6292, -4847, -3615, -2258, -1082, -303, 0, -38,
         -36, 20, 176, 426, 760, 1154, 1490, 1771, 1976, 2166, 2245, 2188,
         2056, 1903, 1816, 1799, 1805, 1865, 1960, 2085, 2146, 2138, 2051,
         2033, 2050, 2108, 2177, 2240, 2438, 3058, 4309, 5851, 6516, 4210,
         -2968, -12430, -16852, -10949, 363, 9760, 16441, 19740, 20431, 17118,
         8523, -1958, -8620, -10163, -8098, -5380, -3313, -1358, 624, 2444,
         3734, 4302, 4177, 3592, 2881, 2359, 2170, 2311, 2665, 3082, 3392,
         3518, 3379, 2997, 2374, 1684, 1174, 1084, 1461, 2167, 2958, 3502,
         3463, 2625, 1026, -813, -2155, -2394, -1417, 502, 2510, 3952, 4607,
         4422, 3472, 2233, 1292, 988, 1317, 1986, 2555, 2764, 2550, 2172, 2049,
         2464, 3125, 3017, 915, -3561, -8014, -8456, -1096, 6992, 12423, 14749,
         14455, 10562, 1976, -7608, -12350, -8935, -2499, 2237, 5774, 7588,
         7813, 6269, 3024, -615, -2828, -2692, -285, 1603, 2451, 2600, 2190,
         1352, 499, 85, 523, 1874, 3510, 4829, 5540, 5406, 4209, 1889, -998,
         -3281, -3815, -1859, 528, 2357, 3651, 4227, 4092, 3365, 2235, 1164,
         532, 632, 1153, 1623, 1859, 1879, 1703, 1424, 1098, 838, 661, 534,
         486, 670, 1195, 1644, 1426, 370, -1011, -1145, 2311, 6797, 9301, 9169,
         6817, 1967, -3957, -7704, -5639, -848, 2731, 5194, 5918, 5063, 3049,
         397, -1852, -2188, -502, 1048, 2126, 2828, 2999, 2692, 2045, 1283,
         629, 375, 727, 1551, 2467, 2978, 2725, 1693, 212, -1137, -1428, -374,
         1258, 2793, 3816, 4034, 3488, 2404, 1190, 373, 246, 547, 939, 1201,
         1268, 1171, 1030, 1024, 1238, 1622, 1922, 1909, 1507, 853, 317, 443,
         1374, 2511, 2949, 2184, 558, -863, -321, 2020, 3978, 4166, 2581, 67,
         -2118, -1993, 87, 2400, 4267, 5089, 4534, 2794, 441, -1586, -2124,
         -1240, 232, 1896, 3296, 3978, 3717, 2609, 1029, -455, -1200, -952,
         114, 1601, 2837, 3221, 2569, 1204, -234, -891, -377, 897, 2337, 3417,
         3733, 3268, 2290, 1212, 476, 209, 324, 576, 694, 629, 518, 484, 749,
         1318, 2048, 2608, 2725, 2282, 1402, 455, 4, 333, 1339, 2467, 2978,
         2359, 814, -735, -990, 370, 2108, 2725, 1767, 41, -1019, -423, 1398,
         3344, 4589, 4741, 3679, 1715, -463, -1925, -1934, -818, 789, 2308,
         3242, 3249, 2380, 1011, -272, -891, -673, 263, 1562, 2707, 3302, 2998,
         1936, 597, -342, -371, 452, 1669, 2653, 3039, 2714, 1883, 881, 186,
         21, 329, 842, 1217, 1279, 1108, 864, 788, 975, 1384, 1850, 2111, 1968,
         1395, 619, -28, -136, 434, 1460, 2431, 2855, 2372, 1175, -163, -720,
         50, 1416, 2192, 1678, 330, -581, -42, 1672, 3431, 4306, 3981, 2659,
         799, -862, -1671, -1280, 23, 1564, 2705, 3034, 2515, 1388, 158, -634,
         -692, -87, 938, 1967, 2678, 2854, 2454, 1611, 669, 110, 167, 730,
         1388, 1774, 1696, 1263, 697, 316, 424, 877, 1483, 1988, 2088, 1703,
         1055, 546, 415, 687, 1173, 1609, 1772, 1506, 931, 300, -101, 0, 628,
         1474, 2184, 2389, 2022, 1227, 409, 87, 617, 1629, 2267, 1809, 429,
         -847, -801, 566, 2286, 3282, 3164, 2117, 772, -317, -712, -263, 752,
         1879, 2583, 2575, 1913, 918, 49, -360, -203, 405, 1161, 1794, 2067,
         1985, 1586, 1030, 529, 329, 558, 1106, 1627, 1832, 1580, 1043, 534,
         321, 496, 1021, 1608, 1975, 1881, 1370, 718, 278, 240, 579, 1092,
         1476, 1587, 1394, 989, 492, 167, 176, 539, 1103, 1567, 1739, 1577,
         1174, 797, 622, 915, 1540, 2004, 1773, 761, -343, -546, 444, 1922,
         2828, 2627, 1578, 400, -336, -413, 161, 1092, 1937, 2314, 2091, 1437,
         685, 189, 102, 403, 845, 1261, 1415, 1310, 1042, 803, 621, 569, 713,
         1062, 1493, 1851, 1852, 1459, 868, 434, 307, 569, 1074, 1565, 1776,
         1579, 991, 294, -169, -196, 214, 803, 1333, 1609, 1619, 1422, 1138,
         888, 837, 987, 1243, 1434, 1378, 1103, 720, 443, 419, 745, 1256, 1689,
         1637, 943, -34, -442, 164, 1448, 2424, 2437, 1588, 564, -104, -150,
         293, 884, 1382, 1583, 1459, 1042, 667, 487, 632, 909, 1217, 1400,
         1331, 1078, 785, 585, 527, 615, 774, 1046, 1331, 1540, 1518, 1131,
         650, 320, 345, 716, 1211, 1602, 1633, 1283, 679, 58, -231, -39, 516,
         1174, 1615, 1714, 1538, 1243, 1007, 876, 946, 1056, 1148, 1137, 961,
         630, 302, 191, 393, 820, 1287, 1625, 1579, 999, 200, -291, 46, 1052,
         2065, 2312, 1639, 616, -83, -133, 255, 811, 1199, 1309, 1148, 857,
         625, 588, 823, 1150, 1442, 1556, 1427, 1110, 725, 469, 436, 559, 763,
         941, 1090, 1140, 1026, 753, 358, 160, 287, 706, 1245, 1626, 1736,
         1467, 1002, 490, 222, 242, 549, 978, 1265, 1338, 1167, 970, 786, 704,
         695, 739, 840, 881, 860, 737, 579, 554, 730, 1058, 1339, 1471, 810,
         99, -400, -318, 434, 1438, 1969, 1667, 868, 292, 329, 788, 1292, 1506,
         1385, 1078, 734, 504, 450, 582, 809, 971, 988, 905, 668, 492, 451,
         583, 801, 1032, 1147, 1125, 1025, 860, 605, 306, 113, 181, 589, 1137,
         1557, 1616, 1399, 996, 605, 415, 508, 811, 1140, 1310, 1225, 887, 517,
         330, 328, 532, 753, 898, 957, 913, 762, 610, 571, 763, 1069, 1345,
         1382, 1141, 654, 82, -402, -405, 139, 988, 1623, 1602, 1057, 543, 545,
         974, 1499, 1640, 1436, 998, 520, 251, 175, 290, 515, 735, 860, 851,
         742, 637, 588, 681, 895, 1088, 1152, 1047, 860, 649, 471, 280, 166,
         280, 646, 1131, 1488, 1492, 1200, 787, 480, 374, 511, 753, 967, 1037,
         887, 578, 340, 305, 463, 683, 852, 852, 888, 810, 704, 668, 658, 772,
         976, 1231, 1405, 1363, 1053, 545, 51, -277, -285, 139, 812, 1384,
         1497, 1109, 597, 447, 774, 1494, 1416, 1196, 728, 291, 102, 154, 406,
         704, 917, 971, 880, 678, 485, 400, 449, 605, 815, 895, 823, 641, 571,
         592, 673, 700, 796, 969, 1234, 1340, 1191, 855, 466, 223, 217, 362,
         566, 726, 777, 736, 587, 478, 524, 685, 865, 937, 878, 722, 578, 579,
         606, 688, 793, 936, 1059, 1065, 905, 569, 253, 65, 92, 348, 785, 1179,
         1314, 992, 542, 305, 507, 918, 1179, 1046, 634, 275, 134, 299, 577,
         824, 946, 937, 835, 679, 539, 461, 474, 549, 636, 711, 631, 449, 353,
         409, 562, 769, 911, 1075, 1218, 1289, 1128, 773, 370, 121, 788, 1292,
         1506, 1385, 1078, 734, 504, 450, 582, 809, 971, 988, 905, 668, 492,
         451, 583, 801, 1032, 1147, 1125, 1025, 860, 605, 306, 113, 181, 589,
         1137, 1557, 1616, 1399, 996, 605, 415, 508, 811, 1140, 1310, 1225,
         887, 517, 330, 328, 532, 753, 898, 957, 913, 762, 610, 571, 763, 1069,
         1345, 1382, 1141, 654, 82, -402, -405, 139, 988, 1623, 1602, 1057,
         543, 545, 974, 1499, 1640, 1436, 998, 520, 251, 175, 290, 515, 735,
         860, 851, 742, 637, 588, 681, 895, 1088, 1152, 1047, 860, 649, 471,
         280, 166, 280, 646, 1131, 1488, 1492, 1200, 787, 480, 374, 511, 753,
         967, 1037, 887, 578, 340, 305, 463, 683, 852, 852, 888, 810, 704, 668,
         658, 772, 976, 1231, 1405, 1363, 1053, 545, 51, -277, -285, 139, 812,
         1384, 1497, 1109, 597, 447, 774, 1494, 1416, 1196, 728, 291, 102, 154,
         406, 704, 917, 971, 880, 678, 485, 400, 449, 605, 815, 895, 823, 641,
         571, 592, 673, 700, 796, 969, 1234, 1340, 1191, 855, 466, 223, 217,
         362, 566, 726, 777, 736, 587, 478, 524, 685, 865, 937, 878, 722, 578,
         579, 606, 688, 793, 936, 1059, 1065, 905, 569, 253, 65, 92, 348, 785,
         1179, 1314, 992, 542, 305, 507, 918, 1179, 1046, 634, 275, 134, 299,
         577, 824, 946, 937, 835, 679, 539, 461, 474, 549, 636, 711, 631, 449,
         353, 409, 562, 769, 911, 1075, 1218, 1289, 1128, 773, 370, 121, 788,
         1292, 1506, 1385, 1078, 734, 504, 450, 582, 809, 971, 988, 905, 668,
         492, 451, 583, 801, 1032, 1147, 1125, 1025, 860, 605, 306, 113, 181,
         589, 1137, 1557, 1616, 1399, 996, 605, 415, 508, 811, 1140, 1310,
         1225, 887, 517, 330, 328, 532, 753, 898, 957, 913, 762, 610, 571, 763,
         1069, 1345, 1382, 1141, 654, 82, -402, -405, 139, 988, 1623, 1602,
         1057, 543, 545, 974, 1499, 1640, 1436, 998, 520, 251, 175, 290, 515,
         735, 860, 851, 742, 637, 588, 681, 895, 1088, 1152, 1047, 860, 649,
         471, 280, 166, 280, 646, 1131, 1488, 1492, 1200, 787, 480, 374, 511,
         753, 967, 1037, 887, 578, 340, 305, 463, 683, 852, 852, 888, 810, 704,
         668, 658, 772, 976, 1231, 1405, 1363, 1053, 545, 51, -277, -285, 139,
         812, 1384, 1497, 1109, 597, 447, 774, 1494, 1416, 1196, 728, 291, 102,
         154, 406, 704, 917, 971, 880, 678, 485, 400, 449, 605, 815, 895, 823,
         641, 571, 592, 673, 700, 796, 969, 1234, 1340, 1191, 855, 466, 223,
         217, 362, 566, 726, 777, 736, 587, 478, 524, 685, 865, 937, 878, 722,
         578, 579, 606, 688, 793, 936, 1059, 1065, 905, 569, 253, 65, 92, 348,
         785, 1179, 1314, 992, 542, 305, 507, 918, 1179, 1046, 634, 275, 134,
         299, 577, 824, 946, 937, 835, 679, 539, 461, 474, 549, 636, 711, 631,
         449, 353, 409, 562, 769, 911, 1075, 1218, 1289, 1128, 773, 370, 121,
         788, 1292, 1506, 1385, 1078, 734, 504, 450, 582, 809, 971, 988, 905,
         668, 492, 451, 583, 801, 1032, 1147, 1125, 1025, 860, 605, 306, 113,
         181, 589, 1137, 1557, 1616, 1399, 996, 605, 415, 508, 811, 1140, 1310,
         1225, 887, 517, 330, 328, 532, 753, 898, 957, 913, 762, 610, 571, 763,
         1069, 1345, 1382, 1141, 654, 82, -402, -405, 139, 988, 1623, 1602,
         1057, 543, 545, 974, 1499, 1640, 1436, 998, 520, 251, 175, 290, 515,
         735, 860, 851, 742, 637, 588, 681, 895, 1088, 1152, 1047, 860, 649,
         471, 280, 166, 280, 646, 1131, 1488, 1492, 1200, 787, 480, 374, 511,
         753, 967, 1037, 887, 578, 340, 305, 463, 683, 852, 852, 888, 810, 704,
         668, 658, 772, 976, 1231, 1405, 1363, 1053, 545, 51, -277, -285, 139,
         812, 1384, 1497, 1109, 597, 447, 774, 1494, 1416, 1196, 728, 291, 102,
         154, 406, 704, 917, 971, 880, 678, 485, 400, 449, 605, 815, 895, 823,
         641, 571, 592, 673, 700, 796, 969, 1234, 1340, 1191, 855, 466, 223,
         217, 362, 566, 726, 777, 736, 587, 478, 524, 685, 865, 937, 878, 722,
         578, 579, 606, 688, 793, 936, 1059, 1065, 905, 569, 253, 65, 92, 348,
         785, 1179, 1314, 992, 542, 305, 507, 918, 1179, 1046, 634, 275, 134,
         299, 577, 824, 946, 937, 835, 679, 539, 461, 474, 549, 636, 711, 631,
         449, 353, 409, 562, 769, 911, 1075, 1218, 1289, 1128, 773, 370, 121,
         788, 1292, 1506, 1385, 1078, 734, 504, 450, 582, 809, 971, 988, 905,
         668, 492, 451, 583, 801, 1032, 1147, 1125, 1025, 860, 605, 306, 113,
         181, 589, 1137, 1557, 1616, 1399, 996, 605, 415, 508, 811, 1140, 1310,
         1225, 887, 517, 330, 328, 532, 753, 898, 957, 913, 762, 610, 571, 763,
         1069, 1345, 1382, 1141, 654, 82, -402, -405, 139, 988, 1623, 1602,
         1057, 543, 545, 974, 1499, 1640, 1436, 998, 520, 251, 175, 290, 515,
         735, 860, 851, 742, 637, 588, 681, 895, 1088, 1152, 1047, 860, 649,
         471, 280, 166, 280, 646, 1131, 1488, 1492, 1200, 787, 480, 374, 511,
         753, 967, 1037, 887, 578, 340, 305, 463, 683, 852, 852, 888, 810, 704,
         668, 658, 772, 976, 1231, 1405, 1363, 1053, 545, 51, -277, -285, 139,
         812, 1384, 1497, 1109, 597, 447, 774, 1494, 1416, 1196, 728, 291, 102,
         154, 406, 704, 917, 971, 880, 678, 485, 400, 449, 605, 815, 895, 823,
         641, 571, 592, 673, 700, 796, 969, 1234, 1340, 1191, 855, 466, 223,
         217, 362, 566, 726, 777, 736, 587, 478, 524, 685, 865, 937, 878, 722,
         578, 579, 606, 688, 793, 936, 1059, 1065, 905, 569, 253, 65, 92, 348,
         785, 1179, 1314, 992, 542, 305, 507, 918, 1179, 1046, 634, 275, 134,
         299, 577, 824, 946, 937, 835, 679, 539, 461, 474, 549, 636, 711, 631,
         449, 353, 409, 562, 769, 911, 1075, 1218, 1289, 1128, 773, 370, 121,
         788, 1292, 1506, 1385, 1078, 734, 504, 450, 582, 809, 971, 988, 905,
         668, 492, 451, 583, 801, 1032, 1147, 1125, 1025, 860, 605, 306, 113,
         181, 589, 1137, 1557, 1616, 1399, 996, 605, 415, 508, 811, 1140, 1310,
         1225, 887, 517, 330, 328, 532, 753, 898, 957, 913, 762, 610, 571, 763,
         1069, 1345, 1382, 1141, 654, 82, -402, -405, 139, 988, 1623, 1602,
         1057, 543, 545, 974, 1499, 1640, 1436, 998, 520, 251, 175, 290, 515,
         735, 860, 851, 742, 637, 588, 681, 895, 1088, 1152, 1047, 860, 649,
         471 };

         return data;
      }
   }
}

