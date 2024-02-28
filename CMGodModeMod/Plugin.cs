using BepInEx;
using BepInEx.Logging;
using CMGodModeMod.Patches;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMGodModeMod
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class GodModeModBase : BaseUnityPlugin
    {
        private const string modGUID = "Christianm.CMGodModeMod";
        private const string modName = "God Mode Mod";
        private const string modVersion = "1.0.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static GodModeModBase Instance;

        internal ManualLogSource mls;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            mls.LogInfo("GOD MOD IS ACTIVE!");

            harmony.PatchAll(typeof(GodModeModBase));
            harmony.PatchAll(typeof(PlayerControllerBPatch));
            harmony.PatchAll(typeof(QuotaSettingsPatch));
        }
    }
}
