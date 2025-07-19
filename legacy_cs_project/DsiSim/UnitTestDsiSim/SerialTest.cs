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
using System.Text;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using DsiApi;
using ModBus;

namespace UnitTestDsiSim
{
   /// <summary>
   /// Summary description for SerialTest
   /// </summary>
   [TestClass]
   public class SerialTest
   {
      public SerialTest()
      {
         // Display each port name to the console.
         Console.WriteLine("The following serial ports were found:");
         foreach (string port in SerialPort.GetPortNames())
         {
            Console.WriteLine(port);
         }
         // 115200, even, 8, 1 stop
         fPort = new SerialPort("COM4", 115200, Parity.Even, 8, StopBits.One);

         fNetAddress = ADsiNetwork.kFactoryId;
      }

      [TestInitialize]
      public void InitSerial()
      {
         fPort.ReadTimeout = 1075; // ms
         fPort.Open();
      }

      [TestCleanup]
      public void CleanupSerial()
      {
         fPort.Close();
      }


      [TestMethod]
      public void TestReadDsiIdViaSerial()
      {
         fPort.DiscardInBuffer();

         // Try the default address.
      //!!!   Assert.IsTrue(SendGetDsiId(ADsiNetwork.kFactoryId));

         const int kResultLength = 7;
         bool success = false;
   /*      try
         {
            fPort.Read(buffer, 0, resultLength);
            Console.WriteLine("Receive: " + BitConverter.ToString(buffer, 0, resultLength));
            success = true;
         }
         catch (TimeoutException)*/
         {
            Console.WriteLine("Timeout sending to default address.");

            for (byte netAddress = 0x01; netAddress <= 0x2F; ++netAddress)
            {
               Assert.IsTrue(SendGetDsiId(netAddress));
               Thread.Sleep(18);
               try
               {
                  var buffer = new byte[8];
                  int read = 0;
                  while (read < kResultLength)
                  {
                     read += fPort.Read(buffer, read, kResultLength - read);
                  }

                  Console.WriteLine("Receive: " + BitConverter.ToString(buffer, 0, kResultLength));

                  Assert.AreEqual(netAddress, buffer[0]);
                  Assert.AreEqual((byte)CommandCode.kReadUserRegisters, buffer[1]);
               }
               catch (TimeoutException)
               {
                  Console.WriteLine("Timeout.");
                  continue;
               }

               fNetAddress = netAddress;
               success = true;
               //break;
            }
         }

         // Did we read an id?
         Assert.IsTrue(success);
      }


      [TestMethod]
      public void TestReadUserRegistersViaSerial()
      {
         for (int i = 0; i < 100; ++i)
         {
            // Read a block of the user registers.
            const UInt16 kNumRegisters = 0x0070; // !!! Was using 0x007d, but firmware has a bug and won't do that amount.
            var command = new AReadUserRegistersCommand(fNetAddress, 0x0189, kNumRegisters);
            SendCommand(command);

            bool success = true;
            int resultLength = command.ExpectedResultBytes();
            int offset = 0;
            var buffer = new byte[resultLength];

            try
            {
               while (offset < resultLength)
               {
                  int read = fPort.Read(buffer, offset, resultLength - offset);
                  Console.WriteLine("Received: " + BitConverter.ToString(buffer, offset, read));
                  offset += read;
               }
            }
            catch (TimeoutException)
            {
               success = false;
               Console.WriteLine("Timeout.");
            }
            Assert.IsTrue(success);
         }
      }


      private bool SendGetDsiId(byte netAddress)
      {
         bool success = true;
         try
         {
            UInt16 quantity = 0x0001;
            var command = new AReadUserRegistersCommand(netAddress,
             (UInt16)AMemoryLayout.UserRegisters.kModBusAddress, quantity);
            SendCommand(command);
         }
         catch (TimeoutException)
         {
            Console.WriteLine("Write Get ID Timeout!");
            success = false;
         }

         return success;
      }


      private bool SendWriteDsiId(byte netAddress, UInt16 newAddress)
      {
         bool success = true;
         try
         {
            UInt16 quantity = 0x0001;
            UInt16[] output = { newAddress };
            var command = new AWriteUserRegistersCommand(netAddress,
             (UInt16)AMemoryLayout.UserRegisters.kModBusAddress, quantity, output);
            SendCommand(command);
         }
         catch (TimeoutException)
         {
            Console.WriteLine("Write Set ID Timeout!");
            success = false;
         }

         return success;
      }


      private void SendCommand(ACommand command)
      {
         APacket commandPacket = command.CommandPacket;
         commandPacket.CalculateErrorChecks();
         byte[] commandBytes = commandPacket.Data();
         Console.WriteLine("Sending: " + BitConverter.ToString(commandBytes));
         fPort.Write(commandBytes, 0, commandBytes.Length);
      }

      SerialPort fPort;
      byte fNetAddress;
   }
}

