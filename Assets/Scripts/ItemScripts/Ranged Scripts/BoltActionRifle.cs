using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoltActionRifle : WeaponInfo
{

    public override void Init()
    {
        MagRounds = MaxMagRounds = 4;
        FiringType = FIRINGTYPE.SEMI_AUTO;
        GunName = GUNNAME.BOLT_ACTION_RIFLE;
        AmmoType = ItemID.SniperAmmo;
        Damage = 80;
        TimeBetweenShots = 1.7f;
        ElapsedTime = ReloadTime = 0;
        MaxReloadTime = 2.2;
        CanFire = false;
        AimCone = 0.0f;
        InfiniteAmmo = false;
        DrawTime = 1;
        itemID = ItemID.Bolt_Action_Rifle;
        itemType = ItemType.Ranged;
        MaxItemCount = 1;
        ItemCount = 1;
    }
}
