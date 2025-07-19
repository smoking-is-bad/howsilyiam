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
using System.IO;
using System.Collections.Generic;
using System.Threading;

using DsiApi;
using Logging;
using ModBus;


namespace DsiSim
{
   using DsiMap = Dictionary<byte, ADsiSim>;

   /// <summary>
   /// Manages packet reading and writing on a network of ADsiSim objects
   /// </summary>
   /// <remarks>
   /// The network routes APacket objects by finding the associated DSI for the packet's
   /// modbus address, then translating that to a method on the DSI.
   /// </remarks>
   public class ASimPacketHandler : IPacketHandler
   {
      /// <summary>
      /// Designated Ctor. Create 32 DSI files.
      /// Addresses are assigned sequentially and written to the DSI file which
      /// uses a file named [address].dsi for storage.
      /// </summary>
      /// <param name="networkPath">The directory within which to create the DSI files</param>
      /// <param name="latencyMs">Simulate latency by providing the number of ms to sleep after a result is read.</param>
      public ASimPacketHandler(string networkPath, int latencyMs = 0)
      {
			fLatencyMs = latencyMs;
			this.InitDsiNetFromFolder(networkPath);
      }


      public ASimPacketHandler(string networkPath, int latencyMs = 0,
       Action<byte[]> sendHandler = null,	Action<byte[]> readHandler = null)
         : this(networkPath, latencyMs)
      {
         fReadHandler = readHandler;
         fSendHandler = sendHandler;
      }


      public APacket HandlePacket(APacket packet, int unused, int retryCount = 0)
      {
         APacket result = null;

         try
         {
            ADsiSim dsi = this.DsiForAddress(packet.GetAddress());
            if (null == dsi)
            {
               return null;
            }
            if (null != fSendHandler)
            {
               fSendHandler(packet.Data());
            }
            result = this.HandleCommandWithDsi(packet, dsi);
            // To simulate latency, delay the response.
            if (0 != fLatencyMs)
            {
               Thread.Sleep(fLatencyMs);
            }

            if (null != fReadHandler)
            {
               fReadHandler(result.Data());
            }
         }
         catch (ArgumentNullException)
         {
            ALog.Info("SimPacketHandler", "Attempting to access unknown DSI at {0}", packet.GetAddress());
         }
         catch (Exception ex)
         {
            ALog.Error("SimPacketHandler", ex);
         }

         return result;
      }


      /// <summary>
      /// Public only for debug use; prefer using command/packet interface.
      /// </summary>
      /// May throw ArgumentNullException if address is not a known DSI address.
      /// <param name="address"></param>
      /// <returns></returns>
      public ADsiSim DsiForAddress(byte address)
      {
         return fDsiNet[address];
      }


      /// <summary>
      /// Translate command in packet into a Modbus command on the given DSI
      /// </summary>
      /// <param name="packet">The command and parameters</param>
      /// <param name="dsi">The DSI to handle the command</param>
      /// <returns>The results of the command.</returns>
      private APacket HandleCommandWithDsi(APacket packet, ADsiSim dsi)
      {
         APacket result = new AErrorPacket(packet.GetCommand(), (byte)ExceptionCode.kIllegalFunction);
         switch ((CommandCode)packet.GetCommand())
         {
            case CommandCode.kReadDiscreteInputs:
            result = this.ReadDiscreteInputs(dsi, packet);
            break;

            case CommandCode.kReadCoils:
            result = this.ReadCoils(dsi, packet);
            break;

            case CommandCode.kWriteSingleCoil:
            result = this.WriteSingleCoil(dsi, packet);
            break;

            case CommandCode.kWriteMultipleCoils:
            result = this.WriteMultipleCoils(dsi, packet);
            break;

            case CommandCode.kReadInputRegisters:
            result = this.ReadInputRegisters(dsi, packet);
            break;

            case CommandCode.kReadHoldingRegisters:
            result = this.ReadHoldingRegisters(dsi, packet);
            break;

            case CommandCode.kWriteSingleRegister:
            result = this.WriteSingleRegister(dsi, packet);
            break;

            case CommandCode.kWriteMultipleRegisters:
            result = this.WriteMultipleRegisters(dsi, packet);
            break;

            case CommandCode.kReadUserRegisters:
            result = this.ReadUserRegisters(dsi, packet);
            break;

            case CommandCode.kWriteUserRegisters:
            result = this.WriteUserRegisters(dsi, packet);
            break;

            default:
            break;
         }

         result.CalculateErrorChecks();
         return result;
      }


      private APacket ReadCoils(ADsiSim dsi, APacket packet)
      {
         UInt16 address;
         UInt16 quantity;
         this.ExtractPacketData(packet, out address, out quantity);

         byte[] resultData = dsi.ReadCoils(address, quantity);

         // TODO: Check for error

         APacket result = new APacket(packet.GetCommand(), resultData);
         return result;
      }


      private APacket ReadDiscreteInputs(ADsiSim dsi, APacket packet)
      {
         UInt16 address;
         UInt16 quantity;
         this.ExtractPacketData(packet, out address, out quantity);

         byte[] resultData = dsi.ReadDiscreteInputs(address, quantity);

         // TODO: Check for error

         APacket result = new APacket(packet.GetCommand(), resultData);
         return result;
      }


