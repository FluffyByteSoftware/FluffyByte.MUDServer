using System.IO.Enumeration;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;
using FluffyByte.MUDServer.Core.FileIO;
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

        }
    }
}