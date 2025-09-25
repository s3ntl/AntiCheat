using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using AntiCheat.Analyzers;
using UnityEngine;
using NuclearOption.Networking;
using AntiCheat.Utils;

namespace AntiCheat
{
    public static class ACThreadManager
    {
        private static int threadSpaceLimit = 2; // how many players thread will be analyze

        public static int threadCount = 0; // debug info 

        private static ConcurrentDictionary<Thread, Tuple<CancellationTokenSource, ConcurrentBag<Player>>> threadPool =
            new ConcurrentDictionary<Thread, Tuple<CancellationTokenSource, ConcurrentBag<Player>>>(); //да, это пиздец

        private static Dictionary<Player, List<Analyzer>> _analyzerPool = new Dictionary<Player, List<Analyzer>>();

        private static HashSet<Type> _registeredAnalyzers = new HashSet<Type>(); // analyzers
        
        private static Dictionary<Player, PlayerData> _playerDataMap = new Dictionary<Player, PlayerData>();
        
        public static void Update()
        {
            foreach (Player player in _analyzerPool.Keys)
            {
                Aircraft playerAicraft = player.Aircraft;
                if (playerAicraft != null)
                {
                    _playerDataMap[player] = new PlayerData
                    {
                        position = playerAicraft.transform.position,
                        quaternion = playerAicraft.transform.rotation,
                        velocity = playerAicraft.rb.velocity,
                        isFlying = true
                    };
                    
                }
                else
                {
                    _playerDataMap[player] = new PlayerData
                    {
                        position = Vector3.zero,
                        velocity = Vector3.zero,
                        isFlying = false
                    };
                }
                PlayerData data = _playerDataMap[player];
               // Plugin.logger.LogInfo($"position {data.position}, isflying {data.isFlying}");
            }
        }
        public static PlayerData GetPlayerData(Player player)
        {
            PlayerData data = _playerDataMap[player];
           // Plugin.logger.LogInfo($"thread {Thread.CurrentThread.Name} getting player data: pos {data.position.ToString()}, isflying {data.isFlying}");
            return data;
        }
        public static void FixedUpdate()
        {
            
        }
        public static void Subscribe()
        {
            Patches.PlayerConnected.OnPlayerConnected += OnPlayerConnected;
            Patches.PlayerDisconnected.OnPlayerDisconnected += OnPlayerDisconnected;
            Patches.MissionLoad.OnMissionLoad += Clear;
            Plugin.logger.LogInfo("Subscribed");
        }

        private static void Clear()
        {
            threadPool.Clear();
            _analyzerPool.Clear();
            _playerDataMap.Clear();
        }

        public static void RegisterAnalyzer(Type analyzerType)
        {
            if (!typeof(IAnalyzer).IsAssignableFrom(analyzerType))
            {
                throw new ArgumentException("Type is not implementing interface IAnalyzer");
            }
            _registeredAnalyzers.Add(analyzerType);
        }

        private static void CreateAnalyzerPool(Player player)
        {
            if (player != null && !_analyzerPool.ContainsKey(player))
            {
                List<Analyzer> analyzerList = new List<Analyzer>();
                foreach (var analyzerType in _registeredAnalyzers)
                {
                    var instance = Activator.CreateInstance(analyzerType) as Analyzer;
                    if (instance != null)
                    {
                        instance.SetupAnalyzer(player);
                        analyzerList.Add(instance);
                        Plugin.logger.LogInfo($"created analyzer: {instance.Name}");
                    }
                }
                _analyzerPool.Add(player, analyzerList);
            }
        }
        
        private static void ThreadJob(CancellationToken token)
        {
            Thread thread = Thread.CurrentThread;
            ConcurrentBag<Player> currentPlayerPool = threadPool[thread].Item2;
            //Plugin.logger.LogInfo("thread job");
            while (!token.IsCancellationRequested)
            {
                foreach (Player player in currentPlayerPool.ToArray())
                {
                    foreach (Analyzer analyzer in _analyzerPool[player])
                    {
                        analyzer.Analyze();
                    }
                }
                //Plugin.logger.LogInfo($"thread {thread.Name} analyzed frame");
                //Thread.Sleep(TimeSpan.FromSeconds(UnityEngine.Time.fixedDeltaTime));
            }
        }

        private static void AddPlayerToThread(Player player)
        {
            foreach (Tuple<CancellationTokenSource, ConcurrentBag<Player>> tuple in threadPool.Values)
            {
                if (tuple.Item2.Count < threadSpaceLimit)
                {
                    tuple.Item2.Add(player);
                    Plugin.logger.LogInfo($"Player {player.PlayerName} added to thread");
                    return;
                }
            }

            Plugin.logger.LogInfo($"creating new thread and adding player {player.PlayerName}");
            CancellationTokenSource cts = new CancellationTokenSource();
            Thread thread = new Thread(() => ThreadJob(cts.Token));

            thread.Name = threadCount.ToString();
            threadCount++;

            threadPool.TryAdd(thread, Tuple.Create(cts, new ConcurrentBag<Player>() { player }));
            thread.Start();
        }

        private static void RemovePlayerComponent(Player player)
        {
            foreach (Tuple<CancellationTokenSource, ConcurrentBag<Player>> tuple in threadPool.Values)
            {
                tuple.Item2.TryTake(out _); 
            }

            _analyzerPool.Remove(player);
            _playerDataMap.Remove(player);
        }

        private static void CancelThread(Thread thread)
        {
            if (threadPool.TryGetValue(thread, out var tuple))
            {
                tuple.Item1.Cancel(); 
                thread.Join(); 
                threadPool.TryRemove(thread, out _); 
            }
        }

        private static void CheckEmptyThreads()
        {
            foreach(Thread thread in threadPool.Keys)
            {
                Tuple<CancellationTokenSource, ConcurrentBag<Player>> tuple = threadPool[thread];
                if (tuple.Item2.Count == 0)
                {
                    Plugin.logger.LogInfo("Empty thread founded, trying to cancel");
                    CancelThread(thread);
                }
            }
        }
        
        private static void CreatePlayerDataMap(Player player)
        {
            _playerDataMap.Add(player, new PlayerData { isFlying = false });
        }
        private static void OnPlayerConnected(Player player)
        {
            CreateAnalyzerPool(player);
            CreatePlayerDataMap(player);
            AddPlayerToThread(player);
        }

        private static void OnPlayerDisconnected(Player player)
        {
            RemovePlayerComponent(player);
            CheckEmptyThreads();
        }
    }
}