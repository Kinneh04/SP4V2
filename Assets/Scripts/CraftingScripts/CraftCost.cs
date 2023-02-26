using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CraftCost : MonoBehaviour
{
    [SerializeField] CraftingManager CM;

    [SerializeField]
    protected TMP_Text
        Amount1, Amount2, Amount3, // Amount
        Item1  , Item2  , Item3  , // Item
        Total1 , Total2 , Total3 , // Total
        Have1  , Have2  , Have3  ; // Have

    int[] materialCounts;
    ItemInfo[] materials;

    void Awake()
    {
        // CM = GameObject.FindGameObjectWithTag("Crafting").GetComponent<CraftingManager>();
        materialCounts = new int[3];
        materials = new ItemInfo[3];
    }


    void Update()
    {
        
    }

    public void ChangeCost()
    {
        string temp;
        if (CM.Material_1)
        {
            Amount1.text = CM.Quantity_1.ToString();
            Item1.text = CM.Material_1.ToString().Substring(0, CM.Material_1.ToString().Length - 10);
            Total1.text = (CM.Quantity_1 * CM.CraftAmount).ToString();
            Have1.text = CM.IM.ItemGetInt(CM.Material_1).ToString();
        }
        else
        {
            Amount1.text = "";
            Item1.text = "";
            Total1.text = "";
            Have1.text = "";
        }
        if (CM.Material_2)
        {
            Amount2.text = CM.Quantity_2.ToString();
            Item2.text = CM.Material_2.ToString().Substring(0, CM.Material_2.ToString().Length - 10);
            Total2.text = (CM.Quantity_2 * CM.CraftAmount).ToString();
            Have2.text = CM.IM.ItemGetInt(CM.Material_2).ToString();
        }
        else
        {
            Amount2.text = "";
            Item2.text = "";
            Total2.text = "";
            Have2.text = "";
        }
        if (CM.Material_3)
        {
            Amount3.text = CM.Quantity_3.ToString();
            Item3.text = CM.Material_3.ToString().Substring(0, CM.Material_3.ToString().Length - 10);
            Total3.text = (CM.Quantity_3 * CM.CraftAmount).ToString();
            Have3.text = CM.IM.ItemGetInt(CM.Material_3).ToString();
        }
        else
        {
            Amount3.text = "";
            Item3.text = "";
            Total3.text = "";
            Have3.text = "";
        }
    }

    public int[] GetCostInt(ItemInfo craft)
    {
        if (CM.ScreenCraft)
        {
            switch (craft.itemID)
            {
                case ItemInfo.ItemID.AK47_Rifle:
                    CM.WorkbenchNeeded = 2;
                    materialCounts[0] = 100;
                    materialCounts[1] = 50;
                    materialCounts[2] = 0;
                    materials[0] = CM.Metal;
                    materials[1] = CM.WeaponParts;
                    materials[2] = null;
                    break;
                case ItemInfo.ItemID.M1911_Pistol:
                    CM.WorkbenchNeeded = 1;
                    materialCounts[0] = 25;
                    materialCounts[1] = 10;
                    materialCounts[2] = 0;
                    materials[0] = CM.Metal;
                    materials[1] = CM.WeaponParts;
                    materials[2] = null;
                    break;
                case ItemInfo.ItemID.Arrow:
                    CM.WorkbenchNeeded = 0;
                    materialCounts[0] = 2;
                    materialCounts[1] = 1;
                    materialCounts[2] = 0;
                    materials[0] = CM.Stone;
                    materials[1] = CM.Wood;
                    materials[2] = null;
                    break;
                case ItemInfo.ItemID.Bolt_Action_Rifle:
                    CM.WorkbenchNeeded = 1;
                    materialCounts[0] = 1;
                    materialCounts[1] = 1;
                    materialCounts[2] = 0;
                    materials[0] = CM.Stone;
                    materials[1] = CM.Wood;
                    materials[2] = null;
                    break;
                case ItemInfo.ItemID.Hunting_Bow: // Bow
                    CM.WorkbenchNeeded = 0;
                    materialCounts[0] = 1;
                    materialCounts[1] = 1;
                    materialCounts[2] = 0;
                    materials[0] = CM.Stone;
                    materials[1] = CM.Wood;
                    materials[2] = null;
                    break;
                case ItemInfo.ItemID.Rocket: // Missile
                    CM.WorkbenchNeeded = 3;
                    materialCounts[0] = 5;
                    materialCounts[1] = 5;
                    materialCounts[2] = 0;
                    materials[0] = CM.Metal;
                    materials[1] = CM.Sulfur;
                    materials[2] = null;
                    break;
                case ItemInfo.ItemID.RocketLauncher:
                    CM.WorkbenchNeeded = 3;
                    materialCounts[0] = 1;
                    materialCounts[1] = 50;
                    materialCounts[2] = 50;
                    materials[0] = CM.Stone;
                    materials[1] = CM.Metal;
                    materials[2] = CM.WeaponParts;
                    break;
                case ItemInfo.ItemID.MP5A4:
                    CM.WorkbenchNeeded = 2;
                    materialCounts[0] = 25;
                    materialCounts[1] = 10;
                    materialCounts[2] = 0;
                    materials[0] = CM.WeaponParts;
                    materials[1] = CM.Metal;
                    materials[2] = null;
                    break;
                case ItemInfo.ItemID.PistolAmmo:
                    CM.WorkbenchNeeded = 1;
                    materialCounts[0] = 1;
                    materialCounts[1] = 1;
                    materialCounts[2] = 0;
                    materials[0] = CM.Metal;
                    materials[1] = CM.Sulfur;
                    materials[2] = null;
                    break;
                case ItemInfo.ItemID.Remington870:
                    CM.WorkbenchNeeded = 1;
                    materialCounts[0] = 20;
                    materialCounts[1] = 10;
                    materialCounts[2] = 0;
                    materials[0] = CM.WeaponParts;
                    materials[1] = CM.Metal;
                    materials[2] = null;
                    break;
                case ItemInfo.ItemID.Revolver:
                    CM.WorkbenchNeeded = 2;
                    materialCounts[0] = 25;
                    materialCounts[1] = 10;
                    materialCounts[2] = 0;
                    materials[0] = CM.WeaponParts;
                    materials[1] = CM.Metal;
                    materials[2] = null;
                    break;
                case ItemInfo.ItemID.RifleAmmo:
                    CM.WorkbenchNeeded = 1;
                    materialCounts[0] = 1;
                    materialCounts[1] = 2;
                    materialCounts[2] = 0;
                    materials[0] = CM.Metal;
                    materials[1] = CM.Sulfur;
                    materials[2] = null;
                    break;
                case ItemInfo.ItemID.ShotgunAmmo:
                    CM.WorkbenchNeeded = 1;
                    materialCounts[0] = 1;
                    materialCounts[1] = 3;
                    materialCounts[2] = 0;
                    materials[0] = CM.Metal;
                    materials[1] = CM.Sulfur;
                    materials[2] = null;
                    break;
                case ItemInfo.ItemID.SmgAmmo:
                    CM.WorkbenchNeeded = 1;
                    materialCounts[0] = 1;
                    materialCounts[1] = 1;
                    materialCounts[2] = 0;
                    materials[0] = CM.Metal;
                    materials[1] = CM.Sulfur;
                    materials[2] = null;
                    break;
                case ItemInfo.ItemID.SniperAmmo:
                    CM.WorkbenchNeeded = 1;
                    materialCounts[0] = 2;
                    materialCounts[1] = 1;
                    materialCounts[2] = 0;
                    materials[0] = CM.Metal;
                    materials[1] = CM.Sulfur;
                    materials[2] = null;
                    break;
                case ItemInfo.ItemID.Homemade_Axe:
                    CM.WorkbenchNeeded = 0;
                    materialCounts[0] = 10;
                    materialCounts[1] = 5;
                    materialCounts[2] = 0;
                    materials[0] = CM.Stone;
                    materials[1] = CM.Wood;
                    materials[2] = null;
                    break;
                case ItemInfo.ItemID.Metal_Axe:
                    CM.WorkbenchNeeded = 1;
                    materialCounts[0] = 10;
                    materialCounts[1] = 5;
                    materialCounts[2] = 0;
                    materials[0] = CM.Metal;
                    materials[1] = CM.Wood;
                    materials[2] = null;
                    break;
                case ItemInfo.ItemID.Homemade_Pick:
                    CM.WorkbenchNeeded = 0;
                    materialCounts[0] = 10;
                    materialCounts[1] = 5;
                    materialCounts[2] = 0;
                    materials[0] = CM.Stone;
                    materials[1] = CM.Wood;
                    materials[2] = null;
                    break;
                case ItemInfo.ItemID.Metal_Pick:
                    CM.WorkbenchNeeded = 1;
                    materialCounts[0] = 10;
                    materialCounts[1] = 5;
                    materialCounts[2] = 0;
                    materials[0] = CM.Metal;
                    materials[1] = CM.Wood;
                    materials[2] = null;
                    break;
                case ItemInfo.ItemID.Spear:
                    CM.WorkbenchNeeded = 0;
                    materialCounts[0] = 10;
                    materialCounts[1] = 5;
                    materialCounts[2] = 0;
                    materials[0] = CM.Metal;
                    materials[1] = CM.Wood;
                    materials[2] = null;
                    break;
                case ItemInfo.ItemID.Torch:
                    CM.WorkbenchNeeded = 0;
                    materialCounts[0] = 5;
                    materialCounts[1] = 0;
                    materialCounts[2] = 0;
                    materials[0] = CM.Wood;
                    materials[1] = null;
                    materials[2] = null;
                    break;
                case ItemInfo.ItemID.Bandage:
                    CM.WorkbenchNeeded = 0;
                    materialCounts[0] = 10;
                    materialCounts[1] = 1;
                    materialCounts[2] = 0;
                    materials[0] = CM.Cloth;
                    materials[1] = CM.Water;
                    materials[2] = null;
                    break;
                case ItemInfo.ItemID.Ibuprofen:
                    CM.WorkbenchNeeded = 1;
                    materialCounts[0] = 10;
                    materialCounts[1] = 0;
                    materialCounts[2] = 0;
                    materials[0] = CM.Water;
                    materials[1] = null;
                    materials[2] = null;
                    break;
                case ItemInfo.ItemID.Toilet_Paper:
                    CM.WorkbenchNeeded = 1;
                    materialCounts[0] = 4;
                    materialCounts[1] = 2;
                    materialCounts[2] = 0;
                    materials[0] = CM.Cloth;
                    materials[1] = CM.Water;
                    materials[2] = null;
                    break;
                case ItemInfo.ItemID.CampfireGhost:
                    CM.WorkbenchNeeded = 0;
                    materialCounts[0] = 10;
                    materialCounts[1] = 0;
                    materialCounts[2] = 0;
                    materials[0] = CM.Wood;
                    materials[1] = null;
                    materials[2] = null;
                    break;
                case ItemInfo.ItemID.SleepingBag:
                    CM.WorkbenchNeeded = 0;
                    materialCounts[0] = 10;
                    materialCounts[1] = 0;
                    materialCounts[2] = 0;
                    materials[0] = CM.Cloth;
                    materials[1] = null;
                    materials[2] = null;
                    break;
                case ItemInfo.ItemID.Workbench_1_Ghost:
                    CM.WorkbenchNeeded = 0;
                    materialCounts[0] = 50;
                    materialCounts[1] = 50;
                    materialCounts[2] = 50;
                    materials[0] = CM.Wood;
                    materials[1] = CM.Stone;
                    materials[2] = CM.Metal;
                    break;
                case ItemInfo.ItemID.Workbench_2_Ghost:
                    CM.WorkbenchNeeded = 1;
                    materialCounts[0] = 300;
                    materialCounts[1] = 300;
                    materialCounts[2] = 300;
                    materials[0] = CM.Metal;
                    materials[1] = CM.Sulfur;
                    materials[2] = CM.Stone;
                    break;
                case ItemInfo.ItemID.Workbench_3_Ghost:
                    CM.WorkbenchNeeded = 2;
                    materialCounts[0] = 1000;
                    materialCounts[1] = 1000;
                    materialCounts[2] = 1000;
                    materials[0] = CM.Wood;
                    materials[1] = CM.Stone;
                    materials[2] = CM.Metal;
                    break;
                case ItemInfo.ItemID.ResearchTable_Ghost:
                    CM.WorkbenchNeeded = 1;
                    materialCounts[0] = 200;
                    materialCounts[1] = 50;
                    materialCounts[2] = 50;
                    materials[0] = CM.Metal;
                    materials[1] = CM.Stone;
                    materials[2] = CM.Wood;
                    break;
                case ItemInfo.ItemID.CodeLock:
                    CM.WorkbenchNeeded = 2;
                    materialCounts[0] = 25;
                    materialCounts[1] = 0;
                    materialCounts[2] = 0;
                    materials[0] = CM.Metal;
                    materials[1] = null;
                    materials[2] = null;
                    break;
                case ItemInfo.ItemID.Building_Blueprint:
                    CM.WorkbenchNeeded = 0;
                    materialCounts[0] = 50;
                    materialCounts[1] = 0;
                    materialCounts[2] = 0;
                    materials[0] = CM.Wood;
                    materials[1] = null;
                    materials[2] = null;
                    break;
                case ItemInfo.ItemID.Hammer:
                    CM.WorkbenchNeeded = 0;
                    materialCounts[0] = 50;
                    materialCounts[1] = 0;
                    materialCounts[2] = 0;
                    materials[0] = CM.Wood;
                    materials[1] = null;
                    materials[2] = null;
                    break;
                case ItemInfo.ItemID.ToolCupboard_Ghost:
                    CM.WorkbenchNeeded = 1;
                    materialCounts[0] = 100;
                    materialCounts[1] = 100;
                    materialCounts[2] = 0;
                    materials[0] = CM.Wood;
                    materials[1] = CM.Metal;
                    materials[2] = null;
                    break;

            }
        }
        else
        {
            switch (craft.itemID)
            {
                case ItemInfo.ItemID.AK47_Rifle:
                    materialCounts[0] = 1;
                    materialCounts[1] = 0;
                    materialCounts[2] = 0;
                    materials[0] = CM.prefab2;
                    materials[1] = null;
                    materials[2] = null;
                    break;
                case ItemInfo.ItemID.Rocket: // Missile
                    materialCounts[0] = 5;
                    materialCounts[1] = 5;
                    materialCounts[2] = 0;
                    materials[0] = CM.Metal;
                    materials[1] = CM.Sulfur;
                    materials[2] = null;
                    break;
                case ItemInfo.ItemID.RocketLauncher:
                    materialCounts[0] = 1;
                    materialCounts[1] = 50;
                    materialCounts[2] = 50;
                    materials[0] = CM.Stone;
                    materials[1] = CM.Metal;
                    materials[2] = CM.WeaponParts;
                    break;
                case ItemInfo.ItemID.PistolAmmo:
                    materialCounts[0] = 1;
                    materialCounts[1] = 1;
                    materialCounts[2] = 0;
                    materials[0] = CM.Metal;
                    materials[1] = CM.Sulfur;
                    materials[2] = null;
                    break;
                case ItemInfo.ItemID.Remington870:
                    materialCounts[0] = 20;
                    materialCounts[1] = 10;
                    materialCounts[2] = 0;
                    materials[0] = CM.WeaponParts;
                    materials[1] = CM.Metal;
                    materials[2] = null;
                    break;
                case ItemInfo.ItemID.Revolver:
                    materialCounts[0] = 25;
                    materialCounts[1] = 10;
                    materialCounts[2] = 0;
                    materials[0] = CM.WeaponParts;
                    materials[1] = CM.Metal;
                    materials[2] = null;
                    break;
                case ItemInfo.ItemID.RifleAmmo:
                    materialCounts[0] = 1;
                    materialCounts[1] = 2;
                    materialCounts[2] = 0;
                    materials[0] = CM.Metal;
                    materials[1] = CM.Sulfur;
                    materials[2] = null;
                    break;
                case ItemInfo.ItemID.ShotgunAmmo:
                    materialCounts[0] = 1;
                    materialCounts[1] = 3;
                    materialCounts[2] = 0;
                    materials[0] = CM.Metal;
                    materials[1] = CM.Sulfur;
                    materials[2] = null;
                    break;
                case ItemInfo.ItemID.SmgAmmo:
                    materialCounts[0] = 1;
                    materialCounts[1] = 1;
                    materialCounts[2] = 0;
                    materials[0] = CM.Metal;
                    materials[1] = CM.Sulfur;
                    materials[2] = null;
                    break;
                case ItemInfo.ItemID.SniperAmmo:
                    materialCounts[0] = 2;
                    materialCounts[1] = 1;
                    materialCounts[2] = 0;
                    materials[0] = CM.Metal;
                    materials[1] = CM.Sulfur;
                    materials[2] = null;
                    break;
                default:
                    materialCounts[0] = 0;
                    materialCounts[1] = 0;
                    materialCounts[2] = 0;
                    materials[0] = null;
                    materials[1] = null;
                    materials[2] = null;
                    break;
            }
        }
        return materialCounts;
    }

    public ItemInfo[] GetCostItem(ItemInfo craft)
    {
        return materials;
    }
}
