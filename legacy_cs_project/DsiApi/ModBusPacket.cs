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
using System.Diagnostics;

using DsiUtil;


namespace ModBus
{
   /// <summary>
   /// Encapsulates a ModBus packet as it will be sent on the wire.
   /// 
   /// The CopyTo... and Value[s]AtOffset functions perform byte swapping as
   /// the protocol expects big endian values.
   /// </summary>
   public class APacket
   {
      /// <summary>
      /// Send packets start with an address byte followed by a command byte.
      /// Payload data follows, so this makes a convenient start offset.
      /// </summary>
      public const int kAddressAndCommandSize = 2;

      public const int kVariableLengthResultOffset = (int)Offsets.kVariableSizePacketResultOffset;

      /// <summary>
      /// All packets end with error checking bytes. See CalculateErrorChecks.
      /// </summary>
      public const int kErrorCheckSize = 2;

      /// <summary>
      /// No packets can exceed this length in bytes.
      /// </summary>
      public const int kMaxPacketSize = 255; // modBus address not included

      /// <summary>
      /// How many bytes of command data (packet data unit) the packet can carry.
      /// </summary>
      public const int kMaxPduSize = 250;//!!!: kMaxPacketSize - kAddressAndCommandSize - kErrorCheckSize;

      /// <summary>
      /// Allocate memory for a send packet and set the first two values. The memory
      /// for error checking is automatically included.
      /// </summary>
      /// <param name="address">The address of the DSI which should handle this command</param>
      /// <param name="command">The command for the DSI</param>
      /// <param name="payloadSize">Size of data following the command and preceding the error 
      /// checking. It can't be more than kMaxPduSize bytes.</param>
      public APacket(byte address, byte command, byte payloadSize)
      {
         int totalSize = PacketSizeForCommand(command, payloadSize);
         this.AllocatePacket(totalSize);

         this.SetAddress(address);
         this.SetCommand(command);
      }

      /// <summary>
      /// Simple result constructor for an array of bytes returned
      /// </summary>
      /// <param name="command">The command for which this is the result. This is put in the first byte</param>
      /// <param name="resultData">The data to follow a length byte</param>
      public APacket(byte command, byte[] resultData)
      {
         int totalSize = (int)Offsets.kVariableSizePacketResultOffset + resultData.Length + kErrorCheckSize;
         this.AllocatePacket(totalSize);

         this.SetAddress(0); // address is unused
         this.SetCommand(command);
         this.SetResultLength((byte)resultData.Length);
         this.CopyToPacket(resultData, (int)Offsets.kVariableSizePacketResultOffset);
      }


      public APacket(byte command, UInt16[] resultData)
      {
         int byteLength = resultData.Length * sizeof(UInt16);
         int totalSize = (int)Offsets.kVariableSizePacketResultOffset + byteLength + kErrorCheckSize;
         this.AllocatePacket(totalSize);

         this.SetAddress(0); // address is unused
         this.SetCommand(command);
         this.SetResultLength((byte)byteLength);
         this.CopyToPacket(resultData, (int)Offsets.kVariableSizePacketResultOffset);
      }


      public APacket(byte command, Tuple<UInt16, UInt16> resultData)
      {
         int totalSize = kAddressAndCommandSize + sizeof(UInt16) * 2 + kErrorCheckSize;
         this.AllocatePacket(totalSize);

         this.SetAddress(0);
         this.SetCommand(command);
         this.CopyToPacket(resultData.Item1, 2);
         this.CopyToPacket(resultData.Item2, 4);
      }


      public APacket(byte command, UInt16 resultData)
      {
         int totalSize = kAddressAndCommandSize + sizeof(UInt16) + kErrorCheckSize;
         this.AllocatePacket(totalSize);

         this.SetAddress(0);
         this.SetCommand(command);
         this.CopyToPacket(resultData, 2);
      }
 
