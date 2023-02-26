using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ToolCupboardUI : MonoBehaviour
{
    private ToolCupboardProperties currentTC = null;
    public Button buildingPrivilegeButton;
    public Color btnOnColor;
    public Color btnOffColor;
    public TMPro.TMP_Text woodText;
    public TMPro.TMP_Text stoneText;
    public TMPro.TMP_Text timeLeft;
    public TMPro.TMP_Text decayingText;
    public CreatePopup cp;

    void Update()
    {
        if (!currentTC)
            return;

        woodText.text = currentTC.woodReq.ToString();
        stoneText.text = currentTC.stoneReq.ToString();

        if (currentTC.isProtected)
        {
            decayingText.gameObject.SetActive(false);
            timeLeft.gameObject.SetActive(true);

            timeLeft.text = "Protected for " + TimeSpan.FromSeconds(currentTC.protectionTimeLeft).ToString("%h'h '%m'm '%s's '");
        }
    }

    public void OpenUI(ToolCupboardProperties tc, PlayerProperties currPlayerProperties)
    {
        currentTC = tc;
        buildingPrivilegeButton.onClick.AddListener(delegate {
            currentTC.ToggleBuildingPrivilege(currPlayerProperties);
            if (currPlayerProperties.hasBuildingPrivilege)
            {
                cp.CreateResourcePopup("Build Privilege Added", 0, true);
                buildingPrivilegeButton.image.color = btnOnColor;
                buildingPrivilegeButton.GetComponentInChildren<TMPro.TMP_Text>().text = "Building Privilege ON";
            }
            else
            {
                cp.CreateResourcePopup("Build Privilege Removed", 0);
                buildingPrivilegeButton.image.color = btnOffColor;
                buildingPrivilegeButton.GetComponentInChildren<TMPro.TMP_Text>().text = "Building Privilege OFF";
            }
        });

        // Also update privilege
        if (currPlayerProperties.hasBuildingPrivilege)
        {
            buildingPrivilegeButton.image.color = btnOnColor;
            buildingPrivilegeButton.GetComponentInChildren<TMPro.TMP_Text>().text = "Building Privilege ON";
        }
        else
        {
            buildingPrivilegeButton.image.color = btnOffColor;
            buildingPrivilegeButton.GetComponentInChildren<TMPro.TMP_Text>().text = "Building Privilege OFF";
        }
    }

    public void CloseUI()
    {
        currentTC = null;
        buildingPrivilegeButton.onClick.RemoveAllListeners();
    }
}
