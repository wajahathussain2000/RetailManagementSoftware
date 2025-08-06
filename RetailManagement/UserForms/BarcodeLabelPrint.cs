using RetailManagement.Database;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;

namespace RetailManagement.UserForms
{
    public partial class BarcodeLabelPrint : Form
    {
        private DataTable itemsData;
        private int labelsPerPage = 10;
        private int currentLabelIndex = 0;
        private DataRow[] selectedItems;

        public BarcodeLabelPrint()
        {
            InitializeComponent();
            LoadData();
        }

        private void BarcodeLabelPrint_Load(object sender, EventArgs e)
        {
            LoadItems();
            numQuantity.Value = 1;
            numLabelsPerPage.Value = 10;
        }

        private void LoadData()
        {
            itemsData = new DataTable();
        }

        private void LoadItems()
        {
            try
            {
                string query = @"
                    SELECT 
                        ItemID,
                        ItemName,
                        Category,
                        Price,
                        Barcode
                    FROM Items 
                    ORDER BY ItemName";

                itemsData = DatabaseConnection.ExecuteQuery(query);
                DisplayItems();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading items: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayItems()
        {
            dgvItems.DataSource = itemsData;
            ConfigureDataGridView();
        }

        private void ConfigureDataGridView()
        {
            dgvItems.Columns["ItemID"].HeaderText = "Item ID";
            dgvItems.Columns["ItemID"].Width = 80;
            dgvItems.Columns["ItemID"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvItems.Columns["ItemName"].HeaderText = "Item Name";
            dgvItems.Columns["ItemName"].Width = 200;

            dgvItems.Columns["Category"].HeaderText = "Category";
            dgvItems.Columns["Category"].Width = 120;

            dgvItems.Columns["Price"].HeaderText = "Price";
            dgvItems.Columns["Price"].Width = 100;
            dgvItems.Columns["Price"].DefaultCellStyle.Format = "N2";
            dgvItems.Columns["Price"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvItems.Columns["Barcode"].HeaderText = "Barcode";
            dgvItems.Columns["Barcode"].Width = 120;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string searchTerm = txtSearch.Text.Trim();
                if (string.IsNullOrEmpty(searchTerm))
                {
                    DisplayItems();
                    return;
                }

                string query = @"
                    SELECT 
                        ItemID,
                        ItemName,
                        Category,
                        Price,
                        Barcode
                    FROM Items 
                    WHERE ItemName LIKE @SearchTerm OR Category LIKE @SearchTerm OR Barcode LIKE @SearchTerm
                    ORDER BY ItemName";

                itemsData = DatabaseConnection.ExecuteQuery(query,
                    new System.Data.SqlClient.SqlParameter[] { new System.Data.SqlClient.SqlParameter("@SearchTerm", "%" + searchTerm + "%") });
                DisplayItems();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching items: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvItems.Rows)
            {
                row.Selected = true;
            }
        }

        private void btnClearSelection_Click(object sender, EventArgs e)
        {
            dgvItems.ClearSelection();
        }

        private void btnPrintLabels_Click(object sender, EventArgs e)
        {
            if (dgvItems.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select at least one item to print labels.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                labelsPerPage = (int)numLabelsPerPage.Value;
                selectedItems = new DataRow[dgvItems.SelectedRows.Count];
                
                for (int i = 0; i < dgvItems.SelectedRows.Count; i++)
                {
                    int rowIndex = dgvItems.SelectedRows[i].Index;
                    selectedItems[i] = itemsData.Rows[rowIndex];
                }

                currentLabelIndex = 0;
                PrintDocument pd = new PrintDocument();
                pd.PrintPage += PrintLabelsPage;
                pd.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error printing labels: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintLabelsPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font titleFont = new Font("Arial", 8, FontStyle.Bold);
            Font normalFont = new Font("Arial", 6);
            Font barcodeFont = new Font("Arial", 10, FontStyle.Bold);
            Brush brush = Brushes.Black;

            int labelWidth = 200;
            int labelHeight = 100;
            int labelsPerRow = 3;
            int marginX = 20;
            int marginY = 20;
            int spacingX = 10;
            int spacingY = 10;

            int itemsPerPage = labelsPerPage;
            int startIndex = currentLabelIndex;
            int endIndex = Math.Min(startIndex + itemsPerPage, selectedItems.Length);

            for (int i = startIndex; i < endIndex; i++)
            {
                int row = (i - startIndex) / labelsPerRow;
                int col = (i - startIndex) % labelsPerRow;

                int x = marginX + col * (labelWidth + spacingX);
                int y = marginY + row * (labelHeight + spacingY);

                var item = selectedItems[i];
                string itemName = item["ItemName"].ToString();
                string category = item["Category"].ToString();
                string price = Convert.ToDecimal(item["Price"]).ToString("N2");
                string barcode = item["Barcode"].ToString();

                // Draw label border
                g.DrawRectangle(Pens.Black, x, y, labelWidth, labelHeight);

                // Draw item name
                string displayName = itemName.Length > 20 ? itemName.Substring(0, 20) + "..." : itemName;
                g.DrawString(displayName, titleFont, brush, x + 5, y + 5);

                // Draw category
                g.DrawString(category, normalFont, brush, x + 5, y + 20);

                // Draw price
                g.DrawString($"₹{price}", titleFont, brush, x + 5, y + 35);

                // Draw barcode text
                g.DrawString(barcode, barcodeFont, brush, x + 5, y + 55);

                // Draw simple barcode pattern
                DrawSimpleBarcode(g, barcode, x + 5, y + 70, 150, 20);
            }

            currentLabelIndex += itemsPerPage;

            // Check if there are more pages
            if (currentLabelIndex < selectedItems.Length)
            {
                e.HasMorePages = true;
            }
            else
            {
                e.HasMorePages = false;
                currentLabelIndex = 0;
            }
        }

        private void DrawSimpleBarcode(Graphics g, string barcode, int x, int y, int width, int height)
        {
            // Simple barcode representation using alternating bars
            int barWidth = width / barcode.Length;
            Random rand = new Random(barcode.GetHashCode()); // Use hash for consistent pattern

            for (int i = 0; i < barcode.Length; i++)
            {
                int barX = x + i * barWidth;
                int barHeight = rand.Next(height / 2, height);
                int barY = y + (height - barHeight) / 2;

                if (i % 2 == 0)
                {
                    g.FillRectangle(Brushes.Black, barX, barY, barWidth - 1, barHeight);
                }
            }
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            if (dgvItems.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select at least one item to preview labels.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                labelsPerPage = (int)numLabelsPerPage.Value;
                selectedItems = new DataRow[dgvItems.SelectedRows.Count];
                
                for (int i = 0; i < dgvItems.SelectedRows.Count; i++)
                {
                    int rowIndex = dgvItems.SelectedRows[i].Index;
                    selectedItems[i] = itemsData.Rows[rowIndex];
                }

                currentLabelIndex = 0;
                PrintPreviewDialog ppd = new PrintPreviewDialog();
                PrintDocument pd = new PrintDocument();
                pd.PrintPage += PrintLabelsPage;
                ppd.Document = pd;
                ppd.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error previewing labels: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (dgvItems.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select at least one item to export.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "CSV files (*.csv)|*.csv";
                sfd.FileName = $"BarcodeLabels_{DateTime.Now:yyyyMMdd}.csv";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    StringBuilder sb = new StringBuilder();

                    // Headers
                    sb.AppendLine("Item ID,Item Name,Category,Price,Barcode,Quantity");

                    // Data
                    foreach (DataGridViewRow row in dgvItems.SelectedRows)
                    {
                        int quantity = (int)numQuantity.Value;
                        sb.AppendLine($"{row.Cells["ItemID"].Value}," +
                                     $"\"{row.Cells["ItemName"].Value}\"," +
                                     $"{row.Cells["Category"].Value}," +
                                     $"{Convert.ToDecimal(row.Cells["Price"].Value):N2}," +
                                     $"{row.Cells["Barcode"].Value}," +
                                     $"{quantity}");
                    }

                    System.IO.File.WriteAllText(sfd.FileName, sb.ToString());
                    MessageBox.Show("Data exported successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
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

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) // Enter key
            {
                btnSearch_Click(sender, e);
                e.Handled = true;
            }
        }
    }
} 