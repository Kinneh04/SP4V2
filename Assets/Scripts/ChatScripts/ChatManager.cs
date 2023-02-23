using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using Photon.Pun;
using ExitGames.Client.Photon;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

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
        if (PV != null)
            PV.RPC("sendEveryoneMessage", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.NickName + " has joined.");


    }

    // Update is called once per frame
    void Update()
    {
        PV.RPC("UpdateOwnChat", RpcTarget.All);

        if (isConnected)
        {
            UI.SetActive(true);
        }
        else
        {
            //Disconnect and dont back
            PV.RPC("sendEveryoneMessage", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.NickName + " has left.");
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

                                    
                                    bool isReceiver = true;
                                    string Username;
                                    for (int player = 0; player < PhotonNetwork.PlayerListOthers.Length; player++)
                                    {
                                        Username = PhotonNetwork.PlayerListOthers[player].NickName;
                                        if (input.text.Length + 5 >= Username.Length) // Check if there is also a message attached in the case where the user's name is really there
                                        {
                                            for (int i = 0; i < Username.Length; i++)
                                            {
                                                if (input.text[i + 3] != Username[i])
                                                {
                                                    isReceiver = false;
                                                    break;
                                                }
                                            }
                                            if (isReceiver)
                                            {
                                                PV.RPC("sendWhisperMessage", RpcTarget.MasterClient, input.text.Substring(4 + Username.Length), Username, PhotonNetwork.LocalPlayer.NickName);
                                                break;
                                            }
                                        }
                                    }
                                    if (!isReceiver)
                                    {
                                        // Couldnt Send Whisper
                                    }

                                    

                                    break;
                            }
                        }
                    }
                    else
                    {
                        if (PV.IsMine)
                        {
                            Debug.Log("Sending");
                            PV.RPC("sendEveryoneMessage", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.NickName + ": " + input.text);
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
    public void sendEveryoneMessage(string message)
    {
        string ChatText = (string)PhotonNetwork.MasterClient.CustomProperties["ChatText"];
        ChatText += message + "\n";

        Hashtable hash = new Hashtable();
        hash.Add("ChatText", ChatText);
        PhotonNetwork.MasterClient.SetCustomProperties(hash);
    }

    [PunRPC]
    public void UpdateOwnChat()
    {
        string ChatText = (string)PhotonNetwork.MasterClient.CustomProperties["ChatText"];

        // Gets the 10 Most Recent Messages
        List<string> Messages = new List<string>();
        int MessageCount = 0;
        int MessageStart = 0;
        for (int i = 0; i < ChatText.Length; i++)
        {
            if (ChatText[i] == '\n')
            {
                string Message = ChatText.Substring(MessageStart, i - MessageStart + 1);
                // Check if Message is for this player
                string Username = PhotonNetwork.LocalPlayer.NickName;
                bool isReceiver = true;
                if (Message[0] == '/' && Message[1] == 'w' && Message.Length - 5 >= Username.Length)
                {
                    for (int j = 0; j < Username.Length; j++)
                    {
                        if (Message[j + 3] != Username[j])
                        {
                            isReceiver = false;
                        }
                    }
                    if (!isReceiver)
                        break;
                    Message = "(Whisper) " + Username + ": " + Message.Substring(Username.Length + 4, Message.Length - Username.Length - 4);
                }
                Messages.Add(Message);
                MessageStart = i + 1;
                MessageCount++;
            }
        }
        if (MessageCount > 10)
            Messages.RemoveRange(0, MessageCount - 10);

        Content.text = "";
        for (int i = 0; i < Messages.Count; i++)
            Content.text += Messages[i];

        //Content.text = ChatText;
    }

    [PunRPC]
    void sendWhisperMessage(string message, string recipient, string sender)
    {
        string ChatText = (string)PhotonNetwork.MasterClient.CustomProperties["ChatText"];
        ChatText += "/w " + sender + " " + recipient + " " + message + "\n";

        Hashtable hash = new Hashtable();
        hash.Add("ChatText", ChatText);
        PhotonNetwork.MasterClient.SetCustomProperties(hash);
    }
}
