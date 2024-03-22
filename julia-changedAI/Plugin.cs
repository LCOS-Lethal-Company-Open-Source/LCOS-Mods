﻿using System;
using BepInEx;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;
namespace ChangedAI;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{

    Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);

    private void Awake()
    {
        // Plugin load logic goes here!
        // This script acts like a unity object.
        Logger.LogInfo($"Fly mod active");
        harmony.PatchAll();

    }

    [HarmonyPatch(typeof(PlayerControllerB), "PlayerJump")]
    class Patch
    {
        static bool Postfix(ref PlayerControllerB __instance)
        {   
            __instance.fallValue = 0;
            return true;
        }
    }  
}
