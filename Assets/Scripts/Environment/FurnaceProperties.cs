using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class FurnaceProperties : MonoBehaviour
{
    public List<ItemInfo> ItemsInFurnace = new List<ItemInfo>();
    public List<int> ItemQuantityInFurnace = new List<int>();

    public List<int> PhotonViewIDs = new List<int>();
    public InventoryManager IM;
    public bool isOn = false;
    public GameObject FireParticleSystem;
    public GameObject SparksParticleSystem;
    public Light lightsource;
    public ItemInfo ItemRequiredToStartFurnace;
    public ItemInfo RawMeat;
    public ItemInfo CookedMeat;
    public ItemInfo Weaponparts;
    public ItemInfo RawSulfur;
    public ItemInfo CookedSulfur;
    public ItemInfo RawMetal;
    public ItemInfo ScrapMetal;
    public float WoodRemovalInterval;
    public float woodTimer;
    public float CookInterval;
    public float Cooktimer;
    public bool isLookingAtIt;

    public PhotonView pv;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }
    bool ItemExistsInFurnace(ItemInfo I)
    {
        for (int i = 0; i < ItemsInFurnace.Count; i++)
        {
            if (I == ItemsInFurnace[i])
                return true;
        }
        return false;
    }

    bool ItemExistsInCrate(ItemInfo I)
    {
        for (int i = 0; i < ItemsInFurnace.Count; i++)
        {
            if (I == ItemsInFurnace[i])
                return true;
        }
        return false;
    }


    [PunRPC]
    void SyncLootAcrossClients(int[] PhotonViewIDs, int[] ItemQuantity)
    {
        print("CLEARING ITEMS IN CRATE!");
        ItemsInFurnace.Clear();
        ItemQuantityInFurnace.Clear();
        for (int i = 0; i < PhotonViewIDs.Length; i++)
        {
            PhotonView ItemPV = PhotonView.Find(PhotonViewIDs[i]);
            ItemInfo ItemToAdd = ItemPV.gameObject.GetComponent<ItemInfo>();
            if (!ItemExistsInCrate(ItemToAdd))
            {
                ItemsInFurnace.Add(ItemToAdd);
                ItemQuantityInFurnace.Add(ItemQuantity[i]);
            }
        }

        ClearLastLootPool();
        DisplayLoot();
    }


    public void UpdateLoot()
    {
        for (int i = 0; i < 6; i++)
        {

            if (IM.InventoryList[i + 42] != null)
            {
                if (!CheckForItemAndQuantity(IM.InventoryList[i + 42], IM.InventoryList[i + 42].ItemCount))
                {
                    ItemsInFurnace.Add(IM.InventoryList[i + 42]);
                    ItemQuantityInFurnace.Add(IM.InventoryList[i + 42].GetItemCount());
                }
            }
        }
        for (int i = 0; i < ItemsInFurnace.Count; i++)
        {
            if (IM.InventoryList[i + 42] == null)
            {

                ItemsInFurnace.RemoveAt(i);
                ItemQuantityInFurnace.RemoveAt(i);
            }

        }
        PrepareToSyncLoot();
        PhotonViewIDs.Clear();
        ClearLastLootPool();
        DisplayLoot();
       
      
    }

   

    public bool CheckForItemAndQuantity(ItemInfo I, int Q)
    {
        for (int i = 0; i < ItemsInFurnace.Count; i++)
        {
            if (ItemsInFurnace[i].itemID == I.itemID && ItemQuantityInFurnace[i] == Q)
            {
                return true;
            }
        }
        return false;
    }

    public void UpdateInventoryWithNewItems()
    {
        for (int i = 0; i < ItemsInFurnace.Count; i++)
        {
            IM.InventoryList[i+42] = null;

            if(ItemsInFurnace[i] != null)
            {
                if (ItemQuantityInFurnace[i] > 0)
                {
                    IM.InventoryList[i + 42] = ItemsInFurnace[i];
                    IM.InventoryList[i + 42].SetItemCount(ItemQuantityInFurnace[i]);
                }
                else
                {
                    IM.InventoryList[i + 42] = null;
                }
            }

        }
        IM.UpdateItemCountPerSlot();
    }


    public void ClearLastLootPool()
    {
        for (int i = 0; i < 6; i++)
        {
            if (IM.InventoryList[i + 42] != null)
            {
                IM.InventoryList[i + 42].ItemCount = 0;
                IM.InventoryList[i + 42] = null;
            }
        }
        IM.UpdateItemCountPerSlot();
    }

    public void DisplayLoot()
    {
        //ClearLastLootPool();
        for (int i = 0; i < ItemsInFurnace.Count; i++)
        {
            IM.AddQuantityInSpecifiedBox(ItemsInFurnace[i], ItemQuantityInFurnace[i], 42 + i);
            IM.UpdateItemCountPerSlot();
        }
        UpdateInventoryWithNewItems();

    }

    bool CheckForItemInFurnace(ItemInfo ItemToCheckFor)
    {
        for(int i = 0; i < ItemsInFurnace.Count; i++)
        {
            if(ItemsInFurnace[i].itemID == ItemToCheckFor.itemID)
            {
                return true;
            }
        }

        return false;
    }

    public void DecrementItemInFurnace(ItemInfo itemToRemove, int Quantity)
    {
        for (int i = 0; i < ItemsInFurnace.Count; i++)
        {
            if (ItemsInFurnace[i].itemID == itemToRemove.itemID)
            {
                ItemQuantityInFurnace[i] -= Quantity;
                if (ItemQuantityInFurnace[i] < 0)
                {
                    ItemsInFurnace.RemoveAt(i);
                    ItemQuantityInFurnace.RemoveAt(i);
                    if (isLookingAtIt)
                    {

                        UpdateInventoryWithNewItems();
                        UpdateLoot();
                    }
                }
                break;
            }
        }
        if (isLookingAtIt)
        {
            //UpdateLoot();
            UpdateInventoryWithNewItems();
        }
    }

    public void IncrementItemInFurnace(ItemInfo itemToAdd, int Quantity)
    {
        for (int i = 0; i < ItemsInFurnace.Count; i++)
        {
            if (ItemsInFurnace[i].itemID == itemToAdd.itemID)
            {
                ItemQuantityInFurnace[i] += Quantity;
                if (isLookingAtIt)
                {

                    UpdateInventoryWithNewItems();
                    //UpdateLoot();
                }
                return;
            }
        }
        GameObject Additem = PhotonNetwork.Instantiate(itemToAdd.gameObject.name, transform.position, Quaternion.identity);
        ItemsInFurnace.Add(Additem.GetComponent<ItemInfo>());
        ItemQuantityInFurnace.Add(Quantity);
        if (isLookingAtIt)
        {
            
            UpdateInventoryWithNewItems();
            UpdateLoot();
        }
    }

    private void LateUpdate()
    {
        if(isOn)
        {
            woodTimer += Time.deltaTime;
            if(woodTimer >= WoodRemovalInterval)
            {
                woodTimer = 0;
                DecrementItemInFurnace(ItemRequiredToStartFurnace, 1);
                if(!CheckForItemInFurnace(ItemRequiredToStartFurnace))
                {
                    TurnOn();
                    //pv.RPC("TurnOnForOtherClients", RpcTarget.All);
                }
            }

            Cooktimer += Time.deltaTime;
            if(Cooktimer >= CookInterval)
            {
                Cooktimer = 0;
                if(CheckForItemInFurnace(RawMeat) && CheckForItemInFurnace(ItemRequiredToStartFurnace))
                {
                    DecrementItemInFurnace(RawMeat, 1);
                    IncrementItemInFurnace(CookedMeat, 1);
                }

                if (CheckForItemInFurnace(RawSulfur) && CheckForItemInFurnace(ItemRequiredToStartFurnace))
                {
                    DecrementItemInFurnace(RawSulfur, 1);
                    IncrementItemInFurnace(CookedSulfur, 3);
                }

                if (CheckForItemInFurnace(RawMetal) && CheckForItemInFurnace(ItemRequiredToStartFurnace))
                {
                    DecrementItemInFurnace(RawMetal, 1);
                    IncrementItemInFurnace(ScrapMetal, 3);
                }
                if (CheckForItemInFurnace(Weaponparts) && CheckForItemInFurnace(ItemRequiredToStartFurnace))
                {
                    DecrementItemInFurnace(Weaponparts, 1);
                    IncrementItemInFurnace(RawMetal, 50);
                }
                //PrepareToSyncLoot();
            }    
        }
    }

    [PunRPC]

    public void TurnOnForOtherClients()
    {
        if (isOn)
        {
            isOn = false;
            FireParticleSystem.GetComponent<ParticleSystem>().Stop();
            SparksParticleSystem.GetComponent<ParticleSystem>().Stop();
            lightsource.intensity = 0;

        }
        else
        {
            isOn = true;
            FireParticleSystem.GetComponent<ParticleSystem>().Play();
            SparksParticleSystem.GetComponent<ParticleSystem>().Play();
            lightsource.intensity = 6;
        }
    }
    public void TurnOn()
    {

        if (isOn)
        {
            isOn = false;
            FireParticleSystem.GetComponent<ParticleSystem>().Stop();
            SparksParticleSystem.GetComponent<ParticleSystem>().Stop();
            lightsource.intensity = 0;

            pv.RPC("TurnOnForOtherClients", RpcTarget.Others);
        }
        else
        {
            UpdateLoot();
            if (CheckForItemInFurnace(ItemRequiredToStartFurnace))
            {
                
                isOn = true;
                FireParticleSystem.GetComponent<ParticleSystem>().Play();
                SparksParticleSystem.GetComponent<ParticleSystem>().Play();
                lightsource.intensity = 6;
            }
            pv.RPC("TurnOnForOtherClients", RpcTarget.Others);
        }
        
        //for (int i = 0; i < ItemsInFurnace.Count; i++)
        //{
        //    if (ItemsInFurnace[i] != null)
        //        PhotonViewIDs.Add(ItemsInFurnace[i].GetComponent<PhotonView>().ViewID);
        //}
        //int[] PVIDArray = PhotonViewIDs.ToArray();
        //int[] PVQuanArray = ItemQuantityInFurnace.ToArray();
        //pv.RPC("SyncLootAcrossClients", RpcTarget.Others, PVIDArray, PVQuanArray);
    }

    public void PrepareToSyncLoot()
    {
  
            print("SYNCING LOOT ACROSS CLIENTS!");
            pv = GetComponent<PhotonView>();
            PhotonViewIDs.Clear();
            //UpdateLoot();
            for (int i = 0; i < ItemsInFurnace.Count; i++)
            {
                if (ItemsInFurnace[i].gameObject.GetComponent<PhotonView>() != null)
                {


                    PhotonViewIDs.Add(ItemsInFurnace[i].gameObject.GetComponent<PhotonView>().ViewID);
                }
            }
            int[] PVIDArray = PhotonViewIDs.ToArray();
            int[] PVQuanArray = ItemQuantityInFurnace.ToArray();
            pv.RPC("SyncLootAcrossClients", RpcTarget.Others, PVIDArray, PVQuanArray);
    }
}
