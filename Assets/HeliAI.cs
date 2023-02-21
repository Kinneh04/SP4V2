using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Photon.Pun;
using UnityEngine.PlayerLoop;

public class HeliAI : MonoBehaviour
{

    public float speed = 10f;
    public GameObject Target;
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
    public void TakeDamage(float Damage)
    { 
        
        if(pv.IsMine)
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
        if (Target == null) Target = GameObject.FindGameObjectWithTag("Player");
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        if (Target!=null && Vector3.Distance(gameObject.transform.position, Target.transform.position) < 500)
        {
            //print(Vector3.Distance(gameObject.transform.position, Target.transform.position));
            ShootCooldown -= Time.deltaTime;
            if(ShootCooldown <= 0)
            {
                burstCooldown -= Time.deltaTime;
                if(burstCooldown <= 0)
                {
                    burstCooldown = 0.35f;
                    ShootMissleAtTarget();
                    burstAmount--;
                }
                if(burstAmount <= 0)
                {
                    burstAmount = 8;
                    ShootCooldown = 8.0f;
                    burstCooldown = 0.35f;
                }
                //ShootCooldown = 3.0f;
                //ShootMissleAtTarget();
            }
        }
    }

    public void ShootMissleAtTarget()
    {
        print("PEW!");
        PhotonView newObject = PhotonNetwork.Instantiate("HeliMissle", spawnPoint.transform.position, Quaternion.identity,0).GetComponent<PhotonView>();
        newObject.transform.LookAt(Target.transform);
    }
}
