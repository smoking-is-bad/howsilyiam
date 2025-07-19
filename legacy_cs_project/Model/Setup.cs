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
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using DsiApi;
using DsiUtil;

namespace Model
{
   /// <summary>
   /// Setup fields which we need for XML but not for marshaling.
   /// </summary>
   [XmlRoot("setup")]
   public partial class ASetup : AMarshaledSetup
   {
      // See ADsiInfo
      public ASetup(AMarshaledSetup setup, int setupNum)
      {
         this.CopyFrom(setup);
         num = setupNum + 1;
      }


      // Needed for serialization.
      public ASetup()
      {
         num = 0;

         // Default values indicating errors reading the probe or shot firing.
         String kDefault = "NaN";
         dsiTemp = kDefault;
         materialTemp = kDefault;
         status = 0;
         calculatedThickness = kDefault;
         lastThickness = kDefault;
         pulserWidth = 0;
         gain = 1;
         muxSwitchSettings = 0;
         switchSettings = 0;
         ascanStart = 0;
      }

      [XmlAttribute]
      public int num;

      [XmlArray("gates", IsNullable = false), XmlArrayItem("gate")]
      public AGate[] gates;

      [XmlIgnore] // See AscanStringData property
      public Int16[] ascanData;

      [XmlIgnore] // See CrossingsStringData property
      public double[] zeroCrossings;
      public double crossingDelayCompensation; // set to upsampler.Delay * 1e09 / 25 / upsampler.scale (i.e. units are low-res samples)

      [XmlIgnore]
      public AThicknessCalculator fCalculator;


      // see http://stackoverflow.com/questions/1379888/how-do-you-serialize-a-string-as-cdata-using-xmlserializer

      public const string kValueSeparater = ",";

      [XmlElement("crossings")]
      public string CrossingsStringData
      {
         get
         {
            if (null == zeroCrossings)
            {
               return ""; // EARLY EXIT
            }
            return zeroCrossings.ToDelimitedString(kValueSeparater);
         }

         set
         {
            List<double> tempData = new List<double>();
            //zeroCrossings = new double[3]; // Populate appends, so start empty.
            tempData.PopulateWithDelimitedString(value, kValueSeparater,
             (string v) => double.Parse(v));
            this.zeroCrossings = tempData.ToArray();
         }
      }

      [XmlElement("ascan", IsNullable = false)]
      public string AscanStringData
      {
         get
         {
            if (null == ascanData)
            {
               return ""; // EARLY EXIT
            }

            // Convert to list of 4 bytes hex values with no delimiters
            StringBuilder sb = new StringBuilder(ascanData.Length * sizeof(Int16));
            foreach (Int16 a in ascanData)
            {
               sb.AppendFormat("{0:x4}", a); // 2 chars represent one byte, Int16 is 2 bytes
            }
            return sb.ToString();
         }

         set
         {
            if (value.Contains(",")) // Old format, comma separated
            {
               List<Int16> tempData = new List<Int16>();
               tempData.PopulateWithDelimitedString(value, kValueSeparater,
                (string v) => Int16.Parse(v));
               this.ascanData = tempData.ToArray();
            }
            else // New format
            {
               // Convert list of hex values representing 2 byte integers with no delimiters
               const int kHexBase = 16;
               int sz = value.Length;
               int step = sizeof(Int16) * 2; // number of chars to produce one Int16 value
               Int16[] tempData = new Int16[sz / step];
               for (int i = 0, j = 0; i < sz; i += step, ++j)
               {
                  tempData[j] = Convert.ToInt16(value.Substring(i, step), kHexBase);
               }
               
               this.ascanData = tempData;
            }
         }
      }
   }


   /// <summary>
   /// Setup data read from DSI memory via marshaling. Prefer ASetup for general use.
   /// </summary>
   [StructLayout(LayoutKind.Sequential, Pack=2, CharSet = CharSet.Ansi)]
   public partial class AMarshaledSetup
   {
      public AMarshaledSetup()
      {
         // empty
      }

