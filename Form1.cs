using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Windows.Forms;
using NvAPIWrapper;
using NvAPIWrapper.GPU;
using LibreHardwareMonitor.Hardware;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq; // Добавляем LINQ

namespace ComPortCpuMonitor
{
    public partial class Form1 : Form
    {
        private PerformanceCounter? cpuCounter;
        private List<PerformanceCounter>? cpuCoreCounters;
        private PerformanceCounter? memoryCounter;
        private SerialPort? serialPort;
        private System.Windows.Forms.Timer updateTimer = null!;
        private Computer computer;
        private bool isDisconnecting = false;

        public Form1()
        {
            InitializeComponent();
            buttonConnect.Click += new EventHandler(ButtonConnect_Click);
            buttonRefreshPorts.Click += new EventHandler(ButtonRefreshPorts_Click);
            computer = new Computer
            {
                IsCpuEnabled = true,
                IsMotherboardEnabled = true,
                IsGpuEnabled = true
            };
            try
            {
                computer.Open();
            }
            catch (Exception ex)
            {
                labelStatus.Text = $"Status: Hardware monitor init error - {ex.Message}";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    NvAPIWrapper.NVIDIA.Initialize();
                }
                catch (Exception ex)
                {
                    labelStatus.Text = $"Status: NVAPI init error - {ex.Message}";
                }

                cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

                cpuCoreCounters = new List<PerformanceCounter>();
                var category = new PerformanceCounterCategory("Processor");
                var instanceNames = category.GetInstanceNames();
                foreach (var instance in instanceNames)
                {
                    if (instance != "_Total")
                    {
                        cpuCoreCounters.Add(new PerformanceCounter("Processor", "% Processor Time", instance));
                    }
                }

                memoryCounter = new PerformanceCounter("Memory", "Available MBytes");

                updateTimer = new System.Windows.Forms.Timer
                {
                    Interval = 1000
                };
                updateTimer.Tick += UpdateTimer_Tick;
                updateTimer.Start();

                RefreshPorts();
            }
            catch (Exception ex)
            {
                labelStatus.Text = $"Status: Init error - {ex.Message}";
            }
        }

        private void RefreshPorts()
        {
            comboBoxPorts.Items.Clear();
            try
            {
                string[] ports = SerialPort.GetPortNames();
                if (ports.Length > 0)
                {
                    comboBoxPorts.Items.AddRange(ports);
                    comboBoxPorts.SelectedIndex = 0;
                    labelStatus.Text = $"Status: Found {ports.Length} port(s): {string.Join(", ", ports)}";
                }
                else
                {
                    comboBoxPorts.Items.Add("No ports available");
                    comboBoxPorts.SelectedIndex = 0;
                    labelStatus.Text = "Status: No ports found";
                }
            }
            catch (Exception ex)
            {
                labelStatus.Text = $"Status: Port error - {ex.Message}";
                comboBoxPorts.Items.Add("Error");
                comboBoxPorts.SelectedIndex = 0;
            }
        }

        private void ButtonRefreshPorts_Click(object sender, EventArgs e)
        {
            RefreshPorts();
        }

