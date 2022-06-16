using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Economy
{
    public class Faction
    {
        private List<Faction> factions = new();

        public string factionName = string.Empty;
        public string factionDescription = string.Empty;

        public List<Faction> factionAllies = new();
        public List<Faction> factionEnemies = new();
        public List<TradeStation> tradeStations = new();

        public Faction(string factionName, string factionDescription, List<Faction> factionAllies, List<Faction> factionEnemies)
        {
            this.factionName = factionName == string.Empty || factionName == null ? "Unnamed faction" : factionName;
            this.factionDescription = factionDescription == string.Empty || factionDescription == null ? "No faction description provided." : factionDescription;
            this.factionAllies = factionAllies;
            this.factionEnemies = factionEnemies;
        }
        public Faction(string factionName, string factionDescription)
        {
            this.factionName = factionName == string.Empty || factionName == null ? "Unnamed faction" : factionName;
            this.factionDescription = factionDescription == string.Empty || factionDescription == null ? "No faction description provided." : factionDescription;
        }
        /// <summary>
        /// isFriend = true returns allies, false returns enemies
        /// </summary>
        /// <param name="isFriend"></param>
        /// <returns></returns>
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
        public void UpdateListOfFactions(List<Faction> factions)
        {
            this.factions = factions;
        }
        public void GenerateRandomTradeStation(List<EconomyItem> economyItems)
        {
            TradeStation tradeStationToAdd = new TradeStation(factions, this, economyItems);
            tradeStations.Add(tradeStationToAdd);
            Debug.Log($"Added a trade station to the {factionName} Faction. Trade Station data is...\n\n{tradeStationToAdd}");
        }
        public override string ToString()
        {
            return
                $"Faction name: {factionName}\n" +
                $"Faction description: {factionDescription}\n" +
                $"Faction allies: {GetFactionAllianceNames(true)}\n" +
                $"Faction enemies: {GetFactionAllianceNames(false)}\n\n";
        }
    }
}
