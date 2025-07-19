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
using System.Diagnostics;
using System.Threading.Tasks;


namespace ModBus
{
   /// <summary>
   /// ModBus addresses conform to these constraints.
   /// </summary>
   public enum Address
   {
      kBroadcast = 0,
      kFirstAddress = 1,
      kMaxAddress = 32 // technically it's 247, but the dsi net maxes at 32.
   }


   /// <summary>
   /// Interface for interacting with objects conforming to ModBus_Application_Protocal_1_1b3(1).pdf,
   /// found here
   ///    https://drive.google.com/open?id=0B2esmYEHmVkcZ3FLMmRtUHZzeFl2aTFVeVNWbVlYZGo1UXNr&authuser=0
   /// </summary>
   /// 
   /// <h2>Addressing</h2>
   /// 
   /// Registers are addressed starting at zero.  Therefore input registers number 1 addressed as 0.
   /// Coils are addressed starting at zero. Therefore coil number 1 is addressed as 0.
   /// Discrete inputs are addressed starting at zero. Therefore input number 1 is addressed as 0.
   /// 
   /// <h2>Layout</h2>
   /// 
   /// Discrete Inputs - bitwise access to up to 2000 readonly bits of memory
   /// Coils - bitwise access to up to 2000 bits of memory
   /// Input Register - read only registers
   /// Holding Register - read/write registers
   /// 
   public interface IModBusDevice
   {
      /// <summary>
      /// Zero all values in memory/storage. (Non spec addition for testing purposes)
      /// </summary>
      void Reset();

      // !!!: Temporarily disabling some to make implementing subclass easier at first.

      /// <summary>
      /// Read from 1 to 2000 contiguous status of coils in a remote device.
      /// </summary>
      /// <param name="address">the address of the first input specified</param>
      /// <param name="quantity">the number of inputs</param>
      /// <returns></returns>
      byte[] ReadCoils(byte netAddress, UInt16 address, UInt16 quantity);


      /// <summary>
      /// Read from 1 to 2000 contiguous status of discrete inputs in a remote device.
      /// </summary>
      /// <param name="address">The starting bit address</param>
      /// <param name="quantity">The number of bits to read</param>
      /// <returns></returns>
      byte[] ReadDiscreteInputs(byte netAddress, UInt16 address, UInt16 quantity);

      /// <summary>
      /// Read the contents of a contiguous block of holding registers in a remote device.
      /// </summary>
      /// <param name="address">The starting register address</param>
      /// <param name="quantity"The number of registers></param>
      /// <returns></returns>
      UInt16[] ReadHoldingRegisters(byte netAddress, UInt16 address, UInt16 quantity);

      /// <summary>
      /// This reads from 1 to 125 contiguous input registers in a remote device.
      /// </summary>
      /// <param name="address">The starting register address.</param>
      /// <param name="quantity">The number of registers</param>
      /// <returns></returns>
      UInt16[] ReadInputRegisters(byte netAddress, UInt16 address, UInt16 quantity);

  
      /// <summary>
      /// This function code is used to write a single output to either ON or OFF in a
      /// remote device. The requested ON/OFF state is specified by a constant in the
      /// request data field. 
      /// </summary>
      /// <param name="address">The bit to write</param>
      /// <param name="value">A value of 0xFF00 requests the output to be ON. 
      /// A value of 0x0000 requests it to be OFF. All other values are illegal and 
      /// will not affect the output.</param>
      /// <returns>The written address on success, or the value 0x85 in the high byte and
      /// an exception code in the low byte on error</returns>
      UInt16 WriteSingleCoil(byte netAddress, UInt16 address, UInt16 value);

      /// <summary>
      /// Write to a single holding register in a remote device.
      /// </summary>
      /// <param name="address">The register address.</param>
      /// <param name="value">The data to write.</param>
      /// <returns>The address and value echoed back from the device.</returns>
      Tuple<UInt16, UInt16> WriteSingleRegister(byte netAddress, UInt16 address, UInt16 value);

      /// <summary>
      /// Write to multiple holding registers in a single request.
      /// </summary>
      /// <param name="address">The starting register address</param>
      /// <param name="quantity">The number of registers to write</param>
      /// <param name="outputValues">The data to write to the registers. It must have at least `quantity` values</param>
      /// <returns>The address and quantity written, or if outputValues does not validate (0x90, 2)</returns>
      Tuple<UInt16, UInt16> WriteMultipleRegisters(byte netAddress, UInt16 address, UInt16 quantity, UInt16[] outputValues);

      /// <summary>
      /// Write to multiple holding registers in a single request.
      /// </summary>
      /// <param name="address">The starting register address</param>
      /// <param name="quantity">The number of registers to write</param>
      /// <param name="outputValues">The data to write to the registers. It must have at least `quantity` values</param>
      /// <returns>The address and quantity written, or if outputValues does not validate (0x90, 2)</returns>
      Tuple<UInt16, UInt16> WriteFunctionRegisters(byte netaddress, UInt16 address, UInt16 quantity, UInt16[] outputValues);

      //byte[] WriteFileRecord(UInt16 fileNumber, UInt16 recordNumber, UInt16 recordLength, byte[] recordData);

