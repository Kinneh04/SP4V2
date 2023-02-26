using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class HarvestToolsProperties : ItemInfo
{
    public PhotonView pv;
    public float damage = 1.0f;
    public float WoodHarvestMultiplier = 1.0f;
    public float StoneHarvestMultiplier = 1.0f;
    public float MetalHarvestMultiplier = 1.0f;
    public bool TriggerEnabled = false;
    public GameObject WoodParticles;
    public GameObject StoneParticles;
    [SerializeField]
    private GameObject BloodParticleSystem;
    public float durability = 100f;
    public float StoneDurabilityBurn = 2.0f;
    public float WoodDurabilityBurn = 2.0f;
    public float MetalDurabilityBurn = 2.0f;
    public ItemInfo II;
    public float usecooldown;

    public AudioManager AM;

    private void Awake()
    {
        II = GetComponent<ItemInfo>();
        pv = GetComponent<PhotonView>();
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Tree") || other.CompareTag("Stone") || other.CompareTag("Metal") || other.CompareTag("Sulfur"))
        {
            if(!AM)
            {
                AM = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
            }
            if (TriggerEnabled)
            {
                TriggerEnabled = false;
                print("Harvested " + other.gameObject.tag);

                if (other.CompareTag("Tree"))
                {
                    other.gameObject.GetComponent<Harvestables>().HarvestAmount(WoodHarvestMultiplier);

                    Instantiate(WoodParticles, transform.position, Quaternion.identity);
                    durability -= WoodDurabilityBurn;
                    AM.MultiplayerPlay3DAudio(46, 1, transform.position);
                }
                else if (other.CompareTag("Stone"))
                {
                    other.gameObject.GetComponent<Harvestables>().HarvestAmount(StoneHarvestMultiplier);
                    Instantiate(StoneParticles, transform.position, Quaternion.identity);
                    durability -= StoneDurabilityBurn;
                    AM.MultiplayerPlay3DAudio(47, 1, transform.position);
                }
                else if(other.CompareTag("Sulfur"))
                {
                    other.gameObject.GetComponent<Harvestables>().HarvestAmount(StoneHarvestMultiplier);
                    Instantiate(StoneParticles, transform.position, Quaternion.identity);
                    durability -= StoneDurabilityBurn;
                    AM.MultiplayerPlay3DAudio(47, 1, transform.position);
                }
                else if (other.CompareTag("Metal"))
                {
                    other.gameObject.GetComponent<Harvestables>().HarvestAmount(MetalHarvestMultiplier);
                    Instantiate(StoneParticles, transform.position, Quaternion.identity);
                    durability -= MetalDurabilityBurn;
                    AM.MultiplayerPlay3DAudio(47, 1, transform.position);
                }
                else if (other.CompareTag("Player") || other.CompareTag("EnemyPlayer"))
                {
                    //other.transform.GetComponent<PlayerProperties>().TakeDamageV2(damage);
                    other.transform.GetComponent<PhotonView>().RPC("TakeServerSideDamage", RpcTarget.All, damage);
                    Instantiate(BloodParticleSystem, other.transform.position, Quaternion.identity);
                    durability -= MetalDurabilityBurn;
                }
            }
        }
        else if (other.CompareTag("Chicken") || other.CompareTag("Deer") || other.CompareTag("Wolf"))
        {
            if (TriggerEnabled)
            {
                if (other.CompareTag("Chicken") && other.GetComponent<ChickenAI>().Harvestable)
                {
                    other.GetComponent<Harvestables>().HarvestAmount(WoodHarvestMultiplier);
                }
                else if (other.CompareTag("Deer") && other.GetComponent<DeerAI>().Harvestable)
                {
                    other.GetComponent<Harvestables>().HarvestAmount(WoodHarvestMultiplier);
                }
                else if (other.CompareTag("Wolf") && other.GetComponent<WolfAI>().Harvestable)
                {
                    other.GetComponent<Harvestables>().HarvestAmount(WoodHarvestMultiplier);
                }
                Instantiate(BloodParticleSystem);
            }
        }
        else if (other.CompareTag("Enemy"))
        {
            if (TriggerEnabled)
            {
                Enemy enemy = other.gameObject.GetComponent<Enemy>();
                enemy.GetDamaged((int)damage);
            }
        }
        else if (other.CompareTag("NormalStructure") || other.CompareTag("FoundationStructure") || other.CompareTag("FloorStructure"))
        {
            if (TriggerEnabled)
            {
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

                if (structure.PlayerID != PhotonNetwork.LocalPlayer.ActorNumber) // Cannot damage own structures!
                {
                    if (structure.isUpgraded)
                    {
                        structure.gameObject.GetComponent<PhotonView>().RPC("DamageStructure", RpcTarget.AllViaServer, (float)(StoneHarvestMultiplier * 0.5f));
                    }
                    else
                    {
                        structure.gameObject.GetComponent<PhotonView>().RPC("DamageStructure", RpcTarget.AllViaServer, WoodHarvestMultiplier);
                    }
                }
            }
        }
    }
}
