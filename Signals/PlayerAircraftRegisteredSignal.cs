using NuclearOption.Networking;

namespace AntiCheat.Signals
{
    public class PlayerAircraftRegisteredSignal
    {
        public readonly Aircraft aircraft;
        public readonly Player player;

        public PlayerAircraftRegisteredSignal(Aircraft aircraft, Player player)
        {
            this.aircraft = aircraft;
            this.player = player;
        }
    }
}
