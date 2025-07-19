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
using System.Runtime.InteropServices;

using DsiExtensions;
using Logging;
using ModBus;

namespace DsiApi
{
   /// <summary>
   /// Handles large scale reads and writes from a dsi by helping with breaking the
   /// calls to read and write into allowed sizes. 
   /// 
   /// Requires callers to provide an action for reading/writing to get the proper
   /// DSI address space.
   /// </summary>
   public static class ADsiIo
   {
      /// <summary>
      /// Callback to read from a specific section of DSI memory.
      /// </summary>
      public delegate UInt16[] ReadAction(UInt16 address, UInt16 quantity);


      /// <summary>
      /// Callback to actually perform the write to a specific section of DSI memory.
      /// </summary>
      public delegate bool WriteAction(UInt16[] registers, UInt16 address);


      /// <summary>
      /// Read using readAction, and set obj's fields via marshaling the read bytes.
      /// </summary>
      /// <typeparam name="T">The type of T</typeparam>
      /// <param name="netAddress">Which DSI to read</param>
      /// <param name="initialAddress">Where to start reading</param>
      /// <param name="obj">The object which will take its values from the read bytes</param>
      /// <param name="maxToRead">The largest amount which could be read</param>
      /// <param name="readAction">This function performs the read</param>
      /// <returns></returns>
      public static bool ReadObject<T>(this ADsiNetwork dsiNet, byte netAddress, UInt16 initialAddress, out T obj, int maxRead,
       ReadAction readAction)
       where T : new()
      {
         int marshaledSize = Marshal.SizeOf(typeof(T));
         UInt16[] registers = new UInt16[marshaledSize / sizeof(UInt16)];

         bool success = dsiNet.ChunkedRead(registers, initialAddress, maxRead, readAction);

         obj = default(T);
         if (success)
         {
            obj = new T();
            success = ABufferExtensions.MarshalUInt16ArrayToObject<T>(registers, obj);
            if (!success)
            {
               ALog.Error("DSI IO:", "Failed to marshal object from DSI register values.");
            }
         }

         return success;
      }


      /// <summary>
      /// Breaking the object's bytes into chunks, call writeAction for each chunk.
      /// </summary>
      /// <param name="netAddress"></param>
      /// <param name="initialAddress"></param>
      /// <param name="obj"></param>
      /// <param name="writeAction"></param>
      /// <returns></returns>
      public static bool WriteObject(this ADsiNetwork dsiNet, byte netAddress,
       UInt16 initialAddress, object obj, WriteAction writeAction)
      {
         UInt16[] registers = ABufferExtensions.CopyToUInt16Array(obj);
         return dsiNet.ChunkedWrite(registers, initialAddress, writeAction);
      }


      /// <summary>
      /// Read the requested number of registers through one or more calls to readAction.
      /// </summary>
      /// <param name="registers">An array whose size is used to define the number of registers to read</param>
      /// <param name="initialAddress">The register address to begin the read.</param>
      /// <param name="maxToRead">The maximum number of registers that the readAction's command
      /// can handle. For example ReadUserRegisters can accept at most 256-2-2-1=251 bytes = 124 registers.
      /// Commands should have a property giving this number. Use the one for the command the read action will
      /// send.</param>
      /// <param name="readAction">Actually makes the call to read from the DSI.</param>
      /// <returns>true on success, false otherwise</returns>
      public static bool ChunkedRead(this ADsiNetwork dsiNet, UInt16[] registers, 
       UInt16 initialAddress, int maxToRead, ReadAction readAction)
      {
         Debug.Assert(null != readAction);

         int regOffset = 0;
         int regCount = registers.Length;

         bool success = true;
         while (success && regCount > 0)
         {
            int regToRead = Math.Min(maxToRead, regCount);

            UInt16 address = (UInt16)(initialAddress + regOffset);

            UInt16[] subRegisters = readAction(address, (UInt16)regToRead);
            success = null != subRegisters;
            if (success)
            {
               int registersRead = subRegisters.Length;
               success = (subRegisters != null);
               if (success)
               {
                  Array.Copy(subRegisters, 0, registers, regOffset, registersRead);
                  regOffset += registersRead;
                  regCount -= registersRead;
               }
            }
         }

         return success;
      }


      /// <summary>
      /// Write the given registers through one or more calls to writeAction.
      /// </summary>
      /// <param name="registers">An array of register values to write to the DSI.</param>
      /// <param name="initialAddress">The register address to begin the read.</param>
      /// <param name="writeAction">Actually makes the call to write to the DSI.</param>
      /// <returns>true on success, false otherwise</returns>
      public static bool ChunkedWrite(this ADsiNetwork dsiNet, UInt16[] registers, 
       UInt16 initialAddress, WriteAction writeAction)
      {
         Debug.Assert(null != writeAction);

         bool success = true;

         // Chunk the writes since some structures will be too big to fit in one write packet. 
         UInt16[] subRegisters = registers;
         int regCount = registers.Length;
         int regOffset = 0;
         int kMaxRegisterCount = AWriteUserRegistersCommand.MaxPayload / sizeof(UInt16);

         //Console.WriteLine("Max registers {0}", kMaxRegisterCount.ToString("X"));

         while (success && regCount > 0)
         {
            // Can only write a limited number of bytes at a time
            UInt16 registersToWrite = (UInt16)Math.Min(kMaxRegisterCount, regCount);
            if (registersToWrite != registers.Length)
            {
               // Have to copy unless I expand writes to take offsets for the registers.
               subRegisters = new UInt16[registersToWrite];
               Array.Copy(registers, regOffset, subRegisters, 0, registersToWrite);
            }

            UInt16 address = (UInt16)(initialAddress + regOffset);
            success = writeAction(subRegisters, address);

            regCount -= registersToWrite;
            regOffset += registersToWrite;
         }

         return success;
      }

   }
}
