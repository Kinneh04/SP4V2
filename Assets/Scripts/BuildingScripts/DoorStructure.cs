using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DoorStructure : MonoBehaviour
{
    public int PlayerID;
    public bool isOpen;
    public bool hasLock;
    public GameObject lockObject;

    private float currRotation;
    private bool isOpening;
    private bool isClosing;

    private void Start()
    {
        isOpen = false;
        hasLock = false;
        isOpening = isClosing = false;
        currRotation = 0;
        //lockObject.SetActive(false);
    }

    private void Update()
    {
        if (isOpening)
        {
            if (currRotation > -90)
            {
                gameObject.transform.Rotate(0, -2, 0);
                currRotation -= 2;
            }
            else
            {
                isOpening = false;
            }
        }
        else if (isClosing)
        {
            if (currRotation < 0)
            {
                gameObject.transform.Rotate(0, 2, 0);
                currRotation += 2;
            }
            else
            {
                isClosing = false;
            }
        }
    }
    
    [PunRPC]
    public void SetIsOpen(bool open)
    {
        isOpen = open;
        // Rotate doors animation
        if (isOpen)
        {
            isClosing = false;
            isOpening = true;
        }
        else
        {
            isOpening = false;
            isClosing = true;
        }
    }

    [PunRPC]
    public void SetHasLock(bool locked)
    {
        hasLock = locked;
        if (hasLock)
        {
            lockObject.SetActive(true);
        }
        else
        {
            lockObject.SetActive(false);
        }
    }
}
