using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadInventory : MonoBehaviour
{
    public int SlotNumber;
    [SerializeField]
    protected GameObject Inventory;
    protected InventoryManager InventoryManager;
    [SerializeField]
    protected TMPro.TMP_Text SlotText;
    void Awake()
    {
        SlotText = this.gameObject.GetComponent<TMPro.TMP_Text>();
        InventoryManager = Inventory.GetComponent<InventoryManager>();
    }
    // Update is called once per frame
    void Update()
    {
        if (InventoryManager.IntGetItem(SlotNumber) != null)
            SlotText.text = InventoryManager.IntGetItem(SlotNumber).GetItemID().ToString();
        else
            SlotText.text = null;
       
        if(InventoryManager.EquippedSlot == SlotNumber)
        {
            gameObject.GetComponentInParent<Image>().color = new Color(0, 0.8f, 1, 0.05f);
        }
        else
        {
            gameObject.GetComponentInParent<Image>().color = new Color(1,1,1,0.05f);
        }
    }
    
}
