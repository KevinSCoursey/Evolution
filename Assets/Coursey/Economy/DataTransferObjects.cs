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
        public int MinTradeStationsPerFaction { get; set; } = 2000000;//1;
        public int MaxTradeStationsPerFaction { get; set; } = 2000000;//10;
        public int MaxInternalTradeRoutesPerStation { get; set; } = 8;//3;
        public int MaxExternalTradeRoutesPerStation { get; set; } = 8;//3;
        public int MaxAttemptsToGenerateSomething { get; set; } = 5;
        public int AverageMoneyHeldPerTradeStation { get; set; } = 150000;
        public int AverageNumItemsExchangedPerTrade { get; set; } = 3;

    }
    public class GeneralConfig : IDataTransferObject
    {
        public int seed = 8675309;
        public float MinutesGameWillRunFloat { get; set; } = 0.1f;
    }
    public interface IDataTransferObject
    {
    }
}