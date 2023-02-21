using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class JLGameManager : MonoBehaviourPunCallbacks
{
    public static JLGameManager Instance = null;


    public void Awake()
    {
        Instance = this;
        StartGame();
    }

    #region PUN CALLBACKS

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RemoveTagsFromOtherPlayers();
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("DemoAsteroids-LobbyScene");
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            
        }
    }

 

    #endregion

    private bool CheckAllPlayerLoadedLevel()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object playerLoadedLevel;

            if (p.CustomProperties.TryGetValue(JLGame.PLAYER_LOADED_LEVEL, out playerLoadedLevel))
            {
                if ((bool)playerLoadedLevel)
                {
                    continue;
                }
            }

            return false;
        }

        return true;
    }

    public void SpawnTestItems()
    {
        PhotonNetwork.Instantiate("Ak47", transform.position, transform.rotation, 0);
    }

    private void StartGame()
    {
        Debug.Log("StartGame!");
        Vector3 position = new Vector3(-9.8f, 4.0f, -3.2f);
        Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        GameObject player = null;

        player = PhotonNetwork.Instantiate("PlayerBean", position, rotation, 0);
        PhotonNetwork.LocalPlayer.TagObject = player;
        RemoveTagsFromOtherPlayers();
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate("Metal axe", position, rotation, 0);
            PhotonNetwork.Instantiate("FurnaceBall", position, rotation, 0);
        }

    }

    void RemoveTagsFromOtherPlayers()
    {
        GameObject[] PlayerList = GameObject.FindGameObjectsWithTag("Player");
        
        foreach (GameObject Player in PlayerList)
        {
            if (!Player.GetComponent<PhotonView>().IsMine)
            {
                Player.tag = "EnemyPlayer";
                Destroy(Player.transform.Find("Capsule").Find("Eyes").GetComponentInChildren<Camera>().gameObject);
                //Destroy(Player.GetComponent<PlayerProperties>());
                Destroy(Player.GetComponent<PlayerMovement>());
                //Destroy(Player.GetComponent<PlayerUseItem>());
                Destroy(Player.GetComponent<BuildingSystem>());
                Destroy(Player.transform.Find("Canvas").gameObject);
            }
        }

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        GameObject[] PlayerList = GameObject.FindGameObjectsWithTag("Player");
        if (PlayerList.Length > 1)
            RemoveTagsFromOtherPlayers();
    }
}
