using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;

namespace Economy
{
    public class EconomyController
    {
        private FactionController factionController;
        private List<Faction> factions;

        public EconomyItemController economyItemController;
        public EconomyEventController economyEventController;
        public static List<TradeRoute> AllTradeRoutes = new List<TradeRoute>();

        public EconomyController(FactionController factionController)
        {
            this.factionController = factionController;
            factions = factionController.factions;
            economyItemController = new EconomyItemController(factions);
            economyEventController = new EconomyEventController(factionController);
            Initialize();
        }

        public static bool DoesTradeRouteExist(TradeRoute tradeRoute)
        {
            if (AllTradeRoutes.Count == 0 || tradeRoute == null) return false;
            foreach(var tradeRouteParse in AllTradeRoutes)
            {
                if(tradeRouteParse.Equals(tradeRoute)) return true;
            }
            return false;
        }
        
        public void Initialize()
        {
            economyItemController.Initialize();
            economyEventController.Initialize();
        }

        #region SQLite
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
                            new KeyValuePair<string, string>("$itemName", economyItem.ItemName)
                        }
                    );

                    if (string.IsNullOrEmpty(itemName))
                    {
                        basicSql.ExecuteNonReader(
                            "INSERT INTO EconomyItem (ItemName, ItemDescription, ItemClassId, RarityInt, PriceDefault, PriceFloor, PriceRoof) VALUES ($itemName, $itemDescription, $itemClassId, $rarityInt, $priceDefault, $priceFloor, $priceRoof)",
                            new List<KeyValuePair<string, string>>
                            {
                                new KeyValuePair<string, string>("$itemName", economyItem.ItemName),
                                new KeyValuePair<string, string>("$itemDescription", economyItem.ItemDescription),
                                new KeyValuePair<string, string>("$itemClassId", ((int)economyItem.ClassOfItem).ToString()),
                                new KeyValuePair<string, string>("$rarityInt", economyItem.RarityInt.ToString()),
                                new KeyValuePair<string, string>("$priceDefault", economyItem.PriceDefault.ToString()),
                                new KeyValuePair<string, string>("$priceFloor", economyItem.PriceFloor.ToString()),
                                new KeyValuePair<string, string>("$priceRoof", economyItem.PriceRoof.ToString())
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
                                    new KeyValuePair<string, string>("$itemName", item.ItemName)
                            })
                            : item.itemId;
                }

                foreach (var economyEvent in economyEventController.economyEvents)
                {
                    string eventName = basicSql.ExecuteScalar(@"SELECT EventName FROM EconomyEvent WHERE EventName = $eventName;",
                        new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("$eventName", economyEvent.EventName)
                        }
                    );

                    if (string.IsNullOrEmpty(eventName))
                    {
                        basicSql.ExecuteNonReader(
                            "INSERT INTO EconomyEvent (EventName, EventDescription) VALUES ($eventName, $eventDescription)",
                            new List<KeyValuePair<string, string>>/*, $itemClassesEffected*/
                            {
                                //todo//new KeyValuePair<string, string>("$itemClassesEffected", economyEvent.eventId),
                                new KeyValuePair<string, string>("$eventName", economyEvent.EventName),
                                new KeyValuePair<string, string>("$eventDescription", economyEvent.EventDescription)
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
                            new KeyValuePair<string, string>("$eventName", economyEvent.EventName)
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
        #endregion SQLite
    }
}
