using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Economy
{
    public class EconomyEventController
    {
        private const bool _debugThisClass = true;

        public List<EconomyEvent> economyEvents = new ();

        public EconomyEventController()
        {
            Initialize();
        }
        public void Initialize()
        {
            AddDefaultEconomyEvents();

            //Just makes this not cause a compile warning
#pragma warning disable CS0162 // Unreachable code detected
            if (_debugThisClass) LogAllEconomyEvents();
#pragma warning restore CS0162 // Unreachable code detected
        }
        public void AddDefaultEconomyEvents()
        {
            //DEFAULT EVENTS
            EconomyEvent eventToAdd = new EconomyEvent(
                name: "Pandemic",
                description: "Something, something, 6 feet and masks.",
                itemClassesImpactedByEvent: new () { ItemClass.None }
                );
            economyEvents.Add(eventToAdd);

            eventToAdd = new EconomyEvent(
                name: "Civil War",
                description: "Increased demand and use of mulitary equipment.",
                itemClassesImpactedByEvent: new() { ItemClass.Military }
                );
            economyEvents.Add(eventToAdd);

            eventToAdd = new EconomyEvent(
                name: "Faction War",
                description: "Significantly increased demand and use of military equipment.",
                itemClassesImpactedByEvent: new() { ItemClass.Military, ItemClass.Ship }
                );
            economyEvents.Add(eventToAdd);

            eventToAdd = new EconomyEvent(
                name: "Economic Boom",
                description: "Improved production!",
                itemClassesImpactedByEvent: new() { ItemClass.Military, ItemClass.Construction, ItemClass.Ship, ItemClass.Produce, ItemClass.Generic }
                );
            economyEvents.Add(eventToAdd);

            eventToAdd = new EconomyEvent(
                name: "Planetary Enviromental Disaster",
                description: "Significantly reduced production.",
                itemClassesImpactedByEvent: new() { ItemClass.Generic, ItemClass.Military, ItemClass.Construction, ItemClass.Ship, ItemClass.Produce }
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
        public void TriggerRandomEvent(Faction faction, bool impactEntireFaction = false)
        {
            int economyEventIndex = MathTools.PseudoRandomInt(0, economyEvents.Count);
            if (impactEntireFaction)
            {
                economyEvents[economyEventIndex].TriggerEvent(faction);
            }
            else
            {
                int tradeStationIndex = MathTools.PseudoRandomInt(0, faction.tradeStations.Count);
                economyEvents[economyEventIndex].TriggerEvent(faction.tradeStations[tradeStationIndex]);
            }
        }
    }
}
