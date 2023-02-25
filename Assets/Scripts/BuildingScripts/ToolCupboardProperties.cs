using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolCupboardProperties : MonoBehaviour
{
    public List<PlayerProperties> playersWithBuildingPrivilege = new List<PlayerProperties>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playersWithBuildingPrivilege.Contains(other.GetComponent<PlayerProperties>()))
            {
                other.GetComponent<PlayerProperties>().hasBuildingPrivilege = true;
                other.GetComponent<PlayerProperties>().BuildingPrivilegeIcon.SetActive(true);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playersWithBuildingPrivilege.Contains(other.GetComponent<PlayerProperties>()))
            {
                other.GetComponent<PlayerProperties>().hasBuildingPrivilege = false;
                other.GetComponent<PlayerProperties>().BuildingPrivilegeIcon.SetActive(false);
            }
        }
    }
}
