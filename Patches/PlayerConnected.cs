using System;
using HarmonyLib;
using NuclearOption.Networking;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(MessageManager))]
    public class PlayerConnected
    {
        public static Action<Player> OnPlayerConnected;
        [HarmonyPatch("JoinMessage")]
        [HarmonyPrefix]
        public static void JoinMessagePrefix(Player joinedPlayer)
        {
            if (OnPlayerConnected != null)
            {
                
                OnPlayerConnected.Invoke(joinedPlayer);
               
            }
        }
    }
}
