using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;

namespace Economy
{
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
                    CREATE TABLE IF NOT EXISTS Faction (Id INTEGER PRIMARY KEY, Name VARCHAR(100), Description TEXT, IsDisabled INTEGER);
                    CREATE TABLE IF NOT EXISTS TradeStation (Id INTEGER PRIMARY KEY, FactionId VARCHAR(3), Name VARCHAR(100), Description TEXT);
                    CREATE TABLE IF NOT EXISTS FactionLinks (Id INTEGER PRIMARY KEY, FacID1 VARCHAR(3), FacID2 VARCHAR(3), Reputation INTEGER);
                    CREATE TABLE IF NOT EXISTS TradeStationInventory (Id INTEGER PRIMARY KEY, TradeStationId VARCHAR(3), ItemId VARCHAR(3), MaxQuantityOfItem INTEGER, PurchasePrice INTEGER, SalePrice INTEGER, IsSpecialized INTEGER);
                    CREATE TABLE IF NOT EXISTS EconomyEventTradeStationLink (Id INTEGER PRIMARY KEY, EventId VARCHAR(3), TradeStationId VARCHAR(3));
                    CREATE TABLE IF NOT EXISTS EconomyItem(Id INTEGER PRIMARY KEY, ItemName VARCHAR(100), ItemDescription TEXT, ItemClassId CHAR(10), RarityInt INTEGER, PriceDefault INTEGER, PriceFloor INTEGER, PriceRoof INTEGER);
                    CREATE TABLE IF NOT EXISTS EconomyEvent (Id INTEGER PRIMARY KEY, EventName VARCHAR(100), EventDescription TEXT);
                    CREATE TABLE IF NOT EXISTS EconomyEventItemClassLink (Id INTEGER PRIMARY KEY, EventId CHAR(10), ItemClassId CHAR(10));");
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
                    faction.factionId = string.IsNullOrEmpty(faction.factionId)
                    ? basicSql.ExecuteScalar(@"
                    SELECT Id FROM Faction WHERE Name = $name;",
                    new List<KeyValuePair<string, string>>
                    {
                    new KeyValuePair<string, string>("$name", faction.factionName)
                    })
                    : faction.factionId;

                    //if faction still doesnt have an id, it doesnt exist in database. add it and assign an id
                    if (string.IsNullOrEmpty(faction.factionId))
                    {
                        faction.factionId = basicSql.ExecuteScalar(@"
                        INSERT INTO Faction (Name, Description, IsDisabled) VALUES ($name, $description, $isDisabled);
                        SELECT last_insert_rowid();",
                        new List<KeyValuePair<string, string>>
                        {
                        new KeyValuePair<string, string>("$name", faction.factionName),
                        new KeyValuePair<string, string>("$description", faction.factionDescription),
                        new KeyValuePair<string, string>("$isDisabled", "False"),
                        });
                        if (_debugThisClass)
                        {
                            Debug.Log($"Faction {faction.factionName} was added to the database with the following information:\n{faction}");
                        }
                    }

                    //if faction now has an id, then it does exist in database and should be updated vice added
                    else
                    {
                        basicSql.ExecuteNonReader(@"
                        UPDATE Faction
                        SET FactionId = Name = $name, Description = $description
                        WHERE Id = $id;
                        ", new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("$id", faction.factionId),
                            new KeyValuePair<string, string>("$name", faction.factionName),
                            new KeyValuePair<string, string>("$description", faction.factionDescription)
                        });
                        if (_debugThisClass)
                        {
                            Debug.Log($"Faction {faction.factionName} was updated in the database with the following information:\n{faction}");
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
                        faction.factionId = string.IsNullOrEmpty(faction.factionId)
                        ? basicSql.ExecuteScalar(@"
                        SELECT Id FROM Faction WHERE Name = $name;",
                        new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("$name", faction.factionName)
                        })
                        : faction.factionId;

