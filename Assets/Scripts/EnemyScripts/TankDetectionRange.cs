using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TankDetectionRange : TankAI
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
            PV.RPC("TankFoundPlayer", RpcTarget.All, other.GetComponent<PhotonView>().ViewID);
        }
    }

    public void OnTriggerExit(Collider other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            PV.RPC("TankLostPlayer", RpcTarget.All, other.GetComponent<PhotonView>().ViewID);
        }
    }
}
