using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Economy
{
    public class TradeStation
    {
        private List<Faction> factions = new();
        private List<EconomyItem> economyItems = new();

        public string tradeStationName = string.Empty;
        public string tradeStationDescription = string.Empty;

        public Faction associatedFaction;
        public Dictionary<EconomyItem, int> items = new Dictionary<EconomyItem, int>();//item, price
        public Dictionary<EconomyItem, int> inventory = new Dictionary<EconomyItem, int>();//item, quantity

        public TradeStation(List<Faction> factions, Faction associatedFaction, List<EconomyItem> economyItems, string tradeStationName = "Unnamed station", string tradeStationDescription = "No description provided.")
        {
            this.factions = factions;
            this.associatedFaction = associatedFaction;
            this.tradeStationName = tradeStationName;
            this.tradeStationDescription = tradeStationDescription;
            this.economyItems = economyItems;
            AddItemsWithSpecialization();
            InitializeInventory();
        }

        private void AddItemsWithSpecialization()
        {
            foreach(EconomyItem economyItem in economyItems)
            {
                if (economyItem.FactionsThatSpecializeInThisItem.Contains(associatedFaction))
                {
                    items.Add(economyItem, economyItem.PriceDefault);
                }
            }
        }

        private void InitializeInventory()
        {
            foreach(EconomyItem economyItem in economyItems)
            {
                if (economyItem.FactionsThatSpecializeInThisItem.Contains(associatedFaction))
                {
                    inventory.Add(economyItem, 5);
                }
            }
        }

        public void CalculatePriceDistribution()
        {

        }

        public string LogItemsAvailable()
        {
            string returnString = string.Empty;
            foreach(KeyValuePair<EconomyItem, int> economyItem in items)
            {
                returnString += $"Item: {economyItem.Key.ItemName} Price: {items[economyItem.Key]} Quantity: {inventory[economyItem.Key]}\n";
            }
            return returnString == string.Empty ? "None" : returnString;
        }

        public override string ToString()
        {
            return
                $"Station name: {tradeStationName}\n" +
                $"Station description: {tradeStationDescription}\n" +
                $"Items available:\n{LogItemsAvailable()}";
        }
    }
}
