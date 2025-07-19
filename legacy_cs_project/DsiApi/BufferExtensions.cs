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

using Logging;

namespace DsiExtensions
{
   /// <summary>
   /// Extensions to Buffer.
   /// </summary>
   public static class ABufferExtensions
   {
      /// <summary>
      /// Copy a range of bits from srcBuffer to dest.
      /// </summary>
      /// <param name="srcBuffer">The bits to copy</param>
      /// <param name="srcBitOffset">The bit in srcBuffer from which to start the copy.</param>
      /// <param name="destBuffer">The bits are copied into destBuffer and the last byte is
      /// padded with 0s if needed</param>
      /// <param name="numBits">The number of bits to copy</param>
      public static void BitCopy(byte[] srcBuffer, int srcBitOffset,
       byte[] destBuffer, int destBitOffset, int numBits)
      {
         Debug.Assert(numBits > 0);

         int srcByte = srcBitOffset / 8;
         int srcBit = srcBitOffset % 8;

         int destByte = destBitOffset / 8;
         int destBit = destBitOffset % 8;

         int remainderBits = numBits % 8;
         int numBytes = numBits / 8 + (remainderBits > 0 ? 1 : 0);
         Debug.Assert(numBytes <= destBuffer.Length);

         // If bitOffset is byte aligned, block copy.
         if (0 == srcBit && 0 == destBit)
         {
            Buffer.BlockCopy(srcBuffer, srcByte, destBuffer, destByte, numBytes);

            // Zero any pad bits at the end.
            if (0 != remainderBits)
            {
               byte srcMask = (byte)(0xFF >> (8 - numBits));
               destBuffer[destByte + numBytes - 1] &= srcMask;
            }

            // All bits were handled, there are no remainders.
            numBits = 0;
         }
         else // Bit-wise copy
         {
            int src = srcBuffer[srcByte];
            int srcMask = 0x00000001;

            int dest = destBuffer[destByte];

            while (true)
            {
               dest |= (((src >> srcBit) & srcMask) << destBit);

               if (--numBits <= 0)
               {
                  destBuffer[destByte] = (byte)dest;
                  break;
               }

               if (8 == ++destBit)
               {
                  destBuffer[destByte] = (byte)dest;
                  dest = destBuffer[++destByte];
                  destBit = 0;
               }

               if (8 == ++srcBit)
               {
                  src = (int)srcBuffer[++srcByte];
                  srcBit = 0;
               }
            }
         }
      }


      public unsafe static byte[] CopyToByteArray(object obj)
      {
         int size = Marshal.SizeOf(obj);

         // Marshal the object into an unmanaged block of code.
         IntPtr objPtr = Marshal.AllocHGlobal(size);
         Marshal.StructureToPtr(obj, objPtr, false);
         Marshal.FreeHGlobal(objPtr);

         // Copy into the array
         byte* src = (byte*)objPtr;
         byte[] output = new byte[size];
         fixed (byte* dest = &output[0])
         {
            for (int i = 0; i < size; ++i)
            {
               *(dest + i) = *src++;
            }
         }

         return output;
      }

      /// <summary>
      /// Convert obj into a byte array indexible by UInt16s. See ACompany for an object example.
      /// </summary>
      /// <param name="obj">An object which has a fixed layout and properly annotated for marshaling.</param>
      /// <returns>The array with the marshaled contents of obj</returns>
      public unsafe static UInt16[] CopyToUInt16Array(object obj)
      {
         int size = Marshal.SizeOf(obj);
         Debug.Assert(size % 2 == 0);

         // Marshal the object into an unmanaged block of code.
         IntPtr objPtr = Marshal.AllocHGlobal(size);
         Marshal.StructureToPtr(obj, objPtr, false);
        
         // Copy into the array
         int numInts = size / sizeof(UInt16);
         UInt16* src = (UInt16*)objPtr;
         UInt16[] output = new UInt16[numInts];
         fixed (UInt16* dest = &output[0])
         {
            for (int i = 0; i < numInts; ++i)
            {
               *(dest + i) = *src++;
            }
         }

         Marshal.FreeHGlobal(objPtr);

         return output;
      }

      /// <summary>
      /// Modifies obj by marshaling the data from input into the memory allocated to it.
      /// </summary>
      /// <typeparam name="T">The type of object to marshal</typeparam>
      /// <param name="input">The values to write into obj</param>
      /// <param name="obj"></param>
      /// <returns></returns>
      public static bool MarshalUInt16ArrayToObject<T>(UInt16[] input, T obj)
      {
         bool success = true;
         try
         {
            GCHandle handle = GCHandle.Alloc(input, GCHandleType.Pinned);
            IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(input, 0);
            Marshal.PtrToStructure(buffer, obj);
            handle.Free();
         }
         catch (ArgumentException argEx)
         {
            ALog.Error("DSI IO", "Error: Failed to marshal object; " + argEx.ToString() + ".");
            success = false;
         }
         catch (Exception anyOtherEx)
         {
            throw anyOtherEx;
         }

         return success;
      }
   }
}

