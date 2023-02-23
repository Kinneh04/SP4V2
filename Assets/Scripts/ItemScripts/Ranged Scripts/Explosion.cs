using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float Damage;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (other.CompareTag("Enemy"))
        {
            other.transform.GetComponent<Enemy>().GetDamaged((int)Damage);
        }
        else if (other.CompareTag("EnemyPlayer") || other.CompareTag("Player"))
        {
            other.transform.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, Damage);
            other.transform.GetComponent<PlayerProperties>().TakeDamage(Damage);
        }
        else if (other.CompareTag("NormalStructure") || other.CompareTag("FoundationStructure") || other.CompareTag("FloorStructure"))
        {
            Debug.Log("EXPLOSION RAN!!");
            StructureObject structure = null;
            if (other.gameObject.layer == LayerMask.NameToLayer("BuildableParent"))
            {
                structure = other.GetComponent<StructureObject>();
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("Buildable"))
            {
                structure = other.GetComponentInParent<StructureObject>();
            }
            if (!structure)
                return;

            Debug.Log(structure.type);

            if (structure.isUpgraded)
            {

                structure.gameObject.GetComponent<PhotonView>().RPC("DamageStructure", RpcTarget.AllViaServer, (float)(Damage * 0.8f));
            }
            else
            {
                structure.gameObject.GetComponent<PhotonView>().RPC("DamageStructure", RpcTarget.AllViaServer, Damage);
            }
        }
    }
}
