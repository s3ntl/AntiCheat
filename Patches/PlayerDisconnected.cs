using System;
using HarmonyLib;
using NuclearOption.Networking;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(MessageManager), "DisconnectedMessage")]
    public class PlayerDisconnected
    {
        public static Action<Player> OnPlayerDisconnected;
        [HarmonyPrefix]
        public static void DisconnectedMessagePrefix(Player player)
        {
            if (OnPlayerDisconnected != null)
            {
                OnPlayerDisconnected.Invoke(player);
            }
        }
    }
}
