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

        private static float scrapSpawnMul;
        private static float scrapSpawnMulLower; // minimum value for min and max scrap multiplier
        private static float scrapSpawnMulUpper; // maximum value for min and max scrap multiplier
        private static bool scrapSpawnMulEnabled;

        private static float scrapValueMul;
        private static float scrapValueMulLower; // minimum value for scrap value multiplier
        private static float scrapValueMulUpper; // maximum value for scrap value multiplier
        private static bool scrapValueMulEnabled;

        public RoundManagerPatch()
        {
            scrapSpawnMul = 0;
            scrapSpawnMulLower = Config.Instance.scrapSpawnMulLower.Value;
            scrapSpawnMulUpper = Config.Instance.scrapSpawnMulUpper.Value;
            scrapSpawnMulEnabled = Config.Instance.scrapSpawnMulEnabled.Value;
            if (scrapSpawnMulLower > scrapSpawnMulUpper || !Config.Instance.scrapSpawnMulEnabled.Value)
            {
                scrapSpawnMulLower = 1;
                scrapSpawnMulUpper = 1;
            }

            scrapValueMul = 0;
            scrapValueMulLower = Config.Instance.scrapValueMulLower.Value;
            scrapValueMulUpper = Config.Instance.scrapValueMulUpper.Value;
            scrapValueMulEnabled = Config.Instance.scrapValueMulEnabled.Value;
            if (scrapValueMulLower > scrapValueMulUpper || !Config.Instance.scrapValueMulEnabled.Value)
            {
                scrapValueMulLower = 1;
                scrapValueMulUpper = 1;
            }
        }



        [HarmonyPatch(nameof(RoundManager.SpawnScrapInLevel))]
        [HarmonyPrefix]
        static void RandomizeSpawnScrapPatch(ref RoundManager __instance)
        {
            random = new Random();

            // Apply scrap spawn multiplier
            if(scrapSpawnMul == 0)
            {
                scrapSpawnMul = __instance.scrapAmountMultiplier * (scrapSpawnMulLower + (float)(random.NextDouble() * (scrapSpawnMulUpper - scrapSpawnMulLower)));
            }
            __instance.scrapAmountMultiplier = scrapSpawnMul;

            // Apply scrap value multiplier
            if(scrapValueMul == 0)
            {
                scrapValueMul = __instance.scrapValueMultiplier * (scrapValueMulLower + (float)(random.NextDouble() * (scrapValueMulUpper - scrapValueMulLower)));
            }
            __instance.scrapValueMultiplier = scrapValueMul;
        }
    }
}
