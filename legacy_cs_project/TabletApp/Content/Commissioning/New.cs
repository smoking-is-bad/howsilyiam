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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Device.Location;
using TabletApp.Properties;
using TabletApp.State;
using TabletApp.Utils;
using TabletApp.Autofill;
using System.IO;
using Model;
using System.Diagnostics;
using TabletApp.Api;
using System.Threading;

namespace TabletApp.Content.Commissioning
{
   /// <summary>
   /// Screen for capturing info for a new network.  Auto-populate drop-downs.  Store info
   /// to the state globals.
   /// </summary>
   public partial class New : BaseContent
   {
      private bool fEditMode = false;

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="parameters">Parameters from the state def file</param>
      public New(Dictionary<string, string> parameters)
         : base(parameters)
      {
         InitializeComponent();

         AStateController.Instance.StateWillChangeEvent += HandleStateWillChangeEvent;

         this.PopulateCompanyComboBox();

         fEditMode = fParams.ContainsKey("edit") && fParams["edit"] == "true";

         if (fEditMode)
         {
            this.PopulateCurrentInfo();
         }

         // hide the New Company button (for now) - assumes SNI will provide pre-populated company.xml files to customers
         this.newCompanyButton.Visible = false;
      }

      /// <summary>
      /// About to disappear - clean up.
      /// </summary>
      public override void WillDisappear()
      {
         AStateController.Instance.StateWillChangeEvent -= HandleStateWillChangeEvent;
      }

      /// <summary>
      /// Get the string to display the company combo for the given ACompany
      /// </summary>
      /// <param name="company"></param>
      /// <returns></returns>
      private string ComboStringForCompany(ACompany company)
      {
         return company.name + " " + company.id;
      }

      /// <summary>
      /// Populate the UI based on the current info from the first DSI in our string.
      /// </summary>
      private void PopulateCurrentInfo()
      {
         AMasterDsiInfo master = (AMasterDsiInfo)AStateController.Instance.GlobalData["masterdsi"];
         ANanoSense nano = master.DsiItems[master.CurrentDsi];
         IList<ACompany> companies = AAutofillManager.Instance.Source.GetCompanyList();
         if (!companies.Contains(nano.company))
         {
            this.companyComboBox.Items.Add(this.ComboStringForCompany(nano.company));
         }
         this.companyComboBox.Text = this.ComboStringForCompany(nano.company);
         this.siteComboBox.Text = (string)nano.site.name;
         this.assetComboBox.Text = (string)nano.asset.name;
         this.plantComboBox.Text = (string)nano.plant.name;
         this.cpNameTextBox.Text = nano.collectionPoint.name;
         this.cpDescTextBox.Text = nano.collectionPoint.description;
         this.latLongView.Latitude = nano.Dsi.Latitude;
         this.latLongView.Longitude = nano.Dsi.Longitude;
         this.numDsisTextBox.IntValue = master.DsiItems.Count;
         this.numDsisTextBox.Enabled = false;
      }

      /// <summary>
      /// Add the companies from the auto-fill list to our company comboxbox.
      /// </summary>
      private void PopulateCompanyComboBox()
      {
         IList<ACompany> companies = AAutofillManager.Instance.Source.GetCompanyList();
         if (companies.Count > 0)
         {
            this.companyComboBox.Items.AddRange(companies.Select(company => this.ComboStringForCompany(company)).ToArray());
            this.companyComboBox.SelectedIndex = 0;
         }
      }

      /// <summary>
      /// Add the auto-fill sites/assets/plants to the corresponding comboboxes.
      /// </summary>
      private void PopulateOtherComboBoxes()
      {
         this.ClearOtherComboBoxes();
         var comps = AAutofillManager.Instance.Source.GetCompanyList();
         if (this.companyComboBox.Items.Count > 0 && this.companyComboBox.SelectedIndex < comps.Count)
         {
            ACompany company = comps[this.companyComboBox.SelectedIndex];
            IList<string> sites = AAutofillManager.Instance.Source.GetCompanySites(company);
            this.siteComboBox.Items.AddRange(sites.ToArray());
            IList<string> assets = AAutofillManager.Instance.Source.GetCompanyAssets(company);
            this.assetComboBox.Items.AddRange(assets.ToArray());
            IList<string> plants = AAutofillManager.Instance.Source.GetCompanyPlants(company);
            this.plantComboBox.Items.AddRange(plants.ToArray());
         }
      }

      /// <summary>
      /// Clear out the sites/assets/plants comboboxes.
      /// </summary>
      private void ClearOtherComboBoxes()
      {
         this.siteComboBox.Items.Clear();
         this.assetComboBox.Items.Clear();
         this.plantComboBox.Items.Clear();
      }

