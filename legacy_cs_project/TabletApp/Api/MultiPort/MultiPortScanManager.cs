using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using DsiApi;
using Model;
using TabletApp.Properties;
using TabletApp.Utils;
using Logging;

namespace TabletApp.Api.MultiPort
{
    /// <summary>
    /// Manages scanning and processing of DSI devices across multiple COM ports
    /// </summary>
    public class MultiPortScanManager
    {
        public class PortScanResult
        {
            public string PortName { get; set; }
            public List<ANanoSense> DsiItems { get; set; } = new List<ANanoSense>();
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
            public ADsiNetwork DsiNetwork { get; set; }
        }

        public class MultiPortProgress
        {
            public int CurrentPortIndex { get; set; }
            public int TotalPorts { get; set; }
            public string CurrentPortName { get; set; }
            public string Status { get; set; }
            public int CurrentDsiCount { get; set; }
        }

        public event Action<MultiPortProgress> ProgressChanged;
        public event Action<PortScanResult> PortScanCompleted;

        private readonly List<string> _availablePorts = new List<string>();
        private readonly Dictionary<string, PortScanResult> _scanResults = new Dictionary<string, PortScanResult>();

        /// <summary>
        /// Get available COM ports, optionally filtered by a list
        /// </summary>
        public List<string> GetAvailablePorts(List<string> filterPorts = null)
        {
            _availablePorts.Clear();
            var systemPorts = SerialPort.GetPortNames().OrderBy(p => p).ToList();
            
            if (filterPorts != null && filterPorts.Any())
            {
                _availablePorts.AddRange(systemPorts.Where(p => filterPorts.Contains(p)));
            }
            else
            {
                _availablePorts.AddRange(systemPorts);
            }
            
            return _availablePorts;
        }

        /// <summary>
        /// Scan all specified COM ports for DSI devices
        /// </summary>
        public async Task<Dictionary<string, PortScanResult>> ScanAllPortsAsync(List<string> portsToScan = null)
        {
            var ports = portsToScan ?? GetAvailablePorts();
            _scanResults.Clear();

            for (int i = 0; i < ports.Count; i++)
            {
                var port = ports[i];
                var progress = new MultiPortProgress
                {
                    CurrentPortIndex = i,
                    TotalPorts = ports.Count,
                    CurrentPortName = port,
                    Status = $"Scanning {port}...",
                    CurrentDsiCount = 0
                };
                ProgressChanged?.Invoke(progress);

                var result = await ScanSinglePortAsync(port);
                _scanResults[port] = result;
                
                progress.Status = result.Success ? 
                    $"Found {result.DsiItems.Count} DSI(s) on {port}" : 
                    $"Failed on {port}: {result.ErrorMessage}";
                progress.CurrentDsiCount = result.DsiItems.Count;
                ProgressChanged?.Invoke(progress);
                
                PortScanCompleted?.Invoke(result);
            }

            return _scanResults;
        }

        /// <summary>
        /// Scan a single COM port for DSI devices
        /// </summary>
        private async Task<PortScanResult> ScanSinglePortAsync(string portName)
        {
            var result = new PortScanResult { PortName = portName };

            try
            {
                // Check if port is available
                if (!IsPortAvailable(portName))
                {
                    result.ErrorMessage = $"Port {portName} is not available or in use";
                    return result;
                }

                // Initialize DSI network for this specific port
                result.DsiNetwork = InitializeDsiNetworkForPort(portName);
                
                // Scan the network
                await AApiManager.Instance.ScanNetworkAsync(result.DsiNetwork, false,
                    (ADsiInfo dsiInfo, List<AProbe> probes) =>
                    {
                        if (ADsiNetwork.kFactoryId == dsiInfo.modbusAddress)
                        {
                            return;
                        }

                        var newItem = new ANanoSense
                        {
                            Dsi = dsiInfo
                        };
                        newItem.Dsi.probes = probes.ToArray();
                        
                        // Get site info
                        result.DsiNetwork.GetSiteInfo(newItem);
                        result.DsiItems.Add(newItem);
                        
                        ALog.Info("MultiPortScan", $"Found DSI at address {dsiInfo.modbusAddress} on {portName}");
                    });

                result.Success = result.DsiItems.Count > 0;
                if (!result.Success)
                {
                    result.ErrorMessage = "No DSI devices found";
                }
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
                result.Success = false;
                ALog.Error("MultiPortScan", $"Error scanning port {portName}: {ex.Message}");
            }

            return result;
        }

