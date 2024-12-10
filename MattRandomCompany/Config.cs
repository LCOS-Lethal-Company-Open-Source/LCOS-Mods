using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Configuration;
using GameNetcodeStuff;
using HarmonyLib;
using Unity.Collections;
using Unity.Netcode;

namespace RandomCompany
{
    [Serializable]
    class Config : SyncedInstance<Config>
    {

        //Scrap

        public readonly ConfigEntry<float> scrapSpawnMulLower;
        public readonly ConfigEntry<float> scrapSpawnMulUpper;
        public readonly ConfigEntry<bool> scrapSpawnMulEnabled;

        public readonly ConfigEntry<float> scrapValueMulLower;
        public readonly ConfigEntry<float> scrapValueMulUpper;
        public readonly ConfigEntry<bool> scrapValueMulEnabled;


        // Factory

        public readonly ConfigEntry<float> factorySizeMulLower;
        public readonly ConfigEntry<float> factorySizeMulUpper;
        public readonly ConfigEntry<bool> factorySizeMulEnabled;

        //Player Controller

        public readonly ConfigEntry<float> moveSpeedMulLower;
        public readonly ConfigEntry<float> moveSpeedMulUpper;
        public readonly ConfigEntry<bool> moveSpeedMulEnabled;

        public readonly ConfigEntry<float> sprintTimeMulLower;
        public readonly ConfigEntry<float> sprintTimeMulUpper;
        public readonly ConfigEntry<bool> sprintTimeMulEnabled;

        public readonly ConfigEntry<float> jumpForceMulLower;
        public readonly ConfigEntry<float> jumpForceMulUpper;
        public readonly ConfigEntry<bool> jumpForceMulEnabled;

        public readonly ConfigEntry<float> climbSpeedMulLower;
        public readonly ConfigEntry<float> climbSpeedMulUpper;
        public readonly ConfigEntry<bool> climbSpeedMulEnabled;

        public Config(ConfigFile cfg) {
            InitInstance(this);
            cfg.SaveOnConfigSet = false;
            
            // Scrap

            scrapSpawnMulLower = cfg.Bind<float>("General.Scrap", "ScrapSpawnMultiplierLowerBound", 1, "The Minimum Scrap Spawn Rate Multiplier");
            scrapSpawnMulUpper = cfg.Bind<float>("General.Scrap", "ScrapSpawnMultiplierUpperBound", 1, "The Maximum Scrap Spawn Rate Multiplier");
            scrapSpawnMulEnabled = cfg.Bind<bool>("General.Scrap.Toggles", "ScrapSpawnRateMultiplierEnabled", true, "Enables Scrap Spawn Rate Multiplier ... true : enabled ... false : disabled");

            scrapValueMulLower = cfg.Bind<float>("General.Scrap", "ScrapValueMultiplierLowerBound", 1, "The Minimum Scrap Value Multiplier");
            scrapValueMulUpper = cfg.Bind<float>("General.Scrap", "ScrapValueMultiplierUpperBound", 1, "The Maximum Scrap Value Multiplier");
            scrapValueMulEnabled = cfg.Bind<bool>("General.Scrap.Toggles", "ScrapValueMultiplierEnabled", true, "Enables Scrap Value Multiplier ... true : enabled ... false : disabled");

            // Factory

            factorySizeMulLower = cfg.Bind<float>("General.Factory", "FactorySizeMultiplierLowerBound", 1, "The Minimum Factory Size Multiplier");
            factorySizeMulUpper = cfg.Bind<float>("General.Factory", "FactorySizeMultiplierUpperBound", 1, "The Maximum Factory Size Multiplier");
            factorySizeMulEnabled = cfg.Bind<bool>("General.Factory.Toggles", "FactorySizeMultipierEnabled", true, "Enables Factory Size Multiplier ... true : enabled ... false : disabled");

            // Player Controller

            moveSpeedMulLower = cfg.Bind<float>("General.Movement", "MoveSpeedMultiplierLowerBound", 1, "The Minimum Movement Speed Multiplier");
            moveSpeedMulUpper = cfg.Bind<float>("General.Movement", "MoveSpeedMultiplierUpperBound", 1, "The Maximum Movement Speed Multiplier");            
            moveSpeedMulEnabled = cfg.Bind<bool>("General.Movement.Toggles", "MoveSpeedEnabled", true, "Enables Player Movement Speed Multiplier ... true : enabled ... false : disabled");

            sprintTimeMulLower = cfg.Bind<float>("General.Movement", "SprintTimeMultiplierLowerBound", 1, "The Minimum Multiplier for Sprint Time");
            sprintTimeMulUpper = cfg.Bind<float>("General.Movement", "SprintTimeMultiplierUpperBound", 1, "The Maximum Multiplier for Sprint Time");
            sprintTimeMulEnabled = cfg.Bind<bool>("General.Movement.Toggles", "SprintTimeMultiplierEnabled", true, "Enables Sprint Time Multiplier ... true : enabled ... false : disabled");

            jumpForceMulLower = cfg.Bind<float>("General.Movement", "JumpForceMultiplierLowerBound", 1, "The Minimum Multiplier for Jump Force");
            jumpForceMulUpper = cfg.Bind<float>("General.Movement", "JumpForceMultiplierUpperBound", 1, "The Maximum Multiplier for Jump Force");
            jumpForceMulEnabled = cfg.Bind<bool>("General.Movement.Toggles", "JumpForceMultiplierEnabled", true, "Enables Jump Force Multiplier ... true : enabled ... false : disabled");

            climbSpeedMulLower = cfg.Bind<float>("General.Movement", "ClimbSpeedMultiplierLowerBound", 1, "The Maximum Climb Speed Multiplier");
            climbSpeedMulUpper = cfg.Bind<float>("General.Movement", "ClimbSpeedMultiplierUpperBound", 1, "The Maximum Climb Speed Multiplier");
            climbSpeedMulEnabled = cfg.Bind<bool>("General.Movement.Toggles", "ClimbSpeedMultiplierEnabled", true, "Enables Player Climb Speed Multiplier ... true : enabled ... false : disabled");

            ClearOrphanedEntries(cfg);

            cfg.Save();
            cfg.SaveOnConfigSet = true;

        }

