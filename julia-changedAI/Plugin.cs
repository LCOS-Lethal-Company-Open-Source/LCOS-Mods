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
        Logger.LogInfo($"Keep Inventory Active");
        harmony.PatchAll();

    }

    [HarmonyPatch(typeof(PlayerControllerB), "DropAllHeldItems")]
    class Patch
    {
        static bool Prefix(ref PlayerControllerB __instance)
        {
            return false;
        }
    }  
}
