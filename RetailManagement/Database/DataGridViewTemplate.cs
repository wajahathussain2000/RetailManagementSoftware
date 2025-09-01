using System;
using System.Windows.Forms;
using RetailManagement.Database;

namespace RetailManagement.Database
{
    /// <summary>
    /// Template class showing safe DataGridView operations
    /// Copy these patterns to prevent DBNull and missing column errors
    /// </summary>
    public static class DataGridViewTemplate
    {
        /// <summary>
        /// Example of safe cell click handler
        /// Use this pattern in all your forms
        /// </summary>
        public static void ExampleCellClickHandler(object sender, DataGridViewCellEventArgs e)
        {
            var gridView = sender as DataGridView;
            if (e.RowIndex >= 0 && gridView != null)
            {
                try
                {
                    DataGridViewRow row = gridView.Rows[e.RowIndex];
                    
                    // Safe way to get values - replace column names with your actual columns
                    int id = SafeDataHelper.SafeGetCellInt32(row, "ID");
                    string name = SafeDataHelper.SafeGetCellString(row, "Name");
                    decimal price = SafeDataHelper.SafeGetCellDecimal(row, "Price");
                    DateTime date = SafeDataHelper.SafeGetCellDateTime(row, "Date");
                    bool isActive = SafeDataHelper.SafeGetCellBool(row, "IsActive");
                    
                    // Use the values safely
                    // Example: textBoxName.Text = name;
                    // Example: textBoxPrice.Text = price.ToString("F2");
                }
                catch (Exception ex)
                {
                    GlobalExceptionHandler.HandleManualException(ex, "DataGridView Cell Click");
                }
            }
        }
        
        /// <summary>
        /// Example of safe database reading with SqlDataReader
        /// </summary>
        public static void ExampleSafeDataReaderUsage()
        {
            // This is just an example - adapt to your actual database code
            /*
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT * FROM YourTable", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Safe way to read data
                            int id = SafeDataHelper.SafeGetInt32(reader, "ID");
                            string name = SafeDataHelper.SafeGetString(reader, "Name");
                            decimal price = SafeDataHelper.SafeGetDecimal(reader, "Price");
                            DateTime date = SafeDataHelper.SafeGetDateTime(reader, "Date");
                        }
                    }
                }
            }
            */
        }
    }
}

/*
QUICK REFERENCE for safe patterns:

// WRONG - Can cause exceptions:
int id = Convert.ToInt32(row.Cells["ID"].Value);
string name = row.Cells["Name"].Value.ToString();
decimal price = Convert.ToDecimal(row.Cells["Price"].Value);

// RIGHT - Safe patterns:
int id = SafeDataHelper.SafeGetCellInt32(row, "ID");
string name = SafeDataHelper.SafeGetCellString(row, "Name");
decimal price = SafeDataHelper.SafeGetCellDecimal(row, "Price");

// For DataReader:
// WRONG:
int id = Convert.ToInt32(reader["ID"]);
string name = reader["Name"].ToString();

// RIGHT:
int id = SafeDataHelper.SafeGetInt32(reader, "ID");
string name = SafeDataHelper.SafeGetString(reader, "Name");
*/
