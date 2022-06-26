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
        public static List<TradeRoute> AllTradeRoutes = new List<TradeRoute>();

        public EconomyController(FactionController factionController)
        {
            this.factionController = factionController;
            factions = factionController.GetFactionList();
            economyItemController = new EconomyItemController(factions);
            economyEventController = new EconomyEventController(factionController);
            Initialize();
        }

        public static bool DoesTradeRouteExist(TradeRoute tradeRoute)
        {
            if (AllTradeRoutes.Count == 0 || tradeRoute == null) return false;
            foreach(var tradeRouteParse in AllTradeRoutes)
            {
                if(tradeRouteParse.Equals(tradeRoute)) return true;
            }
            return false;
        }
        
        public void Initialize()
        {
            economyItemController.Initialize();
            economyEventController.Initialize();
        }
    }
}
