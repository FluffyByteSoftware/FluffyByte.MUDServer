using System.IO.Enumeration;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;
using FluffyByte.FileIO;
using FluffyByte.Logger;
using FluffyByte.FileIO.Interfaces;

using FluffyByte.MUDServer.Utilities;
using System.Runtime.CompilerServices;

namespace FluffyByte.MUDServer.Core.Boot
{
    public class Grub
    {
        public static async Task Main(string[] args)
        {
            if(args.Length == 0)
            {
                
            }

            Scribe.Info("Info");
            Scribe.Debug("Debug");
            Scribe.Warn("Warn");
            Scribe.Error("Error");

            FluffyFile test = new(@"E:\Temp\test.txt");

            string content = await FluffyTextReader.ReadAllTextAsync(test);

            if(content is null)
            {
                Scribe.Info("Failed to load content.");
            }
            else
            {
                Scribe.Info("Content: ");
                Scribe.Info(content);
            }

        }
    }
}