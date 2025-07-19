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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

using DsiApi;
using Model;
using TabletApp.Properties;
using TabletApp.Utils;

namespace TabletApp.Views
{
   public delegate void GraphChanged(AscanWaveform graph);

   /// <summary>
   /// User control that holds the Ascan waveform.
   /// </summary>
   public partial class AscanWaveform : UserControl
   {
      /// <summary>
      /// Constructor
      /// </summary>
      public AscanWaveform()
      {
         InitializeComponent();
         fScroll = 0.0f;
         fZoom = 1.0f;
         fTickFont = new Font("Arial", 6);
         fScrollDir = -1.0f; // TODO: Detect touch device and use -1 for touch, 1 for mouse

         fLabelBrush = new SolidBrush(Color.Black);
      }

      ~AscanWaveform()
      {
         if (null != fBitmap)
         {
            fBitmap.Dispose();
         }
         if (null != fLabelBrush)
         {
            fLabelBrush.Dispose();
         }
         if (null != fTickFont)
         {
            fTickFont.Dispose();
         }
      }

      public event GraphChanged GraphChangedEvent;

      public float GraphZoom
      {
         get { return fZoom; }
         set { fZoom = value; this.ZoomScrollChanged(); }
      }

      public float GraphScroll
      {
         get { return fScroll; }
         set { fScroll = value; this.ZoomScrollChanged(); }
      }

      public int GraphZoomPercent
      {
         get { return (int)(fZoom * 100); }
         set { this.GraphZoom = (float)value / 100f; }
      }

      public int GraphScrollPercent
      {
         get { return (int)((fScroll / this.GraphMaxScroll) * 100f); }
         set { this.GraphScroll = ((float)value / 100f) * this.GraphMaxScroll; }
      }

      private float GraphMaxScroll
      {
         get
         {
            if (null != fProbe && null != fProbe.setups && fProbe.setups.Count() > 0 && null != fProbe.setups[0].ascanData)
            {
               return (float)fProbe.setups[0].ascanData.Length;
            }
            else
            {
               return 0;
            }
         }
      }

      /// <summary>
      /// Populate our UI according to the given probe.
      /// </summary>
      /// <param name="data"></param>
      public void PopulateWithProbe(AProbe probe, bool updateScrollZoom = true)
      {
         fProbe = probe;
         if (null == fProbe.setups[0].fCalculator)
         {
            AThicknessCalculator.PrepareSetupForThicknessCalculation(fProbe, fProbe.setups[0]);
         }

         fBitmap = null;
         if (updateScrollZoom)
         {
            this.GraphZoomPercent = probe.Zoom;
            this.GraphScrollPercent = probe.Scroll;
         }

         // Be lazy and wait until we are asked to paint instead.
         // this.Update();
         // this.DrawGraph();
      }


      /// <summary>
      /// Draw the waveform.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void AAscanWaveform_Paint(object sender, PaintEventArgs e)
      {
         // If we resized, draw again instead of scaling bitmap.
         if ((null != fBitmap)
          && (this.GraphWidth(kInset) != fBitmap.Size.Width 
          || this.GraphHeight(kInset) != fBitmap.Size.Height))
         {
            fBitmap = null;
         }

         if (null == fBitmap)
         {
            this.DrawGraph();
         }

         // There might not have been enough info to draw the bitmap or some other error occurred.
         if (null != fBitmap)
         {
            e.Graphics.DrawImage(fBitmap, 0, 0, this.Bounds.Width, this.Bounds.Height);
         }
      }


      private void AAscanWaveform_MouseDown(object sender, MouseEventArgs e)
      {
         if (!fTrackingMouse && (MouseButtons.None != e.Button))
         {
            fLastPoint = new Point(e.X, e.Y);
            fTrackingMouse = true;
         }
      }


      private void AAscanWaveform_MouseUp(object sender, MouseEventArgs e)
      {
         if (fTrackingMouse)
         {
            this.TrackMouseMove(e);
            fTrackingMouse = false;
         }
      }


      private void AAscanWaveform_MouseMoveZoom(object sender, MouseEventArgs e)
      {
         if (fTrackingMouse)
         {
            this.TrackMouseMove(e);
         }
      }


      private void TrackMouseMove(MouseEventArgs e)
      {
         float dx = e.X - fLastPoint.X;
         float dy = e.Y - fLastPoint.Y;

         this.ZoomAndScroll(dx, dy);

         fLastPoint.X = e.X;
         fLastPoint.Y = e.Y;
      }


