using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HeliRocket : MonoBehaviour
{
    public float speed = 10f;
    public float deviance = 0.1f;
    public GameObject ExplosionEffect;
    public PhotonView pv;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (pv.IsMine)
        {
            // calculate a random speed deviation
            float deviation = Random.Range(-deviance, deviance);

            // move the game object forward at the base speed plus the deviation
            transform.Translate(Vector3.forward * (speed + deviation) * Time.deltaTime);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        print(collision.gameObject.name);
        print("OW!");
        Explode();
    }
    private void OnCollisionStay(Collision collision)
    {
        print(collision.gameObject.name);
        print("OW!");
        Explode();
    }
    
    void Explode()
    {
        Instantiate(ExplosionEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
