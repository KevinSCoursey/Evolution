using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Economy
{
    public class TradeRoute
    {
        private const bool _debugThisClass = false;
        public (TradeStation, TradeStation) Trade;
        public bool TradeRouteValid = true;
        public bool InternalTradeRoute;

        public TradeRoute(TradeStation Item1, TradeStation Item2)
        {
            Trade = (Item1, Item2);
            TradeRouteValid = Item1 != null
                && Item2 != null
                //&& Item1?.guid != Item2?.guid
                && Item1.InternalTradeRoutes.Count < GameSettings.MaxInternalTradeRoutesPerTradeStation
                && Item2.InternalTradeRoutes.Count < GameSettings.MaxInternalTradeRoutesPerTradeStation
                && Item1.ExternalTradeRoutes.Count < GameSettings.MaxExternalTradeRoutesPerTradeStation
                && Item2.ExternalTradeRoutes.Count < GameSettings.MaxExternalTradeRoutesPerTradeStation;

            InternalTradeRoute = TradeRouteValid
                ? Item1.AssociatedFaction == Item2.AssociatedFaction
                : false;
#pragma warning disable CS0162 // Unreachable code detected
            //if (_debugThisClass) Debug.Log($"Guid {Item1?.guid} vs {Item2?.guid}");
            if (_debugThisClass && InternalTradeRoute && TradeRouteValid)
            {
                Debug.Log($"Trade route ({Item1.AssociatedFaction.FactionName} <-> {Item1.AssociatedFaction.FactionName}) has been generated between " +
                $"{Trade.Item1.TradeStationName} and {Trade.Item2.TradeStationName}");
            }
            else if (_debugThisClass && !InternalTradeRoute && TradeRouteValid)
            {
                Debug.Log($"Trade route ({Item1.AssociatedFaction.FactionName} <-> {Item1.AssociatedFaction.FactionName}) has been generated between " +
                $"{Trade.Item1.TradeStationName} and {Trade.Item2.TradeStationName}");
            }
            else if (_debugThisClass && !TradeRouteValid)
            {
                Debug.Log($"A trade route was generated, but it wasn't valid.");
            }
#pragma warning restore CS0162 // Unreachable code detected
        }
        public void AI_ConductTrade()
        {
            int numItemsToTrade = MathTools.PseudoRandomIntExclusiveMax(1, GameSettings.AverageNumItemsExchangedPerTrade * 2 - 1);
            var itemsForItem1 = Trade.Item1.ItemsOfInterest();
            var itemsForItem2 = Trade.Item2.ItemsOfInterest();
            Debug.Log($"{numItemsToTrade} different item(s) will be traded, if possible.");

            if (itemsForItem1.Count < numItemsToTrade)
            {
                Debug.Log($"{Trade.Item1.TradeStationName} reevaluated their interests.");
                itemsForItem1.AddRange(Trade.Item1.ItemsOfInterest(0.5f));
            }
            if (itemsForItem2.Count < numItemsToTrade)
            {
                Debug.Log($"{Trade.Item2.TradeStationName} reevaluated their interests.");
                itemsForItem2.AddRange(Trade.Item2.ItemsOfInterest(0.5f));
            }
            if (itemsForItem1.Count == 0 || itemsForItem2.Count == 0)
            {
                Debug.Log($"A trade between {Trade.Item1.AssociatedFaction.FactionName}'s {Trade.Item1.TradeStationName} and " +
                    $"{Trade.Item2.AssociatedFaction.FactionName}'s {Trade.Item2.TradeStationName} wasn't able to be completed. " +
                    $"There were no mutually beneficial items of interest this time.");
            }
            else
            {
                //TODO ACTUAL ITEM EXCHANGE
                //itemsOfInterest.OrderBy(_ => MathTools.random.Next()).Take(numItemsToTrade);
                Debug.Log($"A trade is occuring...");
                int[] indexOfItemsToTradeItem1 = MathTools.GetRandomIndexes(itemsForItem1, numItemsToTrade);
                List<EconomyItem> itemsToTradeItem1 = new();
                int[] indexOfItemsToTradeItem2 = MathTools.GetRandomIndexes(itemsForItem2, numItemsToTrade);
                List<EconomyItem> itemsToTradeItem2 = new();

                for (int i = 0; i < indexOfItemsToTradeItem1.Length; i++)
                {
                    itemsToTradeItem1.Add(itemsForItem1[i]);
                }

                for (int i = 0; i < indexOfItemsToTradeItem2.Length; i++)
                {
                    itemsToTradeItem2.Add(itemsForItem2[i]);
                }
                foreach (EconomyItem item in itemsToTradeItem1)
                {
                    //no point trading item if they both need it
                    if(itemsForItem2.Find(_ => _.EconomyItemName == item.EconomyItemName) == null)
                    {
                        Trade.Item1.AI_Buy(item, Trade.Item2);
                    }
                    else
                    {
                        Debug.Log($"Both {Trade.Item1.TradeStationName} and {Trade.Item2.TradeStationName} " +
                            $"need {item.EconomyItemName}, so it wasn't traded.");
                    }
                }
                foreach (EconomyItem item in itemsToTradeItem2)
                {
                    //no point trading item if they both need it
                    if (itemsForItem1.Find(_ => _.EconomyItemName == item.EconomyItemName) == null)
                    {
                        Trade.Item2.AI_Buy(item, Trade.Item1);
                    }
                    else
                    {
                        Debug.Log($"Both {Trade.Item2.TradeStationName} and {Trade.Item1.TradeStationName} " +
                            $"need {item.EconomyItemName}, so it wasn't traded.");
                    }
                }
            }
        }
       /* public bool Equals(TradeRoute tradeRoute)
        {
            bool duplicate = (Trade.Item1.guid == tradeRoute.Trade.Item1.guid && Trade.Item2.guid == tradeRoute.Trade.Item2.guid)
                          || (Trade.Item2.guid == tradeRoute.Trade.Item1.guid && Trade.Item1.guid == tradeRoute.Trade.Item2.guid);
            if (duplicate)
            {
                Debug.Log($"{tradeRoute} determined to be duplicate.");
            }
            return duplicate;
        }*/
       /* public static bool Equals(TradeRoute tradeRoute1, TradeRoute tradeRoute2)
        {
            bool duplicate = (tradeRoute1.Trade.Item1.guid == tradeRoute2.Trade.Item1.guid && tradeRoute1.Trade.Item2.guid == tradeRoute2.Trade.Item2.guid)
                          || (tradeRoute1.Trade.Item2.guid == tradeRoute2.Trade.Item1.guid && tradeRoute1.Trade.Item1.guid == tradeRoute2.Trade.Item2.guid);
            if (duplicate && _debugThisClass)
            {
                Debug.Log($"{tradeRoute1} determined to be duplicate.");
            }
            return duplicate;
        }*/
        /*public static bool CheckIfExists(TradeRoute tradeRoute)
        {
            bool exists = false;
            foreach (TradeRoute tradeRouteParse in EconomyController.AllTradeRoutes)
            {
                if (Equals(tradeRouteParse, tradeRoute))
                {
                    exists = true;
                }
            }
            return exists;
        }*/
        public override string ToString()
        {
            string str = string.Empty;
            if (TradeRouteValid)
            {
                if (InternalTradeRoute)
                {
                    str = $"Trade Route: ({Trade.Item1.AssociatedFaction.FactionName} <-> {Trade.Item1.AssociatedFaction.FactionName})\n{Trade.Item1.AssociatedFaction.FactionName}'s " +
                    $"{Trade.Item1.TradeStationName} Trade Station ({Trade.Item1.InternalTradeRoutes.Count + 1}/{GameSettings.MaxInternalTradeRoutesPerTradeStation}) <-> " +
                    $"{Trade.Item2.AssociatedFaction.FactionName}'s {Trade.Item2.TradeStationName} ({Trade.Item2.InternalTradeRoutes.Count + 1}/{GameSettings.MaxInternalTradeRoutesPerTradeStation})";
                }
                else
                {
                    str = $"Trade Route: ({Trade.Item1.AssociatedFaction.FactionName} <-> {Trade.Item2.AssociatedFaction.FactionName})\n{Trade.Item1.AssociatedFaction.FactionName}'s " +
                    $"{Trade.Item1.TradeStationName} Trade Station ({Trade.Item1.ExternalTradeRoutes.Count + 1}/{GameSettings.MaxExternalTradeRoutesPerTradeStation}) <-> " +
                    $"{Trade.Item2.AssociatedFaction.FactionName}'s {Trade.Item2.TradeStationName} Trade Station ({Trade.Item2.ExternalTradeRoutes.Count + 1}/{GameSettings.MaxExternalTradeRoutesPerTradeStation})";
                }
            }
            else
            {
                str = $"Trade Route: (Invalid)";
            }
            return str;
        }
    }
}