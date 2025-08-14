namespace FluffyByte.MUDServer.Core.ConsoleIO
{
    /// <summary>
    /// An interface for reading input from a source, such as a console or network stream.
    /// </summary>
    public interface IInput
    {
        /// <summary>
        /// Asynchronously reads the next line of text from the input stream.
        /// </summary>
        /// <remarks>This method reads a line of text terminated by a newline character or the end of the
        /// input stream.  The returned string does not include the newline character.</remarks>
        /// <param name="ct">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
        /// <returns>A <see cref="ValueTask{TResult}"/> representing the asynchronous operation.  The result contains the next
        /// line of text, or <see langword="null"/> if the end of the input stream is reached.</returns>
        ValueTask<string?> ReadLineAsync(CancellationToken ct = default);

        /// <summary>
        /// Attempts to read a single line of text.
        /// </summary>
        /// <param name="line">When this method returns, contains the line of text read, or <see langword="null"/> if no line was
        /// available.</param>
        /// <returns><see langword="true"/> if a line of text was successfully read; otherwise, <see langword="false"/>.</returns>
        bool TryReadLine(out string? line);
    }
}
