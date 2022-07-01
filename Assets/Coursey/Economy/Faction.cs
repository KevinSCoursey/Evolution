using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Economy
{
    public class Faction
    {
        private const bool _debugThisClass = true;

        public string factionId;
        public string factionName = string.Empty;
        public string factionDescription = string.Empty;

        public List<Faction> factionAllies = new();
        public List<Faction> factionEnemies = new();
        public List<TradeStation> tradeStations = new();

        public Faction(string factionName, string factionDescription)
        {
            this.factionName = string.IsNullOrEmpty(factionName) ? "Unnamed faction" : factionName;
            this.factionDescription = string.IsNullOrEmpty(factionDescription) ? "No faction description provided." : factionDescription;
        }

        public Faction(SqliteDataReader rowData)
        {
            factionId = rowData["Id"].ToString();
            factionName = rowData["Name"].ToString();
            factionDescription = rowData["Description"].ToString();
        }

        public string GetFactionAllianceNames(bool isFriend)
        {
            string returnString = string.Empty;
            if (isFriend && factionAllies.Count > 0)
            {
                foreach (var ally in factionAllies)
                {
                    returnString += $"{ally.factionName}\n";
                }
            }
            else if (!isFriend && factionEnemies.Count > 0)
            {
                foreach (var enemy in factionEnemies)
                {
                    returnString += $"{enemy.factionName}\n";
                }
            }
            else
            {
                returnString = "None";
            }
            return returnString;
        }
        public void GenerateRandomTradeStation()
        {
            int rand = MathTools.PseudoRandomIntExclusiveMax(GameSettings.MinTradeStationsPerFaction, GameSettings.MaxTradeStationsPerFaction);

#pragma warning disable CS0162 // Unreachable code detected
            if (_debugThisClass) Debug.Log($"Adding {rand} Trade Station(s) to the {factionName} Faction...\n");
#pragma warning restore CS0162 // Unreachable code detected

            for (int i = 0; i < rand; i++)//In this case, 1 - 10 stations generated
            {
                TradeStation tradeStationToAdd = new TradeStation(this, tradeStationName: NameRandomizer.GenerateUniqueNamev2());
                tradeStations.Add(tradeStationToAdd);

                using (var basicSql = new BasicSql())
                {
                    /*
                    basicSql.ExecuteNonReader(@"
                    INSERT INTO TradeStation (Id INTEGER, FactionId VARCHAR(3), Name VARCHAR(100), Description TEXT)
                    VALUES ($id, $factionId, $name, $description)
                    ON CONFLICT(Id) DO UPDATE SET
                    Id = $id, FactionId = $factionId, Name = $name, Description = $description
                    ",
                     * */
                    basicSql.ExecuteNonReader(@"
                    UPDATE TradeStation
                    SET FactionId = $factionId, Name = $name, Description = $description
                    WHERE Id = $id;
                    ", new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("$id", tradeStationToAdd.tradeStationId),
                        new KeyValuePair<string, string>("$factionId", tradeStationToAdd.factionId),
                        new KeyValuePair<string, string>("$name", tradeStationToAdd.tradeStationName),
                        new KeyValuePair<string, string>("$description", tradeStationToAdd.tradeStationDescription)
                    });
                }

#pragma warning disable CS0162 // Unreachable code detected
                    if (_debugThisClass) Debug.Log($"Added a trade station to the {factionName} Faction. Trade Station data is...\n\n{tradeStationToAdd}");
#pragma warning restore CS0162 // Unreachable code detected

            }
        }
        public override string ToString()
        {
            return
                $"Faction Id: {factionId}\n" +
                $"Faction name: {factionName}\n" +
                $"Faction description: {factionDescription}\n" +
                $"Faction allies: {GetFactionAllianceNames(true)}\n" +
                $"Faction enemies: {GetFactionAllianceNames(false)}\n\n";
        }
    }
}