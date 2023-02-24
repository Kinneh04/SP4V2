using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePopup : MonoBehaviour
{
    public GameObject popupPrefab;
    public GameObject canvasParent;

    public void CreateResourcePopup(string resource, int amt, bool isGreen = false)
    {
        GameObject popup = Instantiate(popupPrefab, new Vector3(562, 43.8f, 0), Quaternion.identity);
        popup.GetComponent<ResourceUsagePopup>().resourceText.text = resource;
        if (amt == 0)
        {
            popup.GetComponent<ResourceUsagePopup>().amtText.text = "";
        }
        else
        {
            popup.GetComponent<ResourceUsagePopup>().amtText.text = "-" + amt;
        }

        if (isGreen)
            popup.GetComponent<ResourceUsagePopup>().resourceBG.color = popup.GetComponent<ResourceUsagePopup>().greenColor;
        popup.transform.SetParent(canvasParent.transform, false);
    }
}
