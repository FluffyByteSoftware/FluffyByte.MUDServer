using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluffyByte.Utilities.Wizards.IO;

namespace FluffyByte.Utilities.Wizards
{
    public static class Scribe
    {
        // Lock
        private static readonly Lock _lock = new();

        // Console Colors
        private static readonly ConsoleColor _errorColor = ConstellationKeeper.Instance.ErrorColor;
        private static readonly ConsoleColor _infoColor = ConstellationKeeper.Instance.InfoColor;
        private static readonly ConsoleColor _debugColor = ConstellationKeeper.Instance.DebugColor;
        private static readonly ConsoleColor _warningColor = ConstellationKeeper.Instance.WarningColor;

        // TimeStamps
        public static string TimeStampNowUtc()
        {
            return DateTime.UtcNow.ToString("yyyy-dd-MM HH:mm:ss.fff");
        }

        // Info
        public static void Info(string message) => WriteLine(IOMessageLevel.Info, message);
        public static async Task InfoAsync(string message) => await Task.Run(() => Info(message));
        
        // Warn
        public static void Warn(string message) => WriteLine(IOMessageLevel.Warn, message);
        public static async Task WarnAsync(string message) => await Task.Run(() => Warn(message));


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        public static void Warn(Exception ex)
        {
            StringBuilder sb = new();
            sb.AppendLine($"An exception occurred: {ex.Message}");
            sb.AppendLine($"StackTrace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                sb.AppendLine($"InnerException: {ex.InnerException.Message}");
                sb.AppendLine($"InnerException StackTrace: {ex.InnerException.StackTrace}");
            }

            WriteLine(IOMessageLevel.Warn, sb.ToString());
        }

        // Error
        /// <summary>
        /// Error is a method that logs an error message.
        /// </summary>
        /// <param name="message">Message to be displayed</param>
        public static void Error(string message) => WriteLine(IOMessageLevel.Error, message);
        
        /// <summary>
        /// Error is a method that logs an exception message.
        /// </summary>
        /// <param name="ex">Exception to be investigated</param>
        public static void Error(Exception ex)
        {
            StringBuilder sb = new();
            sb.AppendLine($"An exception occurred: {ex.Message}");
            sb.AppendLine($"StackTrace: {ex.StackTrace}");
            
            if (ex.InnerException != null)
            {
                sb.AppendLine($"InnerException: {ex.InnerException.Message}");
                sb.AppendLine($"InnerException StackTrace: {ex.InnerException.StackTrace}");
            }

            WriteLine(IOMessageLevel.Error, sb.ToString());
        }

        /// <summary>
        /// ErrorAsync is an asynchronous method that logs an error message.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        public static async Task ErrorAsync(string message)
            => await Task.Run(() => Error(message));

        /// <summary>
        /// ErrorAsync is an asynchronous method that logs an error message.
        /// </summary>
        /// <param name="ex"></param>
        public static async Task ErrorAsync(Exception ex)
            => await Task.Run(() => Error(ex));

        // Debug
        /// <summary>
        /// Debug is a method that logs a debug message.
        /// It is only logged if the ConstellationKeeper's DebugMode is enabled.
        /// </summary>
        /// <param name="message"></param>
        public static void Debug(string message) => WriteLine(IOMessageLevel.Debug, message);

        // WriteLine
        /// <summary>
        /// Passes the message to the IOWizard singleton to be written to the console.
        /// The message is prefixed with a timestamp and the log level.
        /// </summary>
        /// <param name="logLevel">Desired Log Level: Info, Warn, Error, Debug</param>
        /// <param name="text">The message to be logged.</param>
        /// <exception cref="ArgumentOutOfRangeException">An invalid log level was passed.</exception>
        private static void WriteLine(IOMessageLevel logLevel, string text)
        {
            ConsoleColor outputColor;
            StringBuilder sb = new();

            switch(logLevel)
            {
                case IOMessageLevel.Info:
                    sb.Append(InsertTimeStamp("[ Info ] - " + text));
                    outputColor = _infoColor;
                    break;

                case IOMessageLevel.Warn:
                    sb.Append(InsertTimeStamp("[ Warn ] - " + text));
                    outputColor = _warningColor;
                    break;

                case IOMessageLevel.Error:
                    sb.Append(InsertTimeStamp("[ Error ] - " + text));
                    outputColor = _errorColor;
                    break;

                case IOMessageLevel.Debug:
                    outputColor = ConstellationKeeper.Instance.DebugColor;

                    if (ConstellationKeeper.Instance.DebugMode)
                    {
                        sb.Append(InsertTimeStamp("[ Debug ] - " + text));
                    }
                    else
                    {
                        return; // If DebugMode is off, do not log debug messages.
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }

            // Write to console
            lock(_lock)
            {
                try
                {
                    IOWizard.Instance.WriteLine(
                        text: sb.ToString(),
                        foreGroundColor: outputColor,
                        backGroundColor: null,
                        resetColors: true);
                }
                catch(Exception ex)
                {
                    IOWizard.Instance.WriteLine(
                        $"An error occurred while writing to the console: {ex.Message}",
                        _errorColor);
                }
            }
        }

        // InsertTimeStamp
        /// <summary>
        /// Inserts a timestamp before the raw message.
        /// </summary>
        /// <param name="rawMessage">The message to be formatted.</param>
        /// <returns>[ yyyy-dd-MM HH:mm:ss.fff ] - rawMessage</returns>
        private static string InsertTimeStamp(string rawMessage)
        {
            return $"[ {TimeStampNowUtc()} ] - {rawMessage}";
        }
    }
}
