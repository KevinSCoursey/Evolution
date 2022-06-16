using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Economy
{
    public class EconomyItemController
    {
        private List<Faction> factions;
        public List<EconomyItem> items = new List<EconomyItem>();

        public EconomyItemController(List<Faction> factions)
        {
            this.factions = factions;
        }

        public void Initialize()
        {
            foreach (Faction faction in factions)
            {
                Debug.Log(faction.ToString());
            }
            AddDefaultEconomyItems();
            LogAllEconomyItems();
        }
        private void AddDefaultEconomyItems()
        {
            //Debug.Log($"Adding default economy items...\n");
            //DEFAULT ITEMS
            EconomyItem itemToAdd = new EconomyItem(
                name: "Iron Ore",
                description: "A rock mostly comprised of Fe-26",
                priceVolatilityFactor: 1.0f,
                actualPrice: 5,
                priceDefault: 5,
                priceFloor: 1,
                priceRoof: 10
                );
            itemToAdd.AddFactionSpecialization(factions[0]); items.Add(itemToAdd);

            itemToAdd = new EconomyItem(
                name: "Gold Ore",
                description: "A rock mostly comprised of Ag-47",
                priceVolatilityFactor: 2.0f,
                actualPrice: 50,
                priceDefault: 50,
                priceFloor: 10,
                priceRoof: 100
                );
            itemToAdd.AddFactionSpecialization(factions[0]); items.Add(itemToAdd);

            itemToAdd = new EconomyItem(
                name: "Copper Ore",
                description: "A rock mostly comprised of Cu-29",
                priceVolatilityFactor: 1.0f,
                actualPrice: 3,
                priceDefault: 3,
                priceFloor: 1,
                priceRoof: 15
                );
            itemToAdd.AddFactionSpecialization(factions[0]); items.Add(itemToAdd);

            itemToAdd = new EconomyItem(
                name: "Nickel Ore",
                description: "A rock mostly comprised of Ni-28",
                priceVolatilityFactor: 0.75f,
                actualPrice: 6,
                priceDefault: 6,
                priceFloor: 1,
                priceRoof: 18
                );
            itemToAdd.AddFactionSpecialization(factions[0]); items.Add(itemToAdd);

            itemToAdd = new EconomyItem(
                name: "Deluxe Canned Space-Bugs (Spicy)",
                description: "Goes well with a side of Tums!",
                priceVolatilityFactor: 1.0f,
                actualPrice: 2,
                priceDefault: 2,
                priceFloor: 1,
                priceRoof: 5
                );
            itemToAdd.AddFactionSpecialization(factions[1]); items.Add(itemToAdd);



        }
        private void LogAllEconomyItems()
        {
            foreach (var item in items)
            {
                Debug.Log($"Added the following item to the economy...\n\n{item}");
            }
        }
    }
}