using NuclearOption.Networking;

namespace AntiCheat.Signals
{
    public class KickSignal
    {
        public readonly Player player;
        public readonly string reason;

        public KickSignal(Player player, string reason)
        {
            this.player = player;
            this.reason = reason;
        }
    }
}
