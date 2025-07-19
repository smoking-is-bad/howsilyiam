using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabletApp.Api.MultiPort;
using TabletApp.Properties;
using TabletApp.State;
using TabletApp.Utils;

namespace TabletApp.Content.MultiPort
{
    public partial class MultiPortScan : BaseContent
    {
        private MultiPortScanManager _scanManager;
        private bool _isScanning = false;
        private Timer _portRefreshTimer = new Timer();

        public MultiPortScan(Dictionary<string, string> parameters) : base(parameters)
        {
            InitializeComponent();
            
            _scanManager = new MultiPortScanManager();
            _scanManager.ProgressChanged += OnProgressChanged;
            _scanManager.PortScanCompleted += OnPortScanCompleted;
            
            LoadAvailablePorts();
            StartPortRefreshTimer();
            
            AStateController.Instance.StateWillChangeEvent += HandleStateWillChangeEvent;
        }

        public override void WillDisappear()
        {
            StopPortRefreshTimer();
            AStateController.Instance.StateWillChangeEvent -= HandleStateWillChangeEvent;
            _scanManager.ProgressChanged -= OnProgressChanged;
            _scanManager.PortScanCompleted -= OnPortScanCompleted;
        }

        private void StartPortRefreshTimer()
        {
            _portRefreshTimer.Interval = 2000; // Refresh every 2 seconds
            _portRefreshTimer.Tick += PortRefreshTimer_Tick;
            _portRefreshTimer.Start();
        }

        private void StopPortRefreshTimer()
        {
            _portRefreshTimer.Stop();
            _portRefreshTimer.Tick -= PortRefreshTimer_Tick;
        }

        private void PortRefreshTimer_Tick(object sender, EventArgs e)
        {
            var currentPorts = System.IO.Ports.SerialPort.GetPortNames().OrderBy(p => p).ToList();
            var displayedPorts = portsListBox.Items.Cast<string>().ToList();
            
            if (!currentPorts.SequenceEqual(displayedPorts))
            {
                LoadAvailablePorts();
            }
        }

        private void LoadAvailablePorts()
        {
            var selectedPorts = GetSelectedPorts();
            var ports = _scanManager.GetAvailablePorts();
            
            this.portsListBox.Items.Clear();
            
            foreach (var port in ports)
            {
                int index = this.portsListBox.Items.Add(port);
                // Restore previous selection state
                if (selectedPorts.Contains(port))
                {
                    this.portsListBox.SetItemChecked(index, true);
                }
                else if (port == Settings.Default.ComPort)
                {
                    // Default to checking the configured COM port
                    this.portsListBox.SetItemChecked(index, true);
                }
            }
            
            this.statusLabel.Text = $"Found {ports.Count} COM ports";
        }

        private async void scanButton_Click(object sender, EventArgs e)
        {
            if (_isScanning)
            {
                AOutput.DisplayMessage("Scan already in progress");
                return;
            }

            var selectedPorts = GetSelectedPorts();
            if (!selectedPorts.Any())
            {
                AOutput.DisplayMessage("Please select at least one COM port");
                return;
            }

            _isScanning = true;
            this.scanButton.Enabled = false;
            this.processButton.Enabled = false;
            this.resultsListView.Items.Clear();
            this.progressBar.Visible = true;
            this.progressBar.Value = 0;

            try
            {
                await _scanManager.ScanAllPortsAsync(selectedPorts);
                
                var results = _scanManager.GetScanResults();
                var totalDsis = results.Values.Sum(r => r.DsiItems.Count);
                
                this.statusLabel.Text = $"Scan complete. Found {totalDsis} DSI(s) across {results.Count} port(s)";
                this.processButton.Enabled = totalDsis > 0;
            }
            catch (Exception ex)
            {
                AOutput.DisplayError($"Scan error: {ex.Message}");
            }
            finally
            {
                _isScanning = false;
                this.scanButton.Enabled = true;
                this.progressBar.Visible = false;
            }
        }

        private async void processButton_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes != AOutput.DisplayYesNo(
                "This will read measurements from all found DSIs and upload them. Continue?",
                "Process All DSIs"))
            {
                return;
            }

            this.processButton.Enabled = false;
            this.progressBar.Visible = true;
            this.progressBar.Value = 0;

            try
            {
                bool performUpload = this.uploadCheckBox.Checked;
                await _scanManager.ProcessAllDsiDevicesAsync(performUpload);
                
                AOutput.DisplayMessage("Processing complete!");
            }
            catch (Exception ex)
            {
                AOutput.DisplayError($"Processing error: {ex.Message}");
            }
            finally
            {
                this.processButton.Enabled = true;
                this.progressBar.Visible = false;
            }
        }

        private void OnProgressChanged(MultiPortScanManager.MultiPortProgress progress)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnProgressChanged(progress)));
                return;
            }

            this.statusLabel.Text = progress.Status;
            
            if (progress.TotalPorts > 0)
            {
                int percentage = (int)((progress.CurrentPortIndex + 1) * 100.0 / progress.TotalPorts);
                this.progressBar.Value = Math.Min(percentage, 100);
            }
        }

        private void OnPortScanCompleted(MultiPortScanManager.PortScanResult result)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnPortScanCompleted(result)));
                return;
            }

            var item = new ListViewItem(result.PortName);
            item.SubItems.Add(result.Success ? "Success" : "Failed");
            item.SubItems.Add(result.DsiItems.Count.ToString());
            
            if (result.Success)
            {
                var addresses = string.Join(", ", result.DsiItems.Select(d => d.Dsi.modbusAddress.ToString()));
                item.SubItems.Add($"Addresses: {addresses}");
                item.ForeColor = Color.Green;
            }
            else
            {
                item.SubItems.Add(result.ErrorMessage);
                item.ForeColor = Color.Red;
            }
            
            this.resultsListView.Items.Add(item);
        }

        private List<string> GetSelectedPorts()
        {
            var selected = new List<string>();
            
            for (int i = 0; i < this.portsListBox.Items.Count; i++)
            {
                if (this.portsListBox.GetItemChecked(i))
                {
                    selected.Add(this.portsListBox.Items[i].ToString());
                }
            }
            
            return selected;
        }

        private void HandleStateWillChangeEvent(AState newState, string actionName, CancelEventArgs args)
        {
            if (_isScanning)
            {
                if (DialogResult.No == AOutput.DisplayYesNo(
                    "Scan is in progress. Are you sure you want to cancel?",
                    "Cancel Scan"))
                {
                    args.Cancel = true;
                }
            }
        }

        private void refreshPortsButton_Click(object sender, EventArgs e)
        {
            LoadAvailablePorts();
        }

        private void selectAllCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < this.portsListBox.Items.Count; i++)
            {
                this.portsListBox.SetItemChecked(i, this.selectAllCheckBox.Checked);
            }
        }
    }
}