      /// <summary>
      /// Force each coil in a sequence of coils to either ON or OFF in a remote device.
      /// The Request PDU specifies the coil references to be forced. 
      /// </summary>
      /// <param name="address">The starting coil address</param>
      /// <param name="quantity">The number of coils to set</param>
      /// <param name="outputValues">The requested ON/OFF states are specified by contents of the
      /// request data field. A logical ' 1' in a bit position of the field requests the corresponding
      /// output to be ON. A logical '0' requests the corresponding output to be OFF.</param>
      /// <returns>The normal response returns the function code, starting address, and quantity of
      /// coils forced.</returns>
      Tuple<UInt16, UInt16> WriteMultipleCoils(byte netAddress, UInt16 address, UInt16 quantity, byte[] outputValues);

      // MODBUS protocol allows multipe subrequests as long as the returns data does not exceed 253 bytes, 
      // but for now let's try the simple form of one request.
      //byte[] ReadFileRecord(UInt16 fileNumber, UInt16 recordNumber, UInt16 recordLength);

      //byte[] ReadFifoQueue(UInt16 fifoPointerAddress);

      //byte[] MaskWriteRegister(UInt16 address, UInt16 andMask, UInt16 orMask);

      //byte[] WriteReadMultipleRegisters(UInt16 readStart, UInt16 readQuantity,
      //                                        UInt16 writeStart, UInt16 writeQuantity,
      //                                        byte[] writeData);


      /// <summary>
      /// This function code is used to read the description of the type, the current status,
      /// and other information specific to a remote device. The format of a normal response 
      /// is shown in the following example. The data contents are specific to each type of device.
      /// </summary>
      /// <returns> A tuple with the device ID packed bytewise in a list,
      /// running status (0x00 = ON or 0x01 = OFF) ,
      /// and any app specific additional data in the second list. </returns>
      //Tuple<byte[],byte,byte[]> ReportServerId();

      //UInt16 ReadExceptionStatus();
      //byte[] Diagnostics(byte subfunction, byte what, byte[] data);
      //byte[] GetCommEventCounter();

      // Generic/Application defined commands
      // !!! Do these belong here since they are DSI specific?

      /// <summary>
      /// Read data from the User registers
      /// </summary>
      /// <param name="netAddress">modbus address</param>
      /// <param name="address">register address</param>
      /// <param name="quantity">number of registers to read</param>
      /// <returns>The values read</returns>
      UInt16[] ReadUserRegisters(byte netAddress, UInt16 address, UInt16 quantity);

      /// <summary>
      /// Write data to the User registers
      /// </summary>
      /// <param name="netAddress">modus address</param>
      /// <param name="address">register address</param>
      /// <param name="quantity">number of registers to write</param>
      /// <param name="outputValues">The command and number of registers read or command + 0x80 and
      /// an exception code if there was an error.</param>
      /// <returns></returns>
      Tuple<UInt16, UInt16> WriteUserRegisters(byte netAddress, UInt16 address, 
       UInt16 quantity, UInt16[] outputValues);
   }


   /// <summary>
   /// An object which can handle modbus packets.
   /// </summary>
   public interface IPacketHandler
   {
      /// <summary>
      /// Send a packet and return the resulting packet asynchronously.
      /// </summary>
      /// <param name="packet">The command packet</param>
      /// <param name="expectedResultBytes">The number of bytes to be read for the result of
      /// sending packet.</param>
      /// <returns>The result packet such as one returned from a device that handled 
      /// the command</returns>
      APacket HandlePacket(APacket packet, int expectedResultBytes, int retryCount = 0);
   }


   /// !!!: Move to DsiSim.cs?
   /// <summary>
   /// Description of physical memory layout of the remote device such as where user registers
   /// being in the address space.
   /// </summary>
   /// Each offset is a *byte* offset in the device's memory even though 
   /// Modbus memory is word or sometimes bit addressable.
   public class APhysicalMemoryLayout
   {
      public int ByteOffsetForCoil(UInt16 bitAddress)
      {
         return this.ByteOffsetForBits(fStartCoils, bitAddress);
      }


      public int ByteOffsetForDiscreteInput(UInt16 bitAddress)
      {
         return this.ByteOffsetForBits(fStartDiscreteInputs, bitAddress);
      }


      public int CoilsByteLength()
      {
         return this.NumBytesForBitCount(fNumCoils);
      }


      public int DiscreteInputsByteLength()
      {
         return this.NumBytesForBitCount(fNumDiscreteInputs);
      }


      public int OffsetForHoldingRegister(UInt16 address)
      {
         int offset = fStartHoldingRegisters + address * sizeof(UInt16);
         return offset;
      }


      public int OffsetForInputRegister(UInt16 address)
      {
         int offset = fStartInputRegisters + address * sizeof(UInt16);
         return offset;
      }


      public int OffsetForUserRegister(UInt16 address)
      {
         int offset = fStartUserRegisters + address * sizeof(UInt16);
         return offset;
      }


      private int ByteOffsetForBits(int byteOffset, UInt16 bitAddress)
      {
         // Floor of byte offset containing the coil address.
         int offset = byteOffset + bitAddress / 8;
         return offset;
      }


      private int NumBytesForBitCount(int bitCount)
      {
         // Number of complete bytes containing the coil bits.
         return bitCount / 8 + ((bitCount % 8) > 0 ? 1 : 0);
      }

      public int fStartDiscreteInputs;
      public int fNumDiscreteInputs; // number of input bits
      public int fStartCoils;
      public int fNumCoils; // number of bits of coil memory
      public int fStartInputRegisters;
      public int fStartHoldingRegisters;
      public int fStartUserRegisters;
   };
}

