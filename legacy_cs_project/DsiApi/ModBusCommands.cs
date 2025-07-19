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


namespace ModBus
{
   /// <summary>
   /// Minimal interface for dealing with a command.
   /// </summary>
   public interface ICommand
   {
      bool Execute(IPacketHandler handler);
      void ResultData<ResultT>(out ResultT data);
   }


   /// <summary>
   /// A command encapsulates packet building and result extraction from APackets.
   /// Actual execution of the command is handled through IPackeHandler implementations which
   /// are expected to execute asynchronously (on another thread or process eventually
   /// so that we can block while waiting for the result).
   /// </summary>
   public class ACommand : ICommand
   {
      /// <summary>
      /// Ask the packet handler to handle the packet and wait for the result.
      /// </summary>
      /// <returns>True if the command transmitted successfully</returns>
      public bool Execute(IPacketHandler handler)
      {
         // Call CalculateErrorChecks here to not have to remember to do it during packet 
         // building. This is the last moment it can possibly be done and no data will
         // change in the packet after this.
         fCommand.CalculateErrorChecks();

         fResult = handler.HandlePacket(fCommand, this.ExpectedResultBytes());
         if (this.HasResult && !this.ResultIsGood)
         {
            throw new AModBusException(this.ResultExceptionCode, this.CommandPacket);
         }

         return this.ResultIsGood;
      }


      virtual public int ExpectedResultBytes()
      {
         // Subclasses should override. This is a very large amount which should
         // cause a timeout if the subclass hasn't overriden this.
         return 40000;
      }


      public void ResultData<ResultT>(out ResultT data)
      {
         // Subclass should specialize for its particular result type
         data = default(ResultT);
      }


      public bool HasResult 
      { 
         get 
         { 
            return (null != fResult && fResult.ValidChecksum()); 
         } 
      }

      public bool ResultIsGood
      {
         get
         {
            bool transmissionSucceeded = this.HasResult;
            bool commandSucceeded = true;
            if (transmissionSucceeded)
            {
               commandSucceeded = (0 == (fResult.GetCommand() & APacket.kErrorCommandMask));
               if (!commandSucceeded)
               {
                  fExceptionCode = (ExceptionCode)fResult.Data()[APacket.kAddressAndCommandSize];
               }
            }
            return  transmissionSucceeded && commandSucceeded;
         }
      }

      /// <summary>
      /// Check this if ResultIsGood returns false.
      /// </summary>
      /// <returns> If the exception is ExceptionCode.kNone, the checksum validation failed.
      /// Otherwise, this is the exception returned with the result.</returns>
      public ExceptionCode ResultExceptionCode { get { return fExceptionCode; } }

      public APacket CommandPacket { get { return fCommand; } }

      public APacket ResultPacket { get { return fResult; } }

      /// <summary>
      /// Helper for subclasses which need to read out variable length data which is preceeded
      /// by a byte giving the length of the data.
      /// </summary>
      /// <param name="data">A reference to receive the array of bytes from the packet</param>
      protected void VariableLengthResultData(out byte[] data)
      {
         byte amount = fResult.GetResultLength();
         data = fResult.BytesAtOffset(APacket.kVariableLengthResultOffset, amount);
      }

      /// <summary>
      /// Helper for subclasses which need to read out variable length data which is preceeded
      /// by a byte giving the length of the data.
      /// </summary>
      /// <param name="data">A reference to receive the array of shorts from the packet</param>
      protected void VariableLengthResultData(out  UInt16[] data)
      {
         int amount = fResult.GetResultLength() / sizeof(UInt16);
         data = fResult.ValuesAtOffset(APacket.kVariableLengthResultOffset, (byte)amount);
      }
      
      protected APacket fCommand;
      protected APacket fResult;
      protected ExceptionCode fExceptionCode = ExceptionCode.kNone;
   }


   // Modbus protocol implementations


   public class AReadCoilsCommand : AReadBitsCommand
   {
      public AReadCoilsCommand(byte netAddress, UInt16 address, UInt16 quantity)
       : base(netAddress, CommandCode.kReadCoils, address, quantity)
      {
      }
   }