        /// <summary>
        /// Process all DSI devices found across all ports (read measurements and upload)
        /// </summary>
        public async Task ProcessAllDsiDevicesAsync(bool performUpload = true)
        {
            foreach (var portResult in _scanResults.Values.Where(r => r.Success))
            {
                await ProcessPortDsiDevicesAsync(portResult, performUpload);
            }
        }

        /// <summary>
        /// Process DSI devices for a specific port
        /// </summary>
        private async Task ProcessPortDsiDevicesAsync(PortScanResult portResult, bool performUpload)
        {
            try
            {
                var progress = new MultiPortProgress
                {
                    CurrentPortName = portResult.PortName,
                    Status = $"Reading measurements from {portResult.DsiItems.Count} DSI(s) on {portResult.PortName}..."
                };
                ProgressChanged?.Invoke(progress);

                // Perform measurements for all DSIs on this port
                var measurementParams = portResult.DsiItems.Select(dsi => new AMeasurementParam
                {
                    fDsiAddress = (byte)dsi.Dsi.modbusAddress,
                    fProbes = dsi.Dsi.probes
                }).ToList();

                var timestamp = DateTime.UtcNow;
                
                await AApiManager.Instance.PerformMeasurementsAsync(
                    portResult.DsiNetwork, 
                    portResult.DsiItems.First(), 
                    timestamp, 
                    null,
                    (param, probe, error) =>
                    {
                        if (error != null)
                        {
                            ALog.Error("MultiPortScan", $"Measurement error on {portResult.PortName}: {error}");
                        }
                    });

                // Save files
                progress.Status = $"Saving data from {portResult.PortName}...";
                ProgressChanged?.Invoke(progress);
                
                foreach (var dsi in portResult.DsiItems)
                {
                    string errorMessage;
                    AFileManager.Instance.Save(dsi, out _, out errorMessage);
                    
                    if (errorMessage != null)
                    {
                        ALog.Error("MultiPortScan", $"Save error on {portResult.PortName}: {errorMessage}");
                    }
                }

                // Upload if requested
                if (performUpload && portResult.DsiItems.Any(d => d.backingFilePath != null))
                {
                    progress.Status = $"Uploading data from {portResult.PortName}...";
                    ProgressChanged?.Invoke(progress);
                    
                    await AApiManager.Instance.UploadAsync(portResult.DsiItems.Where(d => d.backingFilePath != null).ToList());
                }

                progress.Status = $"Completed processing {portResult.PortName}";
                ProgressChanged?.Invoke(progress);
            }
            catch (Exception ex)
            {
                ALog.Error("MultiPortScan", $"Error processing port {portResult.PortName}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Check if a COM port is available for use
        /// </summary>
        private bool IsPortAvailable(string portName)
        {
            try
            {
                using (var port = new SerialPort(portName))
                {
                    port.Open();
                    port.Close();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Initialize DSI network for a specific COM port
        /// </summary>
        private ADsiNetwork InitializeDsiNetworkForPort(string portName)
        {
            return new ADsiNetwork(
                portName, 
                Settings.Default.ComBaudRate,
                Settings.Default.LowLevelTimeout, 
                Settings.Default.LowLevelRetryCount,
                null, 
                null, 
                null);
        }

        /// <summary>
        /// Get scan results
        /// </summary>
        public Dictionary<string, PortScanResult> GetScanResults()
        {
            return new Dictionary<string, PortScanResult>(_scanResults);
        }

        /// <summary>
        /// Clear all scan results
        /// </summary>
        public void ClearResults()
        {
            _scanResults.Clear();
        }
    }
}