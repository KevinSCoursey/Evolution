using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Economy
{ 
    public class GameController : MonoBehaviour
    {
        public readonly static int seed = 8675309;
        public FactionController factionController;
        public EconomyController economyController;
        void Start()
        {
            MathTools.random = new(seed);
            Debug.Log($"Seed: {seed}");
            factionController = new FactionController();
            economyController = new EconomyController(factionController.GetFactionList());
            factionController.GenerateRandomTradeStations(economyController.economyItemController.items);
        }
    }
}
