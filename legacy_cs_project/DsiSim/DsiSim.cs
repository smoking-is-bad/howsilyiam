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
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Text;
using System.Timers;

using DsiApi;
using DsiExtensions;
using Logging;
using ModBus;
using MockData;
using Model;


namespace DsiSim
{

   //!!!: Rename to ADsiSimFile
   /// <summary>
   /// Simulator of remote device using a file for memory backing. Also simulates some
   /// DSI behavior. It mostly matches IModBusDevice, but it doesn't take the netAddress parameter 
   /// in the IModBus methods.
   /// </summary>
   public class ADsiSim
   {
		static public ADsiSim CreateDsiSim(string path, byte address, bool reset)
		{
			var dsi = new ADsiSim(path);
			if (reset)
			{
				dsi.Reset(); 
			}
			dsi.WriteUserRegisters((UInt16)AMemoryLayout.UserRegisters.kModBusAddress, 0x0001,
				new UInt16[] { (UInt16)address });
			return dsi;
		}


		static public void CreateSimFiles(string networkPath, int numDsis, bool reset)
		{
			string[] pathComponents = { networkPath, "", };
			byte address = (byte)Address.kFirstAddress;
			for (int n = 0; n < numDsis; ++n)
			{
				pathComponents[1] = address.ToString() + ADsiSim.kExtension;
				string path = Path.Combine(pathComponents);

				ADsiSim.CreateDsiSim(path, address, reset);

				++address;
			}
		}


      public ADsiSim(string backingFilePath)
      {
         fRng = new Random();

         fTracer = new ATracer((string format, object[] args) =>
#if qTraceSimWrites
          {
             ALog.Info("DsiSim", format, args);
          });
#else
          {
             // empty
          });
#endif

         fBackingFilePath = backingFilePath;
         this.MakeMemoryLayout();
         this.EnableProbeFiring = true;
      } 

      /// <summary>
      /// Set to true to allow setting coil address 0 to 1 to simulate a probe firing.
      /// Defaults to true. This isn't true DSI functionality. It aids unit testing.
      /// </summary>
      public bool EnableProbeFiring { get; set; }

      /// <summary>
      /// Move the backing file to the new address.
      /// </summary>
      /// <param name="newAddress">The new modbus id</param>
      /// <returns>true on success, false otherwise</returns>
      public bool MoveTo(byte newAddress)
      {
         bool success = true;
         string currentPath = fBackingFilePath;
         string parent = Directory.GetParent(fBackingFilePath).ToString();
         fBackingFilePath = Path.Combine(parent, newAddress.ToString() + kExtension);
         if (currentPath != fBackingFilePath)
         {
            try
            {
               File.Move(currentPath, fBackingFilePath);
            }
            catch (Exception ex)
            {
               ALog.Error("DsiSim", "Failed to move DSI simulator backing file:" + ex.Message);
               success = false;
            }
         }
         return success;
      }


      public byte[] ReadCoils(UInt16 address, UInt16 quantity)
      {
         byte[] coils = this.ReadCoils();
         return this.BitFieldSubset(coils, address, quantity);
      }


      public byte[] ReadDiscreteInputs(UInt16 address, UInt16 quantity)
      {
         byte[] inputs = this.ReadDiscreteInputs();
         return this.BitFieldSubset(inputs, address, quantity);
      }


      public UInt16[] ReadHoldingRegisters(UInt16 address, UInt16 quantity)
      {
         return this.ReadRegisters(fMemoryLayout.OffsetForHoldingRegister(address), quantity);
      }


      public UInt16[] ReadUserRegisters(UInt16 address, UInt16 quantity)
      {
         UInt16[] registers = this.ReadRegisters(fMemoryLayout.OffsetForUserRegister(address), quantity);
         
         // Simulate factory installed ID.
         if ((UInt16)AMemoryLayout.UserRegisters.kModBusAddress == address &&
            0x0000 == registers[0])
         {
            registers[0] = ADsiNetwork.kFactoryId;
            this.WriteUserRegisters(address, quantity, registers);
         }

         return registers;
      }


