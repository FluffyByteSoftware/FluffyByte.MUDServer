namespace FluffyByte.MUDServer.Core.IO.Disk;

public sealed class FluffyTextFile : IFluffyTextFile
{
    private readonly object _lineLock = new();

    public Guid FileId { get; } = Guid.NewGuid();
    public string FileName { get; }
    public FileInfo FileInfo { get; }
    public List<string> Lines { get; set; }

    public FluffyTextFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

        FileName = Path.GetFileName(filePath);
        FileInfo = new FileInfo(filePath);
        Lines = new List<string>();
    }

    public FluffyTextFile(string filePath, List<string> lines)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

        FileName = Path.GetFileName(filePath);
        FileInfo = new FileInfo(filePath);
        Lines = lines;
    }

    public async Task AppendLineAsync(string newLine)
    {
        await Task.Run(() =>
        {
            lock (_lineLock)
            {
                Lines.Add(newLine);
            }
        });
    }

    public async Task InsertTopLineAsync(string newLine)
    {
        await Task.Run(() =>
        {
            lock (_lineLock)
            {
                Lines.Insert(0, newLine);
            }
        });
    }

    public async Task InsertLineAtNumberAsync(string line, int lineNumber)
    {
        await Task.Run(() =>
        {
            lock (_lineLock)
            {
                if (lineNumber < 0)
                    lineNumber = 0;
                else if (lineNumber > Lines.Count)
                    lineNumber = Lines.Count;

                Lines.Insert(lineNumber, line);
            }
        });
    }

    public async Task RemoveLineAtNumberAsync(int lineNumber)
    {
        await Task.Run(() =>
        {
            lock (_lineLock)
            {
                if (lineNumber >= 0 && lineNumber < Lines.Count)
                {
                    Lines.RemoveAt(lineNumber);
                }
            }
        });
    }
    
    public void AppendLine(string newLine)
    {
        lock (_lineLock)
        {
            Lines.Add(newLine);
        }
    }
    public void InsertTopLine(string newLine)
    {
        lock (_lineLock)
        {
            Lines.Insert(0, newLine);
        }
    }
    
    public void InsertLineAtNumber(string line, int lineNumber)
    {
        lock (_lineLock)
        {
            if (lineNumber < 0)
                lineNumber = 0;
            else if (lineNumber > Lines.Count)
                lineNumber = Lines.Count;

            Lines.Insert(lineNumber, line);
        }
    }

    public void RemoveLineAtNumber(int lineNumber)
    {
        lock (_lineLock)
        {
            if (lineNumber >= 0 && lineNumber < Lines.Count)
            {
                Lines.RemoveAt(lineNumber);
            }
        }
    }
    
    /// <summary>
    /// Gets the total number of lines in the file
    /// </summary>
    public int LineCount
    {
        get
        {
            lock (_lineLock)
            {
                return Lines.Count;
            }
        }
    }

    /// <summary>
    /// Gets a line at the specified index safely
    /// </summary>
    /// <param name="lineNumber">Zero-based line number</param>
    /// <returns>Line content or null if index is out of range</returns>
    public string? GetLine(int lineNumber)
    {
        lock (_lineLock)
        {
            return lineNumber >= 0 && lineNumber < Lines.Count ? Lines[lineNumber] : null;
        }
    }

    /// <summary>
    /// Replaces a line at the specified index
    /// </summary>
    /// <param name="lineNumber">Zero-based line number</param>
    /// <param name="newLine">New line content</param>
    /// <returns>True if line was replaced, false if index was out of range</returns>
    public bool ReplaceLine(int lineNumber, string newLine)
    {
        lock (_lineLock)
        {
            if (lineNumber < 0 || lineNumber >= Lines.Count) return false;
            
            Lines[lineNumber] = newLine;
            
            return true;
        }
    }

    /// <summary>
    /// Clears all lines from the file
    /// </summary>
    public async Task ClearAsync()
    {
        await Task.Run(() =>
        {
            lock (_lineLock)
            {
                Lines.Clear();
            }
        });
    }

    /// <summary>
    /// Gets all lines as a read-only copy
    /// </summary>
    /// <returns>Read-only list of lines</returns>
    public IReadOnlyList<string> GetAllLines()
    {
        lock (_lineLock)
        {
            return Lines.ToList().AsReadOnly();
        }
    }

    public override string ToString()
    {
        return $"FluffyTextFile: {FileName} ({LineCount} lines)";
    }
}