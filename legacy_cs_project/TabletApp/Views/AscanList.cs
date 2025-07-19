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
using System.Threading;
using Model;
using System.Diagnostics;
using TabletApp.Utils;
using TabletApp.Properties;

namespace TabletApp.Views
{
   /// <summary>
   /// User control that represents a list of Ascan items, showing probe info
   /// and the Ascan waveform.
   /// </summary>
   public partial class AscanList : UserControl
   {
      private const int kRowHeight = 108;
      private bool fHandlingGraphChange = false;

      public bool HandleGraphChange { get; set; }
      public List<AscanWaveform> Ascans
      {
         get
         {
            return this.ascanPanel.Controls.OfType<AscanListItem>().Select(a => a.Graph).ToList();
         }
      }

      /// <summary>
      /// Constructor
      /// </summary>
      public AscanList()
      {
         InitializeComponent();
         this.HandleGraphChange = false;
      }

      /// <summary>
      /// Add a new list item for the given probe number.
      /// </summary>
      /// <param name="probeNumber">1-based probe number</param>
      public void AddProbe(AProbe probe)
      {
         if (this.InvokeRequired)
         {
            this.Invoke(new Action(() => this.AddProbe(probe)));
            return;
         }

         bool needsScale = null != this.FindForm();
         AscanListItem item = new AscanListItem();
         if (this.HandleGraphChange)
         {
            item.Graph.GraphChangedEvent += Graph_GraphChangedEvent;
         }
         else
         {
            item.SetAsDefaultButton.Visible = false;
         }
         item.PopulateWithProbe(probe);
         item.Anchor = AnchorStyles.Left | AnchorStyles.Right;
         if (needsScale)
         {
            AScaleUtils.ScaleControl(item);
         }
         this.ascanPanel.Controls.Add(item);
         RowStyle rowStyle = this.ascanPanel.RowStyles[this.ascanPanel.Controls.Count-1];
         rowStyle.Height = kRowHeight * (needsScale ? AScaleUtils.ScaleFactor : 1f);
         int newHeight = (kRowHeight + 1) * this.ascanPanel.Controls.Count;
         this.ascanPanel.Height = (int)(newHeight * (needsScale ? AScaleUtils.ScaleFactor : 1f));
         this.AutoScrollMinSize = new Size(0, (int)((float)newHeight * AScaleUtils.ScaleFactor));
         this.ascanPanel.PerformLayout();
      }

      /// <summary>
      /// Handle graph changes.  If ascans are locked, keep them all in sync when one changes.
      /// </summary>
      /// <param name="graph"></param>
      private void Graph_GraphChangedEvent(AscanWaveform graph)
      {
         if (fHandlingGraphChange)
         {
            return;
         }
         fHandlingGraphChange = true;
         if (Settings.Default.LockAscans)
         {
            foreach (var control in this.ascanPanel.Controls)
            {
               if (control is AscanListItem && graph != control)
               {
                  ((AscanListItem)control).Graph.GraphZoom = graph.GraphZoom;
                  ((AscanListItem)control).Graph.GraphScroll = graph.GraphScroll;
               }
            }
         }
         fHandlingGraphChange = false;
      }

      /// <summary>
      /// Add the given probes to our list.
      /// </summary>
      /// <param name="probes"></param>
      public void PopulateWithProbes(IList<AProbe> probes)
      {
         this.Clear();
         foreach (AProbe probe in probes)
         {
            this.AddProbe(probe);
         }
      }

      /// <summary>
      /// Clear out all controls
      /// </summary>
      public void Clear()
      {
         if (this.InvokeRequired)
         {
            this.Invoke(new Action(() => this.Clear()));
            return;
         }

         for (int i = this.ascanPanel.Controls.Count - 1; i >= 0; --i)
         {
            this.ascanPanel.Controls.RemoveAt(i);
            RowStyle rowStyle = this.ascanPanel.RowStyles[i];
            rowStyle.Height = 0;
         }
         this.ascanPanel.Height = 10;
         this.ascanPanel.PerformLayout();
      }
   }
}

