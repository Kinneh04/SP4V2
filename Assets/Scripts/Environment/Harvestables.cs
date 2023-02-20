using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harvestables : MonoBehaviour
{
    public int ItemAmount = 100;
    public int BaseHarvestAmount = 10;

    public GameObject ItemToGivePlayerOnHarvest;

    public void HarvestAmount(float multiplier)
    {
        InventoryManager II = GameObject.FindGameObjectWithTag("Player").transform.Find("Inventory").GetComponent<InventoryManager>();
        ItemAmount -= (int)(BaseHarvestAmount * multiplier);
        GameObject GO = Instantiate(ItemToGivePlayerOnHarvest);
        II.AddQuantity(GO.GetComponent<ItemInfo>(), (int)(BaseHarvestAmount * multiplier));
        Destroy(GO);
        print("Harvested x" + (int)(BaseHarvestAmount * multiplier));
        if(ItemAmount <= 0)
        {
            Destroy(gameObject);
        }
    }
}
