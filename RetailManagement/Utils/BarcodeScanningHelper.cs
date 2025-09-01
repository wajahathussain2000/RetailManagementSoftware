using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using RetailManagement.Database;

namespace RetailManagement.Utils
{
    /// <summary>
    /// Helper class for barcode scanning functionality
    /// Provides methods to scan barcodes and retrieve item information
    /// </summary>
    public static class BarcodeScanningHelper
    {
        /// <summary>
        /// Searches for an item by its barcode
        /// </summary>
        /// <param name="barcode">The scanned barcode</param>
        /// <returns>DataRow containing item information, or null if not found</returns>
        public static DataRow FindItemByBarcode(string barcode)
        {
            if (string.IsNullOrWhiteSpace(barcode))
                return null;

            try
            {
                string query = @"SELECT 
                                i.ItemID, 
                                i.ItemName, 
                                i.Description, 
                                i.Price, 
                                i.StockQuantity, 
                                i.Category, 
                                i.Barcode, 
                                i.HSN_SAC_Code,
                                i.GST_Rate,
                                i.IsActive,
                                c.CompanyName,
                                i.CompanyID
                            FROM Items i 
                            LEFT JOIN Companies c ON i.CompanyID = c.CompanyID
                            WHERE i.Barcode = @Barcode AND i.IsActive = 1";

                SqlParameter[] parameters = { new SqlParameter("@Barcode", barcode.Trim()) };
                DataTable result = DatabaseConnection.ExecuteQuery(query, parameters);

                if (result.Rows.Count > 0)
                {
                    return result.Rows[0];
                }

                return null;
            }
            catch (Exception ex)
            {
                GlobalExceptionHandler.HandleManualException(ex, "Barcode Search");
                return null;
            }
        }

        /// <summary>
        /// Creates a barcode search textbox that automatically searches when barcode is entered
        /// </summary>
        /// <param name="parent">Parent control</param>
        /// <param name="location">Location of the textbox</param>
        /// <param name="onItemFound">Callback when item is found</param>
        /// <param name="onItemNotFound">Callback when item is not found</param>
        /// <returns>The created textbox</returns>
        public static TextBox CreateBarcodeSearchBox(
            Control parent, 
            System.Drawing.Point location,
            Action<DataRow> onItemFound = null,
            Action<string> onItemNotFound = null)
        {
            TextBox barcodeTextBox = new TextBox
            {
                Name = "txtBarcodeSearch",
                Location = location,
                Size = new System.Drawing.Size(200, 23),
                Text = "Scan or enter barcode...",
                ForeColor = System.Drawing.Color.Gray
            };

            // Add placeholder text functionality
            const string placeholderText = "Scan or enter barcode...";
            bool isPlaceholder = true;

            // Handle focus events for placeholder text
            barcodeTextBox.Enter += (sender, e) =>
            {
                if (isPlaceholder)
                {
                    barcodeTextBox.Text = "";
                    barcodeTextBox.ForeColor = System.Drawing.Color.Black;
                    isPlaceholder = false;
                }
            };

            barcodeTextBox.Leave += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(barcodeTextBox.Text))
                {
                    barcodeTextBox.Text = placeholderText;
                    barcodeTextBox.ForeColor = System.Drawing.Color.Gray;
                    isPlaceholder = true;
                }
                else if (!isPlaceholder)
                {
                    ProcessBarcodeSearch(barcodeTextBox.Text.Trim(), onItemFound, onItemNotFound);
                }
            };

            // Add keyboard event handlers
            barcodeTextBox.KeyPress += (sender, e) =>
            {
                if (isPlaceholder)
                {
                    barcodeTextBox.Text = "";
                    barcodeTextBox.ForeColor = System.Drawing.Color.Black;
                    isPlaceholder = false;
                }

                if (e.KeyChar == (char)Keys.Enter)
                {
                    e.Handled = true;
                    if (!isPlaceholder && !string.IsNullOrWhiteSpace(barcodeTextBox.Text))
                    {
                        ProcessBarcodeSearch(barcodeTextBox.Text.Trim(), onItemFound, onItemNotFound);
                        barcodeTextBox.SelectAll(); // Select all text for next scan
                    }
                }
            };

