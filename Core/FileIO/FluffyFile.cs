namespace FluffyByte.MUDServer.Core.FileIO
{
    public sealed record FluffyFile(string Path,
        FileMode Mode = FileMode.OpenOrCreate,
        FileAccess Access = FileAccess.ReadWrite,
        FileShare Share = FileShare.Read,
        int BufferSize = 8192,
        FileOptions Options = FileOptions.Asynchronous | FileOptions.SequentialScan) : IFluffyFile;
}
