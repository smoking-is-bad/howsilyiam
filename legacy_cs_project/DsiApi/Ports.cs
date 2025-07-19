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
using System.Collections.Generic;
using System.IO.Ports;

using Logging;
using System;

namespace DsiApi
{
   public class APortException : System.Exception
   {
      public APortException(string message) : base(message) {}
   }

   /**
    * A singleton to manage serial ports so they have lifetimes beyond the objects
    * which use them.
    */
   public class APorts
   {
      /** Retrieve the single Ports instance. */
      public static APorts Instance()
      {
         if (null == fInstance)
         {
            fInstance = new APorts();
         }
         return fInstance;
      }


      /** Return list of available port names */
      public static string[] PortNames()
      {
         return SerialPort.GetPortNames();
      }


      ~APorts()
      {
         this.CloseAll();
      }


      public SerialPort Open(string portName, int baudRate, Parity parity = Parity.Even,
         int dataBits = 8, StopBits stopBits = StopBits.One)
      {
         if (null == portName)
         {
            throw new APortException(ApiResources.ErrorPortName);
         }

         if (fPorts.ContainsKey(portName) && fPorts[portName].BaudRate != baudRate)
         {
            this.CloseAndRemove(portName);
         }

         if (!fPorts.ContainsKey(portName))
         {
            this.AddPort(portName, baudRate, parity, dataBits, stopBits);
         }

         if (!fPorts[portName].IsOpen)
         {
            try
            {
               fPorts[portName].Open();
            }
            catch (System.IO.IOException e)
            {
               string message = string.Format(ApiResources.ErrorPortOpen + "\n" + e.Message, portName);
               ALog.Warning("Ports", message);
               throw new APortException(message);
            }
         }

         // discard any data in the input buffer so we can clear out the bootloader message
         fPorts[portName].DiscardInBuffer();

         return fPorts[portName];
      }


      public void CloseAll()
      {
         foreach(string portName in fPorts.Keys)
         {
            this.Close(portName);
         }
         fPorts.Clear();
      }


      public void Close(string portName)
      {
         fPorts[portName].Close();
      }


      public void CloseAndRemove(string portName)
      {
         this.Close(portName);
         fPorts.Remove(portName);
      }


      private void AddPort(string portName, int baudRate, Parity parity, int dataBits, 
       StopBits stopBits)
      {
         try
         {
            fPorts[portName] = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
         }
         catch (Exception e)
         {
            string message = string.Format(ApiResources.ErrorPortOpen + "\n" + e.Message, portName);
            ALog.Warning("Ports", message);
            throw new APortException(message);
         }
      }


      APorts()
      {
         fPorts = new Dictionary<string, SerialPort>();
      }

      private Dictionary<string, SerialPort> fPorts;

      private static APorts fInstance = null;
   }
}

