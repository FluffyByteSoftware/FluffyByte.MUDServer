using FluffyByte.Utilities.Wizards;

namespace FluffyByte.MUDServer
{
    public class Program()
    {
        /// <summary>
        /// This is the main entry point for the application.
        /// </summary>
        /// <param name="args">Arguments to be passed at the 
        /// command line when executing the exe.</param>
        public static void Main(string[] args)
        {
            if(args.Length > 0)
            {
                Scribe.Debug("Command line arguments provided:");
                
                foreach (string arg in args)
                {
                    Scribe.Debug(arg);
                }
            }
                        
            Scribe.Info("Starting pre-boot up operations...");
        }
    }
}