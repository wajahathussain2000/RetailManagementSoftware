using System;
using System.Data;
using System.Windows.Forms;
using RetailManagement.Database;
using RetailManagement.Models;

namespace RetailManagement.Utils
{
    /// <summary>
    /// Helper class for binding customer data to UI controls consistently across the application
    /// </summary>
    public static class CustomerBindingHelper
    {
        /// <summary>
        /// Load customers into a ComboBox with standard configuration
        /// </summary>
        /// <param name="comboBox">The ComboBox to populate</param>
        /// <param name="includeAllOption">Whether to include "All Customers" option</param>
        /// <param name="includeSelectOption">Whether to include "Select Customer" option</param>
        public static void LoadCustomers(ComboBox comboBox, bool includeAllOption = false, bool includeSelectOption = true)
        {
            try
            {
                // Clear existing items
                comboBox.Items.Clear();
                comboBox.DataSource = null;

                // Create data table for binding
                DataTable customers = new DataTable();
                customers.Columns.Add("CustomerID", typeof(int));
                customers.Columns.Add("CustomerName", typeof(string));
                customers.Columns.Add("Display", typeof(string));

                // Add default options
                if (includeSelectOption)
                {
                    DataRow selectRow = customers.NewRow();
                    selectRow["CustomerID"] = 0;
                    selectRow["CustomerName"] = "Select Customer";
                    selectRow["Display"] = "-- Select Customer --";
                    customers.Rows.Add(selectRow);
                }

                if (includeAllOption)
                {
                    DataRow allRow = customers.NewRow();
                    allRow["CustomerID"] = -1;
                    allRow["CustomerName"] = "All Customers";
                    allRow["Display"] = "-- All Customers --";
                    customers.Rows.Add(allRow);
                }

                // Load customers from database
                string query = @"SELECT CustomerID, CustomerName, Phone, Email, City, CurrentBalance 
                               FROM Customers 
                               WHERE IsActive = 1 
                               ORDER BY CustomerName";
                
                DataTable customerData = DatabaseConnection.ExecuteQuery(query);
                
                foreach (DataRow row in customerData.Rows)
                {
                    DataRow newRow = customers.NewRow();
                    newRow["CustomerID"] = SafeDataHelper.SafeToInt32(row["CustomerID"]);
                    newRow["CustomerName"] = SafeDataHelper.SafeToString(row["CustomerName"]);
                    
                    // Create display text with additional info
                    string phone = SafeDataHelper.SafeToString(row["Phone"]);
                    string city = SafeDataHelper.SafeToString(row["City"]);
                    decimal balance = SafeDataHelper.SafeToDecimal(row["CurrentBalance"]);
                    
                    string display = SafeDataHelper.SafeToString(row["CustomerName"]);
                    if (!string.IsNullOrEmpty(phone))
                        display += $" ({phone})";
                    if (!string.IsNullOrEmpty(city))
                        display += $" - {city}";
                    if (balance != 0)
                        display += $" [Bal: {balance:C}]";
                    
                    newRow["Display"] = display;
                    customers.Rows.Add(newRow);
                }

                // Bind to ComboBox
                comboBox.DataSource = customers;
                comboBox.DisplayMember = "Display";
                comboBox.ValueMember = "CustomerID";
                comboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customers: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Get selected customer ID from a ComboBox
        /// </summary>
        /// <param name="comboBox">The ComboBox to get value from</param>
        /// <returns>Customer ID or 0 if none selected</returns>
        public static int GetSelectedCustomerId(ComboBox comboBox)
        {
            try
            {
                if (comboBox.SelectedValue != null)
                {
                    return SafeDataHelper.SafeToInt32(comboBox.SelectedValue);
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Get selected customer name from a ComboBox
        /// </summary>
        /// <param name="comboBox">The ComboBox to get value from</param>
        /// <returns>Customer name or empty string if none selected</returns>
        public static string GetSelectedCustomerName(ComboBox comboBox)
        {
            try
            {
                if (comboBox.SelectedItem != null && comboBox.SelectedValue != null)
                {
                    int customerId = SafeDataHelper.SafeToInt32(comboBox.SelectedValue);
                    if (customerId > 0)
                    {
                        DataRowView row = comboBox.SelectedItem as DataRowView;
                        return SafeDataHelper.SafeToString(row?["CustomerName"]);
                    }
                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Load customers into a simple ComboBox with ComboBoxItem objects
        /// </summary>
        /// <param name="comboBox">The ComboBox to populate</param>
        /// <param name="includeAllOption">Whether to include "All Customers" option</param>
        public static void LoadCustomersSimple(ComboBox comboBox, bool includeAllOption = false)
        {
            try
            {
                comboBox.Items.Clear();
                
                if (includeAllOption)
                {
                    comboBox.Items.Add(new ComboBoxItem { Text = "All Customers", Value = 0 });
                }

                string query = "SELECT CustomerID, CustomerName FROM Customers WHERE IsActive = 1 ORDER BY CustomerName";
                DataTable customers = DatabaseConnection.ExecuteQuery(query);
                
                foreach (DataRow row in customers.Rows)
                {
                    comboBox.Items.Add(new ComboBoxItem
                    {
                        Text = SafeDataHelper.SafeToString(row["CustomerName"]),
                        Value = SafeDataHelper.SafeToInt32(row["CustomerID"])
                    });
                }
                
                if (comboBox.Items.Count > 0)
                    comboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customers: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Refresh customer data in a ComboBox while preserving selection
        /// </summary>
        /// <param name="comboBox">The ComboBox to refresh</param>
        /// <param name="includeAllOption">Whether to include "All Customers" option</param>
        /// <param name="includeSelectOption">Whether to include "Select Customer" option</param>
        public static void RefreshCustomers(ComboBox comboBox, bool includeAllOption = false, bool includeSelectOption = true)
        {
            int selectedId = GetSelectedCustomerId(comboBox);
            LoadCustomers(comboBox, includeAllOption, includeSelectOption);
            
            // Try to restore selection
            if (selectedId > 0)
            {
                SetSelectedCustomer(comboBox, selectedId);
            }
        }

        /// <summary>
        /// Set selected customer by ID
        /// </summary>
        /// <param name="comboBox">The ComboBox to set</param>
        /// <param name="customerId">Customer ID to select</param>
        public static void SetSelectedCustomer(ComboBox comboBox, int customerId)
        {
            try
            {
                comboBox.SelectedValue = customerId;
            }
            catch
            {
                // Ignore selection errors
            }
        }

        /// <summary>
        /// Get customer details by ID
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <returns>DataRow with customer details or null if not found</returns>
        public static DataRow GetCustomerDetails(int customerId)
        {
            try
            {
                string query = @"SELECT CustomerID, CustomerName, Phone, Email, Address, City, State, 
                               PostalCode, GST_Number, CreditLimit, CreditDays, CurrentBalance, IsActive
                               FROM Customers WHERE CustomerID = @CustomerID";
                
                var parameters = new[] { new System.Data.SqlClient.SqlParameter("@CustomerID", customerId) };
                DataTable dt = DatabaseConnection.ExecuteQuery(query, parameters);
                
                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }
            catch
            {
                return null;
            }
        }
    }
}