   public class AReadDiscreteInputsCommand : AReadBitsCommand
   {
      public AReadDiscreteInputsCommand(byte netAddress, UInt16 address, UInt16 quantity)
       : base(netAddress, CommandCode.kReadDiscreteInputs, address, quantity)
      {
      }
   }


   public class AReadHoldingRegistersCommand : AReadUInt16sCommand
   {
      public AReadHoldingRegistersCommand(byte netAddress, UInt16 address, UInt16 quantity)
       : base(netAddress, CommandCode.kReadHoldingRegisters, address, quantity)
      {
      }
   }


   public class AReadInputRegistersCommand : AReadUInt16sCommand
   {
      public AReadInputRegistersCommand(byte netAddress, UInt16 address, UInt16 quantity)
       : base(netAddress, CommandCode.kReadInputRegisters, address, quantity)
      {
      }
   }


   public class AReadUserRegistersCommand : AReadUInt16sCommand
   {
      public AReadUserRegistersCommand(byte netAddress, UInt16 address, UInt16 quantity)
       : base(netAddress, CommandCode.kReadUserRegisters, address, quantity)
      {
      }
   }


   public class AWriteFunctionRegistersCommand : AWriteRegistersCommand
   {
      public AWriteFunctionRegistersCommand(byte netAddress, UInt16 address,
       UInt16 quantity, UInt16[] outputValues)
       : base(netAddress, CommandCode.kWriteFunctionRegisters, address, quantity, outputValues)
      {
      }
   }


   public class AWriteMultipleCoilsCommand : AWriteCoilsCommand
   {
      public AWriteMultipleCoilsCommand(byte netAddress, UInt16 address,
       UInt16 quantity, byte[] outputValues)
       : base(netAddress, address, quantity, outputValues)
      {
      }
   }


   public class AWriteMultipleRegistersCommand : AWriteRegistersCommand
   {
      public AWriteMultipleRegistersCommand(byte netAddress, UInt16 address, 
       UInt16 quantity, UInt16[] outputValues)
       : base(netAddress, CommandCode.kWriteMultipleRegisters, address, quantity, outputValues)
      {
      }
   }


   public class AWriteUserRegistersCommand : AWriteRegistersCommand
   {
      public AWriteUserRegistersCommand(byte netAddress, UInt16 address,
       UInt16 quantity, UInt16[] outputValues)
       : base(netAddress, CommandCode.kWriteUserRegisters, address, quantity, outputValues)
      {
      }
   }


   public class AWriteSingleCoilCommand : AWriteCoilsCommand
   {
      public AWriteSingleCoilCommand(byte netAddress, UInt16 address, UInt16 value)
       : base(netAddress, address, value)
      {
      }
   }


   public class AWriteSingleRegisterCommand : AWriteRegistersCommand
   {
      public AWriteSingleRegisterCommand(byte netAddress, UInt16 address, UInt16 value)
         : base(netAddress, address, value)
      {
      }
   }


   // Support classes

   /// <summary>
   /// A few commands have essentialy the same send and result packet formats. This
   /// one will request a number of BITS to read, and return the bytes read in.
   /// 
   /// This is not intended to be used directly, see one of its subclasses.
   /// </summary>
   public class AReadBitsCommand : ACommand
   {
      public AReadBitsCommand(byte netAddress, CommandCode command, UInt16 address, UInt16 quantity)
      {
         this.ConfigureCommand(netAddress, command, address, quantity);
      }


      public override int ExpectedResultBytes()
      {
         UInt16 numBytes = fCommand.ValueAtOffset(APacket.kAddressAndCommandSize + sizeof(UInt16));
         return APacket.kAddressAndCommandSize + 1 + numBytes + APacket.kErrorCheckSize;
      }


