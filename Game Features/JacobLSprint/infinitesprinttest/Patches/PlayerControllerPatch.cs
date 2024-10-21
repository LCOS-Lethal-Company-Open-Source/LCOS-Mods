using DunGen;
using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.PlayerLoop;

namespace infinitesprinttest.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerPatch
    {
        [HarmonyPatch("Update")]
        [HarmonyPostfix] 
        static void infinitesprintpatch(ref float ___sprintMeter)
        {
            ___sprintMeter = 1f;
        }

    }
}
