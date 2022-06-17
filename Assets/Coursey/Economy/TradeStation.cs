using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Economy
{
    public class TradeStation
    {
        private List<Faction> factions = new();
        private List<EconomyItem> economyItems = new();
        private System.Random random;

        public string tradeStationName = string.Empty;
        public string tradeStationDescription = string.Empty;

        public Faction associatedFaction;
        //public Dictionary<EconomyItem, int> items = new Dictionary<EconomyItem, int>();//item, price
        //public Dictionary<EconomyItem, int> inventory = new Dictionary<EconomyItem, int>();//item, quantity
        public List<EconomyItem> specializedItems = new();
        public List<EconomyItem> inventoryItems = new();

        public TradeStation(List<Faction> factions, Faction associatedFaction, List<EconomyItem> economyItems, System.Random random, string tradeStationName = "Unnamed station", string tradeStationDescription = "No description provided.")
        {
            this.factions = factions;
            this.associatedFaction = associatedFaction;
            this.tradeStationName = tradeStationName;
            this.tradeStationDescription = tradeStationDescription;
            this.economyItems = economyItems;
            this.random = random;
            InitializeInventory();
            CalculateItemDistribution();
            CalculatePriceDistribution();
        }

        private void InitializeInventory()
        {
            foreach(EconomyItem economyItem in economyItems)
            {
                if (economyItem.FactionsThatSpecializeInThisItem.Contains(associatedFaction))
                {
                    inventoryItems.Add(new EconomyItem(economyItem));
                }
            }
        }

        public void CalculatePriceDistribution()
        {

        }
        public void CalculateItemDistribution()
        {
            foreach(IEconomyItem item in inventoryItems)
            {
                item.QuantityOfItem = random.Next(0, 10000);//max currently 10000
            }
        }

        public string LogItemsAvailable()
        {
            string returnString = string.Empty;
            /*foreach(KeyValuePair<EconomyItem, int> economyItem in items)
            {
                returnString += $"Item: {economyItem.Key.ItemName} Price: {items[economyItem.Key]} Quantity: {inventory[economyItem.Key]}\n";
            }*/
            foreach(IEconomyItem item in inventoryItems)
            {
                returnString += $"\n{item.ItemName} | Buy: ${item.PurchasePrice} | Sell: ${item.SalePrice} | Quantity: {item.QuantityOfItem}";
            }
            return returnString == string.Empty ? "None" : returnString;
        }

        public override string ToString()
        {
            return
                $"Station name: {tradeStationName}\n" +
                $"Station description: {tradeStationDescription}\n" +
                $"Items available:{LogItemsAvailable()}";
        }
    }
}
