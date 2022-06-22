using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Economy
{
    public class Faction
    {
        private const bool _debugThisClass = true;


        private List<Faction> factions = new();

        public string factionName = string.Empty;
        public string factionDescription = string.Empty;

        public List<Faction> factionAllies = new();
        public List<Faction> factionEnemies = new();
        public List<TradeStation> tradeStations = new();

        public Faction(string factionName, string factionDescription, List<Faction> factionAllies, List<Faction> factionEnemies)
        {
            this.factionName = string.IsNullOrEmpty(factionName) ? "Unnamed faction" : factionName;
            this.factionDescription = factionDescription == string.Empty || factionDescription == null ? "No faction description provided." : factionDescription;
            this.factionAllies = factionAllies;
            this.factionEnemies = factionEnemies;
        }
        public Faction(string factionName, string factionDescription)
        {
            this.factionName = string.IsNullOrEmpty(factionName) ? "Unnamed faction" : factionName;
            this.factionDescription = string.IsNullOrEmpty(factionDescription) ? "No faction description provided." : factionDescription;
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
            int rand = 1;// MathTools.PseudoRandomInt(1, 10);//random.Next(1, 10);

            //Just makes this not cause a compile warning
#pragma warning disable CS0162 // Unreachable code detected
            if (_debugThisClass) Debug.Log($"Adding {rand} Trade Station(s) to the {factionName} Faction...\n");
#pragma warning restore CS0162 // Unreachable code detected

            for (int i = 0; i < rand; i++)//In this case, 1 - 10 stations generated
            {
                TradeStation tradeStationToAdd = new TradeStation(factions, this, economyItems, tradeStationName: NameRandomizer.GenerateUniqueNamev2());
                tradeStations.Add(tradeStationToAdd);

                //Just makes this not cause a compile warning
#pragma warning disable CS0162 // Unreachable code detected
                if (_debugThisClass) Debug.Log($"Added a trade station to the {factionName} Faction. Trade Station data is...\n\n{tradeStationToAdd}");
#pragma warning restore CS0162 // Unreachable code detected

            }
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
