using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewChildCheck : MonoBehaviour
{
    public PreviewObject po;

    private void OnTriggerEnter(Collider other)
    {
        if (po.type == ObjectTypes.floor)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Buildable") 
                && other.gameObject.transform.position.y >= gameObject.transform.position.y - po.collideLeeway
                && !po.col.Contains(other))
            {
                po.nextToCol.Add(other);
            }
        }
        else if (po.type == ObjectTypes.normal)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Buildable")
                && other.gameObject.transform.position.y >= gameObject.transform.position.y - po.collideLeeway
                && !other.gameObject.CompareTag("NormalStructure")
                && !po.col.Contains(other))
            {
                po.nextToCol.Add(other);
            }
        }
        else if (po.type == ObjectTypes.door)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Buildable")
                && other.gameObject.transform.position.y >= gameObject.transform.position.y - po.collideLeeway
                && other.gameObject.CompareTag("NormalStructure"))
            {
                po.nextToCol.Add(other);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (po.type == ObjectTypes.floor)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Buildable") && po.nextToCol.Contains(other))
            {
                po.nextToCol.Remove(other);
            }
        }
        else if (po.type == ObjectTypes.normal)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Buildable") && po.nextToCol.Contains(other))
            {
                po.nextToCol.Remove(other);
            }
        }
        else if (po.type == ObjectTypes.door)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Buildable") && po.nextToCol.Contains(other))
            {
                po.nextToCol.Remove(other);
            }
        }
    }
}
