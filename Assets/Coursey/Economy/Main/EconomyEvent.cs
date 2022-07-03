using Mono.Data.Sqlite;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Economy
{
    public class EconomyEvent
    {
        private const bool _debugThisClass = true;

        public float ItemEffectFactor = 1f;
        public List<Faction> FactionsEffected = new();
        public string EconomyEventId;
        public string EconomyEventName
        {
            get { return _eventName; }
            set { _eventName = string.IsNullOrEmpty(value) ? _eventName = "Unnamed event" : _eventName = value; }
        }
        private string _eventName = string.Empty;
        public string EconomyEventDescription
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
        public EconomyEvent(string name, string description)
        {
            EconomyEventName = name;
            EconomyEventDescription = description;
        }
        public EconomyEvent(string name, string description, List<ItemClass> itemClassesImpactedByEvent, float factor)
        {
            EconomyEventName = name;
            EconomyEventDescription = description;
            ItemClassesEffectedByEvent = itemClassesImpactedByEvent;
            ItemEffectFactor = factor;
        }
        public EconomyEvent(SqliteDataReader rowData)
        {
            EconomyEventName = rowData["EconomyEventName"].ToString();
            EconomyEventDescription = rowData["EconomyEventDescription"].ToString();
        }
        public int TriggerEvent(Faction faction)
        {
            int numTriggered = 0;
            if (!FactionsEffected.Contains(faction))
            {
                //do something
                FactionsEffected.Add(faction);
                foreach(TradeStation tradeStation in faction.TradeStations)
                {
                    if (!tradeStation.EconomyEvents.Any(_ => _.EconomyEventName == EconomyEventName))
                    //if (!tradeStation.economyEvents.Contains(this))
                    {
                        tradeStation.EconomyEvents.Add(this);
                        numTriggered++;
#pragma warning disable CS0162 // Unreachable code detected
                        if (_debugThisClass) Debug.Log($"The {EconomyEventName} Event has been triggered on Faction {faction.FactionName}!");
#pragma warning restore CS0162 // Unreachable code detected
                    }
                }
            }
            return numTriggered;
        }
        public int TriggerEvent(TradeStation tradeStation)
        {
            if (!tradeStation.EconomyEvents.Any(_ => _.EconomyEventName == EconomyEventName))
            //if (!tradeStation.economyEvents.Contains(this))
            {
                //do something
                tradeStation.EconomyEvents.Add(this);
#pragma warning disable CS0162 // Unreachable code detected
                if (_debugThisClass) Debug.Log($"The {EconomyEventName} Event has been triggered on {tradeStation.AssociatedFaction.FactionName}'s Trade Station {tradeStation.TradeStationName}!");
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
            if (FactionsEffected.Contains(faction))
            {
                //do something
                FactionsEffected.Remove(faction);
                foreach (TradeStation tradeStation in faction.TradeStations)
                {
                    if (tradeStation.EconomyEvents.Contains(this))
                    {
                        tradeStation.EconomyEvents.Remove(this);
                        numTriggered++;
                    }
                }
#pragma warning disable CS0162 // Unreachable code detected
                if (_debugThisClass) Debug.Log($"The {EconomyEventName} Event has been concluded for the Faction {faction.FactionName}!");
#pragma warning restore CS0162 // Unreachable code detected
            }
            return numTriggered;
        }
        public int StopEvent(TradeStation tradeStation)
        {
            if (tradeStation.EconomyEvents.Contains(this))
            {
                //do something
                tradeStation.EconomyEvents.Remove(this);
#pragma warning disable CS0162 // Unreachable code detected
                if (_debugThisClass) Debug.Log($"The {EconomyEventName} Event has been concluded for the Faction {tradeStation.AssociatedFaction.FactionName}'s Trade Station {tradeStation.TradeStationName}!");
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
                $"Event name: {EconomyEventName}\n" +
                $"Event description: {EconomyEventDescription}\n" +
                $"Item classes effected by this event: \n{GetItemClassesImpactedByThisEvent()}\n\n";
        }
    }
}