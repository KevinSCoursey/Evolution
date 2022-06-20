using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Economy
{
    public class EconomyEvent
    {
        private const bool _debugThisClass = true;
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
        public EconomyEvent(string name, string description, List<ItemClass> itemClassesImpactedByEvent)
        {
            EventName = name;
            Description = description;
            ItemClassesEffectedByEvent = itemClassesImpactedByEvent;
        }

        public void TriggerEvent(Faction faction)
        {
#pragma warning disable CS0162 // Unreachable code detected
            if (_debugThisClass) Debug.Log($"The {EventName} Event has been triggered on Faction {faction.factionName}!");
#pragma warning restore CS0162 // Unreachable code detected
        }
        public void TriggerEvent(TradeStation tradeStation)
        {
#pragma warning disable CS0162 // Unreachable code detected
            if (_debugThisClass) Debug.Log($"The {EventName} Event has been triggered on {tradeStation.associatedFaction.factionName}'s Trade Station {tradeStation.tradeStationName}!");
#pragma warning restore CS0162 // Unreachable code detected
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
                $"Item name: {EventName}\n" +
                $"Item description: {Description}\n" +
                $"Item classes effected by this event: \n{GetItemClassesImpactedByThisEvent()}\n\n";
        }
    }
}