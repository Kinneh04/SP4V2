using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float Damage;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.transform.GetComponent<Enemy>().GetDamaged((int)Damage);
        }
        else if (other.CompareTag("EnemyPlayer") || other.CompareTag("Player"))
        {
            other.transform.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, Damage);
            other.transform.GetComponent<PlayerProperties>().TakeDamage(Damage);
        }
    }
}