      private void DrawGraph()
      {
         if (null == fProbe || null == fProbe.setups)
         {
            return;
         }

         try
         {
            // Cache the portion of the total view area which will contain the graph.
            fGraphRect = this.GraphRect(kInset);

            Bitmap bmp = new Bitmap(this.Bounds.Width, this.Bounds.Height);
            Graphics gOff = Graphics.FromImage(bmp);

            SolidBrush brush = new SolidBrush(Color.White);
            gOff.FillRectangle(brush, 0, 0, bmp.Width, bmp.Height);
            brush.Dispose();

            int horizontalDivisions = 4;// this gives 5 lines.
            this.DrawGrid(horizontalDivisions, gOff);

            ASetup setup = fProbe.setups[0];
            AThicknessCalculator calculator = setup.fCalculator;
            if (null == calculator)
            {
               AThicknessCalculator.PrepareSetupForThicknessCalculation(fProbe, setup);
            }

            if (null != setup && null != setup.ascanData)
            {
               // Set up visible sample range along with conversion to the drawing space.
               fSampleWindow = new ADataView(fScroll, setup.ascanData.Length, fZoom, fGraphRect.Width);

               this.DrawWaveform(setup.ascanData, gOff);
               this.DrawGates(setup, gOff);

               // Figure out on which thicknesses the ticks should fall.
               var thicknesses = this.CalculateTickThicknesses(setup, minTicks: 10);
               this.DrawTicks(setup, thicknesses, gOff);

               this.DrawCrossings(setup.zeroCrossings, (float)setup.crossingDelayCompensation, gOff);
            }

            // Looks better drawn over others
            this.DrawAxes(gOff);

            fBitmap = bmp;

            gOff.Dispose();
         }
         catch (Exception ex)
         {
            AOutput.LogException(ex);
         }
      }


      private void DrawAxes(Graphics g)
      {
         var blackPen = new Pen(Color.Black);

         // Y Axis
         g.DrawLine(blackPen, fGraphRect.Left, fGraphRect.Top, fGraphRect.Left, fGraphRect.Bottom);

         // X Axis
         var middleY = fGraphRect.Top + fGraphRect.Height / 2;
         g.DrawLine(blackPen, fGraphRect.Left, middleY, fGraphRect.Right, middleY);

         blackPen.Dispose();
      }


      /// <param name="minTicks">The minimum number of ticks to calculate between fSampleWindow.Start
      /// and fSampleWindow.End</param>
      /// <param name="ascanStart">A translation of the starting thickness, in low resolution sample
      /// indices</param>
      /// <returns>Array of thicknesses at which ticks should occur. If fewer than minTicks would be
      /// visible in the sample window, the space between ticks is reduced.</returns>
      private double[] CalculateTickThicknesses(ASetup setup, int minTicks)
      {
         var calculator = setup.fCalculator;

         // Time, here, means the sample tick, i.e. index into ascan.
         double time = fSampleWindow.End;
         double endThickness = calculator.ThicknessAtRelativeTime(time, this.ZeroTick(setup)) * kUpscaleFactor;

         time = fSampleWindow.Start;
         double startThickness = calculator.ThicknessAtRelativeTime(time, this.ZeroTick(setup)) * kUpscaleFactor;

         double totalThickness = endThickness - startThickness;

         double thicknessPerTick = this.FindThicknessPerTick(totalThickness, minTicks);

         // Add one for left most tick manually added.
         int numTicks = (int)(totalThickness / thicknessPerTick) + 1;

         var thicknesses = new double[numTicks + 1]; // one for final thickness
         thicknesses[0] = startThickness;
         thicknesses[numTicks] = endThickness;

         double thickness = startThickness;
 
         // Jump to next even boundary.
         double used = (thickness % thicknessPerTick);
         double remainder = (thicknessPerTick - used);
         if (thickness < 0)
         {
            remainder = -used;
         }
         thickness += remainder;

         int i = 0;
         while (++i < numTicks)
         {
            thicknesses[i] = thickness;
            thickness += thicknessPerTick;
         }

         return thicknesses;
      }


