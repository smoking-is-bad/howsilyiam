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
using System.Drawing.Drawing2D;
using TabletApp.Utils;
using TabletApp.Content;
using Model;
using System.Diagnostics;
using TabletApp.Api;

namespace TabletApp.Views
{
   /// <summary>
   /// User control that represents a grid of thickness value for all 32x16 probes.
   /// Grid is displayed with probes across the top (16 columns) and
   /// DSIs down the left (rows).
   /// </summary>
   public partial class SummaryGrid : UserControl
   {
      private const int kRowHeight = 25;
      private AlertState[,] fCellStates = new AlertState[1000, 1000];

      public SummaryGrid()
      {
         InitializeComponent();

         this.gridPanel.SuspendLayout();

         // add labels to the header cells
         for (int i = 0; i < this.gridPanel.RowCount; ++i)
         {
            for (int j = 0; j < this.gridPanel.ColumnCount; ++j)
            {
               this.FillHeader(i, j);
            }
         }

         this.gridPanel.ResumeLayout();
      }

      /// <summary>
      /// If it's a logger state, we show a different label for the row numbers.
      /// </summary>
      /// <param name="isLogger"></param>
      public void SetLoggerState(bool isLogger)
      {
         loggerLabel.Visible = isLogger;
         dsiLabel.Visible = !isLogger;
      }

      private void FillHeader(int row, int column)
      {
         bool columnHeader = (0 == row && column > 0);
         bool rowHeader = (0 == column && row > 0);
         if (columnHeader || rowHeader)
         {
            Label label = new Label()
            {
               Anchor = AnchorStyles.None,
               Font = new Font("Arial", 12, FontStyle.Bold),
               BackColor = Color.Transparent
            };
            this.gridPanel.Controls.Add(label, column, row);
            // fill out the header cells
            if (columnHeader)
            {
               label.Text = Convert.ToString(column);
            }
            else if (rowHeader)
            {
               label.Text = Convert.ToString(row);
            }
         }
      }

      /// <summary>
      /// Apply the given state to the cell at the row/column.
      /// </summary>
      /// <param name="dsiNum">0-based index of the DSI (row)</param>
      /// <param name="probeNum">0-based index of the probe (column)</param>
      /// <param name="state"></param>
      public void SetStateForCell(int dsiNum, int probeNum, AlertState state)
      {
         if (dsiNum >= 0 && dsiNum < this.gridPanel.RowCount && probeNum >= 0 && probeNum < this.gridPanel.ColumnCount)
         {
            fCellStates[dsiNum, probeNum] = state;
         }
         else
         {
            AOutput.LogMessage("Invalid DSI or probe number for summary grid state");
         }
      }

      /// <summary>
      /// Populate our UI according to the new list of ANanoSense objects.
      /// </summary>
      /// <param name="items"></param>
      public void PopulateWithDsiList(List<ANanoSense> items)
      {
         bool needsScale = null != this.FindForm();

         this.gridPanel.SuspendLayout();

         if (items.Count > this.gridPanel.RowCount)
         {
            int oldCount = this.gridPanel.RowCount - 1;
            this.gridPanel.RowCount = items.Count + 1;
            for (int i = oldCount; i < this.gridPanel.RowCount; ++i)
            {
               this.FillHeader(i, 0);
            }
            int newHeight = items.Count * kRowHeight;
            this.gridPanel.Height = (int)(newHeight * (needsScale ? AScaleUtils.ScaleFactor : 1f));
            this.AutoScrollMinSize = new Size(0, (int)((float)newHeight * AScaleUtils.ScaleFactor));
         }
         for (int i = 0; i < items.Count; ++i)
         {
            IList<AProbe> probes = items[i].Dsi.probes;
            if (probes.Count > this.gridPanel.ColumnCount)
            {
               int oldCount = this.gridPanel.ColumnCount;
               this.gridPanel.ColumnCount = probes.Count;
               for (int j = oldCount - 1; j < probes.Count; ++j)
               {
                  this.FillHeader(0, j);
               }
            }
            foreach (AProbe probe in probes)
            {
               int probeNum = probe.num;
               int probeIndex = probeNum - 1;
               Debug.Assert(i < this.gridPanel.RowCount && probeIndex < this.gridPanel.ColumnCount);
               Label label = this.gridPanel.GetControlFromPosition(probeNum, i + 1) as Label;
               if (null == label)
               {
                  label = new Label()
                  {
                     Anchor = AnchorStyles.None,
                     Font = new Font("Arial", 12, FontStyle.Regular),
                     BackColor = Color.Transparent
                  };
                  this.gridPanel.Controls.Add(label, probeNum, i + 1);
               }
               Debug.Assert(probe.setups.Count() > 0);
               label.Text = probe.Thickness().FormatAsMeasurmentString(addUnits: false);
               fCellStates[i + 1, probeNum] = probe.CurrentAlertState();
            }
         }

         this.gridPanel.ResumeLayout();
      }

      /// <summary>
      /// Custom drawing of the cell so we can draw our different states
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void gridPanel_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
      {
         Graphics graphics = e.Graphics;
         Rectangle bounds = e.CellBounds;
         Brush brush = Brushes.White;

         if (AlertState.Warning == fCellStates[e.Row, e.Column])
         {
            brush = Brushes.Yellow;
         }
         else if (AlertState.Error == fCellStates[e.Row, e.Column])
         {
            brush = Brushes.Red;
         }
         else if (1 == e.Row % 2)
         {
            brush = Brushes.WhiteSmoke;
         }
         graphics.FillRectangle(brush, bounds);
      }
   }
}

