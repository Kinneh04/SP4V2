using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewObject : MonoBehaviour
{
    public List<Collider> col = new List<Collider>(); // List of objects colliding with object
    public List<Collider> nextToCol = new List<Collider>(); // TODO Add another trigger check
    public float collideLeeway = 3.0f;
    public ObjectTypes type;
    public Material validMat;
    public Material invalidMat;
    public bool IsBuildable;

    private void Start()
    {
        col.Clear();
        nextToCol.Clear();
    }
    void Update()
    {
        ChangeColor();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (type == ObjectTypes.foundation || type == ObjectTypes.stairs)
        {
            // Check if collided AND on the same level. Do not add if on different levels
            if (other.gameObject.layer == LayerMask.NameToLayer("Buildable") && other.gameObject.transform.position.y >= gameObject.transform.position.y - collideLeeway)
                col.Add(other);
        }
        else if (type == ObjectTypes.floor)
        {
            // Can only place next to objects, not including overlapping itself
            // Check PreviewChildCheck
            if (other.gameObject.layer == LayerMask.NameToLayer("Buildable") && other.gameObject.transform.position.y >= gameObject.transform.position.y - collideLeeway)
            {
                if (!other.gameObject.CompareTag("NormalStructure"))
                {
                    col.Add(other);
                    if (nextToCol.Contains(other))
                    {
                        nextToCol.Remove(other);
                    }
                }
            }
        }
        else if (type == ObjectTypes.door)
        {
            // Invalid if there is already a door placed at doorway
            if (other.gameObject.layer == LayerMask.NameToLayer("Buildable") && other.gameObject.CompareTag("DoorStructure"))
            {
                col.Add(other);
            }
        }
        else
        {
            // Normal objects can only be placed on terrain, foundation, or floor
            // Check PreviewChildCheck
            if (other.gameObject.layer == LayerMask.NameToLayer("Buildable") && other.gameObject.CompareTag("NormalStructure") && other.gameObject.transform.position.y >= gameObject.transform.position.y - collideLeeway)
            {
                col.Add(other);

                if (nextToCol.Contains(other))
                {
                    nextToCol.Remove(other);
                }
            }
        }

        // TODO: Redo stairs collision checks
    }

    private void OnTriggerExit(Collider other)
    {
        if (type == ObjectTypes.foundation || type == ObjectTypes.stairs)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Buildable"))
                col.Remove(other);
        }
        else if (type == ObjectTypes.floor)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Buildable"))
            {
                col.Remove(other);
            }
        }
        else if (type == ObjectTypes.door)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Buildable") && other.gameObject.CompareTag("DoorStructure"))
            {
                col.Remove(other);
            }
        }
        else
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Buildable") && other.gameObject.CompareTag("NormalStructure"))
            {
                col.Remove(other);
            }
        }
    }

    public void ChangeColor()
    {
        if (type == ObjectTypes.foundation || type == ObjectTypes.stairs)
        {
            // Disable if any collision
            if (col.Count == 0)
            {
                IsBuildable = true;
            }
            else
            {
                IsBuildable = false;
            }
        }
        else if (type == ObjectTypes.floor && nextToCol.Count > 0)
        {
            if (col.Count == 0)
            {
                IsBuildable = true;
            }
            else
            {
                IsBuildable = false;
            }
        }
        else if (type == ObjectTypes.door)
        {
            // Disable if any collision
            if (col.Count != 0)
            {
                IsBuildable = false;
            }
            // IsBuildable is set in BuildingSystem when aligned with Doorway
        }
        else
        {
            if (col.Count == 0 && nextToCol.Count > 0)
            {
                IsBuildable = true;
            }
            else
            {
                IsBuildable = false;
            }
        }

        // Switch colors
        if (IsBuildable)
        {
            foreach (Transform child in transform)
            {
                if (child.GetComponent<Renderer>() == null)
                    return;


                Material[] mats = child.GetComponent<Renderer>().materials;
                mats[0] = validMat;
                child.GetComponent<Renderer>().materials = mats;
            }
        }
        else
        {
            foreach (Transform child in transform)
            {
                if (child.GetComponent<Renderer>() == null)
                    return;

                Material[] mats = child.GetComponent<Renderer>().materials;
                mats[0] = invalidMat;
                child.GetComponent<Renderer>().materials = mats;
            }
        }
    }
}

public enum ObjectTypes
{
    normal,
    foundation,
    floor,
    stairs,
    door
}
