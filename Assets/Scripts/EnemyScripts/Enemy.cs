using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using Photon.Pun;

public class Enemy : MonoBehaviour
{
    public GameObject TargetPlayer = null;
    public GameObject Structure = null;
    public bool dead = false;
    protected float deadTime;
    protected int Health;
    public bool Harvestable = false;

    public virtual void GetDamaged(int damage)
    {
        Health -= damage;
    }
    [PunRPC]
    public void DefaultRaycastInit()
    {
        WeaponInfo weaponInfo = gameObject.GetComponentInChildren<WeaponInfo>();
        GameObject Raycast = Instantiate(weaponInfo.BulletPrefab, transform.position, Quaternion.identity);
        Raycast.GetComponent<Raycast>().Damage = weaponInfo.GetDamage();
        Raycast.GetComponent<Raycast>().BulletSpawnPoint = gameObject.transform;
        Raycast.GetComponent<Raycast>().ParentGunTip = weaponInfo.BarrelTip;
        Raycast.GetComponent<Raycast>().SetAimCone(weaponInfo.GetAimCone());
        Raycast.GetComponent<Raycast>().Shoot();
    }
    [PunRPC]
    public void DefaultProjectileInit(int PhotonViewID)
    {
        WeaponInfo weaponInfo = gameObject.GetComponentInChildren<WeaponInfo>();
        GameObject Projectile = PhotonView.Find(PhotonViewID).gameObject;
        Projectile.GetComponent<Projectile>().Damage = weaponInfo.GetDamage();
        Projectile.GetComponent<Projectile>().BulletSpawnPoint = transform;
        Projectile.GetComponent<Projectile>().ParentGunTip = weaponInfo.BarrelTip;
        Projectile.GetComponent<Projectile>().SetAimCone(weaponInfo.GetAimCone());
        Projectile.transform.parent = null;
        Projectile.transform.rotation = weaponInfo.transform.rotation;
        Projectile.GetComponent<Projectile>().JustFired = true;
        Projectile.GetComponent<Projectile>().itemID = weaponInfo.GetAmmoType();
        Projectile.GetComponent<Projectile>().ExplosionTimer = 3;
        Projectile.GetComponent<Projectile>().ShootNonRaycastType();
    }
}
