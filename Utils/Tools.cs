using NuclearOption.Networking;
using Cysharp.Threading.Tasks;
using Mirage;


namespace AntiCheat.Utils
{
    public static class Tools
    {
        public static async UniTaskVoid KickPlayerAsync(Player player, string reason)
        {
            
            if (!NetworkManagerNuclearOption.i.Server.Active)
            {
                throw new MethodInvocationException("KickPlayerAsync called when server is not active");
            }

            INetworkPlayer conn = player.Owner;
            NetworkManagerNuclearOption.i.Authenticator.OnKick(conn);
            
            Player localPlayer;
            string hostName = (GameManager.GetLocalPlayer<Player>(out localPlayer) ? localPlayer.PlayerName : "server");
            player.KickReason(reason, hostName);
            await UniTask.Delay(100);
            conn.Disconnect();
        }
    }
}
