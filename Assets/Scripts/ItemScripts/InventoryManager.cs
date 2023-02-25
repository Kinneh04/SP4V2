using System.Collections;
using System.Collections.Generic;
using UnityEngine;using UnityEngine.UI;
using TMPro;
using System;
using Photon.Pun;
public class InventoryManager : MonoBehaviour
{
    public const int MaxInventorySize = 48;
    public int EquippedSlot = 1;
    
    public List<TMP_Text> InventoryQuantityTexts = new List<TMP_Text>();
    public List<Image> InventoryImages = new List<Image>();


    public List<ItemInfo> InventoryList = new();

    public List<Sprite> itemImages = new List<Sprite>();

    public PlayerProperties pp;
    public PlayerUseItem pui;

    public ItemInfo Rock;
    public ItemInfo Torch;

    private void Start()
    {
        UpdateItemCountPerSlot();
    }

    public void ClearInventory()
    {
        for (int i = 0; i < 30; i++)
        {
            if (InventoryList[i] != null)
            {
                InventoryList[i] = null;
            }
        }
        UpdateItemCountPerSlot();
        if (pp.CurrentlyHoldingItem != null)
        {
            pp.CurrentlyHoldingItem.SetActive(false);
            //Destroy(pp.CurrentlyHoldingItem);
            pp.CurrentlyHoldingItem = null;
        }
    }
    public bool Checkforcraft(ItemInfo Item1, int Item1Quantity = 1, ItemInfo Item2 = null, int Item2Quantity = 1, ItemInfo Item3 = null, int Item3Quantity = 1)
    {
        bool item1Found;
        bool item2Found;
        bool item3Found;
        if (Item1)
        {
            item1Found = CheckForItem(Item1, Item1Quantity);
            if (Item2)
            {
                item2Found = CheckForItem(Item2, Item2Quantity);
                if (Item3)
                {
                    item3Found = CheckForItem(Item3, Item3Quantity);
                }
                else
                    item3Found = true;
            }
            else
            {
                item3Found = true;
                item2Found = true;
            }
        }
        else
        {
            return false;
        }
        
        

        if (item1Found)
        {
            if (item2Found)
            {
                if (item3Found)
                {
                    return true;
                }
                else if (Item3 == null)
                {
                    return true;
                }
                else return false;
            }
            else if (Item2 = null)
            {
                return true;
            }
            else return false;
        }
        else return false;
    }


    public bool CheckForItem(ItemInfo Item, int Quantity)
    {
        for(int i = 0; i < InventoryList.Count; i++)
        {
            if (InventoryList[i])
            {
                Debug.Log(InventoryList[i].GetItemID() + " == " + Item.GetItemID() + ", " + Quantity + " == " + InventoryList[i].GetItemCount());
                if (InventoryList[i].GetItemID() == Item.GetItemID() && Quantity <= InventoryList[i].GetItemCount())
                {
                    return true;
                }
            }

        }
        return false;
    }

    public void Drop(int SlotID)
    {
        pui.DropItem(SlotID);
    }


