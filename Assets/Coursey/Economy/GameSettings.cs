using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Economy
{
    public class GameSettings
    {
        public static int TicksPerSecond = 10;
        public static float EconomyEventChancePerTick = 0.015f;
        public static float EconomyEventEffectWholeFactionChance = 0.015f;//multiply by 1000 then take it as a chance out of 1000
        public static float EconomyEventChanceToRemove = 0.025f;
        public static int AverageEconomyItemsProducedPerTick = 5;
        public static int AverageEconomyItemsUsedPerTick = 5;
    }
}