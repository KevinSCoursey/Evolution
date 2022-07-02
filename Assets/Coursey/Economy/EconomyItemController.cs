using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Economy
{
    public static class EconomyItemController
    {
        private const bool _debugThisClass = true;
        public static bool isLoaded { get; private set; } = false;
        public static List<EconomyItem> items = new List<EconomyItem>();
        public static void Initialize()//good
        {
            using(new TimedBlock("Adding default economy items", _debugThisClass))
            {
                AddDefaultEconomyItems();
                DebugPurposeOnly_RandomlySpecializeItems();
                LogAllEconomyItems();
                DataBaseInteract.UpdateItemData(items);
            }
            isLoaded = true;
        }
        private static void AddDefaultEconomyItems()
        {
            //DEFAULT ITEMS
            EconomyItem itemToAdd = new EconomyItem(
                name: "Iron Ore",
                description: "A rock mostly comprised of Fe-26",
                classOfItem: ItemClass.Generic,
                priceDefault: 500,
                priceFloor: 100,
                priceRoof: 1000,
                rarityInt: 3,
                maxQuantityOfItem: 10000
                );
            items.Add(itemToAdd);

            itemToAdd = new EconomyItem(
                name: "Gold Ore",
                description: "A rock mostly comprised of Ag-47",
                classOfItem: ItemClass.Generic,
                priceDefault: 5000,
                priceFloor: 1000,
                priceRoof: 10000,
                rarityInt: 7,
                maxQuantityOfItem: 10000
                );
            items.Add(itemToAdd);

            itemToAdd = new EconomyItem(
                name: "Copper Ore",
                description: "A rock mostly comprised of Cu-29",
                classOfItem: ItemClass.Generic,
                priceDefault: 300,
                priceFloor: 100,
                priceRoof: 1500,
                rarityInt: 4,
                maxQuantityOfItem: 10000
                );
            items.Add(itemToAdd);

            itemToAdd = new EconomyItem(
                name: "Nickel Ore",
                description: "A rock mostly comprised of Ni-28",
                classOfItem: ItemClass.Generic,
                priceDefault: 600,
                priceFloor: 100,
                priceRoof: 1800,
                rarityInt: 5,
                maxQuantityOfItem: 10000
                );
            items.Add(itemToAdd);

            itemToAdd = new EconomyItem(
                name: "Silver Ore",
                description: "A rock mostly comprised of Ag-47",
                classOfItem: ItemClass.Generic,
                priceDefault: 600,
                priceFloor: 100,
                priceRoof: 1800,
                rarityInt: 5,
                maxQuantityOfItem: 10000
                );
            items.Add(itemToAdd);

            itemToAdd = new EconomyItem(
                name: "Magnesium Ore",
                description: "A rock mostly comprised of Mg-12",
                classOfItem: ItemClass.Generic,
                priceDefault: 600,
                priceFloor: 100,
                priceRoof: 1800,
                rarityInt: 5,
                maxQuantityOfItem: 10000
                );
            items.Add(itemToAdd);

            itemToAdd = new EconomyItem(
                name: "Tin Ore",
                description: "A rock mostly comprised of Sn-50",
                classOfItem: ItemClass.Generic,
                priceDefault: 600,
                priceFloor: 100,
                priceRoof: 1800,
                rarityInt: 5,
                maxQuantityOfItem: 10000
                );
            items.Add(itemToAdd);

            itemToAdd = new EconomyItem(
                name: "Lead Ore",
                description: "A rock mostly comprised of Pb-82",
                classOfItem: ItemClass.Generic,
                priceDefault: 600,
                priceFloor: 100,
                priceRoof: 1800,
                rarityInt: 5,
                maxQuantityOfItem: 10000
                );
            items.Add(itemToAdd);

            itemToAdd = new EconomyItem(
                name: "Sulfur Ore",
                description: "A rock mostly comprised of S-16",
                classOfItem: ItemClass.Generic,
                priceDefault: 600,
                priceFloor: 100,
                priceRoof: 1800,
                rarityInt: 5,
                maxQuantityOfItem: 10000
                );
            items.Add(itemToAdd);

            itemToAdd = new EconomyItem(
                name: "Zinc Ore",
                description: "A rock mostly comprised of Zn-30",
                classOfItem: ItemClass.Generic,
                priceDefault: 600,
                priceFloor: 100,
                priceRoof: 1800,
                rarityInt: 5,
                maxQuantityOfItem: 10000
                );
            items.Add(itemToAdd);

            itemToAdd = new EconomyItem(
                name: "Potassium Ore",
                description: "A rock mostly comprised of K-19",
                classOfItem: ItemClass.Generic,
                priceDefault: 600,
                priceFloor: 100,
                priceRoof: 1800,
                rarityInt: 5,
                maxQuantityOfItem: 10000
                );
            items.Add(itemToAdd);

            itemToAdd = new EconomyItem(
                name: "Cobalt Ore",
                description: "A rock mostly comprised of Co-27",
                classOfItem: ItemClass.Generic,
                priceDefault: 600,
                priceFloor: 100,
                priceRoof: 1800,
                rarityInt: 5,
                maxQuantityOfItem: 10000
                );
            items.Add(itemToAdd);

            itemToAdd = new EconomyItem(
                name: "Osmium Ore",
                description: "A rock mostly comprised of Os-76",
                classOfItem: ItemClass.Generic,
                priceDefault: 600,
                priceFloor: 100,
                priceRoof: 1800,
                rarityInt: 5,
                maxQuantityOfItem: 10000
                );
            items.Add(itemToAdd);

            itemToAdd = new EconomyItem(
                name: "Tungsten Ore",
                description: "A rock mostly comprised of W-74",
                classOfItem: ItemClass.Generic,
                priceDefault: 600,
                priceFloor: 100,
                priceRoof: 1800,
                rarityInt: 5,
                maxQuantityOfItem: 10000
                );
            items.Add(itemToAdd);

            itemToAdd = new EconomyItem(
                name: "Silicon Ore",
                description: "A rock mostly comprised of Si-14",
                classOfItem: ItemClass.Generic,
                priceDefault: 600,
                priceFloor: 100,
                priceRoof: 1800,
                rarityInt: 5,
                maxQuantityOfItem: 10000
                );
           items.Add(itemToAdd);

            itemToAdd = new EconomyItem(
                name: "Bandage Pack",
                description: "A bundle of single-use medical wraps",
                classOfItem: ItemClass.Generic,
                priceDefault: 600,
                priceFloor: 100,
                priceRoof: 1800,
                rarityInt: 5,
                maxQuantityOfItem: 10000
                );
            items.Add(itemToAdd);

            itemToAdd = new EconomyItem(
                name: "Cyborg Mind Core",
                description: "A complicated cybernetic brain",
                classOfItem: ItemClass.Generic,
                priceDefault: 600,
                priceFloor: 100,
                priceRoof: 1800,
                rarityInt: 10,
                maxQuantityOfItem: 10000
                );
            items.Add(itemToAdd);

            itemToAdd = new EconomyItem(
                name: "Generic random item 1 placeholder",
                description: "No description provided",
                classOfItem: ItemClass.Generic,
                priceDefault: 600,
                priceFloor: 100,
                priceRoof: 1800,
                rarityInt: 5,
                maxQuantityOfItem: 10000
                );
            items.Add(itemToAdd);

            itemToAdd = new EconomyItem(
                name: "Generic random item 2 placeholder",
                description: "No description provided",
                classOfItem: ItemClass.Generic,
                priceDefault: 600,
                priceFloor: 100,
                priceRoof: 1800,
                rarityInt: 5,
                maxQuantityOfItem: 10000
                );
            items.Add(itemToAdd);

            itemToAdd = new EconomyItem(
                name: "Generic random item 3 placeholder",
                description: "No description provided",
                classOfItem: ItemClass.Generic,
                priceDefault: 600,
                priceFloor: 100,
                priceRoof: 1800,
                rarityInt: 5,
                maxQuantityOfItem: 10000
                );
            items.Add(itemToAdd);

            itemToAdd = new EconomyItem(
                name: "Generic random item 4 placeholder",
                description: "No description provided",
                classOfItem: ItemClass.Generic,
                priceDefault: 600,
                priceFloor: 100,
                priceRoof: 1800,
                rarityInt: 5,
                maxQuantityOfItem: 10000
                );
            items.Add(itemToAdd);

            itemToAdd = new EconomyItem(
                name: "Generic random item 5 placeholder",
                description: "No description provided",
                classOfItem: ItemClass.Generic,
                priceDefault: 600,
                priceFloor: 100,
                priceRoof: 1800,
                rarityInt: 5,
                maxQuantityOfItem: 10000
                );
            items.Add(itemToAdd);

            itemToAdd = new EconomyItem(
                name: "Generic random item 6 placeholder",
                description: "No description provided",
                classOfItem: ItemClass.Generic,
                priceDefault: 600,
                priceFloor: 100,
                priceRoof: 1800,
                rarityInt: 5,
                maxQuantityOfItem: 10000
                );
            items.Add(itemToAdd);

            itemToAdd = new EconomyItem(
                name: "Generic random item 7 placeholder",
                description: "No description provided",
                classOfItem: ItemClass.Generic,
                priceDefault: 600,
                priceFloor: 100,
                priceRoof: 1800,
                rarityInt: 5,
                maxQuantityOfItem: 10000
                );
            items.Add(itemToAdd);

            itemToAdd = new EconomyItem(
                name: "Deluxe Canned Space-Bugs (Spicy)",
                description: "Goes well with a side of Tums!",
                classOfItem: ItemClass.Generic,
                priceDefault: 200,
                priceFloor: 100,
                priceRoof: 500,
                rarityInt: 1,
                maxQuantityOfItem: 10000
                );
            items.Add(itemToAdd);
        }//good
        private static void LogAllEconomyItems()//good
        {
            if (_debugThisClass)
            {
                foreach (var item in items)
                {
                    Debug.Log($"Added the following item to the economy...\n\n{item}");
                }
            }
        }
        private static void DebugPurposeOnly_RandomlySpecializeItems()//may need to rework how specializations work
        {
            foreach(Faction faction in FactionController.factions)
            {
                int[] itemsToSpecialize = MathTools.GetRandomIndexes(items, MathTools.PseudoRandomIntExclusiveMax(3, items.Count / 2));
                for(int i = 0; i < itemsToSpecialize.Length; i++)
                {
                    if (!items[i].GetNamesOfFactionsThatSpecializeInThisItem().Contains(faction.FactionName))
                    {
                        items[i].AddFactionSpecialization(faction);
                    }
                }
            }
        }
    }
}