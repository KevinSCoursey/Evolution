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
        public static int MinTradeStationsPerFaction = 1;
        public static int MaxInternalTradeRoutesPerTradeStation = 3;
        public static int MaxExternalTradeRoutesPerTradeStation = 3;
        public static int MaxAttemptsToGenerateSomething = 5;
        public static int AverageMoneyHeldPerTradeStation = 150000;
        public static int AverageNumItemsExchangedPerTrade = 3;
        public static float PercentTaxExternalTrades = 1.5f;
        public static float SameFactionPriceDiscount = 1.5f;

        public static float MinutesGameWillRunFloat = 1f;


        public static bool RegenerateSQLiteDBsEachRun = true;

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
            var generalSettings = jsonManager.ReadConfig<GeneralConfig>();
            GameController.seed = generalSettings.seed;
            MinutesGameWillRunFloat = generalSettings.MinutesGameWillRunFloat;
            jsonManager.WriteConfig(generalSettings);

            jsonManager = new JsonManager();
            var economySettings = jsonManager.ReadConfig<EconomyConfig>();
            TicksPerSecond = economySettings.TicksPerSecond;
            EconomyEventChancePerTick = economySettings.EconomyEventChancePerTick;
            EconomyEventEffectWholeFactionChance = economySettings.EconomyEventEffectWholeFactionChance;
            EconomyEventChanceToRemove = economySettings.EconomyEventChanceToRemove;
            AverageEconomyItemsProducedPerTick = economySettings.AverageEconomyItemsProducedPerTick;
            AverageEconomyItemsUsedPerTick = economySettings.AverageEconomyItemsUsedPerTick;
            MaxTradeStationsPerFaction = economySettings.MaxTradeStationsPerFaction;
            MaxInternalTradeRoutesPerTradeStation = economySettings.MaxInternalTradeRoutesPerStation;
            MaxExternalTradeRoutesPerTradeStation = economySettings.MaxExternalTradeRoutesPerStation;
            MaxAttemptsToGenerateSomething = economySettings.MaxAttemptsToGenerateSomething;
            MinTradeStationsPerFaction = economySettings.MinTradeStationsPerFaction;
            AverageMoneyHeldPerTradeStation = economySettings.AverageMoneyHeldPerTradeStation;
            AverageNumItemsExchangedPerTrade = economySettings.AverageNumItemsExchangedPerTrade;
            jsonManager.WriteConfig(economySettings);
        }
    }
}