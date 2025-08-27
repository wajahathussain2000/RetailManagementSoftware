using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using RetailManagement.Database;

namespace RetailManagement.UserForms
{
    public partial class BaseReportForm : Form
    {
        protected ReportViewer reportViewer;
        protected DataTable reportData;
        protected string reportPath;
        protected string reportTitle;

        public BaseReportForm()
        {
            InitializeComponent();
            InitializeReportViewer();
        }

        private void InitializeReportViewer()
        {
            reportViewer = new ReportViewer();
            reportViewer.Dock = DockStyle.Fill;
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.LocalReport.ReportPath = reportPath;
            reportViewer.LocalReport.DataSources.Clear();
            this.Controls.Add(reportViewer);
        }

        protected virtual void LoadReportData()
        {
            try
            {
                if (reportData != null && reportData.Rows.Count > 0)
                {
                    ReportDataSource dataSource = new ReportDataSource("DataSet1", reportData);
                    reportViewer.LocalReport.DataSources.Add(dataSource);
                    reportViewer.RefreshReport();
                }
                else
                {
                    MessageBox.Show("No data found for the selected criteria.", "Information", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading report: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected virtual void ExportToPDF(string fileName)
        {
            try
            {
                if (reportData != null && reportData.Rows.Count > 0)
                {
                    Warning[] warnings;
                    string[] streamIds;
                    string mimeType = string.Empty;
                    string encoding = string.Empty;
                    string extension = string.Empty;

                    byte[] bytes = reportViewer.LocalReport.Render("PDF", null, out mimeType, 
                        out encoding, out extension, out streamIds, out warnings);

                    System.IO.File.WriteAllBytes(fileName, bytes);
                    MessageBox.Show("Report exported successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No data to export.", "Information", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting report: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected virtual void ExportToExcel(string fileName)
        {
            try
            {
                if (reportData != null && reportData.Rows.Count > 0)
                {
                    Warning[] warnings;
                    string[] streamIds;
                    string mimeType = string.Empty;
                    string encoding = string.Empty;
                    string extension = string.Empty;

                    byte[] bytes = reportViewer.LocalReport.Render("EXCEL", null, out mimeType, 
                        out encoding, out extension, out streamIds, out warnings);

                    System.IO.File.WriteAllBytes(fileName, bytes);
                    MessageBox.Show("Report exported successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No data to export.", "Information", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting report: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected virtual void PrintReport()
        {
            try
            {
                if (reportData != null && reportData.Rows.Count > 0)
                {
                    reportViewer.PrintDialog();
                }
                else
                {
                    MessageBox.Show("No data to print.", "Information", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error printing report: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
