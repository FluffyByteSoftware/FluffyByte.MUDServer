using FluffyByte.MUDServer.Utilities;

namespace FluffyByte.MUDServer.Core.Boot
{
    public class  Grub
    {
        public static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                Scribe.Write(LogLevel.Info, "Test");
            }

            Scribe.Info("Info");
            Scribe.Debug("Debug");
            Scribe.Warn("Warn");
            Scribe.Error("Error");
        }
    }
}