using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadCrafts : MonoBehaviour
{
    public int SlotNumber = 0;
    [SerializeField]
    protected GameObject Crafting;
    protected CraftingManager CM;
    [SerializeField]
    protected TMPro.TMP_Text SlotText;
    InventoryManager IM;
    void Awake()
    {
        SlotText = this.gameObject.GetComponent<TMPro.TMP_Text>();
        CM = Crafting.GetComponent<CraftingManager>();
        IM = GameObject.FindGameObjectWithTag("Inventory").GetComponent<InventoryManager>();
    }

    void Update()
    {
        if (CM.IntGetItem(SlotNumber) != null)
            SlotText.text = CM.IntGetItem(SlotNumber).GetItemID().ToString();
        else
            SlotText.text = null;

    }
}
