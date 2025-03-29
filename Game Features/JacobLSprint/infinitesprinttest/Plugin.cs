using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using infinitesprinttest.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infinitesprinttest
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class InfiniteSprintTutorial : BaseUnityPlugin
    {
        private const string modGUID = "Poseidon.InfiniteSprintTest";
        private const string modName = "Infinite Sprint Mod Test";
        private const string modVersion = "1.41.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static InfiniteSprintTutorial instance;

        internal ManualLogSource mls;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            mls.LogInfo("The TEST MODE IS AWAKE!!! :D");

            harmony.PatchAll(typeof(InfiniteSprintTutorial));
            harmony.PatchAll(typeof(PlayerControllerPatch));

        }

    }
}
