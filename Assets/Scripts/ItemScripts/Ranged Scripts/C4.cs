using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class C4 : WeaponInfo
{

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
		MagRounds = ItemCount;
	}
    
    // Discharge this weapon
    public override bool Discharge(Transform transform)
	{
		if (CanFire)
		{
			// If there is still ammo in the magazine, then fire
			if (ItemCount > 0)
			{
				//Get Player PhotonView
				int PhotonViewID = PhotonNetwork.Instantiate("C4_Projectile", this.BarrelTip.transform.position, Quaternion.identity).GetComponent<PhotonView>().ViewID;
				PhotonView ProjectilephotonView = GameObject.FindGameObjectWithTag("Player").GetComponent<PhotonView>();
				ProjectilephotonView.RPC("DefaultProjectileInit", RpcTarget.All, PhotonViewID);
				MagRounds = ItemCount - 1;
				// Lock the weapon after this discharge
				CanFire = false;
				//Doesnt need to reload
				// Reduce the rounds by 1
				//ItemCount-=1;
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