    public void SwapTwoSlots(int Slot1ID, int Slot2ID)
    {
        //If slots are empty shift item to new slot
        if (InventoryList[Slot1ID] == null && InventoryList[Slot2ID])
        {
            InventoryList[Slot2ID].transform.parent = null;
            InventoryList[Slot1ID] = InventoryList[Slot2ID];
            InventoryList[Slot2ID] = null;
            pui.ForceGiveItem(InventoryList[EquippedSlot]);
            UpdateItemCountPerSlot();

            if (pp.PlayerLookingAtItem && pp.PlayerLookingAtItem.tag == "Campfire")
            {
                pp.PlayerLookingAtItem.GetComponent<FurnaceProperties>().UpdateLoot();
            }
            else if (pp.PlayerLookingAtItem && pp.PlayerLookingAtItem.tag == "Crate")
            {
                pp.PlayerLookingAtItem.GetComponent<LootProperties>().UpdateLoot();
            }

            return;
        }
        else if (InventoryList[Slot1ID] && InventoryList[Slot2ID] == null)
        {
            InventoryList[Slot1ID].transform.parent = null;
            InventoryList[Slot2ID] = InventoryList[Slot1ID];
            InventoryList[Slot1ID] = null;


            pui.ForceGiveItem(InventoryList[EquippedSlot]);

            if (pp.PlayerLookingAtItem && pp.PlayerLookingAtItem.tag == "Campfire")
            {
                pp.PlayerLookingAtItem.GetComponent<FurnaceProperties>().UpdateLoot();
            }
            else if (pp.PlayerLookingAtItem && pp.PlayerLookingAtItem.tag == "Crate")
            {
                pp.PlayerLookingAtItem.GetComponent<LootProperties>().UpdateLoot();
            }
            UpdateItemCountPerSlot();
            return;
        }
        else if (InventoryList[Slot2ID].ItemCount < InventoryList[Slot2ID].MaxItemCount || InventoryList[Slot1ID].itemID != InventoryList[Slot2ID].itemID)
        {
            int Slot1Quantity = InventoryList[Slot1ID].GetItemCount();
            int Slot2Quantity = InventoryList[Slot2ID].GetItemCount();

            ItemInfo Slot1Item = InventoryList[Slot1ID];
            ItemInfo Slot2Item = InventoryList[Slot2ID];
            //if same itemID try to add them to same stack
            if (Slot1Item.itemID == Slot2Item.itemID)
            {
                try
                {
                    InventoryList[Slot2ID].SetItemCount(Slot1Quantity + Slot2Quantity);
                    Remove(Slot1ID);
                }
                catch (Exception e)
                {
                    InventoryList[Slot1ID].SetItemCount(Slot2Quantity);
                    InventoryList[Slot2ID].SetItemCount(Slot1Quantity);

                    InventoryList[Slot1ID] = Slot2Item;
                    InventoryList[Slot2ID] = Slot1Item;
                }
            }
            else //if different item ID, swap position
            {
                ItemInfo Temp = Slot1Item;
                InventoryList[Slot1ID] = Slot2Item;
                InventoryList[Slot2ID] = Temp;
            }
            UpdateItemCountPerSlot();
            IntGetItem(EquippedSlot);

            if (pp.PlayerLookingAtItem != null && pp.PlayerLookingAtItem.tag == "Campfire")
            {
                pp.PlayerLookingAtItem.GetComponent<FurnaceProperties>().UpdateLoot();
            }
        }
    }
    void Awake()
    {
        for(int i = 0; i< MaxInventorySize; ++i)
        {
            InventoryList.Add(null);
        }
        /*
        ItemInfo item = new ItemInfo();
        item.SetItemID(ItemInfo.ItemID.Rock);
        item.SetItemCount(1);
        item.SetMaxItemCount(1);
        item.SetItemType(ItemInfo.ItemType.Pickaxe);
        InventoryList.Add(1, item);*/

        pp = gameObject.GetComponentInParent<PlayerProperties>();
        pui = GetComponentInParent<PlayerUseItem>();
    }
    // Add a new item if a stack of that said item is full or doesnt exist otherwise add to stack
    public void Add( ItemInfo item)
    {
        if (InventoryList.Count <= MaxInventorySize)
        {
            int SlotNum = GetEmptySlot();
            InventoryList[SlotNum] = item;
        }
        if(item.itemType == ItemInfo.ItemType.unshowable)
        {
            item.transform.SetParent(null);
        }
    }

    public void DeleteHoldingItem()
    {
        Destroy(pp.CurrentlyHoldingItem);
        pp.CurrentlyHoldingItem = null;
    }


    public void AddQuantityInSpecifiedBox(ItemInfo item, int QuantityToAdd, int SlotNum)
    {
        print("ATTEMPT TO ADD ITEM AT SPECIFIC AREA: " + item.itemID);
        if (InventoryList.Count <= MaxInventorySize)
        {
            InventoryList[SlotNum] = item;
            InventoryList[SlotNum].SetItemCount(QuantityToAdd);
        }
        UpdateItemCountPerSlot();
    } 
    public bool AddQuantity(ItemInfo item, int QuantityToAdd = 0)
    {
        bool needSetVariable = false;
        print("ATTEMPT TO ADD ITEM: " + item.itemID);
        if (InventoryList.Count <= MaxInventorySize)
        {
            int SlotNum = CheckForAvailableSlots(item, QuantityToAdd);
            if (InventoryList[SlotNum] != null && item.gameObject.tag != "Weaponry" || InventoryList[SlotNum] != null && item.gameObject.tag != "Workbench" || InventoryList[SlotNum] != null && item.gameObject.tag != "Campfire") //Adds quantity
            {
                InventoryList[SlotNum].ItemCount += QuantityToAdd;
                Destroy(item);
            }
            else //creates a new gameobj and adds quantity
            {
                InventoryList[SlotNum] = item;
                InventoryList[SlotNum].ItemCount = QuantityToAdd;

                // Check if current holding slot is the newly added item
                if (pp.CurrentlyHoldingItem == null && EquippedSlot == SlotNum)
                {
                    // If yes, for BuildPlan / Hammer / CodeLock, set corresponding variables
                    needSetVariable = true; // Send true back to PlayerUseItem to perform setting of variables
                }
            }
        }
        UpdateItemCountPerSlot();
        return needSetVariable;
    }

