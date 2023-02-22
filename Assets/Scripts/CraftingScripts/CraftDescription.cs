using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CraftDescription : MonoBehaviour
{
    public CraftingManager CM;

    [SerializeField]
    protected TMP_Text Name, Amount, Time, Info, Workbench;


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
        switch (craft.itemID)
        {
            case ItemInfo.ItemID.AK47_Rifle:
                Info.text = "Nice Gun";
                Amount.text = "2";
                Time.text = "10.0";
                break;
            case ItemInfo.ItemID.M1911_Pistol:
                Info.text = "Bad Gun";
                Amount.text = "1";
                Time.text = "20.0";
                break;
        }
        if (!CM.ScreenCraft)
            Workbench.text = "Level " + CM.WorkbenchNeeded;
    }
}
