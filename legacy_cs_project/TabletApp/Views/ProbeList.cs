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
using Model;
using TabletApp.Utils;

namespace TabletApp.Views
{
   /// <summary>
   /// Delegate for receiving Probe button clicks to know when one has been selected.
   /// </summary>
   /// <param name="buttonIndex"></param>
   public delegate void ProbeSelectionStateChanged(int probeIndex, bool selected);

   /// <summary>
   /// User control that represents a list of up to 16 sensors.
   /// Uses a TableLayoutPanel to dynamically create a list of 
   /// <code>AProbeListItem</code>s.
   /// </summary>
   public partial class ProbeList : UserControl
   {
      private const int kRowHeight = 44;

      public event ProbeSelectionStateChanged ProbeSelectionStateChangedEvent;
      private IList<AProbe> fProbes;

      private bool fSelectable = false;
      public bool Selectable
      {
         get
         {
            return fSelectable;
         }
         set
         {
            fSelectable = value;
            foreach (ProbeListItem item in this.probePanel.Controls)
            {
               item.Selectable = value;
            }
         }
      }

      /// <summary>
      /// Constructor
      /// </summary>
      public ProbeList()
      {
         InitializeComponent();
      }

      /// <summary>
      /// Remove all probes from the list and update.
      /// </summary>
      public void Clear()
      {
         if (null != fProbes)
         {
            fProbes.Clear();
            this.UpdateList();
         }
      }

      /// <summary>
      /// Populate our UI according to the given AProbe list.
      /// </summary>
      /// <param name="probes"></param>
      public void PopulateWithProbes(IList<AProbe> probes)
      {
         fProbes = new List<AProbe>(probes);
         this.UpdateList();
      }

      /// <summary>
      /// Deselect all probes in this list.
      /// </summary>
      public void ClearSelection()
      {
         this.SetSelected(new List<int>());
      }

      /// <summary>
      /// Set the selected probes per the probe index list provided.
      /// </summary>
      /// <param name="indexes"></param>
      public void SetSelected(List<int> indexes)
      {
         for (int i = 0; i < this.probePanel.RowCount; ++i)
         {
            ProbeListItem item = this.probePanel.GetControlFromPosition(0, i) as ProbeListItem;
            if (null != item)
            {
               item.Selected = (null != indexes ? indexes.Contains(i) : false);
            }
         }
      }

      /// <summary>
      /// Scroll ourselves back to the top probe.
      /// </summary>
      private void ScrollToTop()
      {
         this.VerticalScroll.Value = 0;
         ProbeListItem item = this.probePanel.GetControlFromPosition(0, 0) as ProbeListItem;
         if (null != item)
         {
            this.ScrollControlIntoView(item);
         }
      }

      /// <summary>
      /// Update our list by adding/removing <code>AProbeListItem</code> controls as needed
      /// to match the current count.
      /// </summary>
      private void UpdateList()
      {
         if (fProbes.Count > this.probePanel.RowCount)
         {
            throw new Exception("Invalid probe count");
         }

         bool needsScale = (null != this.FindForm() && AScaleUtils.NeedsScale);

         if (needsScale)
         {
            this.ScaleControl(1f / AScaleUtils.ScaleFactor);
            this.ScaleFontDeep(1f / AScaleUtils.ScaleFactor);
         }

         this.ScrollToTop();

         // add any new ones that are needed
         for (int i = 0; i < fProbes.Count; ++i)
         {
            ProbeListItem item = this.probePanel.GetControlFromPosition(0, i) as ProbeListItem;
            if (null == item)
            {
               RowStyle rowStyle = this.probePanel.RowStyles[i];
               rowStyle.Height = kRowHeight;
               item = new ProbeListItem();
               item.Margin = new Padding(0);
               this.probePanel.Controls.Add(item);
               item.ProbeButton.Click += ProbeButtonClicked;
               item.Selectable = this.Selectable;
            }
            item.PopulateWithProbe(fProbes[i]);
         }

         // remove any that are not needed
         for (int i = this.probePanel.RowCount - 1; i >= fProbes.Count; --i)
         {
            Control control = this.probePanel.GetControlFromPosition(0, i);
            if (null != control)
            {
               this.probePanel.Controls.Remove(control);
            }
            RowStyle rowStyle = this.probePanel.RowStyles[i];
            rowStyle.Height = 0;
         }

         // we have a vertical separator line - adjust its height to the number of sensors displayed
         this.vertSeparator.Height = (fProbes.Count * kRowHeight) - (kRowHeight / 2) + this.probePanel.Top + this.probePanel.Padding.Top;

         int newHeight = (kRowHeight + 1) * fProbes.Count + this.probePanel.Padding.Top;
         this.probePanel.Height = newHeight;
         this.AutoScrollMinSize = new Size(0, (int)((float)newHeight * 0.71f));

         this.probePanel.PerformLayout();

         if (needsScale)
         {
            this.ScaleControl(AScaleUtils.ScaleFactor);
            this.ScaleFontDeep(AScaleUtils.ScaleFactor);
         }
      }

      /// <summary>
      /// Handle a probe button click by posting our own "selection change" event
      /// to anyone who's listening.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void ProbeButtonClicked(object sender, EventArgs e)
      {
         if (null != this.ProbeSelectionStateChangedEvent && this.Selectable)
         {
            Button button = sender as Button;
            ProbeListItem item = button.Parent as ProbeListItem;
            int index = this.probePanel.GetPositionFromControl(item).Row;
            this.ProbeSelectionStateChangedEvent(index, item.Selected);
         }
      }
   }
}

