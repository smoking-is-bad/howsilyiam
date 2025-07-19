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
using System.Drawing;
using TabletApp.State;
using TabletApp.Api;
using Model;
using MockData;

namespace TabletApp.Content.Commissioning
{
   /// <summary>
   /// User control for the commissioning main screen.
   /// </summary>
   public partial class Main : BaseContent
   {
      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="parameters">Parameters from the state def file</param>
      public Main(Dictionary<string, string> parameters)
         : base(parameters)
      {
         InitializeComponent();
         // don't want our buttons to draw a border when in the background
         Color clearColor = Color.FromArgb(0, 255, 255, 255);
         this.addEditButton.FlatAppearance.BorderColor = clearColor;
         this.newButton.FlatAppearance.BorderColor = clearColor;


         // !!! Temp info for now
         AMasterDsiInfo tempInfo = new AMasterDsiInfo();
         tempInfo.DsiItems = new List<ANanoSense>();
         ANanoSense tempNano = new AMockData().NanoSense(12);
         tempNano.site = new ASite() { name = "site" };
         tempNano.plant = new APlant() { name = "plant" };
         tempNano.asset = new AAsset() { name = "asset" };
         tempInfo.DsiItems.Add(tempNano);
         AStateController.Instance.GlobalData["masterdsi"] = tempInfo;
      }

      private void newButton_Click(object sender, EventArgs e)
      {
         AStateController.Instance.ChangeToState("comm-new");
      }

      private void addEditButton_Click(object sender, EventArgs e)
      {
         AStateController.Instance.ChangeToState("comm-message-plugin");
      }

      private void resetButton_Click(object sender, EventArgs e)
      {
         AStateController.Instance.ChangeToState("comm-message-reset");
      }
   }
}

