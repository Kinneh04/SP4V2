using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WolfDetectionRange : MonoBehaviour
{
    // Start is called before the first frame update

    List<GameObject> DetectedPlayers = new List<GameObject>();
    List<GameObject> DetectedPrey = new List<GameObject>();
    PhotonView PV;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
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

    [PunRPC]
    void WolfFoundPlayer(int ID)
    {
        GameObject other = PhotonView.Find(ID).gameObject;
        DetectedPlayers.Add(other);
        // Targeting
        WolfAI wolf = GetComponent<WolfAI>();
        if (wolf.TargetPlayer == null)
        {
            wolf.TargetPlayer = other;
            wolf.change = true;
        }
        else if (Vector3.Distance(wolf.transform.position, wolf.TargetPlayer.transform.position) > Vector3.Distance(wolf.transform.position, other.transform.position))
        {
            wolf.TargetPlayer = other;
            wolf.change = true;
        }
    }

    [PunRPC]
    void WolfFoundPrey(int ID)
    {
        GameObject other = PhotonView.Find(ID).gameObject;
        DetectedPrey.Add(other);
        WolfAI wolf = GetComponent<WolfAI>();
        if (wolf.Prey == null)
        {
            wolf.Prey = other;
            wolf.change = true;
        }
        else if (Vector3.Distance(wolf.transform.position, wolf.Prey.transform.position) > Vector3.Distance(wolf.transform.position, other.transform.position))
        {
            wolf.Prey = other;
            wolf.change = true;
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

    [PunRPC]
    void WolfLostPlayer(int ID)
    {
        GameObject other = PhotonView.Find(ID).gameObject;
        bool TargetLeft = false;
        WolfAI wolf = GetComponent<WolfAI>();
        if (wolf.TargetPlayer == other)
        {
            TargetLeft = true;
            wolf.TargetPlayer = null;
            wolf.change = true;
            DetectedPlayers.Remove(other);
        }
        if (TargetLeft)
        {
            for (int i = 0; i < DetectedPlayers.Count; i++)
            {
                if (wolf.TargetPlayer == null)
                {
                    wolf.TargetPlayer = DetectedPlayers[0];
                }
                else if (Vector3.Distance(wolf.transform.position, wolf.TargetPlayer.transform.position) > Vector3.Distance(wolf.transform.position, DetectedPlayers[i].transform.position))
                {
                    wolf.TargetPlayer = DetectedPlayers[i];
                }
            }
        }
    }

    [PunRPC]
    void WolfLostPrey(int ID)
    {
        GameObject other = PhotonView.Find(ID).gameObject;
        bool PreyLeft = false;
        WolfAI wolf = gameObject.GetComponentInParent<WolfAI>();
        if (wolf.Prey == other)
        {
            PreyLeft = true;
            wolf.Prey = null;
            wolf.change = true;
            DetectedPrey.Remove(other);
        }
        if (PreyLeft)
        {
            for (int i = 0; i < DetectedPrey.Count; i++)
            {
                if (wolf.Prey == null)
                {
                    wolf.Prey = DetectedPrey[0];
                }
                else if (Vector3.Distance(wolf.transform.position, wolf.Prey.transform.position) > Vector3.Distance(wolf.transform.position, DetectedPrey[i].transform.position))
                {
                    wolf.Prey = DetectedPrey[i];
                }
            }
        }
    }
}
