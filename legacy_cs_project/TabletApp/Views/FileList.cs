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
using TabletApp.Properties;
using TabletApp.Views;
using TabletApp.Utils;

namespace TabletApp.Views
{
   /// <summary>
   /// Delegate for file delete from list events
   /// </summary>
   /// <param name="fileIndex"></param>
   public delegate void FileRemoved(int fileIndex);

   /// <summary>
   /// User control that represents a list of files.  Entries can be removed via a button click.
   /// Optionally can apply a status (success/failure) to each entry.
   /// </summary>
   public partial class FileList : UserControl
   {
      public event FileRemoved FileRemovedEvent;

      public List<string> Files { get; private set; }

      /// <summary>
      /// Function for allowing AFileList owners to apply a mapping to the displayed strings.
      /// </summary>
      public Func<string, string> Mapper { get; set; }

      /// <summary>
      /// Constructor
      /// </summary>
      public FileList()
      {
         InitializeComponent();

         this.Files = new List<string>();

         // TableLayoutPanel is a somewhat frustrating API to work with.  At least one row needs to be defined in the
         // designer, and that row cannot (easily) be deleted, so we will always maintain that last hidden row,
         // and then insert rows in front of it for our actual data.  Deleting rows in a TableLayoutPanel is a cumbersome
         // process to say the least, so we will just set the height to 0 when the user hits the delete button.
         fileTable.RowStyles[0].Height = 0;
      }

      /// <summary>
      /// Reset the file table.  We always maintain a last row as a placehold and just show/hide
      /// the other rows for the files that are selected.
      /// </summary>
      public void ResetFileTable()
      {
         this.Files.Clear();
         for (int i = fileTable.Controls.Count - 1; i >= 0; --i)
         {
            fileTable.Controls.RemoveAt(i);
         }
         for (int i = fileTable.RowStyles.Count - 2; i >= 0; --i)
         {
            fileTable.RowStyles.RemoveAt(i);
         }
         fileTable.RowCount = 1;
         fileTable.ColumnCount = 3;
      }

      /// <summary>
      /// Populate the file table with the given filenames
      /// </summary>
      /// <param name="fileNames">Array of filenames</param>
      public void PopulateFileTable(string[] fileNames)
      {
         int row = 0;
         int newHeight = 0;

         foreach (string file in fileNames)
         {
            ++this.fileTable.RowCount;
            RowStyle rowStyle;
            rowStyle = new RowStyle(SizeType.Absolute, 30);
            fileTable.RowStyles.Insert(row, rowStyle);
            DeleteButton button = new DeleteButton();
            button.Click += new EventHandler(this.DeleteFileClick);
            button.Tag = rowStyle;
            fileTable.Controls.Add(button, 1, row);
            string fileToShow = file;
            if (null != this.Mapper)
            {
               fileToShow = this.Mapper(file);
            }
            Label label = new Label()
            {
               Text = fileToShow,
               Font = new Font(FontFamily.GenericSansSerif, 12),
               AutoEllipsis = true,
               Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right
            };
            fileTable.Controls.Add(label, 2, row);
            this.Files.Add(file);
            ++row;
            newHeight += (int)rowStyle.Height;
         }
         this.fileTable.Height = newHeight;
         this.AutoScrollMinSize = new Size(0, (int)((float)newHeight * AScaleUtils.ScaleFactor));

         fileTable.PerformLayout();
      }

      /// <summary>
      /// Add the given filenames to the current list.
      /// </summary>
      /// <param name="fileNames">New array of names to add</param>
      public void Add(string[] fileNames)
      {
         // just reset the current file table, adding the new list to the old list
         List<string> tempFiles = new List<string>(this.Files);
         tempFiles.AddRange(fileNames);
         this.ResetFileTable();
         this.PopulateFileTable(tempFiles.ToArray());
      }

      /// <summary>
      /// Mark the entry at the given index as success or failure, with an optional tooltip.
      /// </summary>
      /// <param name="index">Index of the list of visible files (rows with non-zero height)</param>
      /// <param name="success">Is it a success or failure file</param>
      /// <param name="toolTip">Optional tooltip text (eg error text)</param>
      public void MarkFileAtIndex(int index, bool success, string toolTip = null)
      {
         // mark the file (in column 1) with the appropriate graphic for success/failure
         PictureBox picture = new PictureBox();
         picture.Image = (success ? Resources.upload_success : Resources.upload_fail);
         picture.BorderStyle = System.Windows.Forms.BorderStyle.None;
         picture.BackColor = Color.Transparent;
         picture.SizeMode = PictureBoxSizeMode.Zoom;
         if (null != toolTip)
         {
            ToolTip tip = new ToolTip();
            tip.SetToolTip(picture, toolTip);
         }
         int realIndex = this.RealRowIndex(index, false);
         Control currPicture = this.fileTable.GetControlFromPosition(0, realIndex);
         if (null != currPicture)
         {
            this.fileTable.Controls.Remove(currPicture);
         }
         this.fileTable.Controls.Add(picture, 0, realIndex);
      }

      /// <summary>
      /// Get the real row index, adding or subtracting for zero-height rows to the given
      /// index.
      /// </summary>
      /// <param name="rowIndex">A row index according to the total rows in the table</param>
      /// <param name="subtractHiddenRows">If true, subtract the hidden rows, otherwise add them</param>
      /// <returns>The real index, adding or subtracting any zero-height rows that comes before the given index</returns>
      private int RealRowIndex(int rowIndex, bool subtractHiddenRows)
      {
         int realIndex = rowIndex;

         foreach (RowStyle rowStyle in this.fileTable.RowStyles)
         {
            if (rowIndex == fileTable.RowStyles.IndexOf(rowStyle))
            {
               break;
            }
            if (0 == rowStyle.Height)
            {
               realIndex += (subtractHiddenRows ? -1 : 1);
            }
         }

         return realIndex;
      }

      /// <summary>
      /// Handle click of the delete button.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void DeleteFileClick(object sender, EventArgs e)
      {
         RowStyle rowStyle = (RowStyle)((Control)sender).Tag;
         int row = this.RealRowIndex(fileTable.RowStyles.IndexOf(rowStyle), true);
         rowStyle.Height = 0;
         fileTable.PerformLayout();
         fileTable.Controls.Remove((Control)sender);
         this.Files.RemoveAt(row);
         if (null != this.FileRemovedEvent)
         {
            this.FileRemovedEvent(row);
         }
      }
   }
}