      public UInt16[] ReadInputRegisters(UInt16 address, UInt16 quantity)
      {
         // Simulate firmwar clearing data ready after the ascan data is read.
         this.ClearDataReady();

         if (!this.EnableProbeFiring)
         {
            return this.ReadRegisters(fMemoryLayout.OffsetForInputRegister(address), quantity);
         }
         else
         {
            UInt16[] result = new UInt16[quantity];
#if false
            // Random generated data.
            Int16[] ascan = AAscan.MockData(quantity);
            Buffer.BlockCopy(ascan, 0, result, 0, result.Length);
#else
            // Block copy results in all zeros, but 'manually' converting works.
            Int16[] ascan = AAscan.PresetData();
            address %= AMemoryLayout.kAscanSectionOffset;
            for (int i = 0; i < result.Length; ++i)
            {
               var bytes = BitConverter.GetBytes(ascan[address + i]);
               result[i] = BitConverter.ToUInt16(bytes, 0);
            }
#endif
            return result;
         }
      }


      public void Reset()
      {
         if (File.Exists(fBackingFilePath))
         {
            
            File.Delete(fBackingFilePath);
         }
      }


      /// <summary>
      /// This isn't possible with physical hardware, but is useful for debugging and hacking 
      /// in the sim file.
      /// </summary>
      /// <param name="address"></param>
      /// <param name="quantity"></param>
      /// <param name="outputValues"></param>
      /// <returns></returns>
      public Tuple<UInt16, UInt16> WriteInputRegisters(UInt16 address, UInt16 quantity,
         UInt16[] outputValues)
      {
         return this.PerformWriteOperation<Tuple<UInt16, UInt16>>((BinaryWriter writer) =>
         {
            Tuple<UInt16, UInt16> result;

            int byteOffset = fMemoryLayout.OffsetForInputRegister(address);

            writer.Seek(byteOffset, SeekOrigin.Begin);
            byte[] converted = new byte[quantity * sizeof(UInt16)];
            Buffer.BlockCopy(outputValues, 0, converted, 0, converted.Length);

            // Report register addresses
            fTracer.Trace("WIR:0x{0} - 0x{1}", (byteOffset / 2).ToString("X"),
             ((byteOffset + converted.Length) / 2).ToString("X"));

            writer.Write(converted);
            writer.Close();
            result = new Tuple<UInt16, UInt16>(address, quantity);

            return result;
         });
      }


      public Tuple<UInt16, UInt16> WriteSingleRegister(UInt16 address, UInt16 value)
      {
         return this.PerformWriteOperation<Tuple<UInt16, UInt16>>((BinaryWriter writer) => 
            {
               int byteOffset = fMemoryLayout.OffsetForHoldingRegister(address);
               writer.Seek(byteOffset, SeekOrigin.Begin);

               // Report register addresses.
               fTracer.Trace("WSR:0x{0} - 0x{1}", (byteOffset / 2).ToString("X"),
                ((byteOffset + sizeof(UInt16)) / 2).ToString("X"));

               writer.Write(value);
               writer.Close();

               // Read back the value to simulate device returning the written value.
               UInt16[] values = this.ReadHoldingRegisters(address, 1);
               return new Tuple<UInt16, UInt16>(address, values[0]);
            });
      }


      public Tuple<UInt16, UInt16> WriteMultipleCoils(UInt16 address, UInt16 quantity,
         byte[] outputValues)
      {
         Debug.Assert(quantity > 0);
         Debug.Assert(outputValues.Length >= quantity / 8);

         Tuple<UInt16, UInt16> result;

         try
         {
            byte[] coils = this.ReadCoils();

            // Calculate the byte which contains the bit address of the start coil.
            int byteAddress = address / 8;

            // Working bytewise, force the values from outputValues into coils.
            int numBits = quantity;
            while (numBits > 0)
            {
               ABufferExtensions.BitCopy(outputValues, 0, coils, address, quantity);
               numBits -= 8;
            }

            this.WriteCoils(coils);

            result = new Tuple<UInt16, UInt16>(address, quantity);
         }
         catch (Exception innerEx)
         {
            throw new Exception("Failed to write multiple coils.", innerEx);
         }
         return result;
      }


