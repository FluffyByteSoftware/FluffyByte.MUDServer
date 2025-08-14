using System.Text;
using FluffyByte.MUDServer.Core.FileIO;

namespace FluffyByte.MUDServer.Utilities
{
    public static class FileWizard
    {
        public static event Action? FileChanged;
        public static event Action? FileDeleted;
        public static event Action? FileCreated;

        /// <summary>
        /// Saves the specified data to the given file asynchronously.
        /// </summary>
        /// <remarks>This method performs an atomic save operation, ensuring that the file is not left in
        /// a partially written state in case of an error or cancellation. The caller is responsible for ensuring that
        /// the <paramref name="file"/>  is writable and that sufficient storage space is available.</remarks>
        /// <param name="file">The file to which the data will be saved. Must not be <see langword="null"/>.</param>
        /// <param name="data">The data to be written to the file. This is provided as a read-only memory buffer.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. The operation will be canceled if the token is triggered.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>

        public static Task SaveAsync(IFluffyFile file, ReadOnlyMemory<byte> data,
            CancellationToken cancellationToken = default)
            => SaveAtomicAsync(file, async stream
                => await stream.WriteAsync(data, cancellationToken),
                cancellationToken);

        public static async Task SaveLinesAsync(IFluffyFile file, IEnumerable<string> lines,
            Encoding? enc = null, CancellationToken cancellationToken = default)
            => await SaveAtomicAsync(file, async stream =>
            {
                enc ??= Encoding.UTF8;

                using StreamWriter writer = new(stream, enc, file.BufferSize, true);
                foreach (string line in lines)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await writer.WriteLineAsync(line);
                }

                await writer.FlushAsync(cancellationToken);
            }, cancellationToken);

        public static async Task<byte[]?> ReadAllBytesAsync(IFluffyFile file,
            CancellationToken cancellationToken = default)
        {
            if (!File.Exists(file.Path)) return null;

            using FileStream fs = OpenForRead(file);
            byte[] buffer = new byte[fs.Length];

            int offset = 0, read;

            while ((read = await fs.ReadAsync(buffer.AsMemory(offset), cancellationToken)) > 0)
                offset += read;

            return buffer;
        }

        public static async Task<string?> ReadAllTextAsync(IFluffyFile file, Encoding? enc = null,
            CancellationToken cancellationToken = default)
        {
            if(!File.Exists(file.Path)) return null;
            
            using FileStream fs = OpenForRead(file);
            
            using StreamReader reader = new(fs, enc ?? Encoding.UTF8, detectEncodingFromByteOrderMarks: true,
                bufferSize: file.BufferSize, leaveOpen: false);

            return await reader.ReadToEndAsync(cancellationToken);
        }

        public static async IAsyncEnumerable<string> ReadLineAsync(IFluffyFile file, 
            Encoding? enc = null, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (!File.Exists(file.Path)) yield break;

            using FileStream fs = OpenForRead(file);
            using StreamReader reader = new(fs, enc ?? Encoding.UTF8, detectEncodingFromByteOrderMarks: true,
                bufferSize: file.BufferSize, leaveOpen: false);

            while (!reader.EndOfStream)
            {
                cancellationToken.ThrowIfCancellationRequested();
                string? line = await reader.ReadLineAsync(cancellationToken);

                if (line is not null) yield return line;
            }
        }

        private static FileStream OpenForRead(IFluffyFile file)
        {
            FileStreamOptions opts = new()
            {
                Mode = FileMode.Open,
                Access = FileAccess.Read,
                Share = FileShare.Read,
                BufferSize = Math.Max(1024, file.BufferSize),
                Options = file.Options | FileOptions.Asynchronous 
                | FileOptions.SequentialScan
            };

            return new FileStream(file.Path, opts);
        }

        private static async Task SaveAtomicAsync(IFluffyFile file, Func<Stream, Task> write,
            CancellationToken cancellationToken= default)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(file.Path)!);
            
            string tmp = $"{file.Path}.{Guid.NewGuid():N}.tmp";

            FileStreamOptions options = new()
            {
                Mode = FileMode.CreateNew,
                Access = FileAccess.Write,
                Share = FileShare.None,
                BufferSize = file.BufferSize,
                Options = file.Options | FileOptions.Asynchronous | FileOptions.SequentialScan
            };

            try
            {
                await using (FileStream stream = new(tmp, options))
                {
                    await write(stream);
                    await stream.FlushAsync(cancellationToken);
                }

                if (File.Exists(file.Path))
                    File.Replace(tmp, file.Path, null, ignoreMetadataErrors: true);
                else
                    File.Move(tmp, file.Path);
            }
            catch(Exception ex)
            {
                if (File.Exists(tmp))
                {
                    File.Delete(tmp);
                }

                Console.WriteLine($"Error creating file: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");

                throw;
            }
        }
    }
}
