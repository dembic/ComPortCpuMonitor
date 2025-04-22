namespace ComPortCpuMonitor
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            labelPorts = new Label();
            comboBoxPorts = new ComboBox();
            buttonRefreshPorts = new Button();
            labelCpu = new Label();
            labelStatus = new Label();
            labelMemory = new Label();
            labelGpu = new Label();
            labelGpuMemory = new Label();
            buttonConnect = new Button();
            labelCpuTemp = new Label();
            labelGpuTemp = new Label();
            labelCpuCores = new Label();
            labelGpuClock = new Label();
            labelCpuClock = new Label();
            labelCpuFan = new Label();
            ReceiveESP = new Label();
            labelGpuFan = new Label();
            SuspendLayout();
            // 
            // labelPorts
            // 
            labelPorts.AutoSize = true;
            labelPorts.Location = new Point(12, 9);
            labelPorts.Name = "labelPorts";
            labelPorts.Size = new Size(68, 15);
            labelPorts.TabIndex = 0;
            labelPorts.Text = "COM Ports:";
            // 
            // comboBoxPorts
            // 
            comboBoxPorts.FormattingEnabled = true;
            comboBoxPorts.Location = new Point(86, 6);
            comboBoxPorts.Name = "comboBoxPorts";
            comboBoxPorts.Size = new Size(121, 23);
            comboBoxPorts.TabIndex = 1;
            // 
            // buttonRefreshPorts
            // 
            buttonRefreshPorts.Location = new Point(213, 5);
            buttonRefreshPorts.Name = "buttonRefreshPorts";
            buttonRefreshPorts.Size = new Size(75, 24);
            buttonRefreshPorts.TabIndex = 2;
            buttonRefreshPorts.Text = "Refresh";
            buttonRefreshPorts.UseVisualStyleBackColor = true;
            // 
            // labelCpu
            // 
            labelCpu.AutoSize = true;
            labelCpu.Location = new Point(86, 91);
            labelCpu.Name = "labelCpu";
            labelCpu.Size = new Size(30, 15);
            labelCpu.TabIndex = 3;
            labelCpu.Text = "CPU";
            // 
            // labelStatus
            // 
            labelStatus.AutoSize = true;
            labelStatus.Location = new Point(11, 426);
            labelStatus.Name = "labelStatus";
            labelStatus.Size = new Size(39, 15);
            labelStatus.TabIndex = 4;
            labelStatus.Text = "Status";
            // 
            // labelMemory
            // 
            labelMemory.AutoSize = true;
            labelMemory.Location = new Point(86, 125);
            labelMemory.Name = "labelMemory";
            labelMemory.Size = new Size(35, 15);
            labelMemory.TabIndex = 5;
            labelMemory.Text = "mem";
            // 
            // labelGpu
            // 
            labelGpu.AutoSize = true;
            labelGpu.Location = new Point(86, 166);
            labelGpu.Name = "labelGpu";
            labelGpu.Size = new Size(30, 15);
            labelGpu.TabIndex = 6;
            labelGpu.Text = "GPU";
            // 
            // labelGpuMemory
            // 
            labelGpuMemory.AutoSize = true;
            labelGpuMemory.Location = new Point(86, 208);
            labelGpuMemory.Name = "labelGpuMemory";
            labelGpuMemory.Size = new Size(43, 15);
            labelGpuMemory.TabIndex = 7;
            labelGpuMemory.Text = "Gmem";
            // 
            // buttonConnect
            // 
            buttonConnect.Location = new Point(294, 6);
            buttonConnect.Name = "buttonConnect";
            buttonConnect.Size = new Size(75, 23);
            buttonConnect.TabIndex = 8;
            buttonConnect.Text = "Connect";
            buttonConnect.UseVisualStyleBackColor = true;
            // 
            // labelCpuTemp
            // 
            labelCpuTemp.AutoSize = true;
            labelCpuTemp.Location = new Point(294, 91);
            labelCpuTemp.Name = "labelCpuTemp";
            labelCpuTemp.Size = new Size(60, 15);
            labelCpuTemp.TabIndex = 9;
            labelCpuTemp.Text = "CPUTemp";
            // 
            // labelGpuTemp
            // 
            labelGpuTemp.AutoSize = true;
            labelGpuTemp.Location = new Point(294, 166);
            labelGpuTemp.Name = "labelGpuTemp";
            labelGpuTemp.Size = new Size(60, 15);
            labelGpuTemp.TabIndex = 10;
            labelGpuTemp.Text = "GPUTemp";
            // 
            // labelCpuCores
            // 
            labelCpuCores.AutoSize = true;
            labelCpuCores.Location = new Point(634, 91);
            labelCpuCores.Name = "labelCpuCores";
            labelCpuCores.Size = new Size(60, 15);
            labelCpuCores.TabIndex = 11;
            labelCpuCores.Text = "CPUCores";
            // 
            // labelGpuClock
            // 
            labelGpuClock.AutoSize = true;
            labelGpuClock.Location = new Point(474, 166);
            labelGpuClock.Name = "labelGpuClock";
            labelGpuClock.Size = new Size(60, 15);
            labelGpuClock.TabIndex = 12;
            labelGpuClock.Text = "GPUClock";
            // 
            // labelCpuClock
            // 
            labelCpuClock.AutoSize = true;
            labelCpuClock.Location = new Point(474, 91);
            labelCpuClock.Name = "labelCpuClock";
            labelCpuClock.Size = new Size(60, 15);
            labelCpuClock.TabIndex = 13;
            labelCpuClock.Text = "CPUClock";
            // 
            // labelCpuFan
            // 
            labelCpuFan.AutoSize = true;
            labelCpuFan.Location = new Point(782, 91);
            labelCpuFan.Name = "labelCpuFan";
            labelCpuFan.Size = new Size(49, 15);
            labelCpuFan.TabIndex = 14;
            labelCpuFan.Text = "CPUFan";
            // 
            // ReceiveESP
            // 
            ReceiveESP.AutoSize = true;
            ReceiveESP.Location = new Point(394, 10);
            ReceiveESP.Name = "ReceiveESP";
            ReceiveESP.Size = new Size(70, 15);
            ReceiveESP.TabIndex = 15;
            ReceiveESP.Text = "ESP32Status";
            // 
            // labelGpuFan
            // 
            labelGpuFan.AutoSize = true;
            labelGpuFan.Location = new Point(782, 166);
            labelGpuFan.Name = "labelGpuFan";
            labelGpuFan.Size = new Size(49, 15);
            labelGpuFan.TabIndex = 16;
            labelGpuFan.Text = "GPUFan";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1031, 450);
            Controls.Add(labelGpuFan);
            Controls.Add(ReceiveESP);
            Controls.Add(labelCpuFan);
            Controls.Add(labelCpuClock);
            Controls.Add(labelGpuClock);
            Controls.Add(labelCpuCores);
            Controls.Add(labelGpuTemp);
            Controls.Add(labelCpuTemp);
            Controls.Add(buttonConnect);
            Controls.Add(labelGpuMemory);
            Controls.Add(labelGpu);
            Controls.Add(labelMemory);
            Controls.Add(labelStatus);
            Controls.Add(labelCpu);
            Controls.Add(buttonRefreshPorts);
            Controls.Add(comboBoxPorts);
            Controls.Add(labelPorts);
            Name = "Form1";
            Text = "COM Port & CPU Monitor";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelPorts;
        private ComboBox comboBoxPorts;
        private Button buttonRefreshPorts;
        private Label labelCpu;
        private Label labelStatus;
        private Label labelMemory;
        private Label labelGpu;
        private Label labelGpuMemory;
        private Button buttonConnect;
        private Label labelCpuTemp;
        private Label labelGpuTemp;
        private Label labelCpuCores;
        private Label labelGpuClock;
        private Label labelCpuClock;
        private Label labelCpuFan;
        private Label ReceiveESP;
        private Label labelGpuFan;
    }
}
