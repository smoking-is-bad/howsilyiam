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
using System.IO.Ports;
using System.Text;
using System.Threading;

using Logging;
using ModBus;


namespace DsiApi
{
   /// <summary>
   /// Handles communication with physical devices connected to a serial port.
   /// </summary>
   class ASerialPacketHandler : IPacketHandler
   {
      /// <param name="portName"></param>
      /// <param name="baudRate"></param>
      /// <param name="timeout"></param>
      /// <param name="retries"></param>
      /// <param name="sendHandler">Called before sending a command buffer with the bytes about to be sent. No action is necessary, this is just a hook into the process.</param>
      /// <param name="readHandler">Called after receiving a result buffer with the bytes just received. No action is necessary, this is just a hook into the process.</param>
      public ASerialPacketHandler(string portName, int baudRate = 115200, int timeout = 1000, int retries = 3, Action<byte[]> sendHandler = null,
       Action<byte[]> readHandler = null)
      {
         fSendHandler = sendHandler;
         fReadHandler = readHandler;

         // Store for lazy port initialization of fPort
         fPortName = portName;
         fBaudRate = baudRate;
         fTimeout = timeout;
         fNumRetries = retries;

         fPort = null;
      }

      public APacket HandlePacket(APacket packet, int expectedResultLength, int retryCount = 0)
      {
         int maxRetries = fNumRetries;

         if (null == fPort || !fPort.IsOpen)
         {
            this.OpenPort(fPortName, fBaudRate, fTimeout);
         }


         byte[] packetData = packet.Data();
         int packetLength = packetData.Length;
         if (null != fSendHandler)
         {
            fSendHandler(packetData);
         }
         fPort.Write(packet.Data(), 0, packetLength);

         APacket result = null;
         var buffer = new byte[expectedResultLength];
         int read = 0;
         try
         {
            while (read < expectedResultLength)
            {
               read += fPort.Read(buffer, read, expectedResultLength - read);
            }

            //Thread.Sleep(75); // I found we needed this to not overwhelm the dev board. // I now find we don't need it.

            if (null != fReadHandler)
            {
               fReadHandler(buffer);
            }
         }
         catch (TimeoutException)
         {
            fPort.Close();
            fPort = null;
            ALog.Warning("SerialPacketHandler", "Timed out trying to read COMM port.");
            if (retryCount < maxRetries)
            {
               ++retryCount;
               ALog.Warning("SerialPacketHandler", "Low level retry, number " + retryCount);
               Thread.Sleep(200 * retryCount);
               result = this.HandlePacket(packet, expectedResultLength, retryCount);
            }
            else
            {
               ALog.Warning("SerialPacketHandler", "Max low level retries reached, aborting");
            }
         }

         if (null == result && read > 0)
         {
            result = new APacket(buffer, isResult:true);
         }

         return result;
      }

      private void OpenPort(string portName, int baudRate, int timeout)
      {
         fPort = APorts.Instance().Open(portName, baudRate, Parity.Even, 8, StopBits.One);
         fPort.ReadTimeout = timeout; //ms, firmware claims 10ms response time
      }

      private string fPortName;
      private int fBaudRate;
      private int fTimeout;
      private int fNumRetries;
      private SerialPort fPort;
      private Action<byte[]> fReadHandler;
      private Action<byte[]> fSendHandler;
   }
}

