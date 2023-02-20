using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Remington870 : WeaponInfo
{
    public int PelletCount = 8;
    public GameObject BarrlTip;
    //This is REQUIRED for muzzle flare;
    // Dont change it or it will NOT shoot;
    public override void Init()
    {
        MagRounds = MaxMagRounds = 6;
        FiringType = FIRINGTYPE.SEMI_AUTO;
        GunName = GUNNAME.REMINGTON870;
        AmmoType = ItemID.ShotgunAmmo;
        Damage = 20;
        TimeBetweenShots = 0.25f;
        ElapsedTime = ReloadTime = 0;
        MaxReloadTime = 2.2;
        BarrelTip = BarrlTip;
        CanFire = false;
        AimCone = 0.7f;
        InfiniteAmmo = false;
        DrawTime = 1;
        itemID = ItemID.Remington870;
        itemType = ItemType.Ranged;
        MaxItemCount = 1;
        ItemCount = 1;
    }
    // Discharge this weapon
    public override bool Discharge(Transform transform)
    {
        if (CanFire)
        {
            // If there is still ammo in the magazine, then fire
            if (MagRounds > 0)
            {
                for (int i = 0; i < PelletCount; i++)
                {
                    GameObject projectile = Instantiate(BulletPrefab, transform.position, Quaternion.identity);
                    projectile.GetComponent<Raycast>().Damage = Damage;
                    projectile.GetComponent<Raycast>().BulletSpawnPoint = transform;
                    projectile.GetComponent<Raycast>().ParentGunTip = BarrelTip;
                    projectile.GetComponent<Raycast>().SetAimCone(AimCone);
                    projectile.GetComponent<Raycast>().Shoot();
                }
                // Lock the weapon after this discharge
                CanFire = false;
                // Reset the dElapsedTime to dTimeBetweenShots for the next shot
                ElapsedTime = TimeBetweenShots;
                // Reduce the rounds by 1
                MagRounds--;

                return true;
            }
        }
        return false;
    }
}
