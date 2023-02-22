using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DetectionRange : MonoBehaviour
{
    // Start is called before the first frame update

    List<GameObject> DetectedPlayers = new List<GameObject>();
    PhotonView PV;

    private void Awake()
    {
        PV = GetComponentInParent<PhotonView>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Found Player!");
            //if (PV.IsMine)
                PV.RPC("FoundPlayer", RpcTarget.All, other.GetComponent<PhotonView>().ViewID);
        }
        if (other.gameObject.CompareTag("Monument"))
        {
            PV.RPC("FoundMonument", RpcTarget.All, other.GetComponent<PhotonView>().ViewID);
        }
    }

    [PunRPC]
    void FoundPlayer(int ID)
    {
        GameObject other = PhotonView.Find(ID).gameObject;
        DetectedPlayers.Add(other);
        // Targeting
        Enemy enemy = GetComponent<Enemy>();
        if (enemy.TargetPlayer == null)
        {
            enemy.TargetPlayer = other;
        }
        else if (Vector3.Distance(enemy.transform.position, enemy.TargetPlayer.transform.position) > Vector3.Distance(enemy.transform.position, other.transform.position))
        {
            enemy.TargetPlayer = other;
        }
    }

    [PunRPC]
    void FoundMonument(int ID)
    {
        GameObject other = PhotonView.Find(ID).gameObject;
        Enemy enemy = GetComponent<Enemy>();
        enemy.Structure = other;
    }

    public void OnTriggerExit(Collider other)
    {
        
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Lost Player!");
            //if (PV.IsMine)
                PV.RPC("LostPlayer", RpcTarget.All, other.GetComponent<PhotonView>().ViewID);
        }
    }

    [PunRPC]
    void LostPlayer(int ID)
    {
        GameObject other = PhotonView.Find(ID).gameObject;
        bool TargetLeft = false;
        Enemy enemy = GetComponent<Enemy>();
        if (enemy.TargetPlayer == other)
        {
            TargetLeft = true;
            enemy.TargetPlayer = null;
            DetectedPlayers.Remove(other);
        }
        if (TargetLeft)
        {
            for (int i = 0; i < DetectedPlayers.Count; i++)
            {
                if (enemy.TargetPlayer == null)
                {
                    enemy.TargetPlayer = DetectedPlayers[0];
                }
                else if (Vector3.Distance(enemy.transform.position, enemy.TargetPlayer.transform.position) > Vector3.Distance(enemy.transform.position, DetectedPlayers[i].transform.position))
                {
                    enemy.TargetPlayer = DetectedPlayers[i];
                }
            }
        }
    }
}
