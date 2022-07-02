using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System;

namespace Economy
{
    public class FactionController
    {
        private const bool _debugThisClass = true;
        private List<string> factionNames = new List<string>
        {
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

        public static List<Faction> factions { get ; private set; } = new();
        public FactionController()//good
        {
            Initialize();
        }
        public void Initialize()//good
        {
            using (new TimedBlock("Adding default factions", _debugThisClass))
            {
                AddDefaultFactions();
            }
        }
        private void AddDefaultFactions()
        {
            //ensure same quantity of names and descriptions for faction generation, even if that means having empty strings
            if(factionNames.Count > factionDescriptions.Count)
            {
                while (factionNames.Count > factionDescriptions.Count)
                {
                    factionDescriptions.Add(string.Empty);
                }
            }
            if(factionDescriptions.Count > factionNames.Count)
            {
                while(factionDescriptions.Count > factionNames.Count)
                {
                    factionNames.Add(string.Empty);
                }
            }

            //build factions and add to database if they dont exist
            for (int i = 0; i < factionNames.Count; i++)
            {
                Faction faction = new Faction(factionNames[i], factionDescriptions[i]);
                factions.Add(faction);
            }
            LogAllFactions();
            DataBaseInteract.UpdateFactionData(factions);
        }//good
        public void GameLoop()
        {
            /* Trial 1 - save factions in memory and regenerate trade stations
             * 
             */
            using (var basicSql = new BasicSql())
            {
                List<Faction> newFactions = new List<Faction>();

                //build factions that arent disabled
                basicSql.ExecuteReader(@"SELECT * FROM Faction WHERE IsDisabled = $isDisabled", new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("$isDisabled","False")
                }, (rowDataFac) =>
                {
                    newFactions.Add(new Faction(rowDataFac));
                });

                //build trade stations for each faction that belong to that faction
                foreach (var faction in newFactions)
                {
                    basicSql.ExecuteReader(@"SELECT * FROM TradeStation WHERE FactionId = $factionId", new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("$factionId", faction.FactionId)
                    }, (rowDataTradeStation) =>
                    {
                        TradeStation newTradeStation = new TradeStation(rowDataTradeStation);
                        faction.TradeStations.Add(newTradeStation);

                        //build trade station inventory (NESTED)
                        basicSql.ExecuteReader(@"SELECT * FROM TradeStationInventory WHERE TradeStationId = $tradeStationId", new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("$tradeStationId", newTradeStation.TradeStationId)
                        }, (rowDataTradeStationInventory) =>
                        {
                            newTradeStation.InventoryItems.Add(new EconomyItem(rowDataTradeStationInventory, DataObjectType.TradeStationInventoryItem));
                        });

                        //build economy events (NESTED)
                        basicSql.ExecuteReader(@"SELECT * FROM EconomyEventTradeStationLink WHERE TradeStationId = $tradeStationId", new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("$tradeStationId", newTradeStation.TradeStationId)
                        }, (rowData) =>
                        {
                            newTradeStation.EconomyEvents.Add(new EconomyEvent(rowData));
                        });

                        //perform trade station operations (NESTED)
                        newTradeStation.UseItems();
                        newTradeStation.ProduceItems();
                        newTradeStation.ReCalculatePriceDistribution();
                        newTradeStation.ExecuteAllTrades();
#pragma warning disable CS0162 // Unreachable code detected
                        if (_debugThisClass) Debug.Log($"{newTradeStation}");
#pragma warning restore CS0162 // Unreachable code detected
                    });

                    //build inventory of each trade station
                    // nested speed = 
                    // inside trade station speed =
                    //just done here speed =

                    /*foreach (var tradeStation in faction.tradeStations)
                    {
                        basicSql.ExecuteReader(@"SELECT * FROM TradeStationInventory WHERE TradeStationId = $tradeStationId", new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("$tradeStationId", tradeStation.tradeStationId)
                    }, (rowDataItem) =>
                    {
                        tradeStation.inventoryItems.Add(new EconomyItem(rowDataItem));
                    });*/


