using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using RetailManagement.Database;

namespace RetailManagement.Utils
{
    /// <summary>
    /// Helper class for managing dynamic dashboard status information
    /// </summary>
    public static class DashboardStatusHelper
    {
        /// <summary>
        /// Create a dynamic status panel for the dashboard
        /// </summary>
        /// <param name="parentControl">Parent control to add the status panel to</param>
        /// <returns>The created status panel</returns>
        public static Panel CreateStatusPanel(Control parentControl)
        {
            // Create main status panel
            Panel statusPanel = new Panel();
            statusPanel.Name = "statusPanel";
            statusPanel.Size = new Size(300, 170);
            statusPanel.Location = new Point(parentControl.Width - 320, 20);
            statusPanel.BackColor = Color.FromArgb(200, 255, 255, 255); // Semi-transparent white
            statusPanel.BorderStyle = BorderStyle.FixedSingle;
            
            // Status title
            Label statusTitle = new Label();
            statusTitle.Text = "System Status";
            statusTitle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            statusTitle.ForeColor = Color.DarkBlue;
            statusTitle.Location = new Point(10, 10);
            statusTitle.AutoSize = true;
            statusPanel.Controls.Add(statusTitle);
            
            // Today's date
            Label todayLabel = new Label();
            todayLabel.Name = "lblToday";
            todayLabel.Text = "Today: " + DateTime.Now.ToString("dddd, MMMM dd, yyyy");
            todayLabel.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            todayLabel.Location = new Point(10, 35);
            todayLabel.AutoSize = true;
            statusPanel.Controls.Add(todayLabel);
            
            // Current time
            Label timeLabel = new Label();
            timeLabel.Name = "lblCurrentTime";
            timeLabel.Text = "Time: " + DateTime.Now.ToString("h:mm:ss tt");
            timeLabel.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            timeLabel.Location = new Point(10, 55);
            timeLabel.AutoSize = true;
            statusPanel.Controls.Add(timeLabel);
            
            // Low stock alert count
            Label lowStockLabel = new Label();
            lowStockLabel.Name = "lblLowStock";
            lowStockLabel.Text = "Low Stock Items: Loading...";
            lowStockLabel.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            lowStockLabel.ForeColor = Color.OrangeRed;
            lowStockLabel.Location = new Point(10, 75);
            lowStockLabel.AutoSize = true;
            statusPanel.Controls.Add(lowStockLabel);
            
            // Today's sales count
            Label salesLabel = new Label();
            salesLabel.Name = "lblTodaySales";
            salesLabel.Text = "Today's Sales: Loading...";
            salesLabel.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            salesLabel.ForeColor = Color.Green;
            salesLabel.Location = new Point(10, 95);
            salesLabel.AutoSize = true;
            statusPanel.Controls.Add(salesLabel);
            
            // Last invoice number
            Label lastInvoiceLabel = new Label();
            lastInvoiceLabel.Name = "lblLastInvoice";
            lastInvoiceLabel.Text = "Last Invoice: Loading...";
            lastInvoiceLabel.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            lastInvoiceLabel.ForeColor = Color.Blue;
            lastInvoiceLabel.Location = new Point(10, 115);
            lastInvoiceLabel.AutoSize = true;
            statusPanel.Controls.Add(lastInvoiceLabel);

            // System uptime
            Label uptimeLabel = new Label();
            uptimeLabel.Name = "lblUptime";
            uptimeLabel.Text = "Session: " + DateTime.Now.ToString("h:mm tt");
            uptimeLabel.Font = new Font("Segoe UI", 8, FontStyle.Italic);
            uptimeLabel.ForeColor = Color.Gray;
            uptimeLabel.Location = new Point(10, 135);
            uptimeLabel.AutoSize = true;
            statusPanel.Controls.Add(uptimeLabel);
            
            parentControl.Controls.Add(statusPanel);
            statusPanel.BringToFront();
            
            return statusPanel;
        }
        
