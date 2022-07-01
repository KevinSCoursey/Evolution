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
                        new KeyValuePair<string, string>("$factionId", faction.factionId)
                    }, (rowDataTradeStation) =>
                    {
                        TradeStation newTradeStation = new TradeStation(rowDataTradeStation);
                        faction.tradeStations.Add(newTradeStation);

                        //build trade station inventory (NESTED)
                        basicSql.ExecuteReader(@"SELECT * FROM TradeStationInventory WHERE TradeStationId = $tradeStationId", new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("$tradeStationId", newTradeStation.tradeStationId)
                        }, (rowDataTradeStationInventory) =>
                        {
                            newTradeStation.inventoryItems.Add(new EconomyItem(rowDataTradeStationInventory));
                        });

                        //build economy events (NESTED)
                        basicSql.ExecuteReader(@"SELECT * FROM EconomyEventTradeStationLink WHERE TradeStationId = $tradeStationId", new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("$tradeStationId", newTradeStation.tradeStationId)
                        }, (rowData) =>
                        {
                            newTradeStation.economyEvents.Add(new EconomyEvent(rowData));
                        });

                        //perform trade station operations (NESTED)
                        newTradeStation.UseItems();
                        newTradeStation.ProduceItems();
                        newTradeStation.CalculatePriceDistribution();
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
                faction.GenerateRandomTradeStation(/*economyItems*/);
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
                //tradeRoute = new TradeRoute(tradeStation, GetRandomTradeStation(faction2));
                tradeRoute = new TradeRoute(tradeStation, GetRandomTradeStation("4"));
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

            }/*

                return faction.tradeStations.Count > 1
                    ? faction.tradeStations[MathTools.PseudoRandomIntExclusiveMax(0, faction.tradeStations.Count)]
                    : faction.tradeStations[0];*/
            return null;
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

        #region SQLite
        public void SaveData()
        {
            using (var basicSql = new BasicSql())
            {
                if (GameSettings.RegenerateSQLiteDBsEachRun)
                {
                    basicSql.ExecuteNonReader("DROP TABLE IF EXISTS Faction");
                    basicSql.ExecuteNonReader("DROP TABLE IF EXISTS TradeStation");
                    basicSql.ExecuteNonReader("DROP TABLE IF EXISTS TradeStationInventory");
                    basicSql.ExecuteNonReader("DROP TABLE IF EXISTS EconomyEventTradeStationLink");
                    basicSql.ExecuteNonReader("DROP TABLE IF EXISTS TradeRoute");
                }
                #region Define SqliteTables
                basicSql.ExecuteNonReader(@"
                CREATE TABLE IF NOT EXISTS Faction (Id INTEGER PRIMARY KEY, Name VARCHAR(100), Description TEXT, IsDisabled INTEGER);
                ");
                basicSql.ExecuteNonReader(@"
                CREATE TABLE IF NOT EXISTS TradeStation (Id INTEGER PRIMARY KEY, FactionId VARCHAR(3), Name VARCHAR(100), Description TEXT);
                ");
                basicSql.ExecuteNonReader(@"
                CREATE TABLE IF NOT EXISTS FactionLinks (Id INTEGER PRIMARY KEY, FacID1 VARCHAR(3), FacID2 VARCHAR(3), Reputation INTEGER);
                ");
                basicSql.ExecuteNonReader(@"
                CREATE TABLE IF NOT EXISTS TradeStationInventory (Id INTEGER PRIMARY KEY, TradeStationId VARCHAR(3), ItemId VARCHAR(3), MaxQuantityOfItem INTEGER, PurchasePrice INTEGER, SalePrice INTEGER, IsSpecialized INTEGER);
                ");
                basicSql.ExecuteNonReader(@"
                CREATE TABLE IF NOT EXISTS EconomyEventTradeStationLink (Id INTEGER PRIMARY KEY, EventId VARCHAR(3), TradeStationId VARCHAR(3));
                ");
                #endregion

                foreach (var faction in factions)
                {
                    //check if faction exists with name and add if it doesnt
                    string nameFac = basicSql.ExecuteScalar(@"
                    SELECT Name FROM Faction WHERE Name = $name;",
                    new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("$name", faction.factionName)
                    });
                    if (string.IsNullOrEmpty(nameFac))
                    {
                        basicSql.ExecuteNonReader(
                        "INSERT INTO Faction (Name, Description, IsDisabled) VALUES ($name, $description, $isDisabled)",
                        new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("$name", faction.factionName),
                            new KeyValuePair<string, string>("$description", faction.factionDescription),
                            new KeyValuePair<string, string>("$isDisabled", "False"),
                        });
                    }

                    //asign factionid to all factions
                    string factionId = basicSql.ExecuteScalar(@"
                    SELECT Id FROM Faction WHERE Name = $name;",
                    new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("$name", faction.factionName)
                    });

                    foreach (var tradeStation in faction.tradeStations)
                    {
                        //check if trade station exists in database with name and add if it doesnt
                        string nameTS = basicSql.ExecuteScalar(@"
                        SELECT Name FROM TradeStation WHERE Name = $name;",
                        new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("$name", tradeStation.tradeStationName)
                        });
                        if (string.IsNullOrEmpty(nameTS))
                        {
                            basicSql.ExecuteNonReader(
                            "INSERT INTO TradeStation (FactionId, Name, Description) VALUES ($factionId, $name, $description)",
                            new List<KeyValuePair<string, string>>
                            {
                                new KeyValuePair<string, string>("$factionId", factionId),
                                new KeyValuePair<string, string>("$name", tradeStation.tradeStationName),
                                new KeyValuePair<string, string>("$description", tradeStation.tradeStationDescription)
                            });
                        }

                        //assign id to tradestation if it doesnt have one
                        tradeStation.tradeStationId = string.IsNullOrEmpty(tradeStation.tradeStationId)
                        ? basicSql.ExecuteScalar(@"SELECT Id FROM TradeStation WHERE Name = $name",
                        new List<KeyValuePair<string, string>>
                        {
                        new KeyValuePair<string, string>("$name", tradeStation.tradeStationName)
                        })
                        : tradeStation.tradeStationId;

                        //add trade station inventory to database
                        List<object[]> data = new List<object[]>();
                        foreach (var item in tradeStation.inventoryItems)
                        {
                            data.Add(new object[] { tradeStation.tradeStationId, item.itemId, item.MaxQuantityOfItem, item.PurchasePrice, item.SalePrice, item.IsSpecialized });
                        }
                        var prams = new List<KeyValuePair<string, string>>();
                        var sql = @"
                        INSERT INTO	TradeStationInventory
                        (TradeStationId, ItemId, MaxQuantityOfItem, PurchasePrice, SalePrice, IsSpecialized)
                        VALUES
                        ";
                        var idx = 0;
                        foreach (var obj in data)
                        {
                            sql += idx > 0 ? "," : "";
                            sql += $"($tradeStationId{idx},$itemId{idx},$maxQuantityOfItem{idx},$purchasePrice{idx},$salePrice{idx},$isSpecialized{idx})";
                            prams.Add(new KeyValuePair<string, string>($"$tradeStationId{idx}", obj[0].ToString()));
                            prams.Add(new KeyValuePair<string, string>($"$itemId{idx}", obj[1].ToString()));
                            prams.Add(new KeyValuePair<string, string>($"$maxQuantityOfItem{idx}", obj[2].ToString()));
                            prams.Add(new KeyValuePair<string, string>($"$purchasePrice{idx}", obj[3].ToString()));
                            prams.Add(new KeyValuePair<string, string>($"$salePrice{idx}", obj[4].ToString()));
                            prams.Add(new KeyValuePair<string, string>($"$isSpecialized{idx}", obj[5].ToString()));
                            idx++;
                        }
                        basicSql.ExecuteNonReader(sql + ";", prams);
                    }
                }
            }
        }
        /*public void SaveTradeRouteData()
        {
            using (var basicSql = new BasicSql())
            {
                if (GameSettings.RegenerateSQLiteDBsEachRun)
                {
                    basicSql.ExecuteNonReader("DROP TABLE IF EXISTS TradeRoute");
                }

                basicSql.ExecuteNonReader(@"
                CREATE TABLE IF NOT EXISTS TradeRoute (Id INTEGER PRIMARY KEY, TradeStationId1 VARCHAR(3), TradeStationId2 VARCHAR(3));
                ");

                foreach(var tradeRoute in EconomyController.AllTradeRoutes)
                {
                    string item1 = basicSql.ExecuteScalar(@"SELECT TradeStationId1 FROM TradeRoute WHERE TradeStationId1 = $id1;",
                        new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("$id1", tradeRoute.Trade.Item1.tradeStationId)
                        }
                    );

                    string item2 = basicSql.ExecuteScalar(@"SELECT TradeStationId2 FROM TradeRoute WHERE TradeStationId2 = $id2;",
                        new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("$id2", tradeRoute.Trade.Item2.tradeStationId)
                        }
                    );

                    if (string.IsNullOrEmpty(item1) || string.IsNullOrEmpty(item2))
                    {
                        basicSql.ExecuteNonReader(
                            "INSERT INTO TradeRoute (TradeStationId1, TradeStationId2) VALUES ($id1, $id2)",
                            new List<KeyValuePair<string, string>>
                            {
                                new KeyValuePair<string, string>("$id1", tradeRoute.Trade.Item1.tradeStationId),
                                new KeyValuePair<string, string>("$id2", tradeRoute.Trade.Item2.tradeStationId)
                            }
                            );
                    }
                }
            }
        }*/
        #endregion SQLite
    }
}
