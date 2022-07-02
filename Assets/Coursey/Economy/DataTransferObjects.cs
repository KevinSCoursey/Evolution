using System.Collections;
using System.Collections.Generic;

namespace Economy
{
    public class EconomyConfig : IDataTransferObject
    {
        public float TicksPerSecond { get; set; } = 1;
        public float EconomyEventChancePerTick { get; set; } = 0.015f;
        public float EconomyEventEffectWholeFactionChance { get; set; } = 0.015f;//multiply by 1000 then take it as a chance out of 1000
        public float EconomyEventChanceToRemove { get; set; } = 0.025f;
        public int AverageEconomyItemsProducedPerTick { get; set; } = 5;
        public int AverageEconomyItemsUsedPerTick { get; set; } = 5;
        public int MinTradeStationsPerFaction { get; set; } = 1;
        public int MaxTradeStationsPerFaction { get; set; } = 10;
        public int MaxInternalTradeRoutesPerStation { get; set; } = 3;
        public int MaxExternalTradeRoutesPerStation { get; set; } = 3;
        public int MaxAttemptsToGenerateSomething { get; set; } = 5;
        public int AverageMoneyHeldPerTradeStation { get; set; } = 150000;
        public int AverageNumItemsExchangedPerTrade { get; set; } = 3;
        public float PercentTaxExternalTrades { get; set; } = 1.5f;
        public float SameFactionPriceDiscount { get; set; } = 1.5f;
        public int AverageMaxQuantityOfItem { get; set; } = 10000;

    }
    public class GeneralConfig : IDataTransferObject
    {
        public int seed = 8675309;
        public float MinutesGameWillRunFloat { get; set; } = 0.1f;
        public bool RegenerateSQLiteDBsEachRun { get; set; } = true;
        public int NumTradeStationsPerDBBlock { get; set; } = 1000;
    }
    public interface IDataTransferObject { }
}