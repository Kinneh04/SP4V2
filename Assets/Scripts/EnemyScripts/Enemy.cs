using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class Enemy : MonoBehaviour
{
    public GameObject TargetPlayer = null;
    public GameObject Structure = null;
    public bool dead = false;
    protected float deadTime;

    public virtual void GetDamaged(int damage)
    {
    }
}
