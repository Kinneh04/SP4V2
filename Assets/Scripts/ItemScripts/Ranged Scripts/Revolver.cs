using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Revolver : WeaponInfo
{
    public GameObject BarrlTip;
    //This is REQUIRED for muzzle flare;
    // Dont change it or it will NOT shoot;
    public override void Init()
    {
        MagRounds = MaxMagRounds = 8;
        FiringType = FIRINGTYPE.SEMI_AUTO;
        GunName = GUNNAME.REVOLVER;
        AmmoType = ItemID.PistolAmmo;
        Damage = 35;
        TimeBetweenShots = 0.175f;
        ElapsedTime = ReloadTime = 0;
        MaxReloadTime = 2.2;
        BarrelTip = BarrlTip;
        CanFire = false;
        AimCone = 0.75f;
        InfiniteAmmo = false;
        DrawTime = 1;
        itemID = ItemID.Revolver;
        itemType = ItemType.Ranged;
        MaxItemCount = 1;
        ItemCount = 1;
    }
}
