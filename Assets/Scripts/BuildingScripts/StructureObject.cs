using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureObject : MonoBehaviour
{
    public StructureTypes type;
    public GameObject selectedPrefab;
    public Material stoneMaterial;
    public bool isUpgraded = false;
    public float stability = 100;
    public float pickupCooldown = 15.0f;

    private void Start()
    {
        stability = 100;
        pickupCooldown = 15.0f;
    }

    private void Update()
    {
        if (pickupCooldown > 0.0f)
            pickupCooldown -= Time.deltaTime;
    }

    public void UpgradeStructure()
    {
        stability = 100; // Reset stability
        foreach (Transform child in transform) // Change look to stone
        {
            if (child.GetComponent<Renderer>() == null)
                return;

            Material[] mats = child.GetComponent<Renderer>().materials;
            mats[0] = stoneMaterial;
            child.GetComponent<Renderer>().materials = mats;
        }
        isUpgraded = true;
    }
}

public enum StructureTypes
{
    foundation,
    floor,
    wall,
    stairs,
    doorway,
    window
}
