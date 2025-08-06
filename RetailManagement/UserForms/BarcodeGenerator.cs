using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Printing;
using RetailManagement.Database;

namespace RetailManagement.UserForms
{
    public partial class BarcodeGenerator : Form
    {
        private DataTable itemsData;
        private string selectedBarcode = "";

        public BarcodeGenerator()
        {
            InitializeComponent();
            LoadItems();
        }

        private void LoadItems()
        {
            try
            {
                string query = @"SELECT 
                                    ItemID,
                                    ItemName,
                                    Category,
                                    StockQuantity,
                                    PurchasePrice,
                                    SalePrice
                                FROM Items 
                                WHERE IsActive = 1 
                                ORDER BY ItemName";

                itemsData = DatabaseConnection.ExecuteQuery(query);
                ShowItemsData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading items: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowItemsData()
        {
            dgvItems.DataSource = itemsData;
            dgvItems.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string searchText = txtSearch.Text.Trim();
                if (string.IsNullOrEmpty(searchText))
                {
                    ShowItemsData();
                    return;
                }

                DataView dv = itemsData.DefaultView;
                dv.RowFilter = $"ItemName LIKE '%{searchText}%' OR Category LIKE '%{searchText}%'";
                dgvItems.DataSource = dv;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching items: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            LoadItems();
        }

        private void dgvItems_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvItems.Rows[e.RowIndex];
                int itemId = Convert.ToInt32(row.Cells["ItemID"].Value);
                string itemName = row.Cells["ItemName"].Value.ToString();
                
                // Generate barcode (using ItemID as barcode)
                selectedBarcode = itemId.ToString("D6"); // 6-digit format
                txtBarcode.Text = selectedBarcode;
                txtItemName.Text = itemName;
                
                // Generate barcode image
                GenerateBarcodeImage();
            }
        }

        private void GenerateBarcodeImage()
        {
            try
            {
                if (string.IsNullOrEmpty(selectedBarcode))
                {
                    pictureBoxBarcode.Image = null;
                    return;
                }

                // Create a simple barcode representation
                Bitmap barcodeImage = new Bitmap(300, 100);
                using (Graphics g = Graphics.FromImage(barcodeImage))
                {
                    g.Clear(Color.White);
                    
                    // Draw barcode lines
                    Random rand = new Random(selectedBarcode.GetHashCode()); // Use barcode as seed for consistent pattern
                    int x = 10;
                    int barWidth = 2;
                    
                    for (int i = 0; i < selectedBarcode.Length; i++)
                    {
                        int digit = int.Parse(selectedBarcode[i].ToString());
                        int barHeight = 60 + (digit * 3); // Vary height based on digit
                        
                        // Draw bars
                        for (int j = 0; j < digit + 1; j++)
                        {
                            g.FillRectangle(Brushes.Black, x, 10, barWidth, barHeight);
                            x += barWidth + 1;
                        }
                        x += 2; // Space between digits
                    }
                    
                    // Draw barcode text
                    using (Font font = new Font("Arial", 12, FontStyle.Bold))
                    {
                        g.DrawString(selectedBarcode, font, Brushes.Black, 10, 75);
                    }
                }
                
                pictureBoxBarcode.Image = barcodeImage;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating barcode: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPrintBarcode_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(selectedBarcode))
                {
                    MessageBox.Show("Please select an item first.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    PrintDocument pd = new PrintDocument();
                    pd.PrintPage += PrintBarcodePage;
                    pd.Print();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error printing barcode: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintBarcodePage(object sender, PrintPageEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;
                Font titleFont = new Font("Arial", 14, FontStyle.Bold);
                Font normalFont = new Font("Arial", 10);
                Font barcodeFont = new Font("Arial", 12, FontStyle.Bold);

                int yPos = 50;
                int leftMargin = 50;

                // Print title
                string title = "BARCODE LABEL";
                g.DrawString(title, titleFont, Brushes.Black, leftMargin, yPos);
                yPos += 30;

                // Print item name
                g.DrawString("Item: " + txtItemName.Text, normalFont, Brushes.Black, leftMargin, yPos);
                yPos += 20;

                // Print barcode number
                g.DrawString("Barcode: " + selectedBarcode, normalFont, Brushes.Black, leftMargin, yPos);
                yPos += 30;

                // Draw barcode
                if (pictureBoxBarcode.Image != null)
                {
                    g.DrawImage(pictureBoxBarcode.Image, leftMargin, yPos, 250, 80);
                    yPos += 90;
                }

                // Print date
                string date = "Generated: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                g.DrawString(date, normalFont, Brushes.Black, leftMargin, yPos);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error printing: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPrintMultiple_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvItems.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select items to print barcodes.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    PrintDocument pd = new PrintDocument();
                    pd.PrintPage += PrintMultipleBarcodesPage;
                    pd.Print();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error printing multiple barcodes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintMultipleBarcodesPage(object sender, PrintPageEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;
                Font titleFont = new Font("Arial", 12, FontStyle.Bold);
                Font normalFont = new Font("Arial", 8);
                Font barcodeFont = new Font("Arial", 10, FontStyle.Bold);

                int yPos = 30;
                int leftMargin = 30;
                int xPos = leftMargin;
                int itemsPerRow = 2;
                int currentItem = 0;

                foreach (DataGridViewRow row in dgvItems.SelectedRows)
                {
                    if (currentItem > 0 && currentItem % itemsPerRow == 0)
                    {
                        xPos = leftMargin;
                        yPos += 120; // Move to next row
                    }

                    if (yPos > e.PageBounds.Height - 150)
                    {
                        e.HasMorePages = true;
                        return;
                    }

                    int itemId = Convert.ToInt32(row.Cells["ItemID"].Value);
                    string itemName = row.Cells["ItemName"].Value.ToString();
                    string barcode = itemId.ToString("D6");

                    // Draw barcode box
                    g.DrawRectangle(Pens.Black, xPos, yPos, 250, 100);

                    // Draw item name
                    g.DrawString(itemName, normalFont, Brushes.Black, xPos + 5, yPos + 5);

                    // Draw barcode lines
                    Random rand = new Random(barcode.GetHashCode());
                    int barX = xPos + 10;
                    int barWidth = 1;
                    
                    for (int i = 0; i < barcode.Length; i++)
                    {
                        int digit = int.Parse(barcode[i].ToString());
                        int barHeight = 40 + (digit * 2);
                        
                        for (int j = 0; j < digit + 1; j++)
                        {
                            g.FillRectangle(Brushes.Black, barX, yPos + 25, barWidth, barHeight);
                            barX += barWidth + 1;
                        }
                        barX += 1;
                    }

                    // Draw barcode text
                    g.DrawString(barcode, barcodeFont, Brushes.Black, xPos + 10, yPos + 70);

                    xPos += 270; // Move to next column
                    currentItem++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error printing multiple barcodes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) // Enter key
            {
                btnSearch_Click(sender, e);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtBarcode.Clear();
            txtItemName.Clear();
            pictureBoxBarcode.Image = null;
            selectedBarcode = "";
        }
    }
} 