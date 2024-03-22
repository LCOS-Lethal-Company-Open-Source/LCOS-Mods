using BepInEx;
using BepInEx.Logging;
using CMSound_Mod.Patches;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.PlayerLoop;

namespace CMSound_Mod
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class CMSound_Mod_Base : BaseUnityPlugin
    {
        private const string modGUID = "Poseidon.CMSound_Mod";
        private const string modName = "CM Sound Mod";
        private const string modVersion = "1.0.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        internal static CMSound_Mod_Base Instance;

        internal static ManualLogSource mls;

        internal static List<AudioClip> SoundFX;
        internal static AssetBundle Bundle;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            mls.LogInfo("The Scooby Laugh is Active!!!!!");

            harmony.PatchAll(typeof(CMSound_Mod_Base));
            harmony.PatchAll(typeof(StartOfRoundPatch));

            mls = Logger;

            SoundFX = new List<AudioClip>();
            string FolderLocation = Instance.Info.Location;
            FolderLocation = FolderLocation.TrimEnd("CMSound_Mod.dll".ToCharArray());
            Bundle = AssetBundle.LoadFromFile(FolderLocation + "scoob");
            if (Bundle != null)
            {
                mls.LogInfo("Successfully loaded asset bundle!");
                SoundFX = Bundle.LoadAllAssets<AudioClip>().ToList();
            }
            else
            {
                mls.LogError("Failed to load asset bundle!");
            }

        }

    }
}
