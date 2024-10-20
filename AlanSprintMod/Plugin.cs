using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LCInfSpr.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCInfSpr
{
    [BepInPlugin(modGUID,modName,modVersion)]
    public class TutMod: BaseUnityPlugin
    {
        private const string modGUID = "Alan.LCInfSpr";
        private const string modName = "Tut. Infinite sprint";
        private const string modVersion = "1.0.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static TutMod Instance;

        internal ManualLogSource logSource;

        void Awake() 
        {
            if (Instance == null) 
            { 
            Instance = this;
            }
            

            // logging
            logSource = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            

            logSource.LogInfo("Tutorial mod is active");

            // patch this into game
            harmony.PatchAll(typeof(TutMod));
            harmony.PatchAll(typeof(PlayerControllerBPatch));

        }


    }
}
