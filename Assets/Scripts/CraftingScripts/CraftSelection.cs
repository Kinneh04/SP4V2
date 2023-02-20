using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftSelection : MonoBehaviour
{
    CraftingManager CM;
    bool craftable = false;
    public bool researched = false;
    int slot;

    private void Awake()
    {
        CM = GameObject.FindGameObjectWithTag("Crafting").GetComponent<CraftingManager>();
        slot = GetComponentInChildren<ReadCrafts>().SlotNumber;
    }

    public void SelectCraft()
    {
        CM.Selected(slot, craftable);
    }

    private void Update()
    {
        
    }

    public void load()
    {
        CM.Selected(slot, craftable);
        craftable = CM.canCraft();
        if (!craftable)
        {
            gameObject.GetComponent<Image>().color = new Color(1f, 0.5f, 0.5f, 0.05f);
        }
        else
        {
            gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.05f);
        }
    }
}
