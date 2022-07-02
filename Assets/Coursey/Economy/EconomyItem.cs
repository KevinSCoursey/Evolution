using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mono.Data.Sqlite;

namespace Economy
{
    public class EconomyItem //: IEconomyItem
    {
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
            get { return _quantityOfItem;  }
            set
            {
                _quantityOfItem = Mathf.Clamp(value, 0, MaxQuantityOfItem);
            }
        }
        private int _quantityOfItem = 1;
        public int MaxQuantityOfItem
        {
            get { return _MaxQuantityOfItem; }
            set
            {
                _MaxQuantityOfItem = Mathf.Clamp(value, 0, 2147483647);//2147483647
            }
        }
        private int _MaxQuantityOfItem = 1;

        public ItemClass EconomyItemClass
        {
            get { return _ClassOfItem; }
            set { _ClassOfItem = value; }
        }

        public string EconomyItemId;
        private ItemClass _ClassOfItem = ItemClass.Unknown;
        private SqliteDataReader rowDataTS;

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
            this.PurchasePrice = purchasePrice;
            this.SalePrice = salePrice;
            this.PriceDefault = priceDefault;
            this.PriceFloor = priceFloor;
            this.PriceRoof = priceRoof;
            this.RarityInt = rarityInt;
            this.MaxQuantityOfItem = maxQuantityOfItem;
        }
        public EconomyItem(ItemClass classOfItem, string name, string description, int priceDefault, int priceFloor, int priceRoof, int rarityInt, int maxQuantityOfItem)
        {
            EconomyItemClass = classOfItem;
            EconomyItemName = name;
            EconomyItemDescription = description;
            this.PriceDefault = priceDefault;
            this.PriceFloor = priceFloor;
            this.PriceRoof = priceRoof;
            this.RarityInt = rarityInt;
            this.MaxQuantityOfItem = maxQuantityOfItem;
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

        public EconomyItem(SqliteDataReader rowData, DataObjectType dataObjectType)//two different row datas being passed to this!!!
        {
            if(dataObjectType == DataObjectType.EconomyItem)
            {
                /*var fieldCount = rowData.FieldCount;
                string debug = "";
                for (var currentFieldIdx = 0; currentFieldIdx < fieldCount; currentFieldIdx++)
                {
                    debug += $"{rowData.GetName(currentFieldIdx)} - {rowData[currentFieldIdx].ToString()}\n";
                }
                Debug.Log(debug);*/
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
                var fieldCount = rowData.FieldCount;
                string debug = "";
                for (var currentFieldIdx = 0; currentFieldIdx < fieldCount; currentFieldIdx++)
                {
                    debug += $"{rowData.GetName(currentFieldIdx)} - {rowData[currentFieldIdx].ToString()}\n";
                }
                Debug.Log($"testtesttest---{debug}");
                EconomyItemId = rowData["EconomyItemId"].ToString();
                EconomyItemName = rowData["EconomyItemName"].ToString();
                QuantityOfItem = int.Parse(rowData["QuantityOfItem"].ToString());
                MaxQuantityOfItem = int.Parse(rowData["MaxQuantityOfItem"].ToString());
                PurchasePrice = int.Parse(rowData["MaxQuantityOfItem"].ToString());
                SalePrice = int.Parse(rowData["MaxQuantityOfItem"].ToString());
                IsSpecialized = bool.Parse(rowData["IsSpecialized"].ToString());
            }
        }
    }
    /*public interface IEconomyItem
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
    }*/

    public enum ItemClass
    {
        None, Unknown, Generic, Military, Medical, Produce, Construction, Ship
    }
}