      /// <returns>The tick to be considered thickness 0</returns>
      private double ZeroTick(ASetup setup)
      {
         double zeroTick = 0.0;
         switch (AThicknessCalculator.DetectionModeForGates(setup.gates))
         {
            case AThicknessCalculator.DetectionMode.kBangToFirstEcho:
            zeroTick = -setup.ascanStart;
            break;

            case AThicknessCalculator.DetectionMode.kDelayLineToFirstWall:
            zeroTick = setup.zeroCrossings[0];
            break;

            case AThicknessCalculator.DetectionMode.kMultiEcho:
            default:
            break;
         }

         // 7 is Jim's magic number which I now use in setup.crossingDelayOffset. 
         return zeroTick - setup.crossingDelayCompensation + fProbe.calZeroOffset * ADsiInfo.kSampleRate;
      }


      /// <param name="zeroCrossings">Low-res sample index where a crossing should be drawn</param>
      /// <param name="phaseDelay">Number of low-res samples to subtract from zeroCrossing value to correct for phase delay introduced by upsampling</param>
      private void DrawCrossings(double[] zeroCrossings, float phaseDelay, Graphics g)
      {
         int crossingIndex = 0;
         SolidBrush brush = new SolidBrush(fColors[crossingIndex]);

         GraphicsContainer gState = g.BeginContainer();
         g.SmoothingMode = SmoothingMode.AntiAlias;

         float lastCrossing = (float)zeroCrossings[0];
         float bottomY = fGraphRect.Bottom;
         float x;
         foreach (var crossing in zeroCrossings)
         {
            // Drawing is in the low-res space so compensate for the upsampling filter's phase shift.
            // Assume the high-res crossing falls somewhere between the calculated low res tick
            // and the next one.
            float compensatedCrossing = (float)crossing - phaseDelay + 0.5f;
            if (!float.IsNaN(compensatedCrossing) && fSampleWindow.Contains(compensatedCrossing))
            {
               float pixelCrossing = fSampleWindow.ConvertPosition(compensatedCrossing);
               if (pixelCrossing >= 0) // indicates no gate or gate before sample window, so no crossing
               {
                  brush.Color = fColors[crossingIndex];

                  x = fGraphRect.Left + pixelCrossing;
      
                  float height = fTickFont.GetHeight() - 2.0f;
                  float tY = bottomY + 1.0f;

                  var triangle = new PointF[4];
                  triangle[0] = new PointF(x, tY);
                  triangle[1] = new PointF(x + height * 0.5f, tY + height);
                  triangle[2] = new PointF(x - height * 0.5f, tY + height);
                  triangle[3] = triangle[0];
                  g.FillPolygon(brush, triangle);

#if false // DEBUG - show thickness above marker.
                  var setup = fProbe.setups[0];
                  var mode = AThicknessCalculator.DetectionModeForGates(fProbe.setups[0].gates);
                  Boolean isModeZero = AThicknessCalculator.DetectionMode.kBangToFirstEcho == mode;
                  if (crossingIndex > 0 || isModeZero)
                  {
                     var thickness = 0.0;
                     if (isModeZero)
                     {
                        thickness = setup.fCalculator.ThicknessAtRelativeTime(compensatedCrossing, this.ZeroTick(setup), kUpscaleFactor);
                     }
                     else
                     {
                        thickness = setup.fCalculator.ThicknessAtRelativeTime(compensatedCrossing, lastCrossing, kUpscaleFactor);
                     }
                     var label = ADataExtensions.FormatAsMeasurmentString(thickness.ToString(), addUnits: false);
                     var labelSize = g.MeasureString(label, fTickFont);
                     var b = new SolidBrush(Color.Black);
                     var labelX = triangle[0].X;
                     var labelY = tY - labelSize.Height - 4;
                     g.FillRectangle(b, labelX, labelY, labelSize.Width, labelSize.Height);
                     b.Color = Color.White;
                     g.DrawString(label, fTickFont, b, labelX, labelY);
                     b.Dispose();
                  }
#endif
               }
            }

            lastCrossing = compensatedCrossing;
            ++crossingIndex;
         }

         g.EndContainer(gState);
         brush.Dispose();
      }


