using System;
using BepInEx;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;
namespace Template;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);

    private void Awake()
    {
        // Plugin load logic goes here!
        // This script acts like a unity object.
        Logger.LogInfo($"Funny jump active.");
        harmony.PatchAll();
    }

    [HarmonyPatch(typeof(PlayerControllerB), "PlayerJump")]
    class Patch
    {
        static bool Prefix(ref PlayerControllerB __instance)
        {
            int chanceVal = UnityEngine.Random.Range(0, 100);

            // 40% chance to jump normally
            if (chanceVal >= 60) __instance.jumpForce = 5f;
            // 30% chance to jump slightly higher
            else if (chanceVal >= 30) __instance.jumpForce = UnityEngine.Random.Range(10f, 20f);
            // 15% chance to jump very high
            else if (chanceVal >= 15) __instance.jumpForce = UnityEngine.Random.Range(25f, 35f);
            // 10% chance to jump slightly lower
            else if (chanceVal >= 5) __instance.jumpForce = UnityEngine.Random.Range(3f, 4.5f);
            // 3% chance to jump very low
            else if (chanceVal >= 2) __instance.jumpForce = UnityEngine.Random.Range(1f, 2.5f);
            // 2% chance to jump extremely high
            else __instance.jumpForce = UnityEngine.Random.Range(50f, 100f);
            return true;
        }
    }  
}
