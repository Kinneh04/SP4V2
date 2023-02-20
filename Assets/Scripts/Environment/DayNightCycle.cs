using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DayNightCycle : MonoBehaviour
{
    public Light directionalLight;
    public float secondsInFullDay = 120f;
    [Range(0, 1)]
    public float currentTimeOfDay = 0.25f;
    public float timeMultiplier = 1f;

    public float timeOfDay;


    private void Start()
    {
        timeOfDay = currentTimeOfDay;
        RenderSettings.ambientMode = AmbientMode.Skybox;
    }
    private void FixedUpdate()
    {
        UpdateTimeOfDay();
        UpdateDirectionalLight();
    }

    private void UpdateTimeOfDay()
    {
        timeOfDay += Time.deltaTime / secondsInFullDay * timeMultiplier;
        timeOfDay %= 1;
    }

    private void UpdateDirectionalLight()
    {
        float sunAngle = Mathf.Lerp(-90f, 270f, timeOfDay);
        directionalLight.transform.localRotation = Quaternion.Euler(sunAngle, 0, 0);

        float lightIntensity = Mathf.Clamp01(1 - Mathf.Abs(timeOfDay - 0.5f) * 2);

        if (timeOfDay > 0.45 || timeOfDay < 0.7)
            directionalLight.intensity = lightIntensity * 2;
        else directionalLight.intensity = lightIntensity;

        float lerpAmount = Mathf.Clamp01(Mathf.Abs(timeOfDay - 0.5f) * 2);
        Color currentColor = Color.Lerp(Color.white, Color.black, lerpAmount);

        if (currentColor.r < 45) currentColor -= new Color(10, 10, 10, 0);
        //print(currentColor);
        
        RenderSettings.ambientLight = currentColor;
       
    }
}