    public void RemoveQuantity(ItemInfo item, int QuantityToRemove = 0)
    {
        print("ATTEMPT TO ADD ITEM: " + item.itemID);
        if (InventoryList.Count > 0)
        {
            int SlotNum = CheckForAvailableSlots(item, QuantityToRemove);
            if (InventoryList[SlotNum] != null) //Removes quantity
            {
                InventoryList[SlotNum].ItemCount -= QuantityToRemove;
                Debug.Log("Path1");
            }
        }
        UpdateItemCountPerSlot();
    }


    public int CheckForAvailableSlots(ItemInfo ItemToCheckFor,int Quantity)
    {
        for(int i = 0; i < InventoryList.Count; i++)
        {
            if( InventoryList[i] != null && InventoryList[i].itemID == ItemToCheckFor.itemID && ItemToCheckFor.gameObject.tag != "Weaponry")// && InventoryList[i].MaxItemCount < ItemToCheckFor.MaxItemCount - Quantity)
            {
               // print("SAME SIDE!");
                return i;
            }
            else if(InventoryList[i] == ItemToCheckFor && InventoryList[i].MaxItemCount < ItemToCheckFor.MaxItemCount)
            {
                AddQuantity(ItemToCheckFor, InventoryList[i].MaxItemCount - InventoryList[i].GetItemCount());
                AddQuantity(ItemToCheckFor, Quantity - (InventoryList[i].MaxItemCount -  InventoryList[i].GetItemCount()));
            }
        }
        return GetEmptySlot();
    }

    public void UpdateItemCountPerSlot()
    {
        for(int i = 0; i < MaxInventorySize; i++)
        {

            if (InventoryList[i] != null)
            {
                if (InventoryList[i].itemType == ItemInfo.ItemType.Ranged)
                {
                    WeaponInfo weapon = InventoryList[i].gameObject.GetComponent<WeaponInfo>();
                    InventoryQuantityTexts[i].text = weapon.GetMagRound().ToString();
                    InventoryImages[i].color = new Color(1, 1, 1, 1);
                    InventoryImages[i].sprite = itemImages[(int)InventoryList[i].GetItemID()];
                }
                else if (InventoryList[i].itemType == ItemInfo.ItemType.Axe || InventoryList[i].itemType == ItemInfo.ItemType.Pickaxe)
                {
                    HarvestToolsProperties hvt = InventoryList[i].gameObject.GetComponent<HarvestToolsProperties>();
                    if (hvt.durability <= 0)
                    {
                        Remove(EquippedSlot);
                        DeleteHoldingItem();
                        //InventoryList[i].ItemCount = 0;
                        //InventoryList[i] = null;
                        InventoryImages[i].color = new Color(1, 1, 1, 1);
                        InventoryImages[i].sprite = null;
                        UpdateItemCountPerSlot();
                    }
                    else
                    {
                        InventoryQuantityTexts[i].text = hvt.durability.ToString();
                        InventoryImages[i].color = new Color(1, 1, 1, 1);
                        InventoryImages[i].sprite = itemImages[(int)InventoryList[i].GetItemID()];
                    }
                }
                else if (InventoryList[i].GetItemCount() > 1)
                {
                    InventoryQuantityTexts[i].text = InventoryList[i].GetItemCount().ToString();
                    InventoryImages[i].color = new Color(1, 1, 1, 1);
                    InventoryImages[i].sprite = itemImages[(int)InventoryList[i].GetItemID()];
                }
                else if (InventoryList[i].GetItemCount() > 0)
                {
                    InventoryQuantityTexts[i].text = "";
                    InventoryImages[i].color = new Color(1, 1, 1, 1);
                    InventoryImages[i].sprite = itemImages[(int)InventoryList[i].GetItemID()];
                }
            }
            else
            {
                InventoryQuantityTexts[i].text = "";
                InventoryImages[i].color = new Color(0, 0, 0, 0);
            }
        }
    }

    //Delete a item
    public void Remove(int SlotToRemove, bool DeleteItem = true)
    {
        if(DeleteItem)
            Destroy(InventoryList[SlotToRemove]);
        InventoryList[SlotToRemove] = null;

        UpdateItemCountPerSlot();
    }

