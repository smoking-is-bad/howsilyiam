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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Logging;
using ModBus;
using Model;

namespace DsiApi
{
   /// <summary>
   /// Methods for bulk setting values on a given DSI network, usually for initialization.
   /// </summary>
   public static class ADsiConfiguration
   {
      // USe this value for numDsis
      public const byte kUseDsiCount = 0xFF;

      /// <summary>
      /// Write values taken from a given XML file to a specific DSI. 
      /// To create the DSI files being initialized see ADsiSim.CreateSimFiles(...).
      /// The cell DSI model related fields are protected by reading before overwriting
      /// what was in the XML file.
      /// </summary>
      /// <param name="dsiNet">The DSI network object</param>
      /// <param name="initialSettingsXmlPath">path of XML file to use.</param>
      /// <param name="numDsis">The number of DSIs to initialize with the given file,  at
      ///  addresses 1...numDsis. This is ignored if the XML defines a list of DSIs. If numDsis
      ///  is kUseDsiCount, then the number of DSI elements in the xml file is used.</param>
      public static void InitializeNetworkWithSettings(this ADsiNetwork dsiNet, 
       string initialSettingsXmlPath)
      {
         ANanoSense state = ANanoSense.MakeFromXmlFile(initialSettingsXmlPath);
         if (null == state)
         {
            ALog.Error("DSI", "Failed to parse XML file for initialization.");
            return;
         }

         IEnumerable<int> addresses = state.dsis.Select(dsiInfo => (int)dsiInfo.modbusAddress);
    
         // Use array or copy the one dsi into all elements in the new DSI array if state.dsis not present.
         IEnumerable<ADsiInfo> dsis = state.dsis;
         IEnumerator<ADsiInfo> dsiIter = dsis.GetEnumerator();
         UInt16 numDsis = (UInt16)dsis.Count();

         bool success = true;
         foreach (byte netAddress in addresses)
         {
            dsiIter.MoveNext();

            //Console.Write("\tDsi_{0} |P|={1} ", netAddress, dsiIter.Current.probeCount);
            success = dsiNet.WriteModBusId(netAddress, netAddress);
            if (!success)
            {
               ALog.Error("DSI", "Failed to write modbus id when initializing.");
               break;
            }

            success = dsiNet.ConfigureCompanyAndSiteInfo(netAddress, state);

            ADsiInfo dsi = dsiIter.Current;

            // 1539 - Preserve a cell information in the firmware.
            AMarshaledDsiInfo origDsiInfo = dsiNet.ReadDsiInfo(netAddress);
            dsi.cellNetworkPassword = origDsiInfo.cellNetworkPassword;
            dsi.cellNetworkUserName = origDsiInfo.cellNetworkUserName;
            dsi.cellProvider = origDsiInfo.cellProvider;
            dsi.gsmAccessPoint = origDsiInfo.gsmAccessPoint;
            dsi.shotTimeInterval = origDsiInfo.shotTimeInterval;
            dsi.transmitTimeInterval = origDsiInfo.transmitTimeInterval;
            // if serial number not present in the config, preserve the one from the DSI
            if (null == dsi.serialNumber || 0 == dsi.serialNumber.Length)
            {
               dsi.serialNumber = origDsiInfo.serialNumber;
            }

            success = dsiNet.ConfigureDsiInfoAndProbes(netAddress, numDsis, dsi);
         }
      } 
      

      /// <summary>
      /// Write company, site, plant, asset and collection point information to the DSI.
      /// </summary>
      private static bool ConfigureCompanyAndSiteInfo(this ADsiNetwork dsiNet, byte netAddress, ANanoSense state)
      {
         bool success = dsiNet.WriteCompany(netAddress, state.company);
         Debug.Assert(success);

         success = dsiNet.WriteSite(netAddress, state.site);
         Debug.Assert(success);
         success = dsiNet.WritePlant(netAddress, state.plant);
         Debug.Assert(success);
         success = dsiNet.WriteAsset(netAddress, state.asset);
         Debug.Assert(success);

         success = dsiNet.WriteCollectionPoint(netAddress, state.collectionPoint);
         Debug.Assert(success);

         return success;
      }


      /// <summary>
      /// Write the DSI information and the DSI's probe configuration.
      /// </summary>
      private static bool ConfigureDsiInfoAndProbes(this ADsiNetwork dsiNet, byte netAddress,
       UInt16 dsiCount, ADsiInfo dsi)
      {
         dsi.dsiCount = dsiCount;
         dsi.modbusAddress = netAddress;
         dsi.probeCount = (UInt16)dsi.probes.Length;

         bool success = dsiNet.WriteDsiInfo(netAddress, dsi);
         Debug.Assert(success);

#if false // Debug
            var readBackDsiInfo = dsiNet.ReadDsiInfo(netAddress);
            Debug.Assert(readBackDsiInfo.modbusAddress == state.dsi.modbusAddress);
            Console.WriteLine("Wrote and verified modbusAddress {0}", readBackDsiInfo.modbusAddress);
#endif

         return success && dsiNet.ConfigureProbes(netAddress, dsi);
      }


      /// <summary>
      /// Write probe, setup and gate info for each probe in dsi.probes.
      /// </summary>
      private static bool ConfigureProbes(this ADsiNetwork dsiNet, byte netAddress, ADsiInfo dsi)
      {
         bool success = true;
         foreach (AProbe probe in dsi.probes)
         {
            UInt16 probeIndex = (UInt16)(probe.num - 1);

            probe.numSetups = (UInt16)probe.setups.Length;

            success = dsiNet.WriteProbe(netAddress, probeIndex, probe);
            Debug.Assert(success);

            foreach (ASetup setup in probe.setups)
            {
               UInt16 setupNumber = (UInt16)(setup.num - 1);
               success = dsiNet.WriteSetup(netAddress, probeIndex, setupNumber, setup);
               Debug.Assert(success);

               UInt16 gateNumber = 0;
               foreach (AGate gate in setup.gates)
               {
                  success = dsiNet.WriteGate(netAddress, probeIndex, setupNumber, gateNumber, gate);
                  Debug.Assert(success);
                  ++gateNumber;
               }
            }
         }
         return success;
      }
   }
}
