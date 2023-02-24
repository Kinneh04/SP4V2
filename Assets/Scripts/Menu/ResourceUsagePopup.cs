using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceUsagePopup : MonoBehaviour
{
    public TMPro.TMP_Text resourceText;
    public TMPro.TMP_Text amtText;
    public Image resourceBG;
    public Color greenColor;
    private float fadeOutTime;

    private void Start()
    {
        GetComponent<CanvasGroup>().alpha = 1;
        fadeOutTime = 1.5f;
    }

    private void Update()
    {
        if (fadeOutTime <= 0.0f)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            gameObject.GetComponent<CanvasGroup>().alpha = (float)Math.Round(fadeOutTime / 1.5f, 1);
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + Time.deltaTime * 8, gameObject.transform.position.z); ;
            fadeOutTime -= Time.deltaTime;
        }
    }
}
