using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CMGodModeMod.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatch
    {
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        
        static void infiniteSprintPatch(ref float ___sprintMeter)
        {
            ___sprintMeter = 1f;
        }

        static void increasedJumpForcePatch(ref float ___jumpForce)
        {
            ___jumpForce = 0.01f;
        }

        static void fullHealthPatch(ref int ___health)
        {
            ___health = 100;
        }
    }
}
