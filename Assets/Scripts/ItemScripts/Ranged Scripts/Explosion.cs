using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float Damage;
    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        //print("Exploded on: " + other.tag);
        Debug.Log(other.tag);
        if (other.CompareTag("Enemy"))
        {
            other.transform.GetComponent<Enemy>().GetDamaged((int)Damage);
        }
        else if (other.CompareTag("EnemyPlayer") || other.CompareTag("Player"))
        {
            print("MISSLE HURTY MAN!");
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
            audioManager.GetComponent<PhotonView>().RPC("MultiplayerPlay3DAudio", RpcTarget.All, 31, 0.75f, transform.position);
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
