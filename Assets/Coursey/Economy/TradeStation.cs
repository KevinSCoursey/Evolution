using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;

namespace Economy
{
    public class TradeStation
    {
        private static bool _debugThisClass = true;
        //private List<Faction> factions = new();
        //private List<EconomyItem> economyItems = new();

        public string tradeStationName = string.Empty;
        public string tradeStationDescription = string.Empty;
        public List<EconomyEvent> economyEvents = new();
        public int money = 100000;

        public Faction associatedFaction;
        public string factionId;
        public string tradeStationId;
        public List<ItemClass> itemClassesSpecialized = new() { ItemClass.Generic };
        public List<EconomyItem> inventoryItems = new();
        public List<TradeRoute> internalTradeRoutes = new();
        public List<TradeRoute> externalTradeRoutes = new();

        public TradeStation(Faction associatedFaction, string tradeStationName = "Unnamed station", string tradeStationDescription = "No description provided.")
        {
            this.associatedFaction = associatedFaction;
            this.tradeStationName = tradeStationName;
            this.tradeStationDescription = tradeStationDescription;
            factionId = associatedFaction.factionId;
            using (var basicSql = new BasicSql())
            /*{
                tradeStationId = basicSql.ExecuteScalar(@"SELECT Id FROM TradeStation WHERE Name = $name",
                new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("$name", tradeStationName)
                });
            }*/
            InitializeInventory();
            CalculateItemDistribution();
            CalculatePriceDistribution();
        }
        public TradeStation(SqliteDataReader rowData)
        {
            tradeStationId = rowData["Id"].ToString();
            factionId = rowData["FactionId"].ToString();
            tradeStationName = rowData["Name"].ToString();
            tradeStationDescription = rowData["Description"].ToString();
        }
        private void InitializeInventory()
        {
            using (new TimedBlock("TradeStationInitializeInventory"))
            {
                using (var basicSql = new BasicSql())
                {
                    foreach (var itemClass in itemClassesSpecialized)
                    {
                        basicSql.ExecuteReader(@"SELECT * FROM EconomyItem WHERE ItemClassId = $itemClassId",
                        new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("$itemClassId", itemClass.ToString())
                        }, (rowData) =>
                        {
                            EconomyItem economyItemToAdd = new EconomyItem(rowData);
                            inventoryItems.Add(economyItemToAdd);
                        });
                    }
                }
            }
        }
        public void UseItems()
        {
            foreach (var item in inventoryItems)
            {
                float factor = 1f;
                foreach (var eEvent in economyEvents)
                {
                    if (eEvent.ItemClassesEffectedByEvent.Contains(item.itemClass) && eEvent.itemEffectFactor < 1)
                    {
                        factor *= eEvent.itemEffectFactor;
                    }
                }
                item.QuantityOfItem = (int)(factor *
                (item.QuantityOfItem - GameSettings.AverageEconomyItemsProducedPerTick -
                MathTools.PseudoRandomIntExclusiveMax(1 * item.rarityInt, 5 * item.rarityInt)));
            }
        }
        public void ProduceItems()
        {
            foreach (EconomyItem item in inventoryItems)
            {
                float factor = 1f;
                foreach (EconomyEvent eEvent in economyEvents)
                {
                    if (eEvent.ItemClassesEffectedByEvent.Contains(item.itemClass) && eEvent.itemEffectFactor > 1)
                    {
                        factor *= eEvent.itemEffectFactor;
                    }
                }
                item.QuantityOfItem = (int)(factor *
                (item.QuantityOfItem + GameSettings.AverageEconomyItemsProducedPerTick +
                MathTools.PseudoRandomIntExclusiveMax(1, 5 * item.rarityInt)));
            }
        }
        public void CalculatePriceDistribution()
        {
            foreach (EconomyItem item in inventoryItems)
            {
                item.PurchasePrice = MathTools.CalculateItemPurchasePrice(item, item.FactionsThatSpecializeInThisItem.Contains(associatedFaction));
                item.SalePrice = MathTools.CalculateItemSalePrice(item, item.FactionsThatSpecializeInThisItem.Contains(associatedFaction));
            }
        }
        public void ExecuteAllTrades()
        {
            Debug.Log($"Trade Station {tradeStationName} is beginning all trade routes... \n{this}");
            foreach (TradeRoute tradeRoute in internalTradeRoutes)
            {
                tradeRoute.AI_ConductTrade();
            }
            Debug.Log($"Internal trades complete.");
            foreach (TradeRoute tradeRoute in externalTradeRoutes)
            {
                tradeRoute.AI_ConductTrade();
            }
            Debug.Log($"External trades complete.");
            Debug.Log($"Trade Station {tradeStationName} has concluded all trade routes. \n{this}");
        }
        public List<EconomyItem> ItemsOfInterest(float percentOfMaxQuantity = 0.25f)
        {
            List<EconomyItem> itemsOfInterest = new List<EconomyItem>(inventoryItems.Where(_ => _.QuantityOfItem <= percentOfMaxQuantity * _.MaxQuantityOfItem));

            if (_debugThisClass)
            {
                string debugString = $"{tradeStationName} items of interest are ";
                debugString = debugString + string.Join(", ", itemsOfInterest.Select(_ => _.itemName));
                Debug.Log(debugString);
            }

            return itemsOfInterest;
        }
        public void AI_Buy(EconomyItem economyItem, TradeStation fromTradeStation)//fromtradestation is the one selling in this case
        {
            int numItemsExchanged = 0;
            int numItemsRequested = MathTools.PseudoRandomIntExclusiveMax(15, GameSettings.AverageNumItemsExchangedPerTrade * 2 + 15);

            var referenceTo_fromTradeStation_economyItem = fromTradeStation.inventoryItems.Find(_ => _.itemName == economyItem.itemName);
            var referenceTo_ownTradeStation_economyItem = inventoryItems.Find(_ => _.itemName == economyItem.itemName);

            if(referenceTo_fromTradeStation_economyItem == null)
            {
                EconomyItem itemToAdd = new EconomyItem(economyItem);
                itemToAdd.QuantityOfItem = 0;
                itemToAdd.PurchasePrice = MathTools.CalculateItemPurchasePrice(itemToAdd, itemToAdd.FactionsThatSpecializeInThisItem.Contains(associatedFaction));
                itemToAdd.SalePrice = MathTools.CalculateItemSalePrice(itemToAdd, itemToAdd.FactionsThatSpecializeInThisItem.Contains(associatedFaction));
                fromTradeStation.inventoryItems.Add(itemToAdd);
                referenceTo_fromTradeStation_economyItem = itemToAdd;
                Debug.Log($"{fromTradeStation.tradeStationName} didn't have {economyItem.itemName} in their inventory, so it was added:\n {itemToAdd}");
            }

            if (referenceTo_ownTradeStation_economyItem == null)
            {
                EconomyItem itemToAdd = new EconomyItem(economyItem);
                itemToAdd.QuantityOfItem = 0;
                itemToAdd.PurchasePrice = MathTools.CalculateItemPurchasePrice(itemToAdd, itemToAdd.FactionsThatSpecializeInThisItem.Contains(associatedFaction));
                itemToAdd.SalePrice = MathTools.CalculateItemSalePrice(itemToAdd, itemToAdd.FactionsThatSpecializeInThisItem.Contains(associatedFaction));
                inventoryItems.Add(itemToAdd);
                referenceTo_ownTradeStation_economyItem = itemToAdd;
                Debug.Log($"{fromTradeStation.tradeStationName} didn't have {economyItem.itemName} in their inventory, so it was added:\n {itemToAdd}");
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
                if(numItemsExchanged == 0) return;
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
            int cost = fromTradeStation.associatedFaction == associatedFaction
                ? (int)(referenceTo_fromTradeStation_economyItem.PurchasePrice * (1f - GameSettings.SameFactionPriceDiscount * 0.01f) * numItemsExchanged)
                : (int)(referenceTo_fromTradeStation_economyItem.PurchasePrice * (1f + GameSettings.PercentTaxExternalTrades * 0.01f) * numItemsExchanged);
            Debug.Log($"Cost: ${cost}");
            if (money - cost > 0 && AI_PurchaseValid)
            {
                referenceTo_fromTradeStation_economyItem.QuantityOfItem -= numItemsExchanged;
                referenceTo_ownTradeStation_economyItem.QuantityOfItem += numItemsExchanged;
                fromTradeStation.money += cost;
                money -= cost;
                Debug.Log($"Faction {associatedFaction.factionName}'s Trade Station {tradeStationName} has bought {economyItem.itemName} x{numItemsExchanged} for ${cost} from the " +
                $"{fromTradeStation.associatedFaction.factionName} Faction's {fromTradeStation.tradeStationName} Trade Station. " +
                $"{tradeStationName} now has x{referenceTo_ownTradeStation_economyItem.QuantityOfItem} and {fromTradeStation.tradeStationName} " +
                $"now has x{referenceTo_fromTradeStation_economyItem.QuantityOfItem}");
            }
            else
            {
                Debug.Log($"Faction {associatedFaction.factionName}'s Trade Station {tradeStationName} attempted to buy {economyItem.itemName} x{numItemsExchanged} for ${cost} from the " +
                $"{fromTradeStation.associatedFaction.factionName} Faction's {fromTradeStation.tradeStationName} Trade Station, but they either couldn't afford it or the interaction was invalid. " +
                $"{tradeStationName} still has x{referenceTo_ownTradeStation_economyItem.QuantityOfItem} and {fromTradeStation.tradeStationName} " +
                $"still has x{referenceTo_fromTradeStation_economyItem.QuantityOfItem}");
            }
        }
        public void AI_Sell(EconomyItem economyItem, TradeStation fromTradeStation)
        {

        }

        public void CalculateItemDistribution()
        {
            foreach (EconomyItem item in inventoryItems)
            {
                item.QuantityOfItem = MathTools.PseudoRandomIntExclusiveMax(0, item.MaxQuantityOfItem);//max currently 10000
            }
        }
        public string LogItemsAvailable()
        {
            string returnString = string.Empty;
            foreach (IEconomyItem item in inventoryItems)
            {
                returnString += $"\n{item.itemName} | Buy: ${item.PurchasePrice} / {item.priceRoof} | Sell: ${item.SalePrice} / {item.priceFloor} | Quantity: {item.QuantityOfItem} / {item.MaxQuantityOfItem}";
            }
            return returnString == string.Empty ? "None" : returnString;
        }
        public override string ToString()
        {
            return
                $"Station name: {tradeStationName}\n" +
                $"Station description: {tradeStationDescription}\n" +
                $"Money: ${money}\n" +
                $"Items available:{LogItemsAvailable()}";
        }
        #region SQLite
        //placeholder
        #endregion SQLite
    }
}
