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

using Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabletApp.Api;
using TabletApp.Properties;
using TabletApp.State;
using TabletApp.Utils;
using DsiExtensions;
using DsiApi;
using System.Windows.Forms;
using System.IO;

namespace TabletApp.Content.Commissioning
{
   /// <summary>
   /// User control for the Connect DSI screen - prompt the user to connect and then scan for the DSI.
   /// </summary>
   public class ConnectDsi : Message
   {
      private AMasterDsiInfo fMasterDsiInfo;
      private bool fCurrentBumped = false;

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="parameters">Parameters from the state def file</param>
      public ConnectDsi(Dictionary<string, string> parameters)
         : base(parameters)
      {
         AStateController.Instance.StateWillChangeEvent += HandleStateWillChangeEvent;
         Debug.Assert(AStateController.Instance.GlobalData.ContainsKey("masterdsi"));
         fMasterDsiInfo = (AMasterDsiInfo)AStateController.Instance.GlobalData["masterdsi"];
         Debug.Assert(fMasterDsiInfo.DsiItems.Count > 0);
      }

      /// <summary>
      /// About to disappear - clean up.
      /// </summary>
      public override void WillDisappear()
      {
         AStateController.Instance.StateWillChangeEvent -= HandleStateWillChangeEvent;
      }

      /// <summary>
      /// Handle state change.  Perform the scan and handle the canceled state in case of an error.
      /// </summary>
      /// <param name="newState"></param>
      /// <param name="args"></param>
      private void HandleStateWillChangeEvent(AState newState, string actionName, CancelEventArgs args)
      {
         bool result = false;
         if ("readdsi" == actionName)
         {
            AStateController.Instance.PostPerformActionEvent("showspinnerprogress", Resources.ConnectDsiProgress);
            Task task = Task.Run(
                async () =>
                {
                   result = await this.ScanForDsi();
                }
             );
            task.Wait();
            args.Cancel = !result;
            AStateController.Instance.PostPerformActionEvent("hidespinnerprogress", null);
         }
         else if ("back" == actionName)
         {
            bool replace = (AStateController.Instance.GlobalData.ContainsKey("replacedsi") && "true" == (string)AStateController.Instance.GlobalData["replacedsi"]);
            bool add = (AStateController.Instance.GlobalData.ContainsKey("adddsi") && "true" == (string)AStateController.Instance.GlobalData["adddsi"]);

            // decrement our dsi count to erase the added one
            if (!replace && fCurrentBumped)
            {
               --fMasterDsiInfo.CurrentDsi;
            }
            if (add)
            {
               fMasterDsiInfo.DsiItems.RemoveAt(fMasterDsiInfo.DsiItems.Count - 1);
            }
         }
      }

      /// <summary>
      /// Do the scan for the DSI and store the basic info.
      /// </summary>
      /// <returns></returns>
      private async Task<bool> ScanForDsi()
      {
         bool found = false;

         try
         {
            bool add = (AStateController.Instance.GlobalData.ContainsKey("adddsi") && "true" == (string)AStateController.Instance.GlobalData["adddsi"]);
            bool replace = (AStateController.Instance.GlobalData.ContainsKey("replacedsi") && "true" == (string)AStateController.Instance.GlobalData["replacedsi"]);
            bool copy = (AStateController.Instance.GlobalData.ContainsKey("copydsi") && "true" == (string)AStateController.Instance.GlobalData["copydsi"]);
            bool newNetwork = !(add || replace);
            //string configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.Combine("Commissioning", "DsiConfig.xml"));
            //fMasterDsiInfo.Dsi = AApiManager.Instance.InitializeDsiNetwork(replace && copy ? "none" : configFile);
            fMasterDsiInfo.Dsi = AApiManager.Instance.InitializeDsiNetwork();
            List<ushort> currAddresses = new List<ushort>();

            for (int i = 0; i <= fMasterDsiInfo.CurrentDsi; ++i)
            {
               ANanoSense nano = fMasterDsiInfo.DsiItems[i];
               currAddresses.Add(nano.Dsi.modbusAddress);
            }
            await AApiManager.Instance.ScanNetworkAsync(fMasterDsiInfo.Dsi, true,
               (ADsiInfo dsiInfo, List<AProbe> probes) =>
               {
                  // we allow any address for the New Network path
                  // we allow any address that's not already in the list for the Add/Replace path
                  if (!found && (newNetwork || (!newNetwork && !currAddresses.Contains(dsiInfo.modbusAddress))))
                  {
                     if (replace)
                     {
                        // if replacing, either use the scanned DSI as is, or copy from the one we're replacing
                        // denoted by the CurrentDsi index
                        ANanoSense nanoToReplace = fMasterDsiInfo.DsiItems[fMasterDsiInfo.CurrentDsi];
                        // "copy" means we are copying the old dsi info into the new device
                        // since we are using the dsi at CurrentDsi for the new device, if "copy" is on
                        // we just want to keep that info the same - if "copy" is off, then we need to
                        // copy the new dsi info to the CurrentDsi index
                        if (!copy)
                        {
                           ushort addr = nanoToReplace.Dsi.modbusAddress;
                           nanoToReplace.Dsi = dsiInfo;
                           nanoToReplace.Dsi.modbusAddress = addr;
                        }
                     }
                     else
                     {
                        // if we're not replacing a DSI, increment our current to indicate we're adding one
                        ++fMasterDsiInfo.CurrentDsi;
                        fCurrentBumped = true;
                        // just modify the existing nano that got added on the New screen
                        ANanoSense nano = fMasterDsiInfo.DsiItems[fMasterDsiInfo.CurrentDsi];
                        ADsiInfo tempDsi = nano.Dsi;
                        nano.Dsi = dsiInfo;
                        // copy over the relevant info from initialization
                        nano.Dsi.Longitude = tempDsi.Longitude;
                        nano.Dsi.Latitude = tempDsi.Latitude;
                        nano.Dsi.dsiCount = tempDsi.dsiCount;
                        nano.Dsi.probeCount = tempDsi.probeCount;
                        nano.Dsi.tag = tempDsi.tag;
                     }
                     found = true;
                  }
               });
         }
         catch (OperationCanceledException)
        {
            // silent fail
         }
         catch (ADsiCountException)
         {
            // silent fail
         }
         catch (Exception e)
         {
            AOutput.DisplayError(Resources.ErrorScanNetwork + " " + e.AggregateMessage());
         }

         if (!found)
         {
            if (DialogResult.Yes == AOutput.DisplayYesNo(Resources.ErrorCommDsiNotFound, Resources.GeneralMessageCaption))
            {
               found = await this.ScanForDsi();
            }
         }
         else
         {
            // update dsi count for each dsi
            foreach (ANanoSense nano in fMasterDsiInfo.DsiItems)
            {
               nano.Dsi.dsiCount = (ushort)fMasterDsiInfo.DsiItems.Count;
               fMasterDsiInfo.Dsi.WriteDsiCount((byte)nano.Dsi.modbusAddress, nano.Dsi.dsiCount);
            }
         }

         return found;
      }
   }
}

