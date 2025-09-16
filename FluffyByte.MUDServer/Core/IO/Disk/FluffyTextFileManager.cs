using System.Text;

namespace FluffyByte.MUDServer.Core.IO.Disk;

public static class FluffyTextFileManager
{
    public static async Task<IFluffyTextFile?> TryLoadFileAsync(string filePath, Encoding? encoding = null)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return null;

        encoding ??= Encoding.UTF8;

        try
        {
            if (!File.Exists(filePath))
                return null;

            var lines = await File.ReadAllLinesAsync(filePath, encoding);
            var fluffyFile = new FluffyTextFile(filePath, lines.ToList());
            
            return fluffyFile;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or DirectoryNotFoundException)
        {
            return null;
        }
    }

    public static IFluffyTextFile? LoadFile(string filePath, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;

        try
        {
            if (!File.Exists(filePath))
                return null;

            var lines = File.ReadAllLines(filePath, encoding);
            var fluffyFile = new FluffyTextFile(filePath, lines.ToList());
            
            return fluffyFile;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or DirectoryNotFoundException)
        {
            return null;
        }
    }
    
    public static async Task<bool> TrySaveFileAsync(IFluffyTextFile fluffyFile, Encoding? encoding = null,
        bool createDirectory = true)
    {
        encoding ??= Encoding.UTF8;

        try
        {
            if (createDirectory)
            {
                var directory = Path.GetDirectoryName(fluffyFile.FileInfo.FullName);
                
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }

            await File.WriteAllLinesAsync(fluffyFile.FileInfo.FullName, fluffyFile.Lines, encoding);
            
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or DirectoryNotFoundException)
        {
            return false;
        }
    }

    public static bool SaveFile(IFluffyTextFile fluffyFile, Encoding? encoding = null, bool createDirectory = true)
    {
        encoding ??= Encoding.UTF8;

        try
        {
            File.WriteAllLines(fluffyFile.FileInfo.FullName, fluffyFile.Lines.ToArray(), encoding);
            
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or DirectoryNotFoundException)
        {
            return false;
        }
    }

    private static bool FileExists(string fileName)
    {
        return File.Exists(fileName);
    }

    public static void DeleteFile(IFluffyTextFile fluffyFile)
    {
        if (!FileExists(fluffyFile.FileInfo.FullName))
            return;

        File.Delete(fluffyFile.FileInfo.FullName);
    }

    public static void MoveFile(IFluffyTextFile fluffyFile, string newFileName)
    {
        if (FileExists(newFileName))
        {
            return;
        }

        File.Move(fluffyFile.FileInfo.FullName, newFileName);
    }
}