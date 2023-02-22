using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
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
				PhotonView projectile = PhotonNetwork.Instantiate("missile", transform.position, Quaternion.identity).GetComponent<PhotonView>();
				projectile.gameObject.GetComponent<Projectile>().Damage = Damage;
				projectile.gameObject.GetComponent<Projectile>().BulletSpawnPoint = transform;
				projectile.gameObject.GetComponent<Projectile>().ParentGunTip = BarrlTip;
				projectile.gameObject.GetComponent<Projectile>().SetAimCone(AimCone);
                projectile.gameObject.transform.parent = null;
				projectile.gameObject.transform.rotation = transform.rotation;
				projectile.gameObject.GetComponent<Projectile>().itemID = AmmoType;
				projectile.gameObject.GetComponent<Projectile>().JustFired = true;
				projectile.gameObject.GetComponent<Projectile>().ShootNonRaycastType();


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
