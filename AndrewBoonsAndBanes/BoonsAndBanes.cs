using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using TerminalApi;
using TerminalApi.Classes;
using static TerminalApi.Events.Events;
using static TerminalApi.TerminalApi;


namespace BoonsAndBanes;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
//[BepInDependency("BMX.LobbyCompatibility", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("atomic.terminalapi", BepInDependency.DependencyFlags.HardDependency)]
public class BoonsAndBanes : BaseUnityPlugin
{
    public static BoonsAndBanes Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger { get; private set; } = null!;
    internal static Harmony? Harmony { get; set; } = null;

    internal static TerminalCommandFunctions CommandData = new TerminalCommandFunctions();

    private void Awake()
    {
        Logger = base.Logger;
        Instance = this;

        Harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);

        Harmony.PatchAll(typeof(BoonsAndBanes));
        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
    }

    private void testCommand(){

            AddCommand("does this work?", new CommandInfo()
            {
	        DisplayTextSupplier = () =>
	        {
		        Logger.LogDebug("It Sure did!");
		        return "I Sure Hope it Does!\n\n";
	        },
	        Category = "Other"
            });
    }

    private void TerminalCommands(){
        // may not be needed
            Harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);

            AddCommand("ScrapMultiplier", new CommandInfo()
            {
	        DisplayTextSupplier = () =>
	        {
		        return "Current Scrap Multiplier: " + CommandData.getMultiplier() + "\n";
	        },
	        Category = "BoonsAndBanesMod"
            });
            AddCommand("List Boons", new CommandInfo(){
                DisplayTextSupplier = () =>
                {
                    return ":3";
                },
                Category = "BoonsAndBanesMod",
            });

            AddCommand("Cheat DoubleSellValue", new CommandInfo(){
            
                DisplayTextSupplier = () =>
	            {
                    Harmony.Patch(typeof(TerminalCommandFunctions).GetMethod("doubleSellValue"));
		            return "Doubling Sell value of ScrapGains!";
	            },
	            Category = "BoonsAndBanesMod"
            });

            AddCommand("RemoveCheat DoubleSellValue", new CommandInfo(){
            
                DisplayTextSupplier = () =>
	            {
                    Harmony.Unpatch(typeof(TerminalCommandFunctions).GetMethod("doubleSellValue"), HarmonyPatchType.Postfix);
		            return "Sell Value is back to normal!";
	            },
	            Category = "BoonsAndBanesMod"
            });
    }
}

public class TerminalCommandFunctions(){
    int ScrapMultiplier = 1;
    string[] boonNames = ["ExtraLife"];
    string[] baneNames = ["HalfHealth", "OopsItsAllX", "FasterDayCycle"];
    string[] cheatNames = ["DoubleSellValue"];
    public int getMultiplier(){
        return ScrapMultiplier;
    }

    [HarmonyPatch(typeof(TimeOfDay), "SetBuyingRateForDay")]
    [HarmonyPostfix]
    static void doubleSellValue() {
        StartOfRound.Instance.companyBuyingRate *= 2;
    }


    [HarmonyPatch(typeof(PlayerControllerB), "Update")]
    [HarmonyPostfix]
    static void halfMaxHealth(PlayerControllerB __instance){
            if (__instance.health > 50){
                __instance.health = 50;
            };
    }

    [HarmonyPatch(typeof(TimeOfDay), "CalcuatePlanetTime")]
    [HarmonyTranspiler]
    public float FasterDayCycle(SelectableLevel level)
    {
	return (globalTime + level.OffsetFromGlobalTime) * level.DaySpeedMultiplier * 2 % (totalTime + 1f);
    }

}