      private void ConfigureCommand(byte netAddress, CommandCode command, UInt16 address, UInt16 quantity)
      {
         byte size = (byte)(sizeof(UInt16) * 2);
         fCommand = new APacket(netAddress, (byte)command, size);

         int offset =  APacket.kAddressAndCommandSize;
         fCommand.CopyToPacket(address, offset);
         offset += sizeof(UInt16);
         fCommand.CopyToPacket(quantity, offset);
      }


      public void ResultData(out byte[] data)
      {
         this.VariableLengthResultData(out data);
      }
   }


   /// <summary>
   /// A command which reads a number of UInt16 values. Not for direct use.
   /// </summary>
   public class AReadUInt16sCommand : ACommand
   {
      public AReadUInt16sCommand(byte netAddress, CommandCode command, UInt16 address, UInt16 quantity)
      {
         this.ConfigureCommand(netAddress, command, address, quantity);
      }


      override public int ExpectedResultBytes()
      {
         UInt16 quantity = fCommand.ValueAtOffset(APacket.kAddressAndCommandSize + sizeof(UInt16));
         const int kCountBytes = 1;
         return APacket.kAddressAndCommandSize + kCountBytes + quantity * sizeof(UInt16) +
          APacket.kErrorCheckSize;
      }


      static public int MaxPayload { get { return 248; /*!!!: fudging number until I find a better way. APacket.kMaxPduSize - 1;*/ } } // less one byte for the count of bytes returned.


      private void ConfigureCommand(byte netAddress, CommandCode command, UInt16 address, UInt16 quantity)
      {
         byte size = (byte)(sizeof(UInt16) * 2);
         fCommand = new APacket(netAddress, (byte)command, size);

         int offset = APacket.kAddressAndCommandSize;
         fCommand.CopyToPacket(address, offset);
         offset += sizeof(UInt16);
         fCommand.CopyToPacket(quantity, offset);
      }


      public void ResultData(out UInt16[] data)
      {
         this.VariableLengthResultData(out data);
      }
   }

   /// <summary>
   /// Write to one or more bits, aka coils. Not for direct use.
   /// </summary>
   public class AWriteCoilsCommand : ACommand
   {
      /// <summary>
      /// Write multiple bits starting at a given address, Command.kWriteMultipleCoils.
      /// </summary>
      /// <param name="netAddress"></param>
      /// <param name="address"></param>
      /// <param name="quantity">The number of bits to force</param>
      /// <param name="values"></param>
      public AWriteCoilsCommand(byte netAddress, UInt16 address, UInt16 quantity, byte[] values)
      {
         this.ConfigureCommand(netAddress, address, quantity, values);
      }

 
      /// <summary>
      /// Write single bit at a given address, Command.kWriteSingleCoil.
      /// </summary>
      /// <param name="netAddress"></param>
      /// <param name="address">Offset of bit</param>
      /// <param name="value">0x00 to clear the bit or 0xFF00 to set the bit</param>
      public AWriteCoilsCommand(byte netAddress, UInt16 address, UInt16 value)
      {
         this.ConfigureCommand(netAddress, address, value);
      }


      override public int ExpectedResultBytes()
      {
         return 8; // fixed for all write commands
      }


      private void ConfigureCommand(byte netAddress, UInt16 address,
       UInt16 quantity, byte[] values)
      {
         byte memAddressAndQuantitySize = sizeof(UInt16) * 2;
         byte countSize = 1;
         byte size = (byte)(memAddressAndQuantitySize + countSize +
          values.Length);

         fCommand = new APacket(netAddress, (byte)CommandCode.kWriteMultipleCoils, size);

         int offset =  APacket.kAddressAndCommandSize;

         fCommand.CopyToPacket(address, offset);
         offset += sizeof(UInt16);

         fCommand.CopyToPacket(quantity, offset);
         offset += sizeof(UInt16);

         fCommand.Data()[offset] = (byte)values.Length;
         offset += 1;

         fCommand.CopyToPacket(values, offset);
      }


