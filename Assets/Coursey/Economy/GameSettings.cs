using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Economy
{
    public class GameSettings
    {
        public static float TicksPerSecond = 1;
        public static float EconomyEventChancePerTick = 0.015f;
        public static float EconomyEventEffectWholeFactionChance = 0.015f;//multiply by 1000 then take it as a chance out of 1000
        public static float EconomyEventChanceToRemove = 0.025f;
        public static int AverageEconomyItemsProducedPerTick = 5;
        public static int AverageEconomyItemsUsedPerTick = 5;
        public static int MaxTradeStationsPerFaction = 10;
        //15 minute recalculation
        //frequency-based occurence of events, worse is less frequent

        public static bool LoadSettings()
        {
            if (GameController.gameLoaded)
            {
                return false;//already loaded
            }
            else
            {
                ReadConfigs();
                return true;
            }
        }
        public static void SaveSettings()
        {

        }
        public static void ReadConfigs()
        {
            var jsonManager = new JsonManager();
            var generalSettings = jsonManager.ReadConfig<GeneralSettings>();
            GameController.seed = generalSettings.seed;
            jsonManager.WriteConfig(generalSettings);
        }
    }
}