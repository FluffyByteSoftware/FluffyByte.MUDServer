using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluffyByte.MUDServer.Utilities
{
    public class ConstellationKeeper
    {
        private static readonly Lazy<ConstellationKeeper> _instance = new(() => new());
        public static ConstellationKeeper Instance => _instance.Value;

        private ConstellationKeeper()
        {

        }
    }
}
