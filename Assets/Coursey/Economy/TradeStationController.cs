using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;

namespace Economy
{
    public class TradeStationController
    {
        private const bool _debugThisClass = true;
        public TradeStationController()
        {
            Initialize();
        }//good
        public void Initialize()
        {
            using (new TimedBlock("Generating random trade stations", _debugThisClass))
            {
                GenerateRandomTradeStations();
            }
        }//good
        private void GenerateRandomTradeStations()
        {
            //build trade stations and add to database if they dont exist
            foreach (var faction in FactionController.factions)
            {
                using(new TimedBlock($"Generating trade stations for Faction {faction.FactionName}", _debugThisClass))
                {
                    int numTradeStationsToGenerate = MathTools.PseudoRandomIntExclusiveMax(GameSettings.MinTradeStationsPerFaction, GameSettings.MaxTradeStationsPerFaction);
                    int numTradeStationsPerDBBlock = GameSettings.NumTradeStationsPerDBBlock <= numTradeStationsToGenerate
                        ? GameSettings.NumTradeStationsPerDBBlock
                        : (int)(numTradeStationsToGenerate * 0.5f);
                    int tradeStationCount = 0;
                    List<TradeStation> tradeStationsToAdd = new();
#pragma warning disable CS0162 // Unreachable code detected
                    if (_debugThisClass) Debug.Log($"Adding {numTradeStationsToGenerate} Trade Station(s) to the {faction.FactionName} Faction...\n");
#pragma warning restore CS0162 // Unreachable code detected
                    for (int i = 0; i < numTradeStationsToGenerate; i++)
                    {
                        TradeStation tradeStationToAdd = new TradeStation(faction, tradeStationName: NameRandomizer.GenerateUniqueNamev2());
                        faction.TradeStations.Add(tradeStationToAdd);
                        tradeStationsToAdd.Add(tradeStationToAdd);
#pragma warning disable CS0162 // Unreachable code detected
                        if (_debugThisClass) Debug.Log($"Added a trade station to the {faction.FactionName} Faction. Trade Station data is...\n\n{tradeStationToAdd}");
#pragma warning restore CS0162 // Unreachable code detected
                        tradeStationCount++;
                        if (tradeStationCount >= numTradeStationsPerDBBlock)
                        {
                            DataBaseInteract.UpdateTradeStationData(tradeStationsToAdd);
                            foreach(var tradeStation in tradeStationsToAdd)
                            {
                                tradeStation.Initialize();
                            }
                            DataBaseInteract.UpdateTradeStationInventoryData(tradeStationsToAdd);
                            tradeStationsToAdd.Clear();
                            tradeStationCount = 0;
                        }
                    }
                    if (tradeStationCount != 0)
                    {
                        DataBaseInteract.UpdateTradeStationData(tradeStationsToAdd);
                        foreach (var tradeStation in tradeStationsToAdd)
                        {
                            tradeStation.Initialize();
                        }
                        DataBaseInteract.UpdateTradeStationInventoryData(tradeStationsToAdd);
                        tradeStationsToAdd.Clear();
                        tradeStationCount = 0;
                    }
                }
            }
        }//good
    }
}
