using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using testLethalCompany.myPatches;

namespace forLearning
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class learningClass : BaseUnityPlugin
    {
        private const string modGUID = "BastedEggs.forLearning";
        private const string modName = "forLearning";
        private const string modVersion = "1.0.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static learningClass Instance;

        internal ManualLogSource mls;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            mls.LogInfo("The test mod has awaken!!!!!");

            harmony.PatchAll(typeof(learningClass));
            harmony.PatchAll(typeof(playerControllerBPatch));


        }


    }
}
