using Mono.Data.Sqlite;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using System;

namespace Economy
{
    public class TradeStation
    {
        private static bool _debugThisClass = true;

        public string TradeStationName = string.Empty;
        public string TradeStationDescription = string.Empty;
        public List<EconomyEvent> EconomyEvents = new();
        public int TradeStationMoney = 0;
        public Faction AssociatedFaction;
        public string FactionId;
        public string TradeStationId;
        public List<ItemClass> EconomyItemClassesSpecialized = new() { ItemClass.Generic };
        public List<EconomyItem> InventoryItems = new();
        public List<TradeRoute> InternalTradeRoutes = new();
        public List<TradeRoute> ExternalTradeRoutes = new();

        public TradeStation(Faction associatedFaction, string tradeStationName = "Unnamed station", string tradeStationDescription = "No description provided.")
        {
            AssociatedFaction = associatedFaction;
            TradeStationName = tradeStationName;
            TradeStationDescription = tradeStationDescription;
            FactionId = associatedFaction.FactionId;
            TradeStationMoney = MathTools.PseudoRandomIntExclusiveMax(GameSettings.AverageMoneyHeldPerTradeStation / 10, GameSettings.AverageMoneyHeldPerTradeStation * 2);
        }
        public void Initialize()
        {
            GenerateNewInventory();
        }
        public TradeStation(SqliteDataReader rowData)
        {
            TradeStationId = rowData["TradeStationId"].ToString();
            FactionId = rowData["FactionId"].ToString();
            TradeStationName = rowData["TradeStationName"].ToString();
            TradeStationDescription = rowData["TradeStationDescription"].ToString();
            TradeStationMoney = (int)Convert.ToInt32(rowData["TradeStationMoney"].ToString());
        }
        private void GenerateNewInventory(bool replaceExisting = false)
        {
            if(InventoryItems.Count == 0 || replaceExisting)
            {
                using (new TimedBlock($"Generating new inventory for trade station {TradeStationName}", _debugThisClass))
                {
                    LoadInventory();
                    AddNewItemToInventory(DataBaseInteract.LoadEconomyItemsWithItemClass(EconomyItemClassesSpecialized));
                    ReCalculateItemDistribution();
                    ReCalculatePriceDistribution();
                    if (_debugThisClass)
                    {
                        Debug.Log($"{this}");
                    }
                }
            }
            else
            {
                LoadInventory();
            }
        }
        public void AddNewItemToInventory(EconomyItem itemToAdd)
        {
            if (!InventoryItems.Any(_ => _.EconomyItemId == itemToAdd.EconomyItemId))
            {
                InventoryItems.Add(itemToAdd);
            }
        }
        public void AddNewItemToInventory(List<EconomyItem> itemsToAdd)
        {
            foreach(var itemToAdd in itemsToAdd)
            {
                if (!InventoryItems.Any(_ => _.EconomyItemId == itemToAdd.EconomyItemId))
                {
                    InventoryItems.Add(itemToAdd);
                }
            }
        }
        public void LoadInventory()
        {
            using (new TimedBlock($"Loading inventory data for {TradeStationName}", _debugThisClass))
            {
                InventoryItems = DataBaseInteract.LoadTradeStationInventory(TradeStationId);
                ReCalculatePriceDistribution();
            }
        }
        public void ReCalculateItemDistribution()
        {
            if(InventoryItems.Count == 0) LoadInventory();
            foreach (EconomyItem item in InventoryItems)
            {
                if (item.MaxQuantityOfItem <= 1) item.MaxQuantityOfItem = MathTools.PseudoRandomIntExclusiveMax(0, GameSettings.AverageMaxQuantityOfItem * 2);
                item.QuantityOfItem = MathTools.PseudoRandomIntExclusiveMax(0, item.MaxQuantityOfItem);
            }
        }
        public void ReCalculatePriceDistribution()
        {
            foreach (EconomyItem item in InventoryItems)
            {
                item.PurchasePrice = MathTools.CalculateItemPurchasePrice(item, item.FactionsThatSpecializeInThisItem.Contains(AssociatedFaction));
                item.SalePrice = MathTools.CalculateItemSalePrice(item, item.FactionsThatSpecializeInThisItem.Contains(AssociatedFaction));
            }
        }
        public void UseItems()
        {
            foreach (var item in InventoryItems)
            {
                float factor = 1f;
                foreach (var eEvent in EconomyEvents)
                {
                    if (eEvent.ItemClassesEffectedByEvent.Contains(item.EconomyItemClass) && eEvent.ItemEffectFactor < 1)
                    {
                        factor *= eEvent.ItemEffectFactor;
                    }
                }
                item.QuantityOfItem = (int)(factor *
                (item.QuantityOfItem - GameSettings.AverageEconomyItemsProducedPerTick -
                MathTools.PseudoRandomIntExclusiveMax(1 * item.RarityInt, 5 * item.RarityInt)));
            }
        }
        public void ProduceItems()
        {
            foreach (EconomyItem item in InventoryItems)
            {
                float factor = 1f;
                foreach (EconomyEvent eEvent in EconomyEvents)
                {
                    if (eEvent.ItemClassesEffectedByEvent.Contains(item.EconomyItemClass) && eEvent.ItemEffectFactor > 1)
                    {
                        factor *= eEvent.ItemEffectFactor;
                    }
                }
                item.QuantityOfItem = (int)(factor *
                (item.QuantityOfItem + GameSettings.AverageEconomyItemsProducedPerTick +
                MathTools.PseudoRandomIntExclusiveMax(1, 5 * item.RarityInt)));
            }
        }
        public void ExecuteAllTrades()
        {
            if (_debugThisClass)
            {
                Debug.Log($"Trade Station {TradeStationName} is beginning all trade routes... \n{this}");
            }
            foreach (TradeRoute tradeRoute in InternalTradeRoutes)
            {
                tradeRoute.AI_ConductTrade();
            }
            if (_debugThisClass)
            {
                Debug.Log($"Internal trades complete.");
            }
            foreach (TradeRoute tradeRoute in ExternalTradeRoutes)
            {
                tradeRoute.AI_ConductTrade();
            }
            if (_debugThisClass)
            {
                Debug.Log($"External trades complete.");
                Debug.Log($"Trade Station {TradeStationName} has concluded all trade routes. \n{this}");
            }
        }
        public List<EconomyItem> ItemsOfInterest(float percentOfMaxQuantity = 0.25f)
        {
            List<EconomyItem> itemsOfInterest = new List<EconomyItem>(InventoryItems.Where(_ => _.QuantityOfItem <= percentOfMaxQuantity * _.MaxQuantityOfItem));

            if (_debugThisClass)
            {
                string debugString = $"{TradeStationName} items of interest are ";
                debugString = debugString + string.Join(", ", itemsOfInterest.Select(_ => _.EconomyItemName));
                Debug.Log(debugString);
            }

            return itemsOfInterest;
        }
        public void AI_Buy(EconomyItem economyItem, TradeStation fromTradeStation)
        {//fromtradestation is the one selling in this case
            int numItemsExchanged = 0;
            int numItemsRequested = MathTools.PseudoRandomIntExclusiveMax(15, GameSettings.AverageNumItemsExchangedPerTrade * 2 + 15);

            var referenceTo_fromTradeStation_economyItem = fromTradeStation.InventoryItems.Find(_ => _.EconomyItemName == economyItem.EconomyItemName);
            var referenceTo_ownTradeStation_economyItem = InventoryItems.Find(_ => _.EconomyItemName == economyItem.EconomyItemName);

            if (referenceTo_fromTradeStation_economyItem == null)
            {
                EconomyItem itemToAdd = new EconomyItem(economyItem);
                itemToAdd.QuantityOfItem = 0;
                itemToAdd.PurchasePrice = MathTools.CalculateItemPurchasePrice(itemToAdd, itemToAdd.FactionsThatSpecializeInThisItem.Contains(AssociatedFaction));
                itemToAdd.SalePrice = MathTools.CalculateItemSalePrice(itemToAdd, itemToAdd.FactionsThatSpecializeInThisItem.Contains(AssociatedFaction));
                fromTradeStation.InventoryItems.Add(itemToAdd);
                referenceTo_fromTradeStation_economyItem = itemToAdd;
                if (_debugThisClass)
                {
                    Debug.Log($"{fromTradeStation.TradeStationName} didn't have {economyItem.EconomyItemName} in their inventory, so it was added:\n {itemToAdd}");
                }
            }

            if (referenceTo_ownTradeStation_economyItem == null)
            {
                EconomyItem itemToAdd = new EconomyItem(economyItem);
                itemToAdd.QuantityOfItem = 0;
                itemToAdd.PurchasePrice = MathTools.CalculateItemPurchasePrice(itemToAdd, itemToAdd.FactionsThatSpecializeInThisItem.Contains(AssociatedFaction));
                itemToAdd.SalePrice = MathTools.CalculateItemSalePrice(itemToAdd, itemToAdd.FactionsThatSpecializeInThisItem.Contains(AssociatedFaction));
                InventoryItems.Add(itemToAdd);
                referenceTo_ownTradeStation_economyItem = itemToAdd;
                if (_debugThisClass)
                {
                    Debug.Log($"{fromTradeStation.TradeStationName} didn't have {economyItem.EconomyItemName} in their inventory, so it was added:\n {itemToAdd}");
                }
            }

            bool AI_PurchaseValid = false;

            if (numItemsRequested == 0) return;
            //trade is useless if nothing to take

            //do they have enough items
            if (referenceTo_fromTradeStation_economyItem.QuantityOfItem >= numItemsRequested)
            {
                numItemsExchanged = numItemsRequested;
            }
            else
            {
                numItemsExchanged = referenceTo_fromTradeStation_economyItem.QuantityOfItem;
                //they dont have enough item to give so theyll give all they can
                if (numItemsExchanged == 0) return;
                //trade is useless if nothing to give
            }
            //do they have the space for it
            if (referenceTo_ownTradeStation_economyItem.QuantityOfItem + numItemsExchanged <= referenceTo_ownTradeStation_economyItem.MaxQuantityOfItem)
            {
                AI_PurchaseValid = true;
            }
            else
            {
                numItemsExchanged = referenceTo_ownTradeStation_economyItem.MaxQuantityOfItem - referenceTo_ownTradeStation_economyItem.QuantityOfItem;
                //they dont have enough space so theyll buy all they can
                AI_PurchaseValid = true;
            }
            //can they afford it
            int cost = fromTradeStation.AssociatedFaction == AssociatedFaction
                ? (int)(referenceTo_fromTradeStation_economyItem.PurchasePrice * (1f - GameSettings.SameFactionPriceDiscount * 0.01f) * numItemsExchanged)
                : (int)(referenceTo_fromTradeStation_economyItem.PurchasePrice * (1f + GameSettings.PercentTaxExternalTrades * 0.01f) * numItemsExchanged);
            Debug.Log($"Cost: ${cost}");
            if (TradeStationMoney - cost > 0 && AI_PurchaseValid)
            {
                referenceTo_fromTradeStation_economyItem.QuantityOfItem -= numItemsExchanged;
                referenceTo_ownTradeStation_economyItem.QuantityOfItem += numItemsExchanged;
                fromTradeStation.TradeStationMoney += cost;
                TradeStationMoney -= cost;
                Debug.Log($"Faction {AssociatedFaction.FactionName}'s Trade Station {TradeStationName} has bought {economyItem.EconomyItemName} x{numItemsExchanged} for ${cost} from the " +
                $"{fromTradeStation.AssociatedFaction.FactionName} Faction's {fromTradeStation.TradeStationName} Trade Station. " +
                $"{TradeStationName} now has x{referenceTo_ownTradeStation_economyItem.QuantityOfItem} and {fromTradeStation.TradeStationName} " +
                $"now has x{referenceTo_fromTradeStation_economyItem.QuantityOfItem}");
            }
            else
            {
                Debug.Log($"Faction {AssociatedFaction.FactionName}'s Trade Station {TradeStationName} attempted to buy {economyItem.EconomyItemName} x{numItemsExchanged} for ${cost} from the " +
                $"{fromTradeStation.AssociatedFaction.FactionName} Faction's {fromTradeStation.TradeStationName} Trade Station, but they either couldn't afford it or the interaction was invalid. " +
                $"{TradeStationName} still has x{referenceTo_ownTradeStation_economyItem.QuantityOfItem} and {fromTradeStation.TradeStationName} " +
                $"still has x{referenceTo_fromTradeStation_economyItem.QuantityOfItem}");
            }
        }
        public void AI_Sell(EconomyItem economyItem, TradeStation fromTradeStation)
        {

        }
        public string LogItemsAvailable()
        {
            string returnString = string.Empty;
            foreach (EconomyItem item in InventoryItems)
            {
                returnString += $"\n{item.EconomyItemName} | Buy: ${item.PurchasePrice} / {item.PriceRoof} | Sell: ${item.SalePrice} / {item.PriceFloor} | Quantity: {item.QuantityOfItem} / {item.MaxQuantityOfItem}";
            }
            return returnString == string.Empty ? "None" : returnString;
        }
        public override string ToString()
        {
            return
                $"Trade Station Id: {TradeStationId}\n" +
                $"Station name: {TradeStationName}\n" +
                $"Station description: {TradeStationDescription}\n" +
                $"Money: ${TradeStationMoney}\n" +
                $"Items available:{LogItemsAvailable()}";
        }
    }
}
