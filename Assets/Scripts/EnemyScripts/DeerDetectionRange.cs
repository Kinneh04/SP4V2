using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DeerDetectionRange : MonoBehaviour
{
    // Start is called before the first frame update

    List<GameObject> DetectedPredator = new List<GameObject>();
    PhotonView PV;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
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

    [PunRPC]
    void DeerFoundPlayer(int ID)
    {
        GameObject other = PhotonView.Find(ID).gameObject;
        DetectedPredator.Add(other);
        // Targeting
        DeerAI deer = GetComponent<DeerAI>();
        if (deer.TargetPlayer == null)
        {
            deer.TargetPlayer = other;
            deer.change = true;
        }
        else if (Vector3.Distance(deer.transform.position, deer.TargetPlayer.transform.position) > Vector3.Distance(deer.transform.position, other.transform.position))
        {
            deer.TargetPlayer = other;
            deer.change = true;
        }
    }

    [PunRPC]
    void DeerFoundWolf(int ID)
    {
        GameObject other = PhotonView.Find(ID).gameObject;
        DetectedPredator.Add(other);
        DeerAI deer = GetComponent<DeerAI>();
        if (deer.Predator == null)
        {
            deer.Predator = other;
            deer.change = true;
        }
        else if (Vector3.Distance(deer.transform.position, deer.Predator.transform.position) > Vector3.Distance(deer.transform.position, other.transform.position))
        {
            deer.Predator = other;
            deer.change = true;
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

    [PunRPC]
    void DeerLostPlayer(int ID)
    {
        GameObject other = PhotonView.Find(ID).gameObject;
        bool TargetLeft = false;
        DeerAI deer = GetComponent<DeerAI>();
        if (deer.TargetPlayer == other)
        {
            TargetLeft = true;
            deer.TargetPlayer = null;
            deer.change = true;
            DetectedPredator.Remove(other);
        }
        if (TargetLeft)
        {
            for (int i = 0; i < DetectedPredator.Count; i++)
            {
                if (deer.TargetPlayer == null)
                {
                    deer.TargetPlayer = DetectedPredator[0];
                }
                else if (Vector3.Distance(deer.transform.position, deer.TargetPlayer.transform.position) > Vector3.Distance(deer.transform.position, DetectedPredator[i].transform.position))
                {
                    deer.TargetPlayer = DetectedPredator[i];
                }
            }
        }
    }

    [PunRPC]
    void DeerLostWolf(int ID)
    {
        GameObject other = PhotonView.Find(ID).gameObject;
        bool PredatorLeft = false;
        DeerAI deer = GetComponent<DeerAI>();
        if (deer.Predator == other)
        {
            PredatorLeft = true;
            deer.Predator = null;
            deer.change = true;
            DetectedPredator.Remove(other);
        }
        if (PredatorLeft)
        {
            for (int i = 0; i < DetectedPredator.Count; i++)
            {
                if (deer.Predator == null)
                {
                    deer.Predator = DetectedPredator[0];
                }
                else if (Vector3.Distance(deer.transform.position, deer.Predator.transform.position) > Vector3.Distance(deer.transform.position, DetectedPredator[i].transform.position))
                {
                    deer.Predator = DetectedPredator[i];
                }
            }
        }
    }
}
