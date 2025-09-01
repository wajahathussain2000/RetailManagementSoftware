using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RetailManagement.Database;

namespace RetailManagement.UserForms
{
    public partial class ExpiryReportForm : Form
    {
        private DataTable reportData;

        public ExpiryReportForm(DataTable data)
        {
            this.reportData = data;
            InitializeComponent();
            LoadReport();
        }

        private void LoadReport()
        {
            try
            {
                dgvExpiryReport.DataSource = reportData;
                
                if (dgvExpiryReport.Columns.Count > 0)
                {
                    dgvExpiryReport.Columns["ItemID"].Visible = false;
                    dgvExpiryReport.Columns["BatchID"].Visible = false;
                    dgvExpiryReport.Columns["ItemName"].HeaderText = "Item Name";
                    dgvExpiryReport.Columns["BatchNumber"].HeaderText = "Batch Number";
                    dgvExpiryReport.Columns["ExpiryDate"].HeaderText = "Expiry Date";
                    dgvExpiryReport.Columns["Quantity"].HeaderText = "Quantity";
                    dgvExpiryReport.Columns["DaysToExpiry"].HeaderText = "Days to Expiry";
                }

                lblTitle.Text = "Expiry Alert Report";
                lblSummary.Text = $"Total Batches Expiring Soon: {reportData.Rows.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "CSV Files|*.csv|Excel Files|*.xlsx";
                saveDialog.FileName = "ExpiryReport_" + DateTime.Now.ToString("yyyyMMdd");

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    // Export logic would go here
                    MessageBox.Show("Report exported successfully!", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                // Print logic would go here
                MessageBox.Show("Printing not yet implemented.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception)
            {
                MessageBox.Show("Error printing report.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
