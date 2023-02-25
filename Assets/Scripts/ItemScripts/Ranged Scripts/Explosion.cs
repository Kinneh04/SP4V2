using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float Damage;
    private void OnTriggerEnter(Collider other)
    {
        print("Exploded on: " + other.name);
        Debug.Log(other.name);
        if (other.CompareTag("Enemy"))
        {
            other.transform.GetComponent<Enemy>().GetDamaged((int)Damage);
        }
        else if (other.CompareTag("EnemyPlayer") || other.CompareTag("Player"))
        {
            Debug.Log(other.tag);
            other.transform.GetComponent<PlayerProperties>().TakeDamageV2(Damage);
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
