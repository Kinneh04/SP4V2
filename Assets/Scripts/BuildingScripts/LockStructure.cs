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
    }

    public void SetPin(int inputPin)
    {
        pin = inputPin;
        hasPin = true;
        // todo activate light
    }
}
