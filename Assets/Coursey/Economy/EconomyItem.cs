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

        public string ItemDescription
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
                _RarityInt = (int)Mathf.Clamp(value, 1f, 10f);
            }
        }
        private int _RarityInt = 0;

        public int QuantityOfItem
        {
            get { return _QuantityOfItem;  }
            set
            {
                _QuantityOfItem = Mathf.Clamp(value, 0, MaxQuantityOfItem);
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

        public ItemClass ClassOfItem
        {
            get { return _ClassOfItem; }
            set { _ClassOfItem = value; }
        }

        public string itemId;
        private ItemClass _ClassOfItem = ItemClass.Unknown;

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
                $"Item name: {ItemName} ({ClassOfItem})\n" +
                $"Item description: {ItemDescription}\n" +
                $"Price range of {PriceFloor} - {PriceRoof} (normally {PriceDefault}) with a rarity of {RarityInt}. " +
                $"Factions that specialize in this item: \n{GetNamesOfFactionsThatSpecializeInThisItem()}\n\n";
        }
        public EconomyItem(ItemClass classOfItem, string name, string description, int purchasePrice, int salePrice, int priceDefault, int priceFloor, int priceRoof, int rarityInt, int maxQuantityOfItem)
        {
            ClassOfItem = classOfItem;
            ItemName = name;
            ItemDescription = description;
            PurchasePrice = purchasePrice;
            SalePrice = salePrice;
            PriceDefault = priceDefault;
            PriceFloor = priceFloor;
            PriceRoof = priceRoof;
            RarityInt = rarityInt;
            MaxQuantityOfItem = maxQuantityOfItem;
        }
        public EconomyItem(ItemClass classOfItem, string name, string description, int priceDefault, int priceFloor, int priceRoof, int rarityInt, int maxQuantityOfItem)
        {
            ClassOfItem = classOfItem;
            ItemName = name;
            ItemDescription = description;
            PriceDefault = priceDefault;
            PriceFloor = priceFloor;
            PriceRoof = priceRoof;
            RarityInt = rarityInt;
            MaxQuantityOfItem = maxQuantityOfItem;
        }
        public EconomyItem(EconomyItem economyItem)
        {
            itemId = economyItem.itemId;
            ClassOfItem = economyItem.ClassOfItem;
            ItemName = economyItem.ItemName;
            ItemDescription = economyItem.ItemDescription;
            SalePrice = economyItem.SalePrice;
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
        public string ItemDescription { get; set; }
        public List<Faction> FactionsThatSpecializeInThisItem { get; set; }
        public int PurchasePrice { get; set; }
        public int SalePrice { get; set; }
        public int PriceDefault { get; set; }
        public int PriceFloor { get; set; }
        public int PriceRoof { get; set; }
        public int RarityInt { get; set; }
        public int QuantityOfItem { get; set; }
        public int MaxQuantityOfItem { get; set; }
        public ItemClass ClassOfItem { get; set; }
    }

    public enum ItemClass
    {
        None, Unknown, Generic, Military, Medical, Produce, Construction, Ship
    }
}