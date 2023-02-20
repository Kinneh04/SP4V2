using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class PlayerProperties : MonoBehaviour
{
    public float Health;
    public float MaxHealth;
    public float XSensitivity;
    public float YSensitivity;
    public float Hunger;
    public float Thirst;
    public GameObject PlayerLookingAtItem;
    public GameObject CurrentlyHoldingItem;

    public Slider HealthSlider;
    public Slider HungerSlider;
    public Slider ThirstSlider;

    public Vector3 Spawnpoint;

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



    float Htimer, Ttimer, BTimer;
    public float bleedingInterval, poisonInterval, fullinterval, sickInterval, HealInterval;
    public bool isDead;
    public GameObject deathscreen, awokenMenu;
    public bool inventoryIsOpen;
    public Image TurnOnButton;
    public TMP_Text TurnOnText;

    public bool craftingIsOpen = false;
    public GameObject craftingScreen;

    public GameObject LootScreen, inventoryScreen, furnaceScreen;
    public PlayerMovement PM;
    public GameObject DeathBag;

    public void TurnOnFurnace()
    {
        PlayerLookingAtItem.GetComponent<FurnaceProperties>().TurnOn();
        if (PlayerLookingAtItem.GetComponent<FurnaceProperties>().isOn)
        {
            TurnOnButton.color = Color.red;
            TurnOnText.text = "Turn Off";
        }
        else
        {
            TurnOnButton.color = Color.green;
            TurnOnText.text = "Turn On";
        }
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
        print("HEY!");
        if (!playerMovement.isMovementEnabled)
        {
            inventoryScreen.SetActive(false);
            inventoryIsOpen = false;
            furnaceScreen.SetActive(false);
            
            
            LootScreen.SetActive(false);
            if (PlayerLookingAtItem && PlayerLookingAtItem.tag == "Crate")
            {
                print("UpdatingCrate!");
                PlayerLookingAtItem.GetComponent<LootProperties>().UpdateLoot();
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
        else
        {
            inventoryScreen.SetActive(true);
            inventoryIsOpen = true;
            playerMovement.UnlockCursor();
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
            CraftingManager CM = FindObjectOfType<CraftingManager>();
            Debug.Log("Loading Crafts");
            CM.LoadCrafts(WorkbenchLv, true);
            craftingIsOpen = true;
            playerMovement.UnlockCursor();
        }
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
            CraftingManager CM = FindObjectOfType<CraftingManager>();
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

        if (Lastbedclaimed != null)
            transform.position = Spawnpoint;
        else transform.position = new Vector3(-9.11f, 0.15f, 3.24f);

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

        deathscreen.SetActive(false);
        awokenMenu.SetActive(true);
        isDead = false;
        panim.Play("PBeanIdle");

        PM.canLookAround = true;
        PM.isMovementEnabled = true;
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
    }

    private void Update()
    {
        if(!isDead)
        { 
            Htimer += Time.deltaTime;
            Ttimer += Time.deltaTime;
            
            RadiationExpireTimer -= Time.deltaTime;
           
            if (RadiationExpireTimer <= 0)
            { 
                if (RTimer > 1)
                {
                    RTimer = 0;
                    RadiationAmount -= 1;
                }
            }
            if (RadiationAmount >= 30)
            {
                RadiationAmountText.text = RadiationAmount.ToString();
                
                if (RTimer > 1)
                {
                    RTimer = 0;
                    TakeDamage(1);
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
                    Healtimer = OGHI;
                    Health += 1;

                    if (Health > 100) Health = 100;
                }
                if (Healtimer < 0)
                {
                    isHealing = false;
                    HealIcon.SetActive(false);
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
        else
        {
            if (Input.GetKey(KeyCode.E))
            {
                RespawnAfterDeath();
            }
        }
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;

        if(Health <= 0)
        {
            die();
        }

        float f = Random.Range(1, 100);
        if(bleedChance < f)
        {
            isBleeding = true;
            bleedingIcon.SetActive(true);
            BTimer = 60f;

        }
    }

    public void die()
    {
        if (!isDead)
        {
            PM.isMovementEnabled = false;
            PM.canLookAround = false;
            panim.StopPlayback();
            panim.Play("PBeanDeath");
            StartCoroutine(DeathSequence());
            
            awokenMenu.SetActive(false);
            isDead = true;
        }
    }

    public void ShoveLootInDeathBag(GameObject DB)
    {
        IM.UpdateItemCountPerSlot();
        if(IM.InventoryList[IM.EquippedSlot] != null)
        {
            DB.GetComponent<LootProperties>().ItemsInCrate.Add((IM.InventoryList[IM.EquippedSlot]));
            DB.GetComponent<LootProperties>().ItemQuantityInCrate.Add((IM.InventoryList[IM.EquippedSlot].GetItemCount()));
        }
        for(int i = 0; i < 30; i++)
        {
            if(IM.InventoryList[i] != null)
            {
                DB.GetComponent<LootProperties>().ItemsInCrate.Add((IM.InventoryList[i]));
                DB.GetComponent<LootProperties>().ItemQuantityInCrate.Add((IM.InventoryList[i].GetItemCount()));
            }
        }
        
    }

    public IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(1.6f);
        isDead = true;
        deathscreen.SetActive(true);
        GameObject GO = PhotonNetwork.Instantiate("DeathBag", transform.position, Quaternion.identity);
        ShoveLootInDeathBag(GO);

       
        


    }
}
