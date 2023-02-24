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

    public void GetDamaged(int damage)
    {
        Health -= damage;
    }
}
