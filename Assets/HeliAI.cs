using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Photon.Pun;
using UnityEngine.PlayerLoop;

public class HeliAI : MonoBehaviour
{

    public float speed = 10f;
    public List<GameObject> targets = new List<GameObject>();
    public float ShootCooldown = 5.0f;
    public float burstCooldown = 0.25f;
    public GameObject spawnPoint;
    public PhotonView pv;
    public float HeliHealth = 5000;
    public GameObject ExplosionParticles;
    public GameObject DeathParticles;
    public GameObject LootBox;
    public int burstAmount = 8;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        
    }

    [PunRPC]
    void AddPlayerToTargetsList(int ID)
    {
        if (pv.IsMine)
        {
            GameObject PlayerGO = PhotonView.Find(ID).gameObject;
            targets.Add(PlayerGO);
        }
    }

    [PunRPC]
    void RemovePlayerFromTargetList(int ID)
    {
        if (pv.IsMine)
        {
            GameObject PlayerGO = PhotonView.Find(ID).gameObject;
            if (targets.Contains(PlayerGO))
            {
                targets.Remove(PlayerGO);
            }
        }
    }

    [PunRPC]
    public void TakeDamage(float Damage)
    { 
        
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate(ExplosionParticles.name, transform.position, Quaternion.identity);
        }

        HeliHealth -= Damage;
        if(HeliHealth < 0)
        {
            if (pv.IsMine)
            {
               
                PhotonNetwork.Instantiate(DeathParticles.name, transform.position, Quaternion.identity);
                PhotonNetwork.Instantiate(LootBox.name, transform.position, Quaternion.identity);
                PhotonNetwork.Destroy(gameObject);
            }
        }

    }

    private void Update()
    {
        if (pv.IsMine)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            //print(Vector3.Distance(gameObject.transform.position, Target.transform.position));
            if (targets.Count > 0)
            {
                ShootCooldown -= Time.deltaTime;
                if (ShootCooldown <= 0)
                {
                    burstCooldown -= Time.deltaTime;
                    if (burstCooldown <= 0)
                    {
                        int r = Random.Range(0, targets.Count);
                        burstCooldown = 0.35f;
                        ShootMissleAtTarget(r);
                        burstAmount--;
                        if (burstAmount <= 0)
                        {
                            burstAmount = 5;
                            ShootCooldown = 10.0f;
                            burstCooldown = 0.35f;
                        }
                    }
                }
            }
        }
    }

    public void ShootMissleAtTarget(int targetID)
    {
        print("PEW!");
        PhotonView newObject = PhotonNetwork.Instantiate("Helimissle", spawnPoint.transform.position, Quaternion.identity,0).GetComponent<PhotonView>();
        newObject.transform.LookAt(targets[targetID].transform);
    }
}
