using AntiCheat.Signals;
using AntiCheat.Utils;
using HarmonyLib;
namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(UnitRegistry), "RegisterUnit")]
    public class UnitRegistered
    {
        public static void Prefix(Unit unit, PersistentID id)
        {
            if (unit is Aircraft)
            {
                Aircraft aircraft = (Aircraft)unit;
                if (aircraft.Player != null)
                {
                    EventBus.Instance.Invoke<PlayerAircraftRegisteredSignal>(new PlayerAircraftRegisteredSignal(aircraft, aircraft.Player));    
                }
            }
        }
    }
}
