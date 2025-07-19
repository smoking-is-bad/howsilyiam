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
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

using TabletApp.Api.DsiApi;

namespace Model
{
   /// <summary>
   /// Probe data read from DSI memory via marshaling. Prefer AProbe.
   /// </summary>
   [StructLayout(LayoutKind.Sequential, Pack = 2, CharSet = CharSet.Ansi)]
   public partial class AMarshaledProbe
   {
      // Needed for XML serialization
      public AMarshaledProbe() 
      {
         model = "";
         type = "";
         description = "";
         location = new AGpsCoordinate();
         nominalThickness = "0";
         minimumThickness = "0";
         velocity = "0";
         numSetups = 0;
         calZeroCount = 0;
         warningThickness = "0";
      }

      // See ADsiInfo
      public AMarshaledProbe(AProbe probeAndSetups)
      {
         this.CopyFrom((AMarshaledProbe)probeAndSetups);
      }


      public void CopyFrom(AMarshaledProbe probe)
      {
         model = probe.model;
         type = probe.type;
         description = probe.description;
         location = new AGpsCoordinate() 
          { 
             coordinates = probe.location.coordinates
          };
         nominalThickness = probe.nominalThickness;
         minimumThickness = probe.minimumThickness;
         velocity = probe.velocity;
         numSetups = probe.numSetups;
         calZeroCount = probe.calZeroCount;
         warningThickness = probe.warningThickness;
      }


      public AUtf8String16 model;

      [StringLength(8)]
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
      public string type;

      public AUtf8String64 description;

      [XmlIgnore] // handled with properties on child class
      public AGpsCoordinate location;

      [StringLength(16)]
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
      public string nominalThickness;

      [StringLength(16)]
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
      public string minimumThickness;

      [StringLength(16)]
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
      public string velocity;

      [XmlIgnore]
      [MarshalAs(UnmanagedType.U2)]
      public UInt16 numSetups;

      // 1489: SensorNetworks wanted calZeroCount to align to a 4-byte boundary and added
      // padding before and after it.
      [XmlIgnore]
      [MarshalAs(UnmanagedType.U2)]
      private UInt16 unused1;

      [XmlIgnore] // See calZeroOffset property on AProbe
      [MarshalAs(UnmanagedType.U4)]
      public UInt32 calZeroCount;

      [XmlIgnore]
      [MarshalAs(UnmanagedType.U2)]
      private UInt16 unused2;

      [StringLength(16)]
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
      public string warningThickness;
   }

   /// <summary>
   /// Fields needed for XML but not marshaling.
   /// </summary>
   [XmlRoot("probe")]
   public partial class AProbe : AMarshaledProbe
   {
      // Needed for XML serialization
      public AProbe() 
      { 
         // empty
      }

      public AProbe(AMarshaledProbe probe, int probeNumber)
      {
         this.CopyFrom(probe);
         num = probeNumber + 1;
         if (0 != numSetups)
         {
            setups = new ASetup[numSetups];
         }
      }

      [XmlAttribute]
      public int num;

      [XmlAttribute("latitude")]
      public string Latitude
      {
         get { return location.Latitude; }
         set { location.Latitude = value; }
      }

      [XmlAttribute("longitude")]
      public string Longitude
      {
         get { return location.Longitude; }
         set { location.Longitude = value; }
      }


      [XmlArray("setups", IsNullable = false), XmlArrayItem("setup")]
      public ASetup[] setups;


      public double calZeroOffset //seconds
      {
         get { return (double)calZeroCount * kCalZeroSecondsPerCount; }
         set { calZeroCount = (UInt32)((double)value / kCalZeroSecondsPerCount); }
      }
      
      [XmlIgnore]
      static double kCalZeroSecondsPerCount = 6.103515625 * 1e-12; /* picoseconds/count * seconds/picoseconds */
   }
}
