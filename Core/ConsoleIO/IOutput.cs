using System.Text;

namespace FluffyByte.MUDServer.Core.ConsoleIO
{
    /// <summary>
    /// An interface for writing output to a destination, such as a console or network stream.
    /// </summary>
    public interface IOutput
    {
        /// <summary>
        /// Gets the character encoding used for reading and writing text.
        /// </summary>
        Encoding Encoding { get; }

        /// <summary>
        /// Writes a message to the console asynchronously with the specified foreground color.
        /// </summary>
        /// <param name="message">The message to write to the console. Cannot be null or empty.</param>
        /// <param name="foregroundColor">The color to use for the message text. Defaults to <see cref="ConsoleColor.Gray"/>.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the operation to complete. Defaults to <see
        /// cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask"/> that represents the asynchronous write operation.</returns>
        ValueTask WriteAsync(string message,
            ConsoleColor foregroundColor = ConsoleColor.Gray,
            CancellationToken ct = default);

        /// <summary>
        /// Writes a message to the console asynchronously, with optional foreground color customization.
        /// </summary>
        /// <param name="message">The message to write to the console. Defaults to an empty string if not specified.</param>
        /// <param name="foregroundColor">The color of the text to display in the console. Defaults to <see cref="ConsoleColor.Gray"/>.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the operation to complete. Defaults to <see
        /// cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask"/> that represents the asynchronous write operation.</returns>
        ValueTask WriteLineAsync(string message = "",
            ConsoleColor foregroundColor = ConsoleColor.Gray,
            CancellationToken ct = default);

        /// <summary>
        /// Writes a message to the console with the specified foreground color.
        /// </summary>
        /// <remarks>The method writes the message to the console using the specified foreground color. 
        /// If the <paramref name="message"/> is null or empty, the behavior is undefined.</remarks>
        /// <param name="message">The message to be written to the console. Cannot be null or empty.</param>
        /// <param name="foreGroundColor">The color of the text to be displayed. Defaults to <see cref="ConsoleColor.Gray"/> if not specified.</param>
        void Write(string message,
            ConsoleColor foreGroundColor = ConsoleColor.Gray);

        /// <summary>
        /// Writes a message to the console with the specified foreground color.
        /// </summary>
        /// <param name="message">The message to write to the console. If not specified, an empty string is written.</param>
        /// <param name="foregroundColor">The color of the text to display in the console. Defaults to <see cref="ConsoleColor.Gray"/>.</param>
        void WriteLine(string message = "",
            ConsoleColor foregroundColor = ConsoleColor.Gray);

        /// <summary>
        /// Asynchronously flushes any buffered data to the underlying storage or stream.
        /// </summary>
        /// <remarks>This method ensures that any data buffered in memory is written to the underlying
        /// storage or stream. If the operation is canceled via the provided <paramref name="ct"/>, the returned task
        /// will be in a canceled state.</remarks>
        /// <param name="ct">A <see cref="CancellationToken"/> that can be used to cancel the flush operation.</param>
        /// <returns>A <see cref="ValueTask"/> that represents the asynchronous flush operation.</returns>
        ValueTask FlushAsync(CancellationToken ct = default);
    }
}
