namespace TabletApp.Content.Commissioning
{
   partial class ResetDsi
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
         this.messageLabel = new System.Windows.Forms.Label();
         this.label1 = new System.Windows.Forms.Label();
         this.configPath = new System.Windows.Forms.Label();
         this.label3 = new System.Windows.Forms.Label();
         this.dsiAddress = new TabletApp.Views.NumberTextBox();
         this.deleteButton = new System.Windows.Forms.Button();
         this.factoryAddressCheckbox = new System.Windows.Forms.CheckBox();
         this.SuspendLayout();
         // 
         // messageLabel
         // 
         this.messageLabel.Font = new System.Drawing.Font("Arial", 18F);
         this.messageLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
         this.messageLabel.Location = new System.Drawing.Point(36, 28);
         this.messageLabel.Name = "messageLabel";
         this.messageLabel.Size = new System.Drawing.Size(937, 60);
         this.messageLabel.TabIndex = 1;
         this.messageLabel.Text = "Plug your device into the DSI that you would like to reset.  Click the Reset butt" +
    "on to perform the reset.";
         // 
         // label1
         // 
         this.label1.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
         this.label1.Location = new System.Drawing.Point(36, 104);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(162, 35);
         this.label1.TabIndex = 2;
         this.label1.Text = "Config file:";
         // 
         // configPath
         // 
         this.configPath.AutoEllipsis = true;
         this.configPath.Font = new System.Drawing.Font("Arial", 14F);
         this.configPath.ImeMode = System.Windows.Forms.ImeMode.NoControl;
         this.configPath.Location = new System.Drawing.Point(221, 108);
         this.configPath.Name = "configPath";
         this.configPath.Size = new System.Drawing.Size(767, 31);
         this.configPath.TabIndex = 3;
         this.configPath.Text = "No config";
         // 
         // label3
         // 
         this.label3.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
         this.label3.Location = new System.Drawing.Point(36, 150);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(181, 35);
         this.label3.TabIndex = 5;
         this.label3.Text = "DSI address:";
         // 
         // dsiAddress
         // 
         this.dsiAddress.Anchor = System.Windows.Forms.AnchorStyles.Top;
         this.dsiAddress.Font = new System.Drawing.Font("Arial", 14F);
         this.dsiAddress.Increment = 1F;
         this.dsiAddress.IntValue = 1;
         this.dsiAddress.Location = new System.Drawing.Point(223, 150);
         this.dsiAddress.MaxValue = 212F;
         this.dsiAddress.MinValue = 1F;
         this.dsiAddress.Name = "dsiAddress";
         this.dsiAddress.Size = new System.Drawing.Size(76, 29);
         this.dsiAddress.TabIndex = 6;
         this.dsiAddress.Text = "1";
         this.dsiAddress.Value = 1F;
         // 
         // deleteButton
         // 
         this.deleteButton.BackColor = System.Drawing.Color.Transparent;
         this.deleteButton.BackgroundImage = global::TabletApp.Properties.Resources.delete_file;
         this.deleteButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
         this.deleteButton.FlatAppearance.BorderSize = 0;
         this.deleteButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
         this.deleteButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
         this.deleteButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
         this.deleteButton.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
         this.deleteButton.ForeColor = System.Drawing.Color.White;
         this.deleteButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
         this.deleteButton.Location = new System.Drawing.Point(195, 107);
         this.deleteButton.Name = "deleteButton";
         this.deleteButton.Size = new System.Drawing.Size(27, 26);
         this.deleteButton.TabIndex = 76;
         this.deleteButton.Tag = "0";
         this.deleteButton.UseVisualStyleBackColor = false;
         this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
         // 
         // factoryAddressCheckbox
         // 
         this.factoryAddressCheckbox.AutoSize = true;
         this.factoryAddressCheckbox.Font = new System.Drawing.Font("Arial", 14F);
         this.factoryAddressCheckbox.Location = new System.Drawing.Point(305, 152);
         this.factoryAddressCheckbox.Name = "factoryAddressCheckbox";
         this.factoryAddressCheckbox.Size = new System.Drawing.Size(168, 26);
         this.factoryAddressCheckbox.TabIndex = 77;
         this.factoryAddressCheckbox.Text = "Factory Address";
         this.factoryAddressCheckbox.UseVisualStyleBackColor = true;
         this.factoryAddressCheckbox.CheckedChanged += new System.EventHandler(this.factoryAddressCheckbox_CheckedChanged);
         // 
         // ResetDsi
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.factoryAddressCheckbox);
         this.Controls.Add(this.deleteButton);
         this.Controls.Add(this.dsiAddress);
         this.Controls.Add(this.label3);
         this.Controls.Add(this.configPath);
         this.Controls.Add(this.label1);
         this.Controls.Add(this.messageLabel);
         this.Name = "ResetDsi";
         this.Size = new System.Drawing.Size(1000, 558);
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Label messageLabel;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Label configPath;
      private System.Windows.Forms.Label label3;
      private Views.NumberTextBox dsiAddress;
      private System.Windows.Forms.Button deleteButton;
      private System.Windows.Forms.CheckBox factoryAddressCheckbox;
   }
}
