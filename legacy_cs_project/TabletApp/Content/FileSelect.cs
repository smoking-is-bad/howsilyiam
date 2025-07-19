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
using TabletApp.Persist;
using TabletApp.Api;
using Model;
using TabletApp.Utils;
using System.IO;

namespace TabletApp.Content
{
   /// <summary>
   /// User control for the file select screen.  User for both
   /// upload and review selection, configured based on parameters.
   /// </summary>
   public partial class FileSelect : BaseContent
   {
      private List<string> fDirContents = new List<string>();

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="parameters">Parameters from the state def file</param>
      public FileSelect(Dictionary<string, string> parameters)
         : base(parameters)
      {
         InitializeComponent();

         Color clearColor = Color.FromArgb(0, 255, 255, 255);
         this.addButton.FlatAppearance.BorderColor = clearColor;
         this.addAllButton.FlatAppearance.BorderColor = clearColor;

         AStateController.Instance.PostPerformActionEvent("disablebutton", "proceed");

         this.fileList.FileRemovedEvent += HandleFileRemoved;
         AStateController.Instance.PerformActionEvent += HandlePerformAction;

         this.addButton.Enabled = false;
         this.addAllButton.Enabled = false;

         this.fileList.Mapper = (path) =>
            {
               return AFileManager.Instance.DirStructure.FriendlyPathForPath(path, ":");
            };

         // refresh our list before populating the tree
         AFileManager.Instance.Refresh();
         this.PopulateTree();
      }

      /// <summary>
      /// Populate the TreeView with the hierarchy from our data directory, using the mapped names (not the actual
      /// file/folder names).
      /// </summary>
      private void PopulateTree()
      {
         // suppress repainting the TreeView until all the objects have been created
         this.treeView.BeginUpdate();

         // clear the TreeView each time the method is called
         this.treeView.Nodes.Clear();

         ADirNode root = AFileManager.Instance.DirStructure.RootNode;

         Action<TreeNodeCollection, List<ADirNode>> AddNode = null;
         AddNode = (treeNodes, dirNodes) =>
         {
            foreach (ADirNode child in dirNodes)
            {
               TreeNode treeNode = treeNodes.Add(child.Name);
               treeNode.Tag = child;
               AddNode(treeNode.Nodes, child.Children);
            }
         };

         // recursively add all the nods and children from our tree
         AddNode(this.treeView.Nodes, root.Children);

         // update the TreeView
         this.treeView.EndUpdate();
      }

      /// <summary>
      /// Handle removal of a file - disable the "next" button if there are no files.
      /// </summary>
      /// <param name="fileIndex"></param>
      private void HandleFileRemoved(int fileIndex)
      {
         if (0 == this.fileList.Files.Count)
         {
            AStateController.Instance.PostPerformActionEvent("disablebutton", "proceed");
         }
      }

      /// <summary>
      /// About to disappear - clean up.
      /// </summary>
      public override void WillDisappear()
      {
         // unlisten for our action events
         AStateController.Instance.PerformActionEvent -= HandlePerformAction;
      }

      /// <summary>
      /// Are we in review mode?
      /// </summary>
      /// <returns></returns>
      private bool IsReview()
      {
         return fParams.ContainsKey("mode") && fParams["mode"] == "review";
      }

      /// <summary>
      /// Are we in upload mode?
      /// </summary>
      /// <returns></returns>
      private bool IsUpload()
      {
         return fParams.ContainsKey("mode") && fParams["mode"] == "upload";
      }

      /// <summary>
      /// Are we in email mode?
      /// </summary>
      /// <returns></returns>
      private bool IsEmail()
      {
         return fParams.ContainsKey("mode") && fParams["mode"] == "email";
      }

