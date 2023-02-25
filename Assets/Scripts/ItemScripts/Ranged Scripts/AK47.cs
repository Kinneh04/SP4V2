using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK47 : WeaponInfo
{

    public override void Init()
    {
        MagRounds = MaxMagRounds = 30;
        FiringType = FIRINGTYPE.FULL_AUTO;
        GunName = GUNNAME.AK47;
        AmmoType = ItemID.RifleAmmo;
        
        Damage = 50;
        TimeBetweenShots = 0.1f;
        ElapsedTime = ReloadTime = 0;
        MaxReloadTime = 4.4f;
        CanFire = false;
        AimCone = 0.2f;
        InfiniteAmmo = false;
        DrawTime = 1;
        itemID = ItemID.AK47_Rifle;
        itemType = ItemType.Ranged;
        MaxItemCount = 1;
        ItemCount = 1;
    }
}