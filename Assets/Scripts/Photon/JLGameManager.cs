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
    public GameObject SpawnPointMin, SpawnPointMax;
    public bool EnableRandomSpawn = false;

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
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene");
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
    private void StartGame()
    {
        Debug.Log("StartGame!");
        Vector3 position;
        if (EnableRandomSpawn)
        {
            float xPosition = Random.Range(SpawnPointMin.transform.position.x, SpawnPointMax.transform.position.x);
            float zPosition = Random.Range(SpawnPointMin.transform.position.z, SpawnPointMax.transform.position.z);
            position = new Vector3(xPosition, SpawnPointMax.transform.position.y, zPosition);
        }
        else
        {
            position = new Vector3(-9.8f, 4.0f, -3.2f);
        }
        Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        GameObject player = null;

        PhotonNetwork.Instantiate("AudioManager", new Vector3(0,0,0), new Quaternion(0,0,0,1));
        player = PhotonNetwork.Instantiate("PlayerBean", position, rotation, 0);

        RemoveTagsFromOtherPlayers();
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate("Food Crate", position, rotation, 0);
        }

        if(player.GetComponent<PhotonView>().IsMine)
        {
            SpawnItems();
        }

    }

    void SpawnItems()
    {
        GameObject Player = GameObject.FindGameObjectWithTag("Player");
        PhotonView Rockpv = PhotonNetwork.Instantiate("Rock", transform.position, Quaternion.identity, 0).GetComponent<PhotonView>();
        PhotonView TorchPV = PhotonNetwork.Instantiate("Torch", transform.position, Quaternion.identity, 0).GetComponent<PhotonView>();
        Rockpv.gameObject.SetActive(false);
        TorchPV.gameObject.SetActive(false);
        Rockpv.transform.SetParent(Player.transform.Find("Capsule").Find("RHand"));
        TorchPV.transform.SetParent(Player.transform.Find("Capsule").Find("RHand"));
        Player.GetComponentInChildren<InventoryManager>().AddQuantity(Rockpv.gameObject.GetComponent<HarvestToolsProperties>(), 1);
        Player.GetComponentInChildren<InventoryManager>().AddQuantity(TorchPV.gameObject.GetComponent<HarvestToolsProperties>(), 1);
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
                Destroy(Player.GetComponent<PlayerLookAt>());
                //Destroy(Player.GetComponent<PlayerUseItem>());
               // Destroy(Player.GetComponent<PlayerUseItem>());
                Destroy(Player.GetComponent<BuildingSystem>());
                //Destroy(Player.GetComponent<ChatManager>());
                Destroy(Player.GetComponent<AudioListener>());
                Destroy(Player.transform.Find("Canvas").gameObject);
                Destroy(Player.GetComponent<CraftingManager>());
                Player.GetComponentInChildren<Canvas>().gameObject.SetActive(false);
            }
     
        }

    }
    // Update is called once per frame
    void Update()
    {
        GameObject[] PlayerList = GameObject.FindGameObjectsWithTag("Player");
        if (PlayerList.Length > 1)
            RemoveTagsFromOtherPlayers();
    }
}
