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
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

using TabletApp.Api.DsiApi;

namespace Model
{
   [XmlRoot("location"), StructLayout(LayoutKind.Sequential, Pack = 2)]
   public partial class APostalAddress
   {
      public APostalAddress()
      {
         address1 = "";
         address2 = "";
         city = "";
         state = "";
         postalCode = "";
         country = "";
      }

      public AUtf8String64 address1;
      public AUtf8String64 address2;
      public AUtf8String32 city;
      public AUtf8String32 state;
      public AUtf8String16 postalCode;
      public AUtf8String32 country;
   }


   /// <summary>
   /// GPS Coordinate stored in "lat;lon" form. Properties parse the string and
   /// can retrieve each coordinate.
   /// </summary>
   [XmlRoot("location"), StructLayout(LayoutKind.Sequential, Pack = 2)]
   public partial struct AGpsCoordinate
   {
      public AGpsCoordinate(string coords = null)
      {
         coordinates = coords ?? "";
      }

      /// <summary>
      /// The individual parts of the coordinates are parsed below.
      /// </summary>
      [XmlIgnore]
      [StringLength(32)]
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
      public string coordinates;

      /// <summary>
      /// Parsing helper to get and set latitude from the coordinate string for XML serialization.
      /// </summary>
      [XmlAttribute("latitude")]
      public string Latitude
      {
         get { return this.Coordinate(CoordinateIndex.kLatitude); }
         set { this.SetCoordinate(value, CoordinateIndex.kLatitude); }
      }

      /// <summary>
      /// Parsing helper to get and set longitude from the coordinate string for XML serialization.
      /// </summary>
      [XmlAttribute("longitude")]
      public string Longitude
      {
         get { return this.Coordinate(CoordinateIndex.kLongitude); }
         set { this.SetCoordinate(value, CoordinateIndex.kLongitude); }
      }


      private enum CoordinateIndex : int
      {
         kLatitude = 0,
         kLongitude
      }


      private string Coordinate(CoordinateIndex index)
      {
         if (null == this.coordinates || 3 > this.coordinates.Length)
         {
            return null;
         }
         return this.coordinates.Split(sGpsSeparator)[(int)index];
      }


      private void SetCoordinate(string ordinate, CoordinateIndex index)
      {
         var gpsParts = new string[] { "", "" };
         if (null != this.coordinates && 3 <= this.coordinates.Length)
         {
            gpsParts = this.coordinates.Split(sGpsSeparator);
         }
         gpsParts[(int)index] = ordinate;
         this.coordinates = gpsParts[(int)CoordinateIndex.kLatitude] + sGpsSeparator[0] +
          gpsParts[(int)CoordinateIndex.kLongitude];
      }

      [XmlIgnore]
      private static char[] sGpsSeparator = new char[] { ';' };
   }
}

