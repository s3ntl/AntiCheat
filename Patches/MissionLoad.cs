using System;
using HarmonyLib;
using NuclearOption.SavedMission;


namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(Mission))]
    public static class MissionLoad
    {
        public static Action OnMissionLoad;
        [HarmonyPatch("AfterLoad", typeof(MissionKey))]
        [HarmonyPrefix] 
        public static void Prefix(Mission __instance, MissionKey key)
        {
            if (OnMissionLoad != null)
            {
                OnMissionLoad.Invoke();
            }
        }
    }
}
