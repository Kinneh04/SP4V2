using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponInfo : ItemInfo
{
	public enum FIRINGTYPE
	{
		SEMI_AUTO = 0,
		FULL_AUTO,
		NUM_FIRINGTYPE
	};
	public enum GUNNAME
	{
		HOMEMADE_PISTOL = 0,
		M1911_PISTOL,
		AK47,
		HOMEMADE_SHOTGUN,
		GUN,
		HUNTING_BOW,
		REVOLVER,
		MP5A4,
		REMINGTON870,
		BOLT_ACTION_RIFLE,
		ROCKETLAUNCHER,
		NUM_GUNNAME
	};
	GameObject pm;
	GameObject ProjectilePrefab;
	// The number of ammunition in a magazine for this weapon
	public int MagRounds;
	// The maximum number of ammunition for this magazine for this weapon
	protected int MaxMagRounds;
	//Weapon firing type;
	protected FIRINGTYPE FiringType;
	//Weapon Damage
	protected float Damage;
	protected GUNNAME GunName;
	protected GameObject BarrelTip;
	protected ItemID AmmoType;
	// The time between shots in milliseconds
	protected double TimeBetweenShots;
	// The elapsed time (between shots) in milliseconds
	protected double ElapsedTime;
	// The elapsed time for reloading of a magazine in milliseconds
	protected double ReloadTime;
	// The maximum elapsed time for reloading of a magazine in milliseconds
	protected double MaxReloadTime;
	// Boolean flag to indicate if weapon can fire now
	protected bool CanFire;
	// the bullet spread when firing , in degrees
	protected float AimCone;
	// if have infinite ammo to reload
	protected bool InfiniteAmmo;
	// time it takes to draw the gun
	protected float DrawTime;

	protected float yrecoil;
	protected float xrecoil;

	public GameObject BulletPrefab;


	private void Start()
	{
		Init();
	}

	// Update is called once per frame
	void  Update()
	{// If the weapon can fire, then just fire and return
		if (!CanFire)
		{
			// Update the dReloadTime
			if (ReloadTime >= 0.0f)
			{
				// Reduce the dReloadTime
				ReloadTime -= Time.deltaTime;
			}
			else
			{
				// Set reload time to 0.0f since reloading is completed
				ReloadTime = 0.0f;

				// Update the elapsed time
				if (ElapsedTime > 0.0f)
				{
					ElapsedTime -= Time.deltaTime;
				}
				else
				{
					ElapsedTime = 0;
					CanFire = true;
				}
			}
		}

	}

	// Set the number of ammunition in the magazine for this player
	public void SetMagRound(int newMagRounds)
	{
		MagRounds = newMagRounds;
	}
	// Set the maximum number of ammunition in the magazine for this weapon
	public void SetMaxMagRound(int newMaxMagRounds)
	{
		MaxMagRounds = newMaxMagRounds;
	}

	// Get the number of ammunition in the magazine for this player
	public int GetMagRound()
	{
		return MagRounds;
	}
	// Get the maximum number of ammunition in the magazine for this weapon
	public int GetMaxMagRound()
	{
		return MaxMagRounds;
	}
	
	// Get the current total number of rounds currently carried by this player
	public FIRINGTYPE GetFiringType()
	{
		return FiringType;
	}
	public string sGetFiringType()
	{
		return FiringType.ToString();
	}
	// Set the max total number of rounds currently carried by this player
	public void SetFiringType(FIRINGTYPE newFiringType) 
	{
		FiringType = newFiringType;
	}
	// Get the gun name
	public GUNNAME GetGunName() 
	{
		return GunName;
	}
	public string sGetGunName() 
	{
		switch ((int)GunName)
		{
			case 0:
				return "Homemade Pistol";
			case 1:
				return "M1911 Pistol";
			case 2:
				return "AK47";
			case 3:
				return "Homemade Shotgun";
			case 4:
				return "Gun";
			case 5:
				return "Hunting Bow";
			default:
				return "Gun";
        }
	}
	// Set the gun name
	public void SetGunName(GUNNAME newGunName) 
	{
		GunName = newGunName;
	}
	// Set the time between shots
	public void SetTimeBetweenShots(double newTimeBetweenShots) 
	{
		TimeBetweenShots = newTimeBetweenShots;
	}
	// Set the firing rate in rounds per min
	public void SetFiringRate(int newFiringRate)
	{
		TimeBetweenShots = 60.0 / (double)newFiringRate;  // 60 seconds divided by firing rate
	}
	// Set the firing flag
	virtual public void SetCanFire(bool newCanFire) 
	{
		CanFire = newCanFire;
	}
	// Set the weapon damage
	public void SetDamage(float newDamage)
	{
		Damage = newDamage;
	}
	// Set the ammo type the weapon uses
	public void SetAmmoType(ItemID newAmmoType)
	{
		AmmoType = newAmmoType;
	}

	// Get the time between shots
	public double GetTimeBetweenShots() 
	{
		return TimeBetweenShots;
	}
	// Get the firing rate
	public int GetFiringRate() 
	{
		return (int)(60.0 / TimeBetweenShots);
	}
	// Get the firing flag
	public bool GetCanFire() 
	{
		return CanFire;
	}
	// Get the weapon damage
	public float GetDamage() 
	{
		return Damage;
	}
	// Get the ammo type the weapon uses
	public ItemID GetAmmoType()
	{
		return AmmoType;
	}
	// Set InfiniteAmmo
	public void SetInfiniteAmmo(bool IsInfiniteAmmo)
	{
		InfiniteAmmo = IsInfiniteAmmo;
	}
	// Get InfiniteAmmo
	public bool GetInfiniteAmmo()
	{
		return InfiniteAmmo;
	}
	public void SetAimCone(float newAimCone)
	{
		AimCone = newAimCone;
	}
	public float GetAimCone()
	{
		return AimCone;
	}
	public void SetDrawTime(float newDrawTime)
	{
		DrawTime = newDrawTime;
	}
	public float GetDrawTime()
	{
		return DrawTime;
	}
	//adds DrawTime to ElaspedTime and prevent it from shooting
	public void DrawWeapon()
    {
		ElapsedTime = DrawTime;
		CanFire = false;
    }
	// Discharge this weapon
	virtual public bool Discharge(Transform transform) 
	{
		if (CanFire)
		{
			// If there is still ammo in the magazine, then fire
			if (MagRounds > 0 || InfiniteAmmo)
			{
				GameObject projectile = Instantiate(BulletPrefab, transform.position, Quaternion.identity);
				projectile.GetComponent<Raycast>().Damage = Damage;
				projectile.GetComponent<Raycast>().BulletSpawnPoint = transform;
                projectile.GetComponent<Raycast>().ParentGunTip = BarrelTip;
                projectile.GetComponent<Raycast>().SetAimCone(AimCone);
				projectile.GetComponent<Raycast>().Shoot();
				// Lock the weapon after this discharge
				CanFire = false;
				// Reset the dElapsedTime to dTimeBetweenShots for the next shot
				ElapsedTime = TimeBetweenShots;
				// Reduce the rounds by 1
				MagRounds--;
            
                return true;
			}

		
		}

		//cout << "Unable to discharge weapon." << endl;

		return false;
	}
	// Reload this weapon
	virtual public void Reload() 
	{
		// If the weapon is already reloading, then don't reload again
		if (ReloadTime > 0.0f)
			return;

		
		if(InfiniteAmmo)
		{
			MagRounds = MaxMagRounds;
			// Set the elapsed time for reloading of a magazine to dMaxReloadTime
			ReloadTime = MaxReloadTime;
			// Disable the weapon's ability to discharge
			CanFire = false;
		}
		else if (MagRounds < MaxMagRounds) // Check if there is enough bullets
		{
			InventoryManager inventoryManager = gameObject.transform.parent.parent.parent.GetComponentInChildren<InventoryManager>();
			int TotalRounds = inventoryManager.GetAmmoQuantity(AmmoType); 
			if (MaxMagRounds - MagRounds <= TotalRounds)
			{
				inventoryManager.Remove(AmmoType, MaxMagRounds - MagRounds, false);
				//TotalRounds -= MaxMagRounds - MagRounds;
				MagRounds = MaxMagRounds;
			}
			else
			{
				MagRounds += TotalRounds;
				inventoryManager.Remove(AmmoType, TotalRounds, false);
				TotalRounds = 0;
			}
			// Set the elapsed time for reloading of a magazine to dMaxReloadTime
			ReloadTime = MaxReloadTime;
			// Disable the weapon's ability to discharge
			CanFire = false;
		}
	}

}
