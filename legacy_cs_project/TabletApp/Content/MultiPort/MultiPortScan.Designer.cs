namespace TabletApp.Content.MultiPort
{
    partial class MultiPortScan
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
            this.portsListBox = new System.Windows.Forms.CheckedListBox();
            this.scanButton = new System.Windows.Forms.Button();
            this.processButton = new System.Windows.Forms.Button();
            this.resultsListView = new System.Windows.Forms.ListView();
            this.portColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.statusColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.dsiCountColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.detailsColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.statusLabel = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.uploadCheckBox = new System.Windows.Forms.CheckBox();
            this.refreshPortsButton = new System.Windows.Forms.Button();
            this.selectAllCheckBox = new System.Windows.Forms.CheckBox();
            this.portsLabel = new System.Windows.Forms.Label();
            this.resultsLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // portsListBox
            // 
            this.portsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.portsListBox.FormattingEnabled = true;
            this.portsListBox.Location = new System.Drawing.Point(20, 40);
            this.portsListBox.Name = "portsListBox";
            this.portsListBox.Size = new System.Drawing.Size(200, 124);
            this.portsListBox.TabIndex = 0;
            // 
            // scanButton
            // 
            this.scanButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.scanButton.Location = new System.Drawing.Point(240, 40);
            this.scanButton.Name = "scanButton";
            this.scanButton.Size = new System.Drawing.Size(100, 30);
            this.scanButton.TabIndex = 1;
            this.scanButton.Text = "Scan Ports";
            this.scanButton.UseVisualStyleBackColor = true;
            this.scanButton.Click += new System.EventHandler(this.scanButton_Click);
            // 
            // processButton
            // 
            this.processButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.processButton.Enabled = false;
            this.processButton.Location = new System.Drawing.Point(240, 80);
            this.processButton.Name = "processButton";
            this.processButton.Size = new System.Drawing.Size(100, 30);
            this.processButton.TabIndex = 2;
            this.processButton.Text = "Read && Upload";
            this.processButton.UseVisualStyleBackColor = true;
            this.processButton.Click += new System.EventHandler(this.processButton_Click);
            // 
            // resultsListView
            // 
            this.resultsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.resultsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.portColumn,
            this.statusColumn,
            this.dsiCountColumn,
            this.detailsColumn});
            this.resultsListView.FullRowSelect = true;
            this.resultsListView.GridLines = true;
            this.resultsListView.Location = new System.Drawing.Point(20, 220);
            this.resultsListView.Name = "resultsListView";
            this.resultsListView.Size = new System.Drawing.Size(560, 200);
            this.resultsListView.TabIndex = 3;
            this.resultsListView.UseCompatibleStateImageBehavior = false;
            this.resultsListView.View = System.Windows.Forms.View.Details;
            // 
            // portColumn
            // 
            this.portColumn.Text = "Port";
            this.portColumn.Width = 80;
            // 
            // statusColumn
            // 
            this.statusColumn.Text = "Status";
            this.statusColumn.Width = 100;
            // 
            // dsiCountColumn
            // 
            this.dsiCountColumn.Text = "DSIs Found";
            this.dsiCountColumn.Width = 100;
            // 
            // detailsColumn
            // 
            this.detailsColumn.Text = "Details";
            this.detailsColumn.Width = 276;
            // 
            // statusLabel
            // 
            this.statusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statusLabel.Location = new System.Drawing.Point(20, 440);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(560, 20);
            this.statusLabel.TabIndex = 4;
            this.statusLabel.Text = "Ready";
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(20, 470);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(560, 23);
            this.progressBar.TabIndex = 5;
            this.progressBar.Visible = false;
            // 
            // uploadCheckBox
            // 
            this.uploadCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.uploadCheckBox.AutoSize = true;
            this.uploadCheckBox.Checked = true;
            this.uploadCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.uploadCheckBox.Location = new System.Drawing.Point(240, 120);
            this.uploadCheckBox.Name = "uploadCheckBox";
            this.uploadCheckBox.Size = new System.Drawing.Size(98, 17);
            this.uploadCheckBox.TabIndex = 6;
            this.uploadCheckBox.Text = "Upload to Web";
            this.uploadCheckBox.UseVisualStyleBackColor = true;
            // 
            // refreshPortsButton
            // 
            this.refreshPortsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.refreshPortsButton.Location = new System.Drawing.Point(360, 40);
            this.refreshPortsButton.Name = "refreshPortsButton";
            this.refreshPortsButton.Size = new System.Drawing.Size(100, 30);
            this.refreshPortsButton.TabIndex = 7;
            this.refreshPortsButton.Text = "Refresh Ports";
            this.refreshPortsButton.UseVisualStyleBackColor = true;
            this.refreshPortsButton.Click += new System.EventHandler(this.refreshPortsButton_Click);
            // 
            // selectAllCheckBox
            // 
            this.selectAllCheckBox.AutoSize = true;
            this.selectAllCheckBox.Location = new System.Drawing.Point(20, 170);
            this.selectAllCheckBox.Name = "selectAllCheckBox";
            this.selectAllCheckBox.Size = new System.Drawing.Size(70, 17);
            this.selectAllCheckBox.TabIndex = 8;
            this.selectAllCheckBox.Text = "Select All";
            this.selectAllCheckBox.UseVisualStyleBackColor = true;
            this.selectAllCheckBox.CheckedChanged += new System.EventHandler(this.selectAllCheckBox_CheckedChanged);
            // 
            // portsLabel
            // 
            this.portsLabel.AutoSize = true;
            this.portsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.portsLabel.Location = new System.Drawing.Point(20, 20);
            this.portsLabel.Name = "portsLabel";
            this.portsLabel.Size = new System.Drawing.Size(103, 13);
            this.portsLabel.TabIndex = 9;
            this.portsLabel.Text = "Available Ports:";
            // 
            // resultsLabel
            // 
            this.resultsLabel.AutoSize = true;
            this.resultsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.resultsLabel.Location = new System.Drawing.Point(20, 200);
            this.resultsLabel.Name = "resultsLabel";
            this.resultsLabel.Size = new System.Drawing.Size(88, 13);
            this.resultsLabel.TabIndex = 10;
            this.resultsLabel.Text = "Scan Results:";
            // 
            // MultiPortScan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.resultsLabel);
            this.Controls.Add(this.portsLabel);
            this.Controls.Add(this.selectAllCheckBox);
            this.Controls.Add(this.refreshPortsButton);
            this.Controls.Add(this.uploadCheckBox);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.resultsListView);
            this.Controls.Add(this.processButton);
            this.Controls.Add(this.scanButton);
            this.Controls.Add(this.portsListBox);
            this.Name = "MultiPortScan";
            this.Size = new System.Drawing.Size(600, 500);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox portsListBox;
        private System.Windows.Forms.Button scanButton;
        private System.Windows.Forms.Button processButton;
        private System.Windows.Forms.ListView resultsListView;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.CheckBox uploadCheckBox;
        private System.Windows.Forms.Button refreshPortsButton;
        private System.Windows.Forms.CheckBox selectAllCheckBox;
        private System.Windows.Forms.ColumnHeader portColumn;
        private System.Windows.Forms.ColumnHeader statusColumn;
        private System.Windows.Forms.ColumnHeader dsiCountColumn;
        private System.Windows.Forms.ColumnHeader detailsColumn;
        private System.Windows.Forms.Label portsLabel;
        private System.Windows.Forms.Label resultsLabel;
    }
}