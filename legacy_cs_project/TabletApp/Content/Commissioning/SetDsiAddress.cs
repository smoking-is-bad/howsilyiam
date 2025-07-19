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
using TabletApp.Api;
using TabletApp.Properties;
using TabletApp.State;
using TabletApp.Utils;
using DsiExtensions;
using DsiApi;
using System.Windows.Forms;

namespace TabletApp.Content.Commissioning
{
   /// <summary>
   /// User control for the Set DSI Address screen.  Set the DSI address to the next available address
   /// and do a read to confirm the change.
   /// </summary>
   public class SetDsiAddress : Message
   {
      private AMasterDsiInfo fMasterDsiInfo;

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="parameters">Parameters from the state def file</param>
      public SetDsiAddress(Dictionary<string, string> parameters)
         : base(parameters)
      {
         AStateController.Instance.StateWillChangeEvent += HandleStateWillChangeEvent;
         Debug.Assert(AStateController.Instance.GlobalData.ContainsKey("masterdsi"));
         fMasterDsiInfo = (AMasterDsiInfo)AStateController.Instance.GlobalData["masterdsi"];
         Debug.Assert(fMasterDsiInfo.DsiItems.Count > 0);
         this.MessageText = String.Format(this.MessageText, fMasterDsiInfo.DsiItems.MaxAddress(fMasterDsiInfo.CurrentDsi - 1) + 1);
         bool add = (AStateController.Instance.GlobalData.ContainsKey("adddsi") && "true" == (string)AStateController.Instance.GlobalData["adddsi"]);
         bool replace = (AStateController.Instance.GlobalData.ContainsKey("replacedsi") && "true" == (string)AStateController.Instance.GlobalData["replacedsi"]);
         ANanoSense nano = fMasterDsiInfo.DsiItems[fMasterDsiInfo.CurrentDsi];
         bool isFactory = (ADsiNetwork.kFactoryId == nano.Dsi.modbusAddress);

         // hide the reset button for add/replace path and for 212 devices
         if (add || replace || isFactory)
         {
            AStateController.Instance.PostPerformActionEvent("hidebutton", "reset");
         }
         // otherwise tag on the reset part of the message
         else
         {
            this.MessageText += "\r\n\r\n" + Resources.MessageWarningReset;
         }
      }

      /// <summary>
      /// Respond to our control being shown - validate the DSI model.
      /// </summary>
      public override void DidShow()
      {
         ANanoSense nano = fMasterDsiInfo.DsiItems[fMasterDsiInfo.CurrentDsi];
         if (nano.Dsi.IsLoggerDsi() && nano.Dsi.numShots > 0)
         {
            if (DialogResult.Yes == AOutput.DisplayYesNo(Resources.ErrorLoggerHasShotsMessage, Resources.ErrorLoggerHasShotsTitle))
            {
               AStateController.Instance.ChangeToState("scan");
            }
            else
            {
               AStateController.Instance.ChangeToState("home");
            }
         }
      }

      /// <summary>
      /// About to disappear - clean up.
      /// </summary>
      public override void WillDisappear()
      {
         AStateController.Instance.StateWillChangeEvent -= HandleStateWillChangeEvent;
      }

      /// <summary>
      /// Handle state change.  Set the DSI address per the CurrentDsi in our master data, then
      /// read from the DSI to confirm the change.
      /// </summary>
      /// <param name="newState"></param>
      /// <param name="args"></param>
      private void HandleStateWillChangeEvent(AState newState, string actionName, CancelEventArgs args)
      {
         bool replace = (AStateController.Instance.GlobalData.ContainsKey("replacedsi") && "true" == (string)AStateController.Instance.GlobalData["replacedsi"]);

         if ("setaddress" == actionName)
         {
            try
            {
               // set the new address and then validate that by reading the dsi info
               ANanoSense nano = fMasterDsiInfo.DsiItems[fMasterDsiInfo.CurrentDsi];
               // if we're replacing, the new address to set is the the CurrentDsi's modbus addr
               // and the physical DSI to replace is at 212
               // otherwise, if we're adding, we just increment the CurrentDsi
               Byte newAddress = (replace ? (Byte)nano.Dsi.modbusAddress : (Byte)(fMasterDsiInfo.DsiItems.MaxAddress(fMasterDsiInfo.CurrentDsi - 1) + 1));
               fMasterDsiInfo.Dsi.WriteModBusId((replace ? ADsiNetwork.kFactoryId : (Byte)nano.Dsi.modbusAddress), newAddress);
               AMarshaledDsiInfo newDsiInfo = fMasterDsiInfo.Dsi.ReadDsiInfo(newAddress);
               if (null != newDsiInfo)
               {
                  nano.Dsi.modbusAddress = newAddress;
                  // tell DSI data screen to hide the web login
                  AStateController.Instance.GlobalData["hideweblogin"] = true;
               }
               else
               {
                  AOutput.DisplayError(String.Format(Resources.ErrorCommSetAddressRead, newAddress));
                  args.Cancel = true;
               }
            }
            catch (Exception e)
            {
               AOutput.DisplayError(Resources.ErrorCommSetAddress + " " + e.AggregateMessage());
               args.Cancel = true;
            }
         }
         else if ("resetaddress" == actionName)
         {
            ANanoSense nano = fMasterDsiInfo.DsiItems[fMasterDsiInfo.CurrentDsi];
            if (fMasterDsiInfo.Dsi.WriteModBusId((Byte)nano.Dsi.modbusAddress, ADsiNetwork.kFactoryId))
            {
               AOutput.DisplayMessage(Resources.DsiResetSuccess);
               // decrement our dsi count to erase the added one
               if (!replace)
               {
                  --fMasterDsiInfo.CurrentDsi;
               }
            }
            else
            {
               AOutput.DisplayError(Resources.DsiResetFail);
               args.Cancel = true;
            }
         }
         else if ("back" == actionName)
         {
            bool add = (AStateController.Instance.GlobalData.ContainsKey("adddsi") && "true" == (string)AStateController.Instance.GlobalData["adddsi"]);

            // decrement our dsi count to erase the added one
            if (!replace)
            {
               --fMasterDsiInfo.CurrentDsi;
            }
            if (add)
            {
               fMasterDsiInfo.DsiItems.RemoveAt(fMasterDsiInfo.DsiItems.Count - 1);
            }
         }
      }
   }
}

