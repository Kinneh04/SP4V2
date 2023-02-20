using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WolfAI : Enemy
{
    enum FSM { IDLE, WANDER, ATTACK, DEAD };

    int MaxHealth = 100;
    public float MSpd = 2;
    int Health;
    float IdleTime;
    float MoveTime;
    bool isMoving = false;
    public bool change = false;
    NavMeshAgent navMeshAgent;
    Vector3 destination;

    public GameObject Prey;

    float BiteCD = 0;

    float PounceCD = 0;
    bool Pouncing = false;

    GameObject Target;

    FSM CurrentState;

    // Start is called before the first frame update
    void Start()
    {
        MaxHealth = 100;
        Health = MaxHealth;
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        navMeshAgent.speed = MSpd;
        IdleTime = 2;
        MoveTime = 2;
        deadTime = 1;
        CurrentState = FSM.IDLE;
    }

    // Update is called once per frame
    void Update()
    {
        PounceCD -= Time.deltaTime;
        if (Pouncing)
        {
            if (PounceCD < 5)
            {
                navMeshAgent.speed = MSpd;
                Pouncing = false;
            }
        }

        if (BiteCD > 0)
            BiteCD -= Time.deltaTime;

        if (CurrentState != FSM.ATTACK && (TargetPlayer != null || Prey != null))
        {
            CurrentState = FSM.ATTACK;
            navMeshAgent.speed = MSpd * 1.2f;
        }

        if (Health <= 0)
        {
            CurrentState = FSM.DEAD;
            dead = true;
            navMeshAgent.Stop();
        }

        switch (CurrentState)
        {
            case FSM.IDLE:
                {
                    if (IdleTime > 0)
                        IdleTime -= Time.deltaTime;
                    else
                    {
                        CurrentState = FSM.WANDER;
                        MoveTime = 2;
                    }
                    break;
                }
            case FSM.WANDER:
                {
                    if (isMoving)
                    {
                        if (Vector3.Distance(gameObject.transform.position, destination) < 1)
                        {
                            isMoving = false;
                            IdleTime = Random.Range(1, 5);
                            CurrentState = FSM.IDLE;
                            break;
                        }
                    }
                    else
                    {
                        float x = Random.Range(-5, 5);
                        float z = Random.Range(-5, 5);
                        destination = transform.position;
                        destination += new Vector3(x, 0.0f, z);
                        navMeshAgent.SetDestination(destination);
                        isMoving = true;
                    }

                    if (MoveTime <= 0)
                    {
                        isMoving = false;
                        IdleTime = Random.Range(1, 5);
                        CurrentState = FSM.IDLE;
                        break;
                    }
                    else if (MoveTime > 0)
                    {
                        MoveTime -= Time.deltaTime;
                    }
                    break;
                }
            case FSM.ATTACK:
                {
                    if (change)
                    {
                        if (TargetPlayer != null && Prey != null)
                        {
                            if (Vector3.Distance(transform.position, TargetPlayer.transform.position) < Vector3.Distance(transform.position, Prey.transform.position))
                            {
                                Target = TargetPlayer;
                            }
                            else
                            {
                                Target = Prey;
                            }
                        }
                        else if (TargetPlayer != null)
                        {
                            Target = TargetPlayer;
                        }
                        else if (Prey != null)
                        {
                            Target = Prey;
                        }
                        else
                        {
                            isMoving = false;
                            IdleTime = Random.Range(1, 5);
                            CurrentState = FSM.IDLE;
                            navMeshAgent.speed = MSpd;
                            break;
                        }
                    }
                    float distance = Vector3.Distance(transform.position, TargetPlayer.transform.position);
                    if (isMoving)
                    {
                        if (Vector3.Distance(gameObject.transform.position, destination) < 1)
                        {
                            isMoving = false;
                        }
                    }
                    else
                    {
                        if (!Pouncing)
                        {
                            Debug.Log(destination);
                            destination = TargetPlayer.transform.position;
                            navMeshAgent.SetDestination(destination);
                            isMoving = true;
                        }
                        if (distance < 5)
                            MoveTime = 0.5f;
                        else
                            MoveTime = 1;
                    }
                    if (distance < 5 && distance > 4)
                    {
                        if (PounceCD <= 0)
                        {
                            // Attack
                            
                            PounceCD = 6;
                            Pouncing = true;
                            navMeshAgent.speed = 10;
                            destination = (TargetPlayer.transform.position - transform.position) * 5 + transform.position;

                            navMeshAgent.SetDestination(destination);
                            isMoving = false;
                        }
                    }
                    else if (distance < 4)
                    {
                        Attack();
                    }
                    if (MoveTime <= 0)
                    {
                        isMoving = false;
                        break;
                    }
                    else if (MoveTime > 0)
                    {
                        MoveTime -= Time.deltaTime;
                    }
                    break;
                }
            case FSM.DEAD:
                {
                    if (dead)
                    {
                        Vector3 direction = Vector3.RotateTowards(transform.forward, Vector3.up, Mathf.Deg2Rad * (1 - deadTime) * 90, 0);
                        transform.LookAt(direction + transform.position);
                        deadTime -= Time.deltaTime;
                    }
                    if (deadTime <= 0)
                        Destroy(gameObject);
                    break;
                }
        }
    }

    void Attack()
    {
        if (Target == TargetPlayer)
        {
            if (BiteCD <= 0)
            {
                PlayerProperties player = TargetPlayer.GetComponent<PlayerProperties>();
                player.TakeDamage(10);
                BiteCD = 2;
            }
        }
        else if (Target == Prey)
        {
            if (BiteCD <= 0)
            {
                Enemy enemy = Prey.GetComponent<Enemy>();
                enemy.GetDamaged(10);
                BiteCD = 2;
            }
        }
    }

    override public void GetDamaged(int damage)
    {
        Health -= damage;
    }
}
