using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Economy
{
    public class EconomyEventController
    {
        private const bool _debugThisClass = true;
        private FactionController factionController;

        public List<EconomyEvent> economyEvents = new ();
        public int ConcurrentEvents
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
        private int _ConcurrentEvents = 0;

        public EconomyEventController(FactionController factionController)
        {
            this.factionController = factionController;
            //Initialize();
        }
        public void Initialize()
        {
            AddDefaultEconomyEvents();

            //Just makes this not cause a compile warning
#pragma warning disable CS0162 // Unreachable code detected
            if (_debugThisClass) LogAllEconomyEvents();
#pragma warning restore CS0162 // Unreachable code detected
        }
        public void GameLoop()
        {
            if (ShouldEventBeTriggered())
            {
                ConcurrentEvents += TriggerRandomEvent(factionController.GetRandomFaction(), ShouldEventBeOnWholeFaction());
            }
            if (ShouldEventBeRemoved())
            {
                ConcurrentEvents -= RemoveRandomEvent(factionController.GetRandomFaction(), ShouldEventBeOnWholeFaction());
            }
        }
        private bool ShouldEventBeTriggered()
        {
            if(MathTools.PseudoRandomIntExclusiveMax(1,1000) <= (int)(GameSettings.EconomyEventChancePerTick * 1000))
            {
                return true;
            }
            return false;
        }
        private bool ShouldEventBeOnWholeFaction()
        {
            if (MathTools.PseudoRandomIntExclusiveMax(1, 1000) <= (int)(GameSettings.EconomyEventEffectWholeFactionChance * 1000))
            {
                return true;
            }
            return false;
        }
        private bool ShouldEventBeRemoved()
        {
            if (MathTools.PseudoRandomIntExclusiveMax(1, 1000) <= (int)(GameSettings.EconomyEventChanceToRemove * 1000) * ConcurrentEvents)
            {
                return true;
            }
            return false;
        }
        public void AddDefaultEconomyEvents()//-------------------------DEFAULT EVENTS
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
        }
        public void LogAllEconomyEvents()
        {
            foreach(var eEvent in economyEvents)
            {
                Debug.Log($"Added the following event to the economy...\n\n{eEvent}");
            }
        }
        public int TriggerRandomEvent(Faction faction, bool impactEntireFaction = false)
        {
            int economyEventIndex = MathTools.PseudoRandomIntExclusiveMax(0, economyEvents.Count);
            if (impactEntireFaction)
            {
                return economyEvents[economyEventIndex].TriggerEvent(faction);
            }
            else
            {
                int tradeStationIndex = MathTools.PseudoRandomIntExclusiveMax(0, faction.tradeStations.Count);
                return economyEvents[economyEventIndex].TriggerEvent(faction.tradeStations[tradeStationIndex]);
            }
        }
        public void RemoveRandomEvent(Faction faction)
        {

        }
        public void RemoveRandomEvent(TradeStation tradeStation)
        {

        }
        public int RemoveRandomEvent(Faction faction, bool impactEntireFaction = false)
        {
            int economyEventIndex = MathTools.PseudoRandomIntExclusiveMax(0, economyEvents.Count);
            if (impactEntireFaction)
            {
                return economyEvents[economyEventIndex].StopEvent(faction);
                //possibility that a trade station or faction is found after event triggered so removing from all will make event count get lower than representative
            }
            else
            {
                int tradeStationIndex = MathTools.PseudoRandomIntExclusiveMax(0, faction.tradeStations.Count);
                return economyEvents[economyEventIndex].StopEvent(faction.tradeStations[tradeStationIndex]);
            }
        }
    }
}
