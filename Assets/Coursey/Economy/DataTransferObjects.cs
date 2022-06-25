using System.Collections;
using System.Collections.Generic;

namespace Economy
{
    public class EconomyConfig : IDataTransferObject
    {
        public static float TicksPerSecond { get; set; } = 1;
        public static float EconomyEventChancePerTick { get; set; } = 0.015f;
        public static float EconomyEventEffectWholeFactionChance { get; set; } = 0.015f;//multiply by 1000 then take it as a chance out of 1000
        public static float EconomyEventChanceToRemove { get; set; } = 0.025f;
        public static int AverageEconomyItemsProducedPerTick { get; set; } = 5;
        public static int AverageEconomyItemsUsedPerTick { get; set; } = 5;
        public static int MaxTradeStationsPerFaction { get; set; } = 10;
    }
    public class GeneralSettings : IDataTransferObject
    {
        public int seed = 8675309;
        public float MinutesGameWillRun { get; set; } = 1;
    }
    public interface IDataTransferObject
    {
    }
}