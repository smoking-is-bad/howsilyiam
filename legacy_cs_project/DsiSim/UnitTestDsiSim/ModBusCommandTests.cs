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
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ModBus;

namespace UnitTestDsiSim
{
   /// <summary>
   /// Test that commands are packing parameters into the packets correctly.
   /// </summary>
   [TestClass]
   public class ModBusCommandPacketTests
   {
      [TestMethod]
      public void TestReadCoilsCommandCommand()
      {
         byte netAddress = 0x45;
         UInt16 address = 0x7FA3;
         UInt16 quantity = 0x02CD;

         ACommand command = new AReadCoilsCommand(netAddress, address, quantity);

         APacket packet = command.CommandPacket;

         Assert.AreEqual(netAddress, packet.GetAddress());
         Assert.AreEqual((byte)CommandCode.kReadCoils, packet.GetCommand());
         Assert.AreEqual(address, (UInt16)packet.ValueAtOffset(2));
         Assert.AreEqual(quantity, (UInt16)packet.ValueAtOffset(4));
      }


      [TestMethod]
      public void TestReadDiscreteInputsCommand()
      {
         byte netAddress = 0x45;
         UInt16 address = 0x7FA3;
         UInt16 quantity = 0x02CD;

         ACommand command = new AReadDiscreteInputsCommand(netAddress, address, quantity);

         APacket packet = command.CommandPacket;

         Assert.AreEqual(netAddress, packet.GetAddress());
         Assert.AreEqual((byte)CommandCode.kReadDiscreteInputs, packet.GetCommand());
         Assert.AreEqual(address, (UInt16)packet.ValueAtOffset(2));
         Assert.AreEqual(quantity, (UInt16)packet.ValueAtOffset(4));
      }


      [TestMethod]
      public void TestReadHoldingRegistersCommand()
      {
         byte netAddress = 0x45;
         UInt16 address = 0x7FA3;
         UInt16 quantity = 0x02CD;

         ACommand command = new AReadHoldingRegistersCommand(netAddress, address, quantity);

         APacket packet = command.CommandPacket;

         Assert.AreEqual(netAddress, packet.GetAddress());
         Assert.AreEqual((byte)CommandCode.kReadHoldingRegisters, packet.GetCommand());
         Assert.AreEqual(address, (UInt16)packet.ValueAtOffset(2));
         Assert.AreEqual(quantity, (UInt16)packet.ValueAtOffset(4));
      }


      [TestMethod]
      public void TestReadInputRegistersCommand()
      {
         byte netAddress = 0x45;
         UInt16 address =  0x7FA3;
         UInt16 quantity = 0x02CD;

         ACommand command = new AReadInputRegistersCommand(netAddress, address, quantity);

         APacket packet = command.CommandPacket;

         Assert.AreEqual(netAddress, packet.GetAddress());
         Assert.AreEqual((byte)CommandCode.kReadInputRegisters, packet.GetCommand());
         Assert.AreEqual(address, (UInt16)packet.ValueAtOffset(2));
         Assert.AreEqual(quantity, (UInt16)packet.ValueAtOffset(4));
      }


      [TestMethod]
      public void TestWriteMultipleCoilsCommand()
      {
         byte netAddress = 0x45;
         UInt16 address = 0x7FA3;
         UInt16 quantity = 68; // four bits less than are in values
         byte[] values = new byte[] { 0x01, 0x03, 0x07, 0x0F, 0x1F, 0x3F, 0x7F, 0xFF, 0xF0 };

         ACommand command = new AWriteMultipleCoilsCommand(netAddress, address, quantity, values);

         APacket packet = command.CommandPacket;

         Assert.AreEqual(netAddress, packet.GetAddress());
         Assert.AreEqual((byte)CommandCode.kWriteMultipleCoils, packet.GetCommand());
         Assert.AreEqual(address, (UInt16)packet.ValueAtOffset(2));
         Assert.AreEqual(quantity, (UInt16)packet.ValueAtOffset(4));
         Assert.AreEqual((byte)values.Length, (byte)packet.Data()[6]);
         int offset = 7;
         foreach (byte value in values)
         {
            Assert.AreEqual(value, packet.Data()[offset++]);
         }
      }


      [TestMethod]
      public void TestWriteSingleCoilCommand()
      {
         byte netAddress = 0x45;
         UInt16 address = 0x7FA3;
         UInt16 value = 0xFF00;

         ACommand command = new AWriteSingleCoilCommand(netAddress, address, value);

         APacket packet = command.CommandPacket;

         Assert.AreEqual(netAddress, packet.GetAddress());
         Assert.AreEqual((byte)CommandCode.kWriteSingleCoil, packet.GetCommand());
         Assert.AreEqual(address, (UInt16)packet.ValueAtOffset(2));
         Assert.AreEqual(value, (UInt16)packet.ValueAtOffset(4));
      }

      [TestMethod]
      public void TestWriteSingleRegisterCommand()
      {
         byte netAddress = 0x45;
         UInt16 address = 0x7FA3;
         UInt16 value = 0x02CD;

         ACommand command = new AWriteSingleRegisterCommand(netAddress, address, value);

         APacket packet = command.CommandPacket;

         Assert.AreEqual(netAddress, packet.GetAddress());
         Assert.AreEqual((byte)CommandCode.kWriteSingleRegister, packet.GetCommand());
         Assert.AreEqual(address, (UInt16)packet.ValueAtOffset(2));
         Assert.AreEqual(value, (UInt16)packet.ValueAtOffset(4));
      }

      [TestMethod]
      public void TestWriteMultipleRegistersCommand()
      {
         byte netAddress = 0x45;
         UInt16 address = 0x7FA3;
         var values = new UInt16[] { 0x0103, 0x070F, 0x1F3F, 0x7FFF };
         UInt16 quantity = (UInt16)values.Length;

         ACommand command = new AWriteMultipleRegistersCommand(netAddress, address, quantity, values);

         APacket packet = command.CommandPacket;

         Assert.AreEqual(netAddress, packet.GetAddress());
         Assert.AreEqual((byte)CommandCode.kWriteMultipleRegisters, packet.GetCommand());
         Assert.AreEqual(address, (UInt16)packet.ValueAtOffset(2));
         Assert.AreEqual(quantity, (UInt16)packet.ValueAtOffset(4));
         Assert.AreEqual(quantity * sizeof(UInt16), (byte)packet.Data()[6]);
         int offset = 7;
         foreach (UInt16 expected in values)
         {
            UInt16 value = packet.ValueAtOffset(offset);
            Assert.AreEqual(expected, value);
            offset += sizeof(UInt16);
         }
      }
   }
}