      public AMarshaledSetup(ASetup setupAndGates)
      {
         this.CopyFrom((AMarshaledSetup)setupAndGates);
      }


      public void CopyFrom(AMarshaledSetup setup)
      {
         dsiTemp = setup.dsiTemp;
         materialTemp = setup.materialTemp;
         status = setup.status;
         calculatedThickness = setup.calculatedThickness;
         timestamp = setup.timestamp;
         lastThickness = setup.lastThickness;
         lastTimestamp = setup.lastTimestamp;
         pulserWidth = setup.pulserWidth;
         gain = setup.gain;
         muxSwitchSettings = setup.muxSwitchSettings;
         switchSettings = setup.switchSettings;
         ascanStart = setup.ascanStart;
      }


      [StringLength(8)]
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
      public string dsiTemp;

      [StringLength(8)]
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
      public string materialTemp;

      [MarshalAs(UnmanagedType.U2)]
      public UInt16 status;

      // $$$ We might want to make this and timestamp private to prevent their use.
      [StringLength(16)]
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
      public string calculatedThickness;

      [StringLength(32)]
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
      public string timestamp;

      /// <summary>
      /// The tablet app is responsible for updating this and lastTimestamp after a shot.
      /// We will read calculatedThickness and set lastThickness to it.
      /// The XML no longer contains this tag.
      /// </summary>
      [XmlIgnore]
      [StringLength(16)]
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
      public string lastThickness;

      /// <summary>
      /// The tablet app is responsible for updating lastTimestamp with the timestamp of the
      /// most recent shot on the DSI, written when we also write lastThickness. 
      /// The xML no longer contains this tag.
      /// </summary>
      [XmlIgnore]
      [StringLength(32)]
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
      public string lastTimestamp;

      // These next 5 values are critical for firing a shot.

      [MarshalAs(UnmanagedType.U2)]
      public UInt16 pulserWidth;

      [MarshalAs(UnmanagedType.U2)]
      public UInt16 gain;  // steps in 1/10 dB, so 400 is 40 dB

      [MarshalAs(UnmanagedType.U2)]
      public UInt16 muxSwitchSettings;

      [MarshalAs(UnmanagedType.U2)]
      public UInt16 switchSettings; // 8 bits switch value, 5 bit average.

      [MarshalAs(UnmanagedType.U4)]
      public UInt32 ascanStart;
   }
}

[StructLayout(LayoutKind.Sequential, Pack = 2)]
class AMarshaledZeroCrossings
{
   public AMarshaledZeroCrossings()
   {
      gateOneCrossing = gateTwoCrossing = gateThreeCrossing = 0;
   }


   public AMarshaledZeroCrossings(AMarshaledZeroCrossings other)
   {
      gateOneCrossing = other.gateOneCrossing;
      gateTwoCrossing = other.gateTwoCrossing;
      gateThreeCrossing = other.gateThreeCrossing;
   }


   public AMarshaledZeroCrossings(double[] crossings)
   {
      Debug.Assert(crossings.Length >= 3, "Not enough crossings passed to AMarshaledZeroCrossings(double[])");
      gateOneCrossing = (UInt16)Math.Round(crossings[0]);
      gateTwoCrossing = (UInt16)Math.Round(crossings[1]);
      gateThreeCrossing = (UInt16)Math.Round(crossings[2]);
   }


   public void CopyTo(double[] crossings)
   {
      Debug.Assert(crossings.Length >= 3, "Not enough crossings passed to AMarshaledZeroCrossings.CopyTo");
      crossings[0] = (double)gateOneCrossing;
      crossings[1] = (double)gateTwoCrossing;
      crossings[2] = (double)gateThreeCrossing;
   }


   [MarshalAs(UnmanagedType.U2)]
   public UInt16 gateOneCrossing;
   [MarshalAs(UnmanagedType.U2)]
   public UInt16 gateTwoCrossing;
   [MarshalAs(UnmanagedType.U2)]
   public UInt16 gateThreeCrossing;
}