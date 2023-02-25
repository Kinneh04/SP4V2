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
        PhotonView GOpv = PhotonNetwork.Instantiate(ItemToGivePlayerOnHarvest.name, transform.position,Quaternion.identity).GetComponent<PhotonView>();
        II.AddQuantity(GOpv.GetComponent<ItemInfo>(), (int)(BaseHarvestAmount * multiplier));
        //GOpv.gameObject.SetActive(false);
        GOpv.RPC("ParentToObj", RpcTarget.All, GOpv.ViewID);
        //print("Harvested x" + (int)(BaseHarvestAmount * multiplier));
        pv.RPC("TakeDamage", RpcTarget.All, multiplier);
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