      private void DrawGates(ASetup setup, Graphics g)
      {
         var gates = setup.gates;
         if (null == gates)
         {
            return;
         }
         Debug.Assert(gates.Length <= fColors.Length); // else we don't have enough pen colors ready

         float ampScale = this.CalculateAmpScale();

         float maxSample = fSampleWindow.End;

         float maxX = fGraphRect.Right;
         float midY = fGraphRect.Top + fGraphRect.Height / 2;

         int c = 0;
         var pen = new Pen(fColors[c]);

#if false // DEBUG draw calzero
         gates[0].start = (uint)this.ZeroTick(setup);
         gates[0].width = (uint)(fProbe.calZeroOffset * ADsiInfo.kSampleRate);
         gates[0].threshold = 14000;
#endif

         var mode = AThicknessCalculator.DetectionModeForGates(gates);

         Boolean isModeOne = AThicknessCalculator.DetectionMode.kDelayLineToFirstWall == mode;

         // We make the gate start relative to the samples by subtracting ascanStart.
         float gateStart = -setup.ascanStart;

         for ( ; c < gates.Length; ++c)
         {
            AMarshaledGate gate = gates[c];
            // Second gate in this mode is relative to the crossing of the first.
            if (isModeOne &&  c > 0)
            {
               gateStart = (float)(setup.zeroCrossings[c - 1] + gate.start);
            }
            else
            {
               gateStart += (float)gate.start;
            }

            // Correct for filter delay which is applied to label thicknesses.
            // Bump it a little to account for any sample interpolation.
            gateStart += -(float)setup.crossingDelayCompensation + 0.5f;

            if ((0 == gate.width) || (gateStart > fSampleWindow.End))
            {
               continue;
            }

            //Console.WriteLine("Gate plotted from {0} to {1}", gateStart, gateStart + gate.width);

            float gateWidth = (float)gate.width * fSampleWindow.Conversion;

            float x1 = fGraphRect.Left + fSampleWindow.ConvertPosition(gateStart);
            if (x1 < fGraphRect.Left)
            {
               gateWidth -= (fGraphRect.Left - x1);
               x1 = fGraphRect.Left;
            }

            float x2 = Math.Min(x1 + gateWidth, maxX);
            if (x2 < fGraphRect.Left)
            {
               continue;
            }

            float y = midY - (float)gate.threshold * ampScale;

            pen.Color = fColors[c];
            g.DrawLine(pen, x1, y, x2, y);
         }

         pen.Dispose();
      }

      private void DrawGrid(int divisions, Graphics g)
      {
         float width = fGraphRect.Width;

         var grayPen = new Pen(Color.LightGray);

         float x1 = fGraphRect.Left;
         float x2 = fGraphRect.Right;
         float y = fGraphRect.Top;
         float yOff = fGraphRect.Height / (float)divisions;

         while (y <= fGraphRect.Bottom)
         {
            g.DrawLine(grayPen, x1, y, x2, y);
            y += yOff;
         }

         grayPen.Dispose();
      }


      /// <param name="thickness">The thickness to label the tick.</param>
      private void DrawTick(double thickness, float x, float y1, float y2, Pen tickPen,
       Graphics g)
      {
         // Draw tick
         g.DrawLine(tickPen, x, y1, x, y2);

         var label = ADataExtensions.FormatAsMeasurmentString(thickness.ToString(), addUnits: false);
         /* Debug - Show indices instead of thicknesses */
         //label = String.Format("{0:0.00}", time);
         g.DrawString(label, fTickFont, fLabelBrush, x, y1);
      }


      private void DrawTicks(ASetup setup, double[] thicknesses, Graphics g)
      {
         // For reference, 1 sample index = 25ns, according to DSI documentation

         // Cache tick y values.
         const float kTickMarkLength = 3.0f;
         var p1 = new PointF(fGraphRect.Left, fGraphRect.Bottom);
         var p2 = p1;
         p2.Y += kTickMarkLength;

         float endX = fGraphRect.Right;

         var zeroThick = thicknesses[0];
         var lastThick = thicknesses.Last();
         double scale = fGraphRect.Width / (lastThick - zeroThick);
         var pixels = Array.ConvertAll(thicknesses,
            th => (th - zeroThick) * scale + fGraphRect.Left);

         // Don't draw the first tick if the next one is too close.
         const float kMinPixels = 25;
         int skipCount = 0;
         if (pixels[1] - pixels[0] < kMinPixels)
         {
            skipCount = 1;
         }

         var grayPen = new Pen(Color.LightGray);

         float x = -kMinPixels;
         int thickIdx = skipCount;
         foreach (float newX in pixels.Skip(skipCount))
         {
            if (newX > endX)
            {
               break;
            }
            // Prevent too many ticks from drawing. (Can happen with misconfiguration or wild thicknesses)
            if (newX < (x + kMinPixels))
            {
               thickIdx += 1;
               continue;
            }

            x = newX;
            this.DrawTick(thicknesses[thickIdx++], x, p1.Y, p2.Y, grayPen, g);
         }

         grayPen.Dispose();
      }