      /// <summary>
      /// Create a packet with data already constructed.
      /// </summary>
      /// <param name="data">The packet data, including error checking bytes</param>
      public APacket(byte[] data, bool isResult = false)
      {
         if (!isResult)
         {
            UInt32 totalSize = (UInt32)(data.Length + kErrorCheckSize);
            Debug.Assert(totalSize <= kMaxPacketSize);
            fPacket = new byte[totalSize];
            Buffer.BlockCopy(data, 0, fPacket, 0, data.Length);
         }
         else
         {
            fPacket = data;
         }
      }

      // Reading packet data

      public byte[] Data()
      {
         return fPacket;
      }


      public byte GetCommand()
      {
         return fPacket[(int)Offsets.kCommandOffset];
      }


      public byte GetAddress()
      {
         return fPacket[(int)Offsets.kAddressOffset];
      }


      /// <returns> The 3rd byte in the packet contains the number of bytes which follow
      /// when the packet has variable length data. This will not be valid until the result
      /// is actually read.</returns>
      public byte GetResultLength()
      {
         return fPacket[(int)Offsets.kResultLengthOffset];
      }


      public UInt16 ValueAtOffset(int offset)
      {
         UInt16 value = (UInt16)(fPacket[offset] << 8);
         value |= (UInt16)(fPacket[offset + 1]);
         return value;
      }


      public UInt16[] ValuesAtOffset(int offset, int quantity)
      {
         UInt16[] values = new UInt16[quantity];
         for (int i = 0; i < quantity; ++i, offset += sizeof(UInt16))
         {
            values[i] = this.ValueAtOffset(offset);
         }
         return values;
      }


      public byte ByteAtOffset(int offset)
      {
         return fPacket[offset];
      }


      public byte[] BytesAtOffset(int offset, int quantity)
      {
         byte[] values = new byte[quantity];
         Buffer.BlockCopy(fPacket, offset, values, 0, quantity);
         return values;
      }

      // Writing packet data

      /// <summary>
      /// Set the eror check bits. This must be called after all data is in the packet 
      /// and before attempting to send the packet.
      /// </summary>
      public void CalculateErrorChecks()
      {
         int pduLen = fPacket.Length - kErrorCheckSize;
         UInt16 crc = Checksum.Crc16(fPacket, pduLen);
         this.WriteChecksum(crc);
      }


      public bool ValidChecksum()
      {
         int pduLen = fPacket.Length - kErrorCheckSize;
         UInt16 crc = Checksum.Crc16(fPacket, pduLen);
         // CRC as calculated is byte swapped. So don't reswap the bytes
         UInt16 storedCrc = (UInt16)(fPacket[pduLen] | fPacket[pduLen + 1] << 8);
         return (crc == storedCrc);
      }


      /// <param name="data">The data to copy</param>
      /// <param name="offset">The index of the first byte in the packet to write</param>
      public void CopyToPacket(byte[] data, int offset)
      {
         Debug.Assert((offset + data.Length) <= (fPacket.Length - kErrorCheckSize));
         Buffer.BlockCopy(data, 0, fPacket, offset, data.Length);
      }

      /// <param name="data"></param>
      /// <param name="offset"></param>
      public void CopyToPacket(UInt16 data, int offset)
      {
         Debug.Assert(offset + sizeof(UInt16) <= (fPacket.Length - kErrorCheckSize));

         // ModBus expects big endian.
         this.WriteByteSwapped(data, offset);
      }

      public void CopyToPacket(UInt16[] data, int offset)
      {
         foreach(UInt16 value in data)
         {
            this.CopyToPacket(value, offset);
            offset += sizeof(UInt16);
         }
      }


      public byte ExceptionCodeForCommand()
      {
         int command = (int)this.GetCommand();
         return (byte)(command + kErrorCommandMask);
      }


      public void SetPayload(byte[] data)
      {
         Debug.Assert(data.Length <= (fPacket.Length - kAddressAndCommandSize - kErrorCheckSize));

         Buffer.BlockCopy(data, 0, fPacket, (int)(Offsets.kCommandOffset + 1), data.Length);
         this.CalculateErrorChecks();
      }


