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
using TabletApp.State;
using Model;
using TabletApp.Autofill;
using TabletApp.Utils;
using TabletApp.Properties;

namespace TabletApp.Content.Commissioning
{
   /// <summary>
   /// User control for entering in info for a new company.
   /// </summary>
   public partial class NewCompany : BaseContent
   {
      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="parameters">Parameters from the state def file</param>
      public NewCompany(Dictionary<string, string> parameters)
         : base(parameters)
      {
         InitializeComponent();
         AStateController.Instance.StateWillChangeEvent += HandleStateWillChangeEvent;
      }

      /// <summary>
      /// About to disappear - clean up.
      /// </summary>
      public override void WillDisappear()
      {
         AStateController.Instance.StateWillChangeEvent -= HandleStateWillChangeEvent;
      }

      /// <summary>
      /// Handle state change.  Validate the data and create the new company in auto-fill.
      /// </summary>
      /// <param name="newState"></param>
      /// <param name="args"></param>
      private void HandleStateWillChangeEvent(AState newState, string actionName, CancelEventArgs args)
      {
         if ("createcompany" == actionName)
         {
            if (!this.ValidateData())
            {
               args.Cancel = true;
            }
            else
            {
               this.CreateCompany();
            }
         }
      }

      /// <summary>
      /// Create the new auto-fill company.
      /// </summary>
      private void CreateCompany()
      {
         ACompany newCompany = new ACompany()
         {
            id = this.idTextBox.Text,
            name = this.nameTextBox.Text,
            phone = this.phoneTextBox.Text
         };

         newCompany.location = new APostalAddress()
         {
            address1 = this.addressTextBox.Text,
            address2 = this.address2TextBox.Text,
            city = this.cityTextBox.Text,
            state = this.stateTextBox.Text,
            postalCode = this.zipTextBox.Text,
            country = this.countryTextBox.Text
         };

         AAutofillManager.Instance.Source.AddCompany(newCompany);
      }

      /// <summary>
      /// Validate our data.
      /// </summary>
      /// <returns></returns>
      private bool ValidateData()
      {
         bool valid = true;

         if (valid && 0 == this.idTextBox.Text.Length)
         {
            AOutput.DisplayError(Resources.ErrorCommCompanyIdRequired);
            valid = false;
         }

         if (valid && 0 == this.nameTextBox.Text.Length)
         {
            AOutput.DisplayError(Resources.ErrorCommCompanyNameRequired);
            valid = false;
         }

         if (valid && 0 == this.addressTextBox.Text.Length)
         {
            AOutput.DisplayError(Resources.ErrorCommAddrRequired);
            valid = false;
         }

         if (valid && 0 == this.cityTextBox.Text.Length)
         {
            AOutput.DisplayError(Resources.ErrorCommCityRequired);
            valid = false;
         }

         if (valid && 0 == this.stateTextBox.Text.Length)
         {
            AOutput.DisplayError(Resources.ErrorCommStateRequired);
            valid = false;
         }

         if (valid && 0 == this.zipTextBox.Text.Length)
         {
            AOutput.DisplayError(Resources.ErrorCommZipRequired);
            valid = false;
         }

         return valid;
      }
   }
}

