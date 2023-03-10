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
                if (other.gameObject.CompareTag("NormalStructure"))
                {
                }
                else
                {
                    col.Add(other);
                    if (nextToCol.Contains(other))
                    {
                        nextToCol.Remove(other);
                    }
                }
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
        /*if (other.gameObject.layer == LayerMask.NameToLayer("Buildable") && (type == ObjectTypes.foundation || type == ObjectTypes.floor)) {
            col.Add(other);
        }*/
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
        else
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Buildable") && other.gameObject.CompareTag("NormalStructure"))
            {
                col.Remove(other);
            }
        }

        /*if (other.gameObject.layer == LayerMask.NameToLayer("Buildable") && (type == ObjectTypes.foundation || type == ObjectTypes.floor)) {
            col.Remove(other);
        }*/
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

        /* if (type == ObjectTypes.floor)
         {
             // Cannot stack floors on each other

         }
         else
         {
             if (col.Count == 0)
             {
                 IsBuildable = true;
             }
             else
             {
                 IsBuildable = false;
             }
         }*/

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
    stairs
}
