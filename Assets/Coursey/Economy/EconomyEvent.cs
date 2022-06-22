using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Economy
{
    public class EconomyEvent
    {
        private const bool _debugThisClass = true;

        public float itemEffectFactor = 1f;
        public List<Faction> factionsEffected = new();
        public string EventName
        {
            get { return _EventName; }
            set { _EventName = string.IsNullOrEmpty(value) ? _EventName = "Unnamed event" : _EventName = value; }
        }
        private string _EventName = string.Empty;

        public string Description
        {
            get { return _Description; }
            set { _Description = string.IsNullOrEmpty(value) ? "No description provided" : value; }
        }
        private string _Description = string.Empty;

        public List<ItemClass> ItemClassesEffectedByEvent
        {
            get { return _ItemClassesEffectedByEvent; }
            set
            {
                if (value?.Count > 0)
                {
                    _ItemClassesEffectedByEvent.Clear();
                    foreach (ItemClass itemClass in value)
                    {
                        if (!_ItemClassesEffectedByEvent.Contains(itemClass))
                        {
                            _ItemClassesEffectedByEvent.Add(itemClass);
                        }
                    }
                }
                else
                {
                    _ItemClassesEffectedByEvent = new List<ItemClass> { ItemClass.None };
                }
            }
        }
        private List<ItemClass> _ItemClassesEffectedByEvent = new() { ItemClass.None };

        public EconomyEvent(string name, string description)
        {
            EventName = name;
            Description = description;
        }
        public EconomyEvent(string name, string description, List<ItemClass> itemClassesImpactedByEvent, float factor)
        {
            EventName = name;
            Description = description;
            ItemClassesEffectedByEvent = itemClassesImpactedByEvent;
            this.itemEffectFactor = factor;
        }

        public int TriggerEvent(Faction faction)
        {
            int numTriggered = 0;
            if (!factionsEffected.Contains(faction))
            {
                //do something
                factionsEffected.Add(faction);
                foreach(TradeStation tradeStation in faction.tradeStations)
                {
                    if (!tradeStation.economyEvents.Any(_ => _.EventName == EventName))
                    //if (!tradeStation.economyEvents.Contains(this))
                    {
                        tradeStation.economyEvents.Add(this);
                        numTriggered++;
#pragma warning disable CS0162 // Unreachable code detected
                        if (_debugThisClass) Debug.Log($"The {EventName} Event has been triggered on Faction {faction.factionName}!");
#pragma warning restore CS0162 // Unreachable code detected
                    }
                }
            }
            return numTriggered;
        }
        public int TriggerEvent(TradeStation tradeStation)
        {
            if (!tradeStation.economyEvents.Any(_ => _.EventName == EventName))
            //if (!tradeStation.economyEvents.Contains(this))
            {
                //do something
                tradeStation.economyEvents.Add(this);
#pragma warning disable CS0162 // Unreachable code detected
                if (_debugThisClass) Debug.Log($"The {EventName} Event has been triggered on {tradeStation.associatedFaction.factionName}'s Trade Station {tradeStation.tradeStationName}!");
#pragma warning restore CS0162 // Unreachable code detected
                return 1;
            }
            else
            {
                return 0;
            }
        }
        public int StopEvent(Faction faction)
        {
            int numTriggered = 0;
            if (factionsEffected.Contains(faction))
            {
                //do something
                factionsEffected.Remove(faction);
                foreach (TradeStation tradeStation in faction.tradeStations)
                {
                    if (tradeStation.economyEvents.Contains(this))
                    {
                        tradeStation.economyEvents.Remove(this);
                        numTriggered++;
                    }
                }
#pragma warning disable CS0162 // Unreachable code detected
                if (_debugThisClass) Debug.Log($"The {EventName} Event has been concluded for the Faction {faction.factionName}!");
#pragma warning restore CS0162 // Unreachable code detected
            }
            return numTriggered;
        }
        public int StopEvent(TradeStation tradeStation)
        {
            if (tradeStation.economyEvents.Contains(this))
            {
                //do something
                tradeStation.economyEvents.Remove(this);
#pragma warning disable CS0162 // Unreachable code detected
                if (_debugThisClass) Debug.Log($"The {EventName} Event has been concluded for the Faction {tradeStation.associatedFaction.factionName}'s Trade Station {tradeStation.tradeStationName}!");
#pragma warning restore CS0162 // Unreachable code detected
                return 1;
            }
            else
            {
                return 0;
            }
        }
        public string GetItemClassesImpactedByThisEvent()
        {
            return ItemClassesEffectedByEvent?.Count > 0
                ? string.Join("\n", ItemClassesEffectedByEvent.Select(x => x))
                : "None";
        }
        public override string ToString()
        {
            return
                $"Event name: {EventName}\n" +
                $"Event description: {Description}\n" +
                $"Item classes effected by this event: \n{GetItemClassesImpactedByThisEvent()}\n\n";
        }
    }
}