      private void ConfigureCommand(byte netAddress, UInt16 address, UInt16 value)
      {
         byte size = (byte)(sizeof(UInt16) * 2);
         fCommand = new APacket(netAddress, (byte)CommandCode.kWriteSingleCoil, size);

         int offset = APacket.kAddressAndCommandSize;
         fCommand.CopyToPacket(address, offset);
         offset += sizeof(UInt16);
         fCommand.CopyToPacket(value, offset);
      }


      /// <summary>
      /// Write Multiple Coils result
      /// </summary>
      public void ResultData(out Tuple<UInt16, UInt16> data)
      {
         UInt16 address = fResult.ValueAtOffset(kResultAddressOffset);
         UInt16 quantity = fResult.ValueAtOffset(kResultAmountOffset);
         data = new Tuple<UInt16, UInt16>(address, quantity);
      }

      /// <summary>
      /// Write Single Coil result
      /// </summary>
      public void ResultData(out UInt16 address)
      {
         address = fResult.ValueAtOffset(kResultAddressOffset);
      }

      private const int kResultAddressOffset = APacket.kAddressAndCommandSize;
      private const int kResultAmountOffset = kResultAddressOffset + 2;
   }

   /// <summary>
   /// Write one or more registers. Not for direct use.
   /// </summary>
   public class AWriteRegistersCommand : ACommand
   {
      /// <summary>
      /// Construct a packet which will use a command other than kWriteMultipleRegisters
      /// but still works with register sized data.
      /// </summary>
      /// <param name="command">kWriteRegistersCommand or an application defined command
      /// for a command which manipulates registers</param>
      public AWriteRegistersCommand(byte netAddress, CommandCode command, UInt16 address,
      UInt16 quantity, UInt16[] outputValues)
      {
         this.ConfigureCommand(netAddress, command, address, quantity, outputValues);
      }


      public AWriteRegistersCommand(byte netAddress, UInt16 address, UInt16 value)
      {
         this.ConfigureCommand(netAddress, address, value);
      }


      override public int ExpectedResultBytes()
      {
         return 8; // fixed for all writes; it just returns the number of registers read
      }


      private static int kPayloadHeaderSize = 2 * sizeof(UInt16) + 1;
      public static int MaxPayload { get { return APacket.kMaxPduSize - kPayloadHeaderSize; /* !!!: Fudging the number until I find a better way. */ } }


      private void ConfigureCommand(byte netAddress, CommandCode command, UInt16 address,
       UInt16 quantity, UInt16[] outputValues)
      {
         //???: Then why pass in quantity?
         Debug.Assert(outputValues.Length == quantity);

         int outputByteLength = sizeof(UInt16) * quantity;
         byte size = (byte)(kPayloadHeaderSize + outputByteLength);
         fCommand = new APacket(netAddress, (byte)command, size);

         int offset = APacket.kAddressAndCommandSize;
         fCommand.CopyToPacket(address, offset);
         offset += sizeof(UInt16);

         fCommand.CopyToPacket(quantity, offset);
         offset += sizeof(UInt16);

         fCommand.Data()[offset] = (byte)outputByteLength;
         offset += 1;

         fCommand.CopyToPacket(outputValues, offset);
      }


      private void ConfigureCommand(byte netAddress, UInt16 address, UInt16 value)
      {
         byte size = (byte)(sizeof(UInt16) * 2);
         fCommand = new APacket(netAddress, (byte)CommandCode.kWriteSingleRegister, size);

         int offset = APacket.kAddressAndCommandSize;
         fCommand.CopyToPacket(address, offset);
         offset += sizeof(UInt16);
         fCommand.CopyToPacket(value, offset);
      }


      public void ResultData(out Tuple<UInt16, UInt16> data)
      {
         UInt16 address = fResult.ValueAtOffset(kResultAddressOffset);
         UInt16 quantity = fResult.ValueAtOffset(kResultQuantityOrValueOffset);
         data = new Tuple<UInt16, UInt16>(address, quantity);
      }

      private const int kResultAddressOffset = APacket.kAddressAndCommandSize;
      private const int kResultQuantityOrValueOffset = kResultAddressOffset + 2;
   }
}

