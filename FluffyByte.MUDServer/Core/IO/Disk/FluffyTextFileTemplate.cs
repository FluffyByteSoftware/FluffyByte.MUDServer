namespace FluffyByte.MUDServer.Core.IO.Disk;

public sealed class FluffyTextFileTemplate : IFluffyTextFile 
{
    public Guid FileId { get; } = Guid.NewGuid();

    public string FileName { get; }

    public FileInfo FileInfo { get; }

    public List<string> Lines { get; set; } = [];

    public FluffyTextFileTemplate(string filename)
    {
        FileName = Path.GetFileName(filename);
        FileInfo = new FileInfo(FileName);
    }
    
    public async Task AppendLineAsync(string newLine)
    {
        await Task.CompletedTask;
    }

    public async Task InsertTopLineAsync(string newLine)
    {
        await Task.CompletedTask;
    }

    public async Task InsertLineAtNumberAsync(string line, int lineNumber)
    {
        await Task.CompletedTask;
    }

    public async Task RemoveLineAtNumberAsync(int lineNumber)
    {
        await Task.CompletedTask;
    }

}