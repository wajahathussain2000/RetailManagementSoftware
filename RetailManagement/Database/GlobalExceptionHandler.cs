using System;
using System.IO;
using System.Windows.Forms;

namespace RetailManagement.Database
{
    /// <summary>
    /// Global exception handler to catch unhandled exceptions and prevent application crashes
    /// </summary>
    public static class GlobalExceptionHandler
    {
        private static readonly string LogFilePath = Path.Combine(Application.StartupPath, "error_log.txt");

        /// <summary>
        /// Initialize global exception handling
        /// </summary>
        public static void Initialize()
        {
            // Handle Windows Forms unhandled exceptions
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;
            
            // Handle non-UI thread exceptions
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        /// <summary>
        /// Handle exceptions from UI thread
        /// </summary>
        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            HandleException(e.Exception, "UI Thread Exception");
        }

        /// <summary>
        /// Handle exceptions from non-UI threads
        /// </summary>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException((Exception)e.ExceptionObject, "Non-UI Thread Exception");
        }

        /// <summary>
        /// Central exception handling logic
        /// </summary>
        private static void HandleException(Exception ex, string source)
        {
            try
            {
                // Log the exception
                LogException(ex, source);

                // Determine if this is a critical error or recoverable
                if (IsCriticalError(ex))
                {
                    ShowCriticalErrorMessage(ex);
                }
                else
                {
                    ShowRecoverableErrorMessage(ex);
                }
            }
            catch (Exception logEx)
            {
                // If logging fails, show a basic message
                MessageBox.Show(
                    $"A critical error occurred and could not be logged:\n{ex.Message}\n\nLogging Error: {logEx.Message}",
                    "Critical Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Log exception details to file
        /// </summary>
        private static void LogException(Exception ex, string source)
        {
            string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {source}\n" +
                             $"Exception Type: {ex.GetType().Name}\n" +
                             $"Message: {ex.Message}\n" +
                             $"Stack Trace: {ex.StackTrace}\n";

            if (ex.InnerException != null)
            {
                logEntry += $"Inner Exception: {ex.InnerException.Message}\n" +
                           $"Inner Stack Trace: {ex.InnerException.StackTrace}\n";
            }

            logEntry += new string('-', 80) + "\n\n";

            File.AppendAllText(LogFilePath, logEntry);
        }

        /// <summary>
        /// Determine if an error is critical (should close application)
        /// </summary>
        private static bool IsCriticalError(Exception ex)
        {
            return ex is OutOfMemoryException ||
                   ex is StackOverflowException ||
                   ex is AccessViolationException ||
                   ex is InvalidProgramException;
        }

        /// <summary>
        /// Show message for critical errors
        /// </summary>
        private static void ShowCriticalErrorMessage(Exception ex)
        {
            string message = "A critical error has occurred and the application must close.\n\n" +
                           $"Error: {ex.Message}\n\n" +
                           "The error has been logged for review. Please contact technical support.";

            MessageBox.Show(message, "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Application.Exit();
        }

        /// <summary>
        /// Show message for recoverable errors
        /// </summary>
        private static void ShowRecoverableErrorMessage(Exception ex)
        {
            string message = GetUserFriendlyMessage(ex);
            
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Convert technical exceptions to user-friendly messages
        /// </summary>
        private static string GetUserFriendlyMessage(Exception ex)
        {
            switch (ex)
            {
                case InvalidCastException _ when ex.Message.Contains("DBNull"):
                    return "Some data could not be loaded properly. The operation has been completed with default values.";
                
                case ArgumentException _ when ex.Message.Contains("Column") && ex.Message.Contains("cannot be found"):
                    return "The data structure has changed. Please refresh the view and try again.";
                
                case System.Data.SqlClient.SqlException sqlEx:
                    return GetSqlExceptionMessage(sqlEx);
                
                case UnauthorizedAccessException _:
                    return "You do not have permission to perform this operation.";
                
                case FileNotFoundException _:
                    return "A required file could not be found. Please check your installation.";
                
                case System.Net.NetworkInformation.NetworkInformationException _:
                    return "Network connection issue. Please check your internet connection.";
                
                default:
                    return $"An error occurred while processing your request.\n\nError: {ex.Message}\n\nThe error has been logged for review.";
            }
        }

        /// <summary>
        /// Convert SQL exceptions to user-friendly messages
        /// </summary>
        private static string GetSqlExceptionMessage(System.Data.SqlClient.SqlException sqlEx)
        {
            switch (sqlEx.Number)
            {
                case 2: // Timeout
                    return "The database operation timed out. Please try again.";
                case 18456: // Login failed
                    return "Database connection failed. Please check your credentials.";
                case 547: // Foreign key constraint
                    return "This record cannot be deleted because it is referenced by other data.";
                case 2627: // Primary key violation
                    return "A record with this information already exists.";
                case 8152: // String truncation
                    return "The data you entered is too long for this field.";
                default:
                    return $"A database error occurred: {sqlEx.Message}\n\nThe error has been logged for review.";
            }
        }

        /// <summary>
        /// Public method to manually log and handle exceptions
        /// </summary>
        public static void HandleManualException(Exception ex, string operation)
        {
            LogException(ex, $"Manual Exception - {operation}");
            ShowRecoverableErrorMessage(ex);
        }
    }
}
