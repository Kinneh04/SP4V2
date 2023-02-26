using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CraftDescription : MonoBehaviour
{
    public CraftingManager CM;

    [SerializeField]
    public TMP_Text Name, Amount, Time, Info, tWorkbench;


    private void Awake()
    {
        //CM = GameObject.FindGameObjectWithTag("Crafting").GetComponent<CraftingManager>();
    }

    void Update()
    {
        
    }

    public void ChangeDescription(ItemInfo craft)
    {
        Name.text = craft.itemID.ToString();
        Time.text = "";
        Amount.text = CM.singleAmount.ToString();
        switch (craft.itemID)
        {
            case ItemInfo.ItemID.AK47_Rifle:
                Name.text = "AK47";
                Info.text = "Nice Gun";
                break;
            case ItemInfo.ItemID.M1911_Pistol:
                Name.text = "M1911";
                Info.text = "Bad Gun";
                break;
            case ItemInfo.ItemID.Arrow:
                Name.text = "Arrow";
                Info.text = "Bow Ammo";
                break;
            case ItemInfo.ItemID.Bolt_Action_Rifle:
                Name.text = "Bolt Action Rifle";
                Info.text = "Something?";
                break;
            case ItemInfo.ItemID.Hunting_Bow:
                Name.text = "Bow";
                Info.text = "Traditional Projectile shooter";
                break;
            case ItemInfo.ItemID.Handmade_Shotgun:
                Name.text = "Handmade Shotgun";
                Info.text = "Good close range weapon";
                break;
            case ItemInfo.ItemID.Rocket:
                Name.text = "Rocket";
                Info.text = "Rocket Launcher Ammo";
                break;
            case ItemInfo.ItemID.RocketLauncher:
                Name.text = "Rocket Launcher";
                Info.text = "Uses Rockets to go boom!";
                break;
            case ItemInfo.ItemID.MP5A4:
                Name.text = "MP5A4";
                Info.text = "Fast Low Damage Gun";
                break;
            case ItemInfo.ItemID.PistolAmmo:
                Name.text = "Pistol Ammo";
                Info.text = "Pistol Ammo";
                break;
            case ItemInfo.ItemID.Remington870:
                Name.text = "Remington870";
                Info.text = "Quite the gun";
                break;
            case ItemInfo.ItemID.Revolver:
                Name.text = "Revolver";
                Info.text = "The better pistol";
                break;
            case ItemInfo.ItemID.RifleAmmo:
                Name.text = "Rifle Ammo";
                Info.text = "Rifle Ammo";
                break;
            case ItemInfo.ItemID.ShotgunAmmo:
                Name.text = "Shotgun Ammo";
                Info.text = "Shotgun Ammo";
                break;
            case ItemInfo.ItemID.Building_Blueprint:
                Name.text = "Building Blueprints";
                Info.text = "Used to build structures!";
                break;
            case ItemInfo.ItemID.SniperAmmo:
                Name.text = "Sniper Ammo";
                Info.text = "Sniper Ammo";
                break;
            case ItemInfo.ItemID.ResearchTable_Ghost:
                Name.text = "Research Table";
                Info.text = "Unlock more crafts with this!";
                break;
            case ItemInfo.ItemID.Homemade_Axe:
                Name.text = "Handmade Axe";
                Info.text = "Basic Axe";
                break;
            case ItemInfo.ItemID.Homemade_Pick:
                Name.text = "Handmade Pick";
                Info.text = "Basic Pick";
                break;
            case ItemInfo.ItemID.Metal_Axe:
                Name.text = "Metal Axe";
                Info.text = "A better Axe";
                break;
            case ItemInfo.ItemID.Metal_Pick:
                Name.text = "Metal Pick";
                Info.text = "A better Pick";
                break;
            case ItemInfo.ItemID.Spear:
                Name.text = "Spear";
                Info.text = "Long melee weapon";
                break;
            case ItemInfo.ItemID.Torch:
                Name.text = "Torch";
                Info.text = "Source of Light";
                break;
            case ItemInfo.ItemID.Bandage:
                Name.text = "Bandage";
                Info.text = "The proper healing item";
                break;
            case ItemInfo.ItemID.Ibuprofen:
                Name.text = "Ibuprofen";
                Info.text = "Medicine";
                break;
            case ItemInfo.ItemID.Toilet_Paper:
                Name.text = "Toilet Paper";
                Info.text = "It can heal you? Not really safe";
                break;
            case ItemInfo.ItemID.CampfireGhost:
                Name.text = "Campfire";
                Info.text = "Cooks and lights";
                break;
            case ItemInfo.ItemID.SleepingBagGhost:
                Name.text = "Sleeping Bad";
                Info.text = "Nights have never been better";
                break;
            case ItemInfo.ItemID.CodeLock:
                Name.text = "CodeLock";
                Info.text = "Keeps strangers out";
                break;
            case ItemInfo.ItemID.Workbench_1_Ghost:
                Name.text = "Workbench 1";
                Info.text = "Access to more crafts";
                break;
            case ItemInfo.ItemID.Workbench_2_Ghost:
                Name.text = "Workbench 2";
                Info.text = "Access to more crafts";
                break;
            case ItemInfo.ItemID.Workbench_3_Ghost:
                Name.text = "Workbench 3";
                Info.text = "Access to more crafts";
                break;
            case ItemInfo.ItemID.Hammer:
                Name.text = "Hammer";
                Info.text = "Allows the modification of built structures";
                break;
            case ItemInfo.ItemID.ToolCupboard_Ghost:
                Name.text = "Tool Cupboard";
                Info.text = "Stores your tools";
                break;
            default:
                break;

        }
        if (CM.ScreenCraft)
            tWorkbench.text = "Workbench Level " + CM.WorkbenchNeeded;
    }
}
