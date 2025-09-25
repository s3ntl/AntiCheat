using NuclearOption.Networking;
using AntiCheat.Signals;
using AntiCheat.Utils;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace AntiCheat.Services
{
    public static class KickService
    {
        public static void Subscribe()
        {
            EventBus.Instance.Subscribe<KickSignal>(HandleKick);
            EventBus.Instance.Subscribe<HookExploitUsedSignal>(HandleHookExploit);
        }

        public static void HandleHookExploit(HookExploitUsedSignal signal)
        {
            Player target = signal.target;
            Player attacker = signal.attacker;

            Tools.KickPlayerAsync(attacker, "Hook exploit used");

            _immunePlayers.Add(target, Tuple.Create(DateTime.UtcNow, TimeSpan.FromSeconds(5)));
        }

        public static async void HandleKick(KickSignal signal)
        {
            var task = CheckImmune(signal.player);
            await task;
            if (task.Result)
            {
                Tools.KickPlayerAsync(signal.player, signal.reason);
            }
        }

        public static async Task<bool> CheckImmune(Player player)
        {
            await Task.Delay(1000);
            
            if (_immunePlayers.ContainsKey(player))
            {
                DateTime now = DateTime.UtcNow;
                if (now - _immunePlayers[player].Item1 > _immunePlayers[player].Item2)
                {
                    _immunePlayers.Remove(player);
                    return false;
                }
                return true;
            }
            return false;
        }

        public static Dictionary<Player, Tuple<DateTime, TimeSpan>> _immunePlayers = new Dictionary<Player, Tuple<DateTime, TimeSpan>>();

    }
}
