using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DetectionRange : ScientistAI
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
            PV.RPC("FoundPlayer", RpcTarget.All, other.GetComponent<PhotonView>().ViewID);
        }
        if (other.gameObject.CompareTag("Monument"))
        {
            PV.RPC("FoundMonument", RpcTarget.All, other.GetComponent<PhotonView>().ViewID);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        
        if (other.gameObject.CompareTag("Player"))
        {
            PV.RPC("LostPlayer", RpcTarget.All, other.GetComponent<PhotonView>().ViewID);
        }
    }
}
