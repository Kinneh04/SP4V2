using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class PlayerUseItem : MonoBehaviour
{
    public PlayerProperties playerProperties;
    public GameObject RHand;
    public GameObject Inventory;
    public InventoryManager inventoryManager;
    public Camera Cam;
    public Image imageToFade;
    public GameObject sniperScopeImage;
    public float fadeDuration = 0.2f;
    public float scopeDelay = 0.2f;

    public Animator PAnimator;
    bool LeftMouseButtonPressed = false;   //click once only
    public float timer = 0.0f;
    float cooldowntimer = 0.0f;
    bool canuse = true;
    float animtimer = 0.0f;
    public BuildingSystem bs;
    public HammerSystem hs;
    public PinSystem ps;
    public bool isPlacingItem;
    public bool isADS;
    public float zoomFactor = 2f;
    private bool isZoomedIn = false;
    public GameObject Map;
    public GameObject pinEntry;

    public PlayerLookAt playerLookAt;
    public bool isReleased = true;
    private bool holdingCodeLock = false;
    private GameObject currDoor = null;
    public Material ghostMat;

    PhotonView pv;
    private void Awake()
    {
        inventoryManager = Inventory.GetComponent<InventoryManager>();
        pv = GetComponent<PhotonView>();
    }


    private IEnumerator FadeToBlack()
    {
        // Fade the image to black quickly
        float elapsedTime = 0;
        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            imageToFade.color = new Color(0, 0, 0, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Wait for a short delay
        yield return new WaitForSeconds(scopeDelay);
        // Enable the sniper scope image
        if (Input.GetMouseButton(1))
        {
            sniperScopeImage.SetActive(true);
            Cam.fieldOfView = 15;
        }
        // Show the sniper scope image by fading out the black image
        elapsedTime = 0;
        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            imageToFade.color = new Color(0, 0, 0, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

      
    }

    [PunRPC]
    void ShoveNewItemInRHandOfActor(int ItemView, int ActorNumber)
    {
        GameObject ItemToPairToHand;

        //Player player = PhotonNetwork.CurrentRoom.GetPlayer(ActorNumber);
        // Get the PhotonView component for the player object
        PhotonView ActorPV = PhotonView.Find(ActorNumber);
        GameObject Actor = ActorPV.gameObject;

        //GameObject Actor = ActorView.TagObject as GameObject;

        GameObject RHand = Actor.transform.Find("Capsule").Find("RHand").gameObject;

        ItemToPairToHand = PhotonView.Find(ItemView).gameObject;

        if (ItemToPairToHand != null)
        {
            ItemToPairToHand.transform.position = RHand.transform.position;
            ItemToPairToHand.transform.rotation = RHand.transform.rotation;
            ItemToPairToHand.SetActive(false);
            ItemToPairToHand.transform.SetParent(RHand.transform);
            ItemToPairToHand.GetComponent<Rigidbody>().isKinematic = true;
        }
        else
        {
            print("IT GONE MN!");
        }
    }

    private void Update()
    {
        if (GetComponent<ChatManager>().isTyping)
            return;
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            playerProperties.OpenInventory();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            playerProperties.OpenCrafting(0);
        }
        if(Input.GetKeyDown(KeyCode.M))
        {
            Map.SetActive(!Map.activeSelf);
        }
        if (!playerProperties.isDead && Cursor.lockState == CursorLockMode.Locked)
        {
            if (holdingCodeLock)
            {
                // When looking at a door, show ghost of codelock
                if (playerProperties.PlayerLookingAtItem != null && playerProperties.PlayerLookingAtItem.tag == "DoorStructure")
                {
                    if (playerProperties.PlayerLookingAtItem != currDoor)
                    {
                        DoorStructure ds = null;
                        if (playerProperties.PlayerLookingAtItem.gameObject.layer == LayerMask.NameToLayer("BuildableParent"))
                        {
                            ds = playerProperties.PlayerLookingAtItem.GetComponent<DoorStructure>();
                        }
                        else if (playerProperties.PlayerLookingAtItem.gameObject.layer == LayerMask.NameToLayer("Buildable"))
                        {
                            ds = playerProperties.PlayerLookingAtItem.GetComponentInParent<DoorStructure>();
                        }

                        if (ds && !ds.hasLock && ds.PlayerID == PhotonNetwork.LocalPlayer.ActorNumber)
                        {
                            // Set inactive for previous door
                            if (currDoor != null)
                            {
                                ResetCodelockGhost();
                            }

                            ds.lockObject.SetActive(true);
                            Material[] mats = ds.lockObject.transform.GetComponent<Renderer>().materials;
                            Material[] newMats = { mats[0], ghostMat };
                            ds.lockObject.transform.GetComponent<Renderer>().materials = newMats;

                            currDoor = playerProperties.PlayerLookingAtItem;
                        }
                        else if (currDoor != null)
                        {
                            ResetCodelockGhost();
                        }
                    }
                }
                else if (currDoor != null)
                {
                    ResetCodelockGhost();
                }
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                //PAnimator.Play("PBeanIdle");
                pv.RPC("PlayServerSideAnimation", RpcTarget.All, pv.ViewID, "PBeanIdle");

                isPlacingItem = false;
                if (playerProperties.PlayerLookingAtItem != null && playerProperties.PlayerLookingAtItem.tag == "Crate")
                {
                    playerProperties.PlayerLookingAtItem.GetComponent<LootProperties>().IM = inventoryManager;
                    playerProperties.PlayerLookingAtItem.GetComponent<LootProperties>().DisplayLoot();
                    playerProperties.OpenLootInventory();
                }
                else if (playerProperties.PlayerLookingAtItem != null && playerProperties.PlayerLookingAtItem.tag == "Campfire")
                {
                    playerProperties.PlayerLookingAtItem.GetComponent<FurnaceProperties>().IM = inventoryManager;
                    playerProperties.PlayerLookingAtItem.GetComponent<FurnaceProperties>().DisplayLoot();
                    playerProperties.OpenFurnaceInventory();
                }

                else if (playerProperties.PlayerLookingAtItem != null && playerProperties.PlayerLookingAtItem.tag == "DoorStructure")
                {
                    DoorStructure ds = null;
                    if (playerProperties.PlayerLookingAtItem.gameObject.layer == LayerMask.NameToLayer("BuildableParent"))
                    {
                        ds = playerProperties.PlayerLookingAtItem.GetComponent<DoorStructure>();
                    }
                    else if (playerProperties.PlayerLookingAtItem.gameObject.layer == LayerMask.NameToLayer("Buildable"))
                    {
                        ds = playerProperties.PlayerLookingAtItem.GetComponentInParent<DoorStructure>();
                    }
                    if (!ds)
                        return;

                    if (ds.PlayerID == PhotonNetwork.LocalPlayer.ActorNumber)
                    {
                        if (ds.hasLock) 
                        {
                            // Open PIN entry
                            if (ds.lockObject.GetComponent<LockStructure>().hasPin)
                            {

                            }
                            else // No pin set, open normally
                            {
                                ds.SetIsOpen(!ds.isOpen);
                            }
                        }
                        else
                        {
                            ds.SetIsOpen(!ds.isOpen);
                        }
                    }
                }

                else if (playerProperties.PlayerLookingAtItem != null && playerProperties.PlayerLookingAtItem.GetComponent<ItemInfo>() != null)
                {
                        if (playerProperties.PlayerLookingAtItem.transform.parent != null && playerProperties.PlayerLookingAtItem.transform.parent.tag == "RHand") return;
                    
                        ItemInfo.ItemType GO_Type = playerProperties.PlayerLookingAtItem.GetComponent<ItemInfo>().GetItemType();
                        if (GO_Type == ItemInfo.ItemType.Bush)
                        {
                            playerProperties.PlayerLookingAtItem.GetComponent<BushProperties>().Pick();
                        }
                        else if (playerProperties.PlayerLookingAtItem.tag == "SleepingPoint" && playerProperties.PlayerLookingAtItem.GetComponent<SleepingBagProperties>().isUsed == false)
                        {
                            print("SET TO USED BAG!");
                            playerProperties.Spawnpoint = playerProperties.PlayerLookingAtItem.transform.position;
                            playerProperties.Lastbedclaimed = playerProperties.PlayerLookingAtItem;
                            playerProperties.PlayerLookingAtItem.GetComponent<SleepingBagProperties>().isUsed = true;
                        }
                        else
                        {
                            GameObject GO;
                            if (GO_Type == ItemInfo.ItemType.unshowable)
                            {
                                print("UNSHOWABLE!");
                                GO = playerProperties.PlayerLookingAtItem.GetComponent<ItemInfo>().ReplacementObj;


                                string GOName = GO.name;
                                GO.GetComponent<Rigidbody>().isKinematic = true;
                                GameObject GO_Dupe = Instantiate(GO, transform.position, Quaternion.identity);
                                GO_Dupe.GetComponent<ItemInfo>().NetworkedReplacement = true;
                                GO_Dupe.name = GOName;
                               
                                inventoryManager.AddQuantity(GO_Dupe.GetComponent<ItemInfo>(), 1);
                                Destroy(playerProperties.PlayerLookingAtItem);

                                if (GO_Dupe.GetComponent<PhotonView>() != null && pv.IsMine)
                                    pv.RPC("ShoveNewItemInRHandOfActor", RpcTarget.All, GO_Dupe.GetComponent<PhotonView>().ViewID, pv.ViewID);

                                if (inventoryManager.InventoryList[inventoryManager.EquippedSlot].itemID == GO_Dupe.GetComponent<ItemInfo>().itemID)
                                {
                                    isPlacingItem = true;
                                    GO_Dupe.SetActive(true);
                                }
                                else
                                {
                                    GO_Dupe.SetActive(false);
                                }
                                
                                  inventoryManager.UpdateItemCountPerSlot();  
                            }
                            else
                            {
                                if (playerProperties.CurrentlyHoldingItem != null)
                                {
                                  /*if (playerProperties.CurrentlyHoldingItem.GetComponent<ItemInfo>().GetItemType() == ItemInfo.ItemType.BuildPlan)
                                    {
                                        bs.SetIsBuilding(false);
                                    }
                                    else if (playerProperties.CurrentlyHoldingItem.GetComponent<ItemInfo>().GetItemType() == ItemInfo.ItemType.Hammer)
                                    {
                                        hs.SetIsUsingHammer(false);
                                    }*/
                                }
                                else
                                {
                                    if (GO_Type == ItemInfo.ItemType.BuildPlan)
                                    {
                                        bs.SetIsBuilding(true);
                                    }
                                    else if (GO_Type == ItemInfo.ItemType.Hammer)
                                    {
                                        hs.SetIsUsingHammer(true);
                                    }
                                    else if (GO_Type == ItemInfo.ItemType.CodeLock)
                                    {
                                        holdingCodeLock = true;
                                    }
                                }

                                if (playerProperties.PlayerLookingAtItem.GetComponent<PhotonView>() != null && pv.IsMine)
                                    pv.RPC("ShoveNewItemInRHandOfActor", RpcTarget.All, playerProperties.PlayerLookingAtItem.GetComponent<PhotonView>().ViewID, pv.ViewID);
                                else Debug.LogError("Custom error: Current Item has no PhotonView component. Cannot be displayed server side");
                                inventoryManager.AddQuantity(playerProperties.PlayerLookingAtItem.GetComponent<ItemInfo>(), playerProperties.PlayerLookingAtItem.GetComponent<ItemInfo>().ItemCount);
                                inventoryManager.UpdateItemCountPerSlot();
                                playerProperties.PlayerLookingAtItem = null;
                            }
                        }
                    

                }
                else if (playerProperties.PlayerLookingAtItem != null && playerProperties.PlayerLookingAtItem.tag == "Workbench")
                {
                    // Input Level
                    playerProperties.OpenCrafting(playerProperties.PlayerLookingAtItem.GetComponent<Workbench>().Level);
                }
                else if (playerProperties.PlayerLookingAtItem != null && playerProperties.PlayerLookingAtItem.tag == "ResearchTable")
                {
                    // Input Level
                    playerProperties.OpenResearch();
                }
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                GameObject ItemGO = playerProperties.CurrentlyHoldingItem;
                if (isPlacingItem)
                {
                    ItemGO.transform.rotation = Quaternion.Euler(ItemGO.transform.rotation.x, ItemGO.transform.rotation.y + 90, ItemGO.transform.rotation.z);
                }
                else
                {

                    if (ItemGO.GetComponent<ItemInfo>().GetItemType() == ItemInfo.ItemType.Ranged)
                    {
                        if (ItemGO.GetComponent<WeaponInfo>().Reload())
                        {
                            if (ItemGO.GetComponent<WeaponInfo>().GetGunName() == WeaponInfo.GUNNAME.M1911_PISTOL && !LeftMouseButtonPressed)
                            {
                                //PAnimator.Play("PBeanReloadM1911");
                                pv.RPC("PlayServerSideAnimation", RpcTarget.All, pv.ViewID, "PBeanReloadM1911");
                            }
                            else if (ItemGO.GetComponent<WeaponInfo>().GetGunName() == WeaponInfo.GUNNAME.REVOLVER && !LeftMouseButtonPressed)
                            {
                                //PAnimator.Play("PBeanRevolverReload");
                                pv.RPC("PlayServerSideAnimation", RpcTarget.All, pv.ViewID, "PBeanRevolverReload");
                            }
                            else if (ItemGO.GetComponent<WeaponInfo>().GetGunName() == WeaponInfo.GUNNAME.AK47 && !LeftMouseButtonPressed)
                            {
                                // PAnimator.Play("PBeanReloadAK");
                                pv.RPC("PlayServerSideAnimation", RpcTarget.All, pv.ViewID, "PBeanReloadAK");
                            }
                            else if (ItemGO.GetComponent<WeaponInfo>().GetGunName() == WeaponInfo.GUNNAME.HOMEMADE_SHOTGUN && !LeftMouseButtonPressed)
                            {
                                //PAnimator.Play("PBeanReloadShotgun");
                                pv.RPC("PlayServerSideAnimation", RpcTarget.All, pv.ViewID, "PBeanReloadShotgun");
                            }
                            else if (ItemGO.GetComponent<WeaponInfo>().GetGunName() == WeaponInfo.GUNNAME.MP5A4 && !LeftMouseButtonPressed)
                            {
                                // PAnimator.Play("PBeanSMGReload");
                                pv.RPC("PlayServerSideAnimation", RpcTarget.All, pv.ViewID, "PBeanSMGReload");
                            }
                            else if (ItemGO.GetComponent<WeaponInfo>().GetGunName() == WeaponInfo.GUNNAME.REMINGTON870 && !LeftMouseButtonPressed)
                            {
                                //PAnimator.Play("PBeanShotgunReload");
                                pv.RPC("PlayServerSideAnimation", RpcTarget.All, pv.ViewID, "PBeanShotgunReload");
                            }
                            else if (ItemGO.GetComponent<WeaponInfo>().GetGunName() == WeaponInfo.GUNNAME.BOLT_ACTION_RIFLE && !LeftMouseButtonPressed)
                            {
                                //PAnimator.Play("PBeanSniperReload");
                                pv.RPC("PlayServerSideAnimation", RpcTarget.All, pv.ViewID, "PBeanSniperReload");
                            }
                            else if (ItemGO.GetComponent<WeaponInfo>().GetGunName() == WeaponInfo.GUNNAME.ROCKETLAUNCHER && !LeftMouseButtonPressed)
                            {
                                //PAnimator.Play("PBeanSniperReload");
                                pv.RPC("PlayServerSideAnimation", RpcTarget.All, pv.ViewID, "PBeanReloadRocketLauncher");
                            }
                        }
                    }
                }
            }

            if(Input.GetKeyDown(KeyCode.Backspace))
            {
                if (inventoryManager.InventoryList[inventoryManager.EquippedSlot])
                    inventoryManager.RemoveQuantityFromSlot(inventoryManager.EquippedSlot, 1);
                Destroy(playerProperties.CurrentlyHoldingItem);
                playerProperties.CurrentlyHoldingItem = null;
            }
            if (!canuse)
            {
                cooldowntimer -= Time.deltaTime;
                if (cooldowntimer <= 0)
                {
                    canuse = true;
                }
            }
            if (Input.GetMouseButton(0) && playerProperties.CurrentlyHoldingItem)
            {
                GameObject ItemGO = playerProperties.CurrentlyHoldingItem;
                ItemInfo.ItemType GO_Type = ItemGO.GetComponent<ItemInfo>().GetItemType();
                //Debug.Log(ItemGO.GetComponent<ItemInfo>().GetItemType());
                if (isPlacingItem)
                {
                    print("PlacedItem!");
                    ItemGO.GetComponent<ItemPlacing>().PlaceItem();
                    isPlacingItem = false;
                    inventoryManager.RemoveQuantityFromSlot(inventoryManager.EquippedSlot, 1);
                    inventoryManager.UpdateItemCountPerSlot();
                }
                else if (ItemGO.GetComponent<ItemInfo>().GetItemType() == ItemInfo.ItemType.CodeLock)
                {
                    if (currDoor != null)
                    {
                        ResetCodelockGhost(true);
                    }
                }
                else if (ItemGO.GetComponent<ItemInfo>().GetItemType() == ItemInfo.ItemType.Ranged)
                {
                    if (ItemGO.GetComponent<WeaponInfo>().GetGunName() == WeaponInfo.GUNNAME.M1911_PISTOL  && !LeftMouseButtonPressed)
                    {
                        if (ItemGO.GetComponent<WeaponInfo>().GetMagRound() > 0)
                        {
                            OnShoot();
                            print("Shooting m1911");

                            if (!isADS)
                                PAnimator.Play("PBeanShootM1911");
                            else
                            {
                                PAnimator.Play("PBeanADSPistolShoot");
                            }
                        }
                    }
                    else if (ItemGO.GetComponent<WeaponInfo>().GetGunName() == WeaponInfo.GUNNAME.AK47)
                    {
                        if (ItemGO.GetComponent<WeaponInfo>().GetMagRound() > 0)
                        {
                            OnShoot();
                            if (!isADS)
                                PAnimator.Play("PBeanShootAK");
                            else
                            {
                                PAnimator.Play("PBeanAkADSShoot");
                            }
                        }
                    }
                    else if (ItemGO.GetComponent<WeaponInfo>().GetGunName() == WeaponInfo.GUNNAME.HUNTING_BOW)
                    {
                        if (ItemGO.GetComponent<WeaponInfo>().GetMagRound() > 0)
                        {
                            PAnimator.Play("PBowDraw");
                            StartCoroutine(ItemGO.GetComponent<Bow>().StartCharge());
                        }
                    }
                    else if (ItemGO.GetComponent<WeaponInfo>().GetGunName() == WeaponInfo.GUNNAME.HOMEMADE_SHOTGUN && !LeftMouseButtonPressed)
                    {
                        if (ItemGO.GetComponent<WeaponInfo>().GetMagRound() > 0)
                        {
                            OnShoot();
                            PAnimator.Play("PBeanShootShotgun");
                        }
                    }
                    else if (ItemGO.GetComponent<WeaponInfo>().GetGunName() == WeaponInfo.GUNNAME.REVOLVER && !LeftMouseButtonPressed)
                    {
                        if (ItemGO.GetComponent<WeaponInfo>().GetMagRound() > 0)
                        {
                            OnShoot();
                            if (!isADS)
                                PAnimator.Play("PBeanShootM1911");
                            else
                            {
                                PAnimator.Play("PBeanADSPistolShoot");
                            }
                        }
                    }
                    else if (ItemGO.GetComponent<WeaponInfo>().GetGunName() == WeaponInfo.GUNNAME.BOLT_ACTION_RIFLE && !LeftMouseButtonPressed)
                    {
                        if (ItemGO.GetComponent<WeaponInfo>().GetMagRound() > 0)
                        {
                            OnShoot();
                            if (!isADS)
                            {
                                PAnimator.Play("PBeanSniperShoot");
                            }
                        }
                    }
                    else if (ItemGO.GetComponent<WeaponInfo>().GetGunName() == WeaponInfo.GUNNAME.REMINGTON870 && !LeftMouseButtonPressed)
                    {
                        if (ItemGO.GetComponent<WeaponInfo>().GetMagRound() > 0)
                        {
                            OnShoot();
                            PAnimator.Play("PBeanShotgunShoot");
                        }
                    }
                    else if (ItemGO.GetComponent<WeaponInfo>().GetGunName() == WeaponInfo.GUNNAME.C4 && !LeftMouseButtonPressed)
                    {
                        if (ItemGO.GetComponent<WeaponInfo>().ItemCount > 0)
                        {
                            OnShoot();
                            PAnimator.Play("PBeanThrow");
                            inventoryManager.RemoveQuantityFromSlot(inventoryManager.EquippedSlot, 1);
                        }
                    }
                    else if (ItemGO.GetComponent<WeaponInfo>().GetGunName() == WeaponInfo.GUNNAME.ROCKETLAUNCHER && !LeftMouseButtonPressed)
                    {
                        if (ItemGO.GetComponent<WeaponInfo>().GetMagRound() > 0)
                        {
                            OnShoot();
                            PAnimator.Play("PBeanShootAK");
                        }
                    }
                    else if (ItemGO.GetComponent<WeaponInfo>().GetGunName() == WeaponInfo.GUNNAME.MP5A4)
                    {
                        if (ItemGO.GetComponent<WeaponInfo>().GetMagRound() > 0)
                        {
                            OnShoot();

                            if (!isADS)
                                PAnimator.Play("PBeanSMGHipfire");
                            else
                            {
                                PAnimator.Play("PBeanSMGADSShoot");
                            }
                        }
                    }
                }
                else if (GO_Type == ItemInfo.ItemType.Heals)
                {
                    ItemGO.GetComponent<HealProperties>().useHealitem();
                    if (ItemGO.GetComponent<HealProperties>().NameOfHeal == "Toilet Paper" || ItemGO.GetComponent<HealProperties>().NameOfHeal == "Bandage")
                    {
                        // Add animation for bandaging;
                        pv.RPC("PlayServerSideAnimation", RpcTarget.All, pv.ViewID, "PBeanBandage");
                       // PAnimator.Play("PBeanBandage");
                    }
                    else if (ItemGO.GetComponent<HealProperties>().NameOfHeal == "Ibuprofen")
                    {
                        pv.RPC("PlayServerSideAnimation", RpcTarget.All, pv.ViewID, "PBeanIbuprofen");
                        //  PAnimator.Play("PBeanIbuprofen");
                    }
                }
                else if (canuse)
                {
                    canuse = false;
                    if (GO_Type == ItemInfo.ItemType.BuildPlan)
                    {
                        bs.Build();
                        //cooldowntimer = ItemGO.GetComponent<HarvestToolsProperties>().usecooldown;
                    }
                    else if (GO_Type == ItemInfo.ItemType.Hammer)
                    {
                        hs.PerformAction();
                    }
                    else if (GO_Type == ItemInfo.ItemType.Axe)
                    {
                        SwingItem();
                        cooldowntimer = ItemGO.GetComponent<HarvestToolsProperties>().usecooldown;
                    }
                    else if (GO_Type == ItemInfo.ItemType.Pickaxe)
                    {

                        if (ItemGO.GetComponent<ItemInfo>().itemID == ItemInfo.ItemID.Spear)
                        {
                            StabItem();
                            cooldowntimer = ItemGO.GetComponent<HarvestToolsProperties>().usecooldown;
                        }
                        else
                        {
                            ChopItem();
                            cooldowntimer = ItemGO.GetComponent<HarvestToolsProperties>().usecooldown;
                        }
                    }
 
                    else if (GO_Type == ItemInfo.ItemType.Consumables && isReleased)
                    {
                        isReleased = false;
                        ItemGO.GetComponent<ConsumableProperty>().Eatfood();
                        GetRidOfItem();
                    }
                }

                LeftMouseButtonPressed = true; //button down 
            }
            else if (Input.GetMouseButtonUp(0))
            {
                LeftMouseButtonPressed = false;
                isReleased = true;
                GameObject ItemGO = playerProperties.CurrentlyHoldingItem;
                if ( ItemGO
                    && ItemGO.GetComponent<ItemInfo>().GetItemType() == ItemInfo.ItemType.Ranged
                    && ItemGO.GetComponent<WeaponInfo>().GetGunName() == WeaponInfo.GUNNAME.HUNTING_BOW && playerProperties.CurrentlyHoldingItem)
                {
                    if (ItemGO.GetComponent<Bow>().charged)
                    {
                        OnShoot();
                    }
                    else
                    {
                        ItemGO.GetComponent<Bow>().charged = false;
                    }
                    StopCoroutine(ItemGO.GetComponent<Bow>().StartCharge());
                    pv.RPC("PlayServerSideAnimation", RpcTarget.All, pv.ViewID, "PBowShoot");
                    //PAnimator.Play("PBowShoot");
                }
            }
            else if (Input.GetMouseButton(1) && playerProperties.CurrentlyHoldingItem && playerProperties.CurrentlyHoldingItem.GetComponent<ItemInfo>().GetItemType() != ItemInfo.ItemType.BuildPlan && playerProperties.CurrentlyHoldingItem.GetComponent<ItemInfo>().GetItemType() != ItemInfo.ItemType.Hammer)
            {
                GameObject ItemGO = playerProperties.CurrentlyHoldingItem;
                if (playerProperties.CurrentlyHoldingItem.GetComponent<ItemInfo>().GetItemType() == ItemInfo.ItemType.Ranged )
                {
                    
                    playerLookAt.showDot = false;
                    playerLookAt.dot.SetActive(false);
                    if (ItemGO.GetComponent<WeaponInfo>().GetGunName() == WeaponInfo.GUNNAME.AK47)
                        PAnimator.Play("PBeanAkADS");
                    else if (ItemGO.GetComponent<WeaponInfo>().GetGunName() == WeaponInfo.GUNNAME.M1911_PISTOL || ItemGO.GetComponent<WeaponInfo>().GetGunName() == WeaponInfo.GUNNAME.REVOLVER)
                        PAnimator.Play("PBeanADSPistol");
                    else if (ItemGO.GetComponent<WeaponInfo>().GetGunName() == WeaponInfo.GUNNAME.MP5A4)
                        PAnimator.Play("PBeanSMGADS");
                    else if (ItemGO.GetComponent<WeaponInfo>().GetGunName() == WeaponInfo.GUNNAME.BOLT_ACTION_RIFLE && !isADS)
                    {
                        PAnimator.Play("PBeanSniperADS");
                        StartCoroutine(FadeToBlack());
                    }
                    isADS = true;
                }
                else
                {
                    StartCoroutine(ThrowItem());
                }
            }
            else if (Input.GetMouseButtonUp(1))
            {
                StopCoroutine(FadeToBlack());
                GameObject ItemGO = playerProperties.CurrentlyHoldingItem;
                if (playerProperties.CurrentlyHoldingItem && playerProperties.CurrentlyHoldingItem.GetComponent<ItemInfo>().GetItemType() == ItemInfo.ItemType.Ranged)
                {
                    isADS = false;
                    playerLookAt.showDot = true;
                    sniperScopeImage.SetActive(false);
                    playerLookAt.dot.SetActive(true);
                    pv.RPC("PlayServerSideAnimation", RpcTarget.All, pv.ViewID, "PBeanIdle");
                    //PAnimator.Play("PBeanIdle");
                    if (playerProperties.CurrentlyHoldingItem && playerProperties.CurrentlyHoldingItem.GetComponent<WeaponInfo>().GetGunName() == WeaponInfo.GUNNAME.BOLT_ACTION_RIFLE)
                    {
                        sniperScopeImage.SetActive(false);
                        Cam.fieldOfView = 75;
                    }
                }

            }

            //Updates Gun Ammo if Gun is done reloading
            if (playerProperties.CurrentlyHoldingItem != null)
            {
                if (playerProperties.CurrentlyHoldingItem.GetComponent<ItemInfo>().GetItemType() == ItemInfo.ItemType.Ranged)
                {
                    //Current gun ammo not matching ammo displayed
                    if (inventoryManager.CheckAmmoUpdated())
                    {
                        inventoryManager.UpdateItemCountPerSlot();
                    }
                }
                else if (playerProperties.CurrentlyHoldingItem.GetComponent<ItemInfo>().GetItemType() == ItemInfo.ItemType.Axe
                    || playerProperties.CurrentlyHoldingItem.GetComponent<ItemInfo>().GetItemType() == ItemInfo.ItemType.Pickaxe)
                {
                    //Current gun ammo not matching ammo displayed
                    if (inventoryManager.CheckDurabilityUpdated())
                    {
                        inventoryManager.UpdateItemCountPerSlot();
                    }
                }

            }

            //Updates Gun Ammo if Gun is done reloading
            if (playerProperties.CurrentlyHoldingItem)
            {
                //Debug.Log(playerProperties.CurrentlyHoldingItem.GetComponent<ItemInfo>());
                if (playerProperties.CurrentlyHoldingItem.GetComponent<ItemInfo>().GetItemType() == ItemInfo.ItemType.Ranged)
                {
                    //Current gun ammo not matching ammo displayed
                    if (inventoryManager.CheckAmmoUpdated())
                    {
                        inventoryManager.UpdateItemCountPerSlot();
                    }
                }
                else if (playerProperties.CurrentlyHoldingItem.GetComponent<ItemInfo>().GetItemType() == ItemInfo.ItemType.Axe
                    || playerProperties.CurrentlyHoldingItem.GetComponent<ItemInfo>().GetItemType() == ItemInfo.ItemType.Pickaxe)
                {
                    //Current gun ammo not matching ammo displayed
                    if (inventoryManager.CheckDurabilityUpdated())
                    {
                        inventoryManager.UpdateItemCountPerSlot();
                    }
                }

            }
            

            //Hotbar
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                UpdateInventorySlot(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                UpdateInventorySlot(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                UpdateInventorySlot(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                UpdateInventorySlot(3);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                UpdateInventorySlot(4);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                UpdateInventorySlot(5);
            }



            //Stores Equipped Item into CurrentItem
            ItemInfo CurrentItem = null;
            if (inventoryManager.IntGetItem(inventoryManager.EquippedSlot))
            {
                CurrentItem = inventoryManager.IntGetItem(inventoryManager.EquippedSlot);
            }

            //Stores Equipped Item into CurrentItem
            if (CurrentItem && //check if there is a item to equip 
                (!playerProperties.CurrentlyHoldingItem       //equips player with item in slot if hand empty
                  || (playerProperties.CurrentlyHoldingItem &&  //not null
                  playerProperties.CurrentlyHoldingItem.GetComponent<ItemInfo>().GetItemID() != CurrentItem.GetItemID())     //if player change slot, change item in hand
                )
            )
            {
                if (pv.IsMine)
                {
                    pv.RPC("ClearChildrenInActorRightHand", RpcTarget.All, pv.ViewID);
                    
                    ForceGiveItem(CurrentItem);
                    if(CurrentItem.GetComponent<ItemInfo>().itemType != ItemInfo.ItemType.unshowable) pv.RPC("UpdateOtherClientsAboutYourNewHandItem", RpcTarget.All, CurrentItem.GetComponent<PhotonView>().ViewID, pv.ViewID);
                }

            }
            else if(!CurrentItem && playerProperties.CurrentlyHoldingItem) // if player holding something but current slot supposed to have nothing held
            {
                playerProperties.CurrentlyHoldingItem.gameObject.SetActive(false);
                playerProperties.CurrentlyHoldingItem = null; 
            }
        }
    }

    private void UpdateInventorySlot(int slotNo)
    {
        if (pv.IsMine)
        {
            pv.RPC("ClearChildrenInActorRightHand", RpcTarget.All, pv.ViewID);
        }
        if (inventoryManager.EquippedSlot == slotNo) // Do not do anything as player is already in slot (double tap same slot)
            return;

        inventoryManager.EquippedSlot = slotNo;
        isPlacingItem = false;
        if (inventoryManager.InventoryList[inventoryManager.EquippedSlot] != null)
        {
            if (inventoryManager.InventoryList[inventoryManager.EquippedSlot].GetItemType() == ItemInfo.ItemType.unshowable)
            {
                isPlacingItem = true;
            }
            else if (inventoryManager.InventoryList[inventoryManager.EquippedSlot].GetItemType() == ItemInfo.ItemType.BuildPlan)
            {
                hs.SetIsUsingHammer(false);
                bs.SetIsBuilding(true);
                holdingCodeLock = false;
            }
            else if (inventoryManager.InventoryList[inventoryManager.EquippedSlot].GetItemType() == ItemInfo.ItemType.Hammer)
            {
                hs.SetIsUsingHammer(true);
                bs.SetIsBuilding(false);
                holdingCodeLock = false;
            }
            else if (inventoryManager.InventoryList[inventoryManager.EquippedSlot].GetItemType() == ItemInfo.ItemType.CodeLock)
            {
                hs.SetIsUsingHammer(false);
                bs.SetIsBuilding(false);
                holdingCodeLock = true;
            }
            else
            {
                hs.SetIsUsingHammer(false);
                bs.SetIsBuilding(false);
                holdingCodeLock = false;
            }
        }
        else
        {
            hs.SetIsUsingHammer(false);
            bs.SetIsBuilding(false);
            holdingCodeLock = false;
        }
    }

    [PunRPC]
    void RemoveItemFromHandForOtherClients(string newItem, int ActorNumber)
    {
        PhotonView ActorPV = PhotonView.Find(ActorNumber);
        GameObject Actor = ActorPV.gameObject;
        //Actor.GetComponent<PlayerProperties>().CurrentlyHoldingItem = null;

        GameObject ItemToPairToHand;
        ItemToPairToHand = GameObject.Find(newItem);
        ItemToPairToHand.SetActive(false);
    }

    [PunRPC]
    void DetachItemFromParent(string newItem, int ActorNumber)
    {

        GameObject ItemToPairToHand;

        //Player player = PhotonNetwork.CurrentRoom.GetPlayer(ActorNumber);
        // Get the PhotonView component for the player object
        PhotonView ActorPV = PhotonView.Find(ActorNumber);
        GameObject Actor = ActorPV.gameObject;
        
        //GameObject Actor = ActorView.TagObject as GameObject;
        GameObject RHand = Actor.transform.Find("Capsule").Find("RHand").gameObject;
        if (RHand.transform.Find(newItem) != null)
        {
            ItemToPairToHand = RHand.transform.Find(newItem).gameObject;

            if (ItemToPairToHand.GetComponent<MeshCollider>() != null) ItemToPairToHand.GetComponent<MeshCollider>().isTrigger = false;
            else if (ItemToPairToHand.GetComponent<BoxCollider>() != null) ItemToPairToHand.GetComponent<BoxCollider>().isTrigger = false;

            ItemToPairToHand.transform.SetParent(null);
            ItemToPairToHand.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    [PunRPC]
    void ClearChildrenInActorRightHand(int ActorNumber)
    {
        PhotonView ActorPV = PhotonView.Find(ActorNumber);
        GameObject Actor = ActorPV.gameObject;
        GameObject RHand = Actor.transform.Find("Capsule").Find("RHand").gameObject;
        foreach (Transform child in RHand.transform)
        {
            //LocalDestroy
            child.gameObject.SetActive(false);
        }
    }

    [PunRPC]
    void UpdateOtherClientsAboutYourNewHandItem(int ItemPVID, int ActorNumber)
    {
        GameObject ItemToPairToHand;

        //Player player = PhotonNetwork.CurrentRoom.GetPlayer(ActorNumber);
        // Get the PhotonView component for the player object
        PhotonView ActorPV = PhotonView.Find(ActorNumber);
        GameObject Actor = ActorPV.gameObject;

        //GameObject Actor = ActorView.TagObject as GameObject;

        GameObject RHand = Actor.transform.Find("Capsule").Find("RHand").gameObject;

        ItemToPairToHand = PhotonView.Find(ItemPVID).gameObject;

        if (ItemToPairToHand != null)
        {
            ItemToPairToHand.transform.position = RHand.transform.position;
            ItemToPairToHand.transform.rotation = RHand.transform.rotation;
            ItemToPairToHand.SetActive(true);
            ItemToPairToHand.transform.SetParent(RHand.transform);
            ItemToPairToHand.GetComponent<Rigidbody>().isKinematic = true;
        }
        else
        {
            print("IT GONE MN!");
        }
    }
    

   public void ForceGiveItem(ItemInfo itemInfo)
    {

        /*
        if (playerProperties.CurrentlyHoldingItem != null)
        {
            Destroy(playerProperties.CurrentlyHoldingItem);
            playerProperties.CurrentlyHoldingItem = null;
        }
        

        GameObject ItemToGive = Instantiate(inventoryManager.GetItem(itemInfo.GetItemID()));
        ItemToGive.GetComponent<Rigidbody>().isKinematic = true;
        ItemToGive.transform.position = RHand.transform.position;
        ItemToGive.transform.parent = RHand.transform;
        ItemToGive.hideFlags = HideFlags.NotEditable;
        ItemToGive.transform.rotation = RHand.transform.rotation;
        ItemToGive.GetComponent<ItemInfo>().SetItemID(itemInfo.itemID);
        playerProperties.CurrentlyHoldingItem = ItemToGive;
        */

        if (playerProperties.CurrentlyHoldingItem != null)
        {
            //Destroy(playerProperties.CurrentlyHoldingItem);
            playerProperties.CurrentlyHoldingItem.SetActive(false);
            playerProperties.CurrentlyHoldingItem = null;
        }

        if (itemInfo != null)
        {
            itemInfo.gameObject.SetActive(true);
            itemInfo.GetComponent<Rigidbody>().isKinematic = true;
            itemInfo.transform.position = RHand.transform.position;
            itemInfo.transform.parent = RHand.transform;
            itemInfo.hideFlags = HideFlags.NotEditable;
            itemInfo.transform.rotation = RHand.transform.rotation;
            itemInfo.GetComponent<ItemInfo>().SetItemID(itemInfo.itemID);
            playerProperties.CurrentlyHoldingItem = itemInfo.gameObject;
        }
        else
        {
            playerProperties.CurrentlyHoldingItem = null;
        }
        //Destroy(itemInfo);
    }

    void GetRidOfItem()
    {
        inventoryManager.RemoveQuantityFromSlot(inventoryManager.EquippedSlot, 1);
    }

    public void DropItem(int Slot)
    {
        StartCoroutine(IEDropItem(Slot));
    }

    public IEnumerator ThrowItem()
    {
        bool detached = false;
        if (playerProperties.CurrentlyHoldingItem.GetComponent<ItemInfo>().itemType != ItemInfo.ItemType.unshowable && !detached)
        {
            pv.RPC("PlayServerSideAnimation", RpcTarget.All, pv.ViewID, "PBeanThrow");
           // PAnimator.Play("PBeanThrow");
            GameObject GO = playerProperties.CurrentlyHoldingItem;
            yield return new WaitForSeconds(0.45f);
            pv.RPC("DetachItemFromParent", RpcTarget.All, GO.name, pv.ViewID);
            GO.GetComponent<Rigidbody>().isKinematic = false;
            //GO.GetComponent<MeshCollider>().isTrigger = false;
            playerProperties.CurrentlyHoldingItem = null;
            GO.transform.parent = null;
            GO.GetComponent<Rigidbody>().velocity = gameObject.transform.forward * 10;
            inventoryManager.Remove(inventoryManager.EquippedSlot, false);
            detached = true;
        }
    }
    
    public IEnumerator IEDropItem(int Slot)
    {
        bool detached = false;
        //PAnimator.Play("PBeanThrow");
        pv.RPC("PlayServerSideAnimation", RpcTarget.All, pv.ViewID, "PBeanThrow");

        if (Slot == inventoryManager.EquippedSlot && !detached)
        {


            GameObject GO = playerProperties.CurrentlyHoldingItem;

            if (GO.GetComponent<MeshCollider>() != null) GO.GetComponent<MeshCollider>().isTrigger = false;
            else if (GO.GetComponent<BoxCollider>() != null) GO.GetComponent<BoxCollider>().isTrigger = false;

            if (GO.GetComponent<ItemInfo>().itemType == ItemInfo.ItemType.unshowable)
            {
                GameObject GO_REPLACEMENT = GO.GetComponent<ItemInfo>().ReplacementDropObj;
                GameObject GO_Dupe = PhotonNetwork.Instantiate(GO_REPLACEMENT.name, transform.position, Quaternion.identity);
                GO_Dupe.GetComponent<Rigidbody>().isKinematic = false;
                GO_Dupe.name = GO.name;
                GO_Dupe.transform.parent = null;
                Destroy(GO);
                playerProperties.CurrentlyHoldingItem = null;
                isPlacingItem = false;
                GO_Dupe.GetComponent<Rigidbody>().velocity = gameObject.transform.forward * 2;
                inventoryManager.Remove(inventoryManager.EquippedSlot, false);
            }
            else if(!detached)
            {
                GO.transform.position = GO.transform.parent.position;
                yield return new WaitForSeconds(0.15f);
                pv.RPC("DetachItemFromParent", RpcTarget.All, GO.name, pv.ViewID);
                
                GO.GetComponent<Rigidbody>().isKinematic = false;
                playerProperties.CurrentlyHoldingItem = null;
                GO.transform.parent = null;
                GO.GetComponent<Rigidbody>().velocity = gameObject.transform.forward * 2;
                inventoryManager.Remove(inventoryManager.EquippedSlot, false);
            }
            detached = true;
        }
        else if(!detached)
        {
            GameObject GO = inventoryManager.InventoryList[Slot].gameObject;
            if (GO.GetComponent<ItemInfo>().itemType == ItemInfo.ItemType.unshowable)
            {
                GameObject GO_REPLACEMENT = GO.GetComponent<ItemInfo>().ReplacementDropObj;
                GameObject GO_Dupe = PhotonNetwork.Instantiate(GO_REPLACEMENT.name, transform.position, Quaternion.identity);
                GO_Dupe.GetComponent<Rigidbody>().isKinematic = false;
                GO_Dupe.name = GO.name;
                GO_Dupe.transform.parent = null;
                playerProperties.CurrentlyHoldingItem = null;
                isPlacingItem = false;
                GO_Dupe.GetComponent<Rigidbody>().velocity = gameObject.transform.forward * 2;
                inventoryManager.Remove(inventoryManager.EquippedSlot, false);
                Destroy(GO);
            }
            else
            {
             
                GO.SetActive(true);
                yield return new WaitForSeconds(0.15f);
                pv.RPC("DetachItemFromParent", RpcTarget.All, GO.name, pv.ViewID);
                GO.GetComponent<Rigidbody>().isKinematic = false;
                GO.transform.parent = null;
                GO.GetComponent<Rigidbody>().velocity = gameObject.transform.forward * 2;
                inventoryManager.Remove(Slot, false);
            }
            detached = true;
        }
    }

    public void StabItem()
    {
        print("Stab");
        //PAnimator.Play("PBeanStab");
        pv.RPC("PlayServerSideAnimation", RpcTarget.All, pv.ViewID, "PBeanStab");
        StartCoroutine(triggerCooldown());
    }

    public void ChopItem()
    {
        print("Chop");
        playerProperties.CurrentlyHoldingItem.GetComponent<PhotonView>().RPC("SetTriggerToTrue", RpcTarget.All);
        pv.RPC("PlayServerSideAnimation", RpcTarget.All, pv.ViewID, "PBeanChop");
        //PAnimator.Play("PBeanChop");
        StartCoroutine(triggerCooldown());
    }

    public void SwingItem()
    {
        print("Swing");
        playerProperties.CurrentlyHoldingItem.GetComponent<PhotonView>().RPC("SetTriggerToTrue", RpcTarget.All);
        pv.RPC("PlayServerSideAnimation", RpcTarget.All, pv.ViewID, "PBeanSwing");
        PAnimator.Play("PBeanSwing");
        StartCoroutine(triggerCooldown());
    }

    private void ResetCodelockGhost(bool placeLock = false)
    {
        DoorStructure cds = null;
        if (currDoor.layer == LayerMask.NameToLayer("BuildableParent"))
        {
            cds = currDoor.GetComponent<DoorStructure>();
        }
        else if (currDoor.layer == LayerMask.NameToLayer("Buildable"))
        {
            cds = currDoor.GetComponentInParent<DoorStructure>();
        }

        cds.lockObject.SetActive(false);
        Material[] prevMats = cds.lockObject.transform.GetComponent<Renderer>().materials;
        Material[] prevNewMats = { prevMats[0] };
        cds.lockObject.transform.GetComponent<Renderer>().materials = prevNewMats;

        if (placeLock)
        {
            cds.SetHasLock(true);
            inventoryManager.Remove(inventoryManager.EquippedSlot, false);
        }
        currDoor = null;
    }

    void OnShoot()
    {
        playerProperties.CurrentlyHoldingItem.GetComponent<WeaponInfo>().Discharge();
        inventoryManager.UpdateItemCountPerSlot();
    }

    IEnumerator triggerCooldown()
    {
        yield return new WaitForSeconds(0.45f);

        if (playerProperties.CurrentlyHoldingItem != null)
        {
            playerProperties.CurrentlyHoldingItem.GetComponent<PhotonView>().RPC("SetTriggerToFalse", RpcTarget.All);
        }
    }
}
