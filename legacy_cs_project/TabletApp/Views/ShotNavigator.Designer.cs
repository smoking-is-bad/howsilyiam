namespace TabletApp.Views
{
   partial class ShotNavigator
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
         this.shotNumberLabel = new System.Windows.Forms.Label();
         this.progressText = new System.Windows.Forms.Label();
         this.shotInfoLabel = new System.Windows.Forms.Label();
         this.leftButton = new System.Windows.Forms.Button();
         this.rightButton = new System.Windows.Forms.Button();
         this.editModeShotNumberTextBox = new TabletApp.Views.NumberTextBox();
         this.editModeTotalShotsLabel = new System.Windows.Forms.Label();
         this.SuspendLayout();
         // 
         // shotNumberLabel
         // 
         this.shotNumberLabel.Font = new System.Drawing.Font("Arial", 52F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.shotNumberLabel.Location = new System.Drawing.Point(134, 34);
         this.shotNumberLabel.Name = "shotNumberLabel";
         this.shotNumberLabel.Size = new System.Drawing.Size(445, 101);
         this.shotNumberLabel.TabIndex = 2;
         this.shotNumberLabel.Text = "1";
         this.shotNumberLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
         // 
         // progressText
         // 
         this.progressText.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
         this.progressText.ImeMode = System.Windows.Forms.ImeMode.NoControl;
         this.progressText.Location = new System.Drawing.Point(176, 16);
         this.progressText.Name = "progressText";
         this.progressText.Size = new System.Drawing.Size(364, 18);
         this.progressText.TabIndex = 3;
         this.progressText.Text = "Data Logger DSI";
         this.progressText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
         // 
         // shotInfoLabel
         // 
         this.shotInfoLabel.Font = new System.Drawing.Font("Arial", 12F);
         this.shotInfoLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
         this.shotInfoLabel.Location = new System.Drawing.Point(176, 135);
         this.shotInfoLabel.Name = "shotInfoLabel";
         this.shotInfoLabel.Size = new System.Drawing.Size(364, 18);
         this.shotInfoLabel.TabIndex = 4;
         this.shotInfoLabel.Text = "Current Reading Number";
         this.shotInfoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
         // 
         // leftButton
         // 
         this.leftButton.BackgroundImage = global::TabletApp.Properties.Resources.blue_button_left;
         this.leftButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
         this.leftButton.FlatAppearance.BorderSize = 0;
         this.leftButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
         this.leftButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
         this.leftButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
         this.leftButton.Font = new System.Drawing.Font("Arial", 9F);
         this.leftButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
         this.leftButton.Location = new System.Drawing.Point(8, 27);
         this.leftButton.Name = "leftButton";
         this.leftButton.Size = new System.Drawing.Size(120, 120);
         this.leftButton.TabIndex = 5;
         this.leftButton.UseVisualStyleBackColor = true;
         this.leftButton.Click += new System.EventHandler(this.leftButton_Click);
         // 
         // rightButton
         // 
         this.rightButton.BackgroundImage = global::TabletApp.Properties.Resources.blue_button_right;
         this.rightButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
         this.rightButton.FlatAppearance.BorderSize = 0;
         this.rightButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
         this.rightButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
         this.rightButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
         this.rightButton.Font = new System.Drawing.Font("Arial", 9F);
         this.rightButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
         this.rightButton.Location = new System.Drawing.Point(585, 27);
         this.rightButton.Name = "rightButton";
         this.rightButton.Size = new System.Drawing.Size(120, 120);
         this.rightButton.TabIndex = 6;
         this.rightButton.UseVisualStyleBackColor = true;
         this.rightButton.Click += new System.EventHandler(this.rightButton_Click);
         // 
         // editModeShotNumberTextBox
         // 
         this.editModeShotNumberTextBox.Font = new System.Drawing.Font("Arial", 52F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.editModeShotNumberTextBox.Increment = 1F;
         this.editModeShotNumberTextBox.IntValue = 1;
         this.editModeShotNumberTextBox.Location = new System.Drawing.Point(148, 42);
         this.editModeShotNumberTextBox.MaxValue = 32767F;
         this.editModeShotNumberTextBox.MinValue = -32768F;
         this.editModeShotNumberTextBox.Name = "editModeShotNumberTextBox";
         this.editModeShotNumberTextBox.Size = new System.Drawing.Size(146, 87);
         this.editModeShotNumberTextBox.TabIndex = 7;
         this.editModeShotNumberTextBox.Text = "1";
         this.editModeShotNumberTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
         this.editModeShotNumberTextBox.Value = 1F;
         // 
         // editModeTotalShotsLabel
         // 
         this.editModeTotalShotsLabel.Font = new System.Drawing.Font("Arial", 52F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.editModeTotalShotsLabel.Location = new System.Drawing.Point(300, 37);
         this.editModeTotalShotsLabel.Name = "editModeTotalShotsLabel";
         this.editModeTotalShotsLabel.Size = new System.Drawing.Size(249, 92);
         this.editModeTotalShotsLabel.TabIndex = 8;
         this.editModeTotalShotsLabel.Text = "of XXX";
         this.editModeTotalShotsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // ShotNavigator
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.editModeTotalShotsLabel);
         this.Controls.Add(this.editModeShotNumberTextBox);
         this.Controls.Add(this.rightButton);
         this.Controls.Add(this.leftButton);
         this.Controls.Add(this.shotInfoLabel);
         this.Controls.Add(this.progressText);
         this.Controls.Add(this.shotNumberLabel);
         this.Name = "ShotNavigator";
         this.Size = new System.Drawing.Size(714, 177);
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion
      private System.Windows.Forms.Label shotNumberLabel;
      private System.Windows.Forms.Label progressText;
      private System.Windows.Forms.Label shotInfoLabel;
      private System.Windows.Forms.Button leftButton;
      private System.Windows.Forms.Button rightButton;
      private NumberTextBox editModeShotNumberTextBox;
      private System.Windows.Forms.Label editModeTotalShotsLabel;
   }
}
