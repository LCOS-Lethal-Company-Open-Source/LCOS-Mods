using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Configuration;
using HarmonyLib;

namespace RandomCompany
{
    class Config
    {

        public readonly ConfigEntry<float> scrapSpawnLower;
        public readonly ConfigEntry<float> scrapSpawnUpper;
        public readonly ConfigEntry<bool> scrapSpawnRateEnabled;
        public readonly ConfigEntry<float> scrapValueLower;
        public readonly ConfigEntry<float> scrapValueUpper;
        public readonly ConfigEntry<bool> scrapValueEnabled;

        public Config(ConfigFile cfg) {

            cfg.SaveOnConfigSet = false;

            scrapSpawnLower = cfg.Bind<float>("General", "ScrapSpawnRateLowerBound", 1, "The Minimum Scrap Spawn Rate Multiplier");
            scrapSpawnUpper = cfg.Bind<float>("General", "ScrapSpawnRateUpperBound", 1, "The Maximum Scrap Spawn Rate Multiplier");
            scrapValueLower = cfg.Bind<float>("General", "ScrapValueLowerBound", 1, "The Minimum Scrap Value Multiplier");
            scrapValueUpper = cfg.Bind<float>("General", "ScrapValueUpperBound", 1, "The Maximum Scrap Value Multiplier");

            scrapSpawnRateEnabled = cfg.Bind<bool>("General.Toggles", "ScrapSpawnRateMultiplierEnabled", true, "Enables Scrap Spawn Rate Multiplier ... true : enabled ... false : disabled");
            scrapValueEnabled = cfg.Bind<bool>("General.Toggles", "ScrapValueMultiplierEnabled", true, "Enables Scrap Value Multiplier ... true : enabled ... false : disabled");


            ClearOrphanedEntries(cfg);

            cfg.Save();
            cfg.SaveOnConfigSet = true;

        }

        static void ClearOrphanedEntries(ConfigFile cfg)
        {
            PropertyInfo orphanedEntriesProp = AccessTools.Property(typeof(ConfigFile), "OrphanedEntries");
            var orphanedEntries = (Dictionary<ConfigDefinition, string>)orphanedEntriesProp.GetValue(cfg);
            orphanedEntries.Clear();
        }
    }
}
