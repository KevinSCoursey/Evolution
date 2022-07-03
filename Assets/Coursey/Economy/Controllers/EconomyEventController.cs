using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Economy
{
    public static class EconomyEventController
    {
        private const bool _debugThisClass = true;

        public static bool IsReady = false;
        public static List<EconomyEvent> economyEvents = new ();
        public static int ConcurrentEvents
        {
            get { return _ConcurrentEvents; }
            set
            {
#pragma warning disable CS0162 // Unreachable code detected
                if (_debugThisClass)
                {
                    if (_ConcurrentEvents != value)
                    {
                        Debug.Log($"There are {value} concurrent Economic Events.");
                    }

                    _ConcurrentEvents = value;
                }
#pragma warning restore CS0162 // Unreachable code detected
            }
        }
        private static int _ConcurrentEvents = 0;

        public static void Initialize()//good
        {
            IsReady = false;
            AddDefaultEconomyEvents();
            LogAllEconomyEvents();
            DataBaseInteract.UpdateEconomyEventData(economyEvents);
            IsReady = true;
        }
        public static void AddDefaultEconomyEvents()
        {
            //DEFAULT EVENTS
            EconomyEvent eventToAdd = new EconomyEvent(
                name: "Pandemic",
                description: "Something, something, 6 feet and masks.",
                itemClassesImpactedByEvent: new() { ItemClass.Medical },
                factor: 0.55f
                );
            economyEvents.Add(eventToAdd);

            eventToAdd = new EconomyEvent(
                name: "Civil War",
                description: "Internal conflict within the Faction! Increased demand and use of mulitary equipment.",
                itemClassesImpactedByEvent: new() { ItemClass.Military },
                factor: 0.85f
                );
            economyEvents.Add(eventToAdd);

            eventToAdd = new EconomyEvent(
                name: "Faction War",
                description: "Direct conflict with another Faction! Significantly increased demand and use of military equipment.",
                itemClassesImpactedByEvent: new() { ItemClass.Military, ItemClass.Ship },
                factor: 0.65f
                );
            economyEvents.Add(eventToAdd);

            eventToAdd = new EconomyEvent(
                name: "Economic Boom",
                description: "Improved production!",
                itemClassesImpactedByEvent: new() { ItemClass.Military, ItemClass.Construction, ItemClass.Ship, ItemClass.Produce, ItemClass.Generic },
                factor: 1.15f
                );
            economyEvents.Add(eventToAdd);

            eventToAdd = new EconomyEvent(
                name: "Planetary Enviromental Disaster",
                description: "Significantly reduced production.",
                itemClassesImpactedByEvent: new() { ItemClass.Generic, ItemClass.Military, ItemClass.Construction, ItemClass.Ship, ItemClass.Produce },
                factor: 0.45f
                );
            economyEvents.Add(eventToAdd);
        }//good
        public static void GameLoop()
        {
            IsReady = false;
            if (ShouldEventBeTriggered())
            {
                ConcurrentEvents += TriggerRandomEvent(FactionController.GetRandomFaction(), ShouldEventBeOnWholeFaction());
            }
            if (ShouldEventBeRemoved())
            {
                ConcurrentEvents -= RemoveRandomEvent(FactionController.GetRandomFaction(), ShouldEventBeOnWholeFaction());
            }
            IsReady = true;
        }
        private static bool ShouldEventBeTriggered()
        {
            if(MathTools.PseudoRandomIntExclusiveMax(1,1000) <= (int)(GameSettings.EconomyEventChancePerTick * 1000))
            {
                return true;
            }
            return false;
        }
        private static bool ShouldEventBeOnWholeFaction()
        {
            if (MathTools.PseudoRandomIntExclusiveMax(1, 1000) <= (int)(GameSettings.EconomyEventEffectWholeFactionChance * 1000))
            {
                return true;
            }
            return false;
        }
        private static bool ShouldEventBeRemoved()
        {
            if (MathTools.PseudoRandomIntExclusiveMax(1, 1000) <= (int)(GameSettings.EconomyEventChanceToRemove * 1000) * ConcurrentEvents)
            {
                return true;
            }
            return false;
        }
        public static void LogAllEconomyEvents()
        {
#pragma warning disable CS0162 // Unreachable code detected
            if (_debugThisClass)
            {
                foreach (var eEvent in economyEvents)
                {
                    Debug.Log($"Added the following event to the economy...\n\n{eEvent}");
                }
            }
#pragma warning restore CS0162 // Unreachable code detected
        }
        public static int TriggerRandomEvent(Faction faction, bool impactEntireFaction = false)
        {
            int economyEventIndex = MathTools.PseudoRandomIntExclusiveMax(0, economyEvents.Count);
            if (impactEntireFaction)
            {
                return economyEvents[economyEventIndex].TriggerEvent(faction);
            }
            else
            {
                int tradeStationIndex = MathTools.PseudoRandomIntExclusiveMax(0, faction.TradeStations.Count);
                return economyEvents[economyEventIndex].TriggerEvent(faction.TradeStations[tradeStationIndex]);
            }
        }
        public static void RemoveRandomEvent(Faction faction)
        {

        }
        public static void RemoveRandomEvent(TradeStation tradeStation)
        {

        }
        public static int RemoveRandomEvent(Faction faction, bool impactEntireFaction = false)
        {
            int economyEventIndex = MathTools.PseudoRandomIntExclusiveMax(0, economyEvents.Count);
            if (impactEntireFaction)
            {
                return economyEvents[economyEventIndex].StopEvent(faction);
                //possibility that a trade station or faction is found after event triggered so removing from all will make event count get lower than representative
            }
            else
            {
                int tradeStationIndex = MathTools.PseudoRandomIntExclusiveMax(0, faction.TradeStations.Count);
                return economyEvents[economyEventIndex].StopEvent(faction.TradeStations[tradeStationIndex]);
            }
        }
    }
}
