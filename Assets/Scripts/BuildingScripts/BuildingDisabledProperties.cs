using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingDisabledProperties : MonoBehaviour
{
    public ToolCupboardProperties tcp;
    public List<PlayerProperties> playersInZone = new List<PlayerProperties>();
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playersInZone.Add(other.GetComponent<PlayerProperties>());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerProperties>().isBuildingDisabled = false;
            other.GetComponent<PlayerProperties>().BuildingDisabledIcon.SetActive(false);
            playersInZone.Remove(other.GetComponent<PlayerProperties>());
        }
    }
    private void Update()
    {
        foreach (PlayerProperties pp in playersInZone)
        {
            if (!pp.hasBuildingPrivilege)
            {
                pp.isBuildingDisabled = true;
                pp.BuildingDisabledIcon.SetActive(true);
            }
        }
    }
}
