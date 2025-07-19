// Copyright (c) 2019 Sensor Networks, Inc.
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
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace TabletApp.Api.DsiApi
{
   /// <summary>
   /// Interface for a fixed width buffer containing utf-8 encoded characters.
   /// </summary>
   public interface IUtf8String
   {
      string ToString();
      int Length { get; }
      int Capacity { get;  }
      byte[] RawBytes { get; }
   }


   [StructLayout(LayoutKind.Sequential, Pack = 2)]
   public class AUtf8String8 : AUtf8XmlMixin, IUtf8String
   {
      static public implicit operator AUtf8String8(string value)
      {
         return AUtf8String.Create<AUtf8String8>(value);
      }

      public static implicit operator string(AUtf8String8 utf8str)
      {
         return AUtf8String.ToString(utf8str);
      }

      public override string ToString()
      {
         return (string)this;
      }

      public AUtf8String8() { }
      public AUtf8String8(IUtf8String other) { AUtf8String.DeepCopy(this, other); }


      public int Capacity => bytes.Length;
      public byte[] RawBytes => bytes;
      public int Length => AUtf8String.StringLength(this);

      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
      private byte[] bytes = new byte[8];
   }


   [StructLayout(LayoutKind.Sequential, Pack = 2)]
   public class AUtf8String16 : AUtf8XmlMixin, IUtf8String
   {
      static public implicit operator AUtf8String16(string value)
      {
         return AUtf8String.Create<AUtf8String16>(value);
      }
  
      public static implicit operator string(AUtf8String16 utf8str)
      {
         return AUtf8String.ToString(utf8str);
      }

      public override string ToString()
      {
         return (string)this;
      }


      public AUtf8String16() { }
      public AUtf8String16(IUtf8String other) { AUtf8String.DeepCopy(this, other); }


      public int Capacity => bytes.Length;
      public byte[] RawBytes => bytes;
      public int Length => AUtf8String.StringLength(this);

      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
      private byte[] bytes = new byte[16];
   }


   [StructLayout(LayoutKind.Sequential, Pack = 2)]
   public class AUtf8String32 : AUtf8XmlMixin, IUtf8String
   {
      static public implicit operator AUtf8String32(string value)
      {
         return AUtf8String.Create<AUtf8String32>(value);
      }

      public static implicit operator string(AUtf8String32 utf8str)
      {
         return AUtf8String.ToString(utf8str);
      }

      public override string ToString()
      {
         return (string)this;
      }


      public AUtf8String32() { }
      public AUtf8String32(IUtf8String other) { AUtf8String.DeepCopy(this, other); }


      public int Capacity => bytes.Length;
      public byte[] RawBytes => bytes;
      public int Length => AUtf8String.StringLength(this);

      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
      private byte[] bytes = new byte[32];
   }

   [StructLayout(LayoutKind.Sequential, Pack = 2)]
   public class AUtf8String64 : AUtf8XmlMixin, IUtf8String
   {
      static public implicit operator AUtf8String64(string value)
      {
         return AUtf8String.Create<AUtf8String64>(value);
      }

      public static implicit operator string(AUtf8String64 utf8str)
      {
         return AUtf8String.ToString(utf8str);
      }

      public override string ToString()
      {
         return (string)this;
      }


      public AUtf8String64() { }
      public AUtf8String64(IUtf8String other) { AUtf8String.DeepCopy(this, other); }
 

      public int Capacity => bytes.Length;
      public byte[] RawBytes => bytes;
      public int Length => AUtf8String.StringLength(this);

      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
      private byte[] bytes = new byte[64];
   }


   [StructLayout(LayoutKind.Sequential, Pack = 2)]
   public class AUtf8String128 : AUtf8XmlMixin, IUtf8String
   {
      static public implicit operator AUtf8String128(string value)
      {
         return AUtf8String.Create<AUtf8String128>(value);
      }

      public static implicit operator string(AUtf8String128 utf8str)
      {
         return AUtf8String.ToString(utf8str);
      }

      public override string ToString()
      {
         return (string)this;
      }
   

      public AUtf8String128() {}
      public AUtf8String128(IUtf8String other) { AUtf8String.DeepCopy(this, other); }

 
      public int Capacity => bytes.Length;
      public byte[] RawBytes => bytes;
      public int Length => AUtf8String.StringLength(this);

      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
      private byte[] bytes = new byte[128];
   }

   
   /// <summary>
   /// Shared implementations for handling AUtf8StringNNN family of classes that go through the common interface.
   /// </summary>
   internal static class AUtf8String
   {
      public static T Create<T>(string value) where T : IUtf8String, new()
      {
         var str = new T();
         AUtf8String.CopyFrom(str, value);
         return str;
      }

      public static void CopyBytesTo(IUtf8String utf8str, byte[] other)
      {
         if (other.Length > 0 && utf8str.Capacity > 0)
         {
            Buffer.BlockCopy(other, 0, utf8str.RawBytes, 0, Math.Min(other.Length, utf8str.Capacity));
            if (other.Length < utf8str.Capacity)
            {
               Array.Clear(utf8str.RawBytes, other.Length, utf8str.Capacity - other.Length);
            }
         }
      }

      public static void CopyFrom(IUtf8String utf8str, string value)
      {
         AUtf8String.CopyBytesTo(utf8str, Encoding.UTF8.GetBytes(value));
      }

      public static void DeepCopy(IUtf8String utf8str, IUtf8String other)
      {
         AUtf8String.CopyBytesTo(utf8str, other.RawBytes);
      }

      public static int StringLength(IUtf8String utf8str)
      {
         var index = Array.FindIndex(utf8str.RawBytes, c => c == '\0');
         return index != -1 ? index : utf8str.Capacity;
      }

      public static string ToString(IUtf8String utf8str)
      {
         return Encoding.UTF8.GetString(utf8str.RawBytes).TrimEnd('\0');
      }
   }

   [StructLayout(LayoutKind.Sequential, Pack = 2)]
   public class AUtf8XmlMixin : IXmlSerializable
   {
      public void WriteXml(XmlWriter writer)
      {
         writer.WriteString(AUtf8String.ToString((IUtf8String)this));
      }
      public void ReadXml(XmlReader reader)
      {
         AUtf8String.CopyFrom((IUtf8String)this, reader.ReadString() ?? "");
         reader.Read(); // Move to next node or XmlSerializer gets confused.
      }
      public XmlSchema GetSchema()
      {
         return (null);
      }
   }
}

