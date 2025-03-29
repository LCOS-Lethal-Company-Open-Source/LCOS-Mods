using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCTutoritalMod
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class TutorialModBase : BaseUnityPlugin
    {
        private const string modGUID = "M-Partridge.LCTutorialMod";
        private const string modName = "LC Tutorial Mod";
        private const string modVersion = "1.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static TutorialModBase instance;

        internal ManualLogSource mls;

        void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            mls.LogInfo(modName + " has started");

            harmony.PatchAll();
        }
    }
}
