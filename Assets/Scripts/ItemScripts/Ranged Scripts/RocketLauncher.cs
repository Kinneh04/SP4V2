using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class RocketLauncher : WeaponInfo
{
	public GameObject Miniexplosion;
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
				//Get Player PhotonView
				PhotonView ProjectilephotonView = GameObject.FindGameObjectWithTag("Player").GetComponent<PhotonView>();
				ProjectilephotonView.RPC("DefaultProjectileInit", RpcTarget.All);

				Instantiate(Miniexplosion, BarrelTip.transform.position, Quaternion.identity);
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
