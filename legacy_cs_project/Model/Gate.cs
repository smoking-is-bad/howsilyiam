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
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace Model
{
   [XmlRoot("gate"), StructLayout(LayoutKind.Sequential, Pack = 2)]
   public partial class AMarshaledGate
   {
      public AMarshaledGate()
      {
         width = 0; // start disabled
      }

      public AMarshaledGate(AMarshaledGate other)
      {
         this.CopyValuesFrom(other);
      }

      public void CopyValuesFrom(AMarshaledGate other)
      {
         this.start = other.start;
         this.width = other.width;
         this.threshold = other.threshold;
         this.mode = other.mode;
         this.tof = other.tof;
         this.amplitude = other.amplitude;
      }

      [MarshalAs(UnmanagedType.U4)]
      public UInt32 start;

      [MarshalAs(UnmanagedType.U4)]
      public UInt32 width;

      [MarshalAs(UnmanagedType.I2)]
      public Int16 threshold;

      [MarshalAs(UnmanagedType.U2)]
      public UInt16 mode;
      
      [MarshalAs(UnmanagedType.U4)]
      public UInt32 tof;

      [MarshalAs(UnmanagedType.U2)]
      public UInt16 amplitude;
   }

   /// <summary>
   /// Extra Gate fields needed for XML serialization. 
   /// </summary>
   public partial class AGate : AMarshaledGate
   {
      public AGate()
      {
         //empty
      }

      public AGate(AMarshaledGate marshaledGate)
      {
         this.CopyValuesFrom(marshaledGate);
      }

      [XmlAttribute, Range(1, 3)]
      public int num;
   }
}

