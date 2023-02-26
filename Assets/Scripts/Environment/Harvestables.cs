using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Harvestables : MonoBehaviour
{
    public int ItemAmount = 100;
    public int BaseHarvestAmount = 10;

    public GameObject ItemToGivePlayerOnHarvest;
    public PhotonView pv;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }
    public void HarvestAmount(float multiplier)
    {
        InventoryManager II = GameObject.FindGameObjectWithTag("Player").transform.Find("Inventory").GetComponent<InventoryManager>();
        //PhotonView GOpv = ItemToGivePlayerOnHarvest.GetComponent<PhotonView>(); //PhotonNetwork.Instantiate(ItemToGivePlayerOnHarvest.name, transform.position,Quaternion.identity).GetComponent<PhotonView>();
        //II.AddQuantity(GOpv.GetComponent<ItemInfo>(), (int)(BaseHarvestAmount * multiplier));
        ////GOpv.gameObject.SetActive(false);
      
        //print("Harvested x" + (int)(BaseHarvestAmount * multiplier));


        //PhotonView GOpv = PhotonNetwork.Instantiate(ItemToGivePlayerOnHarvest.name,transform.position,Quaternion.identity).GetComponent<PhotonView>();
        //II.AddQuantity(GOpv.GetComponent<ItemInfo>(), (int)(BaseHarvestAmount * multiplier));
        //pv.RPC("UpdateQuantityForItemOnOtherClients", RpcTarget.Others, GOpv.ViewID, (int)(BaseHarvestAmount * multiplier));
        //GOpv.RPC("ParentToObj", RpcTarget.All, GOpv.ViewID);
        //pv.RPC("TakeDamage", RpcTarget.All, multiplier);

        if(II.FindItemInSlot(ItemToGivePlayerOnHarvest.GetComponent<ItemInfo>()))
        {
            II.AddQuantity(ItemToGivePlayerOnHarvest.GetComponent<ItemInfo>(), (int)(BaseHarvestAmount * multiplier), false);
            PhotonView GOPv = II.FindItemInSlot(ItemToGivePlayerOnHarvest.GetComponent<ItemInfo>()).GetComponent<PhotonView>();
            pv.RPC("UpdateQuantityForItemOnOtherClients", RpcTarget.Others, GOPv.ViewID, (int)(BaseHarvestAmount * multiplier));
            pv.RPC("TakeDamage", RpcTarget.All, multiplier);

        }
        else
        {
            PhotonView GOpv = PhotonNetwork.Instantiate(ItemToGivePlayerOnHarvest.name, transform.position, Quaternion.identity).GetComponent<PhotonView>();
            II.AddQuantity(GOpv.GetComponent<ItemInfo>(), (int)(BaseHarvestAmount * multiplier));
            pv.RPC("UpdateQuantityForItemOnOtherClients", RpcTarget.Others, GOpv.ViewID, (int)(BaseHarvestAmount * multiplier) - 1);
            GOpv.RPC("ParentToObj", RpcTarget.All, GOpv.ViewID);
            
        }

    }

    [PunRPC]
    public void UpdateQuantityForItemOnOtherClients(int ItemID, int Quantity)
    {
        InventoryManager II = GameObject.FindGameObjectWithTag("Player").transform.Find("Inventory").GetComponent<InventoryManager>();
        PhotonView ItemPV = PhotonView.Find(ItemID);
        ItemPV.GetComponent<ItemInfo>().ItemCount += Quantity;
    }

    [PunRPC]
    public void TakeDamage(float multiplier)
    {
        ItemAmount -= (int)(BaseHarvestAmount * multiplier);
        if (ItemAmount <= 0)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
