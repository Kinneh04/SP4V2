using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadiationProperties : MonoBehaviour
{
    float timer = 0;
    public float RadiationExpireTimer = 0.0f;
    public int radiationDamageMultiplier;
    public List<PlayerProperties> playersInZone = new List<PlayerProperties>();
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playersInZone.Add(other.GetComponent<PlayerProperties>());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playersInZone.Remove(other.GetComponent<PlayerProperties>());
        }
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if(timer > 1)
        {
            timer = 0;
            foreach(PlayerProperties pp in playersInZone)
            {
                pp.RadiationAmount += radiationDamageMultiplier;
                pp.RadiationExpireTimer = RadiationExpireTimer;
            }
        }
    }
}
