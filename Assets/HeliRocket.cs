using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HeliRocket : MonoBehaviour
{
    public float speed = 10f;
    public int Damage;
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
        if (pv.IsMine)
        {
            print(collision.gameObject.name);
            print("OW!");
            Explode();
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (pv.IsMine)
        {
            print(collision.gameObject.name);
            print("OW!");
            Explode();
        }
    }
    
    void Explode()
    {
        if (pv.IsMine)
        {
            GameObject GO = PhotonNetwork.Instantiate(ExplosionEffect.name, transform.position, Quaternion.identity).gameObject;
            pv.RPC("AssignDamageValues", RpcTarget.All, GO.GetComponent<PhotonView>().ViewID, Damage);
            GO.GetComponent<Explosion>().Damage = Damage;
            Destroy(gameObject);
        }
    }

    [PunRPC]
    void AssignDamageValues(int PVID, int damageValue)
    {
        GameObject Missle = PhotonView.Find(PVID).gameObject;
        Missle.GetComponent<Explosion>().Damage = damageValue;
    }
}
