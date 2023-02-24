using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ChickenDetectionRange : ChickenAI
{
    // Start is called before the first frame update

    

    public override void Awake()
    {
        PV = GetComponentInParent<PhotonView>();
    }

    public override void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PV.RPC("ChickenFoundPlayer", RpcTarget.All, other.GetComponent<PhotonView>().ViewID);
        }
        else if (other.gameObject.CompareTag("Wolf"))
        {
            PV.RPC("ChickenFoundWolf", RpcTarget.All, other.GetComponent<PhotonView>().ViewID);
        }
    }

    public void OnTriggerExit(Collider other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            PV.RPC("ChickenLostPlayer", RpcTarget.All, other.GetComponent<PhotonView>().ViewID);
        }
        else if (other.gameObject.CompareTag("Wolf"))
        {
            PV.RPC("ChickenLostWolf", RpcTarget.All, other.GetComponent<PhotonView>().ViewID);
        }
    }
}
