using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public Light light;
    public float minIntensity = 0.25f;
    public float maxIntensity = 1f;
    public float flickerSpeed = 5f;

    void Start()
    {
        if (light == null)
        {
            light = GetComponent<Light>();
        }
    }

    void Update()
    {
        float flicker = Mathf.PerlinNoise(Time.time * flickerSpeed, 0);
        light.intensity = Mathf.Lerp(minIntensity, maxIntensity, flicker);
    }
}
