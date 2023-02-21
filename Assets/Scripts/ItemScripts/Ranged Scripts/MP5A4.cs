using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MP5A4 : WeaponInfo
{
    public GameObject BarrlTip;
    //This is REQUIRED for muzzle flare;
    // Dont change it or it will NOT shoot;
    public override void Init()
    {
        MagRounds = MaxMagRounds = 30;
        FiringType = FIRINGTYPE.FULL_AUTO;
        GunName = GUNNAME.MP5A4;
        AmmoType = ItemID.PistolAmmo;
        Damage = 15;
        TimeBetweenShots = 0.04f;
        ElapsedTime = ReloadTime = 0;
        MaxReloadTime = 2.2;
        BarrelTip = BarrlTip;
        CanFire = false;
        AimCone = 0.3f;
        InfiniteAmmo = false;
        DrawTime = 1;
        itemID = ItemID.MP5A4;
        itemType = ItemType.Ranged;
        MaxItemCount = 1;
        ItemCount = 1;
    }
}
