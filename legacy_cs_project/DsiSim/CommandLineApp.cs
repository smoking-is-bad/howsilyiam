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
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

using DsiApi;
using DsiUtil;
using Logging;
using Model;
using Network;

namespace DsiSim
{
   class CommandLineApp
   {
      // Example command line args
      // sim "C:\Users\Jason Bagley\AppData\Local\ConsoleAppDsis" 12 "TabletApp\Sim\singleProbe.xml"
      // port <port name> <baud rate> 
      // port COM4 115200 "TabletApp\Sim\threeProbes.xml"
      public void Run(string[] args)
      {
         try
         {
            if (args.Length >= 1 && 0 == String.Compare(args[0], "/h"))
            {
               this.PrintUsage();
               goto exit;
            }

				ALog.Initialize("DsiSim", "DsiSimLog.txt");

            if (args.Length == 0 || args[0] == "port")
            {
               if (args.Length > 4)
               {
                  this.PrintUsage();
                  goto exit;
               }

               string portName = "COM5"; // COM3 default ftdi port name with dev board; COM4 default port name for DSI prototype
               if (args.Length >= 2)
               {
                  portName = args[1];
               }

               int baudRate = 115200;
               if (args.Length >= 3)
               {
                  baudRate = Convert.ToInt32(args[2]);
               }

               string initPath = null;
               if (args.Length >= 4)
               {
                  initPath = args[3];
               }

               this.InitializeNetwork(portName, baudRate, null);
               var scan1Task = this.ScanNetwork();
               scan1Task.Wait();
               fValidIds = scan1Task.Result;
               fDsi.InitializeNetworkWithSettings(initPath);

               fDsiAddress = fValidIds.Count > 0 ? fValidIds[0] : (Byte)0x01;

               ACompany company = fDsi.ReadCompany(fDsiAddress);
               if (Object.ReferenceEquals(null, company))
               {
                  Console.WriteLine("The board could not be read. Is it attached? Does it need a power cycle?");
                  goto exit;
               }
               Console.WriteLine("{0} Company# {1}", company.name, company.id);

               AProbe probe = fDsi.ReadProbe(fDsiAddress, 0);
               ASetup setup = fDsi.ReadSetup(fDsiAddress, 0, 0);

               setup.switchSettings = 0x040a;
               fDsi.WriteSetup(fDsiAddress, 0, 0, setup);

#if false
               Console.WriteLine("Current firmware ignores modbus id and does not have default, but our code requires it.");
               Console.WriteLine("Setting Modbus ID to 0x01.");
               if (!fDsi.WriteModBusId(ADsiNetwork.kFactoryId, 0x01))
               {
                  Console.WriteLine("Failed to set Modbus ID.");
                  Console.WriteLine("Further results are undefined. Exiting.");
                  goto exit;
               }
               fDsiAddress = 0x01; //!!!: Doesn't matter for now.
#endif

            }
            else if (args[0] == "sim")
            {
               if (args.Length < 2 || args.Length > 4)
               {
                  this.PrintUsage();
                  goto exit;
               }

               string simPath = args[1];
               byte numDsis = 0;
               if (args.Length >= 3)
               {
                  numDsis = Convert.ToByte(args[2]);
               }
               string settingsFile = null;
               if (args.Length >= 4)
               {
                  settingsFile = args[3];
               }
               this.InitializeNetworkSim(simPath, numDsis, settingsFile);
            }
            else
            {
               this.PrintUsage();
               goto exit;
            }


#if true // Disable firing while testing XML upload

#if true
            if (null == fValidIds)
            {
               var task = this.ScanNetwork();
               task.Wait();
               fValidIds = task.Result;
            }
#else
            fValidIds = new List<byte> { 1 };  
#endif

            this.FireSetup(fValidIds);

#else // WIP XML Upload

            this.UploadXml().Wait();

#endif
         }
         catch (AServerException serverEx)
         {
            Console.WriteLine(serverEx.Response.description);
            Console.WriteLine(serverEx.Response.technicalDetails);
         }
         catch (Exception ex)
         {
            Console.WriteLine("Unhandled exception: " + ex.ToString());
         }

exit:
         Console.WriteLine("Press any key to exit.");
         Console.ReadKey();
         if (null != fClient)
         {
            fClient.Logout().Wait();
         }
      }


