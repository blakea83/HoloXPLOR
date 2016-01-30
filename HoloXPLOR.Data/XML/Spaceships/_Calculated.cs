﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace HoloXPLOR.Data.XML.Spaceships
{
    public enum CategoryEnum
    {
        __Empty__ = -1,
        __Unknown__ = 0,

        AmmoBox,
        Armor,
        CounterMeasure,
        Missile,
        MissileRack,
        PowerPlant,
        Shield,
        Storage,
        Engine,
        Thruster,
        Turret,
        Weapon,
    }

    public partial class Item
    {
        private String _displayName;
        [XmlIgnore]
        public String DisplayName
        {
            get { return this._displayName = this._displayName ?? this.Params["display_name", this.Name].Replace("itemName_", "").Replace("item_Name", "").Replace("Item_Name", ""); }
            set { this._displayName = value; }
        }

        private Int32? _itemSize;
        public Int32? ItemSize
        {
            get { return this._itemSize = this._itemSize ?? this.Params["itemSize"].ToInt32(); }
            set { this._itemSize = value; }
        }

        [XmlIgnore]
        public CategoryEnum ItemCategory
        {
            get
            {
                switch (String.Format("{0}:{1}", this.Category, this.Class))
                {
                    #region Mount Points
                    case "VehicleWeapon:MannedTurret":     // Requires More Investigation
                    // case "VehicleWeapon:TurretBase":       // Requires More Investigation
                    case "VehicleWeapon:VehicleTurret":    // Requires More Investigation
                        return CategoryEnum.Turret;
                    case "VehicleWeapon:VehicleMissileRack":
                        return CategoryEnum.MissileRack;
                    #endregion
                    #region Ammo/Consumables
                    case "VehicleItem:VehicleItemAmmoBox":
                        return CategoryEnum.AmmoBox;
                    case ":VehicleItemMissile":
                    case "VehicleItem:VehicleItemMissile":
                        return CategoryEnum.Missile;
                    #endregion
                    #region Weapons
                    case "VehicleWeapon:VehicleWeaponEMP":
                    case "VehicleWeapon:VehicleWeapon":
                        return CategoryEnum.Weapon;
                    #endregion
                    #region Shield
                    case "VehicleItem:VehicleItemShield":
                        return CategoryEnum.Shield;
                    #endregion
                    #region Armor
                    case "VehicleItem:VehicleItemArmor":
                        return CategoryEnum.Armor;
                    #endregion
                    #region PowerPlant
                    case "VehicleItem:VehicleItemPowerPlant":
                        return CategoryEnum.PowerPlant;
                    #endregion
                    #region Countermeasures
                    case "VehicleWeapon:VehicleCountermeasureLauncher":
                        return CategoryEnum.CounterMeasure;
                    #endregion
                    #region Thrusters
                    case "VehicleThruster:VehicleItemThruster":
                        if (this.Params["itemType", null] == "MainThruster")
                            return CategoryEnum.Engine;

                        return CategoryEnum.Thruster;
                    #endregion
                    #region Storage
                    case "VehicleItem:VehicleItemContainer":
                        return CategoryEnum.Storage;
                    #endregion
                    #region Vehicle Parts
                    case "VehicleItem:VehicleItemHUDRadarDisplay":
                    case "VehicleItem:VehicleItemQuantumFuelTank":
                    case "VehicleItem:VehicleItemSelfDestruct":
                    case "VehicleItem:VehicleItemTurretAIModule":
                    case "VehicleItem:VehicleItem":
                    case "VehicleItem:VehicleItemLight":
                    case "VehicleItem:VehicleItemCooler":
                    case "VehicleItem:VehicleItemSeat":
                    case "VehicleItem:VehicleItemLandingGearSystem":
                    case "VehicleItem:VehicleItemQDrive":
                    case "VehicleItem:VehicleItemIdentifier":
                    case "VehicleItem:VehicleItemRadar":
                    case "VehicleItem:VehicleItemFuelTank":
                    case "VehicleItem:VehicleItemCockpitAudio":
                    case "VehicleItem:VehicleItemWeaponControl":
                    case "VehicleItem:VehicleItemTargetSelector":
                    case "VehicleItem:VehicleItemMultiLight":
                    case "VehicleItem:VehicleItemFuelIntake":
                        return CategoryEnum.__Unknown__;
                    #endregion
                }

                return CategoryEnum.__Unknown__;
            }
        }
    }
}
