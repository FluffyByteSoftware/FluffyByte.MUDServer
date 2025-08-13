using System;

namespace FluffyByte.Utilities.Wizards.IO
{
    public interface IIOWizard : IDisposable
    {
        // Output
        void Write(string text, ConsoleColor? foreGroundColor = null,
            ConsoleColor? backGroundColor = null,
            bool resetColors = true);
        void WriteLine(string text, ConsoleColor? foreGroundColor = null,
            ConsoleColor? backGroundColor = null,
            bool resetColors = true);

        void ClearScreen();
        void Clear();
        void SetTitle(string consoleTitle);

        // Input
        string ReadLine();
        //Task<HashCode> ReadPasswordAsync(CancellationToken ct = default);

        // Events
        event Action<string, ConsoleColor?, ConsoleColor?>? OnWrite;
        event Action<string, ConsoleColor?, ConsoleColor?>? OnWriteLine;
        event Action<string>? OnReadLine;
    }
}