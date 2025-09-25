using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuclearOption.Networking;
namespace AntiCheat.Analyzers
{
    public abstract class Analyzer : IAnalyzer
    {
        public abstract void Analyze();

        public abstract int KickCode { get; }

        public abstract string Name { get; }

        public abstract Player Player { get; set; }

        public virtual void SetupAnalyzer(Player player)
        {
            Player = player;
        }
    }
}
