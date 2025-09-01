using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace RetailManagement.Database
{
    /// <summary>
    /// Centralized utility class for safe database operations and data access
    /// Prevents DBNull exceptions, missing column errors, and type conversion issues
    /// </summary>
    public static class SafeDataHelper
    {
        #region Safe Type Conversion Methods

        /// <summary>
        /// Safely converts an object to string, handling null and DBNull values
        /// </summary>
        public static string SafeToString(object value, string defaultValue = "")
        {
            if (value == null || value == DBNull.Value)
                return defaultValue;
            return value.ToString();
        }

        /// <summary>
        /// Safely converts an object to decimal, handling null and DBNull values
        /// </summary>
        public static decimal SafeToDecimal(object value, decimal defaultValue = 0m)
        {
            if (value == null || value == DBNull.Value)
                return defaultValue;
            
            try
            {
                return Convert.ToDecimal(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Safely converts an object to int, handling null and DBNull values
        /// </summary>
        public static int SafeToInt32(object value, int defaultValue = 0)
        {
            if (value == null || value == DBNull.Value)
                return defaultValue;
            
            try
            {
                return Convert.ToInt32(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Safely converts an object to long, handling null and DBNull values
        /// </summary>
        public static long SafeToInt64(object value, long defaultValue = 0L)
        {
            if (value == null || value == DBNull.Value)
                return defaultValue;
            
            try
            {
                return Convert.ToInt64(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Safely converts an object to double, handling null and DBNull values
        /// </summary>
        public static double SafeToDouble(object value, double defaultValue = 0.0)
        {
            if (value == null || value == DBNull.Value)
                return defaultValue;
            
            try
            {
                return Convert.ToDouble(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Safely converts an object to DateTime, handling null and DBNull values
        /// </summary>
        public static DateTime SafeToDateTime(object value, DateTime? defaultValue = null)
        {
            if (value == null || value == DBNull.Value)
                return defaultValue ?? DateTime.MinValue;
            
            try
            {
                return Convert.ToDateTime(value);
            }
            catch
            {
                return defaultValue ?? DateTime.MinValue;
            }
        }

        /// <summary>
        /// Safely converts an object to bool, handling null and DBNull values
        /// </summary>
        public static bool SafeToBool(object value, bool defaultValue = false)
        {
            if (value == null || value == DBNull.Value)
                return defaultValue;
            
            try
            {
                return Convert.ToBoolean(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        #endregion

        #region Safe DataGridView Access Methods

        /// <summary>
        /// Safely gets cell value if column exists in DataGridView
        /// </summary>
        public static object GetCellValueSafe(DataGridViewRow row, string columnName)
        {
            try
            {
                if (row?.DataGridView?.Columns?.Contains(columnName) == true)
                {
                    return row.Cells[columnName].Value;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Safely gets string value from DataGridView cell
        /// </summary>
        public static string SafeGetCellString(DataGridViewRow row, string columnName, string defaultValue = "")
        {
            return SafeToString(GetCellValueSafe(row, columnName), defaultValue);
        }

        /// <summary>
        /// Safely gets decimal value from DataGridView cell
        /// </summary>
        public static decimal SafeGetCellDecimal(DataGridViewRow row, string columnName, decimal defaultValue = 0m)
        {
            return SafeToDecimal(GetCellValueSafe(row, columnName), defaultValue);
        }

        /// <summary>
        /// Safely gets int value from DataGridView cell
        /// </summary>
        public static int SafeGetCellInt32(DataGridViewRow row, string columnName, int defaultValue = 0)
        {
            return SafeToInt32(GetCellValueSafe(row, columnName), defaultValue);
        }

        /// <summary>
        /// Safely gets long value from DataGridView cell
        /// </summary>
        public static long SafeGetCellInt64(DataGridViewRow row, string columnName, long defaultValue = 0L)
        {
            return SafeToInt64(GetCellValueSafe(row, columnName), defaultValue);
        }

        /// <summary>
        /// Safely gets double value from DataGridView cell
        /// </summary>
        public static double SafeGetCellDouble(DataGridViewRow row, string columnName, double defaultValue = 0.0)
        {
            return SafeToDouble(GetCellValueSafe(row, columnName), defaultValue);
        }

        /// <summary>
        /// Safely gets DateTime value from DataGridView cell
        /// </summary>
        public static DateTime SafeGetCellDateTime(DataGridViewRow row, string columnName, DateTime? defaultValue = null)
        {
            return SafeToDateTime(GetCellValueSafe(row, columnName), defaultValue);
        }

        /// <summary>
        /// Safely gets bool value from DataGridView cell
        /// </summary>
        public static bool SafeGetCellBool(DataGridViewRow row, string columnName, bool defaultValue = false)
        {
            return SafeToBool(GetCellValueSafe(row, columnName), defaultValue);
        }

        #endregion

        #region Safe DataReader Access Methods

        /// <summary>
        /// Safely gets string value from SqlDataReader by column name
        /// </summary>
        public static string SafeGetString(SqlDataReader reader, string columnName, string defaultValue = "")
        {
            try
            {
                if (reader.HasColumn(columnName))
                {
                    int ordinal = reader.GetOrdinal(columnName);
                    if (!reader.IsDBNull(ordinal))
                    {
                        return reader.GetString(ordinal);
                    }
                }
                return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Safely gets decimal value from SqlDataReader by column name
        /// </summary>
        public static decimal SafeGetDecimal(SqlDataReader reader, string columnName, decimal defaultValue = 0m)
        {
            try
            {
                if (reader.HasColumn(columnName))
                {
                    int ordinal = reader.GetOrdinal(columnName);
                    if (!reader.IsDBNull(ordinal))
                    {
                        return reader.GetDecimal(ordinal);
                    }
                }
                return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Safely gets int value from SqlDataReader by column name
        /// </summary>
        public static int SafeGetInt32(SqlDataReader reader, string columnName, int defaultValue = 0)
        {
            try
            {
                if (reader.HasColumn(columnName))
                {
                    int ordinal = reader.GetOrdinal(columnName);
                    if (!reader.IsDBNull(ordinal))
                    {
                        return reader.GetInt32(ordinal);
                    }
                }
                return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Safely gets DateTime value from SqlDataReader by column name
        /// </summary>
        public static DateTime SafeGetDateTime(SqlDataReader reader, string columnName, DateTime? defaultValue = null)
        {
            try
            {
                if (reader.HasColumn(columnName))
                {
                    int ordinal = reader.GetOrdinal(columnName);
                    if (!reader.IsDBNull(ordinal))
                    {
                        return reader.GetDateTime(ordinal);
                    }
                }
                return defaultValue ?? DateTime.MinValue;
            }
            catch
            {
                return defaultValue ?? DateTime.MinValue;
            }
        }

        #endregion

        #region Extension Methods

        /// <summary>
        /// Extension method to check if SqlDataReader has a specific column
        /// </summary>
        public static bool HasColumn(this SqlDataReader reader, string columnName)
        {
            try
            {
                return reader.GetOrdinal(columnName) >= 0;
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
        }

        /// <summary>
        /// Extension method to safely get value from DataRow
        /// </summary>
        public static T SafeGet<T>(this DataRow row, string columnName, T defaultValue = default(T))
        {
            try
            {
                if (row.Table.Columns.Contains(columnName) && !row.IsNull(columnName))
                {
                    object value = row[columnName];
                    if (value != null && value != DBNull.Value)
                    {
                        return (T)Convert.ChangeType(value, typeof(T));
                    }
                }
                return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        #endregion

        #region Error Logging

        /// <summary>
        /// Logs database-related errors for debugging
        /// </summary>
        public static void LogDatabaseError(string operation, Exception ex)
        {
            try
            {
                // Simple file logging - you can enhance this with your preferred logging framework
                string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Database Error in {operation}: {ex.Message}\r\n{ex.StackTrace}\r\n";
                System.IO.File.AppendAllText("database_errors.log", logMessage);
            }
            catch
            {
                // Fail silently - don't let logging errors crash the application
            }
        }

        #endregion
    }
}