      private void DrawWaveform(Int16[] samples, Graphics g)
      {
         if (null == samples)
         {
            return;
         }

         // draw here according to our AProbe data
         // Just draw first setup's ascan.
         int width = (int)fGraphRect.Width;

         // Calculate some useful values.
       
         float minY = fGraphRect.Top;
         float midY = minY + fGraphRect.Height / 2.0f;
         float maxY = fGraphRect.Bottom;

         float ampScale = this.CalculateAmpScale();

         // Precalculate points and make one call to DrawLines
         PointF[] points = null;
         PointF pt = new PointF();

         // When there are more samples than pixels
         if (fSampleWindow.Count > width)
         { 
            float exactSample = fSampleWindow.Start;
            float endSamples = fSampleWindow.End;

            float samplesPerPixel = fSampleWindow.Count / fGraphRect.Width;

            points = new PointF[width];
            points[0] = new PointF((float)fGraphRect.Left, midY);
            pt = points[0];

            int i = 0;
            while (i < width)
            {
               int s = (int)Math.Floor(exactSample);

               bool doMin = false;
               float val = -kMaxSampleValue;
               if (samples[s] < 0)
               {
                  doMin = true;
                  val = kMaxSampleValue;
               }

               // End of sample range for this x, also becomes start of next sample group
               exactSample += samplesPerPixel;

               // Just find the min/max in the range of sample represented by the pixel.
               int endSample = (int)Math.Round(exactSample);
               for (; endSample != s && s < samples.Length; ++s)
               {
                  if (doMin)
                  {
                     val = Math.Min(val, (float)samples[s]);
                  }
                  else
                  {
                     val = Math.Max(val, (float)samples[s]);
                  }
               }

               pt.X += 1;

               // 0 on y-axis is at top of view, so we want negative values to increase y.
               pt.Y = this.Clip(midY - val * ampScale, minY, maxY);

               points[i++] = pt;
            }
         }
         else // fewer samples than pixels.
         {
            // Interpolate data between samples (by having DrawLines do it)

            points = new PointF[(int)fSampleWindow.Count];
            pt.X = fGraphRect.Left;

            float xOff = fSampleWindow.Conversion;
          
            int i = 0;
            int s = (int)fSampleWindow.Start;

            while (i < points.Length)
            {
               pt.Y = this.Clip(midY - (float)samples[s++] * ampScale, minY, maxY);
               points[i++] = pt;

               pt.X += xOff;
            }
         }

         GraphicsContainer gState = g.BeginContainer();
         g.SmoothingMode = SmoothingMode.AntiAlias;

         Pen grayPen = new Pen(Color.Gray);
         g.DrawLines(grayPen, points);
         grayPen.Dispose();

         g.EndContainer(gState);
      }


      private double FindThicknessPerTick(double totalThickness, int minTicks)
      {
         double thicknessPerTick = 0.01;
         int numTicks = (int)(totalThickness / thicknessPerTick);

         // Search for satisfying tick placement within constraint of minimum ticks.
         if (Settings.Default.ThicknessIsMetric)
         {
            double magnitude = 100.0; // starts with 1 cm ticks
            do
            {
               thicknessPerTick = 1.0 / magnitude;
               numTicks = (int)(totalThickness / thicknessPerTick);
               magnitude *= 2.0;
            } while (numTicks < minTicks);
         }
         else // imperial units
         {
            const double kInchesPerMeter = 0.0254;
            int n = 4; // 1/32" is good default for unzoomed ascan.
            do
            {
               thicknessPerTick = 12.0 * kInchesPerMeter / (double)(1 << n); // feet converted to meters.
               numTicks = (int)(totalThickness / thicknessPerTick);
               n += 1;
            } while (numTicks < minTicks);
         }

         return thicknessPerTick;
      }


      private float Clip(float v, float min, float max)
      {
         if (v < min)
            return min;
         if (v > max)
            return max;
         return v;
      }


      private float CalculateAmpScale()
      {
         return (float)fGraphRect.Height / (2.0f * kMaxSampleValue);
      }


