using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FluffyByte.MUDServer.Core.IO;

public interface IFluffyTextFile 
{
    Guid FileId { get; }

    string FileName { get; }

    FileInfo FileInfo { get; }

    List<string> Lines { get; set; }
    
    Task AppendLineAsync(string newLine);
    Task InsertTopLineAsync(string newLine);
    Task InsertLineAtNumberAsync(string line, int lineNumber);
    Task RemoveLineAtNumberAsync(int lineNumber);
}