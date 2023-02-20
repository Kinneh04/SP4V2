using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C4 : WeaponInfo
{
	public GameObject BarrlTip;

	public override void Init()
    {
        MagRounds = MaxMagRounds = 1;
        FiringType = FIRINGTYPE.SEMI_AUTO;
        GunName = GUNNAME.C4;
		AmmoType = ItemID.C4;
		Damage = 550;
		TimeBetweenShots = 0.0f;
        ElapsedTime = ReloadTime = 0;
        MaxReloadTime = 0.0f;
        CanFire = false;
        AimCone = 0.0f;
		InfiniteAmmo = false;
		DrawTime = 1;
        itemID = ItemID.C4;
        itemType = ItemType.Ranged;
        MaxItemCount = 10;
    }


    // Discharge this weapon
    public override bool Discharge(Transform transform)
	{
		if (CanFire)
		{
			// If there is still ammo in the magazine, then fire
			if (ItemCount > 0)
			{
				GameObject projectile = Instantiate(BulletPrefab, BarrlTip.transform.position, Quaternion.identity);
				projectile.GetComponent<Projectile>().Damage = Damage;
				projectile.GetComponent<Projectile>().BulletSpawnPoint = transform;
				projectile.GetComponent<Projectile>().ParentGunTip = BarrlTip;
				projectile.GetComponent<Projectile>().SetAimCone(AimCone);
				projectile.GetComponent<Rigidbody>().isKinematic = false;
				projectile.transform.parent = null;
				projectile.transform.rotation = transform.rotation;
				projectile.GetComponent<Projectile>().JustFired = true;
				projectile.GetComponent<Projectile>().itemID = AmmoType;
				projectile.GetComponent<Projectile>().ExplosionTimer = 3;
				projectile.GetComponent<Projectile>().ShootNonRaycastType();

				// Lock the weapon after this discharge
				CanFire = false;
				//Doesnt need to reload
				// Reduce the rounds by 1
				ItemCount-=1;
				if (ItemCount <= 0)
				{
					Destroy(this.gameObject);
				}
				return true;
			}
		}
		return false;
	}

}
