//using System.Runtime.CompilerServices;
using BepInEx;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
//using LobbyCompatibility.Attributes;
//using LobbyCompatibility.Enums;
//using UnityEngine.PlayerLoop;

namespace OneHP;


[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
//[BepInDependency("BMX.LobbyCompatibility", BepInDependency.DependencyFlags.HardDependency)]
//[LobbyCompatibility(CompatibilityLevel.ClientOnly, VersionStrictness.None)]
public class OneHP : BaseUnityPlugin
{
    public static OneHP Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger { get; private set; } = null!;
    internal static Harmony ElmHarmony { get; set; } = new Harmony(MyPluginInfo.PLUGIN_GUID);

    private void Awake()
    {
        Logger = base.Logger;
        Instance = this;

        ElmHarmony.PatchAll(typeof(OneHP));
        ElmHarmony.PatchAll(typeof(noMoreRegen));
        ElmHarmony.PatchAll(typeof(noMoreHealth));

        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
    }

    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class noMoreRegen()
    {
        [HarmonyPostfix]
        [HarmonyPatch("LateUpdatePatch")]
        internal static void noMoreHealthRegen(ref float __healthRegenerateTimer){
            __healthRegenerateTimer = 9999f;
            Logger.LogInfo($"No Regen for you");
        }
    }

    [HarmonyPatch(typeof(StartOfRound))]
    internal class noMoreHealth()
    {
        [HarmonyPostfix]
        [HarmonyPatch("ReviveDeadPlayers")]

        internal static void setHealthToOne(ref int __health){
            __health = 1;
            Logger.LogInfo($"No health for you");
        }
    }
}
