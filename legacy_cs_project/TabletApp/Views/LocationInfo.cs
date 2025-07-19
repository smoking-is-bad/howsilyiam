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

using System.Windows.Forms;
using Model;

namespace TabletApp.Views
{
   /// <summary>
   /// User control that contains the station location information.
   /// </summary>
   public partial class LocationInfo : UserControl
   {
      /// <summary>
      /// Constructor
      /// </summary>
      public LocationInfo()
      {
         InitializeComponent();
      }

      /// <summary>
      /// Populate our UI according to the new dsi.
      /// </summary>
      /// <param name="dsi"></param>
      public void PopulateWithDsi(ANanoSense dsi)
      {
         this.companyName.Text = dsi.company.name;
         this.lineName.Text = dsi.site.name;
         var plantName = (string)dsi.plant.name;
         var assetName = (string)dsi.asset.name;
         if (plantName.Length > 0 && assetName.Length > 0)
         {
            this.plantAsset.Text = plantName + ", " + assetName;
         }
         else if (plantName.Length > 0)
         {
            this.plantAsset.Text = plantName;
         }
         else if (assetName.Length > 0)
         {
            this.plantAsset.Text = assetName;
         }
         this.location.Text = dsi.collectionPoint.location.coordinates;
      }
   }
}