        private async void ButtonConnect_Click(object sender, EventArgs e)
        {
            if (isDisconnecting) return;

            if (serialPort == null || !serialPort.IsOpen)
            {
                labelStatus.Text = "Status: Connecting...";
                labelStatus.Refresh();
                try
                {
                    if (comboBoxPorts.SelectedItem == null || comboBoxPorts.SelectedItem.ToString() == "No ports available")
                    {
                        labelStatus.Text = "Status: Select a valid port";
                        labelStatus.Refresh();
                        return;
                    }

                    serialPort = new SerialPort(comboBoxPorts.SelectedItem.ToString(), 115200)
                    {
                        DtrEnable = false,
                        RtsEnable = false,
                        ReadTimeout = 1000
                    };
                    serialPort.DataReceived += SerialPort_DataReceived;
                    serialPort.Open();

                    await Task.Delay(500);
                    labelStatus.Text = $"Status: Connected to {serialPort.PortName}";
                    buttonConnect.Text = "Disconnect";
                    comboBoxPorts.Enabled = false;
                    buttonRefreshPorts.Enabled = false;
                }
                catch (Exception ex)
                {
                    labelStatus.Text = $"Status: Connection error - {ex.Message}";
                    labelStatus.Refresh();
                    SafeCloseSerialPort();
                }
            }
            else
            {
                isDisconnecting = true;
                labelStatus.Text = "Status: Disconnecting...";
                labelStatus.Refresh();
                try
                {
                    await Task.Run(() =>
                    {
                        SafeCloseSerialPort();
                    });
                    labelStatus.Text = "Status: Disconnected";
                    buttonConnect.Text = "Connect";
                    comboBoxPorts.Enabled = true;
                    buttonRefreshPorts.Enabled = true;
                }
                catch (Exception ex)
                {
                    labelStatus.Text = $"Status: Disconnection error - {ex.Message}";
                    labelStatus.Refresh();
                }
                finally
                {
                    isDisconnecting = false;
                }
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = serialPort.ReadLine();
                this.Invoke((MethodInvoker)delegate
                {
                    ReceiveESP.Text = $"Status: Received from ESP32: {data}";
                });
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    ReceiveESP.Text = $"Status: Read error - {ex.Message}";
                });
            }
        }

        private void SafeCloseSerialPort()
        {
            try
            {
                if (serialPort != null)
                {
                    if (serialPort.IsOpen)
                    {
                        serialPort.Close();
                    }
                    serialPort.Dispose();
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText("log.txt", $"SafeCloseSerialPort error: {ex.Message} at {DateTime.Now}\n");
            }
            finally
            {
                serialPort = null;
            }
        }

        private async Task<bool> TryReconnectSerialPort()
        {
            try
            {
                if (serialPort != null && !serialPort.IsOpen)
                {
                    SafeCloseSerialPort();
                    serialPort = new SerialPort(comboBoxPorts.SelectedItem.ToString(), 115200)
                    {
                        DtrEnable = false,
                        RtsEnable = false,
                        ReadTimeout = 1000
                    };
                    serialPort.DataReceived += SerialPort_DataReceived;
                    serialPort.Open();
                    labelStatus.Text = $"Status: Reconnected to {serialPort.PortName}";
                    return true;
                }
            }
            catch (Exception ex)
            {
                labelStatus.Text = $"Status: Reconnection error - {ex.Message}";
            }
            return false;
        }

        private async void UpdateTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                float cpuUsage = 0, cpuTemp = 0, cpuClock = 0, cpuFan = 0, cpuCores = 0, freeMemory = 0;
                float gpuUsage = 0, gpuTemp = 0, gpuClock = 0, gpuFan = 0;
                long freeMemoryGpu = 0;

                await Task.Run(() =>
                {
                    if (cpuCounter != null)
                    {
                        cpuUsage = cpuCounter.NextValue();
                    }

                    foreach (var hardware in computer.Hardware)
                    {
                        if (hardware.HardwareType == HardwareType.Cpu)
                        {
                            hardware.Update();
                            foreach (var sensor in hardware.Sensors)
                            {
                                if (sensor.SensorType == SensorType.Temperature)
                                {
                                    if (sensor.Name.Contains("Tctl") || sensor.Name.Contains("Tdie") || sensor.Name.Contains("CPU"))
                                    {
                                        cpuTemp = sensor.Value ?? 0;
                                        break;
                                    }
                                }
                            }
                            if (cpuTemp == 0)
                            {
                                foreach (var sensor in hardware.Sensors)
                                {
                                    if (sensor.SensorType == SensorType.Temperature)
                                    {
                                        cpuTemp = sensor.Value ?? 0;
                                        break;
                                    }
                                }
                            }
                            break;
                        }
                    }

                    foreach (var hardware in computer.Hardware)
                    {
                        if (hardware.HardwareType == HardwareType.Cpu)
                        {
                            hardware.Update();
                            float totalClock = 0;
                            int coreCount = 0;
                            foreach (var sensor in hardware.Sensors)
                            {
                                if (sensor.SensorType == SensorType.Clock)
                                {
                                    if (sensor.Name.Contains("Core") || sensor.Name.Contains("Clock"))
                                    {
                                        totalClock += sensor.Value ?? 0;
                                        coreCount++;
                                    }
                                }
                            }
                            if (coreCount > 0)
                            {
                                cpuClock = totalClock / coreCount / 1000;
                            }
                            else
                            {
                                using (var freqCounter = new PerformanceCounter("Processor Information", "Processor Frequency", "_Total"))
                                {
                                    cpuClock = freqCounter.NextValue() / 1000.0f;
                                }
                            }
                            break;
                        }
                    }

                    foreach (var hardware in computer.Hardware)
                    {
                        if (hardware.HardwareType == HardwareType.Motherboard)
                        {
                            hardware.Update();
                            foreach (var subHardware in hardware.SubHardware)
                            {
                                subHardware.Update();
                                foreach (var sensor in subHardware.Sensors)
                                {
                                    if (sensor.SensorType == SensorType.Fan && sensor.Name.Contains("CPU"))
                                    {
                                        cpuFan = sensor.Value ?? 0;
                                        break;
                                    }
                                }
                                if (cpuFan > 0) break;
                            }
                            if (cpuFan == 0)
                            {
                                foreach (var sensor in hardware.Sensors)
                                {
                                    if (sensor.SensorType == SensorType.Fan && sensor.Name.Contains("CPU"))
                                    {
                                        cpuFan = sensor.Value ?? 0;
                                        break;
                                    }
                                }
                            }
                            break;
                        }
                    }

                    if (cpuCoreCounters != null && cpuCoreCounters.Count > 0)
                    {
                        float totalUsage = 0;
                        int coreCount = 0;
                        foreach (var counter in cpuCoreCounters)
                        {
                            totalUsage += counter.NextValue();
                            coreCount++;
                        }
                        cpuCores = totalUsage / coreCount;
                    }

                    if (memoryCounter != null)
                    {
                        freeMemory = memoryCounter.NextValue();
                    }

                    var gpus = PhysicalGPU.GetPhysicalGPUs();
                    if (gpus.Length > 0)
                    {
                        var gpu = gpus[0];
                        gpuUsage = gpu.UsageInformation.GPU.Percentage;
                        freeMemoryGpu = gpu.MemoryInformation.CurrentAvailableDedicatedVideoMemoryInkB / 1024;
                        gpuTemp = gpu.ThermalInformation.ThermalSensors.ToArray()[0].CurrentTemperature;
                        gpuClock = gpu.CurrentClockFrequencies.GraphicsClock.Frequency / 1000000.0f;

                        // Используем LINQ для работы с IEnumerable<GPUCooler>
                        //var coolers = gpu.CoolerInformation.Coolers;
                        //if (coolers.Any()) // Проверяем, есть ли элементы
                        //{
                        //    var firstCooler = coolers.First(); // Получаем первый элемент
                        //    gpuFan = firstCooler.CurrentLevel; // В процентах (0-100)
                        //}
                    }
                    // Получаем GPU Fan через LibreHardwareMonitor
                    foreach (var hardware in computer.Hardware)
                    {
                        if (hardware.HardwareType == HardwareType.GpuNvidia || hardware.HardwareType == HardwareType.GpuAmd)
                        {
                            hardware.Update();
                            foreach (var sensor in hardware.Sensors)
                            {
                                if (sensor.SensorType == SensorType.Fan)
                                {
                                    gpuFan = sensor.Value ?? 0; // RPM
                                    break;
                                }
                            }
                            break;
                        }
                    }
                });

                labelCpu.Text = cpuUsage > 0 ? $"CPU Usage: {cpuUsage:F1}%" : "CPU Usage: Error";
                labelCpuTemp.Text = cpuTemp > 0 ? $"CPU Temp: {cpuTemp:F1}°C" : "CPU Temp: Not detected";
                labelCpuClock.Text = cpuClock > 0 ? $"CPU Clock: {cpuClock:F2} GHz" : "CPU Clock: Not detected";
                labelCpuFan.Text = cpuFan > 0 ? $"CPU Fan: {cpuFan:F0} RPM" : "CPU Fan: Not detected";
                labelCpuCores.Text = cpuCores > 0 ? $"CPU Cores: {cpuCores:F1}%" : "CPU Cores: Not detected";
                labelMemory.Text = freeMemory > 0 ? $"Free Memory: {freeMemory:F0} MB" : "Free Memory: Error";
                labelGpu.Text = gpuUsage > 0 ? $"GPU Usage: {gpuUsage:F1}%" : "GPU Usage: No GPU found";
                labelGpuMemory.Text = freeMemoryGpu > 0 ? $"Free GPU Memory: {freeMemoryGpu:F0} MB" : "Free GPU Memory: No GPU found";
                labelGpuTemp.Text = gpuTemp > 0 ? $"GPU Temp: {gpuTemp:F1}°C" : "GPU Temp: No GPU found";
                labelGpuClock.Text = gpuClock > 0 ? $"GPU Clock: {gpuClock:F2} GHz" : "GPU Clock: Not detected";
                //labelGpuFan.Text = gpuFan > 0 ? $"GPU Fan: {gpuFan:F0}%" : "GPU Fan: Not detected";
                labelGpuFan.Text = gpuFan > 0 ? $"GPU Fan: {gpuFan:F0} RPM" : "GPU Fan: Not detected";

                if (serialPort != null)
                {
                    File.AppendAllText("log.txt", $"Serial send started at {DateTime.Now}\n");
                    try
                    {
                        if (!serialPort.IsOpen)
                        {
                            if (!await TryReconnectSerialPort())
                            {
                                return;
                            }
                        }

                        string data = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                            "CPU:{0:F1};MEM:{1:F0};GPU:{2:F1};GPUMEM:{3:F0};CPUTEMP:{4:F1};GPUTEMP:{5:F1};CPUCLOCK:{6:F2};CPUFAN:{7:F0};CPUCORES:{8:F1};GPUCLOCK:{9:F2};GPUFAN:{10:F0}\n",
                            cpuUsage, freeMemory, gpuUsage, freeMemoryGpu, cpuTemp, gpuTemp, cpuClock, cpuFan, cpuCores, gpuClock, gpuFan);
                        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(data);
                        await serialPort.BaseStream.WriteAsync(buffer, 0, buffer.Length);
                        await serialPort.BaseStream.FlushAsync();
                        labelStatus.Text = $"Status: Sent {data}";
                    }
                    catch (Exception ex)
                    {
                        labelStatus.Text = $"Status: Serial error - {ex.Message}";
                        await TryReconnectSerialPort();
                    }
                }
            }
            catch (Exception ex)
            {
                labelStatus.Text = $"Status: Update error - {ex.Message}";
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            updateTimer?.Stop();
            cpuCounter?.Dispose();
            if (cpuCoreCounters != null)
            {
                foreach (var counter in cpuCoreCounters)
                {
                    counter.Dispose();
                }
            }
            memoryCounter?.Dispose();
            SafeCloseSerialPort();
            try
            {
                computer.Close();
            }
            catch { }
            try
            {
                NvAPIWrapper.NVIDIA.Unload();
            }
            catch { }
        }
    }
}