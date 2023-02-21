using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Raycast : MonoBehaviour
{
    [SerializeField]
    GameObject SpawnBulletPosition;
    [SerializeField]
    private bool AddBulletSpread = true;
    [SerializeField]
    private Vector3 BulletSpreadVariance = new Vector3(0.1f, 0.1f, 0.1f);
    [SerializeField]
    private ParticleSystem ShootingSystem;
    public Transform BulletSpawnPoint;
    [SerializeField]
    private GameObject ImpactParticleSystem;
    [SerializeField]
    private GameObject BloodParticleSystem;
    [SerializeField]
    private TrailRenderer BulletTrail;
    [SerializeField]
    private float ShootDelay = 0.5f;
    [SerializeField]
    private LayerMask Mask;
    public float Damage;
    public GameObject ParentGunTip;
    private float LastShootTime;
    public ParticleSystem MuzzleFlash;
    public GameObject BulletImpact;


    // Start is called before the first frame update
    void Awake()
    {

    }
    public void Shoot()
    {
        //ShootingSystem.Play();   //shooting particle effect
        Vector3 direction = GetDirection();
        Ray ray = new Ray(ParentGunTip.transform.position, direction);
        RaycastHit hit;
        Instantiate(MuzzleFlash, ParentGunTip.transform.position, Quaternion.identity);
        if (Physics.Raycast(ray, out hit, float.MaxValue))
        {
            TrailRenderer trail = Instantiate(BulletTrail, ParentGunTip.transform.position, Quaternion.identity);
            StartCoroutine(SpawnTrail(trail, hit));
            GameObject GO;
            if (hit.transform.gameObject.CompareTag("Enemy"))
            {
                hit.transform.GetComponent<Enemy>().GetDamaged((int)Damage);
                GO = Instantiate(BloodParticleSystem, hit.point, Quaternion.identity);
            }
            else if (hit.transform.gameObject.CompareTag("Wolf"))
            {
                hit.transform.GetComponent<WolfAI>().GetDamaged((int)Damage);
                GO = Instantiate(BloodParticleSystem, hit.point, Quaternion.identity);
            }
            else if (hit.transform.gameObject.CompareTag("Tank"))
            {
                hit.transform.GetComponent<TankAI>().GetDamaged((int)Damage);
                GO = Instantiate(ImpactParticleSystem, hit.point, Quaternion.identity);
            }
            else if (hit.transform.gameObject.CompareTag("Deer"))
            {
                hit.transform.GetComponent<DeerAI>().GetDamaged((int)Damage);
                hit.transform.GetComponent<DeerAI>().DamagedDirection(direction);
                GO = Instantiate(ImpactParticleSystem, hit.point, Quaternion.identity);
            }
            else if (hit.transform.gameObject.CompareTag("Chicken"))
            {
                hit.transform.GetComponent<ChickenAI>().GetDamaged((int)Damage);
                hit.transform.GetComponent<ChickenAI>().DamagedDirection(direction);
                GO = Instantiate(ImpactParticleSystem, hit.point, Quaternion.identity);
            }
            else if (hit.transform.gameObject.CompareTag("Player"))
            {
                hit.transform.GetComponent<PlayerProperties>().TakeDamage(Damage);
                GO = Instantiate(BloodParticleSystem, hit.point, Quaternion.identity);
            }
            else
            {

                try
                {
                    Color hitColor = hit.transform.gameObject.GetComponent<Renderer>().material.color;
                    GO = Instantiate(BulletImpact, hit.point, Quaternion.identity);
                    GO.GetComponentInChildren<ParticleSystem>().startColor = hitColor;
                    Destroy(GO, 1);
                }
                catch (Exception e)
                {
                    return;
                }
            }
            Destroy(GO, 1);
        }
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;

        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hit.point, time);
            time += Time.deltaTime / trail.time;
            yield return null;
        }
        trail.transform.position = hit.point;
        Instantiate(ImpactParticleSystem, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(trail.gameObject, trail.time);
    }

    public void SetAimCone(float AimCone)
    {
        BulletSpreadVariance.x = BulletSpreadVariance.y = BulletSpreadVariance.z = 10 * (AimCone / 90);
    }
    private Vector3 GetDirection()
    {
        Vector3 direction = BulletSpawnPoint.forward;
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

    }
}
