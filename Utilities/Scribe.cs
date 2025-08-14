using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FluffyByte.MUDServer.Core.ConsoleIO;

namespace FluffyByte.MUDServer.Utilities
{
    public enum LogLevel
    {
        Debug = 0,
        Info = 1,
        Warning = 2,
        Error = 3
    }

    /// <summary>
    /// Provides a centralized logging utility for writing messages to the console with various log levels.
    /// </summary>
    /// <remarks>The <see cref="Scribe"/> class offers methods for logging messages with different severity
    /// levels,  including Debug, Info, Warning, and Error. Each log level is associated with a specific console color 
    /// for better visibility. The class also supports asynchronous logging and provides events for monitoring  when
    /// messages are written. Debug messages are ignored if debug mode is disabled.</remarks>
    public static class Scribe
    {
        private static bool _debugMode = true;
        private static readonly ConsoleColor _defaultColor = ConsoleColor.Gray;
        private static readonly ConsoleColor _debugColor = ConsoleColor.Cyan;
        private static readonly ConsoleColor _infoColor = ConsoleColor.Green;
        private static readonly ConsoleColor _warningColor = ConsoleColor.Yellow;
        private static readonly ConsoleColor _errorColor = ConsoleColor.Red;

        public static event Action? DebugMessageWritten;
        public static event Action? InfoMessageWritten;
        public static event Action? WarningMessageWritten;
        public static event Action? ErrorMessageWritten;

        #region Write Methods

        /// <summary>
        /// Writes a log message to the console with the specified log level and formatting.
        /// </summary>
        /// <remarks>Debug messages are ignored if debug mode is disabled. The console output color is
        /// determined by the specified <paramref name="logLevel"/>. After writing the message, the console color is
        /// reset.</remarks>
        /// <param name="logLevel">The severity level of the log message. Determines the color of the output.</param>
        /// <param name="message">The message to be logged. Cannot be null or empty.</param>
        public static void Write(LogLevel logLevel, string message)
        {
            if (logLevel == LogLevel.Debug && !_debugMode)
            {
                return; // Skip debug messages if debug mode is off
            }

            ConsoleColor outputColor = logLevel switch
            {
                LogLevel.Debug => _debugColor,
                LogLevel.Info => _infoColor,
                LogLevel.Warning => _warningColor,
                LogLevel.Error => _errorColor,
                _ => _defaultColor,
            };

            Console.ForegroundColor = outputColor;
            ConsoleWizard.Instance.WriteLine($"[{TimestampInUtc()}] - {message}");
            Console.ResetColor();
        }

        /// <summary>
        /// Asynchronously writes a log entry with the specified log level and message.
        /// </summary>
        /// <param name="logLevel">The severity level of the log entry.</param>
        /// <param name="message">The message to be logged. Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        public static async Task WriteAsync(LogLevel logLevel, string message)
            => await Task.Run(() => Write(logLevel, message));
        
        #endregion

        #region Debug Methods
        /// <summary>
        /// Logs a debug message to the console if debug mode is enabled.
        /// </summary>
        /// <remarks>The message is formatted with a debug-specific prefix and displayed in the configured
        /// debug color. This method has no effect if debug mode is disabled.</remarks>
        /// <param name="message">The message to log. Cannot be null or empty.</param>
        public static void Debug(string message)
        {
            if (!_debugMode) return;

            string formattedMessage = FormatOutputMessage(message, LogLevel.Debug);

            DebugMessageWritten?.Invoke();
            ConsoleWizard.Instance.WriteLine(formattedMessage, _debugColor);
        }

        /// <summary>
        /// Executes a debug operation asynchronously by logging the specified message.
        /// </summary>
        /// <param name="message">The message to be logged. Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous debug operation.</returns>
        public static async Task DebugAsync(string message)
            => await Task.Run(() => Debug(message));

        #endregion

        #region Info Methods
        /// <summary>
        /// Logs an informational message to the console with a predefined format and color.
        /// </summary>
        /// <remarks>The message is formatted with the <see cref="LogLevel.Info"/> level and displayed
        /// using a predefined color. This method is intended for general informational messages that do not indicate
        /// errors or warnings.</remarks>
        /// <param name="message">The message to log. Cannot be null or empty.</param>
        public static void Info(string message)
        {
            string formattedMessage = FormatOutputMessage(message, LogLevel.Info);

            InfoMessageWritten?.Invoke();
            ConsoleWizard.Instance.WriteLine(formattedMessage, _infoColor);
        }

