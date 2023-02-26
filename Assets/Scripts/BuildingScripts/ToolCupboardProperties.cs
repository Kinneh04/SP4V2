using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ToolCupboardProperties : MonoBehaviour
{
    public List<PlayerProperties> playersWithBuildingPrivilege = new List<PlayerProperties>();
    public List<StructureObject> structuresInRange = new List<StructureObject>();

    public AudioManager audioManager;

    public PhotonView pv;
    public List<ItemInfo> ItemsInTC = new List<ItemInfo>();
    public List<int> ItemQuantityInTC = new List<int>();
    public List<int> PhotonViewIDs = new List<int>();
    public InventoryManager IM;

    public bool hasLock;
    public GameObject lockObject;

    public int woodReq = 0;
    public int stoneReq = 0;
    public bool isProtected = false;

    public float protectionTimeLeft = 0.0f;
    public float resourceCollectElapsed = 0.0f;
    public bool isLookingAtIt;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playersWithBuildingPrivilege.Contains(other.GetComponent<PlayerProperties>()))
            {
                other.GetComponent<PlayerProperties>().hasBuildingPrivilege = true;
                other.GetComponent<PlayerProperties>().BuildingPrivilegeIcon.SetActive(true);
            }
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("BuildableParent"))
        {
            structuresInRange.Add(other.gameObject.GetComponent<StructureObject>());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playersWithBuildingPrivilege.Contains(other.GetComponent<PlayerProperties>()))
            {
                other.GetComponent<PlayerProperties>().hasBuildingPrivilege = false;
                other.GetComponent<PlayerProperties>().BuildingPrivilegeIcon.SetActive(false);
            }
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("BuildableParent"))
        {
            structuresInRange.Remove(other.gameObject.GetComponent<StructureObject>());
        }
    }

    private void LateUpdate()
    {
        UpdateResourceReq();

        if (resourceCollectElapsed > 0.0f)
        {
            resourceCollectElapsed -= Time.deltaTime;
        }
        else // Collect resources
        {
            int woodLeft = woodReq;
            int stoneLeft = stoneReq;

            while (woodLeft > 0)
            {
                int woodIndex = ItemsInTC.FindIndex(i => i.itemID == ItemInfo.ItemID.Wood);
                if (woodIndex == -1 && woodLeft > 0) // No wood found, means insufficient!
                {
                    protectionTimeLeft = 0.0f;
                    isProtected = false;
                    return;
                }

                if (ItemQuantityInTC[woodIndex] > woodLeft)
                {
                    ItemQuantityInTC[woodIndex] -= woodLeft;
                    woodLeft = 0;
                }
                else
                {
                    woodLeft -= ItemQuantityInTC[woodIndex];
                    ItemsInTC.RemoveAt(woodIndex);
                    ItemQuantityInTC.RemoveAt(woodIndex);
                    PhotonViewIDs.RemoveAt(woodIndex);
                }
            }

            while (stoneLeft > 0)
            {
                int stoneIndex = ItemsInTC.FindIndex(i => i.itemID == ItemInfo.ItemID.Stone);
                if (stoneIndex == -1 && stoneLeft > 0) // No stone found, means insufficient!
                {
                    protectionTimeLeft = 0.0f;
                    isProtected = false;
                    return;
                }

                if (ItemQuantityInTC[stoneIndex] > stoneLeft)
                {
                    ItemQuantityInTC[stoneIndex] -= stoneLeft;
                    stoneLeft = 0;
                }
                else
                {
                    stoneLeft -= ItemQuantityInTC[stoneIndex];
                    ItemsInTC.RemoveAt(stoneIndex);
                    ItemQuantityInTC.RemoveAt(stoneIndex);
                    PhotonViewIDs.RemoveAt(stoneIndex);
                }
            }

            if (isLookingAtIt)
            {
                UpdateInventoryWithNewItems();
                UpdateLoot();
            }

            int totalWood = 0;
            int totalStone = 0;

            for (int i = 0; i < ItemsInTC.Count; i++)
            {
                if (ItemsInTC[i].itemID == ItemInfo.ItemID.Wood)
                    totalWood += ItemQuantityInTC[i];
                else if (ItemsInTC[i].itemID == ItemInfo.ItemID.Stone)
                    totalStone += ItemQuantityInTC[i];
            }
            float woodTimeLeft = totalWood != 0 ? totalWood / woodReq : 0;
            float stoneTimeLeft = totalStone != 0 ? totalStone / stoneReq : 0;
            if (stoneReq == 0 && woodReq == 0)
            {
                // dont do anything when both are 0
            }
            else if (stoneReq == 0)
                stoneTimeLeft = float.PositiveInfinity;
            else if (woodReq == 0)
                woodTimeLeft = float.PositiveInfinity;

            if (woodTimeLeft < stoneTimeLeft)
                protectionTimeLeft = woodTimeLeft * 60;
            else
                protectionTimeLeft = stoneTimeLeft * 60;
            isProtected = true;
            resourceCollectElapsed = 10.0f;
        }

        if (protectionTimeLeft > 0.0f)
        {
            protectionTimeLeft -= Time.deltaTime;
        }
        else
        {
            isProtected = false;
        }

        if (isProtected)
        {
            foreach (StructureObject structure in structuresInRange)
            {
                structure.isDecaying = false;
            }
        }
    }

    private void UpdateResourceReq()
    {
        woodReq = 0;
        stoneReq = 0;
        foreach (StructureObject structure in structuresInRange)
        {
            if (structure.isUpgraded)
            {
                stoneReq += 10;
            }
            else
            {
                woodReq += 5;
            }
        }
    }

    [PunRPC]
    public void SetTCPHasLock(bool locked)
    {
        hasLock = locked;
        if (hasLock)
        {
            lockObject.SetActive(true);
        }
        else
        {
            lockObject.SetActive(false);
        }
    }

    bool ItemExistsInTC(ItemInfo I)
    {
        for (int i = 0; i < ItemsInTC.Count; i++)
        {
            if (I == ItemsInTC[i])
                return true;
        }
        return false;
    }

    public bool CheckForItemAndQuantity(ItemInfo I, int Q)
    {
        for (int i = 0; i < ItemsInTC.Count; i++)
        {
            if (ItemsInTC[i].itemID == I.itemID && ItemQuantityInTC[i] == Q)
            {
                return true;
            }
        }
        return false;
    }

    [PunRPC]
    public void SyncTCItemsAcrossClients(int[] PhotonViewIDs, int[] ItemQuantity)
    {
        print("CLEARING ITEMS IN CRATE!");
        ItemsInTC.Clear();
        ItemQuantityInTC.Clear();
        for (int i = 0; i < PhotonViewIDs.Length; i++)
        {
            PhotonView ItemPV = PhotonView.Find(PhotonViewIDs[i]);
            ItemInfo ItemToAdd = ItemPV.gameObject.GetComponent<ItemInfo>();
            if (!ItemExistsInTC(ItemToAdd))
            {
                ItemsInTC.Add(ItemToAdd);
                ItemQuantityInTC.Add(ItemQuantity[i]);
            }
        }

        ClearLastLootPool();
        DisplayLoot();
    }

    public void UpdateLoot()
    {
        for (int i = 0; i < 9; i++)
        {

            if (IM.InventoryList[i + 48] != null)
            {
                if (!CheckForItemAndQuantity(IM.InventoryList[i + 48], IM.InventoryList[i + 48].ItemCount))
                {
                    ItemsInTC.Add(IM.InventoryList[i + 48]);
                    ItemQuantityInTC.Add(IM.InventoryList[i + 48].GetItemCount());
                }
            }
        }
        for (int i = 0; i < ItemsInTC.Count; i++)
        {
            if (IM.InventoryList[i + 48] == null)
            {

                ItemsInTC.RemoveAt(i);
                ItemQuantityInTC.RemoveAt(i);
            }

        }
        PrepareToSyncLoot();
        PhotonViewIDs.Clear();
        ClearLastLootPool();
        DisplayLoot();
    }

    public void PrepareToSyncLoot()
    {
        pv = GetComponent<PhotonView>();
        PhotonViewIDs.Clear();
        //UpdateLoot();
        for (int i = 0; i < ItemsInTC.Count; i++)
        {
            if (ItemsInTC[i].gameObject.GetComponent<PhotonView>() != null)
            {


                PhotonViewIDs.Add(ItemsInTC[i].gameObject.GetComponent<PhotonView>().ViewID);
            }
        }
        int[] PVIDArray = PhotonViewIDs.ToArray();
        int[] PVQuanArray = ItemQuantityInTC.ToArray();
        pv.RPC("SyncTCItemsAcrossClients", RpcTarget.Others, PVIDArray, PVQuanArray);
    }

    public void ClearLastLootPool()
    {
        for (int i = 0; i < 9; i++)
        {
            if (IM.InventoryList[i + 48] != null)
            {
                IM.InventoryList[i + 48].ItemCount = 0;
                IM.InventoryList[i + 48] = null;
            }
        }
        IM.UpdateItemCountPerSlot();
    }

    public void DisplayLoot()
    {
        //ClearLastLootPool();
        for (int i = 0; i < ItemsInTC.Count; i++)
        {
            IM.AddQuantityInSpecifiedBox(ItemsInTC[i], ItemQuantityInTC[i], 48 + i);
            IM.UpdateItemCountPerSlot();
        }
        UpdateInventoryWithNewItems();

    }

    public void UpdateInventoryWithNewItems()
    {
        for (int i = 0; i < ItemsInTC.Count; i++)
        {
            IM.InventoryList[i + 48] = null;

            if (ItemsInTC[i] != null)
            {
                if (ItemQuantityInTC[i] > 0)
                {
                    IM.InventoryList[i + 48] = ItemsInTC[i];
                    IM.InventoryList[i + 48].SetItemCount(ItemQuantityInTC[i]);
                }
                else
                {
                    IM.InventoryList[i + 48] = null;
                }
            }

        }
        IM.UpdateItemCountPerSlot();
    }

    public void ToggleBuildingPrivilege(PlayerProperties currPlayerProperties)
    {
        if (playersWithBuildingPrivilege.Contains(currPlayerProperties))
        {
            currPlayerProperties.hasBuildingPrivilege = false;
            currPlayerProperties.BuildingPrivilegeIcon.SetActive(false);

            currPlayerProperties.isBuildingDisabled = true;
            currPlayerProperties.BuildingDisabledIcon.SetActive(true);

            audioManager.PlayAudio((int)AudioManager.AudioID.Click);
            playersWithBuildingPrivilege.Remove(currPlayerProperties);

        }
        else
        {

            currPlayerProperties.hasBuildingPrivilege = true;
            currPlayerProperties.BuildingPrivilegeIcon.SetActive(true);

            currPlayerProperties.isBuildingDisabled = false;
            currPlayerProperties.BuildingDisabledIcon.SetActive(false);

            audioManager.PlayAudio((int)AudioManager.AudioID.LockSuccess);
            playersWithBuildingPrivilege.Add(currPlayerProperties);
        }
    }
}
