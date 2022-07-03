using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Economy
{
    public class Faction
    {
        private const bool _debugThisClass = false;

        public string FactionId;
        public string FactionName = string.Empty;
        public string FactionDescription = string.Empty;
        public List<Faction> FactionAllies = new();
        public List<Faction> FactionEnemies = new();
        public List<TradeStation> TradeStations = new();

        public Faction(string factionName, string factionDescription)
        {
            this.FactionName = string.IsNullOrEmpty(factionName) ? "Unnamed faction" : factionName;
            this.FactionDescription = string.IsNullOrEmpty(factionDescription) ? "No faction description provided." : factionDescription;
        }
        public Faction(SqliteDataReader rowData)
        {
            FactionId = rowData["Id"].ToString();
            FactionName = rowData["Name"].ToString();
            FactionDescription = rowData["Description"].ToString();
        }
        public string GetFactionAllianceNames(bool isFriend)
        {
            string returnString = string.Empty;
            if (isFriend && FactionAllies.Count > 0)
            {
                foreach (var ally in FactionAllies)
                {
                    returnString += $"{ally.FactionName}\n";
                }
            }
            else if (!isFriend && FactionEnemies.Count > 0)
            {
                foreach (var enemy in FactionEnemies)
                {
                    returnString += $"{enemy.FactionName}\n";
                }
            }
            else
            {
                returnString = "None";
            }
            return returnString;
        }
        /*public void GenerateRandomTradeStation()
        {
            int rand = MathTools.PseudoRandomIntExclusiveMax(GameSettings.MinTradeStationsPerFaction, GameSettings.MaxTradeStationsPerFaction);

#pragma warning disable CS0162 // Unreachable code detected
            if (_debugThisClass) Debug.Log($"Adding {rand} Trade Station(s) to the {FactionName} Faction...\n");
#pragma warning restore CS0162 // Unreachable code detected

            for (int i = 0; i < rand; i++)//In this case, 1 - 10 stations generated
            {
                TradeStation tradeStationToAdd = new TradeStation(this, tradeStationName: NameRandomizer.GenerateUniqueNamev2());
                TradeStations.Add(tradeStationToAdd);

                using (var basicSql = new BasicSql())
                {
                    basicSql.ExecuteNonReader(@"
                    UPDATE TradeStation
                    SET FactionId = $factionId, Name = $name, Description = $description
                    WHERE Id = $id;
                    ", new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("$id", tradeStationToAdd.TradeStationId),
                        new KeyValuePair<string, string>("$factionId", tradeStationToAdd.FactionId),
                        new KeyValuePair<string, string>("$name", tradeStationToAdd.TradeStationName),
                        new KeyValuePair<string, string>("$description", tradeStationToAdd.TradeStationDescription)
                    });
                }

#pragma warning disable CS0162 // Unreachable code detected
                    if (_debugThisClass) Debug.Log($"Added a trade station to the {FactionName} Faction. Trade Station data is...\n\n{tradeStationToAdd}");
#pragma warning restore CS0162 // Unreachable code detected

            }
        }*/
        public override string ToString()
        {
            return
                $"Faction Id: {FactionId}\n" +
                $"Faction name: {FactionName}\n" +
                $"Faction description: {FactionDescription}\n" +
                $"Faction allies: {GetFactionAllianceNames(true)}\n" +
                $"Faction enemies: {GetFactionAllianceNames(false)}\n\n";
        }
    }
}