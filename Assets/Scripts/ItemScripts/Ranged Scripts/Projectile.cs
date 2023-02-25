using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
public class Projectile : MonoBehaviour
{
    [SerializeField]
    private bool AddBulletSpread = true;
    [SerializeField]
    private Vector3 BulletSpreadVariance = new Vector3(0.1f, 0.1f, 0.1f);
    public Transform BulletSpawnPoint;
    [SerializeField]
    private GameObject ImpactParticleSystem;
    [SerializeField]
    private GameObject BloodParticleSystem;
    [SerializeField]
    private TrailRenderer BulletTrail;
    [SerializeField]
    public float Damage;
    public GameObject ParentGunTip;
    private float LastShootTime;
    public ParticleSystem MuzzleFlash;
    public GameObject BulletImpact;
    public bool JustFired = false;
    public GameObject Explosion;
    public ItemInfo.ItemID itemID;
    public float ExplosionTimer = 0;

    private void OnCollisionEnter(Collision collision)
    {
        if (JustFired)
        {
            if (itemID == ItemInfo.ItemID.Rocket)
            {
                GameObject Go = PhotonNetwork.Instantiate(Explosion.name, transform.position,Quaternion.identity);
                Go.GetComponent<Explosion>().Damage = Damage;
                Destroy(this.gameObject);
            }
            else if (itemID == ItemInfo.ItemID.C4)
            {
                this.GetComponent<Rigidbody>().isKinematic = true;
                this.transform.parent = collision.transform.parent;
            }
            else if (itemID == ItemInfo.ItemID.Arrow)
            {
                if (collision.transform.gameObject.CompareTag("Enemy"))
                {
                    collision.transform.GetComponent<Enemy>().GetDamaged((int)Damage);
                    Instantiate(BloodParticleSystem, collision.transform.position, Quaternion.identity);
                }
                else if (collision.transform.gameObject.CompareTag("Wolf"))
                {
                    collision.transform.GetComponent<WolfAI>().GetDamaged((int)Damage);
                    Instantiate(BloodParticleSystem, collision.transform.position, Quaternion.identity);
                }
                else if (collision.transform.gameObject.CompareTag("Tank"))
                {
                    collision.transform.GetComponent<TankAI>().GetDamaged((int)Damage);
                    Instantiate(ImpactParticleSystem, collision.transform.position, Quaternion.identity);
                }
                else if (collision.transform.gameObject.CompareTag("Deer"))
                {
                    collision.transform.GetComponent<DeerAI>().GetDamaged((int)Damage);
                    Instantiate(BloodParticleSystem, collision.transform.position, Quaternion.identity);
                }
                else if (collision.transform.gameObject.CompareTag("Chicken"))
                {
                    collision.transform.GetComponent<ChickenAI>().GetDamaged((int)Damage);
                    Instantiate(BloodParticleSystem, collision.transform.position, Quaternion.identity);
                }
                else if (collision.transform.gameObject.CompareTag("Player") || collision.transform.gameObject.CompareTag("EnemyPlayer"))
                {
                    collision.transform.GetComponent<PlayerProperties>().TakeDamageV2(Damage);
                    Instantiate(BloodParticleSystem, collision.transform.position, Quaternion.identity);
                }
                Vector3 PushPreviousDirection; // so the arrow is doesnt go through the wall
                PushPreviousDirection = GetDirection();
                PushPreviousDirection *= 0.05f;
                this.transform.position -= PushPreviousDirection;
                this.GetComponent<Rigidbody>().isKinematic = true;
                this.transform.parent = collision.transform.parent;

            }
            JustFired = false;
        }
    }
    // Start is called before the first frame update
    void Awake()
    {

    }

    public void ShootNonRaycastType()
    {

        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            if (itemID == ItemInfo.ItemID.Arrow)
                rb.velocity = BulletSpawnPoint.transform.forward * 50;
            else if (itemID == ItemInfo.ItemID.Rocket)
                rb.velocity = BulletSpawnPoint.transform.forward * 100;
            else if (itemID == ItemInfo.ItemID.C4)
                rb.velocity = BulletSpawnPoint.transform.forward * 10;
        }
    }
    public void Shoot()
    {
        this.transform.position += GetDirection() * 1;
        this.transform.rotation = Quaternion.Euler(GetDirection().x, GetDirection().y, GetDirection().z);
        this.GetComponent<Rigidbody>().isKinematic = false;
        this.GetComponent<Rigidbody>().velocity = transform.forward * 200;

    }
    public void SetAimCone(float AimCone)
    {
        BulletSpreadVariance.x = BulletSpreadVariance.y = BulletSpreadVariance.z = 10 * (AimCone / 90);
    }
    private Vector3 GetDirection()
    {
        Vector3 direction = BulletSpawnPoint.transform.forward;
        if (AddBulletSpread)
        {
            direction += new Vector3(
                UnityEngine.Random.Range(-BulletSpreadVariance.x, BulletSpreadVariance.x),
                 UnityEngine.Random.Range(-BulletSpreadVariance.y, BulletSpreadVariance.y),
                 UnityEngine.Random.Range(-BulletSpreadVariance.z, BulletSpreadVariance.z)
                );
        }
        direction.Normalize();
        return direction;
    }
    // Update is called once per frame
    void Update()
    {
        if (!JustFired && itemID == ItemInfo.ItemID.C4)
        {
            if (ExplosionTimer > 0)
            {
                ExplosionTimer -= Time.deltaTime;
            }
            else
            {
                GameObject Go = PhotonNetwork.Instantiate(Explosion.name, transform.position, Quaternion.identity);
                Go.GetComponent<Explosion>().Damage = Damage;
                Destroy(this.gameObject);
            }
        }
    }
}