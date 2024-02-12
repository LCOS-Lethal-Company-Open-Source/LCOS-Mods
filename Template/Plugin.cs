using BepInEx;
using HarmonyLib;
namespace Template;
using Template.obj.

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    private void Awake()
    {
        // Plugin load logic goes here!
        // This script acts like a unity object.
        Logger.LogInfo($"Hello World!");

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void infiniteSprint(ref float __sprintMeter){
            __sprintMeter = 1;
        }
    }
}
