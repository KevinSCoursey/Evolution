using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;

namespace Economy
{
    public class EconomyController
    {
        public EconomyController()
        {
            Initialize();
        }
        public void Initialize()
        {

        }
        /*public static bool DoesTradeRouteExist(TradeRoute tradeRoute)
        {
            if (AllTradeRoutes.Count == 0 || tradeRoute == null) return false;
            foreach(var tradeRouteParse in AllTradeRoutes)
            {
                if(tradeRouteParse.Equals(tradeRoute)) return true;
            }
            return false;
        }*/
        
        

        /*#region SQLite
        public void SaveData()
        {
            using (var basicSql = new BasicSql())
            {
                if (GameSettings.RegenerateSQLiteDBsEachRun)
                {
                    basicSql.ExecuteNonReader("DROP TABLE IF EXISTS EconomyItem");
                    basicSql.ExecuteNonReader("DROP TABLE IF EXISTS EconomyEvent");
                    basicSql.ExecuteNonReader("DROP TABLE IF EXISTS EconomyEventItemClassLink");
                }
                basicSql.ExecuteNonReader(@"
                CREATE TABLE IF NOT EXISTS EconomyItem (Id INTEGER PRIMARY KEY, ItemName VARCHAR(100), ItemDescription TEXT, ItemClassId CHAR(10), RarityInt INTEGER, PriceDefault INTEGER, PriceFloor INTEGER, PriceRoof INTEGER);
                ");

                basicSql.ExecuteNonReader(@"
                CREATE TABLE IF NOT EXISTS EconomyEvent (Id INTEGER PRIMARY KEY, EventName VARCHAR(100), EventDescription TEXT);
                ");

                basicSql.ExecuteNonReader(@"
                CREATE TABLE IF NOT EXISTS EconomyEventItemClassLink (Id INTEGER PRIMARY KEY, EventId CHAR(10), ItemClassId CHAR(10));
                ");

                

                foreach (var economyItem in economyItemController.items)
                {
                    string itemName = basicSql.ExecuteScalar(@"SELECT ItemName FROM EconomyItem WHERE ItemName = $itemName;",
                        new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("$itemName", economyItem.itemName)
                        }
                    );

                    if (string.IsNullOrEmpty(itemName))
                    {
                        basicSql.ExecuteNonReader(
                            "INSERT INTO EconomyItem (ItemName, ItemDescription, ItemClassId, RarityInt, PriceDefault, PriceFloor, PriceRoof) VALUES ($itemName, $itemDescription, $itemClassId, $rarityInt, $priceDefault, $priceFloor, $priceRoof)",
                            new List<KeyValuePair<string, string>>
                            {
                                new KeyValuePair<string, string>("$itemName", economyItem.itemName),
                                new KeyValuePair<string, string>("$itemDescription", economyItem.itemDescription),
                                new KeyValuePair<string, string>("$itemClassId", ((int)economyItem.itemClass).ToString()),
                                new KeyValuePair<string, string>("$rarityInt", economyItem.rarityInt.ToString()),
                                new KeyValuePair<string, string>("$priceDefault", economyItem.priceDefault.ToString()),
                                new KeyValuePair<string, string>("$priceFloor", economyItem.priceFloor.ToString()),
                                new KeyValuePair<string, string>("$priceRoof", economyItem.priceRoof.ToString())
                            }
                            );
                    }
                }

                //assign itemids
                foreach (var item in economyItemController.items)
                {
                    item.itemId = string.IsNullOrEmpty(item.itemId)
                            ? basicSql.ExecuteScalar(@"SELECT Id FROM EconomyItem WHERE ItemName = $itemName",
                            new List<KeyValuePair<string, string>>
                            {
                                    new KeyValuePair<string, string>("$itemName", item.itemName)
                            })
                            : item.itemId;
                }

                foreach (var economyEvent in economyEventController.economyEvents)
                {
                    string eventName = basicSql.ExecuteScalar(@"SELECT EventName FROM EconomyEvent WHERE EventName = $eventName;",
                        new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("$eventName", economyEvent.eventName)
                        }
                    );

                    if (string.IsNullOrEmpty(eventName))
                    {
                        basicSql.ExecuteNonReader(
                            "INSERT INTO EconomyEvent (EventName, EventDescription) VALUES ($eventName, $eventDescription)",
                            new List<KeyValuePair<string, string>>*//*, $itemClassesEffected*//*
                            {
                                //todo//new KeyValuePair<string, string>("$itemClassesEffected", economyEvent.eventId),
                                new KeyValuePair<string, string>("$eventName", economyEvent.eventName),
                                new KeyValuePair<string, string>("$eventDescription", economyEvent.eventDescription)
                            }
                            );
                    }
                }

                foreach (var economyEvent in economyEventController.economyEvents)
                {
                    List<KeyValuePair<string, string>> dataElement = new List<KeyValuePair<string, string>>();
                    string eventId = basicSql.ExecuteScalar(@"SELECT Id FROM EconomyEvent WHERE EventName = $eventName;",
                        new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("$eventName", economyEvent.eventName)
                        }
                    );
                    foreach(var itemClassId in economyEvent.ItemClassesEffectedByEvent)
                    {
                        dataElement.Add(new KeyValuePair<string, string>(eventId.ToString(), itemClassId.ToString()));
                    }

                    var prams = new List<KeyValuePair<string, string>>();
                    var sql = @"
                            INSERT INTO	EconomyEventItemClassLink
                            (EventId, ItemClassId)
                            VALUES
                            ";

                    var idx = 0;
                    foreach (var kvp in dataElement)
                    {
                        var linkFromId = kvp.Key;
                        var linkToId = kvp.Value;
                        sql += idx > 0 ? "," : "";
                        sql += $"($fromLink{idx},$linkTo{idx})";
                        prams.Add(new KeyValuePair<string, string>($"$fromLink{idx}", linkFromId.ToString()));
                        prams.Add(new KeyValuePair<string, string>($"$linkTo{idx}", linkToId.ToString()));
                        idx++;
                    }
                    basicSql.ExecuteNonReader(sql, prams);

                }
            }
        }
        #endregion SQLite*/
    }
}
