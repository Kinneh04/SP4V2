using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M1911 : WeaponInfo
{
    public GameObject BarrlTip;
    //This is REQUIRED for muzzle flare;
    // Dont change it or it will NOT shoot;
    public override void Init()
    {
        MagRounds = MaxMagRounds = 11;
        FiringType = FIRINGTYPE.SEMI_AUTO;
        GunName = GUNNAME.M1911_PISTOL;
        AmmoType = ItemID.PistolAmmo;
        Damage = 45;
        TimeBetweenShots = 0.15f;
        ElapsedTime = ReloadTime = 0;
        MaxReloadTime = 2.2;
        BarrelTip = BarrlTip;
        CanFire = false;
        AimCone = 1.0f;
        InfiniteAmmo = false;
        DrawTime = 1;
        itemID = ItemID.M1911_Pistol;
        itemType = ItemType.Ranged;
        MaxItemCount = 1;
        ItemCount = 1;
    }
}
