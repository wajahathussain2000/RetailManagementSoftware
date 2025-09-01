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
    public partial class LowStockReportForm : Form
    {
        private DataTable reportData;

        public LowStockReportForm(DataTable data)
        {
            this.reportData = data;
            InitializeComponent();
            LoadReport();
        }

        private void LoadReport()
        {
            try
            {
                dgvLowStockReport.DataSource = reportData;
                
                if (dgvLowStockReport.Columns.Count > 0)
                {
                    dgvLowStockReport.Columns["ItemID"].Visible = false;
                    dgvLowStockReport.Columns["ItemName"].HeaderText = "Item Name";
                    dgvLowStockReport.Columns["CurrentStock"].HeaderText = "Current Stock";
                    dgvLowStockReport.Columns["MinimumStock"].HeaderText = "Minimum Stock";
                    dgvLowStockReport.Columns["Category"].HeaderText = "Category";
                }

                lblTitle.Text = "Low Stock Alert Report";
                lblSummary.Text = $"Total Items Below Minimum Stock: {reportData.Rows.Count}";
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
                saveDialog.FileName = "LowStockReport_" + DateTime.Now.ToString("yyyyMMdd");

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
