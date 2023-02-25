using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnChange : MonoBehaviour
{

    public GameObject spawnObject;
    public float baseChance = 0.01f;
    public float chanceIncrease = 0.02f;
    public float increaseInterval = 5f;
    private float chanceTimer;

    void Start()
    {
        chanceTimer = increaseInterval;
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // decrement the timer every frame
            chanceTimer -= Time.deltaTime;

            // if the timer has reached 0 or less, increase the spawn chance
            if (chanceTimer <= 0f)
            {
                baseChance += chanceIncrease;
                chanceTimer = increaseInterval;

                float randomValue = Random.Range(0,100);

                // if the random value is less than the spawn chance, spawn the object
                if (randomValue < baseChance)
                {
                    print("SPAWNED HELI!");
                    PhotonNetwork.Instantiate(spawnObject.name, transform.position, Quaternion.identity);
                    baseChance = 0.01f;
                }
            }
        }
    }
}
