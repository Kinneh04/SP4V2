using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LockStructure : MonoBehaviour
{
    public bool hasPin;
    public int pin;
    public Light greenLight;
    public Light redLight;

    public bool isTC = false;
    public int ownerID;

    void Start()
    {
        hasPin = false;
        greenLight.enabled = true;
        redLight.enabled = false;
    }

    [PunRPC]
    public void SetPin(int inputPin)
    {
        pin = inputPin;
        hasPin = true;
        
        greenLight.enabled = false;
        redLight.enabled = true;
    }

    [PunRPC]
    public void RemovePin()
    {
        pin = 0;
        hasPin = false;

        greenLight.enabled = true;
        redLight.enabled = false;
    }
}
