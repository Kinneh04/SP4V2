using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    public InventoryManager IM;
    List<ItemInfo> CraftableList = new List<ItemInfo>();

    public CraftDescription description;
    public CraftCost cost;
    public CraftQuantity quantity;
    public CraftButton button;

    public ItemInfo prefab1, prefab2, prefab3, prefab4, prefab5, prefab6, prefab7, prefab8, prefab9, prefab10, prefab11, prefab12, prefab13, prefab14, prefab15, prefab16, prefab17, prefab18, prefab19, prefab20, prefab21, prefab22, prefab23, prefab24, prefab25, prefab26, prefab27, prefab28, prefab29, prefab30;
    public ItemInfo Metal, Sulfur, Wood, Stone, WeaponParts, Cloth, Water;

    public List<CraftSelection> CraftSelections = new List<CraftSelection>();
    public GameObject craftColumn, researchColumn;

    int SelectedCraft;
    bool Craftable;

    public bool ScreenCraft = false;

    public ItemInfo Material_1;
    public int Quantity_1;
    public ItemInfo Material_2;
    public int Quantity_2;
    public ItemInfo Material_3;
    public int Quantity_3;
    public int WorkbenchNeeded;

    public int CraftAmount;
    public int CurrentWorkbenchLv;

    bool made = false;

    private void Awake()
    {
        CraftableList.Add(prefab1);
        CraftableList.Add(prefab2);
        CraftableList.Add(prefab3);
        CraftableList.Add(prefab4);
        CraftableList.Add(prefab5);
        CraftableList.Add(prefab6);
        CraftableList.Add(prefab8);
        CraftableList.Add(prefab9);
        CraftableList.Add(prefab10);
        CraftableList.Add(prefab11);
        CraftableList.Add(prefab12);
        CraftableList.Add(prefab13);
        CraftableList.Add(prefab14);
        CraftableList.Add(prefab15);
        CraftableList.Add(prefab16);
        CraftableList.Add(prefab18);
        CraftableList.Add(prefab19);
        CraftableList.Add(prefab20);
        CraftableList.Add(prefab21);
        CraftableList.Add(prefab22);
        CraftableList.Add(prefab23);

        CraftableList.Add(prefab24);
        CraftableList.Add(prefab25);
        CraftableList.Add(prefab26);
        CraftableList.Add(prefab27);
        CraftableList.Add(prefab28);

        IM = GameObject.FindGameObjectWithTag("Inventory").GetComponent<InventoryManager>();
        //description = FindObjectOfType<CraftDescription>();

        CraftAmount = 0;
    }

    public ItemInfo IntGetItem(int SlotNumber)
    {
        if (CraftableList[SlotNumber] != null)
        {
            return CraftableList[SlotNumber];
        }
        return null;
    }



    public void Selected(int Craft, bool craftable)
    {
        if (Craft != SelectedCraft)
        {
            SelectedCraft = Craft;
            Craftable = craftable;
            if (ScreenCraft)
            {
                if (Craftable)
                    CraftAmount = 1;
                else
                    CraftAmount = 0;

                // Update Costs + Matereials & Quantites
                int[] aNum = new int[3];
                aNum = cost.GetCostInt(CraftableList[SelectedCraft]);
                ItemInfo[] itemInfos = new ItemInfo[3];
                itemInfos = cost.GetCostItem(CraftableList[SelectedCraft]);
                Quantity_1 = aNum[0];
                Quantity_2 = aNum[1];
                Quantity_3 = aNum[2];
                Material_1 = itemInfos[0];
                Material_2 = itemInfos[1];
                Material_3 = itemInfos[2];
                cost.ChangeCost();

                // Update Description
                description.ChangeDescription(CraftableList[SelectedCraft]);

                // Reset Quantity based on can craft

                button.UpdateCraftButton();
                quantity.ChangeAmount();
            }
            else
            {
                // Update Costs + Matereials & Quantites
                int[] aNum = new int[3];
                aNum = cost.GetCostInt(CraftableList[SelectedCraft]);
                ItemInfo[] itemInfos = new ItemInfo[3];
                itemInfos = cost.GetCostItem(CraftableList[SelectedCraft]);
                Quantity_1 = aNum[0];
                Quantity_2 = aNum[1];
                Quantity_3 = aNum[2];
                Material_1 = itemInfos[0];
                Material_2 = itemInfos[1];
                Material_3 = itemInfos[2];
                cost.ChangeCost();

                // Update Description
                description.ChangeDescription(CraftableList[SelectedCraft]);

                // Reset Quantity based on can craft

                button.UpdateCraftButton();
            }
        }
    }

    public ItemInfo GetMaterial(int materialnum)
    {
        switch (CraftableList[SelectedCraft].itemID)
        {
            case ItemInfo.ItemID.AK47_Rifle: // Example
                if (materialnum == 1)
                    return new ItemInfo();
                break;
        }
        return null;
    }

    public int GetMaterialCount(int materialnum)
    {
        switch (CraftableList[SelectedCraft].itemID)
        {
            case ItemInfo.ItemID.AK47_Rifle: // Example
                if (materialnum == 1)
                    return 1;
                break;
            case ItemInfo.ItemID.M1911_Pistol:
                if (materialnum == 1)
                    return 1;
                break;
        }
        return 0;
    }

    public bool canCraft()
    {
        return IM.Checkforcraft(Material_1, Quantity_1, Material_2, Quantity_2, Material_3, Quantity_3);
    }

    public bool UpdateCanCraft()
    {
        if (WorkbenchNeeded > CurrentWorkbenchLv)
            return false;
        return IM.Checkforcraft(Material_1, Quantity_1 * CraftAmount, Material_2, Quantity_2 * CraftAmount, Material_3, Quantity_3 * CraftAmount);
    }

    public void LoadCrafts(int WorkbenchLv, bool ScreenType)
    {
        ScreenCraft = ScreenType;
        quantity.gameObject.SetActive(ScreenCraft);
        if (!made)
        {
            craftColumn.SetActive(true);
            researchColumn.SetActive(false);
            made = true;
            for (int i = 0; i < FindObjectsOfType<CraftSelection>().Length; i++)
            {
                CraftSelections.Add(FindObjectsOfType<CraftSelection>()[i]);
                if (ScreenCraft)
                {
                    ScreenCraft = false;
                    CraftSelections[i].load();
                    if (!Material_1)
                    {
                        Debug.Log("i:" + i);
                        CraftSelections[i].researched = true;
                    }
                    ScreenCraft = true;
                }
            }

        }

        craftColumn.SetActive(true);
        researchColumn.SetActive(false);

        bool desTrue = false;
        CurrentWorkbenchLv = WorkbenchLv;
        int count = CraftableList.Count;
        for (int i = 0; i < count; i++)
        {
            CraftSelection temp = CraftSelections[i];
            temp.gameObject.SetActive(true);
            temp.load();
            if (ScreenCraft)
            {
                Debug.Log("i:" + i + ", " +( CurrentWorkbenchLv < WorkbenchNeeded) + ", " + temp.researched);
                if (CurrentWorkbenchLv < WorkbenchNeeded || !temp.researched)
                {
                    temp.gameObject.SetActive(false);
                }
                else if (!desTrue)
                {
                    desTrue = true;
                    description.ChangeDescription(CraftableList[i]);
                }
            }
            else
            {
                if (CraftSelections[i].researched || !Material_1)
                    temp.gameObject.SetActive(false);
                else if (!desTrue)
                {
                    desTrue = true;
                    description.ChangeDescription(CraftableList[i]);
                }
            }
        }
    }

    public void UpdateAmountCost()
    {
        cost.ChangeCost();
        quantity.ChangeAmount();
        button.UpdateCraftButton();
    }

    public void craft()
    {
        if (Material_1)
        {
            bool b;
            b = IM.Remove(Material_1.itemID, Quantity_1 * CraftAmount, false);
            if (Material_2)
            {
                IM.Remove(Material_2.itemID, Quantity_2 * CraftAmount, false);
                if (Material_3)
                {
                    IM.Remove(Material_3.itemID, Quantity_3 * CraftAmount, false);
                }
            }
        }
        ItemInfo temp = Instantiate<ItemInfo>(CraftableList[SelectedCraft]);
        IM.AddQuantity(temp, CraftAmount);
        CraftAmount = 0;
        //Selected(SelectedCraft, false);
        if (ScreenCraft)
            CraftSelections[SelectedCraft].load();
        else
        {
            CraftSelections[SelectedCraft].researched = true;
            CraftSelections[SelectedCraft].gameObject.SetActive(false);
        }
        IM.UpdateItemCountPerSlot();
    }
}
