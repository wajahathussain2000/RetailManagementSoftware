using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO.Ports;

namespace RetailManagement.UserForms
{
    public partial class BarcodeScannerDialog : Form
    {
        public string ScannedBarcode { get; private set; }
        private SerialPort scannerPort;
        private System.Windows.Forms.Timer timeoutTimer;
        private int timeoutSeconds = 30;

        private TextBox txtManualBarcode;
        private TextBox txtScannedBarcode;
        private Button btnOK;
        private Button btnCancel;
        private Button btnManualEntry;
        private Label lblStatus;
        private Label lblInstructions;
        private PictureBox picBarcode;
        private ProgressBar progressTimeout;

        public BarcodeScannerDialog()
        {
            InitializeComponent();
            InitializeScanner();
            SetupTimer();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form
            this.Text = "Barcode Scanner";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            // Instructions
            this.lblInstructions = new Label();
            this.lblInstructions.Text = "🔍 Position barcode in front of scanner and trigger scan";
            this.lblInstructions.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            this.lblInstructions.ForeColor = Color.FromArgb(0, 122, 204);
            this.lblInstructions.Location = new Point(20, 20);
            this.lblInstructions.Size = new Size(450, 30);
            this.lblInstructions.TextAlign = ContentAlignment.MiddleCenter;

            // Barcode Icon
            this.picBarcode = new PictureBox();
            this.picBarcode.Location = new Point(200, 60);
            this.picBarcode.Size = new Size(80, 80);
            this.picBarcode.BackColor = Color.FromArgb(240, 240, 240);
            this.picBarcode.BorderStyle = BorderStyle.FixedSingle;
            // Create a simple barcode representation
            this.picBarcode.Paint += PicBarcode_Paint;

            // Status
            this.lblStatus = new Label();
            this.lblStatus.Text = "⏳ Waiting for barcode scan...";
            this.lblStatus.Font = new Font("Segoe UI", 10);
            this.lblStatus.ForeColor = Color.Orange;
            this.lblStatus.Location = new Point(20, 150);
            this.lblStatus.Size = new Size(450, 25);
            this.lblStatus.TextAlign = ContentAlignment.MiddleCenter;

            // Timeout Progress
            this.progressTimeout = new ProgressBar();
            this.progressTimeout.Location = new Point(50, 180);
            this.progressTimeout.Size = new Size(380, 20);
            this.progressTimeout.Maximum = timeoutSeconds;
            this.progressTimeout.Value = timeoutSeconds;
            this.progressTimeout.Style = ProgressBarStyle.Continuous;

            // Scanned Barcode Display
            Label lblScanned = new Label();
            lblScanned.Text = "Scanned Barcode:";
            lblScanned.Location = new Point(20, 220);
            lblScanned.Size = new Size(120, 20);

            this.txtScannedBarcode = new TextBox();
            this.txtScannedBarcode.Location = new Point(150, 220);
            this.txtScannedBarcode.Size = new Size(200, 25);
            this.txtScannedBarcode.ReadOnly = true;
            this.txtScannedBarcode.BackColor = Color.LightGray;
            this.txtScannedBarcode.Font = new Font("Consolas", 10, FontStyle.Bold);

            // Manual Entry Section
            Label lblManual = new Label();
            lblManual.Text = "Manual Entry:";
            lblManual.Location = new Point(20, 260);
            lblManual.Size = new Size(120, 20);

            this.txtManualBarcode = new TextBox();
            this.txtManualBarcode.Location = new Point(150, 260);
            this.txtManualBarcode.Size = new Size(200, 25);
            this.txtManualBarcode.Font = new Font("Consolas", 10);
            this.txtManualBarcode.KeyDown += TxtManualBarcode_KeyDown;

            this.btnManualEntry = new Button();
            this.btnManualEntry.Text = "✏️ Use Manual";
            this.btnManualEntry.Location = new Point(360, 258);
            this.btnManualEntry.Size = new Size(100, 28);
            this.btnManualEntry.BackColor = Color.FromArgb(255, 193, 7);
            this.btnManualEntry.ForeColor = Color.Black;
            this.btnManualEntry.FlatStyle = FlatStyle.Flat;
            this.btnManualEntry.Click += BtnManualEntry_Click;

            // Action Buttons
            this.btnOK = new Button();
            this.btnOK.Text = "✅ OK";
            this.btnOK.Location = new Point(270, 320);
            this.btnOK.Size = new Size(100, 35);
            this.btnOK.BackColor = Color.FromArgb(40, 167, 69);
            this.btnOK.ForeColor = Color.White;
            this.btnOK.FlatStyle = FlatStyle.Flat;
            this.btnOK.Enabled = false;
            this.btnOK.Click += BtnOK_Click;

            this.btnCancel = new Button();
            this.btnCancel.Text = "❌ Cancel";
            this.btnCancel.Location = new Point(380, 320);
            this.btnCancel.Size = new Size(100, 35);
            this.btnCancel.BackColor = Color.FromArgb(220, 53, 69);
            this.btnCancel.ForeColor = Color.White;
            this.btnCancel.FlatStyle = FlatStyle.Flat;
            this.btnCancel.Click += BtnCancel_Click;

            // Add controls to form
            this.Controls.Add(this.lblInstructions);
            this.Controls.Add(this.picBarcode);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.progressTimeout);
            this.Controls.Add(lblScanned);
            this.Controls.Add(this.txtScannedBarcode);
            this.Controls.Add(lblManual);
            this.Controls.Add(this.txtManualBarcode);
            this.Controls.Add(this.btnManualEntry);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);

