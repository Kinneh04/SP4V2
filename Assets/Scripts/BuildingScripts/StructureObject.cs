using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class StructureObject : MonoBehaviour
{
    public StructureTypes type;
    public AudioManager audioManager;
    public GameObject selectedPrefab;
    public Material stoneMaterial;
    public Slider slider;
    public GameObject destroyVFX;
    public List<GameObject> dependentStructures = new List<GameObject>();
    public TMPro.TMP_Text stabilityLabel;
    public int PlayerID;
    public bool isUpgraded = false;
    public float stability = 100;
    public float pickupCooldown = 15.0f;
    public float damageCooldown = 0.5f;

    public bool isDecaying = true;
    public float decayCd = 30.0f;

    private void Start()
    {
        isUpgraded = false;
        stability = 100;
        pickupCooldown = 15.0f;

        if (slider != null)
        {
            slider.gameObject.SetActive(false);
            stabilityLabel.gameObject.SetActive(false);
        }
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }

    private void Update()
    {
        if (pickupCooldown > 0.0f)
            pickupCooldown -= Time.deltaTime;

        if (damageCooldown > 0.0f)
            damageCooldown -= Time.deltaTime;

        if (isDecaying) // -5HP every 30s
        {
            if (decayCd > 0.0f)
            {
                decayCd -= Time.deltaTime;
            }
            else
            {
                GetComponent<PhotonView>().RPC("DamageStructure", RpcTarget.AllViaServer, 1.7f);
                // TODO: Decaying audio maybe?
                decayCd = isUpgraded ? 60.0f : 30.0f; // Stone takes -5HP every 60s
            }
        }

            slider.value = stability;
            stabilityLabel.text = Math.Round(stability, 2) + "%";
    }

    [PunRPC]
    public void UpgradeStructure()
    {
        isUpgraded = true;
        stability = 100; // Reset stability
        foreach (Transform child in transform) // Change look to stone
        {
            if (child.GetComponent<Renderer>() == null)
                return;

            Material[] mats = child.GetComponent<Renderer>().materials;
            mats[0] = stoneMaterial;
            child.GetComponent<Renderer>().materials = mats;
        }
    }

    [PunRPC]
    public void RepairStructure(float amt)
    {
        stability += amt;
        if (stability > 100)
            stability = 100;
    }

    [PunRPC]
    public void DamageStructure(float multiplier)
    {
        if (damageCooldown > 0.0f)
            return;

        stability -= 3 * multiplier;
        damageCooldown = 0.5f;
        if (stability <= 0)
        {
            audioManager.GetComponent<PhotonView>().RPC("MultiplayerPlay3DAudio", RpcTarget.AllViaServer, AudioManager.AudioID.DestroyBuilding, 1.0f, gameObject.transform.position);
            GetComponent<PhotonView>().RPC("DestroyStructureObject", PhotonNetwork.CurrentRoom.GetPlayer(gameObject.GetComponent<StructureObject>().PlayerID), gameObject.GetComponent<PhotonView>().ViewID);
            return;
        }

        SetDamaged(); // Show labels for people that hit this structure
    }

    private void SetDamaged()
    {
        if (PlayerID != PhotonNetwork.LocalPlayer.ActorNumber)
        {
            stabilityLabel.gameObject.SetActive(true);
            slider.gameObject.SetActive(true);
        }
    }

    [PunRPC]
    public void DestroyStructureObject(int viewID)
    {
        // Also destroy structures dependent on this object
        foreach (GameObject structure in PhotonView.Find(viewID).gameObject.GetComponent<StructureObject>().dependentStructures)
        {
            if (structure) // null check for destroyed ones
            {
                PhotonView.Find(viewID).RPC("DestroyStructureObject", PhotonNetwork.CurrentRoom.GetPlayer(structure.GetComponent<StructureObject>().PlayerID), structure.GetComponent<PhotonView>().ViewID);
            }
        }

        if (PhotonView.Find(viewID).gameObject.GetComponent<StructureObject>().isUpgraded)
        {
            PhotonNetwork.Instantiate("DestroyStructureStone", PhotonView.Find(viewID).gameObject.transform.position, PhotonView.Find(viewID).gameObject.transform.rotation);
        }
        else
        {
            PhotonNetwork.Instantiate("DestroyStructure", PhotonView.Find(viewID).gameObject.transform.position, PhotonView.Find(viewID).gameObject.transform.rotation);
        }
        PhotonNetwork.Destroy(PhotonView.Find(viewID));
    }
}

public enum StructureTypes
{
    foundation,
    floor,
    wall,
    stairs,
    window,
    doorway,
    door
}
