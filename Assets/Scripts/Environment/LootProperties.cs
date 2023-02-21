using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootProperties : MonoBehaviour
{
    public List<ItemInfo> PossibleItemsPool = new List<ItemInfo>();
    public int MaxPossibleItems;
    public int MaxQuantityOfEachItem;
    public List<ItemInfo> ItemsInCrate = new List<ItemInfo>();
    public List<int> ItemQuantityInCrate = new List<int>();
    public InventoryManager IM;

    public bool forceLoot = false;

    private void Start()
    {
       
        ChooseRandomLoot();
    }

    public void UpdateLoot()
    {
        for(int i = 0; i < ItemQuantityInCrate.Count;i++)
        {
            if(IM.InventoryList[i+30] == null)
            {

                ItemsInCrate.RemoveAt(i);
                ItemQuantityInCrate.RemoveAt(i);
                i--;

            }
        }

        //for (int i = 0; i < 12; i++)
        //{
        //    if (ItemsInCrate[i] == null)
        //    {
        //        ItemsInCrate.RemoveAt(i);
        //        ItemQuantityInCrate.RemoveAt(i);
        //    }
        //}



            if (ItemsInCrate.Count <= 0) Destroy(gameObject);   
    }

    public void ClearLastLootPool()
    {
        for (int i = 0; i < 12; i++)
        {
            if (IM.InventoryList[i + 30] != null)
            {
                IM.InventoryList[i + 30].ItemCount = 0;
                IM.InventoryList[i + 30] = null;
                IM.Remove(i);
            }
        }
        IM.UpdateItemCountPerSlot();
    }
    public void ChooseRandomLoot()
    {
        int o = Random.Range(1, MaxPossibleItems);

        if (!forceLoot)
        {
            for (int i = 0; i < o; i++)
            {
                int y = Random.Range(0, PossibleItemsPool.Count);
                GameObject GO = Instantiate(PossibleItemsPool[y].gameObject);
                GO.SetActive(false);
                GO.transform.parent = GameObject.FindGameObjectWithTag("LootPool").transform;
                ItemsInCrate.Add(GO.GetComponent<ItemInfo>());
                int q = Random.Range(1, MaxQuantityOfEachItem);
                ItemQuantityInCrate.Add(q);

            }
        }
    }

    public void DisplayLoot()
    {
        ClearLastLootPool();
        for(int i = 0; i < ItemsInCrate.Count; i++)
        {
                IM.AddQuantityInSpecifiedBox(ItemsInCrate[i], ItemQuantityInCrate[i], 30 + i);
        }
        
    }
}
