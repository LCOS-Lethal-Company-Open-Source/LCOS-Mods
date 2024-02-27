using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx;
using HarmonyLib;
using UnityEngine;
namespace Template;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    public static Plugin Instance;
    Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);

    private void Awake()
    {
        // Plugin load logic goes here!
        // This script acts like a unity object.
        Instance = this;
        harmony.PatchAll();
        Logger.LogInfo($"Inverse Coil active!");
    }

    [HarmonyPatch(typeof(SpringManAI))]
    [HarmonyPatch("Update")]
    public static class InverseCoilPatch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction instruction in instructions) 
            {
                if (instruction.opcode == OpCodes.Ldfld && instruction.operand.Equals(typeof(SpringManAI).GetField("currentChaseSpeed", 
                                                                                                                    BindingFlags.NonPublic | 
                                                                                                                    BindingFlags.Instance)))
                {
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Ldc_R4, 12.0f);
                }
                else yield return instruction; 
                if (instruction.opcode == OpCodes.Blt) 
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    yield return new CodeInstruction(OpCodes.Ldc_I4_0);
                    yield return new CodeInstruction(OpCodes.Ceq);
                    yield return new CodeInstruction(OpCodes.Stloc_1);
                }
            }
        }
    }
}

