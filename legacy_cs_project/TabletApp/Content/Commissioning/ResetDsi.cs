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
using System.Threading.Tasks;
using TabletApp.Api;
using TabletApp.Properties;
using TabletApp.State;
using TabletApp.Utils;
using DsiExtensions;
using DsiApi;
using System.Windows.Forms;

namespace TabletApp.Content.Commissioning
{
   public partial class ResetDsi : BaseContent
   {
      private ADsiNetwork fDsiNetwork;
      private ADsiInfo fDsiToReset;
      private string fConfigPath = null;
      private int fPrevValidAddress;
      static readonly int kMinAddress = 1;
      static readonly int kMaxUserAddress = 32;
      static readonly int kDefaultAddress = kMinAddress;
      static readonly int kFactoryAddress = 212;



      public ResetDsi(Dictionary<string, string> parameters)
         : base(parameters)
      {
         InitializeComponent();

         AStateController.Instance.StateWillChangeEvent += HandleStateWillChangeEvent;
         AStateController.Instance.PerformActionEvent += HandlePerformAction;

         this.UpdateConfigInfo(null);
         this.factoryAddressCheckbox.Checked = false;
         fPrevValidAddress = kDefaultAddress;
      }

      /// <summary>
      /// About to disappear - clean up.
      /// </summary>
      public override void WillDisappear()
      {
         AStateController.Instance.StateWillChangeEvent -= HandleStateWillChangeEvent;
         AStateController.Instance.PerformActionEvent -= HandlePerformAction;
      }

      private void UpdateConfigInfo(string configPath)
      {
         fConfigPath = configPath;
         this.configPath.Text = (null != fConfigPath ? fConfigPath : Resources.ResetNoConfig);
         this.deleteButton.Visible = (null != fConfigPath);
      }

      private void HandlePerformAction(string actionName, object data)
      {
         if ("loadconfig" == actionName)
         {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            dialog.FilterIndex = 1;
            dialog.RestoreDirectory = false;
            dialog.Multiselect = false;

            if (DialogResult.OK == dialog.ShowDialog())
            {
               this.UpdateConfigInfo(dialog.FileName);
            }
         }
      }

      /// <summary>
      /// Handle state change.  Perform the scan and reset.
      /// </summary>
      /// <param name="newState"></param>
      /// <param name="args"></param>
      private void HandleStateWillChangeEvent(AState newState, string actionName, CancelEventArgs args)
      {
         if ("resetdsi" == actionName)
         {
            bool found = false;
            AStateController.Instance.PostPerformActionEvent("showspinnerprogress", Resources.ResetDsiProgress);
              Task task = Task.Run(
                  async () =>
                  {
                     found = await this.ScanForDsi();
                  }
               );
              task.Wait();
            AStateController.Instance.PostPerformActionEvent("hidespinnerprogress", null);
            if (found)
            {
               if (!fDsiToReset.IsLoggerDsi() && fDsiToReset.numShots > 0)
               {
                  if (DialogResult.Yes == AOutput.DisplayYesNo(Resources.ErrorLoggerHasShotsMessage, Resources.ErrorLoggerHasShotsTitle))
                  {
                     AStateController.Instance.ChangeToState("scan");
                  }
                  else
                  {
                     AStateController.Instance.ChangeToState("home");
                  }
                  args.Cancel = true;
               }
               else
               {
                  if (fDsiNetwork.WriteModBusId((Byte)fDsiToReset.modbusAddress, (Byte)this.dsiAddress.IntValue))
                  {
                     AOutput.DisplayMessage(Resources.DsiResetSuccess);
                  }
                  else
                  {
                     AOutput.DisplayError(Resources.DsiResetFail);
                  }
               }
            }
         }
      }

      /// <summary>
      /// Do the scan for the DSI and store the first DSI address found.
      /// </summary>
      /// <returns></returns>
      private async Task<bool> ScanForDsi()
      {
         bool found = false;

         try
         {
            fDsiNetwork = AApiManager.Instance.InitializeDsiNetwork(fConfigPath);
            await AApiManager.Instance.ScanNetworkAsync(fDsiNetwork, true,
               (ADsiInfo dsiInfo, List<AProbe> probes) =>
               {
                  if (!found)
                  {
                     fDsiToReset = dsiInfo;
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

         return found;
      }

      private void deleteButton_Click(object sender, EventArgs e)
      {
         this.UpdateConfigInfo(null);
      }

      private void factoryAddressCheckbox_CheckedChanged(object sender, EventArgs e)
      {
         if (this.factoryAddressCheckbox.Checked)
         {
            fPrevValidAddress = this.dsiAddress.IntValue;
            this.dsiAddress.Enabled = false;
            this.dsiAddress.MaxValue = kFactoryAddress;
            this.dsiAddress.IntValue = kFactoryAddress;
            
         }
         else
         {
            this.dsiAddress.Enabled = true;
            this.dsiAddress.MaxValue = kMaxUserAddress;
            this.dsiAddress.Value = fPrevValidAddress;
         }
      }
   }
}