    public void RemoveQuantityFromSlot(int SlotToRemove, int Quantity)
    {
        InventoryList[SlotToRemove].SetItemCount(InventoryList[SlotToRemove].GetItemCount() - Quantity);
        if (InventoryList[SlotToRemove].GetItemCount() <= 0)
        {
            Destroy(InventoryList[SlotToRemove]);
            InventoryList[SlotToRemove] = null;
            DeleteHoldingItem();
        }
        UpdateItemCountPerSlot();
       
    }
    // Remove an quantity of a certain item from inventory; if possible return true else false
    public bool Remove(ItemInfo.ItemID itemID, int Quantity, bool Check)
    {
        List<int> ItemSlotToDeductFrom = new List<int>();
        int QuantityToDeduct = Quantity;   //how much to remove
        int QuantityDeductible = 0;          //how much can be removed
        //Checks which items to deduct from, and checks how much quantity of that item is there
        for (int i = 0; i <  MaxInventorySize; ++i)
        {
            if (InventoryList[i])
            {
                if (InventoryList[i].GetComponent<ItemInfo>().GetItemID() == itemID)
                {
                    ItemSlotToDeductFrom.Add(i);
                    QuantityDeductible += InventoryList[i].GetComponent<ItemInfo>().GetItemCount();
                }
            }
        }
        //if trying to deduct more than possible return false else true
        if(QuantityToDeduct > QuantityDeductible)
        {
            return false;
        }
        else //able to deduct
        {
            //Trying to deduct instead of checking deductible
            if(!Check)
            {
                for (int i = 0; i < ItemSlotToDeductFrom.Count; ++i)
                {
                    if (QuantityToDeduct > 0)
                    {
                        //Delete from inventory if item is lesser or equal than QuantityToDeduct
                        if (InventoryList[ItemSlotToDeductFrom[i]].GetComponent<ItemInfo>().GetItemCount() <= QuantityToDeduct)
                        {
                            QuantityToDeduct -= InventoryList[ItemSlotToDeductFrom[i]].GetComponent<ItemInfo>().GetItemCount();
                            Destroy(InventoryList[ItemSlotToDeductFrom[i]]);
                            InventoryList[ItemSlotToDeductFrom[i]] = null;
                        }
                        else //deduct a certain quantity from item
                        {
                            InventoryList[ItemSlotToDeductFrom[i]].GetComponent<ItemInfo>().SetItemCount(InventoryList[ItemSlotToDeductFrom[i]].GetComponent<ItemInfo>().GetItemCount() - QuantityToDeduct);
                            QuantityToDeduct = 0;
                            break;
                        }
                    }
                }
                return true;
            }
            else
               return true;
        }
    }
    // Get a empty slot
    public int GetEmptySlot()
    {
        for(int i = 0; i < MaxInventorySize; i++)
        {
            if(InventoryList[i] ==null)
            {
                return i;
            }
        }
        return -1;
    }

	// Get an item by its name
    public ItemInfo IntGetItem(int ItemSlot)
    {
        if (InventoryList[ItemSlot])
        {
            return InventoryList[ItemSlot];
        }
        return null;
    }

    public int ItemGetInt(ItemInfo wanted)
    {
        int total = 0;
        for (int i = 0; i < InventoryList.Count; i++)
        {
            if (InventoryList[i])
            {
                if (wanted.itemID == InventoryList[i].itemID)
                {
                    total += InventoryList[i].ItemCount;
                }
            }
        }
        return total;
    }
    public bool CheckAmmoUpdated()
    {
        if(InventoryList[EquippedSlot].GetComponent<WeaponInfo>().GetMagRound().ToString() ==
                    InventoryQuantityTexts[EquippedSlot].text)
        {
            return false;
        }
        return true;
    }
    public bool CheckDurabilityUpdated()
    {
        if (InventoryList[EquippedSlot].GetComponent<HarvestToolsProperties>().durability.ToString() ==
                    InventoryQuantityTexts[EquippedSlot].text)
        {
            return false;
        }
        return true;
    }
    public int GetAmmoQuantity(ItemInfo.ItemID AmmoType)
    {
        int QuantityAvailable = 0;
        //Sums the quantity of that item
        for (int i = 0; i < MaxInventorySize; ++i)
        {
            if (InventoryList[i])
            {
                if (InventoryList[i].GetComponent<ItemInfo>().GetItemID() == AmmoType)
                {
                    QuantityAvailable += InventoryList[i].GetComponent<ItemInfo>().GetItemCount();
                }
            }
        }
        return QuantityAvailable;
    }
}