                    /*basicSql.ExecuteReader(@"SELECT * FROM EconomyEventTradeStationLink WHERE TradeStationId = $tradeStationId", new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("$tradeStationId", tradeStation.tradeStationId)
                }, (rowData) =>
                {
                    tradeStation.economyEvents.Add(new EconomyEvent(rowData));
                }); ;*/

                    /*tradeStation.UseItems();
                    tradeStation.ProduceItems();
                    tradeStation.CalculatePriceDistribution();
                    tradeStation.ExecuteAllTrades();*/

                    /*#pragma warning disable CS0162 // Unreachable code detected
                                            if (_debugThisClass) Debug.Log($"{tradeStation}");
                    #pragma warning restore CS0162 // Unreachable code detected*/
                }
            }
        }
        public void GenerateRandomTradeStations(List<EconomyItem> economyItems)
        {
            foreach (Faction faction in factions)
            {
                faction.GenerateRandomTradeStation();
            }
        }
        /*public void GenerateRandomTradeRoutes()
        {
            foreach (var faction in factions)
            {
                EstablishTradeRoutes(faction);
            }

        }*/
        /*public void EstablishTradeRoutes(Faction fac)
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
        }*/
        public TradeRoute GenerateRandomInternalTradeRoute(TradeStation tradeStation)
        {
            if (tradeStation == null)
            {
                Debug.Log("Null trade station attempted to be used in generating a trade route");
                return null;
            }
            var faction = tradeStation.associatedFaction;
            TradeRoute tradeRoute = null;
            if (faction.TradeStations.Count > 2)
            {
                tradeRoute = new TradeRoute(tradeStation, GetRandomTradeStationExcludingThisOne(tradeStation));
            }
            else if (faction.TradeStations.Count == 2)
            {
                tradeRoute = new TradeRoute(faction.TradeStations[0], faction.TradeStations[1]);
            }
            else
            {
                Debug.Log($"A trade route ({faction.FactionName} <-> {faction.FactionName}) was attempted, " +
                $"but there aren't enough Trade Stations belonging to them!");
                tradeRoute = new TradeRoute(faction.TradeStations[0], null);
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
            if(faction2.TradeStations.Count > 0)
            {
                //tradeRoute = new TradeRoute(tradeStation, GetRandomTradeStation(faction2));
                tradeRoute = new TradeRoute(tradeStation, GetRandomTradeStation("4"));
            }
            else
            {
                Debug.Log($"A trade route ({faction1.FactionName} <-> {faction2.FactionName}) was attempted, " +
                $"but there aren't enough Trade Stations belonging to {faction2.FactionName}.");
                tradeRoute = new TradeRoute(tradeStation, null);
            }
            return tradeRoute.TradeRouteValid
                ? tradeRoute
                : null;
        }
        public static Faction GetRandomFaction(int attempt = 0)
        {
            Debug.Log($"Getting random faction... #fac = {factions.Count}");
            if (attempt >= GameSettings.MaxAttemptsToGenerateSomething)
            {
                Debug.Log($"Getting a random faction failed after {attempt} attempts.");
                return null;
            }
            return factions[MathTools.PseudoRandomIntExclusiveMax(0, factions.Count)];
        }
        public static Faction GetRandomFactionExcludingThisOne(Faction exclude)
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
        public TradeStation GetRandomTradeStation(string factionId)
        {
            List<TradeStation> factionTradeStations = new();

            using (var basicSql = new BasicSql())
            {
                var faction = basicSql.ExecuteScalar(@"SELECT * FROM TradeStations WHERE Id = $factionId", new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("$factionId", factionId)
                });
            }
            return null;
        }
        public TradeStation GetRandomTradeStationExcludingThisOne(TradeStation exclude)
        {
            TradeStation tradeStation = null;
            List<TradeStation> tradeStations = exclude.associatedFaction.TradeStations;
            if (exclude == null || exclude.associatedFaction.TradeStations.Count == 1)
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
        public void LogAllFactions()
        {
            if (_debugThisClass)
            {
                foreach(var faction in factions)
                {
                    Debug.Log($"Added the following faction...\n\n{faction}");
                }
            }
        }
    }
}
