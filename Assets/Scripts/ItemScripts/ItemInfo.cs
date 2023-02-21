using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class ItemInfo : MonoBehaviour
{
    public PhotonView pv;
    public enum ItemType {
        Axe,
        Pickaxe, 
        Melee,
        Ranged, 
        Bush, 
        Consumables,
        Materials,
        Heals,
        BuildPlan,
        unshowable,
        NUM_ITEMTYPE
    };

    public enum ItemID
    {
        Expired_Milk,//
        Wood,//
        Stone,//
        RawMetal,//
        RawSulfur,//
        Berry,//
        Water_Bottle,//
        Red_Mushroom,//
        Magic_Mushroom,//
        Raw_Meat,//
        Cooked_steak,//
        Rock,//
        Metal_Pick,//
        Homemade_Pick,//
        Metal_Axe,//
        Homemade_Axe,//
        M1911_Pistol,//
        AK47_Rifle,//
        Handmade_Shotgun,//
        Toilet_Paper,//
        Bandage,//
        Ibuprofen,//
        Torch,//
        Building_Blueprint,//
        Sleeping_Bag,//
        Hemp,//
        Hunting_Bow,//
        Bolt_Action_Rifle,//
        Spear,  //
        PistolAmmo,//
        RifleAmmo,//
        GunParts,//
        PistolStock,//
        RifleStock,//
        CampfireGhost,//
        SleepingBagGhost,//
        RefinedSulfur,//
        RefinedMetal,//
        Arrow,//
        Rocket,
        ShotgunAmmo,//
        SmgAmmo,
        SniperAmmo,
        Revolver,
        MP5A4,
        Remington870,//
        C4,
        RocketLauncher,
        Campfire,
        SleepingBag,
        Workbench_1,
        Workbench_2,
        Workbench_3,
        Workbench_1_Ghost,
        Workbench_2_Ghost,
        Workbench_3_Ghost,
        ResearchTable,
        ResearchTable_Ghost,
        NUM_ITEMID  
    };


    public GameObject ReplacementObj;
    public GameObject OwnerActor;

	public ItemType itemType;
    public ItemID itemID;

    public int ItemCount;
    public int MaxItemCount;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }
    [PunRPC]
    void ParentToObj(int ActorNumber)
    {
        PhotonView GOPV = PhotonView.Find(ActorNumber);
        GOPV.gameObject.transform.parent = GameObject.FindGameObjectWithTag("LootPool").transform;
    }
    virtual public void Init()
    {

    }
    public ItemType GetItemType()
    {
        return itemType;
    }
    public void SetItemType(ItemType itemType)
    {
        this.itemType = itemType;
    }
    public ItemID GetItemID()
    {
        return itemID;
    }
    public void SetItemID(ItemID itemID)
    {
        this.itemID = itemID;
    }
    public int GetMaxItemCount()
    {
        return MaxItemCount;
    }
    public void SetMaxItemCount(int maxitemcount)
    {
        this.MaxItemCount = maxitemcount;
    }
    public int GetItemCount()
    {
        return ItemCount;
    }
    public void SetItemCount(int itemcount)
    {
        this.ItemCount = itemcount;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
