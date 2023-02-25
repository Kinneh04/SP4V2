using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DeerDetectionRange : DeerAI
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
            PV.RPC("DeerFoundPlayer", RpcTarget.All, other.GetComponent<PhotonView>().ViewID);
        }
        else if (other.gameObject.CompareTag("Wolf"))
        {
            PV.RPC("DeerFoundWolf", RpcTarget.All, other.GetComponent<PhotonView>().ViewID);
        }
    }

    public void OnTriggerExit(Collider other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            PV.RPC("DeerLostPlayer", RpcTarget.All, other.GetComponent<PhotonView>().ViewID);
        }
        else if (other.gameObject.CompareTag("Wolf"))
        {
            PV.RPC("DeerLostWolf", RpcTarget.All, other.GetComponent<PhotonView>().ViewID);
        }
    }
}