      public Tuple<UInt16, UInt16> WriteMultipleRegisters(UInt16 address, UInt16 quantity,
         UInt16[] outputValues)
      {
         return this.PerformWriteOperation<Tuple<UInt16, UInt16>>((BinaryWriter writer) =>
            {
               Tuple<UInt16, UInt16> result;

               int byteOffset = fMemoryLayout.OffsetForHoldingRegister(address);

               writer.Seek(byteOffset, SeekOrigin.Begin);
               byte[] converted = new byte[quantity * sizeof(UInt16)];
               Buffer.BlockCopy(outputValues, 0, converted, 0, converted.Length);

               // Report register addresses
               fTracer.Trace("WMR:0x{0} - 0x{1}", (byteOffset / 2).ToString("X"),
                ((byteOffset + converted.Length) / 2).ToString("X"));

               writer.Write(converted);
               writer.Close();
               result = new Tuple<UInt16, UInt16>(address, quantity);

               return result;
            });
      }


      public Tuple<UInt16, UInt16> WriteUserRegisters(UInt16 address, UInt16 quantity,
        UInt16[] outputValues)
      {
         return this.PerformWriteOperation<Tuple<UInt16, UInt16>>((BinaryWriter writer) =>
         {
            Tuple<UInt16, UInt16> result;

            int byteOffset = fMemoryLayout.OffsetForUserRegister(address);

            writer.Seek(byteOffset, SeekOrigin.Begin);
            byte[] converted = new byte[quantity * sizeof(UInt16)];
            Buffer.BlockCopy(outputValues, 0, converted, 0, converted.Length);

            // Report register addresses
            fTracer.Trace("WUR:0x{0} - 0x{1}", (byteOffset / 2).ToString("X"),
             ((byteOffset + converted.Length) / 2).ToString("X"));

            writer.Write(converted);
            writer.Close();
            result = new Tuple<UInt16, UInt16>(address, quantity);

            return result;
         });
      }


      public UInt16 WriteSingleCoil(UInt16 address, UInt16 value)
      {
         if (0x0000 != value && 0xFF00 != value)
         {
            return 0x8503; // return error with exception code 3
         }

         byte[] coils = this.ReadCoils();

         this.SetBitInBitField(coils, address, value);

         this.WriteCoils(coils);

         if (this.EnableProbeFiring && 0x0000 == address)
         {
            this.SimulateDataReady();
         }

         return address;
      }


      private void ClearDataReady()
      {
         this.WriteDiscreteInput(0x0000, 0x0000);
      }


      private void SimulateDataReady()
      {
         const int delay = 10;
         //Console.WriteLine("Setting probe data ready timer to fire in {0} ms.", delay);

         // TODO: Write dsi and material temp if we ever need it.

         var timer = new System.Timers.Timer(delay) { AutoReset = false };
         timer.Elapsed += (sender, args) =>
         {
            //Console.WriteLine("Fire data is ready.");
            this.WriteDiscreteInput(0x0000, 0xFF00);
         };
        
         timer.Enabled = true;
      }

      /// <summary>
      /// ModBus discrete inputs are not writable by ModBus clients, but we have to simulate them
      /// being set by whatever physical inputs to the device.
      /// </summary>
      /// <param name="address">The bit offset of the bit to set/clear</param>
      /// <param name="value">0x0000 to clear the bit, 0xFF00 to set the bit</param>
      private void WriteDiscreteInput(UInt16 address, UInt16 value)
      {
         byte[] inputs = this.ReadDiscreteInputs();
         this.SetBitInBitField(inputs, address, value);
         // Write back the modified inputs.
         this.WriteBlock(inputs, fMemoryLayout.ByteOffsetForDiscreteInput(0x0000));
      }


      private void SetBitInBitField(byte[] bitField, UInt16 address, UInt16 value)
      {
         int byteOffset = address / 8;
         int mask = (1 << (address % 8));
         int flag = (0x0000 == value ? 0 : 1); // 1 to set the bit, 0 to clear

         int output = bitField[byteOffset];
         output ^= (-flag ^ output) & mask;
         bitField[byteOffset] = (byte)output;
      }


      /// <summary>
      /// Read a block starting at address and convert to UInt16[].
      /// </summary>
      /// <param name="byteAddress">The absolute byte address of the block to read,
      /// i.e. already offset for register type and register address</param>
      /// <param name="quantity">The number of registers to read</param>
      /// <returns>Block of memory in array of UInt16 values</returns>
      private UInt16[] ReadRegisters(int byteAddress, UInt16 quantity)
      {
         return this.PerformReadOperation<UInt16[]>((BinaryReader reader) =>
            {
               reader.BaseStream.Seek(byteAddress,  SeekOrigin.Begin);

               int byteQuantity = quantity * sizeof(UInt16);
               byte[] byteResults = new byte[byteQuantity];
               reader.Read(byteResults, 0, byteQuantity);

               var results = new UInt16[quantity];
               Buffer.BlockCopy(byteResults, 0, results, 0, byteQuantity);

               return results;
            });
      }


