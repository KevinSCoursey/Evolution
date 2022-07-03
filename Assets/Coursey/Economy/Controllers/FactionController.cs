using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System;

namespace Economy
{
    public static class FactionController
    {
        private const bool _debugThisClass = true;
        private static List<string> _factionNames = new List<string>
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
        private static List<string> _factionDescriptions = new List<string>
        {
            string.Empty,
            "OH GOD RUUUUUUN!!!!"
        };

        public static bool IsReady = false;

        public static List<Faction> Factions { get ; private set; } = new();//keep factions loaded
        public static void Initialize()//good
        {
            IsReady = false;
            using (new TimedBlock("Adding default factions", _debugThisClass))
            {
                AddDefaultFactions();
            }
            IsReady = true;
        }
        private static void AddDefaultFactions()
        {
            //ensure same quantity of names and descriptions for faction generation, even if that means having empty strings
            if(_factionNames.Count > _factionDescriptions.Count)
            {
                while (_factionNames.Count > _factionDescriptions.Count)
                {
                    _factionDescriptions.Add(string.Empty);
                }
            }
            if(_factionDescriptions.Count > _factionNames.Count)
            {
                while(_factionDescriptions.Count > _factionNames.Count)
                {
                    _factionNames.Add(string.Empty);
                }
            }

            //build factions and add to database if they dont exist
            for (int i = 0; i < _factionNames.Count; i++)
            {
                Faction faction = new Faction(_factionNames[i], _factionDescriptions[i]);
                Factions.Add(faction);
            }
            LogAllFactions();
            DataBaseInteract.UpdateFactionData(Factions);
        }//good
        public static void GameLoop()
        {
            IsReady=false;
            List<TradeStation> tradeStations = new();
            foreach (var faction in Factions)
            {
                using (new TimedBlock("ASDF :: LoadTradeStationsForFaction")) ;
                    tradeStations = DataBaseInteract.LoadTradeStationsForFaction(faction.FactionId);//doesnt have inventory loaded here?
#pragma warning disable CS0162 // Unreachable code detected
                if (_debugThisClass)
                {
                    foreach (var tradeStation in tradeStations)
                    {
                        Debug.Log($"Loading Trade Station for {faction.FactionName}...\n{tradeStation}");
                    }
                }
#pragma warning restore CS0162 // Unreachable code detected
                if(tradeStations.Count >= GameSettings.NumTradeStationsPerDBBlock)
                {
                    TradeStationController.GameLoop(tradeStations);
                    tradeStations.Clear();
                }
                
            }
            if(tradeStations.Count != 0)
            {
                TradeStationController.GameLoop(tradeStations);
                tradeStations.Clear();
            }
            IsReady =true;




            /*using (var basicSql = new BasicSql())
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
                    });*/

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
                //}
            //}
        }
        /*public void GenerateRandomTradeStations(List<EconomyItem> economyItems)
        {
            foreach (Faction faction in factions)
            {
                faction.GenerateRandomTradeStation();
            }
        }*/
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
        public static TradeRoute GenerateRandomInternalTradeRoute(TradeStation tradeStation)
        {
            if (tradeStation == null)
            {
                Debug.Log("Null trade station attempted to be used in generating a trade route");
                return null;
            }
            var faction = tradeStation.AssociatedFaction;
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
        public static TradeRoute GenerateRandomExternalTradeRoute(TradeStation tradeStation)
        {
            if (tradeStation == null)
            {
                Debug.Log("Null trade station attempted to be used in generating a trade route");
                return null;
            }
            if (Factions.Count < 2)
            {
                Debug.Log($"An external-to-faction trade route was attempted, but there aren't enough factions!");
                return null;
            }

            var faction1 = tradeStation.AssociatedFaction;
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
            Debug.Log($"Getting random faction... #fac = {Factions.Count}");
            if (attempt >= GameSettings.MaxAttemptsToGenerateSomething)
            {
                Debug.Log($"Getting a random faction failed after {attempt} attempts.");
                return null;
            }
            return Factions[MathTools.PseudoRandomIntExclusiveMax(0, Factions.Count)];
        }
        public static Faction GetRandomFactionExcludingThisOne(Faction exclude)
        {
            //broken
            Faction faction = null; 
            if (exclude == null || Factions.Count == 1)
            {
                return null;
            }
            else
            {
                int index = Factions.IndexOf(exclude);
                faction = Factions[MathTools.PseudoRandomIntExcluding(0, Factions.Count, index)];
            }
            return faction == exclude
                ? null
                : faction;
        }
        public static TradeStation GetRandomTradeStation(string factionId)
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
        public static TradeStation GetRandomTradeStationExcludingThisOne(TradeStation exclude)
        {
            TradeStation tradeStation = null;
            List<TradeStation> tradeStations = exclude.AssociatedFaction.TradeStations;
            if (exclude == null || exclude.AssociatedFaction.TradeStations.Count == 1)
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
        public static void LogAllFactions()
        {
#pragma warning disable CS0162 // Unreachable code detected
            if (_debugThisClass)
            {
                foreach (var faction in Factions)
                {
                    Debug.Log($"Added the following faction...\n\n{faction}");
                }
            }
#pragma warning restore CS0162 // Unreachable code detected
        }
    }
}