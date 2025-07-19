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

// Enable logging packet communication between hardware and the app to the file modbus.txt.
//#define qLogPackets

// Enable the simulator - be sure to specify the sim xml file to use below
//#define qUseSim

using DsiApi;
using DsiSim;
using DsiUtil;
using Model;
using Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Handlers;
using System.Threading.Tasks;
using TabletApp.Properties;
using TabletApp.Utils;
using DsiExtensions;

using System.Diagnostics;

namespace TabletApp.Api
{
   /// <summary>
   /// Singleton class for handling DsiApi interactions
   /// </summary>
   public class AApiManager : ASingleton<AApiManager>
   {
      ANanoSenseClient fClient = new ANanoSenseClient();

      /// <summary>
      /// Initialize the DSI network, either sim or real.
      /// </summary>
      public ADsiNetwork InitializeDsiNetwork(string xmlConfigOverride = null)
      {
         ADsiNetwork dsiNetwork = null;
         string xmlConfigPath = null;

         if (null != xmlConfigOverride)
         {
            // "none" for the xmlConfigOverride tells us to override the path to null
            if ("none" == xmlConfigOverride)
            {
               xmlConfigPath = null;
            }
            else 
            {
               xmlConfigPath = xmlConfigOverride;
            }
         }

         // create the appropriate ADsiNetwork object - sim vs real
#if qUseSim
         {
            UInt16 numDsisToCreate = 0;

            xmlConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.Combine("Sim", "testdata_3DSIs.xml"));

            string path = "C:\\nanosense";
            Directory.CreateDirectory(path);
            if (null != xmlConfigPath)
            {
               // Create and zero out the 32 dsi files from address 1 to 32...
               ADsiSim.CreateSimFiles(path, 32, reset: true);
            }
            // Or we can just read in any existing DSI files by doing nothing 

            dsiNetwork = new ADsiNetwork(numDsisToCreate, path, 0, null, null, xmlConfigPath);

         }
#else
         {

#if qLogPackets

            string logPath = "C:\\nanosense\\modbus-" + DateTime.Now.ToIsoTimestamp().Replace(':', '-') + ".txt";
            StreamWriter sw = new StreamWriter(new FileStream(logPath, FileMode.Append));
            Console.SetOut(sw);
            Action<byte[]> writeCallback = (byte[] buffer) => { Tracing.Print(buffer, "Sending"); };
            Action<byte[]> readCallback = (byte[] buffer) => { Tracing.Print(buffer, "Receive"); };

#else
            Action<byte[]> readCallback = null;
            Action<byte[]> writeCallback = null;
#endif // qLogPackets

            dsiNetwork = new ADsiNetwork(Settings.Default.ComPort, Settings.Default.ComBaudRate, 
               Settings.Default.LowLevelTimeout, Settings.Default.LowLevelRetryCount,
               readCallback, writeCallback, xmlConfigPath);
         }
#endif // qUseSim

         return dsiNetwork;
      }

