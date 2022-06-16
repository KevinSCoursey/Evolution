using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Economy
{
    public class EconomyItem : IEconomyItem
    {
        public string ItemName
        {
            get { return _ItemName; }
            set { _ItemName = value == string.Empty || value == null ? _ItemName = "Unnamed item" : _ItemName = value; }
        }
        private string _ItemName = string.Empty;

        public string Description
        {
            get { return _ItemDescription; }
            set { _ItemDescription = value == string.Empty || value == null ? "No description provided" : value; }
        }
        private string _ItemDescription = string.Empty;

        public List<Faction> FactionsThatSpecializeInThisItem
        {
            get { return _FactionsThatSpecializeInThisItem; }
            set
            {
                if (value != null && value.Count == 0) _FactionsThatSpecializeInThisItem = new List<Faction>();
                else _FactionsThatSpecializeInThisItem = value;
            }
        }
        private List<Faction> _FactionsThatSpecializeInThisItem = new();

        public float PriceVolatilityFactor
        {
            get { return _PriceVolatilityFactor; }
            set { _PriceVolatilityFactor = value; }
        }
        private float _PriceVolatilityFactor = 1.0f;

        public int ActualPrice
        {
            get { return _ActualPrice; }
            set
            {
                if (value > PriceRoof) _ActualPrice = PriceRoof;
                else if (value < PriceFloor) _ActualPrice = PriceFloor;
                else _ActualPrice = value;
            }
        }
        private int _ActualPrice = 1;

        public int PriceDefault
        {
            get { return _PriceDefault; }
            set { _PriceDefault = value; }
        }
        private int _PriceDefault = 1;

        public int PriceFloor
        {
            get { return _PriceFloor; }
            set
            {
                if (value > PriceRoof) _PriceFloor = PriceRoof;
                else _PriceFloor = value < 0 ? 0 : value;
            }
        }
        private int _PriceFloor = 1;

        public int PriceRoof
        {
            get { return _PriceRoof; }
            set
            {
                if (value < PriceFloor) _PriceRoof = PriceFloor;
                else _PriceRoof = value;
            }
        }
        private int _PriceRoof = 1;

        public string GetNamesOfFactionsThatSpecializeInThisItem()
        {
            string returnString = string.Empty;
            if (FactionsThatSpecializeInThisItem == null) return "None";
            if (FactionsThatSpecializeInThisItem.Count > 0)
            {
                foreach (Faction faction in FactionsThatSpecializeInThisItem)
                {
                    returnString += $"{faction.factionName}\n";
                }
            }
            else return "None";
            return returnString;
        }
        public void AddFactionSpecialization(Faction faction)
        {
            FactionsThatSpecializeInThisItem.Add(faction);
        }
        public override string ToString()
        {
            return
                $"Item name: {ItemName}\n" +
                $"Item description: {Description}\n" +
                $"Price range of {PriceFloor} - {PriceRoof} (normally {PriceDefault}) with a volatility factor of {PriceVolatilityFactor}. " +
                $"Factions that specialize in this item: {GetNamesOfFactionsThatSpecializeInThisItem()}\n\n";
        }
        public EconomyItem(string name, string description, float priceVolatilityFactor, int actualPrice, int priceDefault, int priceFloor, int priceRoof)
        {
            ItemName = name;
            Description = description;
            PriceVolatilityFactor = priceVolatilityFactor;
            ActualPrice = actualPrice;
            PriceDefault = priceDefault;
            PriceFloor = priceFloor;
            PriceRoof = priceRoof;
        }
    }
    public interface IEconomyItem
    {
        public string ItemName { get; set; }
        public string Description { get; set; }
        public List<Faction> FactionsThatSpecializeInThisItem { get; set; }
        public float PriceVolatilityFactor { get; set; }
        public int ActualPrice { get; set; }
        public int PriceDefault { get; set; }
        public int PriceFloor { get; set; }
        public int PriceRoof { get; set; }
    }
}