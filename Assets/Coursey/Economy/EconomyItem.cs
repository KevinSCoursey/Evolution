using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mono.Data.Sqlite;

namespace Economy
{
    public class EconomyItem
    {
        private static bool _debugThisClass = false;
        public string EconomyItemName
        {
            get { return _itemName; }
            set { _itemName = string.IsNullOrEmpty(value) ? _itemName = "Unnamed item" : _itemName = value; }
        }
        private string _itemName = string.Empty;
        public string EconomyItemDescription
        {
            get { return _itemDescription; }
            set { _itemDescription = string.IsNullOrEmpty(value) ? "No description provided" : value; }
        }
        private string _itemDescription = string.Empty;
        public List<Faction> FactionsThatSpecializeInThisItem
        {
            get { return _factionsThatSpecializeInThisItem; }
            set
            {
                if (value != null && value.Count == 0) _factionsThatSpecializeInThisItem = new List<Faction>();
                else _factionsThatSpecializeInThisItem = value;
            }
        }
        private List<Faction> _factionsThatSpecializeInThisItem = new();
        public bool IsSpecialized = false;
        public int PurchasePrice
        {
            get { return _purchasePrice; }
            set
            {
                _purchasePrice = Mathf.Clamp(value, PriceFloor, PriceRoof);
            }
        }
        private int _purchasePrice = 1;
        public int SalePrice
        {
            get { return _salePrice; }
            set
            {
                _salePrice = Mathf.Clamp(value, PriceFloor, PriceRoof);
            }
        }
        private int _salePrice = 1;
        public int PriceDefault
        {
            get { return _priceDefault; }
            set { _priceDefault = value; }
        }
        private int _priceDefault = 1;
        public int PriceFloor
        {
            get { return _priceFloor; }
            set
            {
                if (value > PriceRoof) _priceFloor = PriceRoof;
                else _priceFloor = value < 0 ? 0 : value;
            }
        }
        private int _priceFloor = 1;
        public int PriceRoof
        {
            get { return _priceRoof; }
            set
            {
                if (value < PriceFloor) _priceRoof = PriceFloor;
                else _priceRoof = value;
            }
        }
        private int _priceRoof = 1;
        public int RarityInt
        {
            get { return _rarityInt; }
            set
            {
                _rarityInt = (int)Mathf.Clamp(value, 1f, 10f);
            }
        }
        private int _rarityInt = 0;
        public int QuantityOfItem
        {
            get { return _quantityOfItem;  }
            set
            {
                _quantityOfItem = Mathf.Clamp(value, 0, MaxQuantityOfItem);
            }
        }
        private int _quantityOfItem = 1;
        public int MaxQuantityOfItem
        {
            get { return _maxQuantityOfItem; }
            set
            {
                _maxQuantityOfItem = Mathf.Clamp(value, 0, 2147483647);//2147483647
            }
        }
        private int _maxQuantityOfItem = 1;
        public ItemClass EconomyItemClass
        {
            get { return _economyItemClass; }
            set { _economyItemClass = value; }
        }
        private ItemClass _economyItemClass = ItemClass.Unknown;

        public string EconomyItemId;

        public string GetNamesOfFactionsThatSpecializeInThisItem()
        {
            return FactionsThatSpecializeInThisItem == null || FactionsThatSpecializeInThisItem.Count == 0
                ? "None"
                : string.Join("\n", FactionsThatSpecializeInThisItem.Select( x => x.FactionName));
        }
        public void AddFactionSpecialization(Faction faction)
        {
            FactionsThatSpecializeInThisItem.Add(faction);
        }
        public override string ToString()
        {
            return
                $"Item name: {EconomyItemName} ({EconomyItemClass})\n" +
                $"Item description: {EconomyItemDescription}\n" +
                $"Price range of {PriceFloor} - {PriceRoof} (normally {PriceDefault}) with a rarity of {RarityInt}. " +
                $"Factions that specialize in this item: \n{GetNamesOfFactionsThatSpecializeInThisItem()}\n\n";
        }
        public EconomyItem(ItemClass classOfItem, string name, string description, int purchasePrice, int salePrice, int priceDefault, int priceFloor, int priceRoof, int rarityInt, int maxQuantityOfItem)
        {
            EconomyItemClass = classOfItem;
            EconomyItemName = name;
            EconomyItemDescription = description;
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
            EconomyItemClass = classOfItem;
            EconomyItemName = name;
            EconomyItemDescription = description;
            PriceDefault = priceDefault;
            PriceFloor = priceFloor;
            PriceRoof = priceRoof;
            RarityInt = rarityInt;
            MaxQuantityOfItem = maxQuantityOfItem;
        }
        public EconomyItem(EconomyItem economyItem)
        {
            EconomyItemId = economyItem.EconomyItemId;
            IsSpecialized = economyItem.IsSpecialized;
            EconomyItemClass = economyItem.EconomyItemClass;
            EconomyItemName = economyItem.EconomyItemName;
            EconomyItemDescription = economyItem.EconomyItemDescription;
            SalePrice = economyItem.SalePrice;
            PriceDefault = economyItem.PriceDefault;
            PriceFloor = economyItem.PriceFloor;
            PriceRoof = economyItem.PriceRoof;
            PurchasePrice= economyItem.PurchasePrice;
            RarityInt = economyItem.RarityInt;
            FactionsThatSpecializeInThisItem = economyItem.FactionsThatSpecializeInThisItem;
            MaxQuantityOfItem = economyItem.MaxQuantityOfItem;
        }
        public EconomyItem(SqliteDataReader rowData, DataObjectType dataObjectType)
        {
            if(dataObjectType == DataObjectType.EconomyItem)
            {
                BasicSql.DebugRowData(rowData, _debugThisClass);
                EconomyItemId = rowData["EconomyItemId"].ToString();
                EconomyItemName = rowData["EconomyItemName"].ToString();
                EconomyItemDescription = rowData["EconomyItemDescription"].ToString();
                EconomyItemClass = Enum.Parse<ItemClass>(rowData["EconomyItemClassId"].ToString());
                RarityInt = int.Parse(rowData["RarityInt"].ToString());
                PriceDefault = int.Parse(rowData["PriceDefault"].ToString());
                PriceFloor = int.Parse(rowData["PriceFloor"].ToString());
                PriceRoof = int.Parse(rowData["PriceRoof"].ToString());
            }
            if (dataObjectType == DataObjectType.TradeStationInventoryItem)
            {
                BasicSql.DebugRowData(rowData, _debugThisClass);
                EconomyItemId = rowData["EconomyItemId"].ToString();
                EconomyItemName = rowData["EconomyItemName"].ToString();
                QuantityOfItem = int.Parse(rowData["QuantityOfItem"].ToString());
                MaxQuantityOfItem = int.Parse(rowData["MaxQuantityOfItem"].ToString());
                PurchasePrice = int.Parse(rowData["MaxQuantityOfItem"].ToString());
                SalePrice = int.Parse(rowData["MaxQuantityOfItem"].ToString());
                IsSpecialized = rowData["IsSpecialized"].ToString() ==" True";
            }
        }
    }
    public enum ItemClass
    {
        None, Unknown, Generic, Military, Medical, Produce, Construction, Ship
    }
}