        /// <summary>
        /// Update dynamic status information
        /// </summary>
        /// <param name="statusPanel">The status panel to update</param>
        public static void UpdateStatusPanel(Panel statusPanel)
        {
            if (statusPanel == null) return;
            
            try
            {
                DateTime now = DateTime.Now;
                
                // Update today's date
                var todayLabel = statusPanel.Controls["lblToday"] as Label;
                if (todayLabel != null)
                {
                    todayLabel.Text = "Today: " + now.ToString("dddd, MMMM dd, yyyy");
                }
                
                // Update current time
                var timeLabel = statusPanel.Controls["lblCurrentTime"] as Label;
                if (timeLabel != null)
                {
                    timeLabel.Text = "Time: " + now.ToString("h:mm:ss tt");
                }
                
                // Update low stock count (async to avoid blocking UI)
                UpdateLowStockCount(statusPanel);
                
                // Update today's sales (async to avoid blocking UI)
                UpdateTodaySalesCount(statusPanel);
                
                // Update last invoice number (async to avoid blocking UI)
                UpdateLastInvoiceNumber(statusPanel);
                
                // Update session info
                var uptimeLabel = statusPanel.Controls["lblUptime"] as Label;
                if (uptimeLabel != null && UserSession.IsLoggedIn)
                {
                    string fullName = UserSession.FullName ?? "User";
                    uptimeLabel.Text = $"Session: {fullName} | {now.ToString("h:mm tt")}";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating status panel: {ex.Message}");
            }
        }
        
        private static void UpdateLowStockCount(Panel statusPanel)
        {
            try
            {
                // Run in background to avoid blocking UI
                System.Threading.Tasks.Task.Run(() =>
                {
                    try
                    {
                        string query = @"SELECT COUNT(*) FROM Items 
                                       WHERE IsActive = 1 AND 
                                       (StockQuantity <= ISNULL(ReorderLevel, 10) OR StockQuantity <= 10)";
                        
                        object result = DatabaseConnection.ExecuteScalar(query);
                        int lowStockCount = SafeDataHelper.SafeToInt32(result);
                        
                        // Update UI on main thread
                        if (statusPanel.InvokeRequired)
                        {
                            statusPanel.Invoke(new Action(() =>
                            {
                                var lowStockLabel = statusPanel.Controls["lblLowStock"] as Label;
                                if (lowStockLabel != null)
                                {
                                    lowStockLabel.Text = $"Low Stock Items: {lowStockCount}";
                                    lowStockLabel.ForeColor = lowStockCount > 0 ? Color.Red : Color.Green;
                                }
                            }));
                        }
                    }
                    catch
                    {
                        // Ignore database errors for status updates
                    }
                });
            }
            catch
            {
                // Ignore any errors
            }
        }
        
        private static void UpdateTodaySalesCount(Panel statusPanel)
        {
            try
            {
                // Run in background to avoid blocking UI
                System.Threading.Tasks.Task.Run(() =>
                {
                    try
                    {
                        string today = DateTime.Now.ToString("yyyy-MM-dd");
                        string query = @"SELECT COUNT(*), ISNULL(SUM(NetAmount), 0) 
                                       FROM Sales 
                                       WHERE IsActive = 1 AND CAST(SaleDate AS DATE) = @Today";
                        
                        var parameters = new[] { new System.Data.SqlClient.SqlParameter("@Today", today) };
                        DataTable result = DatabaseConnection.ExecuteQuery(query, parameters);
                        
                        if (result.Rows.Count > 0)
                        {
                            int salesCount = SafeDataHelper.SafeToInt32(result.Rows[0][0]);
                            decimal salesAmount = SafeDataHelper.SafeToDecimal(result.Rows[0][1]);
                            
                            // Update UI on main thread
                            if (statusPanel.InvokeRequired)
                            {
                                statusPanel.Invoke(new Action(() =>
                                {
                                    var salesLabel = statusPanel.Controls["lblTodaySales"] as Label;
                                    if (salesLabel != null)
                                    {
                                        salesLabel.Text = $"Today's Sales: {salesCount} ({salesAmount:C})";
                                    }
                                }));
                            }
                        }
                    }
                    catch
                    {
                        // Ignore database errors for status updates
                    }
                });
            }
            catch
            {
                // Ignore any errors
            }
        }
        
        /// <summary>
        /// Create a digital clock control
        /// </summary>
        /// <param name="parentControl">Parent control</param>
        /// <param name="location">Location for the clock</param>
        /// <returns>The clock label</returns>
        public static Label CreateDigitalClock(Control parentControl, Point location)
        {
            Label clockLabel = new Label();
            clockLabel.Name = "digitalClock";
            clockLabel.Font = new Font("Digital-7", 24, FontStyle.Bold); // Fallback to Consolas if Digital-7 not available
            clockLabel.ForeColor = Color.Lime;
            clockLabel.BackColor = Color.Black;
            clockLabel.BorderStyle = BorderStyle.Fixed3D;
            clockLabel.TextAlign = ContentAlignment.MiddleCenter;
            clockLabel.Size = new Size(200, 50);
            clockLabel.Location = location;
            clockLabel.Text = DateTime.Now.ToString("HH:mm:ss");
            
            parentControl.Controls.Add(clockLabel);
            return clockLabel;
        }
        
        /// <summary>
        /// Update digital clock display
        /// </summary>
        /// <param name="clockLabel">The clock label to update</param>
        public static void UpdateDigitalClock(Label clockLabel)
        {
            if (clockLabel != null)
            {
                clockLabel.Text = DateTime.Now.ToString("HH:mm:ss");
            }
        }
        
        private static void UpdateLastInvoiceNumber(Panel statusPanel)
        {
            try
            {
                // Run in background to avoid blocking UI
                System.Threading.Tasks.Task.Run(() =>
                {
                    try
                    {
                        // Get the last invoice number from Sales table
                        string query = @"SELECT TOP 1 BillNumber, SaleDate, NetAmount 
                                       FROM Sales 
                                       WHERE IsActive = 1 
                                       ORDER BY SaleID DESC";
                        
                        DataTable result = DatabaseConnection.ExecuteQuery(query);
                        
                        if (result.Rows.Count > 0)
                        {
                            string lastBillNumber = SafeDataHelper.SafeToString(result.Rows[0]["BillNumber"]);
                            DateTime saleDate = SafeDataHelper.SafeToDateTime(result.Rows[0]["SaleDate"]);
                            decimal amount = SafeDataHelper.SafeToDecimal(result.Rows[0]["NetAmount"]);
                            
                            // Extract numeric part from bill number for display
                            string numericPart = lastBillNumber;
                            if (lastBillNumber.StartsWith("BILL"))
                            {
                                numericPart = lastBillNumber.Substring(4);
                                // Remove leading zeros for display
                                if (int.TryParse(numericPart, out int billNum))
                                {
                                    numericPart = billNum.ToString("N0"); // Add comma formatting
                                }
                            }
                            
                            // Update UI on main thread
                            if (statusPanel.InvokeRequired)
                            {
                                statusPanel.Invoke(new Action(() =>
                                {
                                    var lastInvoiceLabel = statusPanel.Controls["lblLastInvoice"] as Label;
                                    if (lastInvoiceLabel != null)
                                    {
                                        lastInvoiceLabel.Text = $"Last Invoice: {numericPart}";
                                        string tooltipText = $"Bill: {lastBillNumber}\nDate: {saleDate:MMM dd, yyyy}\nAmount: {amount:C}";
                                        
                                        // Add tooltip to show more details
                                        if (lastInvoiceLabel.Parent != null)
                                        {
                                            var form = lastInvoiceLabel.FindForm();
                                            if (form != null)
                                            {
                                                // Check if form already has a tooltip component
                                                ToolTip tooltip = null;
                                                if (form.Tag is ToolTip existingTooltip)
                                                {
                                                    tooltip = existingTooltip;
                                                }
                                                else
                                                {
                                                    tooltip = new ToolTip();
                                                    form.Tag = tooltip; // Store tooltip reference in form's Tag
                                                }
                                                tooltip.SetToolTip(lastInvoiceLabel, tooltipText);
                                            }
                                        }
                                    }
                                }));
                            }
                        }
                        else
                        {
                            // No invoices found
                            if (statusPanel.InvokeRequired)
                            {
                                statusPanel.Invoke(new Action(() =>
                                {
                                    var lastInvoiceLabel = statusPanel.Controls["lblLastInvoice"] as Label;
                                    if (lastInvoiceLabel != null)
                                    {
                                        lastInvoiceLabel.Text = "Last Invoice: None";
                                    }
                                }));
                            }
                        }
                    }
                    catch
                    {
                        // Ignore database errors for status updates
                        if (statusPanel.InvokeRequired)
                        {
                            statusPanel.Invoke(new Action(() =>
                            {
                                var lastInvoiceLabel = statusPanel.Controls["lblLastInvoice"] as Label;
                                if (lastInvoiceLabel != null)
                                {
                                    lastInvoiceLabel.Text = "Last Invoice: Error";
                                }
                            }));
                        }
                    }
                });
            }
            catch
            {
                // Ignore any errors
            }
        }
    }
}
