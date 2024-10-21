using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testLethalCompany.myPatches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class playerControllerBPatch
    {
       [HarmonyPatch("Update")] //or HarmonyPatch(nameof(PlayerControllerB.update())) RECOMMENDED
       [HarmonyPostfix]
       static void infiniteSprintPatch(ref float ___sprintMeter)
        {
            ___sprintMeter = 1;
        }
    }
}
