using System.Collections.Generic;
using UnityEngine;

namespace Economy
{
    public class TradeStation
    {
        private List<Faction> factions = new();
        private List<EconomyItem> economyItems = new();
        private const float _purchasePriceMultiplier = 1.25f;
        private const float _salePriceMultiplier = 0.75f;

        public string tradeStationName = string.Empty;
        public string tradeStationDescription = string.Empty;
        public List<EconomyEvent> economyEvents = new();

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
            /*int[,] pseudoRandomIntPairArray = MathTools.PseudoRandomIntPairArray(inventoryItems.Count, 1, 4, true);//inventory volatility 3 - 13, just a weight factor
            for (int i = 0; i < inventoryItems.Count; i++)
            {
                if (inventoryItems[i].FactionsThatSpecializeInThisItem.Contains(associatedFaction))//if faction specializes, less interest in purchase from player
                {
                    inventoryItems[i].PurchasePrice = (int)(inventoryItems[i].PriceDefault * pseudoRandomIntPairArray[i, 0] * inventoryItems[i].PriceVolatilityFactor * _purchasePriceMultiplier);
                    inventoryItems[i].SalePrice = (int)(inventoryItems[i].PriceDefault * pseudoRandomIntPairArray[i, 1] * inventoryItems[i].PriceVolatilityFactor * _salePriceMultiplier * _salePriceMultiplier);
                }
                else//if faction specializes, more interest in purchase from player
                {
                    inventoryItems[i].PurchasePrice = (int)(inventoryItems[i].PriceDefault * pseudoRandomIntPairArray[i, 0] * inventoryItems[i].PriceVolatilityFactor * _purchasePriceMultiplier * _purchasePriceMultiplier);
                    inventoryItems[i].SalePrice = (int)(inventoryItems[i].PriceDefault * pseudoRandomIntPairArray[i, 1] * inventoryItems[i].PriceVolatilityFactor * _salePriceMultiplier);
                }
            }*/
            foreach(IEconomyItem item in inventoryItems)
            {
                item.PurchasePrice = MathTools.CalculateItemPurchasePrice(item, item.FactionsThatSpecializeInThisItem.Contains(associatedFaction));
                item.SalePrice = MathTools.CalculateItemSalePrice(item, item.FactionsThatSpecializeInThisItem.Contains(associatedFaction));
            }
        }
        public void CalculateItemDistribution()
        {
            foreach (IEconomyItem item in inventoryItems)
            {
                item.QuantityOfItem = MathTools.PseudoRandomInt(0, item.MaxQuantityOfItem);//max currently 10000
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
                returnString += $"\n{item.ItemName} | Buy: ${item.PurchasePrice} / {item.PriceRoof} | Sell: ${item.SalePrice} / {item.PriceFloor} | Quantity: {item.QuantityOfItem} / {item.MaxQuantityOfItem}";
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