        /// <summary>
        /// Logs an informational message asynchronously.
        /// </summary>
        /// <param name="message">The message to log. Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous logging operation.</returns>
        public static async Task InfoAsync(string message)
            => await Task.Run(() => Info(message));

        #endregion

        #region Warn Methods
        /// <summary>
        /// Logs a warning message to the console with a predefined warning color.
        /// </summary>
        /// <remarks>The message is formatted with the warning log level before being displayed.  The
        /// output is styled using a predefined warning color for better visibility.</remarks>
        /// <param name="message">The warning message to be logged. Cannot be null or empty.</param>
        public static void Warn(string message)
        {
            string formattedMessage = FormatOutputMessage(message, LogLevel.Warning);

            WarningMessageWritten?.Invoke();

            ConsoleWizard.Instance.WriteLine(formattedMessage, _warningColor);
        }

        /// <summary>
        /// Logs a warning message containing details about the specified exception, including the caller's context.
        /// </summary>
        /// <remarks>This method captures contextual information about the exception, including the
        /// caller's name, file path, and line number, and logs it as a warning. The exception's message, stack trace,
        /// and any inner exception details are included in the log output.</remarks>
        /// <param name="ex">The exception to log. Cannot be <see langword="null"/>.</param>
        /// <param name="caller">The name of the calling member. This parameter is automatically populated by the compiler.</param>
        /// <param name="filePath">The file path of the source code where the method was called. This parameter is automatically populated by
        /// the compiler.</param>
        /// <param name="lineNumber">The line number in the source code where the method was called. This parameter is automatically populated by
        /// the compiler.</param>
        public static void Warn(Exception ex, 
            [CallerMemberName] string caller = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0
            )
        {
            StringBuilder output = new();

            output.AppendLine($"Exception in {caller} at {filePath}:{lineNumber} - {ex.Message}");
            output.AppendLine($"Stack Trace: {ex.StackTrace}");
            output.AppendLine($"Inner Exception: {ex.InnerException?.Message}");
            output.AppendLine($"Inner Stack Trace: {ex.InnerException?.StackTrace}");
            
            Warn(FormatOutputMessage(output.ToString(), LogLevel.Warning));
        }

        /// <summary>
        /// Logs a warning message asynchronously.
        /// </summary>
        /// <remarks>This method executes the logging operation on a separate thread to avoid blocking the
        /// caller. Ensure that the <paramref name="message"/> parameter is properly validated before calling this
        /// method.</remarks>
        /// <param name="message">The warning message to log. Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async Task WarnAsync(string message)
            => await Task.Run(() => Warn(message));

