using System.Collections.Generic;
using UnityEngine;

namespace Economy
{
    public class TradeStation
    {
        private List<Faction> factions = new();
        private List<EconomyItem> economyItems = new();
        private const float _purchasePriceMultiplier = 1.5f;
        private const float _salePriceMultiplier = 1.25f;

        public string tradeStationName = string.Empty;
        public string tradeStationDescription = string.Empty;

        public Faction associatedFaction;
        //public Dictionary<EconomyItem, int> items = new Dictionary<EconomyItem, int>();//item, price
        //public Dictionary<EconomyItem, int> inventory = new Dictionary<EconomyItem, int>();//item, quantity
        public List<EconomyItem> specializedItems = new();
        public List<EconomyItem> inventoryItems = new();

        public TradeStation(List<Faction> factions, Faction associatedFaction, List<EconomyItem> economyItems, string tradeStationName = "Unnamed station", string tradeStationDescription = "No description provided.")
        {
            this.factions = factions;
            this.associatedFaction = associatedFaction;
            this.tradeStationName = tradeStationName;
            this.tradeStationDescription = tradeStationDescription;
            this.economyItems = economyItems;
            InitializeInventory();
            CalculateItemDistribution();
            CalculatePriceDistribution();
        }

        private void InitializeInventory()
        {
            foreach (EconomyItem economyItem in economyItems)
            {
                if (economyItem.FactionsThatSpecializeInThisItem.Contains(associatedFaction))
                {
                    inventoryItems.Add(new EconomyItem(economyItem));
                }
            }
        }

        public void CalculatePriceDistribution()
        {
            int[,] pseudoRandomIntPairArray = MathTools.PseudoRandomIntPairArray(inventoryItems.Count, 1, 20, true);//inventory volatility 3 - 13, just a weight factor
            for (int i = 0; i < inventoryItems.Count; i++)
            {
                if (inventoryItems[i].FactionsThatSpecializeInThisItem.Contains(associatedFaction))//if faction specializes, less interest in purchase from player
                {
                    inventoryItems[i].PurchasePrice = (int)(pseudoRandomIntPairArray[i, 0] * inventoryItems[i].PriceVolatilityFactor * _purchasePriceMultiplier);
                    inventoryItems[i].SalePrice = (int)(pseudoRandomIntPairArray[i, 1] / inventoryItems[i].PriceVolatilityFactor * _salePriceMultiplier / 2);
                }
                else
                {
                    inventoryItems[i].PurchasePrice = (int)(pseudoRandomIntPairArray[i, 0] * inventoryItems[i].PriceVolatilityFactor * _purchasePriceMultiplier);
                    inventoryItems[i].SalePrice = (int)(pseudoRandomIntPairArray[i, 1] / inventoryItems[i].PriceVolatilityFactor * _salePriceMultiplier);
                }
            }
        }
        public void CalculateItemDistribution()
        {
            foreach (IEconomyItem item in inventoryItems)
            {
                item.QuantityOfItem = MathTools.PseudoRandomInt(0, 10000);//max currently 10000
            }
        }

        public string LogItemsAvailable()
        {
            string returnString = string.Empty;
            /*foreach(KeyValuePair<EconomyItem, int> economyItem in items)
            {
                returnString += $"Item: {economyItem.Key.ItemName} Price: {items[economyItem.Key]} Quantity: {inventory[economyItem.Key]}\n";
            }*/
            foreach (IEconomyItem item in inventoryItems)
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
