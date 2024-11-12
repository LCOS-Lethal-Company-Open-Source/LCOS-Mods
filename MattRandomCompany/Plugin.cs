using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Configuration;
using RandomCompany.Patches;

namespace RandomCompany
{
    /*
     * What is randomized?
     *  Scrap spawn rates (.5x to 2x scrap)
     *  Scrap values
     * 
     */

    [BepInPlugin(modGUID, modName, modVersion)]
    public class RandomCompany : BaseUnityPlugin
    {
        private const string modGUID = "M-Partridge.RandomCompany";
        private const string modName = "Random Company";
        private const string modVersion = "1.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static RandomCompany instance;
        private RoundManagerPatch rmp;
        private PlayerControllerBPatch pcbp;

        internal static Config cfg { get; private set; } = null;
        internal ManualLogSource mls;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            cfg = new Config(base.Config);

            rmp = new RoundManagerPatch();
            pcbp = new PlayerControllerBPatch();


            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            mls.LogInfo(modName + " has started");

            harmony.PatchAll();

            

        }
        public static void logMessage(string message)
        {
            instance.mls.LogInfo(modName + " : " + message);
        }
    }
}
