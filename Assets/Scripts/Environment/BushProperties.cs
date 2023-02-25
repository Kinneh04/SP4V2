using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BushProperties : MonoBehaviour
{
    int giveAmount = 1;
    public float CoolDownInSeconds = 10f;
    public GameObject PickedState;
    public GameObject GameObjectToGive;
    public int minAmount;
    InventoryManager IM;
    public int maxAmount;
    public PhotonView pv;
    private void Start()
    {
        pv = GetComponent<PhotonView>();
    }
    public void Pick()
    {
        if (PickedState.activeSelf)
        {
            IM = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<InventoryManager>();
            PickedState.SetActive(false);
            int PickAmount = Random.Range(minAmount, maxAmount);
            PhotonView GO = PhotonNetwork.Instantiate(GameObjectToGive.name, transform.position, Quaternion.identity).GetComponent<PhotonView>();
            ItemInfo GOIF = GO.GetComponent<ItemInfo>();
            IM.AddQuantity(GOIF, PickAmount);
            GO.gameObject.SetActive(false);
            IM.UpdateItemCountPerSlot();
            pv.RPC("SomeoneJustPickedIt", RpcTarget.All);
            StartCoroutine(Refresh());
        }
    }

    IEnumerator Refresh()
    {
        yield return new WaitForSeconds(CoolDownInSeconds);
        pv.RPC("ReadyTobePickedAgin", RpcTarget.All);
    }

    [PunRPC]
    void ReadyTobePickedAgin()
    {
        PickedState.SetActive(true);
    }

    [PunRPC]
    void SomeoneJustPickedIt()
    {
        PickedState.SetActive(false);
    }
}