      /// <summary>
      /// Handle file selection
      /// </summary>
      /// <param name="actionName"></param>
      /// <param name="data"></param>
      private void HandlePerformAction(string actionName, object data)
      {
         if (this.InvokeRequired)
         {
            this.Invoke(new Action(() => this.HandlePerformAction(actionName, data)));
            return;
         }

         // "next" action means user is moving on - populate the masterdsi info according to the selected files
         if ("next" == actionName)
         {
            List<string> filepaths = this.fileList.Files;
            AMasterDsiInfo info = new AMasterDsiInfo();
            List<ANanoSense> nanos = null;

            nanos = AFileManager.Instance.Load(filepaths);
            nanos.ForEach(d => { if (d.Dsi.Model == "logger") d.Dsi.Model = "shot"; });
            if (this.IsReview())
            {
               // if we're in review mode, we need to fool the app into thinking these are datalogger DSIs
               // so that the datalogger UI is shown when reviewing
               ANanoSense nano = nanos.First().ShallowCopy();
               nano.Dsi.CopyFrom(nanos.First().Dsi);
               nano.Dsi.Model = "logger";
               nano.Dsi.numShots = nanos.Count();
               nano.Dsi.probes = nanos.First().Dsi.probes;
               nanos.Insert(0, nano);
            }
            info.DsiItems = nanos;
            info.CurrentDsi = 1;
            AStateController.Instance.GlobalData["masterdsi"] = info;

            // for email, store the csv files separately
            if (this.IsEmail())
            {
               var csvs = filepaths.Where(p => p.EndsWith(".csv")).ToList();
               AStateController.Instance.GlobalData["emailcsvs"] = csvs;
            }
         }
      }

      /// <summary>
      /// Add the file(s) from the contents list to the main list.  If selectedOnly is true,
      /// only add the selected files, otherwise add them all.
      /// </summary>
      /// <param name="selectedOnly"></param>
      private void AddFiles(bool selectedOnly)
      {
         List<string> filesToAdd = new List<string>();
         int index = 0;

         foreach (string filePath in fDirContents)
         {
            if (!this.fileList.Files.Contains(filePath) && (!selectedOnly || this.contentsList.SelectedIndices.Contains(index)))
            {
               filesToAdd.Add(filePath);
            }
            ++index;
         }

         this.fileList.Add(filesToAdd.ToArray());
         AStateController.Instance.PostPerformActionEvent("enablebutton", "proceed");
      }

      /// <summary>
      /// Update the list of available files - handle multi-selected folders.
      /// </summary>
      private void UpdateContentsList()
      {
         this.multiSelectLabel.Visible = this.treeView.SelectedNodes.Count > 1;
         this.contentsList.Items.Clear();
         fDirContents.Clear();
         foreach (TreeNode node in this.treeView.SelectedNodes)
         {
            ADirNode dirNode = (ADirNode)node.Tag;
            string partialPath = dirNode.RelativePathFromRoot();
            string basePath = AFileManager.Instance.DirStructure.RootDirectory;
            string fullPath = Path.Combine(basePath, partialPath);
            // only care about .sni files and .csv for email mode
            fDirContents.AddRange(new List<string>(Directory.GetFiles(fullPath).Where(path => path.EndsWith(".sni") || (this.IsEmail() ? path.EndsWith(".csv") : false))));
            string[] filePaths;
            filePaths = fDirContents.Select(path => Path.GetFileName(path)).ToArray();
            this.contentsList.Items.AddRange(filePaths);
         }
         this.addButton.Enabled = false;
         this.addAllButton.Enabled = this.contentsList.Items.Count > 0;
      }

      /// <summary>
      /// Selection changed - update the contents list.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void treeView_SelectionsChanged(object sender, EventArgs e)
      {
         this.UpdateContentsList();
      }

      /// <summary>
      /// Only allow mult-select for childless nodes with the same parent.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void treeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
      {
         if (this.treeView.SelectedNodes.Count > 0)
         {
            TreeNode node = e.Node;
            ADirNode dirNode = (ADirNode)node.Tag;
            if (0 != dirNode.Children.Count)
            {
               e.Cancel = true;
            }
         }
      }

      /// <summary>
      /// Add button was clicked - add the selected file(s) to the main list.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void addButton_Click(object sender, EventArgs e)
      {
         this.AddFiles(selectedOnly: true);
      }

      /// <summary>
      /// Selection changed in the contents list - update the Add button state.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void contentsList_SelectedIndexChanged(object sender, EventArgs e)
      {
         this.addButton.Enabled = this.contentsList.SelectedIndices.Count > 0;
      }

      /// <summary>
      /// Change the Add button image based on its enabled state.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void addButton_EnabledChanged(object sender, EventArgs e)
      {
         Button button = sender as Button;
         button.BackgroundImage = (button.Enabled ? Resources.blue_button_small : Resources.gray_button);
      }

      /// <summary>
      /// Double-click on the contents list - add whatever files are selected to the main list.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void contentsList_MouseDoubleClick(object sender, MouseEventArgs e)
      {
         this.AddFiles(selectedOnly: true);
      }

      /// <summary>
      /// Add All button clicked - add all of the files from the contents list.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void addAllButton_Click(object sender, EventArgs e)
      {
         this.AddFiles(selectedOnly: false);
      }
   }
}

