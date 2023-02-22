using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RandomItem : MonoBehaviour
{
    public float minSpawnTimer;
    public GameObject ItemInPlace;
    public float maxSpawnTimer;
    public float timer = 0.0f;
    public List<GameObject> ItemsInPool = new List<GameObject>();

    // Update is called once per frame

    private void Start()
    {
        timer = Random.Range(minSpawnTimer, maxSpawnTimer);
    }
    void FixedUpdate()
    {
        if (ItemInPlace == null)
        {
            timer -= Time.deltaTime;

            if(timer <= 0)
            {
                int i = Random.Range(0, ItemsInPool.Count);

                GameObject ItemGO = PhotonNetwork.Instantiate(ItemsInPool[i].name, transform.position, Quaternion.identity);
                ItemInPlace = ItemGO;
            }
        }
        else if(timer <= 0)
        {
            timer = Random.Range(minSpawnTimer, maxSpawnTimer);
        }
    }
}
