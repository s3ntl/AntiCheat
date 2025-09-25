using BepInEx;
using HarmonyLib;
using BepInEx.Logging;
using System.Collections.Generic;
using AntiCheat.Analyzers;
using System.Threading;
using UnityEngine;


namespace AntiCheat
{
    
    [BepInPlugin("AntiCheat", "AntiCheat", "1.0.0.0")]
    
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource logger;


        public void Awake()
        {
            Plugin.logger = base.Logger;
            Harmony harmony = new Harmony("AntiCheat");
            harmony.PatchAll();

            
            //ACThreadManager.RegisterAnalyzer(typeof(SpeedHackAnalyzer));
            ACThreadManager.RegisterAnalyzer(typeof(TeleportAnalyzer));
            ACThreadManager.Subscribe();
            logger.LogInfo("Anticheat loaded");
        }

       
        
        public void Update() 
        {
           ACThreadManager.Update();
        }

        public void FixedUpdate()
        {
           ACThreadManager.FixedUpdate();
        }

    }
}
