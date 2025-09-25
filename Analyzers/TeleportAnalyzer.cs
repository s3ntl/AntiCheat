using System;
using AntiCheat.Signals;
using AntiCheat.Utils;
using NuclearOption.Networking;

namespace AntiCheat.Analyzers
{
    public class TeleportAnalyzer : Analyzer
    {
        
        public override int KickCode { get; } = 2;

        public override string Name { get; } = "teleport";

        public override Player Player { get; set; }

        public PlayerData PreviousFrameData { get; private set; }

        public DateTime SpawnTime { get; private set; }

        public override void Analyze()
        {
            PlayerData currentFrameData = ACThreadManager.GetPlayerData(Player);

            if (currentFrameData.isFlying == false || PreviousFrameData.isFlying == false) 
            { 
                PreviousFrameData = currentFrameData; 
                return; 
            }

            float speed = PreviousFrameData.velocity.sqrMagnitude;
            float distanceBetweenFrames = (PreviousFrameData.position - currentFrameData.position).sqrMagnitude;

            if (distanceBetweenFrames > speed * 1.8 + 10 && (DateTime.UtcNow - SpawnTime > TimeSpan.FromSeconds(2)) && distanceBetweenFrames < 200)
            {
                Plugin.logger.LogInfo($"Player {Player} suspected of using a teleporter!");
            }
            PreviousFrameData = currentFrameData;
        }

        public override void SetupAnalyzer(Player player)
        {
            base.SetupAnalyzer(player);
            EventBus.Instance.Subscribe<PlayerAircraftRegisteredSignal>(HandlePlayerAircraftRegister);
        }

        private void HandlePlayerAircraftRegister(PlayerAircraftRegisteredSignal signal)
        {
            if (signal.player == this.Player) SpawnTime = DateTime.UtcNow;
        }
    }
}
