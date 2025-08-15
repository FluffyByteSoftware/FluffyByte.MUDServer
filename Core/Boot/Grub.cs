using System.IO.Enumeration;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;
using FluffyByte.FileIO;
using FluffyByte.Logger;

using FluffyByte.MUDServer.Utilities;

namespace FluffyByte.MUDServer.Core.Boot
{
    public class Grub
    {
        public static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                
            }

            Scribe.Info("Info");
            Scribe.Debug("Debug");
            Scribe.Warn("Warn");
            Scribe.Error("Error");

            CancellationToken ct = new();

            FluffyFile? temp = FileWizard.LoadFluffyFile(@"E:\Temp\test.txt", FileAccess.ReadWrite, createIfMissing: true, cancellationToken: ct).Result;

            if(temp is not null)
            {
                Scribe.Info("Loaded file.");
                Scribe.Info($"Path: {temp.Path}");
                Scribe.Info($"Mode: {temp.Mode}");
                Scribe.Info($"Last Modified: {File.GetLastWriteTime(temp.Path)}");


                byte[]? contents = FileWizard.ReadAllBytesAsync(temp, ct).Result;

                if (contents is not null)
                {
                    Scribe.Info("File contents read successfully.");
                    Scribe.Info($"Contents Length: {contents.Length} bytes");
                    Scribe.Info($"Scribe Contents: {Encoding.UTF8.GetString(contents)}");
                }
            }
            else
            {
                Scribe.Error("Failed to load file.");
            }
        }
    }
}