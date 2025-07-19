using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabletApp.Properties;

namespace TabletApp.Views
{
   /// <summary>
   /// Delegate for receiving changes to current shot number
   /// </summary>
   /// <param name="buttonIndex"></param>
   public delegate void CurrentShotChanged(int currentShot);

   /// <summary>
   /// Represents the navigation control for a data-logger DSI, allowing the user
   /// to step through all of the shots stored on the DSI.
   /// </summary>
   public partial class ShotNavigator : UserControl
   {
      public event CurrentShotChanged ShotChangedEvent;

      private int fTotalShots = 0;
      private int fCurrentShot = 1;    // 1-based
      private bool fEditMode = false;

      public int CurrentShot
      {
         get
         {
            return fCurrentShot;
         }
      }

      public ShotNavigator()
      {
         InitializeComponent();
         this.SetEditMode(false);
         this.editModeShotNumberTextBox.EntryCompleteEvent += EditModeShotNumberTextBox_EntryCompleteEvent;
         this.editModeShotNumberTextBox.MinValue = 1;
      }

      /// <summary>
      /// User changed the current shot via the edit box.  Update our shot info.
      /// </summary>
      /// <param name="numberTextBox"></param>
      private void EditModeShotNumberTextBox_EntryCompleteEvent(NumberTextBox numberTextBox)
      {
         this.SetShotInfo(numberTextBox.IntValue, fTotalShots);
         NotifyShotChange();
      }

      /// <summary>
      /// Set the title.
      /// </summary>
      /// <param name="text"></param>
      public void SetTitleText(string text)
      {
         this.progressText.Text = text;
      }

      /// <summary>
      /// Info mode represents a static view of the shot info, hiding the step arrows
      /// and showing the total number of shots available.  Otherwise, show the step
      /// arrows and show the current shot number.
      /// </summary>
      /// <param name="infoMode"></param>
      public void SetInfoMode(bool infoMode)
      {
         leftButton.Visible = !infoMode;
         rightButton.Visible = !infoMode;
         shotInfoLabel.Text = (infoMode ? Resources.DataLoggerTotalShots : Resources.DataLoggerCurrentShotNumber);
      }

      /// <summary>
      /// Edit mode presents an editable text box to the user for the current shot number.
      /// </summary>
      /// <param name="editMode"></param>
      public void SetEditMode(bool editMode)
      {
         fEditMode = editMode;
         editModeShotNumberTextBox.Visible = editMode;
         editModeTotalShotsLabel.Visible = editMode;
         shotNumberLabel.Visible = !editMode;
      }

      /// <summary>
      /// Set the total number of shots available.
      /// </summary>
      /// <param name="numShots"></param>
      public void SetNumShots(int numShots)
      {
         shotNumberLabel.Text = "" + numShots;
         fTotalShots = numShots;
      }

      /// <summary>
      /// Set the shot info for non-info mode.
      /// </summary>
      /// <param name="currentShot"></param>
      /// <param name="totalShots"></param>
      public void SetShotInfo(int currentShot, int totalShots)
      {
         if (this.InvokeRequired)
         {
            this.Invoke(new Action(() => this.SetShotInfo(currentShot, totalShots)));
            return;
         }

         fTotalShots = totalShots;
         fCurrentShot = currentShot;
         if (fEditMode)
         {
            editModeShotNumberTextBox.IntValue = currentShot;
            editModeTotalShotsLabel.Text = "of " + totalShots;
         }
         else
         {
            shotNumberLabel.Text = "" + currentShot + " of " + totalShots;
         }
         shotInfoLabel.Text = Resources.DataLoggerCurrentShotNumber;
         editModeShotNumberTextBox.MaxValue = totalShots;
      }

      /// <summary>
      /// Update the current shot number.
      /// </summary>
      /// <param name="currentShot"></param>
      public void UpdateCurrentShot(int currentShot)
      {
         this.SetShotInfo(currentShot, fTotalShots);
      }

      /// <summary>
      /// Increment the current shot by one.
      /// </summary>
      public void IncrementCurrentShot()
      {
         this.UpdateCurrentShot(++fCurrentShot);
      }

      /// <summary>
      /// Notify any event listeners that the current shot has changed.
      /// </summary>
      private void NotifyShotChange()
      {
         if (null != this.ShotChangedEvent)
         {
            this.ShotChangedEvent(fCurrentShot);
         }
      }

      /// <summary>
      /// Right button click - increment shot number.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void rightButton_Click(object sender, EventArgs e)
      {
         if (fCurrentShot + 1 <= fTotalShots)
         {
            ++fCurrentShot;
            this.UpdateCurrentShot(fCurrentShot);
            NotifyShotChange();
         }
      }

      /// <summary>
      /// Left button click - decrement shot number.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void leftButton_Click(object sender, EventArgs e)
      {
         if (fCurrentShot - 1 > 0)
         {
            --fCurrentShot;
            this.UpdateCurrentShot(fCurrentShot);
            NotifyShotChange();
         }
      }
   }
}
