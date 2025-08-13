// FluffyByte.Utilities.Wizards.IOWizard.cs
// Written by: Jacob Chacko
// The IOWizard is a wrapper around the console input and output operations.

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Channels;
using FluffyByte.Utilities.Wizards.IO;

namespace FluffyByte.Utilities.Wizards
{
    public sealed class IOWizard : IIOWizard
    {
        #region Singleton
        private static readonly Lazy<IOWizard> _instance = new(() => new IOWizard());
        public static IOWizard Instance => _instance.Value;
        private IOWizard()
        {

        }
        #endregion

        #region Variables
        private readonly CancellationTokenSource _cts = new();
        private readonly Lock _gate = new();

        public event Action<string, ConsoleColor?, ConsoleColor?>? OnWrite;
        public event Action<string, ConsoleColor?, ConsoleColor?>? OnWriteLine;
        public event Action<string>? OnReadLine;
        #endregion

        #region Public Methods
        // Outputs
        /// <summary>
        /// Displays text in the console with optional foreground and background colors.
        /// </summary>
        /// <param name="text">Text to be displayed</param>
        /// <param name="foreGroundColor">ConsoleColor for foreground</param>
        /// <param name="backGroundColor">ConsoleColor for background</param>
        /// <param name="resetColors">Boolean to reset at the end of the line.</param>
        public void Write(string text, 
            ConsoleColor? foreGroundColor = null,
            ConsoleColor? backGroundColor = null, 
            bool resetColors = true)
        {
            lock(_gate)
            {
                if (foreGroundColor.HasValue)
                {
                    Console.ForegroundColor = foreGroundColor.Value;
                }
                
                if (backGroundColor.HasValue)
                {
                    Console.BackgroundColor = backGroundColor.Value;
                }

                Console.Write(text);
                
                OnWrite?.Invoke(text, foreGroundColor, backGroundColor);
                
                if (resetColors)
                {
                    Console.ResetColor();
                }
            }
        }

        /// <summary>
        /// Displays text in the console with optional foreground and background colors, 
        /// followed by a new line.
        /// </summary>
        /// <param name="text">Text to be displayed</param>
        /// <param name="foreGroundColor">ConsoleColor for foreground</param>
        /// <param name="backGroundColor">ConsoleColor for background</param>
        /// <param name="resetColors">Boolean to reset at the end of the line.</param>
        public void WriteLine(string text, 
            ConsoleColor? foreGroundColor = null,
            ConsoleColor? backGroundColor = null, 
            bool resetColors = true)
        {
            lock(_gate)
            {
                if (foreGroundColor.HasValue)
                {
                    Console.ForegroundColor = foreGroundColor.Value;
                }
                
                if (backGroundColor.HasValue)
                {
                    Console.BackgroundColor = backGroundColor.Value;
                }
                Console.WriteLine(text);
                
                OnWriteLine?.Invoke(text, foreGroundColor, backGroundColor);
                
                if (resetColors)
                {
                    Console.ResetColor();
                }
            }
        }

        /// <summary>
        /// Clears the console screen.
        /// </summary>
        public void ClearScreen()
        {
            lock(_gate)
            {
                Console.Clear();
            }
        }

        /// <summary>
        /// Clears the console output, but does not clear the screen.
        /// </summary>
        public void Clear()
        {
            lock(_gate)
            {
                Console.Clear();
            }
        }

        /// <summary>
        /// Sets the console window title.
        /// </summary>
        public void SetTitle(string consoleTitle)
        {
            lock(_gate)
            {
                Console.Title = consoleTitle;
            }
        }

        // Inputs
        /// <summary>
        /// Reads a line of input from the console.
        /// </summary>
        /// <returns>Text inputted</returns>
        public string ReadLine()
        {
            string? raw = Console.ReadLine();
            string normalized = string.IsNullOrWhiteSpace(raw) ? "\n" : raw!;
            OnReadLine?.Invoke(normalized);

            return normalized;
        }
        #endregion

        /// <summary>
        /// Disposes the IOWizard, cancelling any ongoing operations 
        /// and releasing resources.
        /// </summary>
        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
            _gate.Exit();
        }
    }
}