      private RectangleF GraphRect(int inset)
      {
         return new RectangleF(inset, inset, this.GraphWidth(inset), this.GraphHeight(inset));
      }


      private int GraphHeight(int inset)
      {
         return this.Bounds.Height - 2 * inset;
      }


      private int GraphWidth(int inset)
      {
         return this.Bounds.Width - 2 * inset;
      }

      /**
       * Update Zoom and Scroll, redraw the graph, and force a redraw.
       */
      private void ZoomAndScroll(float dx, float dy)
      {
         // Vertical movement zooms, horizontal movement scrolls.
         if (Math.Abs(dy) > Math.Abs(dx))
         {
            fZoom -= dy / (this.Bounds.Height * 5.0f); // throttled zooming
         }
         else
         {
            // Scale scrolling to sample space.
            float numSamples = this.GraphMaxScroll;
            fScroll += fScrollDir * dx * numSamples / this.GraphWidth(kInset);
            //Console.WriteLine("Scroll: {0}", fScroll);
         }
         this.ZoomScrollChanged();
      }

      /// <summary>
      /// Call after changing fZoom or fScroll to validate and redraw graph
      /// </summary>
      private void ZoomScrollChanged()
      {
         this.ValidateZoomScroll();
         this.DrawGraph();
         this.Refresh();
         if (null != this.GraphChangedEvent)
         {
            this.GraphChangedEvent(this);
         }
      }

      private void ValidateZoomScroll()
      {
         fZoom = this.Clip(fZoom, 0.02f, 1.0f);
         if (1.0f <= fZoom)
         {
            fScroll = 0.0f;
         }
         fScroll = this.Clip(fScroll, 0.0f, this.GraphMaxScroll);
      }

      // TODO: Configuration for colors of gates/crossing
      Color[] fColors = { Color.Green, Color.Red, Color.Blue };

      private AProbe fProbe;
      private Bitmap fBitmap;

      private Font fTickFont;
      private Brush fLabelBrush;

      private ADataView fSampleWindow; //!< bounds of samples being drawn, as indices into ascan data.
      private RectangleF fGraphRect;   //!< bounds of the graph drawing in pixels

      const int kInset = 10;
      const float kMaxSampleValue = 25000.0f;
      const double kUpscaleFactor = 8.0; // Conversion from low to high resolution sample space. I wish this didn't need to know.

      private bool fTrackingMouse;
      private Point fLastPoint;
      private float fZoom;
      private float fScroll;
      private float fScrollDir; //!< Reverses scroll direction when using touch device.
   }

   /**
    * Manages a view into a one dimensional data set, and conversion to one other space
    * through scaling.
    */
   class ADataView
   {
      /// <param name="scroll">The start index of the view into the data.</param>
      /// <param name="totalCount">The total number of samples in the data set.</param>
      /// <param name="zoom">The relative width of the view to the total set.E.g. 0.5 puts half the samples in the view.</param>
      /// <param name="destinationWidth">The size of the space to which indices are mapped. Assumes this.Start is at 0 in the destination space and End is at destinationWidth.</param>
      public ADataView(float scroll, float totalCount, float zoom, float destinationWidth)
      {
         float count = totalCount;

         // Limit scroll to [0,totalCount)
         if (scroll >= totalCount)
         {
            scroll = count - 1;
         }
         else if (scroll < 0)
         {
            scroll = 0;
         }
         
         // Apply zoom and limit
         count *= zoom;
         if (count < 2)
         {
            count = 2;
         }

         // Keep a full window by backing up scroll if count would take it past end of data.
         if ((scroll + count) > totalCount)
         {
            scroll = totalCount - count;
         }

         this.Start = scroll;
         this.Count = count;
         this.Conversion = destinationWidth / count;
      }

      public float Start { get; set; }
      public float Count { get; set; }
      public float End { get { return this.Start + this.Count;  } }


      public bool Contains(float samplePos)
      {
         return ((this.Start <= samplePos) && (samplePos < this.End));
      }

      /// <param name="samplePos"> Index of sample in total sample space.</param>
      /// <returns>samplePos mapped from window to destination space.</returns>
      public float ConvertPosition(float samplePos)
      {
         return (samplePos - this.Start) * this.Conversion;
      }

      /// <summary>
      /// Factor for converting indices from the data set space to destination space.
      /// </summary>
      public float Conversion { get; private set; }
   }
}

