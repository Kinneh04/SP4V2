using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bow : WeaponInfo
{

	public bool charged = false;

	public override void Init()
    {
        MagRounds = MaxMagRounds = 1;
        FiringType = FIRINGTYPE.SEMI_AUTO;
        GunName = GUNNAME.HUNTING_BOW;
		AmmoType = ItemID.Arrow;
		Damage = 35;
		TimeBetweenShots = 0.6f;
        ElapsedTime = ReloadTime = 0;
        MaxReloadTime = 0.5f;
        CanFire = false;
        AimCone = 1.0f;
		InfiniteAmmo = false;
		DrawTime = 1;
        itemID = ItemID.Hunting_Bow;
        itemType = ItemType.Ranged;
        MaxItemCount = 1;
        ItemCount = 1;
    }

	public IEnumerator StartCharge()
	{
		yield return new WaitForSeconds(0.4f);
        if (Input.GetMouseButton(0)) charged = true;
	}

    // Discharge this weapon
    public override bool Discharge()
	{
		if (CanFire && charged)
		{
			// If there is still ammo in the magazine, then fire
			if (MagRounds > 0)
			{
				//Get Player PhotonView
				int PhotonViewID = PhotonNetwork.Instantiate("Arrow", this.BarrelTip.transform.position, Quaternion.identity).GetComponent<PhotonView>().ViewID;
				PhotonView ProjectilephotonView = GameObject.FindGameObjectWithTag("Player").GetComponent<PhotonView>();
				ProjectilephotonView.RPC("DefaultProjectileInit", RpcTarget.All, PhotonViewID);
				// Lock the weapon after this discharge
				CanFire = false;
				charged = false;
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
