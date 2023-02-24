using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ChickenDetectionRange : MonoBehaviour
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
            PV.RPC("ChickenFoundPlayer", RpcTarget.All, other.GetComponent<PhotonView>().ViewID);
        }
        else if (other.gameObject.CompareTag("Wolf"))
        {
            PV.RPC("ChickenFoundWolf", RpcTarget.All, other.GetComponent<PhotonView>().ViewID);
        }
    }

    [PunRPC]
    void ChickenFoundPlayer(int ID)
    {
        GameObject other = PhotonView.Find(ID).gameObject;
        DetectedPredator.Add(other);
        // Targeting
        ChickenAI chicken = GetComponent<ChickenAI>();
        if (chicken.TargetPlayer == null)
        {
            chicken.TargetPlayer = other;
            chicken.change = true;
        }
        else if (Vector3.Distance(chicken.transform.position, chicken.TargetPlayer.transform.position) > Vector3.Distance(chicken.transform.position, other.transform.position))
        {
            chicken.TargetPlayer = other;
            chicken.change = true;
        }
    }

    [PunRPC]
    void ChickenFoundWolf(int ID)
    {
        Enemy other = PhotonView.Find(ID).gameObject.GetComponent<Enemy>();
        if (other.dead)
            return;
        DetectedPredator.Add(other.gameObject);
        ChickenAI chicken = GetComponent<ChickenAI>();
        if (chicken.Predator == null)
        {
            chicken.Predator = other.gameObject;
            chicken.change = true;
        }
        else if (Vector3.Distance(chicken.transform.position, chicken.Predator.transform.position) > Vector3.Distance(chicken.transform.position, other.transform.position))
        {
            chicken.Predator = other.gameObject;
            chicken.change = true;
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

    [PunRPC]
    void ChickenLostPlayer(int ID)
    {
        GameObject other = PhotonView.Find(ID).gameObject;
        bool TargetLeft = false;
        ChickenAI chicken = GetComponent<ChickenAI>();
        if (chicken.TargetPlayer == other)
        {
            TargetLeft = true;
            chicken.TargetPlayer = null;
            chicken.change = true;
            DetectedPredator.Remove(other);
        }
        if (TargetLeft)
        {
            for (int i = 0; i < DetectedPredator.Count; i++)
            {
                if (chicken.TargetPlayer == null)
                {
                    chicken.TargetPlayer = DetectedPredator[0];
                }
                else if (Vector3.Distance(chicken.transform.position, chicken.TargetPlayer.transform.position) > Vector3.Distance(chicken.transform.position, DetectedPredator[i].transform.position))
                {
                    chicken.TargetPlayer = DetectedPredator[i];
                }
            }
        }
    }

    [PunRPC]
    void ChickenLostWolf(int ID)
    {
        GameObject other = PhotonView.Find(ID).gameObject;
        bool PredatorLeft = false;
        ChickenAI chicken = GetComponent<ChickenAI>();
        if (chicken.Predator == other)
        {
            PredatorLeft = true;
            chicken.Predator = null;
            chicken.change = true;
            DetectedPredator.Remove(other);
        }
        if (PredatorLeft)
        {
            for (int i = 0; i < DetectedPredator.Count; i++)
            {
                if (DetectedPredator[i].GetComponent<Enemy>().dead)
                    continue;
                if (chicken.Predator == null)
                {
                    chicken.Predator = DetectedPredator[i];
                }
                else if (Vector3.Distance(chicken.transform.position, chicken.Predator.transform.position) > Vector3.Distance(chicken.transform.position, DetectedPredator[i].transform.position))
                {
                    chicken.Predator = DetectedPredator[i];
                }
            }
        }
    }
}
