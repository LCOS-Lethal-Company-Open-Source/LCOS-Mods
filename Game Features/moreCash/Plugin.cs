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
        Logger.LogInfo($"moreCash Active");
        harmony.PatchAll(typeof(moreCash));
    }

    [HarmonyPatch(typeof(TimeOfDay), "Awake")]
    
    class moreCash
    {
        private static void Postfix(ref TimeOfDay __instance)
        {
            int startingAmount = 1000;
            __instance.quotaVariables.startingCredits = startingAmount;
        }  
    }
}