      public APacket ReadHoldingRegisters(ADsiSim dsi, APacket packet)
      {
         UInt16 address;
         UInt16 quantity;
         this.ExtractPacketData(packet, out address, out quantity);

         UInt16[] resultData = dsi.ReadHoldingRegisters(address, quantity);

         // TODO: Check for error

         APacket result = new APacket(packet.GetCommand(), resultData);
         return result;
      }


      public APacket ReadUserRegisters(ADsiSim dsi, APacket packet)
      {
         UInt16 address;
         UInt16 quantity;
         this.ExtractPacketData(packet, out address, out quantity);

         UInt16[] resultData = dsi.ReadUserRegisters(address, quantity);

         // TODO: Check for error

         APacket result = new APacket(packet.GetCommand(), resultData);
         return result;
      }


      public APacket ReadInputRegisters(ADsiSim dsi, APacket packet)
      {
         UInt16 address;
         UInt16 quantity;
         this.ExtractPacketData(packet, out address, out quantity);

         UInt16[] resultData = dsi.ReadInputRegisters(address, quantity);

         // TODO: Check for error

         APacket result = new APacket(packet.GetCommand(), resultData);
         return result;
      }


      public APacket WriteSingleRegister(ADsiSim dsi, APacket packet)
      {
         UInt16 address;
         UInt16 quantity;
         this.ExtractPacketData(packet, out address, out quantity);
         Tuple<UInt16, UInt16> resultData = dsi.WriteSingleRegister(address, quantity);

         // TODO: Check for error

         APacket result = new APacket(packet.GetCommand(), resultData);
         return result;
      }


      public APacket WriteMultipleCoils(ADsiSim dsi, APacket packet)
      {
         UInt16 address;
         UInt16 quantity;
         byte[] outputValues;
         this.ExtractPacketData(packet, out address, out quantity, out outputValues);

         Tuple<UInt16, UInt16> resultData = dsi.WriteMultipleCoils(address, quantity, outputValues);

         // TODO: Check for error

         APacket result = new APacket(packet.GetCommand(), resultData);
         return result;
      }


      public APacket WriteMultipleRegisters(ADsiSim dsi, APacket packet)
      {
         UInt16 address;
         UInt16 quantity;
         UInt16[] outputValues;
         this.ExtractPacketData(packet, out address, out quantity, out outputValues);

         Tuple<UInt16, UInt16> resultData = dsi.WriteMultipleRegisters(address, quantity, outputValues);

         // TODO: Check for error

         APacket result = new APacket(packet.GetCommand(), resultData);
         return result;
      }

      public APacket WriteUserRegisters(ADsiSim dsi, APacket packet)
      {
         UInt16 address;
         UInt16 quantity;
         UInt16[] outputValues;
         this.ExtractPacketData(packet, out address, out quantity, out outputValues);

         Tuple<UInt16, UInt16> resultData = dsi.WriteUserRegisters(address, quantity, outputValues);

         // TODO: Check for error

         // When the modbus ID changes, we need to remap the file correctly.
         if ((UInt16)AMemoryLayout.UserRegisters.kModBusAddress == address)
         {
            byte newAddress = (byte)(outputValues[0] & 0x00FF);
            dsi.MoveTo(newAddress);
            fDsiNet.Remove(packet.GetAddress());
            fDsiNet.Add(newAddress, dsi);
         }

         APacket result = new APacket(packet.GetCommand(), resultData);
         return result;
      }


      public APacket WriteSingleCoil(ADsiSim dsi, APacket packet)
      {
         UInt16 address;
         UInt16 value;
         this.ExtractPacketData(packet, out address, out value);

         UInt16 resultData = dsi.WriteSingleCoil(address, value);

         // TODO: Check for error

         APacket result = new APacket(packet.GetCommand(), resultData);
         return result;
      }


      private int ExtractPacketData(APacket packet, out UInt16 address, out UInt16 quantity)
      {
         int offset = APacket.kAddressAndCommandSize;
         address = packet.ValueAtOffset(offset);
         offset += sizeof(UInt16);
         quantity = packet.ValueAtOffset(offset);
         offset += sizeof(UInt16);
         return offset;
      }


      private int ExtractPacketData(APacket packet, out UInt16 address, out UInt16 quantity,
       out UInt16[] outputValues)
      {
         int offset = this.ExtractPacketData(packet, out address, out quantity);
         offset += 1; // skip byte count. We trust quantity.
         outputValues = packet.ValuesAtOffset(offset, quantity);
         offset += quantity * sizeof(UInt16);
         return offset;
      }

      private int ExtractPacketData(APacket packet, out UInt16 address, out UInt16 quantity,
      out byte[] outputValues)
      {
         int offset = this.ExtractPacketData(packet, out address, out quantity);
         int numBytes = (int)packet.ByteAtOffset(offset);
         offset += 1;
         outputValues = packet.BytesAtOffset(offset, numBytes);
         offset += numBytes;
         return offset;
      }


		private void InitDsiNetFromFolder(string networkPath)
		{
			fDsiNet = new DsiMap();
			var dsiFilenames = Directory.EnumerateFiles(networkPath, "*.dsi");
			foreach (var filename in dsiFilenames)
			{
				byte address = Convert.ToByte(Path.GetFileNameWithoutExtension(filename));
				fDsiNet.Add(address, ADsiSim.CreateDsiSim(filename, address, reset:false));
			}
		}


      private DsiMap fDsiNet;
      private Action<byte[]> fReadHandler;
      private Action<byte[]> fSendHandler;
      int fLatencyMs;
   }
}

