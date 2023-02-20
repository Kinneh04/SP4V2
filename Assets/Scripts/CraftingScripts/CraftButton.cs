using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class CraftButton : MonoBehaviour
{
    CraftingManager CM;
    public Button button;

    private void Awake()
    {
        CM = GameObject.FindGameObjectWithTag("Crafting").GetComponent<CraftingManager>();
    }
    public void Craft()
    {
        if (CM.UpdateCanCraft())
        {
            if (CM.CraftAmount > 0)
                CM.craft();
        }
    }

    public void UpdateCraftButton()
    {
        button.interactable = CM.UpdateCanCraft();
    }
    // Scrolling maybe
    // Tabs


}
