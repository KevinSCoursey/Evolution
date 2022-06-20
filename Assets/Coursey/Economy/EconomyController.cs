using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Economy
{
    public class EconomyController
    {
        private List<Faction> factions;
        public EconomyItemController economyItemController;
        public EconomyEventController economyEventController;

        public EconomyController(List<Faction> factions)
        {
            this.factions = factions;
            economyItemController = new EconomyItemController(factions);
            economyEventController = new EconomyEventController();
            Initialize();
        }
        public void Initialize()
        {
            economyItemController.Initialize();
            economyEventController.Initialize();
        }
    }
}
