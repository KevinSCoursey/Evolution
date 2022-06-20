using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Economy
{
    public class FactionController
    {
        private const bool _debugThisClass = false;


        public List<Faction> factions = new();

        private List<string> factionNames = new List<string>
        { 
            "Humans",
            "The Zerg"
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

                //Just makes this not cause a compile warning
#pragma warning disable CS0162 // Unreachable code detected
                if (_debugThisClass) Debug.Log($"Added the following faction...\n\n{faction}");
#pragma warning restore CS0162 // Unreachable code detected

            }
            foreach(Faction faction in factions)
            {
                faction.UpdateListOfFactions(factions);
            }
        }
        public List<Faction> GetFactionList()
        {
            return factions;
        }
        public void GenerateRandomTradeStations(List<EconomyItem> economyItems)
        {
            foreach (Faction faction in factions)
            {
                faction.GenerateRandomTradeStation(economyItems);
            }
        }
        public Faction GetRandomFaction()
        {
            return factions[MathTools.PseudoRandomInt(0, factions.Count)];
        }
    }
}
