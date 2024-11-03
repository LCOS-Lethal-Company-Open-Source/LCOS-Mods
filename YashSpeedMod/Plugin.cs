using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using YashSpeedMod.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCModTest
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class TutorialModBase : BaseUnityPlugin

    {
        private const string modGUID = "YashSpeedMod";
        private const string modName = "Speed Mod";
        private const string modVersion = "1.0.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static TutorialModBase Instance;

        internal ManualLogSource mls;

        void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            mls.LogInfo("The test mod has awoken");

            harmony.PatchAll(typeof(TutorialModBase));
            harmony.PatchAll(typeof(PlayerControllerBPatch));
        }
    }

    

    
}
