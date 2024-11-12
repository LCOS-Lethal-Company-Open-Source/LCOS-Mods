using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Configuration;

namespace RandomCompany.Patches
{
    [HarmonyPatch(typeof(RoundManager))]
    internal class RoundManagerPatch
    {
        private static System.Random random;
        private static float scrapSpawnLower; // minimum value for min and max scrap multiplier
        private static float scrapSpawnUpper; // maximum value for min and max scrap multiplier
        private static float scrapValueLower = .1f; // minimum value for scrap value multiplier
        private static float scrapValueUpper = 10f; // maximum value for scrap value multiplier

        public RoundManagerPatch()
        {
            scrapSpawnLower = RandomCompany.cfg.scrapSpawnLower.Value;
            scrapSpawnUpper = RandomCompany.cfg.scrapSpawnUpper.Value;
            if (scrapSpawnLower > scrapSpawnUpper || !RandomCompany.cfg.scrapSpawnRateEnabled.Value)
            {
                scrapSpawnLower = 1;
                scrapSpawnUpper = 1;
            }

            scrapValueLower = RandomCompany.cfg.scrapValueLower.Value;
            scrapValueUpper = RandomCompany.cfg.scrapValueUpper.Value;
            if (scrapValueLower > scrapValueUpper || !RandomCompany.cfg.scrapValueEnabled.Value)
            {
                scrapValueLower = 1;
                scrapValueUpper = 1;
            }
        }



        [HarmonyPatch(nameof(RoundManager.SpawnScrapInLevel))]
        [HarmonyPrefix]
        static void RandomizeSpawnScrapPatch(ref RoundManager __instance)
        {
            random = new Random();
            double scrapSpawnMul = scrapSpawnLower + (random.NextDouble() * (scrapSpawnUpper - scrapSpawnLower));
            string message = "scrapAmountMultiplier changed from " + __instance.scrapAmountMultiplier + " to ";
            __instance.scrapAmountMultiplier *= (float)scrapSpawnMul;
            message += __instance.scrapAmountMultiplier;
            RandomCompany.logMessage(message);

            float scrapValueMul = (float)(scrapValueLower + (random.NextDouble() * (scrapValueUpper - scrapValueLower)));
            message = "scrapValueMultiplier changed from " + __instance.scrapValueMultiplier + " to ";
            __instance.scrapValueMultiplier *= scrapValueMul;
            message += __instance.scrapValueMultiplier;
            RandomCompany.logMessage(message);
        }
    }
}