      private void FireSetup(List<byte> ids)
      {
         Console.WriteLine("**Firing a setup...");
         Console.Write("Choose a DSI address [");
         this.PrintRuns(ids);
         Console.Write("]:");
         byte address = Convert.ToByte(Console.ReadLine());
         byte probe = 16;
         while (probe < 0 || probe > 15)
         {
            Console.Write("Probe number [0-15]:");
            probe = Convert.ToByte(Console.ReadLine());
         }
         byte setup = 8;
         while (setup < 0 || setup > 7)
         {
            Console.Write("Setup number [0-7]:");
            setup = Convert.ToByte(Console.ReadLine());
         }

			String errorMessage;
         ASetup setupWithResults = fDsi.FireShot(address, probe, setup,
				DateTime.UtcNow.ToIsoTimestamp(), out errorMessage);

         if (null == errorMessage)
         {
            Console.WriteLine("Measured thickness {0} on {1}", setupWithResults.calculatedThickness,
               setupWithResults.timestamp);
         }
			else
			{
				Console.WriteLine("Error firing shot: {0}", errorMessage);
			}
      }

      private void InitializeNetwork(string portName, int baudRate, string settingsPath)
      {
         Console.WriteLine("Initializing serial device at {0} {1}.",
          portName, baudRate.ToString());
         fDsi = new ADsiNetwork(portName, baudRate,
          (byte[] buffer) => { buffer.Print("Sending"); },
          (byte[] buffer) => { buffer.Print("Receive"); },
          settingsPath);
      }


      private void InitializeNetworkSim(string path, byte numDsis, string settingsPath = null)
      {
         Console.WriteLine("Initializing sim at {0}.", path);
         fDsi = new ADsiNetwork(numDsis, path, 0,
          (byte[] buffer) => { buffer.Print("Sending"); },
          (byte[] buffer) => { buffer.Print("Receive"); },
          settingsPath);
      }


      private async Task<List<byte>> ScanNetwork()
      {
         Console.WriteLine("**Scanning network...");
         Tuple<List<byte>, UInt16> scanResult = await fDsi.ScanNetworkASync(
          (ADsiInfo dsiInfo, List<AProbe> probes) =>
          {
             Console.WriteLine("Dsi#{0}, |P|={1}", dsiInfo.modbusAddress, probes.Count);
          });
         List<byte> ids = scanResult.Item1;
         UInt16 numDsis = scanResult.Item2;
         Console.WriteLine("Number of valid addresses from scan: {0}", ids.Count);
         Console.WriteLine("Number of DSIs read from random responding DSI: {0}", numDsis);

         return ids;
      }

      private void PrintRuns(List<byte> ids)
      {
         bool runStart = true;
         byte lastId = 255;
         string runClosing = "";
         foreach (byte id in ids)
         {
            if (lastId != (id - 1))
            {
               if (runStart)
               {
                  Console.Write(runClosing + id);
                  runStart = false;
               }
               else
               {
                  Console.Write("-" + id);
                  runClosing = ", ";
                  runStart = true;
               }
            }

            lastId = id;
         }

         if (ids.Count > 1 && !runStart)
         {
            Console.Write("-" + lastId);
         }
      }


      private void PrintUsage()
      {
         // TODO: Update. This doesn't have numDsis or initilization file path."
         Console.WriteLine("dsi [port [<port name> [<baud rate>]] | [sim <path\\to\\sim\\directory>");
         Console.WriteLine("  port - Connect to a dsi on a serial port.");
         Console.WriteLine("  port name (optional) - The name of the serial port. Default is \"COM3\".");
         Console.WriteLine("                         If the port name is given a baud rate may also be specified.");
         Console.WriteLine("  baud rate (optional) - The communication rate of the port. Default is 115200.");
         Console.WriteLine("  sim - Connect to a dsi simulation directory");
         Console.WriteLine("  path to sim directory - The directory containing dsi sim files.");
      }


      private async Task UploadXml()
      {
         fClient = new ANanoSenseClient();
         Console.WriteLine("Logging in as jbagley.");
         var networkTask = fClient.Login("jbagley", "8ngKxUPge");
         networkTask.Wait();

         Console.WriteLine("Current dir is " + Directory.GetCurrentDirectory());
         Console.Write("***Enter path to XML to upload: ");
         var xmlPath = Console.ReadLine();
         Console.WriteLine("Uploading");

         await fClient.UploadXmlFile(xmlPath, setTestId: true);
      }


      private async void Login(string username, string password)
      {
         using (var client = new HttpClient())
         {
            string url = "http://nanosense.artlogicdev.net/admin?username=" + username + "&" + password;
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
            Console.WriteLine(await response.Content.ReadAsStringAsync());
         }
      }

      private ADsiNetwork fDsi;
      private byte fDsiAddress;
      private List<byte> fValidIds;
      private ANanoSenseClient fClient;
   }
}

