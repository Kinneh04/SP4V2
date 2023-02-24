using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WolfDetectionRange : WolfAI
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
            PV.RPC("WolfFoundPlayer", RpcTarget.All, other.GetComponent<PhotonView>().ViewID);
        }
        else if (other.gameObject.CompareTag("Chicken"))
        {
            PV.RPC("WolfFoundPrey", RpcTarget.All, other.GetComponent<PhotonView>().ViewID);
        }
        else if (other.gameObject.CompareTag("Deer"))
        {
            PV.RPC("WolfFoundPrey", RpcTarget.All, other.GetComponent<PhotonView>().ViewID);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PV.RPC("WolfLostPlayer", RpcTarget.All, other.GetComponent<PhotonView>().ViewID);
        }
        else if (other.gameObject.CompareTag("Chicken") || other.gameObject.CompareTag("Deer"))
        {
            PV.RPC("WolfLostPrey", RpcTarget.All, other.GetComponent<PhotonView>().ViewID);
        }

    }
}
