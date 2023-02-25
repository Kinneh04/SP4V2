using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PinSystem : MonoBehaviour
{
    public PlayerMovement pm;
    public CreatePopup cp;
    public GameObject PinUI;
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
    }

    private void Update()
    {
        PinDisplay.text = currPin;
  //      if (Input.GetKey(KeyCode.Escape))
		//{
  //          CloseUI();
  //      }
    }

    public void StartCreatingPIN(LockStructure ls)
    {
        OpenUI(ls);
        removePinBtn.onClick.AddListener(delegate {
            ls.GetComponent<PhotonView>().RPC("RemovePin", RpcTarget.All);
            cp.CreateResourcePopup("Removed old PIN", 0);
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
        title.text = "Enter the PIN to the door";
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
                CloseUI();
            }
            else if (isEnteringPin)
            {
                if (ls.pin == int.Parse(currPin))
                {
                    ls.gameObject.transform.root.GetComponent<PhotonView>().RPC("SetIsOpen", RpcTarget.AllViaServer, true);
                    cp.CreateResourcePopup("PIN Correct", 0, true);
                    CloseUI();
                }
                else
                {
                    cp.CreateResourcePopup("PIN Incorrect", 0);
                    CloseUI();
                }
            }
            Cursor.lockState = CursorLockMode.Locked;
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
}
