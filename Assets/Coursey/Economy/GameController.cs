using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Economy
{ 
    public class GameController : MonoBehaviour
    {
        public FactionController factionController;
        public EconomyController economyController;
        void Start()
        {
            factionController = new FactionController();
            factionController.Initialize();
            economyController = new EconomyController(factionController.GetFactionList());
            economyController.Initialize();
            factionController.GenerateRandomTradeStations(economyController.economyItemController.items);
        }
    }
}
