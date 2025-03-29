using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LCTutorialMod.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCTutorialMod
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class TutorialModBase : BaseUnityPlugin
    {
        private const string modGUID = "Poseidon.CMLCTutorialMod";
        private const string modName = "LC Tutorial Mod";
        private const string modVersion = "1.0.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static TutorialModBase Instance;

        internal ManualLogSource mls;


        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            mls.LogInfo("The test mod as awoken :)");

            harmony.PatchAll(typeof(TutorialModBase));
            harmony.PatchAll(typeof(PlayerControllerBPatch));
        }

    }
}
