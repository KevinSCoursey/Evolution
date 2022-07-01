using Mono.Data.Sqlite;
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
        public string eventId;
        public string eventName
        {
            get { return _eventName; }
            set { _eventName = string.IsNullOrEmpty(value) ? _eventName = "Unnamed event" : _eventName = value; }
        }
        private string _eventName = string.Empty;

        public string eventDescription
        {
            get { return _eventDescription; }
            set { _eventDescription = string.IsNullOrEmpty(value) ? "No description provided" : value; }
        }
        private string _eventDescription = string.Empty;

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
        private SqliteDataReader rowData;

        public EconomyEvent(string name, string description)
        {
            eventName = name;
            eventDescription = description;
        }
        public EconomyEvent(string name, string description, List<ItemClass> itemClassesImpactedByEvent, float factor)
        {
            eventName = name;
            eventDescription = description;
            ItemClassesEffectedByEvent = itemClassesImpactedByEvent;
            this.itemEffectFactor = factor;
        }

        public EconomyEvent(SqliteDataReader rowData)
        {
            eventName = rowData["EventName"].ToString();
            eventDescription = rowData["EventDescription"].ToString();
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
                    if (!tradeStation.economyEvents.Any(_ => _.eventName == eventName))
                    //if (!tradeStation.economyEvents.Contains(this))
                    {
                        tradeStation.economyEvents.Add(this);
                        numTriggered++;
#pragma warning disable CS0162 // Unreachable code detected
                        if (_debugThisClass) Debug.Log($"The {eventName} Event has been triggered on Faction {faction.factionName}!");
#pragma warning restore CS0162 // Unreachable code detected
                    }
                }
            }
            return numTriggered;
        }
        public int TriggerEvent(TradeStation tradeStation)
        {
            if (!tradeStation.economyEvents.Any(_ => _.eventName == eventName))
            //if (!tradeStation.economyEvents.Contains(this))
            {
                //do something
                tradeStation.economyEvents.Add(this);
#pragma warning disable CS0162 // Unreachable code detected
                if (_debugThisClass) Debug.Log($"The {eventName} Event has been triggered on {tradeStation.associatedFaction.factionName}'s Trade Station {tradeStation.tradeStationName}!");
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
                if (_debugThisClass) Debug.Log($"The {eventName} Event has been concluded for the Faction {faction.factionName}!");
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
                if (_debugThisClass) Debug.Log($"The {eventName} Event has been concluded for the Faction {tradeStation.associatedFaction.factionName}'s Trade Station {tradeStation.tradeStationName}!");
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
                $"Event name: {eventName}\n" +
                $"Event description: {eventDescription}\n" +
                $"Item classes effected by this event: \n{GetItemClassesImpactedByThisEvent()}\n\n";
        }
    }
}