      public override string ToString()
      {
         var bufferString = BitConverter.ToString(fPacket, 0, Math.Min(16, fPacket.Length));
         if (fPacket.Length > 16)
         {
            bufferString += "...";
         }
         return bufferString;
      }


      private static int PacketSizeForCommand(byte command, byte pduSize)
      {
         return kAddressAndCommandSize + pduSize + kErrorCheckSize;
      }


      private void SetAddress(byte address)
      {
         fPacket[(int)Offsets.kAddressOffset] = address;
      }


      private void SetCommand(byte command)
      {
         fPacket[(int)Offsets.kCommandOffset] = command;
      }

      private void SetResultLength(byte length)
      {
         fPacket[(int)Offsets.kResultLengthOffset] = length;
      }


      private void AllocatePacket(int totalSize)
      {
         Debug.Assert(totalSize <= kMaxPacketSize);
         fPacket = new byte[totalSize];
      }


      private void WriteByteSwapped(UInt16 data, int offset)
      {
         fPacket[offset] = (byte)(data >> 8);
         fPacket[offset + 1] = (byte)(data & 0xFF);
      }


      private void WriteChecksum(UInt16 checksum)
      {
         // Checksum bytes are expected to be swapped already due to the reference
         // implementation in the Modbus specs.
         int offset = fPacket.Length - kErrorCheckSize;
         fPacket[offset] = (byte)(checksum & 0xFF);
         fPacket[offset + 1] = (byte)(checksum >> 8);
      }

      protected enum Offsets
      {
         kAddressOffset,
         kCommandOffset,
         kResultLengthOffset, // usually only present for variable length packets
         kVariableSizePacketResultOffset
      }

      /// <summary>
      /// Check a result's command to see if this bit is set
      /// </summary>
      public const byte kErrorCommandMask = 0x80;

      protected byte[] fPacket;
   }


   /// <summary>
   /// A packet which indicates execution of command encountered an error
   /// </summary>
   public class AErrorPacket : APacket
   {
      /// <param name="command">The command code which failed to be executed successfully</param>
      /// <param name="exceptionCode">A code indicating the error encountered</param>
      public AErrorPacket(byte command, byte exceptionCode)
      : base(0, (byte)(command + kErrorCommandMask), 1)
      {
         fPacket[kAddressAndCommandSize] = exceptionCode;
      }
   }


   public enum CommandCode
   {
      kReadDiscreteInputs = 0x02,
      kReadCoils = 0x01,
      kWriteSingleCoil = 0x05,
      kWriteMultipleCoils = 0x0F,
      kReadInputRegisters = 0x04,
      kReadHoldingRegisters = 0x03,
      kWriteSingleRegister = 0x06,    // holding register
      kWriteMultipleRegisters = 0x10, // holding registers

      // Below as yet are needed.

      //kReadWriteMultipleRegisters = 0x17,
      //kMaskWriteRegister = 0x16,
      //kReadFifoQueue = 0x18,
      //kReadFileRecord = 0x14,
      //kWriteFileRecord = 0x15,
      //kReadExceptionStatus = 0x07,
      //kDiagnostic = 0x08,
      //kGetComEventCounter = 0x0B,
      //kGetComEventLog = 0x0C,
      //kReportServerId = 0x11,
      //kReadDeviceIdentification = 0x2B, // subcode 14
      //kEncapsulatedInterfaceTransport = 0x2B, // subcodes 13,14
      //kCANOpenGeneralReference = 0x2B, // subcode 13

      // Application defined codes
      kReadUserRegisters = 0x43,
      kWriteUserRegisters = 0x42,
      kWriteFunctionRegisters = 0x44,
   }


   public enum ExceptionCode
   {
      kNone = 0,
      kIllegalFunction = 1,
      kIllegalDataAddress = 2,
      kIllegalDataValue = 3,
      kServerDeviceFailure = 4,
      kAcknowledge = 5,
      kServerDeviceBusy = 6,
      kMemoryParityError = 8,
      kGatewayPathUnavailable = 0x0A,
      kGatewayTargetDeviceFailedToRespond = 0x0B
      // 4,5,6 indicate error executing code
   }

}

