using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockStructure : MonoBehaviour
{
    public bool hasPin;
    public int pin;
    public Light greenLight;
    public Light redLight;

    void Start()
    {
        hasPin = false;
        greenLight.enabled = true;
        redLight.enabled = false;
    }

    public void SetPin(int inputPin)
    {
        pin = inputPin;
        hasPin = true;
        
        greenLight.enabled = false;
        redLight.enabled = true;
    }

    public void RemovePin()
    {
        pin = 0;
        hasPin = false;

        greenLight.enabled = true;
        redLight.enabled = false;
    }
}
