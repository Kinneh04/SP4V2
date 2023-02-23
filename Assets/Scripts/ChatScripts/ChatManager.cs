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

    public List<ChatText> ChatTexts = new List<ChatText>();

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
        for (int i = 0; i < 10; i++)
        {
            ChatTexts.Add(Content.gameObject.transform.GetChild(i).GetComponent<ChatText>());
        }
        if (PV != null)
            PV.RPC("sendEveryoneMessage", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.NickName + " has joined.");


    }

    // Update is called once per frame
    void Update()
    {
        if (!PV.IsMine) return;
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
                input.gameObject.SetActive(false);
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
                                    {


                                        bool isReceiver = false;
                                        string Username;
                                        for (int player = 0; player < PhotonNetwork.PlayerListOthers.Length; player++)
                                        {
                                            isReceiver = true;
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
                                            PV.RPC("sendErrorMessage", RpcTarget.MasterClient, "/e Invalid Target", PhotonNetwork.LocalPlayer.NickName);
                                        }



                                        break;
                                    }
                                case 'g': // Give Items
                                    {


                                        break;
                                    }
                                
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
                    input.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                // Enter Chat Mode
                isTyping = true;
                input.gameObject.SetActive(true);
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
        if (PV.IsMine)
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

                    if (Message[0] == '/' && Message[1] == 'w')
                    {
                        // Check if Message is for this player

                        string receiver = "";
                        string sender = "";

                        for (int player = 0; player < PhotonNetwork.PlayerList.Length; player++)
                        {
                            sender = PhotonNetwork.PlayerList[player].NickName;

                            if (Message.Length + 3 < sender.Length)
                                break;

                            for (int j = 0; j < sender.Length; j++)
                            {
                                if (Message[j + 3] != sender[j])
                                {
                                    break;
                                }
                            }
                        }
                        for (int player = 0; player < PhotonNetwork.PlayerList.Length; player++)
                        {
                            receiver = PhotonNetwork.PlayerList[player].NickName;
                            if (receiver == sender)
                                break;

                            if (Message.Length + 3 + sender.Length < receiver.Length)
                                break;

                            for (int j = 0; j < receiver.Length; j++)
                            {
                                if (Message[j + sender.Length + 4] != receiver[j])
                                {
                                    break;
                                }
                            }
                        }
                        Message = "(Whisper) " + sender + ": " + Message.Substring(sender.Length + receiver.Length + 5);
                    }
                    else if (Message[0] == '/' && Message[1] == 'e')
                    {
                        string username = "";
                        for (int player = 0; player < PhotonNetwork.PlayerList.Length; player++)
                        {
                            username = PhotonNetwork.PlayerList[player].NickName;

                            if (Message.Length + 3 < username.Length)
                                break;

                            for (int j = 0; j < username.Length; j++)
                            {
                                if (Message[j + 3] != username[j])
                                {
                                    break;
                                }
                            }
                        }
                        Message = "(Error) " + Message.Substring(username.Length + 7);
                    }

                    Messages.Add(Message);
                    MessageStart = i + 1;
                    MessageCount++;
                }
            }
            bool over = false;
            if (MessageCount > 10)
            {
                Messages.RemoveRange(0, MessageCount - 10);
                MessageCount = 10;
                over = true;
            }

            if (over)
            {
                if (ChatTexts[9].CurrentText != Messages[9])
                {
                    for (int i = 0; i < 10; i++)
                    {
                        if (i < 9)
                        {
                            ChatTexts[i].ReplaceMessage(ChatTexts[i + 1]);
                            ChatTexts[i].gameObject.SetActive(!ChatTexts[i].fade);
                        }
                        else
                        {
                            ChatTexts[i].SetMessage(Messages[i]);
                            ChatTexts[i].gameObject.SetActive(!ChatTexts[i].fade);
                        }
                    }
                }
            }

            for (int i = 0; i < 10; i++)
            {
                if (i < MessageCount)
                {
                    ChatTexts[i].SetMessage(Messages[i]);
                    ChatTexts[i].gameObject.SetActive(!ChatTexts[i].fade);
                }
                else
                    ChatTexts[i].gameObject.SetActive(false);
            }
        }
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

    [PunRPC]
    void sendErrorMessage(string message, string recipient)
    {
        string ChatText = (string)PhotonNetwork.MasterClient.CustomProperties["ChatText"];
        ChatText += "/e " + recipient + " " + message + "\n";

        Hashtable hash = new Hashtable();
        hash.Add("ChatText", ChatText);
        PhotonNetwork.MasterClient.SetCustomProperties(hash);
    }
}
