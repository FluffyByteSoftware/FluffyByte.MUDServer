namespace FluffyByte.MUDServer.Core.IO.Disk;

public interface IFluffyTextFile 
{
    Guid FileId { get; }

    string FileName { get; }

    FileInfo FileInfo { get; }

    List<string> Lines { get; set; }
    
    /// <summary>
    /// Gets the total number of lines in the file
    /// </summary>
    int LineCount { get; }
    
    Task AppendLineAsync(string newLine);
    Task InsertTopLineAsync(string newLine);
    Task InsertLineAtNumberAsync(string line, int lineNumber);
    Task RemoveLineAtNumberAsync(int lineNumber);

    void AppendLine(string newLine);
    void InsertTopLine(string newLine);
    void InsertLineAtNumber(string line, int lineNumber);
    void RemoveLineAtNumber(int lineNumber);
    
    /// <summary>
    /// Gets a line at the specified index safely
    /// </summary>
    /// <param name="lineNumber">Zero-based line number</param>
    /// <returns>Line content or null if index is out of range</returns>
    string? GetLine(int lineNumber);
    
    /// <summary>
    /// Replaces a line at the specified index
    /// </summary>
    /// <param name="lineNumber">Zero-based line number</param>
    /// <param name="newLine">New line content</param>
    /// <returns>True if line was replaced, false if index was out of range</returns>
    bool ReplaceLine(int lineNumber, string newLine);
    
    /// <summary>
    /// Clears all lines from the file
    /// </summary>
    Task ClearAsync();
    
    /// <summary>
    /// Gets all lines as a read-only copy
    /// </summary>
    /// <returns>Read-only list of lines</returns>
    IReadOnlyList<string> GetAllLines();
}