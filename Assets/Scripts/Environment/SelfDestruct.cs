using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SelfDestruct : MonoBehaviour
{
    public float seconds;
    PhotonView PV;
    public bool NetworkedDestruct = false;
    private void Start()
    {
        if (NetworkedDestruct)
        {
            Destroy(gameObject, seconds);
        }
        else
        {
            PV = GetComponent<PhotonView>();
            NetworkedDestroyAfterSeconds(seconds);
        }
    }

    IEnumerator NetworkedDestroyAfterSeconds(float S)
    {
        yield return new WaitForSeconds(S);
        PhotonNetwork.Destroy(gameObject);
    }

}
