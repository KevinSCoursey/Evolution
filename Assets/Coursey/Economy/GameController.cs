using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Economy
{ 
    public class GameController : MonoBehaviour
    {
        private const bool _debugThisClass = true;


        public readonly static int seed = 8675309;
        public FactionController factionController;
        public EconomyController economyController;
        void Start()
        {
            MathTools.random = new(seed);
            
            //Just makes this not cause a compile warning
#pragma warning disable CS0162 // Unreachable code detected
            if (_debugThisClass) Debug.Log($"Seed: {seed}");
#pragma warning restore CS0162 // Unreachable code detected

            factionController = new FactionController();
            economyController = new EconomyController(factionController.GetFactionList());
            factionController.GenerateRandomTradeStations(economyController.economyItemController.items);
        }
    }
}
