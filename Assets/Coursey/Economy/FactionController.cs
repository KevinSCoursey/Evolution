using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Economy
{
    public class FactionController
    {
        private const bool _debugThisClass = true;


        public List<Faction> factions = new();

        private List<string> factionNames = new List<string>
        {
            //DEFAULT FACTIONS
            "Humans",
            "The Zerg",
            "PlaceholderFac1",
            "PlaceholderFac2",
            "PlaceholderFac3",
            "PlaceholderFac4",
            "PlaceholderFac5",
            "PlaceholderFac6",
            "PlaceholderFac7",
            "PlaceholderFac8"
        };
        private List<string> factionDescriptions = new List<string>
        {
            string.Empty,
            "OH GOD RUUUUUUN!!!!"
        };
        public FactionController()
        {
            Initialize();
        }
        public void Initialize()
        {
            AddDefaultFactions();
        }
        public void GameLoop()
        {
            foreach (Faction faction in factions)
            {
                foreach (TradeStation tradeStation in faction.tradeStations)
                {
                    tradeStation.UseItems();
                    tradeStation.ProduceItems();
                    tradeStation.CalculatePriceDistribution();
                    tradeStation.ExecuteAllTrades();

#pragma warning disable CS0162 // Unreachable code detected
                    if (_debugThisClass) Debug.Log($"{tradeStation}");
#pragma warning restore CS0162 // Unreachable code detected
                }
            }
        }
        private void AddDefaultFactions()
        {
            while (factionNames.Count > factionDescriptions.Count)
            {
                factionDescriptions.Add(string.Empty);
            }
            for (int i = 0; i < factionNames.Count; i++)
            {
                Faction faction = new Faction(factionNames[i], factionDescriptions[i]);
                factions.Add(faction);

#pragma warning disable CS0162 // Unreachable code detected
                if (_debugThisClass) Debug.Log($"Added the following faction...\n\n{faction}");
#pragma warning restore CS0162 // Unreachable code detected

            }
            foreach (Faction faction in factions)
            {
                faction.UpdateListOfFactions(factions);
            }
        }
        public List<Faction> GetFactionList()
        {
            return factions;
        }
        public void GenerateRandomTradeStations(List<EconomyItem> economyItems)
        {
            foreach (Faction faction in factions)
            {
                faction.GenerateRandomTradeStation(economyItems);
            }
        }
        public void GenerateRandomTradeRoutes()
        {
            foreach (var faction in factions)
            {
                EstablishTradeRoutes(faction);
            }

        }
        public void EstablishTradeRoutes(Faction fac)
        {
            foreach (TradeStation tradeStation in fac.tradeStations)
            {
                for (int i = 0; i < GameSettings.MaxInternalTradeRoutesPerTradeStation; i++)
                {
                    TradeRoute tradeRoute = GenerateRandomInternalTradeRoute(tradeStation);
                    if (tradeRoute != null && tradeRoute.TradeRouteValid && !TradeRoute.CheckIfExists(tradeRoute))
                    {
                        Debug.Log($"Added {tradeRoute}");
                        EconomyController.AllTradeRoutes.Add(tradeRoute);
                        tradeRoute.Trade.Item1.internalTradeRoutes.Add(tradeRoute);
                        tradeRoute.Trade.Item2.internalTradeRoutes.Add(tradeRoute);
                    }
                    tradeRoute = GenerateRandomExternalTradeRoute(tradeStation);
                    if (tradeRoute != null && tradeRoute.TradeRouteValid && !TradeRoute.CheckIfExists(tradeRoute))
                    {
                        Debug.Log($"Added {tradeRoute}");
                        EconomyController.AllTradeRoutes.Add(tradeRoute);
                        tradeRoute.Trade.Item1.externalTradeRoutes.Add(tradeRoute);
                        tradeRoute.Trade.Item2.externalTradeRoutes.Add(tradeRoute);
                    }
                }
            }
        }
        public TradeRoute GenerateRandomInternalTradeRoute(TradeStation tradeStation)
        {
            if (tradeStation == null)
            {
                Debug.Log("Null trade station attempted to be used in generating a trade route");
                return null;
            }
            var faction = tradeStation.associatedFaction;
            TradeRoute tradeRoute = null;
            if (faction.tradeStations.Count > 2)
            {
                tradeRoute = new TradeRoute(tradeStation, GetRandomTradeStationExcludingThisOne(tradeStation));
            }
            else if (faction.tradeStations.Count == 2)
            {
                tradeRoute = new TradeRoute(faction.tradeStations[0], faction.tradeStations[1]);
            }
            else
            {
                Debug.Log($"A trade route ({faction.factionName} <-> {faction.factionName}) was attempted, " +
                $"but there aren't enough Trade Stations belonging to them!");
                tradeRoute = new TradeRoute(faction.tradeStations[0], null);
            }
            return tradeRoute.TradeRouteValid
                ? tradeRoute
                : null;
        }
        public TradeRoute GenerateRandomExternalTradeRoute(TradeStation tradeStation)
        {
            if (tradeStation == null)
            {
                Debug.Log("Null trade station attempted to be used in generating a trade route");
                return null;
            }
            if (factions.Count < 2)
            {
                Debug.Log($"An external-to-faction trade route was attempted, but there aren't enough factions!");
                return null;
            }

            var faction1 = tradeStation.associatedFaction;
            var faction2 = GetRandomFactionExcludingThisOne(faction1);
            TradeRoute tradeRoute = null;
            if (faction2 == null) return null;
            if(faction2.tradeStations.Count > 0)
            {
                tradeRoute = new TradeRoute(tradeStation, GetRandomTradeStation(faction2));
            }
            else
            {
                Debug.Log($"A trade route ({faction1.factionName} <-> {faction2.factionName}) was attempted, " +
                $"but there aren't enough Trade Stations belonging to {faction2.factionName}.");
                tradeRoute = new TradeRoute(tradeStation, null);
            }
            return tradeRoute.TradeRouteValid
                ? tradeRoute
                : null;
        }
        public Faction GetRandomFaction(int attempt = 0)
        {
            Debug.Log($"Getting random faction... #fac = {factions.Count}");
            if (attempt >= GameSettings.MaxAttemptsToGenerateSomething)
            {
                Debug.Log($"Getting a random faction failed after {attempt} attempts.");
                return null;
            }
            return factions[MathTools.PseudoRandomIntExclusiveMax(0, factions.Count)];
        }
        public Faction GetRandomFactionExcludingThisOne(Faction exclude)
        {
            //broken
            Faction faction = null; 
            if (exclude == null || factions.Count == 1)
            {
                return null;
            }
            else
            {
                int index = factions.IndexOf(exclude);
                faction = factions[MathTools.PseudoRandomIntExcluding(0, factions.Count, index)];
            }
            return faction == exclude
                ? null
                : faction;
        }
        public TradeStation GetRandomTradeStation()
        {
            var fac = GetRandomFaction();
            Debug.Log($"Getting random trade station... #tradestations = {fac.tradeStations.Count}");
            if (fac.tradeStations.Count == 0) return null;
            return fac.tradeStations[MathTools.PseudoRandomIntExclusiveMax(0, fac.tradeStations.Count)];
        }
        public TradeStation GetRandomTradeStation(Faction faction)
        {
            return faction.tradeStations.Count > 1
                ? faction.tradeStations[MathTools.PseudoRandomIntExclusiveMax(0, faction.tradeStations.Count)]
                : faction.tradeStations[0];
        }
        public TradeStation GetRandomTradeStationExcludingThisOne(TradeStation exclude)
        {
            TradeStation tradeStation = null;
            List<TradeStation> tradeStations = exclude.associatedFaction.tradeStations;
            if (exclude == null || exclude.associatedFaction.tradeStations.Count == 1)
            {
                return null;
            }
            else
            {
                int index = tradeStations.IndexOf(exclude);
                tradeStation = tradeStations[MathTools.PseudoRandomIntExcluding(0, tradeStations.Count - 1, index)];
            }
            return tradeStation == exclude
                ? null
                : tradeStation;
        }
    }
}
