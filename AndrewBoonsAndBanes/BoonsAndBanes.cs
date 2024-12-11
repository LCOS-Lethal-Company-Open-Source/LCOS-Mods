using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using TerminalApi.Classes;
using static TerminalApi.TerminalApi;
using System.Collections.Generic;  // Required for IEnumerable<T>
using System.Reflection.Emit;
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
        // adds commands to the terminal so that the player can easily add
        // or remove the boons and banes
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
                    return "Nothing yet....";
                },
                Category = "BoonsAndBanesMod",
            });
            AddCommand("List Banes", new CommandInfo(){
                DisplayTextSupplier = () =>
                {
                    return "IncreaseDaySpeed - increases day Speed\n, MultiplyEnemyMaxHealth - Doubles enemy Max Health\n, NightmareDweller - makes the cave Dweller alot harder\n, IncreaseEnemyCap - more enemies can spawn initally and more can appear in the moon\n";
                },
                Category = "BoonsAndBanesMod",
            });
            AddCommand("List Cheats", new CommandInfo(){
                DisplayTextSupplier = () =>
                {
                    return "DoubleSellValue";
                },
                Category = "BoonsAndBanesMod",
            });

            // increases the dayspeed making runs have to be alot faster
            AddCommand("Bane IncreaseDaySpeed", new CommandInfo()
            {
                DisplayTextSupplier = () =>
                {
                    currentDaySpeedMultiplier += 1.0f;  
                    ScrapMultiplier.IncreaseMultiplier(0.5f);
                    Logger.LogInfo($"Day speed multiplier increased to {currentDaySpeedMultiplier}");

                    Harmony.Patch(typeof(TimeOfDay).GetMethod("ApplyDaySpeedMultiplier"),
                        new HarmonyMethod(typeof(TerminalCommandFunctions).GetMethod("ApplyDaySpeedMultiplierPatch")));
                    
                    return $"Day speed multiplier increased to {currentDaySpeedMultiplier}!";
                },
                Category = "BoonsAndBanesMod"
            });

            // doubles enemy max health
            AddCommand("Bane MultiplyEnemyHealth", new CommandInfo()
            {
                DisplayTextSupplier = () =>
                {
                    enemyHealthMultiplyer += 1.0f;  
                    Logger.LogInfo($"EnemyHealth Multiplied to {enemyHealthMultiplyer}");
                    ScrapMultiplier.IncreaseMultiplier(0.4f);


                    Harmony.Patch(typeof(EnemyAI).GetMethod("EnemyAI"),
                        new HarmonyMethod(typeof(TerminalCommandFunctions).GetMethod("EnemyHealthMultiply")));
                    
                    return $"EnemyHealth Multiplied to  {currentDaySpeedMultiplier}!";
                },
                Category = "BoonsAndBanesMod"
            });
            
            // makes cave dwellers harder
            AddCommand("Bane NightmareDweller", new CommandInfo()
            {
                DisplayTextSupplier = () =>
                {
                    Logger.LogInfo($"Cave Dwelleres are now alot harder");
                    ScrapMultiplier.IncreaseMultiplier(1.0f);

                    Harmony.PatchAll(typeof(DwellerNightmare));
                    
                    return $"Cave Dwelleres are now alot harder!";
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

            AddCommand("RemoveBane NightmareDweller", new CommandInfo()
            {
                DisplayTextSupplier = () =>
                {
                    Logger.LogInfo($"Cave Dweller AI back to normal");
                    ScrapMultiplier.DecreaseMultiplier(1.0f);

                    // This sucks but will have to do...
                    Harmony.Unpatch(typeof(CaveDwellerAI).GetMethod("DoNonBabyUpdateLogic"), typeof(DwellerNightmare).GetMethod("AdjustEnemySpeed"));
                    Harmony.Unpatch(typeof(CaveDwellerAI).GetMethod("DoNonBabyUpdateLogic"), typeof(DwellerNightmare).GetMethod("DoNonBabyUpdateLogic"));
                    Harmony.Unpatch(typeof(CaveDwellerAI).GetMethod("DoNonBabyUpdateLogic"), typeof(DwellerNightmare).GetMethod("IncreaseDetectionRange"));
                    Harmony.Unpatch(typeof(CaveDwellerAI).GetMethod("DoNonBabyUpdateLogic"), typeof(DwellerNightmare).GetMethod("MakeEscapeHarder"));
                    Harmony.Unpatch(typeof(CaveDwellerAI).GetMethod("DoNonBabyUpdateLogic"), typeof(DwellerNightmare).GetMethod("FasterLeapAndChase"));
                    Harmony.Unpatch(typeof(CaveDwellerAI).GetMethod("DoNonBabyUpdateLogic"), typeof(DwellerNightmare).GetMethod("IncreaseAggression"));

                    return $"Cave Dweller AI back to normal";
                },
                Category = "BoonsAndBanesMod"
            });

            AddCommand("RemoveBane MultiplyEnemyHealth", new CommandInfo()
            {
                DisplayTextSupplier = () =>
                {
                    enemyHealthMultiplyer -= 1.0f;  
                    Logger.LogInfo($"EnemyHealth Multiplied to {enemyHealthMultiplyer}");
                    ScrapMultiplier.DecreaseMultiplier(0.4f);

                    Harmony.Unpatch(typeof(EnemyAI).GetMethod("EnemyAI"), typeof(TerminalCommandFunctions).GetMethod("EnemyHealthMultiply"));
                    
                    return $"EnemyHealth Multiplied to {currentDaySpeedMultiplier}!";
                },
                Category = "BoonsAndBanesMod"
            });

            AddCommand("RemoveBane DaySpeedMultiplierPatch", new CommandInfo()
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

            AddCommand("RemoveBane IncreaseEnemyCap", new CommandInfo(){
            
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
    //collection of functions that actually change the code of the game
    internal static ManualLogSource Logger2 { get; private set; } = null!;
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
    [HarmonyPatch(typeof(RoundManager))] 
    [HarmonyPatch("SpawnScrapInLevel")]   
    public static void Prefix(ref int num, ref List<Item> ScrapToSpawn, ref List<int> list)
    {
        float customMultiplier = ScrapMultiplier.value; 
        num = (int)(num * customMultiplier);

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

    [HarmonyPatch(typeof(RoundManager))]  
    [HarmonyPatch("SpawnScrapInLevel")]    
    public static void Postfix(ref List<int> list, ref int num4)
    {
        float totalValueMultiplier = 1.5f; 
        num4 = (int)(num4 * totalValueMultiplier);
        Logger2.LogDebug($"Adjusted total value of spawned scrap: {num4}");
    } 

    // Increases enemy cap for enemys spawning after every round
    [HarmonyPatch(typeof(RoundManager))]
    [HarmonyPostfix]
    public static void EnemyCapAfterStartOfRound(RoundManager __instance){
        __instance.currentMaxOutsidePower *= 2;
        __instance.currentMaxInsidePower *= 2;
    }

    // Increases the Enemy cap for the inital enemy spawning
    [HarmonyPatch(typeof(SelectableLevel))]
    [HarmonyPostfix]
    public static void EnemyCap(SelectableLevel __instance){
        __instance.maxEnemyPowerCount *= 2;
    }
}

//scuffed method to multiply the scrap gain
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


[HarmonyPatch]
public class DwellerNightmare
{
    // Adjust enemy speed in different behavior states
    [HarmonyPatch(typeof(CaveDwellerAI), "DoNonBabyUpdateLogic")]
    [HarmonyPrefix]
    public static void AdjustEnemySpeed(ref float __state, CaveDwellerAI __instance)
    {
        // Example of changing speed based on state
        switch (__instance.currentBehaviourStateIndex)
        {
            case 1: // State 1 - Chasing
                __instance.agent.speed = 8f; // Increase speed when chasing
                break;
            case 2: // State 2 - Sneaking
                __instance.agent.speed = 5f; // Increased sneak speed
                break;
            case 3: // State 3 - Leaping
                __instance.agent.speed = 10f; // Increase leap speed
                break;
        }
    }

    // Shorten the scream timer to make the Dweller scream more often
    [HarmonyPatch(typeof(CaveDwellerAI), "DoNonBabyUpdateLogic")]
    [HarmonyPostfix]
    public static void ShortenScreamTimer(ref float screamTimer, CaveDwellerAI __instance)
    {
        if (__instance.screamTimer > 0f)
        {
            screamTimer -= Time.deltaTime * 1.5f; // Make the scream timer decrease faster
        }
    }

    // Increase the detection range for line of sight
    [HarmonyPatch(typeof(CaveDwellerAI), "DoNonBabyUpdateLogic")]
    [HarmonyPrefix]
    public static bool IncreaseDetectionRange(ref bool __result, Vector3 targetPosition, CaveDwellerAI __instance)
    {
        // Increase the line of sight range
        float increasedRange = 150f; // Increase range to 150 units
        __result = GameNetworkManager.Instance.localPlayerController.HasLineOfSightToPosition(targetPosition, increasedRange, 30, 3f);
        return false; // Prevent the original method from running
    }

    // Make it harder for the Dweller to escape
    [HarmonyPatch(typeof(CaveDwellerAI), "DoNonBabyUpdateLogic")]
    [HarmonyPrefix]
    public static bool MakeEscapeHarder(ref bool __result, CaveDwellerAI __instance)
    {
        __result = false; // Make the Dweller never get trapped
        return false; // Prevent the original method from running
    }

    // Faster transition to leaping or chasing state
    [HarmonyPatch(typeof(CaveDwellerAI), "DoNonBabyUpdateLogic")]
    [HarmonyPostfix]
    public static void FasterLeapAndChase(ref bool leaping, ref bool chasingAfterLeap, CaveDwellerAI __instance)
    {
        if (__instance.screamTimer <= 0f && !leaping && !chasingAfterLeap)
        {
            // Make the Dweller more likely to enter the leaping or chasing state
            leaping = true; // Force a leap immediately
        }
    }

    // Additional aggressive behavior (optional)
    [HarmonyPatch(typeof(CaveDwellerAI), "DoNonBabyUpdateLogic")]
    [HarmonyPrefix]
    public static void IncreaseAggression(CaveDwellerAI __instance)
    {
        // Making the Dweller more aggressive
        if (__instance.targetPlayer != null)
        {
            float playerDistance = Vector3.Distance(__instance.transform.position, __instance.targetPlayer.transform.position);
            if (playerDistance < 15f) // Closer proximity triggers a more aggressive response
            {
                __instance.currentBehaviourStateIndex = 3; // Switch to aggressive state
                __instance.screaming = true;
                __instance.creatureVoice.PlayOneShot(__instance.growlSFX);
            }
        }
    }
}