      /// <summary>
      /// Handle state change so we can validate and update the auto-fill db.
      /// </summary>
      /// <param name="newState"></param>
      /// <param name="args"></param>
      private void HandleStateWillChangeEvent(AState newState, string actionName, CancelEventArgs args)
      {
         // if we're moving on, validate and update the autofill db
         if ("save" == actionName)
         {
            if (!this.ValidateData())
            {
               args.Cancel = true;
            }
            else
            {
               this.UpdateAutofill();
               if (fEditMode)
               {
                  this.SaveMasterInfoEdit();
               }
               else
               {
                  this.SaveMasterInfoNew();
               }
            }
         }
      }

      /// <summary>
      /// Update the AMasterDsiInfo object to be used by other screens down the line
      /// based on the data the user has entered.
      /// </summary>
      private void SaveMasterInfoNew()
      {
         AMasterDsiInfo master = new AMasterDsiInfo();
         master.DsiItems = new List<ANanoSense>();
         for (int i = 0; i < this.numDsisTextBox.IntValue; ++i)
         {
            ANanoSense nano = new ANanoSense();
            nano.company = AAutofillManager.Instance.Source.GetCompanyList()[this.companyComboBox.SelectedIndex];
            nano.site = new ASite() { name = this.siteComboBox.Text };
            nano.plant = new APlant() { name = this.plantComboBox.Text };
            nano.asset = new AAsset() { name = this.assetComboBox.Text };
            nano.collectionPoint = new ACollectionPoint()
            {
               name = this.cpNameTextBox.Text,
               description = this.cpDescTextBox.Text,
               Latitude = this.latLongView.Latitude,
               Longitude = this.latLongView.Longitude
            };
            nano.Dsi = new ADsiInfo()
            {
               Latitude = this.latLongView.Latitude,
               Longitude = this.latLongView.Longitude,
               dsiCount = (ushort)this.numDsisTextBox.IntValue
            };
            nano.Dsi.probes = new AProbe[0];
            nano.Dsi.probeCount = 1;
            nano.Dsi.tag = "";
            master.DsiItems.Add(nano);
         }
         master.CurrentDsi = -1;    // start at -1, will be incremented when we add the first DSI
         AStateController.Instance.GlobalData["masterdsi"] = master;
      }

      /// <summary>
      /// Save the info to the current master info for all DSIs (for Edit mode).
      /// </summary>
      private void SaveMasterInfoEdit()
      {
         AMasterDsiInfo master = (AMasterDsiInfo)AStateController.Instance.GlobalData["masterdsi"];
         var comps = AAutofillManager.Instance.Source.GetCompanyList();
         ANanoSense currNano = master.DsiItems[master.CurrentDsi];
         foreach (ANanoSense nano in master.DsiItems)
         {
            if (this.companyComboBox.SelectedIndex < comps.Count)
            {
               nano.company = comps[this.companyComboBox.SelectedIndex];
            }
            else
            {
               nano.company = currNano.company;
            }
            nano.site.name = this.siteComboBox.Text;
            nano.asset.name = this.assetComboBox.Text;
            nano.plant.name = this.plantComboBox.Text;
            nano.collectionPoint.name = this.cpNameTextBox.Text;
            nano.collectionPoint.description = this.cpDescTextBox.Text;
            nano.collectionPoint.Latitude = this.latLongView.Latitude;
            nano.collectionPoint.Longitude = this.latLongView.Longitude;
            nano.Dsi.Latitude = this.latLongView.Latitude;
            nano.Dsi.Longitude = this.latLongView.Longitude;

            bool success = false;
            try
            {
               success = master.Dsi.WriteCompany((byte)nano.Dsi.modbusAddress, nano.company);
               success = (success ? master.Dsi.WriteSite((byte)nano.Dsi.modbusAddress, nano.site) : success);
               success = (success ? master.Dsi.WriteAsset((byte)nano.Dsi.modbusAddress, nano.asset) : success);
               success = (success ? master.Dsi.WritePlant((byte)nano.Dsi.modbusAddress, nano.plant) : success);
               success = (success ? master.Dsi.WriteCollectionPoint((byte)nano.Dsi.modbusAddress, nano.collectionPoint) : success);
               success = (success ? master.Dsi.WriteDsiInfo((byte)nano.Dsi.modbusAddress, nano.Dsi) : success);
            }
            catch (Exception) 
            {
               success = false;
            }
            finally
            {
               if (!success)
               {
                  AOutput.DisplayError(Resources.ErrorWriteGlobalInfo);
               }
            }
         }
      }

