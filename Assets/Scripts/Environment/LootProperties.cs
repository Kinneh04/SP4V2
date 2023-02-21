using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LootProperties : MonoBehaviour
{
    public List<ItemInfo> PossibleItemsPool = new List<ItemInfo>();
    public int MaxPossibleItems;
    public int MaxQuantityOfEachItem;
    public List<ItemInfo> ItemsInCrate = new List<ItemInfo>();
    public List<int> PhotonViewIDs = new List<int>();
    public List<int> ItemQuantityInCrate = new List<int>();
    public InventoryManager IM;

    public PhotonView pv;

    public bool forceLoot = false;

    private void Start()
    {
        pv = GetComponent<PhotonView>();
        ChooseRandomLoot();
    }

    bool ItemExistsInCrate(ItemInfo I)
    {
        for(int i =0; i < ItemsInCrate.Count;i++)
        {
            if (I == ItemsInCrate[i])
                return true;
        }
        return false;
    }


    [PunRPC]
    void SyncLootAcrossClients(int[] PhotonViewIDs, int[] ItemQuantity)
    {
        for(int i = 0; i < PhotonViewIDs.Length; i++)
        {
            PhotonView ItemPV = PhotonView.Find(PhotonViewIDs[i]);
            ItemInfo ItemToAdd = ItemPV.gameObject.GetComponent<ItemInfo>();
            if (!ItemExistsInCrate(ItemToAdd))
            {
                ItemsInCrate.Add(ItemToAdd);
                ItemQuantityInCrate.Add(ItemQuantity[i]);
            }
        }
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

        //else
        //{
        //    PhotonViewIDs.Clear();
        //    for(int i = 0; i < ItemsInCrate.Count;i++)
        //    {
        //        PhotonViewIDs.Add(ItemsInCrate[i].GetComponent<PhotonView>().ViewID);
        //    }
        //    int[] PVIDArray = PhotonViewIDs.ToArray();
        //    int[] PVQuanArray = ItemQuantityInCrate.ToArray();
        //    pv.RPC("SyncLootAcrossClients", RpcTarget.Others, PVIDArray, PVQuanArray);
        //}
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

        if (!forceLoot && pv.IsMine)
        {
            for (int i = 0; i < o; i++)
            {
                int y = Random.Range(0, PossibleItemsPool.Count);
                PhotonView GOPV = PhotonNetwork.Instantiate(PossibleItemsPool[y].gameObject.name, transform.position, Quaternion.identity).GetComponent<PhotonView>();
                GOPV.gameObject.SetActive(false);
                GOPV.RPC("ParentToObj", RpcTarget.Others, GOPV.ViewID);
                GOPV.gameObject.transform.parent = GameObject.FindGameObjectWithTag("LootPool").transform;
                ItemsInCrate.Add(GOPV.gameObject.GetComponent<ItemInfo>());
                PhotonViewIDs.Add(GOPV.ViewID);
                int q = Random.Range(1, MaxQuantityOfEachItem);
                ItemQuantityInCrate.Add(q);

            }

            int[] PVIDArray = PhotonViewIDs.ToArray();
            int[] PVQuanArray = ItemQuantityInCrate.ToArray();
            pv.RPC("SyncLootAcrossClients", RpcTarget.Others, PVIDArray, PVQuanArray);
        }
        else if(pv.IsMine)
        {
            int[] PVIDArray = PhotonViewIDs.ToArray();
            int[] PVQuanArray = ItemQuantityInCrate.ToArray();
            pv.RPC("SyncLootAcrossClients", RpcTarget.Others, PVIDArray, PVQuanArray);
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