            this.ResumeLayout(false);
        }

        private void PicBarcode_Paint(object sender, PaintEventArgs e)
        {
            // Draw a simple barcode representation
            Graphics g = e.Graphics;
            Brush blackBrush = Brushes.Black;
            
            // Draw vertical lines to simulate barcode
            for (int i = 0; i < 60; i += 3)
            {
                int width = (i % 2 == 0) ? 2 : 1;
                g.FillRectangle(blackBrush, i + 10, 10, width, 60);
            }
            
            // Draw barcode text
            g.DrawString("| | | | | | | |", new Font("Arial", 8), blackBrush, 5, 75);
        }

        private void InitializeScanner()
        {
            try
            {
                // Try to detect and connect to barcode scanner via serial port
                string[] ports = SerialPort.GetPortNames();
                
                foreach (string port in ports)
                {
                    try
                    {
                        scannerPort = new SerialPort(port, 9600, Parity.None, 8, StopBits.One);
                        scannerPort.DataReceived += ScannerPort_DataReceived;
                        scannerPort.ReadTimeout = 1000;
                        scannerPort.WriteTimeout = 1000;
                        
                        scannerPort.Open();
                        
                        // Test if it's a barcode scanner by sending a test command
                        // Most scanners respond to basic commands
                        break;
                    }
                    catch
                    {
                        // If this port doesn't work, try the next one
                        if (scannerPort != null && scannerPort.IsOpen)
                        {
                            scannerPort.Close();
                        }
                        scannerPort = null;
                    }
                }
                
                if (scannerPort == null || !scannerPort.IsOpen)
                {
                    lblStatus.Text = "⚠️ No barcode scanner detected - Manual entry available";
                    lblStatus.ForeColor = Color.Orange;
                }
                else
                {
                    lblStatus.Text = "📡 Scanner connected - Ready to scan";
                    lblStatus.ForeColor = Color.Green;
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "❌ Scanner connection error - Manual entry available";
                lblStatus.ForeColor = Color.Red;
            }
        }

        private void ScannerPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = scannerPort.ReadLine().Trim();
                
                if (!string.IsNullOrEmpty(data))
                {
                    // Invoke on UI thread
                    this.Invoke(new Action(() =>
                    {
                        OnBarcodeScanned(data);
                    }));
                }
            }
            catch (Exception ex)
            {
                // Handle scanner communication errors
                this.Invoke(new Action(() =>
                {
                    lblStatus.Text = "❌ Scanner read error - Try manual entry";
                    lblStatus.ForeColor = Color.Red;
                }));
            }
        }

        private void OnBarcodeScanned(string barcode)
        {
            try
            {
                ScannedBarcode = barcode;
                txtScannedBarcode.Text = barcode;
                
                lblStatus.Text = "✅ Barcode scanned successfully!";
                lblStatus.ForeColor = Color.Green;
                
                btnOK.Enabled = true;
                
                // Auto-close after successful scan (optional)
                timeoutTimer?.Stop();
                
                // Play success sound
                System.Media.SystemSounds.Beep.Play();
                
                // Highlight the scanned barcode
                txtScannedBarcode.BackColor = Color.LightGreen;
                
                // Auto-accept after 2 seconds (optional)
                System.Windows.Forms.Timer autoAcceptTimer = new System.Windows.Forms.Timer();
                autoAcceptTimer.Interval = 2000;
                autoAcceptTimer.Tick += (s, e) =>
                {
                    autoAcceptTimer.Stop();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                };
                autoAcceptTimer.Start();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "❌ Error processing scanned barcode";
                lblStatus.ForeColor = Color.Red;
            }
        }

        private void SetupTimer()
        {
            timeoutTimer = new System.Windows.Forms.Timer();
            timeoutTimer.Interval = 1000; // 1 second
            timeoutTimer.Tick += TimeoutTimer_Tick;
            timeoutTimer.Start();
        }

        private void TimeoutTimer_Tick(object sender, EventArgs e)
        {
            progressTimeout.Value--;
            
            if (progressTimeout.Value <= 0)
            {
                timeoutTimer.Stop();
                lblStatus.Text = "⏰ Scan timeout - Use manual entry or try again";
                lblStatus.ForeColor = Color.Red;
                txtManualBarcode.Focus();
            }
        }

        private void TxtManualBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BtnManualEntry_Click(sender, e);
                e.Handled = true;
            }
        }

        private void BtnManualEntry_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtManualBarcode.Text))
            {
                ScannedBarcode = txtManualBarcode.Text.Trim();
                txtScannedBarcode.Text = ScannedBarcode;
                txtScannedBarcode.BackColor = Color.LightBlue;
                
                lblStatus.Text = "✏️ Manual barcode entry accepted";
                lblStatus.ForeColor = Color.Blue;
                
                btnOK.Enabled = true;
                timeoutTimer?.Stop();
            }
            else
            {
                MessageBox.Show("Please enter a barcode.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtManualBarcode.Focus();
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(ScannedBarcode))
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("No barcode has been scanned or entered.", "No Barcode", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                timeoutTimer?.Stop();
                timeoutTimer?.Dispose();
                
                if (scannerPort != null && scannerPort.IsOpen)
                {
                    scannerPort.DataReceived -= ScannerPort_DataReceived;
                    scannerPort.Close();
                    scannerPort.Dispose();
                }
            }
            catch
            {
                // Ignore cleanup errors
            }
            
            base.OnFormClosing(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            // Focus on manual entry field initially
            txtManualBarcode.Focus();
            
            // Show helpful tip
            ToolTip tip = new ToolTip();
            tip.SetToolTip(txtManualBarcode, "Type barcode manually or use scanner");
            tip.SetToolTip(picBarcode, "Point scanner at barcode and trigger");
        }
    }
}
