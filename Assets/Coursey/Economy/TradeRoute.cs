using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                && Item1?.guid != Item2?.guid
                && Item1.internalTradeRoutes.Count < GameSettings.MaxInternalTradeRoutesPerTradeStation
                && Item2.internalTradeRoutes.Count < GameSettings.MaxInternalTradeRoutesPerTradeStation
                && Item1.externalTradeRoutes.Count < GameSettings.MaxExternalTradeRoutesPerTradeStation
                && Item2.externalTradeRoutes.Count < GameSettings.MaxExternalTradeRoutesPerTradeStation;


            InternalTradeRoute = TradeRouteValid
                ? Item1.associatedFaction == Item2.associatedFaction
                : false;
#pragma warning disable CS0162 // Unreachable code detected
            if (_debugThisClass) Debug.Log($"Guid {Item1?.guid} vs {Item2?.guid}");
            if (_debugThisClass && InternalTradeRoute && TradeRouteValid)
            {
                Debug.Log($"Trade route ({Item1.associatedFaction.factionName} <-> {Item1.associatedFaction.factionName}) has been generated between " +
                $"{Trade.Item1.tradeStationName} and {Trade.Item2.tradeStationName}");
            }
            else if (_debugThisClass && !InternalTradeRoute && TradeRouteValid)
            {
                Debug.Log($"Trade route ({Item1.associatedFaction.factionName} <-> {Item1.associatedFaction.factionName}) has been generated between " +
                $"{Trade.Item1.tradeStationName} and {Trade.Item2.tradeStationName}");
            }
            else if (_debugThisClass && !TradeRouteValid)
            {
                Debug.Log($"A trade route was generated, but it wasn't valid.");
            }
#pragma warning restore CS0162 // Unreachable code detected
        }

        public void ConductTrade()
        {

        }
        public bool Equals(TradeRoute tradeRoute)
        {
            bool duplicate = (Trade.Item1.guid == tradeRoute.Trade.Item1.guid && Trade.Item2.guid == tradeRoute.Trade.Item2.guid)
                          || (Trade.Item2.guid == tradeRoute.Trade.Item1.guid && Trade.Item1.guid == tradeRoute.Trade.Item2.guid);
            if (duplicate)
            {
                Debug.Log($"{tradeRoute} determined to be duplicate.");
            }
            return duplicate;
        }
        public static bool Equals(TradeRoute tradeRoute1, TradeRoute tradeRoute2)
        {
            bool duplicate = (tradeRoute1.Trade.Item1.guid == tradeRoute2.Trade.Item1.guid && tradeRoute1.Trade.Item2.guid == tradeRoute2.Trade.Item2.guid)
                          || (tradeRoute1.Trade.Item2.guid == tradeRoute2.Trade.Item1.guid && tradeRoute1.Trade.Item1.guid == tradeRoute2.Trade.Item2.guid);
            if (duplicate && _debugThisClass)
            {
                Debug.Log($"{tradeRoute1} determined to be duplicate.");
            }
            return duplicate;
        }
        public static bool CheckIfExists(TradeRoute tradeRoute)
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
        }
        public override string ToString()
        {
            string str = string.Empty;
            if (TradeRouteValid)
            {
                if (InternalTradeRoute)
                {
                    str = $"Trade Route: ({Trade.Item1.associatedFaction.factionName} <-> {Trade.Item1.associatedFaction.factionName})\n{Trade.Item1.associatedFaction.factionName}'s " +
                    $"{Trade.Item1.tradeStationName} Trade Station ({Trade.Item1.internalTradeRoutes.Count + 1}/{GameSettings.MaxInternalTradeRoutesPerTradeStation}) <-> " +
                    $"{Trade.Item2.associatedFaction.factionName}'s {Trade.Item2.tradeStationName} ({Trade.Item2.internalTradeRoutes.Count + 1}/{GameSettings.MaxInternalTradeRoutesPerTradeStation})";
                }
                else
                {
                    str = $"Trade Route: ({Trade.Item1.associatedFaction.factionName} <-> {Trade.Item2.associatedFaction.factionName})\n{Trade.Item1.associatedFaction.factionName}'s " +
                    $"{Trade.Item1.tradeStationName} Trade Station ({Trade.Item1.externalTradeRoutes.Count + 1}/{GameSettings.MaxExternalTradeRoutesPerTradeStation}) <-> " +
                    $"{Trade.Item2.associatedFaction.factionName}'s {Trade.Item2.tradeStationName} Trade Station ({Trade.Item2.externalTradeRoutes.Count + 1}/{GameSettings.MaxExternalTradeRoutesPerTradeStation})";
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