      /// <summary>
      /// Read a block of memory.
      /// </summary>
      /// <param name="blockByteOffset">The byte address from which to read the block</param>
      /// <param name="blockByteLength">The number of bytes to read.</param>
      /// <returns></returns>
      private byte[] ReadBlock(int blockByteOffset, int blockByteLength)
      {
         return this.PerformReadOperation<byte[]>((BinaryReader reader) =>
            {
               reader.BaseStream.Seek(blockByteOffset, SeekOrigin.Begin);
               return reader.ReadBytes(blockByteLength);
            });
      }

      /// <summary>
      /// Write a block of memory.
      /// </summary>
      /// <param name="block">The memory to write</param>
      /// <param name="blockByteOffset">The offset in storage to write the block</param>
      private void WriteBlock(byte[] block, int blockByteOffset)
      {
         this.PerformWriteOperation<bool>((BinaryWriter writer) =>
         {
            fTracer.Trace("W:0x{0} - 0x{1} block", (blockByteOffset / 2).ToString("X"),
               ((blockByteOffset + block.Length) / 2).ToString("X"));
            writer.Seek(blockByteOffset, SeekOrigin.Begin);
            writer.Write(block);
            writer.Flush();
            return true;
         });
      }


      /// <summary>
      /// Copy a field of bits from a larger block of data.
      /// </summary>
      /// <param name="bitField">The source of the bits</param>
      /// <param name="address">The bitoffset in bitField at which to begin reading</param>
      /// <param name="quantity">The number of bits to extract</param>
      /// <returns>A new block of memory with the requested bits. The first bit is in the lsb
      /// of byte 0.</returns>
      private byte[] BitFieldSubset(byte[] bitField, UInt16 address, UInt16 quantity)
      {
         int remainderBits = quantity % 8;
         int numBytes = quantity / 8 + (remainderBits > 0 ? 1 : 0);
         Debug.Assert(numBytes <= bitField.Length);

         byte[] subset = new byte[numBytes];
         ABufferExtensions.BitCopy(bitField, address, subset, 0, quantity);
         return subset;
      }

      /// <summary>
      /// Read a block of memory containing all coil bits with the first bit byte aligned at
      /// the start of the returned buffer.
      /// </summary>
      /// Throws on error.
      /// <returns>The buffer containing the coil bits with address 0 at the least significant
      /// bit of byte 0</returns>
      private byte[] ReadCoils()
      {
         byte[] coils = this.ReadBlock(fMemoryLayout.ByteOffsetForCoil(0x0000),
            fMemoryLayout.CoilsByteLength());
         if (0 == coils.Length )
         {
            coils = new byte[fMemoryLayout.CoilsByteLength()];
         }
         return coils;
      }


      /// <summary>
      /// Write a block of memory containing coil bits.
      /// </summary>
      /// The bits are assumed to start at coil address 0, with address 0 being the least
      /// significant bit of byte 0.
      /// Throws on error.
      /// <param name="coils"></param>
      private void WriteCoils(byte[] coils)
      {
         this.WriteBlock(coils, fMemoryLayout.ByteOffsetForCoil(0x0000));
      }


      private byte[] ReadDiscreteInputs()
      {
         byte[] inputs = this.ReadBlock(fMemoryLayout.ByteOffsetForDiscreteInput(0x0000),
            fMemoryLayout.DiscreteInputsByteLength());
         if (null == inputs || 0 == inputs.Length)
         {
            inputs = new byte[fMemoryLayout.DiscreteInputsByteLength()];
         }
         return inputs;
      }


