using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mono.Data.Sqlite;

namespace Economy
{
    public class EconomyItem : IEconomyItem
    {
        public string itemName
        {
            get { return _itemName; }
            set { _itemName = string.IsNullOrEmpty(value) ? _itemName = "Unnamed item" : _itemName = value; }
        }
        private string _itemName = string.Empty;

        public string itemDescription
        {
            get { return _itemDescription; }
            set { _itemDescription = string.IsNullOrEmpty(value) ? "No description provided" : value; }
        }
        private string _itemDescription = string.Empty;

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
        public bool IsSpecialized = false;

        public int PurchasePrice
        {
            get { return _PurchasePrice; }
            set
            {
                _PurchasePrice = Mathf.Clamp(value, priceFloor, priceRoof);
            }
        }
        private int _PurchasePrice = 1;
        public int SalePrice
        {
            get { return _SalePrice; }
            set
            {
                _SalePrice = Mathf.Clamp(value, priceFloor, priceRoof);
            }
        }
        private int _SalePrice = 1;

        public int priceDefault
        {
            get { return _PriceDefault; }
            set { _PriceDefault = value; }
        }
        private int _PriceDefault = 1;

        public int priceFloor
        {
            get { return _PriceFloor; }
            set
            {
                if (value > priceRoof) _PriceFloor = priceRoof;
                else _PriceFloor = value < 0 ? 0 : value;
            }
        }
        private int _PriceFloor = 1;

        public int priceRoof
        {
            get { return _PriceRoof; }
            set
            {
                if (value < priceFloor) _PriceRoof = priceFloor;
                else _PriceRoof = value;
            }
        }
        private int _PriceRoof = 1;

        public int rarityInt
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

        public ItemClass itemClass
        {
            get { return _ClassOfItem; }
            set { _ClassOfItem = value; }
        }

        public string itemId;
        private ItemClass _ClassOfItem = ItemClass.Unknown;
        private SqliteDataReader rowDataTS;

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
                $"Item name: {itemName} ({itemClass})\n" +
                $"Item description: {itemDescription}\n" +
                $"Price range of {priceFloor} - {priceRoof} (normally {priceDefault}) with a rarity of {rarityInt}. " +
                $"Factions that specialize in this item: \n{GetNamesOfFactionsThatSpecializeInThisItem()}\n\n";
        }
        public EconomyItem(ItemClass classOfItem, string name, string description, int purchasePrice, int salePrice, int priceDefault, int priceFloor, int priceRoof, int rarityInt, int maxQuantityOfItem)
        {
            itemClass = classOfItem;
            itemName = name;
            itemDescription = description;
            PurchasePrice = purchasePrice;
            SalePrice = salePrice;
            this.priceDefault = priceDefault;
            this.priceFloor = priceFloor;
            this.priceRoof = priceRoof;
            this.rarityInt = rarityInt;
            MaxQuantityOfItem = maxQuantityOfItem;
        }
        public EconomyItem(ItemClass classOfItem, string name, string description, int priceDefault, int priceFloor, int priceRoof, int rarityInt, int maxQuantityOfItem)
        {
            itemClass = classOfItem;
            itemName = name;
            itemDescription = description;
            this.priceDefault = priceDefault;
            this.priceFloor = priceFloor;
            this.priceRoof = priceRoof;
            this.rarityInt = rarityInt;
            MaxQuantityOfItem = maxQuantityOfItem;
        }
        public EconomyItem(EconomyItem economyItem)
        {
            itemId = economyItem.itemId;
            IsSpecialized = economyItem.IsSpecialized;
            itemClass = economyItem.itemClass;
            itemName = economyItem.itemName;
            itemDescription = economyItem.itemDescription;
            SalePrice = economyItem.SalePrice;
            priceDefault = economyItem.priceDefault;
            priceFloor = economyItem.priceFloor;
            priceRoof = economyItem.priceRoof;
            PurchasePrice= economyItem.PurchasePrice;
            rarityInt = economyItem.rarityInt;
            FactionsThatSpecializeInThisItem = economyItem.FactionsThatSpecializeInThisItem;
            MaxQuantityOfItem = economyItem.MaxQuantityOfItem;
        }

        public EconomyItem(SqliteDataReader rowData)
        {
            var fieldCount = rowData.FieldCount;
            for (var currentFieldIdx = 0; currentFieldIdx < fieldCount; currentFieldIdx++)
            {
                Debug.Log(rowData.GetName(currentFieldIdx));
            }
            itemClass = Enum.Parse<ItemClass>(rowData["ItemClassId"].ToString());
            itemId = rowData["Id"].ToString();
            MaxQuantityOfItem = int.Parse(rowData["MaxQuantityOfItem"].ToString());
            PurchasePrice = int.Parse(rowData["PurchasePrice"].ToString());
            SalePrice = int.Parse(rowData["SalePrice"].ToString());
            IsSpecialized = rowData["IsSpecialized"].ToString().Equals("True");
        }
    }
    public interface IEconomyItem
    {
        public string itemName { get; set; }
        public string itemDescription { get; set; }
        public List<Faction> FactionsThatSpecializeInThisItem { get; set; }
        public int PurchasePrice { get; set; }
        public int SalePrice { get; set; }
        public int priceDefault { get; set; }
        public int priceFloor { get; set; }
        public int priceRoof { get; set; }
        public int rarityInt { get; set; }
        public int QuantityOfItem { get; set; }
        public int MaxQuantityOfItem { get; set; }
        public ItemClass itemClass { get; set; }
    }

    public enum ItemClass
    {
        None, Unknown, Generic, Military, Medical, Produce, Construction, Ship
    }
}