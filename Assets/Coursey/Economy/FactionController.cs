using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System;

namespace Economy
{
    public class TimedBlock : IDisposable
    {
        private bool disposedValue;
        private string _blockName;
        private DateTime _start;

        public TimedBlock(string blockName)
        {
            _blockName = blockName;
            _start = DateTime.Now;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~TimedBlock()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            var stop = DateTime.Now;
            var span = stop - _start;
            Debug.Log($"{_blockName} took {span.TotalMilliseconds} ms");

            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

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
            //SaveData();
        }
        public void GameLoop()
        {
            Faction newFaction = null;
            using (var basicSql = new BasicSql())
            {
                using (new TimedBlock("Factions"))
                {
                    basicSql.ExecuteReader(@"SELECT * FROM Faction WHERE IsDisabled = $isDisabled", new List<KeyValuePair<string, string>>{
                    new KeyValuePair<string, string>("$isDisabled","False")
                }, (rowDataFac) =>
                    {
                        newFaction = new Faction(rowDataFac);
                    });
                }

                using (new TimedBlock("TradeStation"))
                {
                    basicSql.ExecuteReader(@"SELECT * FROM TradeStation WHERE FactionId = $factionId", new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("$factionId", newFaction.factionId)
                    }, (rowDataTS) =>
                    {
                        newFaction.tradeStations.Add(new TradeStation(rowDataTS));
                    });
                }

                foreach(var tradeStation in newFaction.tradeStations)
                {
                    

                    //build inventory
                    List<EconomyItem> inventoryItems = new();
                    basicSql.ExecuteReader(@"SELECT * FROM TradeStationInventory WHERE TradeStationId = $tradeStationId", new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("$tradeStationId", tradeStation.tradeStationId)
                    }, (rowDataItem) =>
                    {
                        inventoryItems.Add(new EconomyItem(rowDataItem));
                    });
                   
                    using (new TimedBlock("UseItems"))
                        tradeStation.UseItems();

                    using (new TimedBlock("ProduceItems"))
                        tradeStation.ProduceItems();

                    using (new TimedBlock("CalculatePriceDistribution"))
                        tradeStation.CalculatePriceDistribution();

                    using (new TimedBlock("ExecuteAllTrades"))
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
        /*        public TradeStation GetRandomTradeStation()
                {
                    var fac = GetRandomFaction();
                    Debug.Log($"Getting random trade station... #tradestations = {fac.tradeStations.Count}");
                    if (fac.tradeStations.Count == 0) return null;
                    return fac.tradeStations[MathTools.PseudoRandomIntExclusiveMax(0, fac.tradeStations.Count)];
                }*/
        /*public TradeStation GetRandomTradeStation(Faction faction)
        {
            return faction.tradeStations.Count > 1
                ? faction.tradeStations[MathTools.PseudoRandomIntExclusiveMax(0, faction.tradeStations.Count)]
                : faction.tradeStations[0];
        }*/
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
                }
                basicSql.ExecuteNonReader(@"
                CREATE TABLE IF NOT EXISTS Faction (Id INTEGER PRIMARY KEY, Name VARCHAR(100), Description TEXT, IsDisabled INTEGER);
                ");

                basicSql.ExecuteNonReader(@"
                CREATE TABLE IF NOT EXISTS TradeStation (Id INTEGER PRIMARY KEY, FactionId VARCHAR(3), Name VARCHAR(100), Description TEXT);
                ");

                //NYI, reputation scale 0 - 100 0 = hostile 50 = neutral 75 = friendly
                basicSql.ExecuteNonReader(@"
                CREATE TABLE IF NOT EXISTS FactionLinks (Id INTEGER PRIMARY KEY, FacID1 VARCHAR(3), FacID2 VARCHAR(3), Reputation INTEGER);
                ");

                basicSql.ExecuteNonReader(@"
                CREATE TABLE IF NOT EXISTS TradeStationInventory (Id INTEGER PRIMARY KEY, TradeStationId VARCHAR(3), ItemId VARCHAR(3), MaxQuantityOfItem INTEGER, PurchasePrice INTEGER, SalePrice INTEGER, IsSpecialized INTEGER);
                ");

                basicSql.ExecuteNonReader(@"
                CREATE TABLE IF NOT EXISTS EconomyEventTradeStationLink (Id INTEGER PRIMARY KEY, EventId VARCHAR(3), TradeStationId VARCHAR(3));
                ");

                foreach (var faction in factions)
                {
                    string name = basicSql.ExecuteScalar(@"SELECT Name FROM Faction WHERE Name = $name;",
                        new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("$name", faction.factionName)
                        }
                    );

                    

                    /*faction.factionId = basicSql.ExecuteScalar<string>(@"SELECT Id FROM Factions WHERE Name = $name;",
                        new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("$name", faction.factionName)
                        }
                     );*/

                    if (string.IsNullOrEmpty(name))
                    {
                        basicSql.ExecuteNonReader(
                            "INSERT INTO Faction (Name, Description, IsDisabled) VALUES ($name, $description, $isDisabled)",
                            new List<KeyValuePair<string, string>>
                            {
                                new KeyValuePair<string, string>("$name", faction.factionName),
                                new KeyValuePair<string, string>("$description", faction.factionDescription),
                                new KeyValuePair<string, string>("$isDisabled", "False"),
                            }
                            );
                    }

                    string factionId = basicSql.ExecuteScalar(@"SELECT Id FROM Faction WHERE Name = $name;",
                        new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("$name", faction.factionName)
                        }
                    );

                    foreach (var tradeStation in faction.tradeStations)
                    {
                        string nameTS = basicSql.ExecuteScalar(
                        @"SELECT Name FROM TradeStation WHERE Name = $name;",
                        new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("$name", tradeStation.tradeStationName)
                        });


                        tradeStation.tradeStationId = string.IsNullOrEmpty(tradeStation.tradeStationId)
                                ? basicSql.ExecuteScalar(@"SELECT Id FROM TradeStation WHERE Name = $name",
                                new List<KeyValuePair<string, string>>
                                {
                                new KeyValuePair<string, string>("$name", tradeStation.tradeStationName)
                                })
                                : tradeStation.tradeStationId;



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
                    }

                    foreach (var tradeStation in faction.tradeStations)
                    {
                        List<object[]> data = new List<object[]>();
                        string tradeStationId = basicSql.ExecuteScalar(@"SELECT Id FROM TradeStation WHERE Name = $name;",
                            new List<KeyValuePair<string, string>>
                            {
                            new KeyValuePair<string, string>("$name", tradeStation.tradeStationName)
                            }
                        );
                        foreach (var item in tradeStation.inventoryItems)
                        {
                            object[] obj = { tradeStationId, item.itemId, item.MaxQuantityOfItem, item.PurchasePrice, item.SalePrice, item.IsSpecialized};//ItemId VARCHAR(3), MaxQuantityOfItem INTEGER, PurchasePrice INTEGER, SalePrice INTEGER);
                            data.Add(obj);
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
                        basicSql.ExecuteNonReader(sql, prams);

                    }
                }
            }
        }
        #endregion SQLite
    }
}