        public static void RequestSync()
        {
            if (!IsClient) return;

            FastBufferWriter stream = new FastBufferWriter(IntSize, Allocator.Temp);
            MessageManager.SendNamedMessage("Random Company_OnRequestConfigSync", 0uL, stream);
        }

        public static void OnRequestSync(ulong clientId, FastBufferReader _)
        {
            if (!IsHost) return;

            RandomCompany.logMessage($"Config sync request received from client: {clientId}");

            byte[] array = SerializeToBytes(Instance);
            int value = array.Length;

            FastBufferWriter stream = new FastBufferWriter(value + IntSize, Allocator.Temp);

            try
            {
                stream.WriteValueSafe(in value, default);
                stream.WriteBytesSafe(array);

                MessageManager.SendNamedMessage("Random Company_OnReceiveConfigSync", clientId, stream);
            }
            catch (Exception e)
            {
                RandomCompany.logMessage($"Error occurred syncing config with client: {clientId}\n{e}");
            }
        }

        public static void OnReceiveSync(ulong _, FastBufferReader reader)
        {
            if (!reader.TryBeginRead(IntSize))
            {
                RandomCompany.logMessage("Config sync error: Could not begin reading buffer.");
                return;
            }

            reader.ReadValueSafe(out int val, default);
            if (!reader.TryBeginRead(val))
            {
                RandomCompany.logMessage("Config sync error: Host could not sync.");
                return;
            }

            byte[] data = new byte[val];
            reader.ReadBytesSafe(ref data, val);

            SyncInstance(data);

            RandomCompany.logMessage("Successfully synced config with host.");
        }

      

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameNetworkManager), "StartDisconnect")]
        public static void PlayerLeave()
        {
            Config.RevertSync();
        }
        static void ClearOrphanedEntries(ConfigFile cfg)
        {
            PropertyInfo orphanedEntriesProp = AccessTools.Property(typeof(ConfigFile), "OrphanedEntries");
            var orphanedEntries = (Dictionary<ConfigDefinition, string>)orphanedEntriesProp.GetValue(cfg);
            orphanedEntries.Clear();
        }
    }
}