      /// <summary>
      /// Open the backing file, and prepare it for binary reading before calling
      /// a function with the reader.
      /// </summary>
      /// <typeparam name="T">The return type of fileOp operation</typeparam>
      /// <param name="fileOp">The function which will do the reading and return
      /// the result</param>
      /// <returns>The fileOp's result or default(T) if there is a problem opening
      /// the file.</returns>
      private T PerformReadOperation<T>(Func<BinaryReader, T> fileOp)
      {
         T result = default(T);

         using (var stream = File.Open(fBackingFilePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
         {
            using (var reader = new BinaryReader(stream))
            {
               result = fileOp(reader);
            }
            stream.Close();
         }

         return result;
      }


      /// <summary>
      /// Open the backing file, and prepare it for binary writing before calling
      /// a function with the writer.
      /// </summary>
      /// <typeparam name="T">The return type of fileOp operation</typeparam>
      /// <param name="fileOp">The function which will do the writing and return the result</param>
      /// <returns>The fileOp's result or default(T) if there is a problem opening
      /// the file.</returns>
      private T PerformWriteOperation<T>(Func<BinaryWriter, T> fileOp)
      {
         T result;
         using (var stream = File.Open(fBackingFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
         {
            using (var writer = new BinaryWriter(stream))
            {
               result = fileOp(writer);
            }
            stream.Close();
         }

         return result;
      }


      public void MakeMemoryLayout()
      {
         // This configuration matches documented DSI layout in terms of amounts,
         // but relative positions in the absolute address space probably doesn't match.
         int numDiscreteInputs = 8;
         int numCoils = 16;
         int numInputRegisters = 16 * 0x0800;
         int numUserRegisters = 0x0204;
         int numHoldingRegisters = 0x7c59; // about 63k, each probe section within has room to spare
         this.MakeMemoryLayout(numDiscreteInputs, numCoils, numInputRegisters, numUserRegisters, numHoldingRegisters);
      }

      /// <summary>
      ///  // Layout for a device with contiguous memory and the given amounts of memory in each section.
      /// </summary>
      /// The number of holder registers is unspecified because it is at the end of the memory.
      public void MakeMemoryLayout(int numDiscreteInputs, int numCoils, int numInputRegisters,
       int numUserRegisters, int numHoldingRegisters)
      {
         var offsets = new Dictionary<string, int>(); // Track offsets for debug reporting.

         fMemoryLayout = new APhysicalMemoryLayout();

         int offset = 0;

         offsets.Add("HoldingRegisters", offset);
         fMemoryLayout.fStartHoldingRegisters = offset;
         offset += numHoldingRegisters * sizeof(UInt16);

         offsets.Add("UserRegisters", offset);
         fMemoryLayout.fStartUserRegisters = offset;
         offset += numUserRegisters * sizeof(UInt16);

         offsets.Add("InputRegisters", offset);
         fMemoryLayout.fStartInputRegisters = offset;
         offset += numInputRegisters * sizeof(UInt16);

         offsets.Add("Discrete Inputs", offset);
         fMemoryLayout.fStartDiscreteInputs = offset;
         fMemoryLayout.fNumDiscreteInputs = numDiscreteInputs;
         offset += fMemoryLayout.fNumDiscreteInputs / 8 + ((fMemoryLayout.fNumDiscreteInputs % 8 > 0) ? 1 : 0);

         offsets.Add("Coils", offset);
         fMemoryLayout.fStartCoils = offset;
         fMemoryLayout.fNumCoils = numCoils;
         offset += fMemoryLayout.fNumCoils / 8 + ((fMemoryLayout.fNumCoils % 8 > 0) ? 1 : 0);


#if true // Debug
         fTracer.Trace("SimFile Layout\r\n---------------");
         fTracer.Trace("Byte/Register: Memory area\r\n---------------");
         foreach (KeyValuePair<string, int> pair in offsets)
         {
            // Report register address
            fTracer.Trace("0x{0}/0x{1}: {2}", pair.Value.ToString("X"), (pair.Value / 2).ToString("X"),
             pair.Key);
         }
#endif
      }

      public static string kExtension = ".dsi";

      private APhysicalMemoryLayout fMemoryLayout;
      private string fBackingFilePath;
      private Random fRng;
      private ATracer fTracer;
   }


   /// <summary>
   /// Customizable trace function.
   /// </summary>
   class ATracer
   {
      public delegate void TraceFunction(string format, params object[] args);

      public ATracer(TraceFunction traceFunc)
      {
         Debug.Assert(traceFunc != null);
         fTrace = traceFunc;
      }
      public void Trace(string format, params object[] args)
      {
         fTrace(format, args);
      }
      private TraceFunction fTrace;
   }
}

