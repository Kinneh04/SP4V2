using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncher : WeaponInfo
{
	public GameObject BarrlTip;

	public override void Init()
    {
        MagRounds = MaxMagRounds = 1;
        FiringType = FIRINGTYPE.SEMI_AUTO;
        GunName = GUNNAME.ROCKETLAUNCHER;
		AmmoType = ItemID.Rocket;
		Damage = 350;
		TimeBetweenShots = 3.0f;
        ElapsedTime = ReloadTime = 0;
        MaxReloadTime = 3.0f;
        CanFire = false;
        AimCone = 2.25f;
		InfiniteAmmo = false;
		DrawTime = 1;
        itemID = ItemID.RocketLauncher;
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
			if (MagRounds > 0 || InfiniteAmmo)
			{
				GameObject projectile = Instantiate(BulletPrefab, BarrlTip.transform.position, Quaternion.identity);
				projectile.GetComponent<Projectile>().Damage = Damage;
				projectile.GetComponent<Projectile>().BulletSpawnPoint = transform;
				projectile.GetComponent<Projectile>().ParentGunTip = BarrlTip;
				projectile.GetComponent<Projectile>().SetAimCone(AimCone);
                projectile.transform.parent = null;
				projectile.transform.rotation = transform.rotation;
				projectile.GetComponent<Projectile>().itemID = AmmoType;
				projectile.GetComponent<Projectile>().JustFired = true;
				projectile.GetComponent<Projectile>().ShootNonRaycastType();

				// Lock the weapon after this discharge
				CanFire = false;
				// Reset the dElapsedTime to dTimeBetweenShots for the next shot
				ElapsedTime = TimeBetweenShots;
				// Reduce the rounds by 1
				if (!InfiniteAmmo)
					MagRounds--;

				return true;
			}
		}
		return false;
	}

}
