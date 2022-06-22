using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Economy
{
    public class EconomyController
    {
        private FactionController factionController;
        private List<Faction> factions;

        public EconomyItemController economyItemController;
        public EconomyEventController economyEventController;

        public EconomyController(FactionController factionController)
        {
            this.factionController = factionController;
            factions = factionController.GetFactionList();
            economyItemController = new EconomyItemController(factions);
            economyEventController = new EconomyEventController(factionController);
            Initialize();
        }
        public void Initialize()
        {
            economyItemController.Initialize();
            economyEventController.Initialize();
        }
    }
}
