using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CraftingMenu : MonoBehaviour
{
    CraftingManager CM;

    private void Start()
    {
        CM = GameObject.FindGameObjectWithTag("Crafting").GetComponent<CraftingManager>();
    }
    public void Craft(ItemInfo item)
    {
        // Get Quantity from the text
        // Check for craftable
        // deduct from inventory
        // add item to inventory
    }


}
