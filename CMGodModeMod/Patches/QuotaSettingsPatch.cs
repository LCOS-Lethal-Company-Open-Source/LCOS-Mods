using GameNetcodeStuff;
using HarmonyLib;
using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMGodModeMod.Patches
{
    [HarmonyPatch(typeof(TimeOfDay), "Awake")]
    class QuotaSettingsPatch
    {
        [HarmonyPatch("update")]
        [HarmonyPostfix]

        private static void infiniteDeadlinePatch(ref TimeOfDay __instance)
        {
            __instance.quotaVariables.deadlineDaysAmount = 999;
        }

        private static void StartingCreditsPatch(ref TimeOfDay __instance)
        {
            __instance.quotaVariables.startingCredits = 10000;
        }
    }
}