            parent.Controls.Add(barcodeTextBox);
            return barcodeTextBox;
        }

        /// <summary>
        /// Processes barcode search and executes callbacks
        /// </summary>
        private static void ProcessBarcodeSearch(string barcode, Action<DataRow> onItemFound, Action<string> onItemNotFound)
        {
            if (string.IsNullOrWhiteSpace(barcode))
                return;

            try
            {
                DataRow item = FindItemByBarcode(barcode);
                
                if (item != null)
                {
                    onItemFound?.Invoke(item);
                }
                else
                {
                    onItemNotFound?.Invoke(barcode);
                    
                    // Show default not found message if no callback provided
                    if (onItemNotFound == null)
                    {
                        MessageBox.Show($"Item with barcode '{barcode}' not found.", 
                                      "Item Not Found", 
                                      MessageBoxButtons.OK, 
                                      MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                GlobalExceptionHandler.HandleManualException(ex, "Barcode Processing");
            }
        }

        /// <summary>
        /// Validates if a barcode format is correct
        /// </summary>
        /// <param name="barcode">Barcode to validate</param>
        /// <returns>True if valid format</returns>
        public static bool IsValidBarcodeFormat(string barcode)
        {
            if (string.IsNullOrWhiteSpace(barcode))
                return false;

            // Remove whitespace
            barcode = barcode.Trim();

            // Check minimum and maximum length
            if (barcode.Length < 6 || barcode.Length > 20)
                return false;

            // Check if it contains only valid characters (alphanumeric)
            foreach (char c in barcode)
            {
                if (!char.IsLetterOrDigit(c))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if a barcode already exists in the database
        /// </summary>
        /// <param name="barcode">Barcode to check</param>
        /// <param name="excludeItemId">Item ID to exclude from check (for updates)</param>
        /// <returns>True if barcode exists</returns>
        public static bool BarcodeExists(string barcode, int? excludeItemId = null)
        {
            if (string.IsNullOrWhiteSpace(barcode))
                return false;

            try
            {
                string query = "SELECT COUNT(*) FROM Items WHERE Barcode = @Barcode AND IsActive = 1";
                SqlParameter[] parameters;

                if (excludeItemId.HasValue)
                {
                    query += " AND ItemID != @ItemID";
                    parameters = new SqlParameter[] 
                    {
                        new SqlParameter("@Barcode", barcode.Trim()),
                        new SqlParameter("@ItemID", excludeItemId.Value)
                    };
                }
                else
                {
                    parameters = new SqlParameter[] { new SqlParameter("@Barcode", barcode.Trim()) };
                }

                int count = SafeDataHelper.SafeToInt32(DatabaseConnection.ExecuteScalar(query, parameters));
                return count > 0;
            }
            catch (Exception ex)
            {
                GlobalExceptionHandler.HandleManualException(ex, "Barcode Existence Check");
                return false;
            }
        }

        /// <summary>
        /// Gets stock information for a barcode
        /// </summary>
        /// <param name="barcode">Barcode to check</param>
        /// <returns>Stock quantity or -1 if not found</returns>
        public static int GetStockByBarcode(string barcode)
        {
            try
            {
                DataRow item = FindItemByBarcode(barcode);
                if (item != null)
                {
                    return SafeDataHelper.SafeToInt32(item["StockQuantity"]);
                }
                return -1; // Not found
            }
            catch (Exception ex)
            {
                GlobalExceptionHandler.HandleManualException(ex, "Stock Check by Barcode");
                return -1;
            }
        }

        /// <summary>
        /// Updates stock quantity for an item by barcode
        /// </summary>
        /// <param name="barcode">Item barcode</param>
        /// <param name="quantityChange">Quantity to add (positive) or subtract (negative)</param>
        /// <returns>True if successful</returns>
        public static bool UpdateStockByBarcode(string barcode, int quantityChange)
        {
            try
            {
                string query = @"UPDATE Items 
                               SET StockQuantity = StockQuantity + @QuantityChange 
                               WHERE Barcode = @Barcode AND IsActive = 1";

                SqlParameter[] parameters = 
                {
                    new SqlParameter("@QuantityChange", quantityChange),
                    new SqlParameter("@Barcode", barcode.Trim())
                };

                int rowsAffected = DatabaseConnection.ExecuteNonQuery(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                GlobalExceptionHandler.HandleManualException(ex, "Stock Update by Barcode");
                return false;
            }
        }

        /// <summary>
        /// Creates a label for barcode scanning instructions
        /// </summary>
        /// <param name="parent">Parent control</param>
        /// <param name="location">Location of the label</param>
        /// <returns>The created label</returns>
        public static Label CreateBarcodeInstructionLabel(Control parent, System.Drawing.Point location)
        {
            Label instructionLabel = new Label
            {
                Text = "📱 Scan barcode or press Enter",
                Location = location,
                Size = new System.Drawing.Size(200, 20),
                ForeColor = System.Drawing.Color.Gray,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Italic)
            };

            parent.Controls.Add(instructionLabel);
            return instructionLabel;
        }

        /// <summary>
        /// Demonstrates barcode scanning functionality
        /// Call this method to test barcode scanning in any form
        /// </summary>
        /// <param name="parent">Parent form or control</param>
        public static void ShowBarcodeScanningDemo(Control parent)
        {
            // Create a demo panel
            Panel demoPanel = new Panel
            {
                Size = new System.Drawing.Size(300, 150),
                Location = new System.Drawing.Point(10, 10),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = System.Drawing.Color.LightYellow
            };

            Label titleLabel = new Label
            {
                Text = "Barcode Scanning Demo",
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(280, 20),
                Font = new System.Drawing.Font("Microsoft Sans Serif", 10f, System.Drawing.FontStyle.Bold)
            };

            CreateBarcodeInstructionLabel(demoPanel, new System.Drawing.Point(10, 35));

            CreateBarcodeSearchBox(demoPanel, new System.Drawing.Point(10, 60),
                onItemFound: (item) =>
                {
                    string itemName = SafeDataHelper.SafeToString(item["ItemName"]);
                    decimal price = SafeDataHelper.SafeToDecimal(item["Price"]);
                    MessageBox.Show($"Item Found!\nName: {itemName}\nPrice: {price:C2}", 
                                  "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                },
                onItemNotFound: (barcode) =>
                {
                    MessageBox.Show($"Item with barcode '{barcode}' not found in inventory.", 
                                  "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                });

            Label resultLabel = new Label
            {
                Text = "Scan a barcode to see results above...",
                Location = new System.Drawing.Point(10, 90),
                Size = new System.Drawing.Size(280, 40),
                ForeColor = System.Drawing.Color.Blue
            };

            demoPanel.Controls.Add(titleLabel);
            demoPanel.Controls.Add(resultLabel);
            parent.Controls.Add(demoPanel);
        }

        /// <summary>
        /// Creates a textbox with placeholder text functionality for .NET Framework
        /// </summary>
        /// <param name="parent">Parent control</param>
        /// <param name="location">Location of the textbox</param>
        /// <param name="size">Size of the textbox</param>
        /// <param name="placeholderText">Placeholder text to display</param>
        /// <param name="name">Name of the textbox</param>
        /// <returns>TextBox with placeholder functionality</returns>
        public static TextBox CreatePlaceholderTextBox(
            Control parent,
            System.Drawing.Point location,
            System.Drawing.Size size,
            string placeholderText,
            string name = "")
        {
            TextBox textBox = new TextBox
            {
                Name = name,
                Location = location,
                Size = size,
                Text = placeholderText,
                ForeColor = System.Drawing.Color.Gray
            };

            bool isPlaceholder = true;

            // Handle focus events for placeholder text
            textBox.Enter += (sender, e) =>
            {
                if (isPlaceholder)
                {
                    textBox.Text = "";
                    textBox.ForeColor = System.Drawing.Color.Black;
                    isPlaceholder = false;
                }
            };

            textBox.Leave += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeholderText;
                    textBox.ForeColor = System.Drawing.Color.Gray;
                    isPlaceholder = true;
                }
            };

            // Add method to get actual text (excluding placeholder)
            textBox.Tag = new Func<string>(() => isPlaceholder ? "" : textBox.Text);

            parent.Controls.Add(textBox);
            return textBox;
        }

        /// <summary>
        /// Gets the actual text from a placeholder textbox (excluding placeholder text)
        /// </summary>
        /// <param name="textBox">TextBox created with CreatePlaceholderTextBox</param>
        /// <returns>Actual text or empty string if placeholder is showing</returns>
        public static string GetActualText(TextBox textBox)
        {
            if (textBox.Tag is Func<string> getActualText)
            {
                return getActualText();
            }
            return textBox.Text; // Fallback for regular textboxes
        }
    }
}