                        //if faction still doesnt have an id, it doesnt exist in database. add it and assign an id
                        if (string.IsNullOrEmpty(faction.factionId))
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
                            data.Add(new object[] { faction.factionName, faction.factionDescription, "False" });
                        }
                        var prams = new List<KeyValuePair<string, string>>();
                        var sql = @"
                        INSERT INTO	Faction
                        (Name, Description, IsDisabled)
                        VALUES";
                        var idx = 0;
                        foreach (var obj in data)
                        {
                            sql += idx > 0 ? "," : "";
                            sql += $"($name{idx},$description{idx},$isDisabled{idx})";
                            prams.Add(new KeyValuePair<string, string>($"$name{idx}", obj[0].ToString()));
                            prams.Add(new KeyValuePair<string, string>($"$description{idx}", obj[1].ToString()));
                            prams.Add(new KeyValuePair<string, string>($"$isDisabled{idx}", obj[2].ToString()));
                            idx++;
                        }
                        basicSql.ExecuteNonReader(sql, prams);

                        //assign the newly added faction its appropriate id
                        foreach (var faction in factionsToAdd)
                        {
                            faction.factionId = basicSql.ExecuteScalar(@"
                            SELECT Id FROM Faction WHERE Name = $name;",
                            new List<KeyValuePair<string, string>>
                            {
                            new KeyValuePair<string, string>("$name", faction.factionName)
                            });
                            if (_debugThisClass)
                            {
                                Debug.Log($"Faction {faction.factionName} was added to the database with the following information:\n{faction}");
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
                            SET Name = $name, Description = $description
                            WHERE Id = $id;
                            ", new List<KeyValuePair<string, string>>
                            {
                                new KeyValuePair<string, string>("$id", faction.factionId),
                                new KeyValuePair<string, string>("$name", faction.factionName),
                                new KeyValuePair<string, string>("$description", faction.factionDescription)
                            });
                            if (_debugThisClass)
                            {
                                Debug.Log($"Faction {faction.factionName} was updated in the database with the following information:\n{faction}");
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
                    tradeStation.tradeStationId = string.IsNullOrEmpty(tradeStation.tradeStationId)
                    ? basicSql.ExecuteScalar(@"
                    SELECT Id FROM TradeStation WHERE Name = $name;",
                    new List<KeyValuePair<string, string>>
                    {
                    new KeyValuePair<string, string>("$name", tradeStation.tradeStationName)
                    })
                    : tradeStation.tradeStationId;

                    //if trade station still doesnt have an id, it doesnt exist in database. add it and assign an id
                    if (string.IsNullOrEmpty(tradeStation.tradeStationId))
                    {
                        tradeStation.tradeStationId = basicSql.ExecuteScalar(@"
                        INSERT INTO TradeStation (FactionId, Name, Description) VALUES ($factionId, $name, $description);
                        SELECT last_insert_rowid();",
                        new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("$factionId", tradeStation.tradeStationId),
                            new KeyValuePair<string, string>("$name", tradeStation.tradeStationName),
                            new KeyValuePair<string, string>("$description", tradeStation.tradeStationDescription)
                            });
                        if (_debugThisClass)
                        {
                            Debug.Log($"TradeStation {tradeStation.tradeStationName} was added to the database with the following information:\n{tradeStation}");
                        }
                    }

                    //if trade station now has an id, then it does exist in database and should be updated vice added
                    else
                    {
                        basicSql.ExecuteNonReader(@"
                        UPDATE TradeStation
                        SET FactionId = $factionId, Name = $name, Description = $description
                        WHERE Id = $id;
                        ", new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("$id", tradeStation.tradeStationId),
                            new KeyValuePair<string, string>("$factionId", tradeStation.factionId),
                            new KeyValuePair<string, string>("$name", tradeStation.tradeStationName),
                            new KeyValuePair<string, string>("$description", tradeStation.tradeStationDescription)
                        });
                        if (_debugThisClass)
                        {
                            Debug.Log($"TradeStation {tradeStation.tradeStationName} was updated in the database with the following information:\n{tradeStation}");
                        }
                    }
                }
            }
        }
        public static void UpdateTradeStationData(List<TradeStation> tradeStations)
        {
            using (new TimedBlock("Update multiple trade stations data in database", _debugThisClass))
            {
                using (var basicSql = new BasicSql())
                {
                    List<TradeStation> tradeStationsToAdd = new();
                    List<TradeStation> tradeStationsToUpdate = new();
                    //split trade stations into a list of adding and a list of updating
                    foreach (var tradeStation in tradeStations)
                    {
                        //if trade station doesnt have an id, assign it one if it exists in database, will still be null if it doesnt
                        tradeStation.tradeStationId = string.IsNullOrEmpty(tradeStation.tradeStationId)
                        ? basicSql.ExecuteScalar(@"
                        SELECT Id FROM TradeStation WHERE Name = $name;",
                        new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("$name", tradeStation.tradeStationName)
                        })
                        : tradeStation.tradeStationId;

                        //if trade station still doesnt have an id, it doesnt exist in database. add it and assign an id
                        if (string.IsNullOrEmpty(tradeStation.tradeStationId))
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
                            data.Add(new object[] { tradeStation.factionId, tradeStation.tradeStationName, tradeStation.tradeStationDescription });
                        }
                        var prams = new List<KeyValuePair<string, string>>();
                        var sql = @"
                        INSERT INTO	TradeStation
                        (FactionId, Name, Description)
                        VALUES";
                        var idx = 0;
                        foreach (var obj in data)
                        {
                            sql += idx > 0 ? "," : "";
                            sql += $"($factionId{idx},$name{idx},$description{idx})";
                            prams.Add(new KeyValuePair<string, string>($"$factionId{idx}", obj[0].ToString()));
                            prams.Add(new KeyValuePair<string, string>($"$name{idx}", obj[1].ToString()));
                            prams.Add(new KeyValuePair<string, string>($"$description{idx}", obj[2].ToString()));
                            idx++;
                        }
                        Debug.Log(sql);
                        basicSql.ExecuteNonReader(sql, prams);

                        //assign the newly added trade station its appropriate id
                        foreach (var tradeStation in tradeStationsToAdd)
                        {
                            tradeStation.tradeStationId = basicSql.ExecuteScalar(@"
                            SELECT Id FROM TradeStation WHERE Name = $name;",
                            new List<KeyValuePair<string, string>>
                            {
                            new KeyValuePair<string, string>("$name", tradeStation.tradeStationName)
                            });
                            if (_debugThisClass)
                            {
                                Debug.Log($"TradeStation {tradeStation.tradeStationName} was added to the database with the following information:\n{tradeStation}");
                            }
                        }

                    }

                    //update the applicable pre-existing trade stations in the database
                    if (tradeStationsToUpdate.Count > 0)
                    {
                        foreach (var tradeStation in tradeStationsToUpdate)
                        {
                            basicSql.ExecuteNonReader(@"
                            UPDATE TradeStation
                            SET FactionId = $factionId, Name = $name, Description = $description
                            WHERE Id = $id;
                            ", new List<KeyValuePair<string, string>>
                            {
                                new KeyValuePair<string, string>("$id", tradeStation.tradeStationId),
                                new KeyValuePair<string, string>("$factionId", tradeStation.factionId),
                                new KeyValuePair<string, string>("$name", tradeStation.tradeStationName),
                                new KeyValuePair<string, string>("$description", tradeStation.tradeStationDescription)
                            });
                            if (_debugThisClass)
                            {
                                Debug.Log($"TradeStation {tradeStation} was updated in the database with the following information:\n{tradeStation}");
                            }
                        }
                    }
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
                    item.itemId = string.IsNullOrEmpty(item.itemId)
                    ? basicSql.ExecuteScalar(@"
                    SELECT Id FROM EconomyItem WHERE ItemName = $name;",
                    new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("$name", item.itemName)
                    })
                    : item.itemId;

                    //if item still doesnt have an id, it doesnt exist in database. add it and assign an id
                    if (string.IsNullOrEmpty(item.itemId))
                    {
                        item.itemId = basicSql.ExecuteScalar(@"
                        INSERT INTO EconomyItem (ItemName, ItemDescription, ItemClassId, RarityInt, PriceDefault, PriceFloor, PriceRoof) VALUES ($itemName, $itemDescription, $itemClassId, $rarityInt, $priceDefault, $priceFloor, $priceRoof);
                        SELECT last_insert_rowid();",
                        new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("$itemName", item.itemName),
                            new KeyValuePair<string, string>("$itemDescription", item.itemDescription),
                            new KeyValuePair<string, string>("$itemClassId", item.itemClass.ToString()),
                            new KeyValuePair<string, string>("$rarityInt", item.rarityInt.ToString()),
                            new KeyValuePair<string, string>("$priceDefault", item.priceDefault.ToString()),
                            new KeyValuePair<string, string>("$priceFloor", item.priceFloor.ToString()),
                            new KeyValuePair<string, string>("$priceRoof", item.priceRoof.ToString())
                        });
                        if (_debugThisClass)
                        {
                            Debug.Log($"EconomyItem {item.itemName} was added to the database with the following information:\n{item}");
                        }
                    }

                    //if item now has an id, then it does exist in database and should be updated vice added
                    else
                    {
                        basicSql.ExecuteNonReader(@"
                        UPDATE EconomyItem
                        SET ItemName = $itemName, ItemDescription = $itemDescription, ItemClassId = $itemClassId, RarityInt = $rarityInt, PriceDefault = $priceDefault, PriceFloor = $priceFloor, PriceRoof = $priceRoof
                        WHERE Id = $id;
                        ", new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("$id", item.itemId),
                            new KeyValuePair<string, string>("$itemName", item.itemName),
                            new KeyValuePair<string, string>("$itemDescription", item.itemDescription),
                            new KeyValuePair<string, string>("$itemClassId", item.itemClass.ToString()),
                            new KeyValuePair<string, string>("$rarityInt", item.rarityInt.ToString()),
                            new KeyValuePair<string, string>("$priceDefault", item.priceDefault.ToString()),
                            new KeyValuePair<string, string>("$priceFloor", item.priceFloor.ToString()),
                            new KeyValuePair<string, string>("$priceRoof", item.priceRoof.ToString())
                        });
                        if (_debugThisClass)
                        {
                            Debug.Log($"EconomyItem {item.itemName} was updated in the database with the following information:\n{item}");
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
                        item.itemId = string.IsNullOrEmpty(item.itemId)
                        ? basicSql.ExecuteScalar(@"
                        SELECT Id FROM EconomyItem WHERE ItemName = $name;",
                        new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("$name", item.itemName)
                        })
                        : item.itemId;

                        //if item still doesnt have an id, it doesnt exist in database. add it and assign an id
                        if (string.IsNullOrEmpty(item.itemId))
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
                            data.Add(new object[] { item.itemName, item.itemDescription, item.itemClass, item.rarityInt, item.priceDefault, item.priceFloor, item.priceRoof });
                        }
                        var prams = new List<KeyValuePair<string, string>>();
                        var sql = @"
                        INSERT INTO	EconomyItem
                        (ItemName, ItemDescription, ItemClassId, RarityInt, PriceDefault, PriceFloor, PriceRoof)
                        VALUES";
                        var idx = 0;
                        foreach (var obj in data)
                        {
                            sql += idx > 0 ? "," : "";
                            sql += $"($name{idx},$description{idx},$itemClassId{idx},$rarityInt{idx},$priceDefault{idx},$priceFloor{idx},$priceRoof{idx})";
                            prams.Add(new KeyValuePair<string, string>($"$name{idx}", obj[0].ToString()));
                            prams.Add(new KeyValuePair<string, string>($"$description{idx}", obj[1].ToString()));
                            prams.Add(new KeyValuePair<string, string>($"$itemClassId{idx}", obj[2].ToString()));
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
                            item.itemId = basicSql.ExecuteScalar(@"
                            SELECT Id FROM EconomyItem WHERE ItemName = $name;",
                            new List<KeyValuePair<string, string>>
                            {
                            new KeyValuePair<string, string>("$name", item.itemName)
                            });
                            if (_debugThisClass)
                            {
                                Debug.Log($"EconomyItem {item.itemName} was added to the database with the following information:\n{item}");
                            }
                        }
                    }

                    //update the applicable pre-existing items in the database
                    if (itemsToUpdate.Count > 0)
                    {
                        foreach (var item in itemsToUpdate)
                        {
                            basicSql.ExecuteNonReader(@"
                            UPDATE EconomyItem
                            SET ItemName = $itemName, ItemDescription = $itemDescription, ItemClassId = $itemClassId, RarityInt = $rarityInt, PriceDefault = $priceDefault, PriceFloor = $priceFloor, PriceRoof = $priceRoof
                            WHERE Id = $id;
                            ", new List<KeyValuePair<string, string>>
                            {
                                new KeyValuePair<string, string>("$id", item.itemId),
                                new KeyValuePair<string, string>("$itemName", item.itemName),
                                new KeyValuePair<string, string>("$itemDescription", item.itemDescription),
                                new KeyValuePair<string, string>("$itemClassId", item.itemClass.ToString()),
                                new KeyValuePair<string, string>("$rarityInt", item.rarityInt.ToString()),
                                new KeyValuePair<string, string>("$priceDefault", item.priceDefault.ToString()),
                                new KeyValuePair<string, string>("$priceFloor", item.priceFloor.ToString()),
                                new KeyValuePair<string, string>("$priceRoof", item.priceRoof.ToString())
                            });
                            if (_debugThisClass)
                            {
                                Debug.Log($"EconomyItem {item.itemName} was updated in the database with the following information:\n{item}");
                            }
                        }
                        
                    }
                }
            }
        }//using this one is 4x faster
        public static void UpdateEventData(EconomyEvent eEvent)
        {
            using (new TimedBlock("Update event data in database", _debugThisClass))
            {
                using (var basicSql = new BasicSql())
                {
                    //if event doesnt have an id, assign it one if it exists in database, will still be null if it doesnt
                    eEvent.eventId = string.IsNullOrEmpty(eEvent.eventId)
                    ? basicSql.ExecuteScalar(@"
                    SELECT Id FROM EconomyEvent WHERE EventName = $name;",
                    new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("$name", eEvent.eventName)
                    })
                    : eEvent.eventId;

                    //if event still doesnt have an id, it doesnt exist in database. add it and assign an id
                    if (string.IsNullOrEmpty(eEvent.eventId))
                    {
                        eEvent.eventId = basicSql.ExecuteScalar(@"
                        INSERT INTO EconomyEvent (EventName, EventDescription) VALUES ($eventName, $eventDescription);
                        SELECT last_insert_rowid();",
                        new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("$eventName", eEvent.eventName),
                            new KeyValuePair<string, string>("$eventDescription", eEvent.eventDescription)
                        });
                        if (_debugThisClass)
                        {
                            Debug.Log($"EconomyEvent {eEvent.eventName} was added to the database with the following information:\n{eEvent}");
                        }
                    }

                    //if event now has an id, then it does exist in database and should be updated vice added
                    else
                    {
                        basicSql.ExecuteNonReader(@"
                        UPDATE EconomyEvent
                        SET EventName = $eventName, EventDescription = $eventDescription
                        WHERE Id = $id;
                        ", new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("$id", eEvent.eventId),
                            new KeyValuePair<string, string>("$eventName", eEvent.eventName),
                            new KeyValuePair<string, string>("$eventDescription", eEvent.eventDescription)
                        });
                        if (_debugThisClass)
                        {
                            Debug.Log($"EconomyEvent {eEvent.eventName} was updated in the database with the following information:\n{eEvent}");
                        }
                    }
                }
            }
        }
        public static void UpdateEventData(List<EconomyEvent> events)
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
                        eEvent.eventId = string.IsNullOrEmpty(eEvent.eventId)
                        ? basicSql.ExecuteScalar(@"
                        SELECT Id FROM EconomyEvent WHERE EventName = $name;",
                        new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("$name", eEvent.eventName)
                        })
                        : eEvent.eventId;

                        //if event still doesnt have an id, it doesnt exist in database. add it and assign an id
                        if (string.IsNullOrEmpty(eEvent.eventId))
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
                            data.Add(new object[] { eEvent.eventName, eEvent.eventDescription });
                        }
                        var prams = new List<KeyValuePair<string, string>>();
                        var sql = @"
                        INSERT INTO	EconomyEvent
                        (EventName, EventDescription)
                        VALUES";
                        var idx = 0;
                        foreach (var obj in data)
                        {
                            sql += idx > 0 ? "," : "";
                            sql += $"($name{idx},$description{idx})";
                            prams.Add(new KeyValuePair<string, string>($"$name{idx}", obj[0].ToString()));
                            prams.Add(new KeyValuePair<string, string>($"$description{idx}", obj[1].ToString()));
                            idx++;
                        }
                        basicSql.ExecuteNonReader(sql, prams);

                        //assign new event its appropriate id
                        foreach (var eEvent in eventsToAdd)
                        {
                            eEvent.eventId = basicSql.ExecuteScalar(@"
                            SELECT Id FROM EconomyEvent WHERE EventName = $name;",
                            new List<KeyValuePair<string, string>>
                            {
                            new KeyValuePair<string, string>("$name", eEvent.eventName)
                            });
                            if (_debugThisClass)
                            {
                                Debug.Log($"EconomyEvent {eEvent.eventName} was added to the database with the following information:\n{eEvent}");
                            }
                        }
                    }

                    //update the applicable pre-existing events in the database
                    if (eventsToUpdate.Count > 0)
                    {
                        foreach (var eEvent in eventsToUpdate)
                        {
                            basicSql.ExecuteNonReader(@"
                            UPDATE EconomyEvent
                            SET EventName = $eventName, EventDescription = $eventDescription
                            WHERE Id = $id;
                            ", new List<KeyValuePair<string, string>>
                            {
                                new KeyValuePair<string, string>("$id", eEvent.eventId),
                                new KeyValuePair<string, string>("$eventName", eEvent.eventName),
                                new KeyValuePair<string, string>("$eventDescription", eEvent.eventDescription)
                            });
                            if (_debugThisClass)
                            {
                                Debug.Log($"EconomyEvent {eEvent.eventName} was updated in the database with the following information:\n{eEvent}");
                            }
                        }
                    }
                }
            }
        }//using this one is 4x faster
    }
}