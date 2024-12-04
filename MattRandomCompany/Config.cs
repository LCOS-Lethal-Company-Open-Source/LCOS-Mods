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
        public readonly ConfigEntry<float> scrapSpawnLower;
        public readonly ConfigEntry<float> scrapSpawnUpper;
        public readonly ConfigEntry<bool> scrapSpawnRateEnabled;

        public readonly ConfigEntry<float> scrapValueLower;
        public readonly ConfigEntry<float> scrapValueUpper;
        public readonly ConfigEntry<bool> scrapValueEnabled;

        //Player Controller
        public readonly ConfigEntry<float> moveSpeedLower;
        public readonly ConfigEntry<float> moveSpeedUpper;
        public readonly ConfigEntry<bool> moveSpeedEnabled;

        public readonly ConfigEntry<float> climbSpeedLower;
        public readonly ConfigEntry<float> climbSpeedUpper;
        public readonly ConfigEntry<bool> climbSpeedEnabled;

        public Config(ConfigFile cfg) {
            InitInstance(this);
            cfg.SaveOnConfigSet = false;
            
            // Scrap

            scrapSpawnLower = cfg.Bind<float>("General.Scrap", "ScrapSpawnRateLowerBound", 1, "The Minimum Scrap Spawn Rate Multiplier");
            scrapSpawnUpper = cfg.Bind<float>("General.Scrap", "ScrapSpawnRateUpperBound", 1, "The Maximum Scrap Spawn Rate Multiplier");
            scrapSpawnRateEnabled = cfg.Bind<bool>("General.Scrap.Toggles", "ScrapSpawnRateMultiplierEnabled", true, "Enables Scrap Spawn Rate Multiplier ... true : enabled ... false : disabled");

            scrapValueLower = cfg.Bind<float>("General.Scrap", "ScrapValueLowerBound", 1, "The Minimum Scrap Value Multiplier");
            scrapValueUpper = cfg.Bind<float>("General.Scrap", "ScrapValueUpperBound", 1, "The Maximum Scrap Value Multiplier");
            scrapValueEnabled = cfg.Bind<bool>("General.Scrap.Toggles", "ScrapValueMultiplierEnabled", true, "Enables Scrap Value Multiplier ... true : enabled ... false : disabled");

            // Player Controller

            moveSpeedLower = cfg.Bind<float>("General.Movement", "MoveSpeedLowerBound", 1, "The Minimum Movement Speed Multiplier");
            moveSpeedUpper = cfg.Bind<float>("General.Movement", "MoveSpeedUpperBound", 1, "The Maximum Movement Speed Multiplier");            
            moveSpeedEnabled = cfg.Bind<bool>("General.Movement.Toggles", "MoveSpeedEnabled", true, "Enables Player Movement Speed Multiplier ... true : enabled ... false : disabled");


            climbSpeedLower = cfg.Bind<float>("General.Movement", "ClimbSpeedLowerBound", 1, "The Maximum Climb Speed Multiplier");
            climbSpeedUpper = cfg.Bind<float>("General.Movement", "ClimbSpeedUpperBound", 1, "The Maximum Climb Speed Multiplier");
            climbSpeedEnabled = cfg.Bind<bool>("General.Movement.Toggles", "ClimbSpeedEnabled", true, "Enables Player Climb Speed Multiplier ... true : enabled ... false : disabled");

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
