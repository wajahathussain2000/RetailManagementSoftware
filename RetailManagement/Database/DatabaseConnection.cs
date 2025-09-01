using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace RetailManagement.Database
{
    public class DatabaseConnection
    {
        // Use connection string from App.config, with fallback to local SQL Express
        private static string connectionString = GetConnectionString();

        private static string GetConnectionString()
        {
            try
            {
                // Try to get from App.config first
                var configConnectionString = ConfigurationManager.ConnectionStrings["RetailManagementDB"]?.ConnectionString;
                if (!string.IsNullOrEmpty(configConnectionString))
                {
                    return configConnectionString;
                }
            }
            catch
            {
                // ConfigurationManager might not be available, use fallback
            }
            
            // Fallback to local SQL Express
            return "Data Source=DESKTOP-KPKLR5V\\SQLEXPRESS;Initial Catalog=RetailManagementDB;Integrated Security=True;TrustServerCertificate=true;";
        }

        public static string GetConnectionStringPublic()
        {
            return connectionString;
        }

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        public static DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection connection = GetConnection())
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    DataTable dataTable = new DataTable();
                    try
                    {
                        connection.Open();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Database Error: " + ex.Message);
                    }
                    return dataTable;
                }
            }
        }

        public static int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection connection = GetConnection())
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    try
                    {
                        connection.Open();
                        return command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Database Error: " + ex.Message);
                    }
                }
            }
        }

        public static int ExecuteNonQuery(string query, SqlParameter[] parameters, SqlTransaction transaction)
        {
            using (SqlCommand command = new SqlCommand(query, transaction.Connection, transaction))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                try
                {
                    return command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception("Database Error: " + ex.Message);
                }
            }
        }

        public static object ExecuteScalar(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection connection = GetConnection())
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    try
                    {
                        connection.Open();
                        return command.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Database Error: " + ex.Message);
                    }
                }
            }
        }

        public static object ExecuteScalar(string query, SqlParameter[] parameters, SqlTransaction transaction)
        {
            using (SqlCommand command = new SqlCommand(query, transaction.Connection, transaction))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                try
                {
                    return command.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    throw new Exception("Database Error: " + ex.Message);
                }
            }
        }

        // Helper method to get the correct stock column name from Items table
        private static string _stockColumnName = null;
        public static string GetStockColumnName()
        {
            if (_stockColumnName != null)
                return _stockColumnName;

            try
            {
                string columnQuery = @"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS 
                                     WHERE TABLE_NAME = 'Items' ORDER BY ORDINAL_POSITION";
                
                DataTable columnsTable = ExecuteQuery(columnQuery);
                
                foreach (DataRow colRow in columnsTable.Rows)
                {
                    string colName = colRow["COLUMN_NAME"].ToString();
                    if (colName.Equals("StockQuantity", StringComparison.OrdinalIgnoreCase))
                    {
                        _stockColumnName = "StockQuantity";
                        return _stockColumnName;
                    }
                    else if (colName.Equals("Quantity", StringComparison.OrdinalIgnoreCase))
                    {
                        _stockColumnName = "Quantity";
                    }
                    else if (colName.Equals("Stock", StringComparison.OrdinalIgnoreCase))
                    {
                        _stockColumnName = "Stock";
                    }
                    else if (colName.Equals("QtyInStock", StringComparison.OrdinalIgnoreCase))
                    {
                        _stockColumnName = "QtyInStock";
                    }
                }
                
                // If no specific stock column found, use the first one we found or default
                if (_stockColumnName == null)
                    _stockColumnName = "Quantity";
                    
                return _stockColumnName;
            }
            catch
            {
                _stockColumnName = "Quantity"; // Default fallback
                return _stockColumnName;
            }
        }
    }
} 