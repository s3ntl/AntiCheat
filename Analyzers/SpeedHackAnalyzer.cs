using NuclearOption.Networking;

namespace AntiCheat.Analyzers
{
    public class SpeedHackAnalyzer : Analyzer
    {
        public override int KickCode { get; } = 1;

        public override string Name { get; } = "speedhack";

        public override Player Player { get; set; }

        

        public override void Analyze()
        {
           // Plugin.logger.LogInfo("speedhack analyzer");
           // not implemented yet
        }
    }
}