      /// <summary>
      /// Update the auto-fill db according to any new info entered for site, asset, or plant.
      /// </summary>
      private void UpdateAutofill()
      {
         var comps = AAutofillManager.Instance.Source.GetCompanyList();
         if (this.companyComboBox.Items.Count > 0 && this.companyComboBox.SelectedIndex < comps.Count)
         {
            ACompany company = comps[this.companyComboBox.SelectedIndex];
            Debug.Assert(null != company);

            if (this.siteComboBox.Text.Length > 0 && !this.siteComboBox.Items.Contains(this.siteComboBox.Text))
            {
               AAutofillManager.Instance.Source.AddSite(this.siteComboBox.Text, company);
            }

            if (this.assetComboBox.Text.Length > 0 && !this.assetComboBox.Items.Contains(this.assetComboBox.Text))
            {
               AAutofillManager.Instance.Source.AddAsset(this.assetComboBox.Text, company);
            }

            if (this.plantComboBox.Text.Length > 0 && !this.plantComboBox.Items.Contains(this.plantComboBox.Text))
            {
               AAutofillManager.Instance.Source.AddPlant(this.plantComboBox.Text, company);
            }
         }
      }

      /// <summary>
      /// Validate our data.
      /// </summary>
      /// <returns></returns>
      private bool ValidateData()
      {
         bool valid = true;

         // company must have a selection
         if (null == this.companyComboBox.SelectedItem)
         {
            AOutput.DisplayError(Resources.ErrorCommCompanyRequired);
            valid = false;
         }

         if (valid && 0 == this.siteComboBox.Text.Length && 0 == this.plantComboBox.Text.Length && 0 == this.assetComboBox.Text.Length)
         {
            AOutput.DisplayError(Resources.ErrorCommSitePlantAssetRequired);
            valid = false;
         }

         if (valid && 0 == this.cpNameTextBox.Text.Length)
         {
            AOutput.DisplayError(Resources.ErrorCommCollectionPointNameRequired);
            valid = false;
         }

         if (valid && (0 == this.latLongView.Latitude.Length || 0 == this.latLongView.Longitude.Length))
         {
            AOutput.DisplayError(Resources.ErrorCommCollectionPointLocationRequired);
            valid = false;
         }

         if (valid && 0 == this.numDsisTextBox.Text.Length)
         {
            AOutput.DisplayError(Resources.ErrorCommNumDsisRequired);
            valid = false;
         }

         if (valid && Convert.ToInt16(this.numDsisTextBox.Text) > 32)
         {
            AOutput.DisplayError(Resources.ErrorCommNumDsis);
            valid = false;
         }

         return valid;
      }

      /// <summary>
      /// Get the user's location via .NET location services.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void cpGetLocationButton_Click(object sender, EventArgs e)
      {
         GeoCoordinateWatcher watcher = new GeoCoordinateWatcher();

         // Initiate acquisition of location data, so that we 
         // can get location coordinates from the Position property.
         // 1. If we don’t have access to the location sensors, 
         // the user will be prompted with a permission dialog.
         // 2. If we have permissions, following this call, we can 
         //    access location data from the Position property, 
         //    and we'll get PositionChanged events if we have set
         //    up an event handler.

         GeoCoordinate coord = null;
         AStateController.Instance.PostPerformActionEvent("showspinnerprogress", Resources.GetLocationProgress);
         for (int i = 0; i < 3; ++i)
         {
            watcher.TryStart(false, TimeSpan.FromMilliseconds(5000));   // 5 second timeout
            coord = watcher.Position.Location;
            if (coord.IsUnknown != true)
            {
               break;
            }
         }
         AStateController.Instance.PostPerformActionEvent("hidespinnerprogress", null);

         if (coord.IsUnknown != true)
         {
            this.latLongView.Latitude = Convert.ToString(coord.Latitude);
            this.latLongView.Longitude = Convert.ToString(coord.Longitude);
         }
         else
         {
            AOutput.DisplayError(Resources.ErrorNoLocation);
         }
      }

      /// <summary>
      /// Create a new company object.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void newCompanyButton_Click(object sender, EventArgs e)
      {
         AStateController.Instance.ChangeToState("comm-newcompany");
      }

      /// <summary>
      /// Company selection changed - update the auto-fill info for the other combos.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void companyComboBox_SelectedIndexChanged(object sender, EventArgs e)
      {
         this.PopulateOtherComboBoxes();
      }
   }
}

