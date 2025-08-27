using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RetailManagement.Database;
using RetailManagement.Models;

namespace RetailManagement.UserForms
{
    public partial class PurchaseReturnReport : Form
    {
        private DataTable returnData;

        public PurchaseReturnReport()
        {
            InitializeComponent();
            InitializeForm();
            LoadPurchaseReturnReport();
        }

        private void InitializeForm()
        {
            // Load companies
            LoadCompanies();
            
            // Initialize data table
            returnData = new DataTable();
        }

        private void LoadCompanies()
        {
            try
            {
                string query = "SELECT CompanyID, CompanyName FROM Companies WHERE IsActive = 1 ORDER BY CompanyName";
                DataTable companies = DatabaseConnection.ExecuteQuery(query);
                
                cmbCompany.Items.Clear();
                cmbCompany.Items.Add("All Companies");
                
                foreach (DataRow row in companies.Rows)
                {
                    cmbCompany.Items.Add(new ComboBoxItem 
                    { 
                        Text = row["CompanyName"].ToString(), 
                        Value = Convert.ToInt32(row["CompanyID"]) 
                    });
                }
                
                cmbCompany.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading companies: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadPurchaseReturnReport()
        {
            try
            {
                DateTime fromDate = dtpFromDate.Value.Date;
                DateTime toDate = dtpToDate.Value.Date;
                
                string query = @"SELECT 
                                    pr.ReturnID,
                                    pr.ReturnNumber,
                                    pr.ReturnDate,
                                    c.CompanyName,
                                    pr.TotalAmount,
                                    pr.ReturnReason,
                                    pr.Remarks,
                                    COUNT(pri.ItemID) as ItemCount
                               FROM PurchaseReturns pr
                               INNER JOIN Companies c ON pr.CompanyID = c.CompanyID
                               LEFT JOIN PurchaseReturnItems pri ON pr.ReturnID = pri.ReturnID
                               WHERE pr.ReturnDate BETWEEN @FromDate AND @ToDate
                               AND pr.IsActive = 1";

                // Add company filter
                if (cmbCompany.SelectedIndex > 0)
                {
                    ComboBoxItem selectedItem = (ComboBoxItem)cmbCompany.SelectedItem;
                    query += " AND pr.CompanyID = @CompanyID";
                }

                query += " GROUP BY pr.ReturnID, pr.ReturnNumber, pr.ReturnDate, c.CompanyName, pr.TotalAmount, pr.ReturnReason, pr.Remarks";
                query += " ORDER BY pr.ReturnDate DESC";

                SqlParameter[] parameters = null;
                if (cmbCompany.SelectedIndex > 0)
                {
                    ComboBoxItem selectedItem = (ComboBoxItem)cmbCompany.SelectedItem;
                    parameters = new SqlParameter[]
                    {
                        new SqlParameter("@FromDate", fromDate),
                        new SqlParameter("@ToDate", toDate),
                        new SqlParameter("@CompanyID", selectedItem.Value)
                    };
                }
                else
                {
                    parameters = new SqlParameter[]
                    {
                        new SqlParameter("@FromDate", fromDate),
                        new SqlParameter("@ToDate", toDate)
                    };
                }

                returnData = DatabaseConnection.ExecuteQuery(query, parameters);
                ShowReturnData();
                CalculateTotals();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading purchase return report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowReturnData()
        {
            SetupDataGridView();
            
            dgvPurchaseReturn.Rows.Clear();

            if (returnData != null && returnData.Rows.Count > 0)
            {
                foreach (DataRow row in returnData.Rows)
                {
                    int rowIndex = dgvPurchaseReturn.Rows.Add();
                    dgvPurchaseReturn.Rows[rowIndex].Cells["ReturnID"].Value = row["ReturnID"];
                    dgvPurchaseReturn.Rows[rowIndex].Cells["ReturnNumber"].Value = row["ReturnNumber"];
                    dgvPurchaseReturn.Rows[rowIndex].Cells["ReturnDate"].Value = Convert.ToDateTime(row["ReturnDate"]).ToString("dd/MM/yyyy");
                    dgvPurchaseReturn.Rows[rowIndex].Cells["CompanyName"].Value = row["CompanyName"];
                    dgvPurchaseReturn.Rows[rowIndex].Cells["TotalAmount"].Value = Convert.ToDecimal(row["TotalAmount"]).ToString("N2");
                    dgvPurchaseReturn.Rows[rowIndex].Cells["ReturnReason"].Value = row["ReturnReason"];
                    dgvPurchaseReturn.Rows[rowIndex].Cells["ItemCount"].Value = row["ItemCount"];
                    dgvPurchaseReturn.Rows[rowIndex].Cells["Remarks"].Value = row["Remarks"];

                    // Color coding for return reasons
                    string returnReason = row["ReturnReason"].ToString().ToLower();
                    if (returnReason.Contains("damaged") || returnReason.Contains("defective"))
                    {
                        dgvPurchaseReturn.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightCoral;
                        dgvPurchaseReturn.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.DarkRed;
                    }
                    else if (returnReason.Contains("expired"))
                    {
                        dgvPurchaseReturn.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightYellow;
                        dgvPurchaseReturn.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.DarkOrange;
                    }
                    else if (returnReason.Contains("wrong") || returnReason.Contains("incorrect"))
                    {
                        dgvPurchaseReturn.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightBlue;
                        dgvPurchaseReturn.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.DarkBlue;
                    }
                }
            }
        }

        private void SetupDataGridView()
        {
            dgvPurchaseReturn.Columns.Clear();
            
            dgvPurchaseReturn.Columns.Add("ReturnID", "Return ID");
            dgvPurchaseReturn.Columns.Add("ReturnNumber", "Return #");
            dgvPurchaseReturn.Columns.Add("ReturnDate", "Date");
            dgvPurchaseReturn.Columns.Add("CompanyName", "Company");
            dgvPurchaseReturn.Columns.Add("TotalAmount", "Total Amount");
            dgvPurchaseReturn.Columns.Add("ReturnReason", "Return Reason");
            dgvPurchaseReturn.Columns.Add("ItemCount", "Items");
            dgvPurchaseReturn.Columns.Add("Remarks", "Remarks");

            // Configure columns
            dgvPurchaseReturn.Columns["ReturnID"].Width = 80;
            dgvPurchaseReturn.Columns["ReturnNumber"].Width = 120;
            dgvPurchaseReturn.Columns["ReturnDate"].Width = 100;
            dgvPurchaseReturn.Columns["CompanyName"].Width = 150;
            dgvPurchaseReturn.Columns["TotalAmount"].Width = 120;
            dgvPurchaseReturn.Columns["ReturnReason"].Width = 150;
            dgvPurchaseReturn.Columns["ItemCount"].Width = 60;
            dgvPurchaseReturn.Columns["Remarks"].Width = 150;

            // Set alignment
            dgvPurchaseReturn.Columns["ReturnID"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPurchaseReturn.Columns["TotalAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPurchaseReturn.Columns["ItemCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Set header alignment
            dgvPurchaseReturn.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPurchaseReturn.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        }

        private void CalculateTotals()
        {
            int totalReturns = 0;
            decimal totalAmount = 0;
            int totalItems = 0;

            if (returnData != null && returnData.Rows.Count > 0)
            {
                totalReturns = returnData.Rows.Count;
                foreach (DataRow row in returnData.Rows)
                {
                    totalAmount += Convert.ToDecimal(row["TotalAmount"]);
                    totalItems += Convert.ToInt32(row["ItemCount"]);
                }
            }

            lblTotalReturns.Text = "Total Returns: " + totalReturns.ToString();
            lblTotalAmount.Text = "Total Amount: " + totalAmount.ToString("N2");
            lblTotalItems.Text = "Total Items: " + totalItems.ToString();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadPurchaseReturnReport();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (returnData.Rows.Count == 0)
            {
                MessageBox.Show("No data to print.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(PrintPurchaseReturnReport);
            
            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = pd;
            
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                pd.Print();
            }
        }

        private void PrintPurchaseReturnReport(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font titleFont = new Font("Arial", 16, FontStyle.Bold);
            Font headerFont = new Font("Arial", 10, FontStyle.Bold);
            Font dataFont = new Font("Arial", 9);
            Font summaryFont = new Font("Arial", 10, FontStyle.Bold);

            int yPos = 50;
            int leftMargin = 50;

            // Print title
            string title = "Purchase Return Report";
            if (cmbCompany.SelectedIndex > 0)
            {
                ComboBoxItem selectedItem = (ComboBoxItem)cmbCompany.SelectedItem;
                title += $" - {selectedItem.Text}";
            }
            title += $" ({dtpFromDate.Value:dd/MM/yyyy} to {dtpToDate.Value:dd/MM/yyyy})";
            g.DrawString(title, titleFont, Brushes.Black, leftMargin, yPos);
            yPos += 30;

            // Print date
            g.DrawString($"Generated on: {DateTime.Now:dd/MM/yyyy HH:mm}", dataFont, Brushes.Black, leftMargin, yPos);
            yPos += 30;

            // Print headers
            string[] headers = { "Return #", "Date", "Company", "Amount", "Reason", "Items" };
            int[] columnWidths = { 100, 80, 150, 100, 120, 60 };
            int xPos = leftMargin;

            for (int i = 0; i < headers.Length; i++)
            {
                g.DrawString(headers[i], headerFont, Brushes.Black, xPos, yPos);
                xPos += columnWidths[i];
            }
            yPos += 20;

            // Print data
            foreach (DataRow row in returnData.Rows)
            {
                if (yPos > e.MarginBounds.Bottom - 100)
                {
                    e.HasMorePages = true;
                    return;
                }

                xPos = leftMargin;
                
                // Return Number
                g.DrawString(row["ReturnNumber"].ToString(), dataFont, Brushes.Black, xPos, yPos);
                xPos += columnWidths[0];

                // Date
                g.DrawString(Convert.ToDateTime(row["ReturnDate"]).ToString("dd/MM/yyyy"), dataFont, Brushes.Black, xPos, yPos);
                xPos += columnWidths[1];

                // Company
                g.DrawString(row["CompanyName"].ToString(), dataFont, Brushes.Black, xPos, yPos);
                xPos += columnWidths[2];

                // Amount
                g.DrawString(Convert.ToDecimal(row["TotalAmount"]).ToString("N2"), dataFont, Brushes.Black, xPos, yPos);
                xPos += columnWidths[3];

                // Return Reason
                g.DrawString(row["ReturnReason"].ToString(), dataFont, Brushes.Black, xPos, yPos);
                xPos += columnWidths[4];

                // Items
                g.DrawString(row["ItemCount"].ToString(), dataFont, Brushes.Black, xPos, yPos);

                yPos += 15;
            }

            // Print summary
            yPos += 20;
            g.DrawString($"Total Returns: {returnData.Rows.Count}", summaryFont, Brushes.Black, leftMargin, yPos);
            yPos += 15;
            
            decimal totalAmount = 0;
            foreach (DataRow row in returnData.Rows)
            {
                totalAmount += Convert.ToDecimal(row["TotalAmount"]);
            }
            g.DrawString($"Total Amount: {totalAmount:N2}", summaryFont, Brushes.Black, leftMargin, yPos);
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (returnData.Rows.Count == 0)
            {
                MessageBox.Show("No data to export.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "CSV files (*.csv)|*.csv";
            saveDialog.FileName = $"PurchaseReturnReport_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                ExportToCSV(saveDialog.FileName);
            }
        }

        private void ExportToCSV(string fileName)
        {
            try
            {
                StringBuilder csv = new StringBuilder();
                
                // Add headers
                csv.AppendLine("Return Number,Date,Company,Total Amount,Return Reason,Items,Remarks");
                
                // Add data
                foreach (DataRow row in returnData.Rows)
                {
                    string returnNumber = row["ReturnNumber"].ToString().Replace(",", ";");
                    string date = Convert.ToDateTime(row["ReturnDate"]).ToString("dd/MM/yyyy");
                    string company = row["CompanyName"].ToString().Replace(",", ";");
                    string amount = Convert.ToDecimal(row["TotalAmount"]).ToString("N2");
                    string returnReason = row["ReturnReason"].ToString().Replace(",", ";");
                    string items = row["ItemCount"].ToString();
                    string remarks = row["Remarks"].ToString().Replace(",", ";");

                    csv.AppendLine($"{returnNumber},{date},{company},{amount},{returnReason},{items},{remarks}");
                }

                System.IO.File.WriteAllText(fileName, csv.ToString());
                MessageBox.Show("Data exported successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadPurchaseReturnReport();
        }

        private void dtpFromDate_ValueChanged(object sender, EventArgs e)
        {
            LoadPurchaseReturnReport();
        }

        private void dtpToDate_ValueChanged(object sender, EventArgs e)
        {
            LoadPurchaseReturnReport();
        }

        // Using shared ComboBoxItem from Models namespace
    }
} 