        /// <summary>
        /// Logs a warning message asynchronously, including details about the exception and the caller's context.
        /// </summary>
        /// <param name="ex">The exception to log. Cannot be <see langword="null"/>.</param>
        /// <param name="caller">The name of the calling member. This is automatically populated by the compiler.</param>
        /// <param name="filePath">The full file path of the source code file containing the caller. This is automatically populated by the
        /// compiler.</param>
        /// <param name="lineNumber">The line number in the source code file at which the method was called. This is automatically populated by
        /// the compiler.</param>
        /// <returns>A task that represents the asynchronous logging operation.</returns>
        public static async Task WarnAsync(Exception ex,
            [CallerMemberName] string caller = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
            => await Task.Run(() => Warn(ex, caller, filePath, lineNumber));
        #endregion


        #region Error Methods
        /// <summary>
        /// Logs an error message with a predefined error format and color.
        /// </summary>
        /// <param name="message">The error message to log. Cannot be null or empty.</param>
        public static void Error(string message)
        {
            string formattedMessage = FormatOutputMessage(message, LogLevel.Error);
            ConsoleWizard.Instance.WriteLine(formattedMessage, _errorColor);
        }

        /// <summary>
        /// Logs detailed information about an exception, including the caller's context and stack trace.
        /// </summary>
        /// <remarks>This method captures and logs detailed exception information, including the exception
        /// message, stack trace,  and any inner exception details. It also includes contextual information about the
        /// caller, such as the method name,  file path, and line number, to aid in debugging.</remarks>
        /// <param name="ex">The exception to log. Cannot be <see langword="null"/>.</param>
        /// <param name="caller">The name of the method or property that invoked this method. This parameter is automatically populated by
        /// the compiler.</param>
        /// <param name="filePath">The full path of the source file that contains the caller. This parameter is automatically populated by the
        /// compiler.</param>
        /// <param name="lineNumber">The line number in the source file at which this method was called. This parameter is automatically
        /// populated by the compiler.</param>
        public static void Error(Exception ex,
            [CallerMemberName] string caller = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            StringBuilder output = new();

            output.AppendLine($"Exception in {caller} at {filePath}:{lineNumber} - {ex.Message}");
            output.AppendLine($"Stack Trace: {ex.StackTrace}");
            output.AppendLine($"Inner Exception: {ex.InnerException?.Message}");
            output.AppendLine($"Inner Stack Trace: {ex.InnerException?.StackTrace}");

            ErrorMessageWritten?.Invoke();
            Error(FormatOutputMessage(output.ToString(), LogLevel.Error));
        }

        /// <summary>
        /// Logs an error message asynchronously.
        /// </summary>
        /// <remarks>This method schedules the logging operation on a background thread.  Use this method
        /// when you need non-blocking error logging.</remarks>
        /// <param name="message">The error message to log. Cannot be null or empty.</param>
        public static void ErrorAsync(string message)
            => Task.Run(() => Error(message));

        /// <summary>
        /// Logs an exception asynchronously, including contextual information about the caller.
        /// </summary>
        /// <param name="ex">The exception to log. This parameter cannot be <see langword="null"/>.</param>
        /// <param name="caller">The name of the calling member. This is automatically populated by the compiler if not explicitly provided.</param>
        /// <param name="filePath">The full file path of the source code file containing the caller. This is automatically populated by the
        /// compiler if not explicitly provided.</param>
        /// <param name="lineNumber">The line number in the source code file where the method was called. This is automatically populated by the
        /// compiler if not explicitly provided.</param>
        /// <returns>A task that represents the asynchronous logging operation.</returns>
        public static async Task ErrorAsync(Exception ex,
            [CallerMemberName] string caller = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
            => await Task.Run(() => Error(ex, caller, filePath, lineNumber));
        #endregion

        #region Other Methods
        /// <summary>
        /// Formats a log message by combining a timestamp, log level, and input message.
        /// </summary>
        /// <param name="input">The message to be included in the log output.</param>
        /// <param name="logLevel">The severity level of the log message.</param>
        /// <returns>A formatted string containing the timestamp, log level, and input message.</returns>
        private static string FormatOutputMessage(string input, LogLevel logLevel)
        {
            StringBuilder output = new();

            output.Append($"[{TimestampInUtc()} - {logLevel.ToString()}] - {input}");

            return output.ToString();
        }

        /// <summary>
        /// Generates a timestamp string representing the current date and time in UTC.
        /// </summary>
        /// <remarks>The returned timestamp is in Coordinated Universal Time (UTC) and uses a fixed format
        /// of "yyyy-MM-dd HH:mm:ss.fff", where: - "yyyy" is the four-digit year. - "MM" is the two-digit month. - "dd"
        /// is the two-digit day. - "HH" is the two-digit hour in 24-hour format. - "mm" is the two-digit minute. - "ss"
        /// is the two-digit second. - "fff" is the three-digit millisecond.</remarks>
        /// <returns>A string formatted as "yyyy-MM-dd HH:mm:ss.fff" that represents the current UTC date and time.</returns>
        private static string TimestampInUtc() => DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");

        /// <summary>
        /// Enables or disables debug mode for the application.
        /// </summary>
        /// <param name="debugMode">A value indicating whether debug mode should be enabled.  <see langword="true"/> enables debug mode; <see
        /// langword="false"/> disables it.</param>
        public static void SetDebugMode(bool debugMode)
        {
            _debugMode = debugMode;
        }

        #endregion
    }
}
