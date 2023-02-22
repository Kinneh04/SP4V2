using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HandmadeShotgun : WeaponInfo
{
	public int PelletCount = 16;
	public GameObject BarrlTip;
	
	//This is REQUIRED for muzzle flare;
	// Dont change it or it will NOT shoot;
	public override void Init()
    {
        MagRounds = MaxMagRounds = 1;
        FiringType = FIRINGTYPE.SEMI_AUTO;
        GunName = GUNNAME.HOMEMADE_SHOTGUN;
		AmmoType = ItemID.ShotgunAmmo;
		Damage = 7;
        TimeBetweenShots = 1.5f;
		BarrelTip = BarrlTip;
        ElapsedTime = ReloadTime = 0;
        MaxReloadTime = 1.4f;
        CanFire = false;
        AimCone = 2.0f;
        InfiniteAmmo = false;
        DrawTime = 1;
        itemID = ItemID.Handmade_Shotgun;
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
					PhotonView ProjectilephotonView = GameObject.FindGameObjectWithTag("Player").GetComponent<PhotonView>();
					ProjectilephotonView.RPC("DefaultBulletInit", RpcTarget.All);
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
