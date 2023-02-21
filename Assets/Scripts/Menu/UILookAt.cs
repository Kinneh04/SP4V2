using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class UILookAt : MonoBehaviour
{
    private Camera cam;
    public PhotonView pv;
    private bool hasFlipped;

    private void Start()
    {
        cam = GameObject.FindGameObjectWithTag("Camera").GetComponent<Camera>();
        hasFlipped = false;
    }
    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
        if (transform.parent.CompareTag("NormalStructure"))
        {
            switch(transform.parent.transform.rotation.eulerAngles.y)
            {
                case 0:
                    {
                        if (transform.parent.transform.position.z < cam.transform.position.z && !hasFlipped)
                        {
                            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1.2f);
                            hasFlipped = true;
                        }
                        else if (hasFlipped && transform.parent.transform.position.z > cam.transform.position.z)
                        {
                            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1.2f);
                            hasFlipped = false;
                        }
                        break;
                    }
                case 90:
                    {
                        if (transform.parent.transform.position.x < cam.transform.position.x && !hasFlipped)
                        {
                            transform.position = new Vector3(transform.position.x + 1.2f, transform.position.y, transform.position.z);
                            hasFlipped = true;
                        }
                        else if (hasFlipped && transform.parent.transform.position.x > cam.transform.position.x)
                        {
                            transform.position = new Vector3(transform.position.x - 1.2f, transform.position.y, transform.position.z);
                            hasFlipped = false;
                        }
                        break;
                    }
                case 180:
                    {
                        if (transform.parent.transform.position.z > cam.transform.position.z && !hasFlipped)
                        {
                            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1.2f);
                            hasFlipped = true;
                        }
                        else if (hasFlipped && transform.parent.transform.position.z < cam.transform.position.z)
                        {
                            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1.2f);
                            hasFlipped = false;
                        }
                        break;
                    }
                case 270:
                    {
                        if (transform.parent.transform.position.x > cam.transform.position.x && !hasFlipped)
                        {
                            transform.position = new Vector3(transform.position.x - 1.2f, transform.position.y, transform.position.z);
                            hasFlipped = true;
                        }
                        else if (hasFlipped && transform.parent.transform.position.x < cam.transform.position.x)
                        {
                            transform.position = new Vector3(transform.position.x + 1.2f, transform.position.y, transform.position.z);
                            hasFlipped = false;
                        }
                        break;
                    }
                default:
                    break;
            }
        }
        else if (transform.parent.CompareTag("FloorStructure") || transform.parent.CompareTag("FoundationStructure"))
        {
            if (transform.parent.transform.position.y > cam.transform.position.y && !hasFlipped)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - 1.0f, transform.position.z);
                hasFlipped = true;
            }
            else if (hasFlipped && transform.parent.transform.position.y < cam.transform.position.y)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);
                hasFlipped = false;
            }
        }
    }
}
