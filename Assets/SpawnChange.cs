using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnChange : MonoBehaviour
{

    public GameObject spawnObject;
    public float Countdown;
    public float maxCountdown;
    public float minCountdown;

    void Start()
    {
        if(PhotonNetwork.IsMasterClient)
        Countdown = Random.Range(maxCountdown, minCountdown);
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Countdown -= Time.deltaTime;
            if (Countdown <= 0)
            {
                PhotonNetwork.Instantiate(spawnObject.name, transform.position, Quaternion.identity);
                Countdown = Random.Range(maxCountdown, minCountdown);
            }
        }
    }
}