      /// <summary>
      /// Initialize DSI network for a specific COM port (overload for multi-port support)
      /// </summary>
      public ADsiNetwork InitializeDsiNetwork(string comPort, string xmlConfigOverride = null)
      {
         ADsiNetwork dsiNetwork = null;
         string xmlConfigPath = null;

         if (null != xmlConfigOverride)
         {
            // "none" for the xmlConfigOverride tells us to override the path to null
            if ("none" == xmlConfigOverride)
            {
               xmlConfigPath = null;
            }
            else 
            {
               xmlConfigPath = xmlConfigOverride;
            }
         }

#if qUseSim
         // Existing simulator code...
         UInt16 numDsisToCreate = 0;
         xmlConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.Combine("Sim", "testdata_3DSIs.xml"));
         string path = "C:\\nanosense";
         Directory.CreateDirectory(path);
         if (null != xmlConfigPath)
         {
            ADsiSim.CreateSimFiles(path, 32, reset: true);
         }
         dsiNetwork = new ADsiNetwork(numDsisToCreate, path, 0, null, null, xmlConfigPath);
#else
         {
#if qLogPackets
            string logPath = "C:\\nanosense\\modbus-" + comPort + "-" + DateTime.Now.ToIsoTimestamp().Replace(':', '-') + ".txt";
            StreamWriter sw = new StreamWriter(new FileStream(logPath, FileMode.Append));
            Console.SetOut(sw);
            Action<byte[]> writeCallback = (byte[] buffer) => { Tracing.Print(buffer, "Sending"); };
            Action<byte[]> readCallback = (byte[] buffer) => { Tracing.Print(buffer, "Receive"); };
#else
            Action<byte[]> readCallback = null;
            Action<byte[]> writeCallback = null;
#endif // qLogPackets

            // Use provided COM port instead of settings
            dsiNetwork = new ADsiNetwork(comPort, Settings.Default.ComBaudRate, 
               Settings.Default.LowLevelTimeout, Settings.Default.LowLevelRetryCount,
               readCallback, writeCallback, xmlConfigPath);
         }
#endif // qUseSim

