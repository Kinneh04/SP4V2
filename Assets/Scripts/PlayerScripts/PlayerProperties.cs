using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class PlayerProperties : MonoBehaviour
{
    public float Health;
    public float MaxHealth;
    public float XSensitivity;
    public float YSensitivity;
    public float Hunger;
    public float Thirst;
    public GameObject PlayerLookingAtItem = null;
    public GameObject CurrentlyHoldingItem = null;

    public Image BloodImage;

    public Slider HealthSlider;
    public Slider HungerSlider;
    public Slider ThirstSlider;

    public Vector3 Spawnpoint;
    public Vector3 OGSpawnPoint;

    public bool isPoisoned;
    public float PoisonTimer;

    public float sickChance;
    public bool isSick;

    public float bleedChance;
    public bool isBleeding;

    public float Healtimer;
    public bool isHealing;

    public PlayerMovement playerMovement;
    public InventoryManager IM;

    public bool isFull;
    public float FullTimer;

    public float HungerDecrementTimer;
    public float ThirstDecrementTimer;

    public GameObject FullIcon;
    public GameObject PoisonIcon;
    public GameObject SickIcon;
    public GameObject HealIcon;
    public GameObject BuildingDisabledIcon;
    public GameObject BuildingPrivilegeIcon;
    public Animator panim;
    public GameObject bleedingIcon;

    public TMP_Text FullTimerText;
    public TMP_Text bleedintTimerText;
    public TMP_Text PoisonTimerText;
    public TMP_Text HealTimerText;
    public GameObject Lastbedclaimed;
    public TMP_Text HealthText, Hungertext, thirsttext;

    float OGBI, OGPI, OGFI, OGSI, OGHI;

    public GameObject RadiationIcon;
    public TMP_Text RadiationAmountText;
    public int RadiationAmount;
    public float RadiationExpireTimer;
    float RTimer;

    public bool isBuildingDisabled;
    public bool hasBuildingPrivilege;

    bool isShowingBlood = false;
    float bloodTimer = 0f;

    float Htimer, Ttimer, BTimer;
    public float bleedingInterval, poisonInterval, fullinterval, sickInterval, HealInterval;
    public bool isDead;
    public GameObject deathscreen, awokenMenu;
    public GameObject SleepingMenu;
    public bool isSleeping;
    public bool inventoryIsOpen;

    public bool craftingIsOpen = false;
    public GameObject craftingScreen;
    public CraftingManager CM;

    public GameObject LootScreen, inventoryScreen, furnaceScreen;
    public PlayerMovement PM;
    public GameObject DeathBag;

    public GameObject PauseMenu;
    public Camera Camera;
    public GameObject GraphicsLoader;
    PhotonView pv;
    public bool RadiationNoisePlayed = false;

    public GameObject CannotDropHere;

    float DeathTimer;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();

        Sleep();
        OGSpawnPoint = transform.position;
        Camera = gameObject.transform.Find("Capsule").Find("Eyes").Find("Camera").GetComponent<Camera>();
        GraphicsLoader = GameObject.Find("GraphicsLoader");
        float renderDistance = GraphicsLoader.GetComponent<DropdownHolder>().renderDistance;
        if(renderDistance > 10)
        {
            Camera.farClipPlane = renderDistance;
        }
    }

    public void Sleep()
    {
        SleepingMenu.SetActive(true);
        isSleeping = true;
    }

    public void DisconnectFromServer()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("MainMenuScene");
    }

    public void TurnOnFurnace()
    {
        PlayerLookingAtItem.GetComponent<FurnaceProperties>().TurnOn();
    }
    public void OpenFurnaceInventory()
    {
        inventoryScreen.SetActive(true);
        furnaceScreen.SetActive(true);
        PlayerLookingAtItem.GetComponent<FurnaceProperties>().isLookingAtIt = true;
        inventoryIsOpen = true;
        playerMovement.UnlockCursor();
    }
    public void OpenInventory()
    {
        if (pv.IsMine)
        {
            print("HEY!");
            if (!playerMovement.isMovementEnabled)
            {
                //print("HEY!");
                if (!playerMovement.isMovementEnabled)
                {
                    inventoryScreen.SetActive(false);
                    inventoryIsOpen = false;
                    furnaceScreen.SetActive(false);


                    LootScreen.SetActive(false);
                    if (PlayerLookingAtItem && PlayerLookingAtItem.tag == "Crate" && LootScreen.activeSelf)
                    {
                        print("UpdatingCrate!");
                        PlayerLookingAtItem.GetComponent<LootProperties>().PrepareToSyncLoot();



                        PlayerLookingAtItem.GetComponent<LootProperties>().ClearLastLootPool();
                    }
                    else if (PlayerLookingAtItem && PlayerLookingAtItem.tag == "Campfire")
                    {
                        PlayerLookingAtItem.GetComponent<FurnaceProperties>().isLookingAtIt = false;
                        PlayerLookingAtItem.GetComponent<FurnaceProperties>().UpdateLoot();
                        PlayerLookingAtItem.GetComponent<FurnaceProperties>().ClearLastLootPool();
                    }
                    playerMovement.LockCursor();
                }
                else if (PlayerLookingAtItem && PlayerLookingAtItem.tag == "Campfire")
                {
                    PlayerLookingAtItem.GetComponent<FurnaceProperties>().isLookingAtIt = false;
                    PlayerLookingAtItem.GetComponent<FurnaceProperties>().UpdateLoot();
                    PlayerLookingAtItem.GetComponent<FurnaceProperties>().ClearLastLootPool();
                }
                playerMovement.LockCursor();
            }
            else
            {
                inventoryScreen.SetActive(true);
                inventoryIsOpen = true;
                playerMovement.UnlockCursor();
            }
        }
    }

    public void OpenCrafting(int WorkbenchLv)
    {
        if (craftingIsOpen)
        {
            craftingScreen.SetActive(false);
            craftingIsOpen = false;
            playerMovement.LockCursor();
        }
        else
        {
            craftingScreen.SetActive(true);
            Debug.Log("Loading Crafts");
            CM.LoadCrafts(WorkbenchLv, true);
            craftingIsOpen = true;
            playerMovement.UnlockCursor();
        }
    }

    [PunRPC]
    void PlayServerSideAnimation(int viewID, string animationName)
    {
        GameObject obj = PhotonView.Find(viewID).gameObject;
        Animator remoteAnimator = obj.GetComponent<Animator>();
        remoteAnimator.Play(animationName);
    }

    public void OpenResearch()
    {
        if (craftingIsOpen)
        {
            craftingScreen.SetActive(false);
            craftingIsOpen = false;
            playerMovement.LockCursor();
        }
        else
        {
            craftingScreen.SetActive(true);
            Debug.Log("Loading Research");
            CM.LoadCrafts(0, false);
            craftingIsOpen = true;
            playerMovement.UnlockCursor();
        }
    }
    public void OpenLootInventory()
    {
        inventoryScreen.SetActive(true);
        LootScreen.SetActive(true);
        inventoryIsOpen = true;
        playerMovement.UnlockCursor();
    }

    public void RespawnAfterDeath()
    {
        IM.ClearInventory();
        IM.UpdateItemCountPerSlot();
        BloodImage.color = new Color(1, 0, 0, 0);
        if (Lastbedclaimed != null)
            transform.position = Spawnpoint;
        else transform.position = OGSpawnPoint;

        RadiationAmount = 0;
        RadiationIcon.SetActive(false);
        isPoisoned = false;
        isBleeding = false;
        isFull = false;
        isHealing = false;
        Health = 75;
        Hunger = 60;
        Thirst = 40;
       
        HealIcon.SetActive(false);
        FullIcon.SetActive(false);
        PoisonIcon.SetActive(false);
        bleedingIcon.SetActive(false);
        BuildingDisabledIcon.SetActive(false);
        BuildingPrivilegeIcon.SetActive(false);

        deathscreen.SetActive(false);
        awokenMenu.SetActive(true);
        isDead = false;
        //panim.Play("PBeanIdle");
        pv.RPC("PlayServerSideAnimation", RpcTarget.All, pv.ViewID, "PBeanIdle");

        PM.canLookAround = true;
        PM.isMovementEnabled = true;

        craftingIsOpen = false;
        craftingScreen.SetActive(false);
        inventoryIsOpen = false;
        inventoryScreen.SetActive(false);
        RadiationNoisePlayed = false;

        PhotonView Rockpv = PhotonNetwork.Instantiate("Rock", transform.position, Quaternion.identity, 0).GetComponent<PhotonView>();
        PhotonView TorchPV = PhotonNetwork.Instantiate("Torch", transform.position, Quaternion.identity, 0).GetComponent<PhotonView>();
        Rockpv.gameObject.SetActive(false);
        TorchPV.gameObject.SetActive(false);
        Rockpv.transform.SetParent(gameObject.transform.Find("Capsule").Find("RHand"));
        TorchPV.transform.SetParent(gameObject.transform.Find("Capsule").Find("RHand"));
        IM.AddQuantity(Rockpv.gameObject.GetComponent<HarvestToolsProperties>(), 1);
        IM.AddQuantity(TorchPV.gameObject.GetComponent<HarvestToolsProperties>(), 1);

        IM.UpdateItemCountPerSlot();
        IM.EquippedSlot = 0;
        pv.RPC("UpdateOtherClientsAboutYourNewHandItem", RpcTarget.All, Rockpv.ViewID, pv.ViewID);
    }

    public void HealHealth(int HealthAmt, bool HealsBleed, bool HealsPoison, float poisonChance)
    {
        if(HealsBleed)
        {
            bleedingIcon.SetActive(false);
            isBleeding = false;
            BTimer = 0;
        }
        if(HealsPoison)
        {
            PoisonIcon.SetActive(false);
            isPoisoned = false;
            PoisonTimer = 0;
        }

        if(poisonChance > 0)
        {
            float r = Random.Range(1, 100);
            if(r < poisonChance)
            {
                isPoisoned = true;
                PoisonIcon.SetActive(true);
                PoisonTimer = 25.0f;
            }
        }

        Health += HealthAmt;
        if (Health > 100) Health = 100;
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        HealthSlider.maxValue = 100;
        ThirstSlider.maxValue = 100;
        HungerSlider.maxValue = 100;

        HealthSlider.value = Health;
        ThirstSlider.value = Thirst;
        HungerSlider.value = Hunger;

        HealthText.text = Health.ToString();
        Hungertext.text = Hunger.ToString();
        thirsttext.text = Thirst.ToString();
        OGFI = fullinterval;
        OGBI = bleedingInterval;
        OGPI = poisonInterval;
        OGSI = sickInterval;
        OGHI = HealInterval;
    }

    private void Update()
    {

        if(!isDead && pv.IsMine && !isSleeping)
        { 
            Htimer += Time.deltaTime;
            Ttimer += Time.deltaTime;
            DeathTimer -= Time.deltaTime; 


            RadiationExpireTimer -= Time.deltaTime;
           
            if(RadiationAmount <= 0)
            {
                RadiationNoisePlayed = false;
            }
            if (RadiationExpireTimer <= 0)
            {
                
                if (RTimer > 1)
                {
                    RTimer = 0;
                    RadiationAmount -= 1;
                }
            }
            if (RadiationAmount == 25 && !RadiationNoisePlayed)
            {
                RadiationNoisePlayed = true;
                AudioManager AM = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
                AM.PlayAudio(57);
            }
            if (RadiationAmount >= 30)
            {
                

                RadiationAmountText.text = RadiationAmount.ToString();
                RTimer += Time.deltaTime;
                if (RTimer > 1)
                {
                    RTimer = 0;
                    TakeDamage(1 + RadiationAmount / 50);
                }
            }
            else if(RadiationAmount > 0)
            {
                RTimer += Time.deltaTime;
                RadiationExpireTimer -= Time.deltaTime;
                RadiationAmountText.text = RadiationAmount.ToString();
                RadiationIcon.SetActive(true);
            }
            else if(RadiationAmount <= 0)
            {
                if (RadiationAmount < 0) RadiationAmount = 0;
                RadiationIcon.SetActive(false);
            }
          
            HealthSlider.value = Health;
            HungerSlider.value = Hunger;
            ThirstSlider.value = Thirst;
            HealthText.text = Health.ToString();
            thirsttext.text = Thirst.ToString();
            Hungertext.text = Hunger.ToString();
            if (Htimer > HungerDecrementTimer)
            {
                if (Hunger <= 0) Health -= 1;
                else Hunger -= 1;
                Htimer = 0;

                if (Hunger < 25 || Thirst < 20)
                {
                    sickChance += 2;
                }
                else
                {
                    sickChance--;
                }
                float t = Random.Range(0, 100);
                if (t < sickChance)
                {
                    isSick = true;
                    SickIcon.SetActive(true);
                }
            }
            if (Ttimer > ThirstDecrementTimer)
            {
                if (Thirst <= 0) TakeDamage(1);
                else Thirst -= 1;
                Ttimer = 0;
            }

            if (isFull)
            {
                FullTimer -= Time.deltaTime;
                FullTimerText.text = FullTimer.ToString();

                fullinterval -= Time.deltaTime;
                if (fullinterval < 0)
                {
                    fullinterval = OGFI;
                    Hunger++;
                }

                if (FullTimer < 0)
                {
                    isFull = false;
                    FullIcon.SetActive(false);
                }
            }

            if (isPoisoned)
            {
                PoisonTimer -= Time.deltaTime;
                PoisonTimerText.text = PoisonTimer.ToString();

                poisonInterval -= Time.deltaTime;
                if (poisonInterval < 0)
                {
                    poisonInterval = OGPI;
                    TakeDamage(1);
                }

                if (PoisonTimer < 0)
                {
                    isPoisoned = false;
                    PoisonIcon.SetActive(false);
                }
            }

            if (isHealing)
            {
                HealInterval -= Time.deltaTime;
                HealTimerText.text = Healtimer.ToString();
                Healtimer -= Time.deltaTime;
                if (HealInterval < 0)
                {
                    HealInterval = OGHI;
                    Health += 1;

                    if (Health > 100) Health = 100;
                }
                if (Healtimer < 0)
                {
                    isHealing = false;
                    HealIcon.SetActive(false);
                }
            }

            if(isShowingBlood)
            {
                bloodTimer -= Time.deltaTime;
                if(bloodTimer <= 0)
                {
                    isShowingBlood = false;
                    BloodImage.color = Color.Lerp(BloodImage.color, new Color(1, 0, 0, 0), 0.5f);
                }
            }

            if (isSick)
            {
                sickInterval -= Time.deltaTime;
                if (sickInterval < 0)
                {
                    sickInterval = OGSI;
                    TakeDamage(1);
                }
                if (Hunger > 90 || Thirst > 90)
                {
                    isSick = false;
                    SickIcon.SetActive(false);
                }
            }

            if (isBleeding)
            {
                BTimer -= Time.deltaTime;
                bleedingInterval -= Time.deltaTime;
                if (bleedingInterval < 0)
                {
                    bleedingInterval = OGBI;
                    TakeDamage(1);
                }
                bleedintTimerText.text = BTimer.ToString();
                if (BTimer < 0)
                {
                    isBleeding = false;
                    bleedingIcon.SetActive(false);
                }
            }
        }
        else if(isSleeping)
        {
            if (Input.GetKey(KeyCode.E) && pv.IsMine)
            {
                SleepingMenu.SetActive(false);
                isSleeping = false;
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.E))
            {
                RespawnAfterDeath();
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GetComponent<ChatManager>().isTyping)
                return;
            if (PauseMenu.activeSelf)
            {
                PauseMenu.SetActive(false);
                playerMovement.LockCursor();
            }
            else
            {
                PauseMenu.SetActive(true);
                playerMovement.UnlockCursor();
            }
        }

        // Align icons to right, filling up inactive icons' spaces
        ShiftIcons();
    }

    public IEnumerator ShowBlood()
    {
        while (BloodImage.color.a < 0.2f)
        {
            BloodImage.color =new Color(BloodImage.color.r, BloodImage.color.g, BloodImage.color.b, BloodImage.color.a + 0.01f);
            yield return new WaitForSeconds(0.1f);
        }
    }

    [PunRPC]
    public void TakeServerSideDamage(int PVID, float damage)
    {
        PhotonView PV = PhotonView.Find(PVID);
        if (PV.IsMine)
        {
            Health -= damage;

            if (Health < 50)
            {
                float q = Health / MaxHealth;
                StartCoroutine(ShowBlood());
                isShowingBlood = true;
                bloodTimer = 5.0f;
                if (Health <= 0)
                {
                    die();
                }
            }

        }
    }
    public void TakeDamage(float damage)
    {
        if (pv.IsMine)
        {
            Health -= damage;

            if (Health < 50)
            {
                float q = Health / MaxHealth;
                StartCoroutine(ShowBlood());
                isShowingBlood = true;
                bloodTimer = 5.0f;
                if (Health <= 0)
                {
                    die();
                }
            }

        }
    }
    //Its TakeDamage but without punrpc
    public void TakeDamageV2(float damage)
    {
        if (pv.IsMine)
        {
            Health -= damage;
            if (Health < 50)
            {
                float q = Health / MaxHealth;
                StartCoroutine(ShowBlood());
                isShowingBlood = true;
                bloodTimer = 5.0f;
                if (Health <= 0)
                {
                    die();
                }
            }
            float f = Random.Range(1, 100);
            if (bleedChance < f)
            {
                isBleeding = true;
                bleedingIcon.SetActive(true);
                BTimer = 60f;
            }
        }
    }

    public void die()
    {
        if (!isDead)
        {
            PM.isMovementEnabled = false;
            PM.canLookAround = false;
            panim.StopPlayback();
            //panim.Play("PBeanDeath");
            pv.RPC("PlayServerSideAnimation", RpcTarget.All, pv.ViewID, "PBeanDeath");
            StartCoroutine(DeathSequence());
            
            awokenMenu.SetActive(false);
            isDead = true;

            if(DeathTimer <= 0)
            {
                AudioManager AM = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
                AM.PlayAudio(48);
               
            }
            DeathTimer = 150;
            AudioManager AM2 = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
            AM2.PlayAudio(5);
        }
    }

    [PunRPC]
    public void ShoveLootInDeathBag(int DBVID)
    {

        GameObject DB = PhotonView.Find(DBVID).gameObject;

        IM.UpdateItemCountPerSlot();
        //if(IM.InventoryList[IM.EquippedSlot] != null)
        //{
        //    DB.GetComponent<LootProperties>().ItemsInCrate.Add((IM.InventoryList[IM.EquippedSlot]));
        //    DB.GetComponent<LootProperties>().ItemQuantityInCrate.Add((IM.InventoryList[IM.EquippedSlot].GetItemCount()));
        //}
        for(int i = 0; i < 30; i++)
        {
            if(IM.InventoryList[i] != null)
            {
                DB.GetComponent<LootProperties>().ItemsInCrate.Add((IM.InventoryList[i]));
                DB.GetComponent<LootProperties>().ItemQuantityInCrate.Add((IM.InventoryList[i].GetItemCount()));
                IM.InventoryList[i].gameObject.transform.parent = null;
                IM.InventoryList[i].gameObject.SetActive(false);
                //DB.gameObject.transform.parent = null;
            }
        }

        DB.GetComponent<LootProperties>().PrepareToSyncLoot();
    }
    [PunRPC]
    public void DefaultRaycastInit()
    {
        WeaponInfo weaponInfo = gameObject.GetComponentInChildren<WeaponInfo>();
        GameObject Raycast = Instantiate(weaponInfo.BulletPrefab, transform.position, Quaternion.identity);
        Raycast.GetComponent<Raycast>().Damage = weaponInfo.GetDamage();
        Raycast.GetComponent<Raycast>().BulletSpawnPoint = gameObject.transform;
        Raycast.GetComponent<Raycast>().ParentGunTip = weaponInfo.BarrelTip;
        Raycast.GetComponent<Raycast>().SetAimCone(weaponInfo.GetAimCone());
        Raycast.GetComponent<Raycast>().Shoot();
    }
    [PunRPC]
    public void DefaultProjectileInit(int PhotonViewID)
    {
        WeaponInfo weaponInfo = gameObject.GetComponentInChildren<WeaponInfo>();
        GameObject Projectile = PhotonView.Find(PhotonViewID).gameObject;
        Projectile.GetComponent<Projectile>().Damage = weaponInfo.GetDamage();
        Projectile.GetComponent<Projectile>().BulletSpawnPoint = transform;
        Projectile.GetComponent<Projectile>().ParentGunTip = weaponInfo.BarrelTip;
        Projectile.GetComponent<Projectile>().SetAimCone(weaponInfo.GetAimCone());
        Projectile.transform.parent = null;
        Projectile.transform.rotation = weaponInfo.transform.rotation;
        Projectile.GetComponent<Projectile>().JustFired = true;
        Projectile.GetComponent<Projectile>().itemID = weaponInfo.GetAmmoType();
        Projectile.GetComponent<Projectile>().ExplosionTimer = 3;
        Projectile.GetComponent<Projectile>().ShootNonRaycastType();
    }
    
    public IEnumerator DeathSequence()
    {
        if (pv.IsMine)
        {
            yield return new WaitForSeconds(1.6f);
            isDead = true;
            deathscreen.SetActive(true);
            PhotonView pvBag = PhotonNetwork.Instantiate(DeathBag.name, transform.position, Quaternion.identity).GetComponent<PhotonView>();

           // pv.RPC("ShoveLootInDeathBag", RpcTarget.All, pvBag.ViewID);
            ShoveLootInDeathBag(pvBag.ViewID);
        }
    }

    private void ShiftIcons()
    {
        if (pv.IsMine)
        {
            List<GameObject> iconList = new List<GameObject> { RadiationIcon, HealIcon, bleedingIcon, SickIcon, FullIcon, PoisonIcon, BuildingDisabledIcon, BuildingPrivilegeIcon };
            int currIndex = 0;

            foreach (GameObject icon in iconList)
            {
                if (icon != null && icon.activeSelf)
                {
                    icon.GetComponent<RectTransform>().anchoredPosition = new Vector2(currIndex * -105.6f + 429.6f, icon.GetComponent<RectTransform>().anchoredPosition.y);
                    currIndex++;
                }
            }
        }
    }
}
