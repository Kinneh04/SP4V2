using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeliDetect : MonoBehaviour
{
    public PhotonView pv;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && pv.IsMine || other.CompareTag("EnemyPlayer") && pv.IsMine)
        {
            pv.RPC("AddPlayerToTargetsList", RpcTarget.All, other.GetComponent<PhotonView>().ViewID);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && pv.IsMine || other.CompareTag("EnemyPlayer") && pv.IsMine)
        {
            pv.RPC("RemovePlayerFromTargetList", RpcTarget.All, other.GetComponent<PhotonView>().ViewID);
        }
    }
}
