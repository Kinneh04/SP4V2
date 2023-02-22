using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using Photon.Pun;
using ExitGames.Client.Photon;
using TMPro;

public class ChatManager : MonoBehaviour
{
    // ChatContent & InputField
    [SerializeField]
    TMP_Text Content;

    [SerializeField]
    GameObject UI;

    [SerializeField]
    GameObject Background;

    [SerializeField]
    TMP_InputField input;

    PhotonView PV;


    ChatClient chatClient;
    bool isConnected = false;
    public bool isTyping = false;

    // Start is called before the first frame update
    void Awake()
    {
        // When join game, enter chat with your game name 
        isConnected = true;
        PV = GetComponent<PhotonView>();
        Content.fontSize = 24;
    }

    // Update is called once per frame
    void Update()
    {
        if (isConnected)
        {
            UI.SetActive(true);
        }
        else
        {
            //Disconnect and dont back
        }
        Background.SetActive(isTyping);
        if (isTyping)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                isTyping = false;
                input.text = "";
                input.DeactivateInputField();
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                // Send Message here
                if (input.text != "")
                {
                    // Stay in Chat Mode
                    if (input.text[0] == '/')
                    {
                        if (input.text.Length > 1)
                        {
                            switch(input.text[1])
                            {
                                case 'w': // Whisper
                                    break;
                            }
                        }
                    }
                    else
                    {
                        if (PV.IsMine)
                        {
                            Debug.Log("Sending");
                            PV.RPC("sendEveryoneMessage", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName + ": " + input.text);
                        }
                    }
                    input.text = "";
                    input.ActivateInputField();
                }
                else
                {
                    // Leave Chat Mode
                    isTyping = false;
                    input.DeactivateInputField();
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                // Enter Chat Mode
                isTyping = true;
                input.ActivateInputField();
            }
        }
    }

    [PunRPC]
    void sendEveryoneMessage(string message)
    {
        Debug.Log(message);
        UpdateOwnChat(message);
    }

    void UpdateOwnChat(string message)
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + "Update Chat: " + Content.text);
        Content.text += "\n " + message;
    }

    [PunRPC]
    void sendWhisperMessage(string message, string recipient)
    {
        if (PhotonNetwork.LocalPlayer.NickName == recipient)
            Content.text = "\n " + PhotonNetwork.LocalPlayer.NickName + ": " + message;
    }
}
