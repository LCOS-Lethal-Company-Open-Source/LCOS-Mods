using BepInEx;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;

namespace OneHP;


[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class OneHP : BaseUnityPlugin
{
    public static OneHP Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger { get; private set; } = null!;
    internal static Harmony ElmHarmony { get; set; } = new Harmony(MyPluginInfo.PLUGIN_GUID);

    private void Awake()
    {
        Logger = base.Logger;
        Instance = this;

        ElmHarmony.PatchAll(typeof(HealthEditing));

        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
    }
}

class HealthEditing
{
    [HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
    [HarmonyPrefix]

        static void infiniteCooldown(PlayerControllerB __instance){
            __instance.healthRegenerateTimer = 9999f;

        }

    [HarmonyPatch(typeof(StartOfRound), "ReviveDeadPlayers")]
    [HarmonyPostfix]
        static void startAt1(PlayerControllerB __instance){
            __instance.health = 1;
        }

    [HarmonyPatch(typeof(PlayerControllerB), "Update")]
    [HarmonyPostfix]
        static void keepSettingTo1(PlayerControllerB __instance){
            __instance.health = 1;
        }
}


