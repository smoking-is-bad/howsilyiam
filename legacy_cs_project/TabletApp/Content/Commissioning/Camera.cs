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
using AForge.Video;
using AForge.Video.DirectShow;
using System.Media;
using TabletApp.Utils;
using TabletApp.Properties;
using System.IO;
using System.Drawing.Imaging;

namespace TabletApp.Content.Commissioning
{
   /// <summary>
   /// User control for the camera screen.  Allow the user to take a picture.
   /// </summary>
   public partial class Camera : BaseContent
   {
      private FilterInfoCollection fCameras;
      private VideoCaptureDevice fCaptureDevice;
      private bool fShowVideo = true;

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="parameters">Parameters from the state def file</param>
      public Camera(Dictionary<string, string> parameters)
         : base(parameters)
      {
         InitializeComponent();
         fCameras = new FilterInfoCollection(FilterCategory.VideoInputDevice);
         foreach (FilterInfo Device in fCameras)
         {
            this.deviceComboBox.Items.Add(Device.Name);
         }

         this.deviceComboBox.SelectedIndex = fCameras.Count - 1;
         this.StartNewDevice();
      }

      /// <summary>
      /// Start capturing on the default camera.
      /// </summary>
      private void StartNewDevice()
      {
         if (null != fCaptureDevice)
         {
            fCaptureDevice.Stop();
         }
         if (fCameras.Count > 0)
         {
            fCaptureDevice = new VideoCaptureDevice(fCameras[this.deviceComboBox.SelectedIndex].MonikerString);
            fCaptureDevice.NewFrame += new NewFrameEventHandler(HandleNewFrame);
            fCaptureDevice.Start();
         }
         else
         {
            this.takePictureButton.Enabled = false;
            AOutput.DisplayError(Resources.ErrorCameraNotFound);
         }
      }

      /// <summary>
      /// Handle a new frame - show it in our picture box.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="eventArgs"></param>
      private void HandleNewFrame(object sender, NewFrameEventArgs eventArgs)
      {
         if (fShowVideo)
         {
            this.pictureBox.Image = (Bitmap)eventArgs.Frame.Clone();
         }
      }

      /// <summary>
      /// About to disappear - clean up.
      /// </summary>
      public override void WillDisappear()
      {
         if (null != fCaptureDevice)
         {
            fCaptureDevice.Stop();
         }
      }

      /// <summary>
      /// Take a picture.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void takePictureButton_Click(object sender, EventArgs e)
      {
         // camera sound
         AUtilities.PlaySound("TabletApp.Resources.shutter.wav");

         // save our png
         using (Bitmap bitmap = new Bitmap(this.pictureBox.Image))
         {
            string saveDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), Resources.RootXmlFolderName);
            string saveFile = Path.Combine(saveDir, "ultrasense.png");
            bitmap.Save(saveFile, ImageFormat.Png);
            // !!! TODO save to appropriate folder/filename
         }

         // pause the video for a couple seconds
         fShowVideo = false;
         Task.Run(
            async () =>
            {
               await Task.Delay(2000);
               fShowVideo = true;
            }
         );
      }

      private void deviceComboBox_SelectedIndexChanged(object sender, EventArgs e)
      {
         this.StartNewDevice();
      }

      private void takePictureButton_EnabledChanged(object sender, EventArgs e)
      {
         Button button = sender as Button;
         button.BackgroundImage = (button.Enabled ? Resources.blue_button_small : Resources.gray_button);
      }
   }
}

