using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class StructureObject : MonoBehaviour
{
    public StructureTypes type;
    public GameObject selectedPrefab;
    public Material stoneMaterial;
    public Slider slider;
    public TMPro.TMP_Text stabilityLabel;
    public int PlayerID;
    public bool isUpgraded = false;
    public float stability = 100;
    public float pickupCooldown = 15.0f;
    public float damageCooldown = 0.5f;

    public bool isDamaged;

    private void Start()
    {
        isDamaged = false;
        isUpgraded = false;
        stability = 100;
        pickupCooldown = 15.0f;

        slider.gameObject.SetActive(false);
        stabilityLabel.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (pickupCooldown > 0.0f)
            pickupCooldown -= Time.deltaTime;

        if (damageCooldown > 0.0f)
            damageCooldown -= Time.deltaTime;

        if (isDamaged) // To save resources, will only start updating when someone damages this, otherwise it's perma 100%
        {
            slider.value = stability;
            stabilityLabel.text = Math.Round(stability, 2) + "%";
        }
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

        Debug.Log("MULTIPLIER: " + multiplier);
        stability -= 3 * multiplier;
        damageCooldown = 0.5f;
        if (stability <= 0)
        {
            PhotonNetwork.Destroy(gameObject);
            return;
        }

        if (!isDamaged)
        {
            SetDamaged();
        }
    }

    private void SetDamaged()
    {
        if (PlayerID != PhotonNetwork.LocalPlayer.ActorNumber)
        {
            stabilityLabel.gameObject.SetActive(true);
            slider.gameObject.SetActive(true);
        }

        isDamaged = true;
    }
}

public enum StructureTypes
{
    foundation,
    floor,
    wall,
    stairs,
    doorway,
    window,
    door
}
