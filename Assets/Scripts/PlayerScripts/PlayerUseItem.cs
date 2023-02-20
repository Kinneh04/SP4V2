using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun.Demo.Cockpit;

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
    public bool isPlacingItem;
    public bool isADS;
    public float zoomFactor = 2f;
    private bool isZoomedIn = false;

    public PlayerLookAt playerLookAt;
    public bool isReleased = true;
    private void Awake()
    {
        inventoryManager = Inventory.GetComponent<InventoryManager>();
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            playerProperties.OpenInventory();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            playerProperties.OpenCrafting(0);
        }

        if (!playerProperties.isDead && Cursor.lockState == CursorLockMode.Locked)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                PAnimator.Play("PBeanIdle");
              
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

                else if (playerProperties.PlayerLookingAtItem != null && playerProperties.PlayerLookingAtItem.GetComponent<ItemInfo>() != null)
                {
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
                            GO_Dupe.name = GOName;

                            inventoryManager.AddQuantity(GO_Dupe.GetComponent<ItemInfo>(), 1);
                            if (inventoryManager.InventoryList[inventoryManager.EquippedSlot] != null)
                            {
                                Destroy(GO_Dupe);
                            }
                           
                            Destroy(playerProperties.PlayerLookingAtItem);
                            if (inventoryManager.InventoryList[inventoryManager.EquippedSlot].itemID == GO_Dupe.GetComponent<ItemInfo>().itemID)
                            {
                                isPlacingItem = true;
                            }
                            inventoryManager.UpdateItemCountPerSlot();
                            Debug.Log("HEY!");
                        }
                        else
                        {
                            if (playerProperties.CurrentlyHoldingItem != null)
                            {
                                if (playerProperties.CurrentlyHoldingItem.GetComponent<ItemInfo>().GetItemType() == ItemInfo.ItemType.BuildPlan)
                                {
                                    bs.SetIsBuilding(false);
                                }
                            }
                            inventoryManager.AddQuantity(playerProperties.PlayerLookingAtItem.GetComponent<ItemInfo>());
                            playerProperties.PlayerLookingAtItem.SetActive(false);
                            inventoryManager.UpdateItemCountPerSlot();
                            if (GO_Type == ItemInfo.ItemType.BuildPlan)
                            {
                                bs.SetIsBuilding(true);
                            }
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
                    playerProperties.OpenCrafting(playerProperties.PlayerLookingAtItem.GetComponent<Workbench>().Level);
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
                        ItemGO.GetComponent<WeaponInfo>().Reload();
                        if (ItemGO.GetComponent<WeaponInfo>().GetGunName() == WeaponInfo.GUNNAME.M1911_PISTOL && !LeftMouseButtonPressed)
                        {
                            PAnimator.Play("PBeanReloadM1911");
                        }
                        else if (ItemGO.GetComponent<WeaponInfo>().GetGunName() == WeaponInfo.GUNNAME.REVOLVER && !LeftMouseButtonPressed)
                        {
                            PAnimator.Play("PBeanRevolverReload");
                        }
                        else if (ItemGO.GetComponent<WeaponInfo>().GetGunName() == WeaponInfo.GUNNAME.AK47 && !LeftMouseButtonPressed)
                        {
                            PAnimator.Play("PBeanReloadAK");
                        }
                        else if (ItemGO.GetComponent<WeaponInfo>().GetGunName() == WeaponInfo.GUNNAME.HOMEMADE_SHOTGUN && !LeftMouseButtonPressed)
                        {
                            PAnimator.Play("PBeanReloadShotgun");
                        }
                        else if (ItemGO.GetComponent<WeaponInfo>().GetGunName() == WeaponInfo.GUNNAME.MP5A4 && !LeftMouseButtonPressed)
                        {
                            PAnimator.Play("PBeanSMGReload");
                        }
                        else if (ItemGO.GetComponent<WeaponInfo>().GetGunName() == WeaponInfo.GUNNAME.REMINGTON870 && !LeftMouseButtonPressed)
                        {
                            PAnimator.Play("PBeanShotgunReload");
                        }
                        else if (ItemGO.GetComponent<WeaponInfo>().GetGunName() == WeaponInfo.GUNNAME.BOLT_ACTION_RIFLE && !LeftMouseButtonPressed)
                        {
                            PAnimator.Play("PBeanSniperReload");
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
                    inventoryManager.UpdateItemCountPerSlot();
                    inventoryManager.UpdateItemCountPerSlot();
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
                        PAnimator.Play("PBeanBandage");
                    }
                    else if (ItemGO.GetComponent<HealProperties>().NameOfHeal == "Ibuprofen")
                    {
                        PAnimator.Play("PBeanIbuprofen");
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
                    else if (GO_Type == ItemInfo.ItemType.Axe)
                    {
                        SwingItem();
                        cooldowntimer = ItemGO.GetComponent<HarvestToolsProperties>().usecooldown;
                    }
                    else if (GO_Type == ItemInfo.ItemType.Pickaxe)
                    {
                        ChopItem();
                        cooldowntimer = ItemGO.GetComponent<HarvestToolsProperties>().usecooldown;
                    }
                    else if (GO_Type == ItemInfo.ItemType.Melee)
                    {
                        StabItem();
                        cooldowntimer = ItemGO.GetComponent<HarvestToolsProperties>().usecooldown;
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
                    PAnimator.Play("PBowShoot");
                }
            }
            else if (Input.GetMouseButton(1) && playerProperties.CurrentlyHoldingItem && playerProperties.CurrentlyHoldingItem.GetComponent<ItemInfo>().GetItemType() != ItemInfo.ItemType.BuildPlan)
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
                    PAnimator.Play("PBeanIdle");
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
                Debug.Log(playerProperties.CurrentlyHoldingItem.GetComponent<ItemInfo>());
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
                inventoryManager.EquippedSlot = 0;
                isPlacingItem = false;
                if (inventoryManager.InventoryList[inventoryManager.EquippedSlot])
                {
                    if(inventoryManager.InventoryList[inventoryManager.EquippedSlot].GetItemType() == ItemInfo.ItemType.unshowable)
                    {
                        isPlacingItem = true;
                    }
                    else if (inventoryManager.InventoryList[inventoryManager.EquippedSlot].GetItemType() != ItemInfo.ItemType.BuildPlan)
                    {
                        bs.SetIsBuilding(false);
                    }
                    else bs.SetIsBuilding(true);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                inventoryManager.EquippedSlot = 1;
                isPlacingItem = false;
                if (inventoryManager.InventoryList[inventoryManager.EquippedSlot] )
                {
                    if (inventoryManager.InventoryList[inventoryManager.EquippedSlot].GetItemType() == ItemInfo.ItemType.unshowable)
                    {
                        isPlacingItem = true;
                    }
                    else if (inventoryManager.InventoryList[inventoryManager.EquippedSlot].GetItemType() != ItemInfo.ItemType.BuildPlan)
                    {
                        bs.SetIsBuilding(false);
                    }
                    else bs.SetIsBuilding(true);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                inventoryManager.EquippedSlot = 2;
                isPlacingItem = false;
                if (inventoryManager.InventoryList[inventoryManager.EquippedSlot] )
                {
                    if (inventoryManager.InventoryList[inventoryManager.EquippedSlot].GetItemType() == ItemInfo.ItemType.unshowable)
                    {
                        isPlacingItem = true;
                    }
                    else if (inventoryManager.InventoryList[inventoryManager.EquippedSlot].GetItemType() != ItemInfo.ItemType.BuildPlan)
                    {
                        bs.SetIsBuilding(false);
                    }
                    else bs.SetIsBuilding(true);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                inventoryManager.EquippedSlot = 3;
                isPlacingItem = false;
                if (inventoryManager.InventoryList[inventoryManager.EquippedSlot] )
                {
                    if (inventoryManager.InventoryList[inventoryManager.EquippedSlot].GetItemType() == ItemInfo.ItemType.unshowable)
                    {
                        isPlacingItem = true;
                    }
                    else
                    if (inventoryManager.InventoryList[inventoryManager.EquippedSlot].GetItemType() != ItemInfo.ItemType.BuildPlan)
                    {
                        bs.SetIsBuilding(false);
                    }
                    else bs.SetIsBuilding(true);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                inventoryManager.EquippedSlot = 4;
                isPlacingItem = false;
                if (inventoryManager.InventoryList[inventoryManager.EquippedSlot] )
                {
                    if (inventoryManager.InventoryList[inventoryManager.EquippedSlot].GetItemType() == ItemInfo.ItemType.unshowable)
                    {
                        isPlacingItem = true;
                    }
                    else
                    if (inventoryManager.InventoryList[inventoryManager.EquippedSlot].GetItemType() != ItemInfo.ItemType.BuildPlan)
                    {
                        bs.SetIsBuilding(false);
                    }
                    else bs.SetIsBuilding(true);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                inventoryManager.EquippedSlot = 5;
                isPlacingItem = false;
                if (inventoryManager.InventoryList[inventoryManager.EquippedSlot] )
                {
                    if (inventoryManager.InventoryList[inventoryManager.EquippedSlot].GetItemType() == ItemInfo.ItemType.unshowable)
                    {
                        isPlacingItem = true;
                    }
                    else
                    if (inventoryManager.InventoryList[inventoryManager.EquippedSlot].GetItemType() != ItemInfo.ItemType.BuildPlan)
                    {
                        bs.SetIsBuilding(false);
                    }
                    else bs.SetIsBuilding(true);
                }
            }
            //Stores Equipped Item into CurrentItem
            ItemInfo CurrentItem = null;
            if (inventoryManager.IntGetItem(inventoryManager.EquippedSlot))
            {
                CurrentItem = inventoryManager.IntGetItem(inventoryManager.EquippedSlot);
            }
            if (CurrentItem && //check if there is a item to equip 
                (!playerProperties.CurrentlyHoldingItem       //equips player with item in slot if hand empty
                  || (playerProperties.CurrentlyHoldingItem &&  //not null
                  playerProperties.CurrentlyHoldingItem.GetComponent<ItemInfo>().GetItemID() != CurrentItem.GetItemID())     //if player change slot, change item in hand
                )
            )
            {
                ForceGiveItem(CurrentItem);
            }
            else if(!CurrentItem && playerProperties.CurrentlyHoldingItem) // if player holding something but current slot supposed to have nothing held
            {
                playerProperties.CurrentlyHoldingItem.gameObject.SetActive(false);
                playerProperties.CurrentlyHoldingItem = null; 
            }
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
        if (playerProperties.CurrentlyHoldingItem.GetComponent<ItemInfo>().itemType != ItemInfo.ItemType.unshowable)
        {
            PAnimator.Play("PBeanThrow");
            GameObject GO = playerProperties.CurrentlyHoldingItem;
            yield return new WaitForSeconds(0.45f);
            GO.GetComponent<Rigidbody>().isKinematic = false;
            playerProperties.CurrentlyHoldingItem = null;
            GO.transform.parent = null;
            GO.GetComponent<Rigidbody>().velocity = gameObject.transform.forward * 10;
            inventoryManager.Remove(inventoryManager.EquippedSlot, false);
        }
    }
    
    public IEnumerator IEDropItem(int Slot)
    {
        PAnimator.Play("PBeanThrow");

        if (Slot == inventoryManager.EquippedSlot)
        {
            GameObject GO = playerProperties.CurrentlyHoldingItem;
            GO.transform.position = GO.transform.parent.position;
            yield return new WaitForSeconds(0.15f);
            GO.GetComponent<Rigidbody>().isKinematic = false;
            playerProperties.CurrentlyHoldingItem = null;
            GO.transform.parent = null;
            GO.GetComponent<Rigidbody>().velocity = gameObject.transform.forward * 5;
            inventoryManager.Remove(inventoryManager.EquippedSlot, false);
        }
        else
        {
            GameObject GO = inventoryManager.InventoryList[Slot].gameObject;
            GO.SetActive(true);
            yield return new WaitForSeconds(0.15f);
            GO.GetComponent<Rigidbody>().isKinematic = false;
            GO.transform.parent = null;
            GO.GetComponent<Rigidbody>().velocity = gameObject.transform.forward * 20;
            inventoryManager.Remove(Slot, false);
        }
    }

    public void StabItem()
    {
        print("Stab");
        PAnimator.Play("PBeanStab");
        StartCoroutine(triggerCooldown());
    }

    public void ChopItem()
    {
        print("Chop");
        playerProperties.CurrentlyHoldingItem.GetComponent<HarvestToolsProperties>().TriggerEnabled = true;
        PAnimator.Play("PBeanChop");
        StartCoroutine(triggerCooldown());
    }

    public void SwingItem()
    {
        print("Swing");
        playerProperties.CurrentlyHoldingItem.GetComponent<HarvestToolsProperties>().TriggerEnabled = true;
        PAnimator.Play("PBeanSwing");
        StartCoroutine(triggerCooldown());
    }

    void OnShoot()
    {
        playerProperties.CurrentlyHoldingItem.GetComponent<WeaponInfo>().Discharge(gameObject.GetComponentInChildren<Camera>().transform);
        inventoryManager.UpdateItemCountPerSlot();
    }

    IEnumerator triggerCooldown()
    {
        yield return new WaitForSeconds(0.45f);

        if (playerProperties.CurrentlyHoldingItem != null)
        {
            playerProperties.CurrentlyHoldingItem.GetComponent<HarvestToolsProperties>().TriggerEnabled = false;
        }
    }
}
