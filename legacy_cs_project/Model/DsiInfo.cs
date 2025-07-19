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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

using DsiUtil;
using TabletApp.Api.DsiApi;

namespace Model
{
   /// <summary>
   /// Fields read directly from the DSI via marshaling. Prefer ADsiInfo for general use.
   /// </summary>
   [StructLayout(LayoutKind.Sequential, Pack = 2, CharSet = CharSet.Ansi)]
   public class AMarshaledDsiInfo
   {
      public AMarshaledDsiInfo()
      {
         serialNumber = "";
         tag = "";
         dsiCount = 1; // always at least one
         firmware = new AFirmwareVersion();
         shotTimeInterval = 0;
         transmitTimeInterval = 0;
         ascanTxInterval = 0;
         dsiModel = "";

         cellProvider = "";
         cellNetworkUserName = "";

         cellNetworkPassword = "";
         gsmAccessPoint = "";

         cloudAppUserName = "";
         cloudAppPassword = "";
         cloudAppUrl = "";
      }

      // We need round trip between child and parent classes for serialization and marshaling.
      public AMarshaledDsiInfo(ADsiInfo dsiAndProbes)
      {
         this.CopyFrom((AMarshaledDsiInfo)dsiAndProbes);
      }


      public void CopyFrom(AMarshaledDsiInfo marshaledInfo)
      {
         modbusAddress = marshaledInfo.modbusAddress;
         serialNumber = marshaledInfo.serialNumber;
         tag = marshaledInfo.tag;
         firmware = new AFirmwareVersion()
         {
            fpga = marshaledInfo.firmware.fpga,
            micro = marshaledInfo.firmware.micro
         };
         location = new AGpsCoordinate()
         {
            coordinates = marshaledInfo.location.coordinates
         };
         dsiCount = marshaledInfo.dsiCount;
         probeCount = marshaledInfo.probeCount;
         packetVersion = marshaledInfo.packetVersion;
         baudRate = marshaledInfo.baudRate;
         parityAndStopBits = marshaledInfo.parityAndStopBits;
         description = marshaledInfo.description;
         shotTimeInterval = marshaledInfo.shotTimeInterval;
         transmitTimeInterval = marshaledInfo.transmitTimeInterval;
         ascanTxInterval = marshaledInfo.ascanTxInterval;
         dsiModel = marshaledInfo.dsiModel;

         cellProvider = marshaledInfo.cellProvider;
         cellNetworkUserName = marshaledInfo.cellNetworkUserName;

         cellNetworkPassword = marshaledInfo.cellNetworkPassword;
         gsmAccessPoint = marshaledInfo.gsmAccessPoint;
         cloudAppUserName = marshaledInfo.cloudAppUserName;
         cloudAppPassword = marshaledInfo.cloudAppPassword;
         cloudAppUrl = marshaledInfo.cloudAppUrl;
      }


      [XmlAttribute("address")]
      [MarshalAs(UnmanagedType.U2)]
      public UInt16 modbusAddress; // only low byte is valid, and 0 is broadcast

      public AUtf8String32 serialNumber;

      public AUtf8String64 tag;

      public AFirmwareVersion firmware;

      [XmlIgnore]
      public AGpsCoordinate location;
      /// <summary>
      /// GPS lat,lon as attributes is handled in the properties of ADsiInfo
      /// </summary>

      [MarshalAs(UnmanagedType.U2)]
      public UInt16 dsiCount;

      [MarshalAs(UnmanagedType.U2)]
      public UInt16 probeCount;

      public AUtf8String32 packetVersion;

      public AUtf8String16 baudRate;

      [XmlElement("parity")]
      [MarshalAs(UnmanagedType.U2)]
      public UInt16 parityAndStopBits;

      public AUtf8String128 description;

      [MarshalAs(UnmanagedType.U2)]
      public UInt16 shotTimeInterval;

      [MarshalAs(UnmanagedType.U2)]
      public UInt16 transmitTimeInterval;

      [MarshalAs(UnmanagedType.U2)]
      public UInt16 ascanTxInterval;

      [XmlIgnore] // see Model property
      [StringLength(8)]
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
      protected string dsiModel;

      [XmlElement("dsiModel")]
      public string Model
      {
         get { return dsiModel.Trim().ToLower(); } // we were doing this in a few places but not others, so I made it mandatory
         set { this.dsiModel = value; }
      }

      [XmlIgnore]
      public AUtf8String16 cellProvider;

