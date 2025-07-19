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
namespace TabletApp.Views
{
   partial class LocationInfo
   {
      /// <summary> 
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary> 
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing && (components != null))
         {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LocationInfo));
         this.companyName = new System.Windows.Forms.Label();
         this.lineName = new System.Windows.Forms.Label();
         this.location = new System.Windows.Forms.Label();
         this.plantAsset = new System.Windows.Forms.Label();
         this.SuspendLayout();
         // 
         // companyName
         // 
         resources.ApplyResources(this.companyName, "companyName");
         this.companyName.Name = "companyName";
         // 
         // lineName
         // 
         resources.ApplyResources(this.lineName, "lineName");
         this.lineName.Name = "lineName";
         // 
         // location
         // 
         resources.ApplyResources(this.location, "location");
         this.location.Name = "location";
         // 
         // plantAsset
         // 
         resources.ApplyResources(this.plantAsset, "plantAsset");
         this.plantAsset.Name = "plantAsset";
         // 
         // ALocationInfo
         // 
         resources.ApplyResources(this, "$this");
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.BackColor = System.Drawing.Color.Transparent;
         this.Controls.Add(this.plantAsset);
         this.Controls.Add(this.location);
         this.Controls.Add(this.lineName);
         this.Controls.Add(this.companyName);
         this.Name = "ALocationInfo";
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.Label companyName;
      private System.Windows.Forms.Label lineName;
      private System.Windows.Forms.Label location;
      private System.Windows.Forms.Label plantAsset;
   }
}

