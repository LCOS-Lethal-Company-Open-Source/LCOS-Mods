using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Steamworks.ServerList;
using TerminalApi;
using TerminalApi.Classes;
using static TerminalApi.Events.Events;
using static TerminalApi.TerminalApi;
using System.Collections.Generic;  // Required for IEnumerable<T>
using System.Reflection.Emit;
using UnityEngine.Animations.Rigging;  // Required for OpCodes
using UnityEngine;

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

    public static float currentDaySpeedMultiplier = 1.0f;  // Default multiplier
    public static float enemyHealthMultiplyer = 1.0f;

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

            AddCommand("Bane IncreaseDaySpeed", new CommandInfo()
            {
                DisplayTextSupplier = () =>
                {
                    // Increment the multiplier by 1.0f (you can adjust the increment value)
                    currentDaySpeedMultiplier += 1.0f;  
                    ScrapMultiplier.IncreaseMultiplier(0.5f);
                    Logger.LogInfo($"Day speed multiplier increased to {currentDaySpeedMultiplier}");

                    // Apply the updated multiplier using Harmony
                    Harmony.Patch(typeof(TimeOfDay).GetMethod("ApplyDaySpeedMultiplier"),
                        new HarmonyMethod(typeof(TerminalCommandFunctions).GetMethod("ApplyDaySpeedMultiplierPatch")));
                    
                    return $"Day speed multiplier increased to {currentDaySpeedMultiplier}!";
                },
                Category = "BoonsAndBanesMod"
            });

            AddCommand("Bane MultiplyEnemyHealth", new CommandInfo()
            {
                DisplayTextSupplier = () =>
                {
                    // Increment the multiplier by 1.0f (you can adjust the increment value)
                    enemyHealthMultiplyer += 1.0f;  
                    Logger.LogInfo($"EnemyHealth Multiplied to {enemyHealthMultiplyer}");

                    // Apply the updated multiplier using Harmony
                    Harmony.Patch(typeof(EnemyAI).GetMethod("EnemyAI"),
                        new HarmonyMethod(typeof(TerminalCommandFunctions).GetMethod("EnemyHealthMultiply")));
                    
                    return $"EnemyHealth Multiplied to  {currentDaySpeedMultiplier}!";
                },
                Category = "BoonsAndBanesMod"
            });

            AddCommand("RemoveBane MultiplyEnemyHealth", new CommandInfo()
            {
                DisplayTextSupplier = () =>
                {
                    // Increment the multiplier by 1.0f (you can adjust the increment value)
                    enemyHealthMultiplyer -= 1.0f;  
                    Logger.LogInfo($"EnemyHealth Multiplied to {enemyHealthMultiplyer}");

                    // Apply the updated multiplier using Harmony
                    Harmony.Unpatch(typeof(EnemyAI).GetMethod("EnemyAI"), typeof(TerminalCommandFunctions).GetMethod("EnemyHealthMultiply"));
                    
                    return $"EnemyHealth Multiplied to  {currentDaySpeedMultiplier}!";
                },
                Category = "BoonsAndBanesMod"
            });

            AddCommand("Bane RemoveDaySpeedMultiplierPatch", new CommandInfo()
            {
                DisplayTextSupplier = () =>
                {
                    // Unpatch the method
                    Harmony.Unpatch(typeof(TimeOfDay).GetMethod("ApplyDaySpeedMultiplier"), typeof(TerminalCommandFunctions).GetMethod("ApplyDaySpeedMultiplierPatch"));
                    ScrapMultiplier.DecreaseMultiplier(0.5f * currentDaySpeedMultiplier);
                    currentDaySpeedMultiplier = 0.0f;

                    Logger.LogInfo("Day speed multiplier patch removed!");

                    return "Day speed multiplier patch removed, restoring original behavior.";
                },
                Category = "BoonsAndBanesMod"
            });

            AddCommand("Bane IncreaseEnemyCap", new CommandInfo(){
            
                DisplayTextSupplier = () =>
	            {
                    
                    Harmony.Patch(typeof(RoundManager).GetMethod(""), new HarmonyMethod(typeof(TerminalCommandFunctions).GetMethod("EnemyCapAfterStartOfRound")));
                    Harmony.Patch(typeof(RoundManager).GetMethod(""), new HarmonyMethod(typeof(TerminalCommandFunctions).GetMethod("EnemyCap")));

                    ScrapMultiplier.IncreaseMultiplier(0.3f * currentDaySpeedMultiplier); 
		            return "Doubling Enemy Spawn Cap";
	            },
	            Category = "BoonsAndBanesMod"
            });

            AddCommand("RemoveBane DoubleSellValue", new CommandInfo(){
            
                DisplayTextSupplier = () =>
	            {
                    Harmony.Unpatch(typeof(TerminalCommandFunctions).GetMethod("EnemyCapAfterStartOfRound"), HarmonyPatchType.Prefix);
                    Harmony.Unpatch(typeof(TerminalCommandFunctions).GetMethod("EnemyCap"), HarmonyPatchType.Prefix);
		            return "No Longer increasing Enemy Spawn Cap";
	            },
	            Category = "BoonsAndBanesMod"
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

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
//[BepInDependency("BMX.LobbyCompatibility", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("atomic.terminalapi", BepInDependency.DependencyFlags.HardDependency)]
public class TerminalCommandFunctions : BaseUnityPlugin{
    internal new static ManualLogSource Logger2 { get; private set; } = null!;
    string[] boonNames = ["ExtraLife"];
    string[] baneNames = ["HalfHealth", "OopsItsAllX", "FasterDayCycle"];
    string[] cheatNames = ["DoubleSellValue"];

    ScrapMultiplier multi = new ScrapMultiplier();

    private void Awake(){
        Logger2 = base.Logger;
    }

    public float getMultiplier(){
        return ScrapMultiplier.value;
    }

    [HarmonyPatch(typeof(TimeOfDay), "SetBuyingRateForDay")]
    [HarmonyPostfix]
    static void doubleSellValue() {
        StartOfRound.Instance.companyBuyingRate *= 2;
    }

    [HarmonyPatch(typeof(EnemyAI))]
    [HarmonyPrefix]
    public static void EnemyHealthMultiply(ref float health)
    {
        // Apply multiplier to the damage value (or HP value)
        float multiplier = 2.0f;  // Example: double the HP
        health *= multiplier;  // Multiply the damage (or HP value)
    }

    // no reason for why this is broken???
    /*
    [HarmonyPatch(typeof(PlayerControllerB), "Update")]
    [HarmonyPostfix]
    static void halfMaxHealth(PlayerControllerB __instance){
            if (__instance.health > 50){
                __instance.health = 50;
            };
    }
    */

    //float globalTime = Traverse.Create(typeof(TimeOfDay)).Field("globalTime").GetValue() as float;
    //float totalTime = Traverse.Create(typeof(TimeOfDay)).Field("totalTime").GetValue() as float;


             // The field that stores the DaySpeedMultiplier in SelectableLevel
        static FieldInfo f_daySpeedMultiplier = AccessTools.Field(typeof(SelectableLevel), "DaySpeedMultiplier");

        // The method we want to call to apply a custom multiplier to DaySpeedMultiplier
        static MethodInfo m_ApplyDaySpeedMultiplier = SymbolExtensions.GetMethodInfo(() => ApplyDaySpeedMultiplier(1.5f)); // 1.5x speed as an example

        // Example method that applies a multiplier to the DaySpeedMultiplier
        public static void ApplyDaySpeedMultiplier(float multiplier)
        {
            // Modify the multiplier, here we simply adjust it to speed up or slow down the day cycle
            float currentMultiplier = (float)f_daySpeedMultiplier.GetValue(null); // Get current multiplier value (static access)
            f_daySpeedMultiplier.SetValue(null, currentMultiplier * multiplier); // Adjust multiplier (static access)
        }

        // This is the transpiler function that modifies CalculatePlanetTime
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var found = false;

            foreach (var instruction in instructions)
            {
                // Look for the instruction that loads the DaySpeedMultiplier value (likely a load of the field)
                if (instruction.LoadsField(f_daySpeedMultiplier))
                {
                    // Before using the DaySpeedMultiplier, call our method to apply the multiplier
                    yield return new CodeInstruction(OpCodes.Call, m_ApplyDaySpeedMultiplier);  // Call method to modify multiplier
                    found = true;
                }

                yield return instruction;
            }

            // If the DaySpeedMultiplier field is not found, report an error
            if (!found)
            {
                Logger2.LogDebug("Day Speed Multiplier Failed\n");
            }
        }

        // This will be called before the SpawnScrapInLevel method is executed.
    [HarmonyPatch(typeof(RoundManager))]  // Replace with the class containing SpawnScrapInLevel
    [HarmonyPatch("SpawnScrapInLevel")]    // The method you want to patch
    public static void Prefix(ref int num, ref List<Item> ScrapToSpawn, ref List<int> list)
    {
        float customMultiplier = ScrapMultiplier.value;  // Adjust this multiplier as needed
        num = (int)(num * customMultiplier);

        // Use reflection to set scrapValue if the Item class doesn't expose it directly
        foreach (var item in ScrapToSpawn)
        {
            FieldInfo scrapValueField = item.GetType().GetField("scrapValue", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (scrapValueField != null)
            {
                int currentScrapValue = (int)scrapValueField.GetValue(item);
                scrapValueField.SetValue(item, currentScrapValue * customMultiplier);
            }
        }

        // Modify the list of scrap values, if needed
        for (int i = 0; i < list.Count; i++)
        {
            list[i] = (int)(list[i] * customMultiplier);  // Multiply individual scrap value in list
        }
    }

    // Optional: Adjust logic after spawn if needed, e.g., multiplying values at the end
    [HarmonyPatch(typeof(RoundManager))]  // Replace with the class containing SpawnScrapInLevel
    [HarmonyPatch("SpawnScrapInLevel")]    // The method you want to patch
    public static void Postfix(ref List<int> list, ref int num4)
    {
        // Adjust num4 if necessary (total scrap value), for example:
        float totalValueMultiplier = 1.5f;  // Example multiplier for total value
        num4 = (int)(num4 * totalValueMultiplier);
        Logger2.LogDebug($"Adjusted total value of spawned scrap: {num4}");
    } 

    [HarmonyPatch(typeof(RoundManager))]
    [HarmonyPostfix]
    public static void EnemyCapAfterStartOfRound(RoundManager __instance, int multiplier){
        __instance.currentMaxOutsidePower *= multiplier;
        __instance.currentMaxInsidePower *= multiplier;
    }

    [HarmonyPatch(typeof(SelectableLevel))]
    [HarmonyPostfix]
    public static void EnemyCap(SelectableLevel __instance, int multiplier){
        __instance.maxEnemyPowerCount *= multiplier;
    }
    //old

    /*
    static int FasterDayCycle(SelectableLevel __instance, float ___globalTime, float ___totalTime, int daytimeMultipler = 2)
    {
         __instance.DaySpeedMultiplier * = 
	    return (___globalTime + __instance.OffsetFromGlobalTime) * __instance.DaySpeedMultiplier * daytimeMultipler % (___totalTime + 1f);
    }
    */
}

//scuffed but shouldwork
class ScrapMultiplier(){
    public static float value = 1;

    public static void IncreaseMultiplier(float factor)
    {
        value *= factor;
    }

    // This function decreases the multiplier by a specific factor (e.g., when a bane is applied)
    public static void DecreaseMultiplier(float factor)
    {
        value /= factor;
    }

    // Method to set multiplier directly
    public static void SetMultiplier(float newMultiplier)
    {
        value = newMultiplier;
    }

    // Method to reset multiplier to default value
    public static void ResetMultiplier()
    {
        value = 1.0f;
    }
}

public class EnemyManager : MonoBehaviour
{
    // Function to multiply all enemy HP by a specified multiplier
    public static void MultiplyAllEnemiesHP(int multiplier)
    {
        // Find all EnemyAI objects in the scene
        EnemyAI[] allEnemies = FindObjectsOfType<EnemyAI>();

        // Loop through each enemy and multiply their HP
        foreach (EnemyAI enemy in allEnemies)
        {
            if (enemy != null)  // Make sure the enemy exists
            {
                enemy.enemyHP *= multiplier;  // Multiply enemy HP by the multiplier
                Debug.Log($"Enemy HP multiplied. New HP: {enemy.enemyHP}");
            }
        }
    }
}
