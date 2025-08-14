using System.Linq.Expressions;
using System.Text;
using System.Threading.Channels;

namespace FluffyByte.MUDServer.Core.ConsoleIO
{
    /// <summary>
    /// Thread-safe console I/O bridge with an input pump, colorized output, and async-friendly APIs.
    /// </summary>
    public sealed class ConsoleWizard : IInput, IOutput, IAsyncDisposable, IDisposable
    {
        #region Singleton
        private static readonly Lazy<ConsoleWizard> _instance = new(() => new ConsoleWizard());
        public static ConsoleWizard Instance => _instance.Value;
        private ConsoleWizard()
        {
            _inPumpTask = Task.Run(PumpInputAsync);
        }
        #endregion

        /// <summary>
        /// Gets or sets a value indicating whether leading and trailing whitespace  should be removed 
        /// from input strings.
        /// </summary>
        public bool TrimInput { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether empty lines should be ignored during processing.
        /// </summary>
        public bool IgnoreEmptyLines { get; set; } = true;

        // Events (lightweight; fire only from the pump / write paths to avoid duplicates)
        public event Action<string>? OnInputReceived;
        public event Action<string, ConsoleColor>? OnOutputWritten;

        // State
        private readonly CancellationTokenSource _cts = new();
        
        private readonly Channel<string> _inQueue = Channel.CreateUnbounded<string>(new UnboundedChannelOptions
        {
            SingleReader = false,   // allow multiple awaiting readers
            SingleWriter = true,
            AllowSynchronousContinuations = false
        });

        private readonly Task _inPumpTask;
        private readonly SemaphoreSlim _writeGate = new(1, 1);
        private volatile bool _disposed;
        public bool IsDisposed => _disposed;

        #region IInput
        /// <summary>
        /// Gets the character encoding used for input and output operations.
        /// </summary>
        public Encoding Encoding { get; } = Console.OutputEncoding;

        /// <summary>
        /// Asynchronously reads the next line of input from the internal queue.
        /// </summary>
        /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the operation to complete. The default value
        /// is <see langword="default"/>.</param>
        /// <returns>A <see cref="ValueTask{TResult}"/> representing the asynchronous operation. The result contains the next
        /// line of input as a string,  or <see langword="null"/> if the input channel is closed and no more data is
        /// available.</returns>
        public async ValueTask<string?> ReadLineAsync(CancellationToken ct = default)
        {
            try
            {
                return await _inQueue.Reader.ReadAsync(ct).ConfigureAwait(false);
            }
            catch (ChannelClosedException)
            {
                return null;
            }
        }

        /// <summary>
        /// Attempts to read a line of text from the input queue.
        /// </summary>
        /// <remarks>This method is non-blocking and will return immediately. If no line is available in
        /// the input queue,  the method returns <see langword="false"/> and the <paramref name="line"/> parameter is
        /// set to <see langword="null"/>.</remarks>
        /// <param name="line">When this method returns, contains the line of text read from the input queue,  or <see langword="null"/> if
        /// no line was available. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true"/> if a line of text was successfully read; otherwise, <see langword="false"/>.</returns>
        public bool TryReadLine(out string? line)
            => _inQueue.Reader.TryRead(out line);
        #endregion

        #region IOutput

        /// <summary>
        /// Writes the specified message to the output asynchronously with the specified foreground color.
        /// </summary>
        /// <param name="message">The message to write. Cannot be <see langword="null"/>.</param>
        /// <param name="foregroundColor">The color to use for the message text. Defaults to <see cref="ConsoleColor.Gray"/>.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the operation to complete. Defaults to <see
        /// cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask"/> that represents the asynchronous write operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="message"/> is <see langword="null"/>.</exception>
        public ValueTask WriteAsync(string message,
            ConsoleColor foregroundColor = ConsoleColor.Gray,
            CancellationToken ct = default)
        {
            if(message is null)
                throw new ArgumentNullException(nameof(message), "Message cannot be null.");

            return new(WriteCoreAsync(message, foregroundColor, appendNewLine: false, ct));
        }

        /// <summary>
        /// Writes a message to the console asynchronously, followed by a newline.
        /// </summary>
        /// <param name="message">The message to write. Cannot be <see langword="null"/>. Defaults to an empty string.</param>
        /// <param name="foregroundColor">The color of the text to display in the console. Defaults to <see cref="ConsoleColor.Gray"/>.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the operation to complete. Defaults to <see
        /// cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask"/> that represents the asynchronous write operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="message"/> is <see langword="null"/>.</exception>
        public ValueTask WriteLineAsync(string message = "",
            ConsoleColor foregroundColor = ConsoleColor.Gray,
            CancellationToken ct = default)
        {
            if(message is null)
                throw new ArgumentNullException(nameof(message), "Message cannot be null.");

            return new(WriteCoreAsync(message, foregroundColor, appendNewLine: true, ct));
        }

        /// <summary>
        /// Writes a message to the console with the specified color.
        /// </summary>
        /// <param name="message">The message to write. Cannot be <see langword="null"/>.</param>
        /// <param name="consoleColor">The color to use for the message text. Defaults to <see cref="ConsoleColor.Gray"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="message"/> is <see langword="null"/>.</exception>
        public void Write(string message, ConsoleColor consoleColor = ConsoleColor.Gray)
        { 
            if(message is null) 
                throw new ArgumentNullException(nameof(message), "Message cannot be null.");

            WriteCoreAsync(message, consoleColor, appendNewLine: false, CancellationToken.None).
                GetAwaiter().GetResult();
        }

        /// <summary>
        /// Writes a message to the console with the specified color, followed by a newline.
        /// </summary>
        /// <param name="message">The message to write. Cannot be <see langword="null"/>. Defaults to an empty string.</param>
        /// <param name="consoleColor">The color to use for the message text. Defaults to <see cref="ConsoleColor.Gray"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="message"/> is <see langword="null"/>.</exception>
        public void WriteLine(string message = "", ConsoleColor consoleColor = ConsoleColor.Gray)
        {
            if(message is null)
                throw new ArgumentNullException(nameof(message), "Message cannot be null.");

            WriteCoreAsync(message, consoleColor, appendNewLine: true, CancellationToken.None).
                GetAwaiter().GetResult();
        }

        /// <summary>
        /// Flushes any buffered data to the underlying output stream asynchronously.
        /// </summary>
        /// <remarks>This method ensures that all buffered data is written to the underlying output
        /// stream.  If the operation is canceled via the provided <paramref name="ct"/>, the returned task will be in a
        /// canceled state.</remarks>
        /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the operation to complete. The default value
        /// is <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask"/> that represents the asynchronous flush operation.</returns>
        public async ValueTask FlushAsync(CancellationToken ct = default)
        {
            await Console.Out.FlushAsync(ct).ConfigureAwait(false);
        }
        #endregion

        #region Internals
        /// <summary>
        /// Continuously reads input lines from the console and processes them based on the configured options.
        /// </summary>
        /// <remarks>This method reads lines asynchronously from the console input stream until
        /// cancellation is requested  or the end of the input stream is reached. Each line is optionally trimmed and
        /// filtered based on the  <see cref="TrimInput"/> and <see cref="IgnoreEmptyLines"/> properties. Processed
        /// lines are then written  to an internal queue and the <see cref="OnInputReceived"/> event is invoked for each
        /// line.</remarks>
        /// <returns></returns>
        private async Task PumpInputAsync()
        {
            try
            {
                while (!_cts.IsCancellationRequested)
                {
                    string? line = await Console.In.ReadLineAsync(_cts.Token).ConfigureAwait(false);
                    
                    if (line is null)
                        break; // EOF

                    if (TrimInput)
                        line = line.Trim();

                    if (IgnoreEmptyLines && line.Length == 0)
                        continue;

                    // Fire event here so both async and sync consumers are notified exactly once.
                    _inQueue.Writer.TryWrite(line);

                    try
                    {
                        OnInputReceived?.Invoke(line);
                    }
                    catch { }
                }
            }
            catch (OperationCanceledException)
            {
                // Normal during shutdown.
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in input pump: {ex.Message}");
            }
            finally
            {
                _inQueue.Writer.TryComplete();
            }
        }

        /// <summary>
        /// Writes a message to the console with the specified color and optional newline, asynchronously.
        /// </summary>
        /// <param name="message">The message to write to the console. Cannot be null or empty.</param>
        /// <param name="color">The <see cref="ConsoleColor"/> to use when writing the message.</param>
        /// <param name="appendNewLine">A value indicating whether to append a newline after the message.  <see langword="true"/> to append a
        /// newline; otherwise, <see langword="false"/>.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous write operation.</returns>
        private Task WriteCoreAsync(string message, ConsoleColor color, bool appendNewLine,
            CancellationToken ct)
            => WriteCoreImplAsync(message, color, appendNewLine, ct);

        /// <summary>
        /// Writes a message to the console with the specified color and optional newline, ensuring thread-safe access.
        /// </summary>
        /// <remarks>This method ensures that console writes are thread-safe by using a synchronization
        /// mechanism.  If the console output is redirected, the color settings will not be applied.  Any exceptions
        /// thrown by subscribers to the <c>OnOutputWritten</c> event are caught and ignored.</remarks>
        /// <param name="message">The message to write to the console. Cannot be <see langword="null"/>.</param>
        /// <param name="color">The <see cref="ConsoleColor"/> to use for the message text.</param>
        /// <param name="appendNewline"><see langword="true"/> to append a newline after the message; otherwise, <see langword="false"/>.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the operation to complete.</param>
        /// <returns></returns>
        private async Task WriteCoreImplAsync(string message, ConsoleColor color, 
            bool appendNewline, CancellationToken ct)
        {
            await _writeGate.WaitAsync(ct).ConfigureAwait(false);

            Action<string, ConsoleColor>? onWritten = OnOutputWritten;

            Console.ResetColor();

            try
            {
                if (!Console.IsOutputRedirected)
                {
                    Console.ForegroundColor = color;
                }

                await Console.Out.WriteAsync(message.AsMemory(), ct).ConfigureAwait(false);

                if (appendNewline)
                {
                    await Console.Out.WriteAsync(Environment.NewLine.AsMemory(), ct).ConfigureAwait(false);
                }

                await Console.Out.FlushAsync(ct).ConfigureAwait(false);
            }
            catch(OperationCanceledException)
            {
                throw;
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine($"Error writing to console: {ex.Message}");
            }
            finally
            {
                Console.ResetColor();

                _writeGate.Release();
            }

            try
            {
                onWritten?.Invoke(message + (appendNewline ? Environment.NewLine : string.Empty), color);
            }
            catch { /* swallow subscriber exceptions */ }
        }
        #endregion

        #region Disposal

        /// <summary>
        /// Releases all resources used by the current instance of the class.
        /// </summary>
        /// <remarks>This method synchronously disposes of resources. For asynchronous disposal, use <see
        /// cref="DisposeAsync"/>. Once disposed, the instance cannot be used again.</remarks>
        public void Dispose()
        {
            if (_disposed) return;

            DisposeAsync().AsTask().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Asynchronously releases the resources used by the instance.
        /// </summary>
        /// <remarks>This method cancels any ongoing operations, waits for the input processing task to
        /// complete,  and disposes of internal resources. It should be called when the instance is no longer needed  to
        /// ensure proper cleanup of resources.</remarks>
        /// <returns>A <see cref="ValueTask"/> that represents the asynchronous dispose operation.</returns>
        public async ValueTask DisposeAsync()
        {

            if (_disposed) return;

            _disposed = true;

            _cts.Cancel(); // Signal the input pump to stop

            try
            {
                await _inPumpTask.ConfigureAwait(false);
            }
            catch(OperationCanceledException)
            {
                // Expected during shutdown
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error during input pump shutdown: {ex.Message}");
            }
            finally
            {
                _cts.Dispose();
                _writeGate.Dispose();
                _inQueue.Writer.TryComplete();
            }
        }

        /// <summary>
        /// Throws an <see cref="ObjectDisposedException"/> if the current instance has been disposed.
        /// </summary>
        /// <remarks>This method should be called by other members of the class to ensure that operations 
        /// are not performed on a disposed instance. If the instance has been disposed, an  <see
        /// cref="ObjectDisposedException"/> is thrown.</remarks>
        /// <exception cref="ObjectDisposedException">Thrown if the current instance has already been disposed.</exception>
        private void ThrowIfDisposed()
        {
            if(_disposed)
                throw new ObjectDisposedException(nameof(ConsoleWizard), 
                    "This instance has already been disposed.");
        }
        #endregion
    }
}
