using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluffyByte.Utilities.Wizards
{
    public sealed class ConstellationKeeper
    {
        private static readonly Lazy<ConstellationKeeper> _instance = new(() => new());
        public static ConstellationKeeper Instance => _instance.Value;

        private ConstellationKeeper()
        {
        }

        public ConsoleColor WarningColor { get; private set; } = ConsoleColor.Yellow;
        public ConsoleColor ErrorColor { get; private set; } = ConsoleColor.Red;
        public ConsoleColor InfoColor { get; private set; } = ConsoleColor.Gray;
        public ConsoleColor DebugColor { get; private set; } = ConsoleColor.Green;

        public bool DebugMode { get; set; } = true;
    }
}