      [XmlIgnore]  // See ADsiInfo.cellAccount
      public AUtf8String32 cellNetworkUserName;

      [XmlIgnore] // See ADsiInfo.cellAcount
      public AUtf8String32 cellNetworkPassword;

      public AUtf8String32 gsmAccessPoint;

      [XmlIgnore] // See ADsiInfo.account
      [StringLength(32)]
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
      public string cloudAppUserName;

      [XmlIgnore] // See ADsiInfo.account
      [StringLength(32)]
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
      public string cloudAppPassword;

      [XmlIgnore] // See ADsiInfo.account
      [StringLength(128)]
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
      public string cloudAppUrl;

      /// <summary>For data-logger DSIs, the number of shots stored in the DSI</summary>
      [XmlIgnore]
      public int numShots;
   }


   /// <summary>
   /// Fields needed for XML serialization but not marshaling from DSI memory.
   /// </summary>
   [XmlRoot("dsi")]
   public partial class ADsiInfo : AMarshaledDsiInfo
   {
      [XmlIgnore]
      public const double kSampleRate = 40e6;

      public ADsiInfo() 
      {
         // empty
      }

      static ADsiInfo()
      {
         sKey = Convert.FromBase64String("gxhdS3sLmjJI2nDVs9rDeKYzCmdeFtqk1SYi5/7CdGA=");
         sInitVector = Convert.FromBase64String("Nvu4QFjgp6H+pl2v72dnaw==");
      }


      public ADsiInfo(AMarshaledDsiInfo marshaledInfo)
      {
         this.CopyFrom(marshaledInfo);
      }

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

      /// <summary>
      /// Wrapper to provide encryption of the saved cloud app credentials
      /// </summary>
      [XmlElement("account")]
      public string Account
      {
         get
         {
            string[] credentialsList = { cloudAppUrl, cloudAppUserName, cloudAppPassword };
            return this.EncodeList(credentialsList);
         }

         set
         {
            string[] credentialsList = this.DecodeListString(value);
            if (null != credentialsList)
            {
               this.cloudAppUrl = credentialsList[0];
               this.cloudAppUserName = credentialsList[1];
               this.cloudAppPassword = credentialsList[2];
            }
         }
      }

      /// <summary>
      /// Wrapper to provide encryption of the saved cell credentials
      /// </summary>
      [XmlIgnore]
      public string CellAccount
      {
         get
         {
            string[] credentialsList = { cellProvider, cellNetworkUserName, cellNetworkPassword };
            return this.EncodeList(credentialsList); 
         }

         set
         {
            string[] credentialsList = this.DecodeListString(value);
            if (null != credentialsList)
            {
               this.cellProvider = credentialsList[0];
               this.cellNetworkUserName = credentialsList[1];
               this.cellNetworkPassword = credentialsList[2];
            }
         }
      }

      /// <param name="list">strings to encrypt</param>
      /// <returns>AES encrypted and Base64 encoded contents of zero-byte delimited string joined frm list.</returns>
      private string EncodeList(string[] list)
      {
         string joinedList = String.Join("\0", list);
         try
         {
            byte[] encrypted = ACrypto.EncryptStringToBytes_Aes(joinedList, sKey, sInitVector);
            return Convert.ToBase64String(encrypted);
         }
         catch (Exception ex)
         {
            Debug.Assert(false, "Configuration Error with AES encryption: " + ex.Message);
            return "";
         }
      }

      private string[] DecodeListString(string listString)
      {
         try
         {
            byte[] encrypted = Convert.FromBase64String(listString);
            string decrypted = ACrypto.DecryptStringFromBytes_Aes(encrypted, sKey, sInitVector);
            return decrypted.Split(new char[] { '\0' });
         }
         catch (Exception ex)
         {
            Debug.Assert(false, "Configuration Error with AES encryption: " + ex.Message);
            return null;
         }
      }


      [XmlArray(IsNullable = true), XmlArrayItem("probe")]
      public AProbe[] probes;

      [XmlIgnore]
      private static readonly byte[] sKey;
      [XmlIgnore]
      private static readonly byte[] sInitVector;
   }


   [XmlRoot("firmware"), StructLayout(LayoutKind.Sequential, Pack = 2)]
   public partial struct AFirmwareVersion
   {
      public AFirmwareVersion(string microVer = null, string fpgaVer = null)
      {
         micro = microVer ?? "";
         fpga = fpgaVer ?? "";
      }

      [StringLength(32)]
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
      public string micro;
      [StringLength(32)]
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
      public string fpga;
   }

}

