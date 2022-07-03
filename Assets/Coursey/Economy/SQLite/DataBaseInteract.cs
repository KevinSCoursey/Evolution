using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System.Linq;

namespace Economy
{
    //https://www.sqlitetutorial.net/sqlite-index/
    public class DataBaseInteract
    {
        private static bool _debugThisClass = true;
        public static void ClearDataBase()
        {
            using (new TimedBlock("Clearing all data in database", _debugThisClass))
            {
                using (var basicSql = new BasicSql())
                {
                    if (GameSettings.RegenerateSQLiteDBsEachRun)
                    {
                        basicSql.ExecuteNonReader(@"
                        DROP TABLE IF EXISTS Faction;
                        DROP TABLE IF EXISTS TradeStation;
                        DROP TABLE IF EXISTS TradeStationInventory;
                        DROP TABLE IF EXISTS EconomyEventTradeStationLink;
                        DROP TABLE IF EXISTS TradeRoute;
                        DROP TABLE IF EXISTS EconomyItem;
                        DROP TABLE IF EXISTS EconomyEvent;
                        DROP TABLE IF EXISTS EconomyEventItemClassLink;");
                        if (_debugThisClass)
                        {
                            Debug.Log($"All of the database was cleared.");
                        }
                    }
                    else
                    {
                        if (_debugThisClass)
                        {
                            Debug.Log($"Database was not cleared.");
                        }
                    }
                }
            }
        }
        public static void CreateDataBase()
        {
            using (new TimedBlock("Creating tables in the database", _debugThisClass))
            {
                using (var basicSql = new BasicSql())
                {
                    basicSql.ExecuteNonReader(@"
                    CREATE TABLE IF NOT EXISTS Faction (FactionId INTEGER PRIMARY KEY, FactionName VARCHAR(100) UNIQUE NOT NULL, FactionDescription TEXT NOT NULL,
                    IsDisabled INTEGER NOT NULL);

                    CREATE TABLE IF NOT EXISTS TradeStation (TradeStationId INTEGER PRIMARY KEY, FactionId VARCHAR(3) NOT NULL, TradeStationName VARCHAR(100) UNIQUE NOT NULL, TradeStationDescription TEXT NOT NULL, TradeStationMoney INTEGER NOT NULL);

                    CREATE TABLE IF NOT EXISTS FactionLinks (FactionLinkId INTEGER PRIMARY KEY, FactionId1 VARCHAR(3) NOT NULL, FactionId2 VARCHAR(3) NOT NULL,
                    Reputation INTEGER NOT NULL, UNIQUE (FactionId1, FactionId2));

                    CREATE TABLE IF NOT EXISTS TradeStationInventory (TradeStationInventoryId INTEGER PRIMARY KEY, TradeStationId VARCHAR(3) NOT NULL, EconomyItemId VARCHAR(3) NOT NULL,
                    QuantityOfItem INTEGER NOT NULL, MaxQuantityOfItem INTEGER NOT NULL, PurchasePrice INTEGER NOT NULL, SalePrice INTEGER NOT NULL,
                    IsSpecialized INTEGER NOT NULL, UNIQUE (TradeStationId, EconomyItemId));

                    CREATE TABLE IF NOT EXISTS EconomyEventTradeStationLink (EconomyEventTradeStationLinkId INTEGER PRIMARY KEY, EconomyEventId VARCHAR(3) UNIQUE NOT NULL, TradeStationId VARCHAR(3) NOT NULL,
                    UNIQUE (EconomyEventId, TradeStationId));

                    CREATE TABLE IF NOT EXISTS EconomyItem(EconomyItemId INTEGER PRIMARY KEY, EconomyItemName VARCHAR(100) UNIQUE NOT NULL, EconomyItemDescription TEXT NOT NULL,
                    EconomyItemClassId CHAR(10) NOT NULL, RarityInt INTEGER NOT NULL, PriceDefault INTEGER NOT NULL, PriceFloor INTEGER NOT NULL, PriceRoof INTEGER NOT NULL);

                    CREATE TABLE IF NOT EXISTS EconomyEvent (EconomyEventId INTEGER PRIMARY KEY, EconomyEventName VARCHAR(100) UNIQUE NOT NULL, EconomyEventDescription TEXT UNIQUE NOT NULL);

                    CREATE TABLE IF NOT EXISTS EconomyEventItemClassLink (EconomyEventItemClassLinkId INTEGER PRIMARY KEY, EconomyEventId CHAR(10) NOT NULL, EconomyItemClassId CHAR(10) NOT NULL, UNIQUE (EconomyEventId, EconomyItemClassId));

                    CREATE INDEX IF NOT EXISTS idx_TradeStationInventory__TradeStationId_EconomyItemId ON TradeStationInventory(TradeStationId, EconomyItemId);
                    CREATE INDEX IF NOT EXISTS idx_EconomyEvent__EconomyEventName ON EconomyEvent(EconomyEventName);
                    CREATE INDEX IF NOT EXISTS idx_EconomyItem__EconomyItemClassId ON EconomyItem(EconomyItemClassId);
                    CREATE INDEX IF NOT EXISTS idx_EconomyItem__EconomyItemName ON EconomyItem(EconomyItemName);
                    CREATE INDEX IF NOT EXISTS idx_TradeStation__FactionId ON TradeStation(FactionId);
                    CREATE INDEX IF NOT EXISTS idx_TradeStation__TradeStationName ON TradeStation(TradeStationName);
                    CREATE INDEX IF NOT EXISTS idx_Faction__FactionName ON Faction(FactionName);
                    ");

                }
            }
        }
        public static void UpdateFactionData(Faction faction)
        {
            using (new TimedBlock("Update faction data in database", _debugThisClass))
            {
                using (var basicSql = new BasicSql())
                {
                    //if faction doesnt have an id, assign it one if it exists in database, will still be null if it doesnt
                    faction.FactionId = string.IsNullOrEmpty(faction.FactionId)
                    ? basicSql.ExecuteScalar(@"
                    SELECT FactionId FROM Faction WHERE FactionName = $factionName;",
                    new List<KeyValuePair<string, string>>
                    {
                    new KeyValuePair<string, string>("$factionName", faction.FactionName)
                    })
                    : faction.FactionId;

                    //if faction still doesnt have an id, it doesnt exist in database. add it and assign an id
                    if (string.IsNullOrEmpty(faction.FactionId))
                    {
                        faction.FactionId = basicSql.ExecuteScalar(@"
                        INSERT INTO Faction (FactionName, FactionDescription, IsDisabled) VALUES ($factionName, $factionDescription, $isDisabled);
                        SELECT last_insert_rowid();",
                        new List<KeyValuePair<string, string>>
                        {
                        new KeyValuePair<string, string>("$factionName", faction.FactionName),
                        new KeyValuePair<string, string>("$factionDescription", faction.FactionDescription),
                        new KeyValuePair<string, string>("$isDisabled", "False"),
                        });
                        if (_debugThisClass)
                        {
                            Debug.Log($"Faction {faction.FactionName} was added to the database with the following information:\n{faction}");
                        }
                    }

                    //if faction now has an id, then it does exist in database and should be updated vice added
                    else
                    {
                        basicSql.ExecuteNonReader(@"
                        UPDATE Faction
                        SET FactionId = $factionId, FactionName = $factionName, FactionDescription = $factionDescription
                        WHERE FactionId = $factionId;
                        ", new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("$factionId", faction.FactionId),
                            new KeyValuePair<string, string>("$factionName", faction.FactionName),
                            new KeyValuePair<string, string>("$factionDescription", faction.FactionDescription)
                        });
                        if (_debugThisClass)
                        {
                            Debug.Log($"Faction {faction.FactionName} was updated in the database with the following information:\n{faction}");
                        }
                    }
                }
            }
        }
        public static void UpdateFactionData(List<Faction> factions)
        {
            using (new TimedBlock("Update multiple factions data in database", _debugThisClass))
            {
                using (var basicSql = new BasicSql())
                {
                    List<Faction> factionsToAdd = new();
                    List<Faction> factionsToUpdate = new();
                    //split factions into a list of adding and a list of updating
                    foreach(var faction in factions)
                    {
                        //if faction doesnt have an id, assign it one if it exists in database, will still be null if it doesnt
                        faction.FactionId = string.IsNullOrEmpty(faction.FactionId)
                        ? basicSql.ExecuteScalar(@"
                        SELECT FactionId FROM Faction WHERE FactionName = $factionName;",
                        new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("$factionName", faction.FactionName)
                        })
                        : faction.FactionId;

                        //if faction still doesnt have an id, it doesnt exist in database. add it and assign an id
                        if (string.IsNullOrEmpty(faction.FactionId))
                        {
                            factionsToAdd.Add(faction);
                        }

                        //if faction now has an id, then it does exist in database and should be updated vice added
                        else
                        {
                            factionsToUpdate.Add(faction);
                        }
                    }

                    //add the applicable new factions to the database
                    if(factionsToAdd.Count > 0)
                    {
                        List<object[]> data = new List<object[]>();
                        foreach (var faction in factionsToAdd)
                        {
                            data.Add(new object[] { faction.FactionName, faction.FactionDescription, "False" });
                        }
                        var prams = new List<KeyValuePair<string, string>>();
                        var sql = @"
                        INSERT INTO	Faction
                        (FactionName, FactionDescription, IsDisabled)
                        VALUES";
                        var idx = 0;
                        foreach (var obj in data)
                        {
                            sql += idx > 0 ? "," : "";
                            sql += $"($factionName{idx},$factionDescription{idx},$isDisabled{idx})";
                            prams.Add(new KeyValuePair<string, string>($"$factionName{idx}", obj[0].ToString()));
                            prams.Add(new KeyValuePair<string, string>($"$factionDescription{idx}", obj[1].ToString()));
                            prams.Add(new KeyValuePair<string, string>($"$isDisabled{idx}", obj[2].ToString()));
                            idx++;
                        }
                        basicSql.ExecuteNonReader(sql, prams);

                        //assign the newly added faction its appropriate id
                        foreach (var faction in factionsToAdd)
                        {
                            faction.FactionId = basicSql.ExecuteScalar(@"
                            SELECT FactionId FROM Faction WHERE FactionName = $factionName;",
                            new List<KeyValuePair<string, string>>
                            {
                            new KeyValuePair<string, string>("$factionName", faction.FactionName)
                            });
                            if (_debugThisClass)
                            {
                                Debug.Log($"Faction {faction.FactionName} was added to the database with the following information:\n{faction}");
                            }
                        }

                    }

                    //update the applicable pre-existing factions in the database
                    if(factionsToUpdate.Count > 0)
                    {
                        foreach (var faction in factionsToUpdate)
                        {
                            basicSql.ExecuteNonReader(@"
                            UPDATE Faction
                            SET FactionName = $factionName, FactionDescription = $factionDescription
                            WHERE FactionId = $factionId;
                            ", new List<KeyValuePair<string, string>>
                            {
                                new KeyValuePair<string, string>("$factionId", faction.FactionId),
                                new KeyValuePair<string, string>("$factionName", faction.FactionName),
                                new KeyValuePair<string, string>("$factionDescription", faction.FactionDescription)
                            });
                            if (_debugThisClass)
                            {
                                Debug.Log($"Faction {faction.FactionName} was updated in the database with the following information:\n{faction}");
                            }
                        }
                    }
                }
            }
        }//Using this one is 4x faster
        public static void UpdateTradeStationData(TradeStation tradeStation)
        {
            using (new TimedBlock("Update trade station data in database", _debugThisClass))
            {
                using (var basicSql = new BasicSql())
                {
                    //if trade station doesnt have an id, assign it one if it exists in database, will still be null if it doesnt
                    tradeStation.TradeStationId = string.IsNullOrEmpty(tradeStation.TradeStationId)
                    ? basicSql.ExecuteScalar(@"
                    SELECT TradeStationId FROM TradeStation WHERE TradeStationName = $tradeStationName;",
                    new List<KeyValuePair<string, string>>
                    {
                    new KeyValuePair<string, string>("$tradeStationName", tradeStation.TradeStationName)
                    })
                    : tradeStation.TradeStationId;

                    //if trade station still doesnt have an id, it doesnt exist in database. add it and assign an id
                    if (string.IsNullOrEmpty(tradeStation.TradeStationId))
                    {
                        tradeStation.TradeStationId = basicSql.ExecuteScalar(@"
                        INSERT INTO TradeStation (FactionId, TradeStationName, TradeStationDescription) VALUES ($factionId, $tradeStationName, $tradeStationDescription, $tradeStationMoney);
                        SELECT last_insert_rowid();",
                        new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("$factionId", tradeStation.TradeStationId),
                            new KeyValuePair<string, string>("$tradeStationName", tradeStation.TradeStationName),
                            new KeyValuePair<string, string>("$tradeStationDescription", tradeStation.TradeStationDescription),
                            new KeyValuePair<string, string>("$tradeStationMoney", tradeStation.TradeStationMoney.ToString())
                            });
                        if (_debugThisClass)
                        {
                            Debug.Log($"TradeStation {tradeStation.TradeStationName} was added to the database with the following information:\n{tradeStation}");
                        }
                    }

                    //if trade station now has an id, then it does exist in database and should be updated vice added
                    else
                    {
                        basicSql.ExecuteNonReader(@"
                        UPDATE TradeStation
                        SET FactionId = $factionId, TradeStationName = $tradeStationName, TradeStationDescription = $tradeStationDescription, TradeStationMoney = $tradeStationMoney
                        WHERE TradeStationId = $tradeStationId;
                        ", new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("$tradeStationId", tradeStation.TradeStationId),
                            new KeyValuePair<string, string>("$factionId", tradeStation.FactionId),
                            new KeyValuePair<string, string>("$tradeStationName", tradeStation.TradeStationName),
                            new KeyValuePair<string, string>("$tradeStationDescription", tradeStation.TradeStationDescription),
                            new KeyValuePair<string, string>("$tradeStationMoney", tradeStation.TradeStationMoney.ToString())
                        });
                        if (_debugThisClass)
                        {
                            Debug.Log($"TradeStation {tradeStation.TradeStationName} was updated in the database with the following information:\n{tradeStation}");
                        }
                    }
                }
            }
        }
        public static void UpdateTradeStationData(List<TradeStation> tradeStationList)
        {
            using (new TimedBlock("Update multiple trade stations data in database", _debugThisClass))
            {
                using (var basicSql = new BasicSql(true))
                {
                    List<TradeStation> tradeStationsToAdd = new();
                    List<TradeStation> tradeStationsToUpdate = new();

                    var batchSize = 400;
                    var start = 0;
                    while (start < tradeStationList.Count)
                    {
                        var tradeStations = new List<TradeStation>();
                        for (var currentIdx = 0; currentIdx < batchSize && start + currentIdx < tradeStationList.Count; currentIdx++)
                        {
                            tradeStations.Add(tradeStationList[start + currentIdx]);
                        }
                        start += batchSize;

                        //split trade stations into a list of adding and a list of updating
                        foreach (var tradeStation in tradeStations)
                        {
                            //if trade station doesnt have an id, assign it one if it exists in database, will still be null if it doesnt
                            tradeStation.TradeStationId = string.IsNullOrEmpty(tradeStation.TradeStationId)
                            ? basicSql.ExecuteScalar(@"
                            SELECT TradeStationId FROM TradeStation WHERE TradeStationName = $tradeStationName;",
                            new List<KeyValuePair<string, string>>
                            {
                                new KeyValuePair<string, string>("$tradeStationName", tradeStation.TradeStationName)
                            })
                            : tradeStation.TradeStationId;

                            //if trade station still doesnt have an id, it doesnt exist in database. add it and assign an id
                            if (string.IsNullOrEmpty(tradeStation.TradeStationId))
                            {
                                tradeStationsToAdd.Add(tradeStation);
                            }

                            //if trade station now has an id, then it does exist in database and should be updated vice added
                            else
                            {
                                tradeStationsToUpdate.Add(tradeStation);
                            }
                        }

                        //add the applicable new trade stations to the database
                        if (tradeStationsToAdd.Count > 0)
                        {
                            List<object[]> data = new List<object[]>();
                            foreach (var tradeStation in tradeStationsToAdd)
                            {
                                data.Add(new object[] { tradeStation.FactionId, tradeStation.TradeStationName, tradeStation.TradeStationDescription, tradeStation.TradeStationMoney });
                            }
                            var prams = new List<KeyValuePair<string, string>>();
                            var sql = @"
                            INSERT INTO	TradeStation
                            (FactionId, TradeStationName, TradeStationDescription, TradeStationMoney)
                            VALUES";
                            var idx = 0;
                            foreach (var obj in data)
                            {
                                sql += idx > 0 ? "," : "";
                                sql += $"($factionId{idx},$tradeStationName{idx},$tradeStationDescription{idx},$tradeStationMoney{idx})";
                                prams.Add(new KeyValuePair<string, string>($"$factionId{idx}", obj[0].ToString()));
                                prams.Add(new KeyValuePair<string, string>($"$tradeStationName{idx}", obj[1].ToString()));
                                prams.Add(new KeyValuePair<string, string>($"$tradeStationDescription{idx}", obj[2].ToString()));
                                prams.Add(new KeyValuePair<string, string>($"$tradeStationMoney{idx}", obj[3].ToString()));
                                idx++;
                            }
                            sql += "ON CONFLICT DO NOTHING";
                            basicSql.ExecuteNonReader(sql, prams);

                            //assign the newly added trade station its appropriate id
                            foreach (var tradeStation in tradeStationsToAdd)
                            {
                                tradeStation.TradeStationId = basicSql.ExecuteScalar(@"
                                SELECT TradeStationId FROM TradeStation WHERE TradeStationName = $tradeStationName;",
                                new List<KeyValuePair<string, string>>
                                {
                                new KeyValuePair<string, string>("$tradeStationName", tradeStation.TradeStationName)
                                });
                                if (_debugThisClass)
                                {
                                    Debug.Log($"TradeStation {tradeStation.TradeStationName} was added to the database with the following information:\n{tradeStation}");
                                }
                            }
                        }
                    }

                    //update the applicable pre-existing trade stations in the database
                    if (tradeStationsToUpdate.Count > 0)
                    {
                        using (new TimedBlock("ASDF :: LOWER TRANS"))
                        {
                            basicSql.BeginTransaction();
                            foreach (var tradeStation in tradeStationsToUpdate)
                            {
                                basicSql.ExecuteNonReader(@"
                            UPDATE TradeStation
                            SET FactionId = $factionId, TradeStationName = $tradeStationName, TradeStationDescription = $tradeStationDescription
                            WHERE TradeStationId = $tradeStationId;
                            ", new List<KeyValuePair<string, string>>
                            {
                                new KeyValuePair<string, string>("$tradeStationId", tradeStation.TradeStationId),
                                new KeyValuePair<string, string>("$factionId", tradeStation.FactionId),
                                new KeyValuePair<string, string>("$tradeStationName", tradeStation.TradeStationName),
                                new KeyValuePair<string, string>("$tradeStationDescription", tradeStation.TradeStationDescription)
                            });
                                if (_debugThisClass)
                                {
                                    Debug.Log($"TradeStation {tradeStation} was updated in the database with the following information:\n{tradeStation}");
                                }
                            }
                            basicSql.CommitTransaction();
                        }
                    }
                }
            }
        }//Using this one is 4x faster
        public static void UpdateTradeStationInventoryData(TradeStation tradeStation)
        {
            using (new TimedBlock("Update trade station inventory data in database", _debugThisClass))
            {
                using (var basicSql = new BasicSql())
                {
                    foreach(var item in tradeStation.InventoryItems)
                    {
                        basicSql.ExecuteNonReader(@"
                        INSERT INTO TradeStationInventory (TradeStationId, EconomyItemId, QuantityOfItem, MaxQuantityOfItem, PurchasePrice, SalePrice, IsSpecialized) 
                        VALUES ($tradeStationId, $economyItemId, $quantityOfItem, $maxQuantityOfItem, $purchasePrice, $salePrice, $isSpecialized)
                        ON CONFLICT (TradeStationId, EconomyItemId)
                        DO UPDATE SET QuantityOfItem = $quantityOfItem, MaxQuantityOfItem = $maxQuantityOfItem, PurchasePrice = $purchasePrice, SalePrice = $salePrice, IsSpecialized = $isSpecialized",
                        new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("$tradeStationId", tradeStation.TradeStationId),
                            new KeyValuePair<string, string>("$economyItemId", item.EconomyItemId),
                            new KeyValuePair<string, string>("$quantityOfItem", item.QuantityOfItem.ToString()),
                            new KeyValuePair<string, string>("$maxQuantityOfItem", item.MaxQuantityOfItem.ToString()),
                            new KeyValuePair<string, string>("$purchasePrice", item.PurchasePrice.ToString()),
                            new KeyValuePair<string, string>("$salePrice", item.SalePrice.ToString()),
                            new KeyValuePair<string, string>("$isSpecialized", item.IsSpecialized.ToString())
                        });
                    }
                }
            }
        }
        public static void UpdateTradeStationInventoryData(List<TradeStation> tradeStations)
        {
            using (new TimedBlock("Update multiple trade station inventory data in database", _debugThisClass))
            {
                using (var basicSql = new BasicSql(true))
                {
                    basicSql.BeginTransaction();
                    foreach(var tradeStation in tradeStations)
                    {
                        foreach (var item in tradeStation.InventoryItems)
                        {
                            basicSql.ExecuteNonReader(@"
                            INSERT INTO TradeStationInventory (TradeStationId, EconomyItemId, QuantityOfItem, MaxQuantityOfItem, PurchasePrice, SalePrice, IsSpecialized) 
                            VALUES ($tradeStationId, $economyItemId, $quantityOfItem, $maxQuantityOfItem, $purchasePrice, $salePrice, $isSpecialized)
                            ON CONFLICT (TradeStationId, EconomyItemId)
                            DO UPDATE SET QuantityOfItem = $quantityOfItem, MaxQuantityOfItem = $maxQuantityOfItem, PurchasePrice = $purchasePrice, SalePrice = $salePrice, IsSpecialized = $isSpecialized",
                            new List<KeyValuePair<string, string>>
                            {
                                new KeyValuePair<string, string>("$tradeStationId", tradeStation.TradeStationId),
                                new KeyValuePair<string, string>("$economyItemId", item.EconomyItemId),
                                new KeyValuePair<string, string>("$quantityOfItem", item.QuantityOfItem.ToString()),
                                new KeyValuePair<string, string>("$maxQuantityOfItem", item.MaxQuantityOfItem.ToString()),
                                new KeyValuePair<string, string>("$purchasePrice", item.PurchasePrice.ToString()),
                                new KeyValuePair<string, string>("$salePrice", item.SalePrice.ToString()),
                                new KeyValuePair<string, string>("$isSpecialized", item.IsSpecialized.ToString())
                            });
                        }
                    }
                    basicSql.CommitTransaction();
                }
            }
        }//Using this one is 4x faster
        public static void UpdateItemData(EconomyItem item)
        {
            using (new TimedBlock("Update item data in database", _debugThisClass))
            {
                using (var basicSql = new BasicSql())
                {
                    //if item doesnt have an id, assign it one if it exists in database, will still be null if it doesnt
                    item.EconomyItemId = string.IsNullOrEmpty(item.EconomyItemId)
                    ? basicSql.ExecuteScalar(@"
                    SELECT EconomyItemId FROM EconomyItem WHERE EconomyItemName = $economyItemName;",
                    new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("$economyItemName", item.EconomyItemName)
                    })
                    : item.EconomyItemId;

                    //if item still doesnt have an id, it doesnt exist in database. add it and assign an id
                    if (string.IsNullOrEmpty(item.EconomyItemId))
                    {
                        item.EconomyItemId = basicSql.ExecuteScalar(@"
                        INSERT INTO EconomyItem (EconomyItemName, EconomyItemDescription, EconomyItemClassId, RarityInt, PriceDefault, PriceFloor, PriceRoof) VALUES ($economyItemName, $economyItemDescription, $economyItemClassId, $rarityInt, $priceDefault, $priceFloor, $priceRoof);
                        SELECT last_insert_rowid();",
                        new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("$economyItemName", item.EconomyItemName),
                            new KeyValuePair<string, string>("$economyItemDescription", item.EconomyItemDescription),
                            new KeyValuePair<string, string>("$economyItemClassId", item.EconomyItemClass.ToString()),
                            new KeyValuePair<string, string>("$rarityInt", item.RarityInt.ToString()),
                            new KeyValuePair<string, string>("$priceDefault", item.PriceDefault.ToString()),
                            new KeyValuePair<string, string>("$priceFloor", item.PriceFloor.ToString()),
                            new KeyValuePair<string, string>("$priceRoof", item.PriceRoof.ToString())
                        });
                        if (_debugThisClass)
                        {
                            Debug.Log($"EconomyItem {item.EconomyItemName} was added to the database with the following information:\n{item}");
                        }
                    }

                    //if item now has an id, then it does exist in database and should be updated vice added
                    else
                    {
                        basicSql.ExecuteNonReader(@"
                        UPDATE EconomyItem
                        SET EconomyItemName = $economyItemName, EconomyItemDescription = $economyItemDescription, EconomyItemClassId = $economyItemClassId, RarityInt = $rarityInt, PriceDefault = $priceDefault, PriceFloor = $priceFloor, PriceRoof = $priceRoof
                        WHERE EconomyItemId = $economyItemId;
                        ", new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("$economyItemId", item.EconomyItemId),
                            new KeyValuePair<string, string>("$economyItemName", item.EconomyItemName),
                            new KeyValuePair<string, string>("$economyItemDescription", item.EconomyItemDescription),
                            new KeyValuePair<string, string>("$economyItemClassId", item.EconomyItemClass.ToString()),
                            new KeyValuePair<string, string>("$rarityInt", item.RarityInt.ToString()),
                            new KeyValuePair<string, string>("$priceDefault", item.PriceDefault.ToString()),
                            new KeyValuePair<string, string>("$priceFloor", item.PriceFloor.ToString()),
                            new KeyValuePair<string, string>("$priceRoof", item.PriceRoof.ToString())
                        });
                        if (_debugThisClass)
                        {
                            Debug.Log($"EconomyItem {item.EconomyItemName} was updated in the database with the following information:\n{item}");
                        }
                    }
                }
            }
        }
        public static void UpdateItemData(List<EconomyItem> items)
        {
            using (new TimedBlock("Update multiple items data in database", _debugThisClass))
            {
                using (var basicSql = new BasicSql())
                {
                    List<EconomyItem> itemsToAdd = new();
                    List<EconomyItem> itemsToUpdate = new();
                    //split itemss into a list of adding and a list of updating
                    foreach (var item in items)
                    {
                        //if item doesnt have an id, assign it one if it exists in database, will still be null if it doesnt
                        item.EconomyItemId = string.IsNullOrEmpty(item.EconomyItemId)
                        ? basicSql.ExecuteScalar(@"
                        SELECT EconomyItemId FROM EconomyItem WHERE EconomyItemName = $economyItemName;",
                        new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("$economyItemName", item.EconomyItemName)
                        })
                        : item.EconomyItemId;

                        //if item still doesnt have an id, it doesnt exist in database. add it and assign an id
                        if (string.IsNullOrEmpty(item.EconomyItemId))
                        {
                            itemsToAdd.Add(item);
                        }

                        //if item now has an id, then it does exist in database and should be updated vice added
                        else
                        {
                            itemsToUpdate.Add(item);
                        }
                    }

                    //add the applicable new items to the database
                    if (itemsToAdd.Count > 0)
                    {
                        List<object[]> data = new List<object[]>();
                        foreach (var item in itemsToAdd)
                        {
                            //ItemName VARCHAR(100), ItemDescription TEXT, ItemClassId CHAR(10), RarityInt INTEGER, PriceDefault INTEGER, PriceFloor INTEGER, PriceRoof INTEGER);
                            data.Add(new object[] { item.EconomyItemName, item.EconomyItemDescription, item.EconomyItemClass, item.RarityInt, item.PriceDefault, item.PriceFloor, item.PriceRoof });
                        }
                        var prams = new List<KeyValuePair<string, string>>();
                        var sql = @"
                        INSERT INTO	EconomyItem
                        (EconomyItemName, EconomyItemDescription, EconomyItemClassId, RarityInt, PriceDefault, PriceFloor, PriceRoof)
                        VALUES";
                        var idx = 0;
                        foreach (var obj in data)
                        {
                            sql += idx > 0 ? "," : "";
                            sql += $"($economyItemName{idx},$economyItemDescription{idx},$economyItemClassId{idx},$rarityInt{idx},$priceDefault{idx},$priceFloor{idx},$priceRoof{idx})";
                            prams.Add(new KeyValuePair<string, string>($"$economyItemName{idx}", obj[0].ToString()));
                            prams.Add(new KeyValuePair<string, string>($"$economyItemDescription{idx}", obj[1].ToString()));
                            prams.Add(new KeyValuePair<string, string>($"$economyItemClassId{idx}", obj[2].ToString()));
                            prams.Add(new KeyValuePair<string, string>($"$rarityInt{idx}", obj[3].ToString()));
                            prams.Add(new KeyValuePair<string, string>($"$priceDefault{idx}", obj[4].ToString()));
                            prams.Add(new KeyValuePair<string, string>($"$priceFloor{idx}", obj[5].ToString()));
                            prams.Add(new KeyValuePair<string, string>($"$priceRoof{idx}", obj[6].ToString()));
                            idx++;
                        }
                        basicSql.ExecuteNonReader(sql, prams);

                        //assign the newly added item its appropriate id
                        foreach (var item in itemsToAdd)
                        {
                            item.EconomyItemId = basicSql.ExecuteScalar(@"
                            SELECT EconomyItemId FROM EconomyItem WHERE EconomyItemName = $economyItemName;",
                            new List<KeyValuePair<string, string>>
                            {
                            new KeyValuePair<string, string>("$economyItemName", item.EconomyItemName)
                            });
                            if (_debugThisClass)
                            {
                                Debug.Log($"EconomyItem {item.EconomyItemName} was added to the database with the following information:\n{item}");
                            }
                        }
                    }

                    //update the applicable pre-existing items in the database
                    if (itemsToUpdate.Count > 0)
                    {

                        foreach (var item in itemsToUpdate)
                        {
                            basicSql.UseTransaction = true;
                            basicSql.BeginTransaction();
                            basicSql.ExecuteNonReader(@"
                            UPDATE EconomyItem
                            SET EconomyItemName = $economyItemName, EconomyItemDescription = $economyItemDescription, EconomyItemClassId = $economyItemClassId, RarityInt = $rarityInt, PriceDefault = $priceDefault, PriceFloor = $priceFloor, PriceRoof = $priceRoof
                            WHERE EconomyItemId = $economyItemId;
                            ", new List<KeyValuePair<string, string>>
                            {
                                new KeyValuePair<string, string>("$economyItemId", item.EconomyItemId),
                                new KeyValuePair<string, string>("$economyItemName", item.EconomyItemName),
                                new KeyValuePair<string, string>("$economyItemDescription", item.EconomyItemDescription),
                                new KeyValuePair<string, string>("$economyItemClassId", item.EconomyItemClass.ToString()),
                                new KeyValuePair<string, string>("$rarityInt", item.RarityInt.ToString()),
                                new KeyValuePair<string, string>("$priceDefault", item.PriceDefault.ToString()),
                                new KeyValuePair<string, string>("$priceFloor", item.PriceFloor.ToString()),
                                new KeyValuePair<string, string>("$priceRoof", item.PriceRoof.ToString())
                            });
                            if (_debugThisClass)
                            {
                                Debug.Log($"EconomyItem {item.EconomyItemName} was updated in the database with the following information:\n{item}");
                            }
                            basicSql.CommitTransaction();
                            basicSql.UseTransaction = false;
                        }
                        
                    }
                }
            }
        }//using this one is 4x faster
        public static void UpdateEconomyEventData(EconomyEvent eEvent)
        {
            using (new TimedBlock("Update event data in database", _debugThisClass))
            {
                using (var basicSql = new BasicSql())
                {
                    //if event doesnt have an id, assign it one if it exists in database, will still be null if it doesnt
                    eEvent.EconomyEventId = string.IsNullOrEmpty(eEvent.EconomyEventId)
                    ? basicSql.ExecuteScalar(@"
                    SELECT EconomyEventId FROM EconomyEvent WHERE EconomyEventName = $economyEventName;",
                    new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("$economyEventName", eEvent.EconomyEventName)
                    })
                    : eEvent.EconomyEventId;

                    //if event still doesnt have an id, it doesnt exist in database. add it and assign an id
                    if (string.IsNullOrEmpty(eEvent.EconomyEventId))
                    {
                        eEvent.EconomyEventId = basicSql.ExecuteScalar(@"
                        INSERT INTO EconomyEvent (EconomyEventName, EconomyEventDescription) VALUES ($economyEventName, $economyEventDescription);
                        SELECT last_insert_rowid();",
                        new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("$economyEventName", eEvent.EconomyEventName),
                            new KeyValuePair<string, string>("$economyEventDescription", eEvent.EconomyEventDescription)
                        });
                        if (_debugThisClass)
                        {
                            Debug.Log($"EconomyEvent {eEvent.EconomyEventName} was added to the database with the following information:\n{eEvent}");
                        }
                    }

                    //if event now has an id, then it does exist in database and should be updated vice added
                    else
                    {
                        basicSql.ExecuteNonReader(@"
                        UPDATE EconomyEvent
                        SET EconomyEventName = $economyEventName, EconomyEventDescription = $economyEventDescription
                        WHERE EconomyEventId = $economyEventId;
                        ", new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("$economyEventId", eEvent.EconomyEventId),
                            new KeyValuePair<string, string>("$economyEventName", eEvent.EconomyEventName),
                            new KeyValuePair<string, string>("$economyEventDescription", eEvent.EconomyEventDescription)
                        });
                        if (_debugThisClass)
                        {
                            Debug.Log($"EconomyEvent {eEvent.EconomyEventName} was updated in the database with the following information:\n{eEvent}");
                        }
                    }
                }
            }
        }
        public static void UpdateEconomyEventData(List<EconomyEvent> events)
        {
            using (new TimedBlock("Update multiple events data in database", _debugThisClass))
            {
                using (var basicSql = new BasicSql())
                {
                    List<EconomyEvent> eventsToAdd = new();
                    List<EconomyEvent> eventsToUpdate = new();
                    //split events into a list of adding and a list of updating
                    foreach (var eEvent in events)
                    {
                        //if event doesnt have an id, assign it one if it exists in database, will still be null if it doesnt
                        eEvent.EconomyEventId = string.IsNullOrEmpty(eEvent.EconomyEventId)
                        ? basicSql.ExecuteScalar(@"
                        SELECT EconomyEventId FROM EconomyEvent WHERE EconomyEventName = $economyEventName;",
                        new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("$economyEventName", eEvent.EconomyEventName)
                        })
                        : eEvent.EconomyEventId;

                        //if event still doesnt have an id, it doesnt exist in database. add it and assign an id
                        if (string.IsNullOrEmpty(eEvent.EconomyEventId))
                        {
                            eventsToAdd.Add(eEvent);
                        }

                        //if event now has an id, then it does exist in database and should be updated vice added
                        else
                        {
                            eventsToUpdate.Add(eEvent);
                        }
                    }

                    //add the applicable new events to the database
                    if (eventsToAdd.Count > 0)
                    {
                        List<object[]> data = new List<object[]>();
                        foreach (var eEvent in eventsToAdd)
                        {
                            //EventName VARCHAR(100), EventDescription TEXT);
                            data.Add(new object[] { eEvent.EconomyEventName, eEvent.EconomyEventDescription });
                        }
                        var prams = new List<KeyValuePair<string, string>>();
                        var sql = @"
                        INSERT INTO	EconomyEvent
                        (EconomyEventName, EconomyEventDescription)
                        VALUES";
                        var idx = 0;
                        foreach (var obj in data)
                        {
                            sql += idx > 0 ? "," : "";
                            sql += $"($economyEventName{idx},$economyEventDescription{idx})";
                            prams.Add(new KeyValuePair<string, string>($"$economyEventName{idx}", obj[0].ToString()));
                            prams.Add(new KeyValuePair<string, string>($"$economyEventDescription{idx}", obj[1].ToString()));
                            idx++;
                        }
                        basicSql.ExecuteNonReader(sql, prams);

                        //assign new event its appropriate id
                        foreach (var eEvent in eventsToAdd)
                        {
                            eEvent.EconomyEventId = basicSql.ExecuteScalar(@"
                            SELECT EconomyEventId FROM EconomyEvent WHERE EconomyEventName = $economyEventName;",
                            new List<KeyValuePair<string, string>>
                            {
                            new KeyValuePair<string, string>("$economyEventName", eEvent.EconomyEventName)
                            });
                            if (_debugThisClass)
                            {
                                Debug.Log($"EconomyEvent {eEvent.EconomyEventName} was added to the database with the following information:\n{eEvent}");
                            }
                        }
                    }

                    //update the applicable pre-existing events in the database
                    if (eventsToUpdate.Count > 0)
                    {
                        basicSql.UseTransaction = true;
                        basicSql.BeginTransaction();
                        foreach (var eEvent in eventsToUpdate)
                        {
                            basicSql.ExecuteNonReader(@"
                            UPDATE EconomyEvent
                            SET EconomyEventName = $economyEventName, EconomyEventDescription = $economyEventDescription
                            WHERE EconomyEventId = $economyEventId;
                            ", new List<KeyValuePair<string, string>>
                            {
                                new KeyValuePair<string, string>("$economyEventId", eEvent.EconomyEventId),
                                new KeyValuePair<string, string>("$economyEventName", eEvent.EconomyEventName),
                                new KeyValuePair<string, string>("$economyEventDescription", eEvent.EconomyEventDescription)
                            });
                            if (_debugThisClass)
                            {
                                Debug.Log($"EconomyEvent {eEvent.EconomyEventName} was updated in the database with the following information:\n{eEvent}");
                            }
                        }
                        basicSql.CommitTransaction();
                        basicSql.UseTransaction = false;
                    }
                }
            }
        }//using this one is 4x faster
        public static T LoadDataObject<T>(DataObjectType dataType, string objectName, string objectId = null) where T : class
        {
            if(dataType == DataObjectType.Faction)
            {
                Faction faction = null;
                using (new TimedBlock($"Loading faction {objectName} data", _debugThisClass))
                {
                    using (var basicSql = new BasicSql())
                    {
                        string sql = @"SELECT * FROM Faction WHERE FactionName = $objectName";
                        basicSql.ExecuteReader(
                        sql,
                        new List<KeyValuePair<string, string>>()
                        {
                            new KeyValuePair<string, string>("$objectName", objectName),
                        },
                        (rowData) =>
                        {
                            faction = new Faction(rowData);
                        });
                    }
                }
                return faction as T;
            }
            return default;
        }//untested
        /*public static void UpdateObjectData<T>(DataObjectType dataType, object data) where T : class
        {
            if(dataType == DataObjectType.TradeStation)
            {
                UpdateTradeStationData(data);
            }
        }*/
        public static List<EconomyItem> LoadTradeStationInventory(string tradeStationId)
        {
            List<EconomyItem> inventory = new ();
            using (new TimedBlock("Loading trade station inventory", _debugThisClass))
            {
                //var first = true;
                using (var basicSql = new BasicSql())
                {
                    var sql = @"
                    SELECT TradeStationInventory.*, EconomyItem.*
                    FROM EconomyItem 
                    INNER JOIN TradeStationInventory ON EconomyItem.EconomyItemId = TradeStationInventory.EconomyItemId 
                    WHERE TradeStationInventory.TradeStationId = $tradeStationId";
                    basicSql.ExecuteReader(
                    sql,
                    new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("$tradeStationId", tradeStationId),
                    },
                    (rowData) =>
                    {
                        /*if (first)
                        {
                            for (var fieldNum = 0; fieldNum < rowData.FieldCount; fieldNum++)
                            {
                                var fieldName = rowData.GetName(fieldNum);
                                //Debug.Log($"FIELD NAME :: {fieldName}");
                            }
                        }*/
                        var itemToAdd = new EconomyItem(rowData, DataObjectType.TradeStationInventoryItem);
                        BasicSql.DebugRowData(rowData, _debugThisClass);
                        if (!inventory.Any(_ => _.EconomyItemId == itemToAdd.EconomyItemId))
                        {
                            inventory.Add(itemToAdd);
                        }
                    });
                }
            }
            return inventory;
        }
        public static List<EconomyItem> LoadEconomyItemsWithItemClass(ItemClass itemClass)
        {
            List<EconomyItem> items = new();
            using(new TimedBlock($"Loading economy items of {itemClass}", _debugThisClass))
            {
                using(var basicSql = new BasicSql())
                {
                    basicSql.ExecuteReader(@"SELECT * FROM EconomyItem WHERE EconomyItemClassId = $economtyItemClassId",
                    new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("$economyItemClassId", itemClass.ToString())
                    }, (rowData) =>
                    {
                        EconomyItem economyItemToAdd = new EconomyItem(rowData, DataObjectType.EconomyItem);
                        items.Add(economyItemToAdd);
                    });
                }
            }
            return items;
        }
        public static List<EconomyItem> LoadEconomyItemsWithItemClass(List<ItemClass> itemClasses)
        {
            List<EconomyItem> items = new();
            using (new TimedBlock($"Loading economy items of {itemClasses}", _debugThisClass))
            {
                using (var basicSql = new BasicSql())
                {
                    var prams = new List<KeyValuePair<string, string>>();
                    var sql = @"
                        SELECT * FROM EconomyItem
                        WHERE EconomyItemClassId IN (";
                    var idx = 0;
                    foreach (var itemClass in itemClasses)
                    {
                        sql += idx > 0 ? "," : "";
                        sql += $"$economyItemClassId{idx}";
                        prams.Add(new KeyValuePair<string, string>($"$economyItemClassId{idx}", itemClass.ToString()));
                        idx++;
                    }
                    sql += ")";

                    basicSql.ExecuteReader(
                    sql,
                    prams,
                    (rowData) =>
                    {
                        var itemToAdd = new EconomyItem(rowData, DataObjectType.EconomyItem);
                        if (!items.Any(_ => _.EconomyItemId == itemToAdd.EconomyItemId))
                        {
                            items.Add(itemToAdd);
                        }
                    });
                }
            }
            return items;
        }
        public static List<TradeStation> LoadTradeStationsForFaction(string factionId)
        {
            List<TradeStation> tradeStations = new();
            using (new TimedBlock("Loading trade stations", _debugThisClass))
            {
                using (var basicSql = new BasicSql())
                {
                    var sql = @"
                    SELECT *
                    FROM TradeStation 
                    WHERE FactionId = $factionId";
                    basicSql.ExecuteReader(
                    sql,
                    new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("$factionId", factionId),
                    },
                    (rowData) =>
                    {
                        var tradeStationToAdd = new TradeStation(rowData);
                        if (!tradeStations.Any(_ => _.TradeStationId == tradeStationToAdd.TradeStationId))
                        {
                            tradeStations.Add(tradeStationToAdd);
                        }
                    });
                }
            }
            foreach (var tradeStation in tradeStations)
            {
                tradeStation.LoadInventory();
                tradeStation.ReCalculatePriceDistribution();
            }
            return tradeStations;
        }
    }
    public enum DataObjectType
    {
        Faction, TradeStation, EconomyItem, EconomyEvent, TradeStationInventoryItem
    }
}