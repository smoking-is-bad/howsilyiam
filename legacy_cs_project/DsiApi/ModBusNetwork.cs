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
using System.Text;
using System.Threading.Tasks;

using ModBus;

namespace DsiApi
{
   /// <summary>
   /// Implementor of ModBus interface which uses an ADsiNetwork to actually handle the
   /// fulfillment of the ModBus calls.
   /// </summary>
   public class AModBusNetwork : IModBusDevice
   {
      public AModBusNetwork(IPacketHandler dsiNetwork)
      {
         Debug.Assert(dsiNetwork != null);
         fDsiNetwork = dsiNetwork;
      }


      public void Reset()
      {
        // fDsiSim.Reset();
      }


      public byte[] ReadCoils(byte netAddress, UInt16 address, UInt16 quantity)
      {
         byte[] coils = null;
         var command = new AReadCoilsCommand(netAddress, address, quantity);
         if (command.Execute(fDsiNetwork))
         {
            command.ResultData(out coils);
         }
         return coils;
      }


      public byte[] ReadDiscreteInputs(byte netAddress, UInt16 address, UInt16 quantity)
      {
         byte[] inputs = null;
         var command = new AReadDiscreteInputsCommand(netAddress, address, quantity);
         if (command.Execute(fDsiNetwork))
         {
            command.ResultData(out inputs);
         }
         return inputs;
      }


      public UInt16[] ReadHoldingRegisters(byte netAddress, UInt16 address, UInt16 quantity)
      {
         UInt16[] registers = null;
         var command = new AReadHoldingRegistersCommand(netAddress, address, quantity);
         if (command.Execute(fDsiNetwork))
         {
            command.ResultData(out registers);
         }
         return registers;
      }


      // Note this is not a modbus specified command. It is application defined.
      public UInt16[] ReadInputRegisters(byte netAddress, UInt16 address, UInt16 quantity)
      {
         UInt16[] registers = null;
         var command = new AReadInputRegistersCommand(netAddress, address, quantity);
         if (command.Execute(fDsiNetwork))
         {
            command.ResultData(out registers);
         }
         return registers;
      }


      public UInt16[] ReadUserRegisters(byte netAddress, UInt16 address, UInt16 quantity)
      {
         UInt16[] registers = null;
         var command = new AReadUserRegistersCommand(netAddress, address, quantity);
         if (command.Execute(fDsiNetwork))
         {
            command.ResultData(out registers);
         }
         return registers;
      }


      public Tuple<UInt16, UInt16> WriteSingleRegister(byte netAddress, UInt16 address, UInt16 value)
      {
         Tuple<UInt16, UInt16> returnValue = null;
         var command = new AWriteSingleRegisterCommand(netAddress, address, value);
         if (command.Execute(fDsiNetwork))
         {
            command.ResultData(out returnValue);
         }
         return returnValue;
      }


      public Tuple<UInt16, UInt16> WriteMultipleCoils(byte netAddress, UInt16 address, UInt16 quantity,
         byte[] outputValues)
      {
         Tuple<UInt16, UInt16> returnValue = null;
         var command = new AWriteMultipleCoilsCommand(netAddress, address, quantity, outputValues);
         if (command.Execute(fDsiNetwork))
         {
            command.ResultData(out returnValue);
         }
         return returnValue;
      }


      public Tuple<UInt16, UInt16> WriteMultipleRegisters(byte netAddress, UInt16 address, UInt16 quantity,
         UInt16[] outputValues)
      {
         Tuple<UInt16, UInt16> returnValue = null;
         var command = new AWriteMultipleRegistersCommand(netAddress, address, quantity, outputValues);
         if (command.Execute(fDsiNetwork))
         {
            command.ResultData(out returnValue);
         }
         return returnValue;
      }


      public UInt16 WriteSingleCoil(byte netAddress, UInt16 address, UInt16 value)
      {
         UInt16 returnValue = 0x0000;
         var command = new AWriteSingleCoilCommand(netAddress, address, value);
         if (command.Execute(fDsiNetwork))
         {
            command.ResultData(out returnValue);
         }
         return returnValue;
      }


      // Note this is not a modbus specified command. It is application defined.
      public Tuple<UInt16, UInt16> WriteUserRegisters(byte netAddress, UInt16 address,
       UInt16 quantity, UInt16[] outputValues)
      {
         Tuple<UInt16, UInt16> returnValue = null;
         var command = new AWriteUserRegistersCommand(netAddress, address, quantity, outputValues);
         if (command.Execute(fDsiNetwork))
         {
            command.ResultData(out returnValue);
         }
         return returnValue;
      }


      public Tuple<UInt16, UInt16> WriteFunctionRegisters(byte netAddress, UInt16 address,
       UInt16 quantity, UInt16[] outputValues)
      {
         Tuple<UInt16, UInt16> returnValue = null;
         var command = new AWriteFunctionRegistersCommand(netAddress, address, quantity,
          outputValues);
         if (command.Execute(fDsiNetwork))
         {
            command.ResultData(out returnValue);
         }
         return returnValue;
      }


      private void ExecuteCommand(ACommand command, ref byte[] result)
      {
         command.Execute(fDsiNetwork);
         if (command.ResultIsGood)
         {
            command.ResultData<byte[]>(out result);
         }
         else if (command.HasResult)
         {
            throw new AModBusException(command.ResultExceptionCode, command.CommandPacket);
         }
      }


      private IPacketHandler fDsiNetwork;
   }
}