         return dsiNetwork;
      }

      /// <summary>
      /// Convenient routine for scanning the DSI network.  This takes care of any timeout handling
      /// and return value validation.
      /// </summary>
      /// <param name="dsiNetwork">The DSI network</param>
      /// <param name="callback">Optional callback</param>
      /// <returns>Task</returns>
      public async Task ScanNetworkAsync(ADsiNetwork dsiNetwork, bool stopIfFactory, Action<ADsiInfo, List<AProbe>> callback)
      {
         try
         {
            int dsiCount = 0;
            const int timeout = 300000;

            var task = dsiNetwork.ScanNetworkASync(stopIfFactory, 
               (ADsiInfo dsiInfo, List<AProbe> probes) =>
               {
                  //Console.WriteLine("Found dsi {0}", dsiInfo.modbusAddress);
                  ++dsiCount;
                  if (null != callback)
                  {
                     if (dsiInfo.IsLoggerDsi())
                     {
                        // set the DSIs number of shots to the total number of logger shots divided the number of probes
                        // account for the possibility of it not being an exactly multiple of probeCount (eg if a shot failed)
                        dsiInfo.numShots = (int)Math.Ceiling(((float)dsiNetwork.ReadDataLoggerNumShots((byte)dsiInfo.modbusAddress) / (float)dsiInfo.probeCount));
                     }
                     callback(dsiInfo, probes);
                  }
               });

            // execute the task with a timeout
            if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
            {
               // task completed within timeout
               Tuple<List<byte>, UInt16> result = await task;

               if (task.IsCanceled)
               {
                  throw new OperationCanceledException();
               }
               // check for task exception
               if (task.IsFaulted)
               {
                  throw task.Exception;
               }
               if (0 == result.Item1.Count || 0 == result.Item2)
               {
                  throw new ADsiCountException(Resources.ErrorNoDsisFound);
               }
               else if (result.Item1.Count != dsiCount)
               {
                  throw new ADsiCountException(Resources.ErrorScannedCounts);
               }
               else if (result.Item1.Count != result.Item2)
               {
                  throw new ADsiCountException(Resources.ErrorScannedCounts2);
               }
            }
            else
            {
               // task timed out
               throw new Exception(Resources.ErrorScanNetworkTimeout);
            }
         }
         catch (Exception e)
         {
            // log our exception and rethrow
            AOutput.LogException(e);
            throw e;
         }
      }

      /// <summary>
      /// Perform measurement of all our available DSIs/probes.
      /// </summary>
      public async Task PerformMeasurementsAsync(ADsiNetwork dsiNetwork, ANanoSense dsiItem, DateTime timestamp, AProbe onlyThisProbe, Action<AMeasurementParam, AProbe, String> callback)
      {
         try
         {
            const int timeout = 240000;
            
            // for the dsi, perform measurement on all of its probes
            List<AMeasurementParam> parms = new List<AMeasurementParam>();
            parms.Add(new AMeasurementParam() { fDsiAddress = (byte)dsiItem.Dsi.modbusAddress, fProbes = (null != onlyThisProbe ? new AProbe[1] { onlyThisProbe } : dsiItem.Dsi.probes) });

            var task = dsiNetwork.PerformMeasurementsAsync(parms, timestamp, callback);

            // execute the task with a timeout
            if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
            {
               if (task.IsCanceled)
               {
                  throw new OperationCanceledException();
               }
               // check for task exception
               if (task.IsFaulted)
               {
                  throw task.Exception;
               }
            }
            else
            {
               // task timed out
               throw new Exception(Resources.ErrorReadProbesTimeout);
            }
         }
         catch (Exception e)
         {
            // log our exception and rethrow
            AOutput.LogException(e);
            throw e;
         }
      }

      /// <summary>
      /// Read datalogger shots from the given DSI.
      /// </summary>
      public async Task ReadDataLoggerShotsAsync(ADsiNetwork dsiNetwork, ANanoSense dsiItem, Action<AMeasurementParam, AProbe, String> callback)
      {
         try
         {
            const int timeout = 86400000;    // 24 hours

            AMeasurementParam param = new AMeasurementParam() { fDsiAddress = (byte)dsiItem.Dsi.modbusAddress, fProbes = dsiItem.Dsi.probes };

            var task = dsiNetwork.ReadDataLoggerShotsAsync(param, callback);

            // execute the task with a timeout
            if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
            {
               if (task.IsCanceled)
               {
                  throw new OperationCanceledException();
               }
               // check for task exception
               if (task.IsFaulted)
               {
                  throw task.Exception;
               }
            }
            else
            {
               // task timed out
               throw new Exception(Resources.ErrorReadProbesTimeout);
            }
         }
         catch (Exception e)
         {
            // log our exception and rethrow
            AOutput.LogException(e);
            throw e;
         }
      }

      /// <summary>
      /// Perform a login on the NanoSense server.
      /// </summary>
      public async Task LoginAsync(ANanoSense nano)
      {
         await this.LoginAsync(nano.Dsi.cloudAppUrl, nano.Dsi.cloudAppUserName, nano.Dsi.cloudAppPassword);
      }

      public async Task LoginAsync(string url, string user, string pass)
      {
         try
         {
            const int timeout = 30000;
            fClient.SetBaseUrl(url.FullyQualifiedUri());
            Task task = fClient.Login(user, pass);
            if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
            {
               if (task.IsCanceled)
               {
                  throw new OperationCanceledException();
               }
               // check for task exception
               if (task.IsFaulted)
               {
                  throw task.Exception;
               }
            }
            else
            {
               // task timed out
               throw new Exception(Resources.ErrorLoginTimeout);
            }
         }
         catch (Exception e)
         {
            // log our exception and rethrow
            AOutput.LogException(e);
            throw e;
         }
      }

      /// <summary>
      /// Perform a logout
      /// </summary>
      public async Task LogoutAsync()
      {
         try
         {
            const int timeout = 30000;
            Task task = fClient.Logout();
            if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
            {
               if (task.IsCanceled)
               {
                  throw new OperationCanceledException();
               }
               // check for task exception
               if (task.IsFaulted)
               {
                  throw task.Exception;
               }
            }
            else
            {
               // task timed out
               throw new Exception(Resources.ErrorLogoutTimeout);
            }
         }
         catch (Exception e)
         {
            // log our exception and rethrow
            AOutput.LogException(e);
            throw e;
         }
      }
      
      /// <summary>
      /// Get the total byte count for all the files represented by the list of ANanoSense objects.
      /// </summary>
      /// <param name="dsis">List of ANanoSense objects</param>
      /// <returns>Total bytes</returns>
      private long GetTotalUploadBytes(List<ANanoSense> dsis)
      {
         long total = 0;

         foreach (ANanoSense dsi in dsis)
         {
            if (null != dsi.backingFilePath)
            {
               try
               {
                  FileInfo info = new FileInfo(dsi.backingFilePath);
                  total += info.Length * 2;     // !!! TODO: this accounts for UTF8 encoding, I think - need a better way to get the upload size
               }
               catch (Exception)
               {
                  // fail silently
               } 
            }
         }

         return total;
      }

      /// <summary>
      /// Asynchronous upload of the given list of DSIs.  Each DSI must have already been saved and have
      /// their <code>backingFilePath</code> var set.
      /// </summary>
      /// <param name="dsis">List of ANanoSense objects</param>
      /// <param name="progressCallback">Optional callback for progress information</param>
      /// <returns></returns>
      public async Task UploadAsync(List<ANanoSense> dsis, Action<int, int, long, long, bool, string> progressCallback = null)
      {
         try
         {
            await this.LoginAsync(dsis.FirstOrDefault());

            ProgressMessageHandler progressHandler = fClient.GetProgressHandler();
            int currentFile = 1;
            int totalFiles = dsis.Count;
            long totalAccumBytes = 0;
            long totalBytes = this.GetTotalUploadBytes(dsis);
            foreach (String filePath in dsis.ToFileList())
            {
               if (null != filePath)
               {
                  EventHandler<HttpProgressEventArgs> eventHandler = null;
                  string exceptionString = null;

                  try
                  {
                     const int timeout = 300000;    // !!! TODO: what is the proper timeout here?
                     
                     if (progressCallback != null)
                     {
                        // setup an event handler for this file upload
                        eventHandler = (sender, args) =>
                        {
                           totalAccumBytes += args.BytesTransferred;
                           if (null != progressCallback)
                           {
                              progressCallback(currentFile, totalFiles, totalAccumBytes, totalBytes, false, null);
                           }
                        };
                        progressHandler.HttpSendProgress += eventHandler;
                     }

                     Task task = fClient.UploadXmlFile(filePath);
                     if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
                     {
                        if (task.IsCanceled)
                        {
                           throw new OperationCanceledException();
                        }
                        // check for task exception
                        if (task.IsFaulted)
                        {
                           throw task.Exception;
                        }
                     }
                     else
                     {
                        // task timed out
                        throw new Exception(Resources.ErrorUploadTimeout);
                     }

                     // upload complete - inform the client
                     if (null != progressCallback)
                     {
                        progressCallback(currentFile, totalFiles, totalAccumBytes, totalBytes, true, null);
                     }
                  }
                  catch (OperationCanceledException oce)
                  {
                     throw oce;
                  }
                  catch (Exception e)
                  {
                     exceptionString = e.AggregateMessage();
                  }
                  finally
                  {
                     if (null != eventHandler)
                     {
                        progressHandler.HttpSendProgress -= eventHandler;
                     }
                     // inform the caller of the failed upload and move on to the next
                     if (null != progressCallback)
                     {
                        progressCallback(currentFile, totalFiles, totalAccumBytes, totalBytes, true, exceptionString);
                     }
                  }
               }
               ++currentFile;
            }
            await this.LogoutAsync();
         }
         catch (Exception e)
         {
            // log our exception and rethrow
            AOutput.LogException(e);
            throw e;
         }
      }

      /// <summary>
      /// Set the real time clock on the DSI.
      /// </summary>
      /// <param name="dsiNetwork"></param>
      /// <param name="nano"></param>
      public void SetRealTimeClock(ADsiNetwork dsiNetwork, ANanoSense nano)
      {
         dsiNetwork.SetRealTimeClock((byte)nano.Dsi.modbusAddress, DateTime.UtcNow);
      }
   }
}

