using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PinSystem : MonoBehaviour
{
    public AudioManager audioManager;
    public PlayerMovement pm;
    public CreatePopup cp;
    public GameObject PinUI;
    public PlayerProperties playerProperties;
    public List<Button> keypadBtns = new List<Button>();
    public Button removePinBtn;
    public TMPro.TMP_Text title;
    public TMPro.TMP_Text PinDisplay;
    private string currPin;
    private bool isSettingPin;
    private bool isEnteringPin;

    private void Awake()
    {
        currPin = "";
        isSettingPin = isEnteringPin = false;
        PinUI.SetActive(false);
        removePinBtn.gameObject.SetActive(false);

        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }

    private void Update()
    {
        PinDisplay.text = currPin;
        if (Input.GetKey(KeyCode.Escape) && PinUI && PinUI.activeSelf)
		{
            CloseUI();
        }
    }

    public void StartCreatingPIN(LockStructure ls)
    {
        OpenUI(ls);
        removePinBtn.onClick.AddListener(delegate {
            ls.GetComponent<PhotonView>().RPC("RemovePin", RpcTarget.All);
            cp.CreateResourcePopup("Removed old PIN", 0);
            audioManager.PlayAudio((int)AudioManager.AudioID.LockSuccess);
            CloseUI();
        });
        removePinBtn.gameObject.SetActive(true);
        title.text = "Set a PIN";
        isSettingPin = true;
    }

    public void StartEnteringPIN(LockStructure ls)
    {
        OpenUI(ls);
        removePinBtn.gameObject.SetActive(false);
        title.text = "Enter the PIN";
        isEnteringPin = true;
    }

    private void KeypadPress(string num, LockStructure ls)
    {
        currPin += num;

        if (currPin.Length == 4) // Finish entering pin
        {
            if (isSettingPin)
            {
                ls.GetComponent<PhotonView>().RPC("SetPin", RpcTarget.All, int.Parse(currPin));
                cp.CreateResourcePopup("Setting PIN success", 0, true);
                audioManager.PlayAudio((int)AudioManager.AudioID.LockSuccess);
                CloseUI();
            }
            else if (isEnteringPin)
            {
                if (ls.pin == int.Parse(currPin))
                {
                    if (ls.isTC)
                    {
                        UpdateBuildingPrivilege(ls.gameObject.transform.root.GetComponent<ToolCupboardProperties>());
                        CloseUI();
                    }
                    else
                    {
                        ls.gameObject.transform.root.GetComponent<PhotonView>().RPC("SetIsOpen", RpcTarget.AllViaServer, true);
                        cp.CreateResourcePopup("PIN Correct", 0, true);
                        audioManager.PlayAudio((int)AudioManager.AudioID.LockSuccess);
                        CloseUI();
                    }
                }
                else
                {
                    cp.CreateResourcePopup("PIN Incorrect", 0);
                    audioManager.PlayAudio((int)AudioManager.AudioID.LockFail);
                    CloseUI();
                }
            }
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            audioManager.PlayAudio((int)AudioManager.AudioID.LockBtnPress);
        }
    }

    private void OpenUI(LockStructure ls)
    {
        pm.canLookAround = false;
        PinUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;

        foreach (Button btn in keypadBtns)
        {
            if (btn.name == "C")
            {
                btn.onClick.AddListener(delegate { currPin = ""; });
            }
            else
            {
                btn.onClick.AddListener(delegate { KeypadPress(btn.name, ls); });
            }
        }
    }

    private void CloseUI()
    {
        pm.canLookAround = true;
        foreach (Button btn in keypadBtns)
        {
            btn.onClick.RemoveAllListeners();
        }
        removePinBtn.onClick.RemoveAllListeners();
        currPin = "";
        PinUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        isSettingPin = false;
        isEnteringPin = false;
    }

    private void UpdateBuildingPrivilege(ToolCupboardProperties tcp)
    {
        if (tcp.playersWithBuildingPrivilege.Contains(playerProperties))
        {
            playerProperties.hasBuildingPrivilege = false;
            playerProperties.BuildingPrivilegeIcon.SetActive(false);

            playerProperties.isBuildingDisabled = true;
            playerProperties.BuildingDisabledIcon.SetActive(true);

            audioManager.PlayAudio((int)AudioManager.AudioID.Click);
            cp.CreateResourcePopup("Build Privilege Removed", 0);
            tcp.playersWithBuildingPrivilege.Remove(playerProperties);

        }
        else
        {

            playerProperties.hasBuildingPrivilege = true;
            playerProperties.BuildingPrivilegeIcon.SetActive(true);

            playerProperties.isBuildingDisabled = false;
            playerProperties.BuildingDisabledIcon.SetActive(false);

            audioManager.PlayAudio((int)AudioManager.AudioID.LockSuccess);
            cp.CreateResourcePopup("Build Privilege Added", 0, true);
            tcp.playersWithBuildingPrivilege.Add(playerProperties);
        }
    }
}
