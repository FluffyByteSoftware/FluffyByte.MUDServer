namespace FluffyByte.MUDServer.Core.FileIO
{
    public interface IFluffyFile
    {
        string Path { get; }
        FileMode Mode { get; }
        FileAccess Access { get; }
        FileShare Share { get; }
        int BufferSize { get; }
        FileOptions Options { get; }
    }

}
