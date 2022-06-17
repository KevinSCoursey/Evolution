using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Economy
{
    public class EconomyItem : IEconomyItem
    {
        public string ItemName
        {
            get { return _ItemName; }
            set { _ItemName = string.IsNullOrEmpty(value) ? _ItemName = "Unnamed item" : _ItemName = value; }
        }
        private string _ItemName = string.Empty;

        public string Description
        {
            get { return _ItemDescription; }
            set { _ItemDescription = string.IsNullOrEmpty(value) ? "No description provided" : value; }
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
            set
            {
                _PriceVolatilityFactor = Mathf.Clamp(value, 1f, 10f);
            }
        }
        private float _PriceVolatilityFactor = 1.0f;

        public int PurchasePrice
        {
            get { return _PurchasePrice; }
            set
            {
                _PurchasePrice = Mathf.Clamp(value, PriceFloor, PriceRoof);
            }
        }
        private int _PurchasePrice = 1;
        public int SalePrice
        {
            get { return _SalePrice; }
            set
            {
                _SalePrice = Mathf.Clamp(value, PriceFloor, PriceRoof);
            }
        }
        private int _SalePrice = 1;
        public int ActualPrice
        {
            get { return _ActualPrice; }
            set
            {
                _ActualPrice = Mathf.Clamp(value, PriceFloor, PriceRoof);
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

        public int RarityInt
        {
            get { return _RarityInt; }
            set
            {
                _RarityInt = (int)Mathf.Clamp(value, 0f, 10f);
            }
        }
        private int _RarityInt = 0;

        public int QuantityOfItem
        {
            get { return _QuantityOfItem;  }
            set
            {
                _QuantityOfItem = Mathf.Clamp(value, 0, 2147483647);//2147483647
            }
        }
        private int _QuantityOfItem = 1;
        public int MaxQuantityOfItem
        {
            get { return _MaxQuantityOfItem; }
            set
            {
                _MaxQuantityOfItem = Mathf.Clamp(value, 0, 2147483647);//2147483647
            }
        }
        private int _MaxQuantityOfItem = 1;

        public string GetNamesOfFactionsThatSpecializeInThisItem()
        {
            return FactionsThatSpecializeInThisItem == null || FactionsThatSpecializeInThisItem.Count == 0
                ? "None"
                : string.Join("\n", FactionsThatSpecializeInThisItem.Select( x => x.factionName));
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
                $"Price range of {PriceFloor} - {PriceRoof} (normally {PriceDefault}) with a volatility factor of {PriceVolatilityFactor} and rarity of {RarityInt}. " +
                $"Factions that specialize in this item: {GetNamesOfFactionsThatSpecializeInThisItem()}\n\n";
        }
        public EconomyItem(string name, string description, float priceVolatilityFactor, int purchasePrice, int salePrice, int actualPrice, int priceDefault, int priceFloor, int priceRoof, int rarityInt, int maxQuantityOfItem)
        {
            ItemName = name;
            Description = description;
            PriceVolatilityFactor = priceVolatilityFactor;
            PurchasePrice = purchasePrice;
            SalePrice = salePrice;
            ActualPrice = actualPrice;
            PriceDefault = priceDefault;
            PriceFloor = priceFloor;
            PriceRoof = priceRoof;
            RarityInt = rarityInt;
            MaxQuantityOfItem = maxQuantityOfItem;
        }
        public EconomyItem(string name, string description, float priceVolatilityFactor, int actualPrice, int priceDefault, int priceFloor, int priceRoof, int rarityInt, int maxQuantityOfItem)
        {
            ItemName = name;
            Description = description;
            PriceVolatilityFactor = priceVolatilityFactor;
            ActualPrice = actualPrice;
            PriceDefault = priceDefault;
            PriceFloor = priceFloor;
            PriceRoof = priceRoof;
            RarityInt = rarityInt;
            MaxQuantityOfItem = maxQuantityOfItem;
        }
        public EconomyItem(EconomyItem economyItem)
        {
            ItemName = economyItem.ItemName;
            Description = economyItem.Description;
            PriceVolatilityFactor = economyItem.PriceVolatilityFactor;
            SalePrice = economyItem.SalePrice;
            ActualPrice = economyItem.ActualPrice;
            PriceDefault = economyItem.PriceDefault;
            PriceFloor = economyItem.PriceFloor;
            PriceRoof = economyItem.PriceRoof;
            PurchasePrice= economyItem.PurchasePrice;
            RarityInt = economyItem.RarityInt;
            FactionsThatSpecializeInThisItem = economyItem.FactionsThatSpecializeInThisItem;
            MaxQuantityOfItem = economyItem.MaxQuantityOfItem;
        }
    }
    public interface IEconomyItem
    {
        public string ItemName { get; set; }
        public string Description { get; set; }
        public List<Faction> FactionsThatSpecializeInThisItem { get; set; }
        public float PriceVolatilityFactor { get; set; }
        public int PurchasePrice { get; set; }
        public int SalePrice { get; set; }
        public int ActualPrice { get; set; }
        public int PriceDefault { get; set; }
        public int PriceFloor { get; set; }
        public int PriceRoof { get; set; }
        public int RarityInt { get; set; }
        public int QuantityOfItem { get; set; }
        public int MaxQuantityOfItem { get; set; }
    }
}