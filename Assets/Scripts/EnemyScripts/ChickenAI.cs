using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class ChickenAI : Enemy
{
    enum FSM { IDLE, WANDER, RUN, DEAD };

    int MaxHealth = 100;
    public float MSpd = 2;
    int Health;
    float IdleTime;
    float MoveTime;
    float stamina = 100;
    bool isMoving = false;
    bool isRunning = false;
    NavMeshAgent navMeshAgent;
    Vector3 destination;
    Vector3 FromWhere;
    float hitTime;

    PhotonView PV;

    public bool change = false;
    public GameObject Predator = null;

    FSM CurrentState;
    // Start is called before the first frame update
    void Awake()
    {
        MaxHealth = 100;
        Health = MaxHealth;
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        navMeshAgent.speed = MSpd;
        IdleTime = 2;
        MoveTime = 2;
        hitTime = 0;
        deadTime = 1;
        CurrentState = FSM.IDLE;
        PV = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hitTime > 0)
        {
            if (TargetPlayer == null && Predator == null)
                hitTime -= Time.deltaTime;
            else if (TargetPlayer != null && Predator != null)
            {
                if (Vector3.Distance(transform.position, TargetPlayer.transform.position) < Vector3.Distance(transform.position, Predator.transform.position))
                {
                    FromWhere = TargetPlayer.transform.position;
                }
                else
                {
                    FromWhere = Predator.transform.position;
                }
            }
            else if (TargetPlayer != null)
            {
                FromWhere = TargetPlayer.transform.position;
            }
            else if (Predator != null)
            {
                FromWhere = Predator.transform.position;
            }
            if (!isRunning)
            {
                isRunning = true;
                navMeshAgent.speed = MSpd * 1.5f;
            }
        }
        else if (CurrentState == FSM.RUN)
        {
            isRunning = false;
            isMoving = false;
            CurrentState = FSM.WANDER;
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
                    if (stamina < 50)
                        stamina += Time.deltaTime * 10;
                    else
                        stamina = 50;
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
                    if (stamina < 50)
                        stamina += Time.deltaTime * 5;
                    else
                        stamina = 50;
                    if (isRunning)
                    {
                        if (stamina >= 100)
                        {
                            stamina = 100;
                            CurrentState = FSM.RUN;
                            isMoving = false;
                            MoveTime = 2;
                            break;
                        }
                        if (isMoving)
                        {
                            if (Vector3.Distance(transform.position, destination) < 1 || MoveTime <= 0)
                            {
                                MoveTime = 2;
                                isMoving = false;
                            }
                        }
                        else
                        {
                            Quaternion prev = transform.rotation;
                            transform.LookAt(transform.position + FromWhere);
                            Quaternion angleOfDanger = transform.rotation;
                            transform.rotation = prev;
                            float result = angleOfDanger.eulerAngles.y + Random.Range(-45, 45);

                            Vector3 direction = new Vector3(Mathf.Sin(result * Mathf.Deg2Rad), 0, Mathf.Cos(result * Mathf.Deg2Rad));

                            destination = transform.position + direction * 5;
                            navMeshAgent.SetDestination(destination);
                            isMoving = true;
                        }
                        if (MoveTime <= 0)
                        {
                            isMoving = false;
                            MoveTime = 2;
                            break;
                        }
                        else if (MoveTime > 0)
                        {
                            MoveTime -= Time.deltaTime;
                        }
                    }
                    else
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
                            MoveTime = 2;
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
                    }
                    break;
                }
            case FSM.RUN:
                {
                    if (!isRunning)
                    {
                        isMoving = false;
                        CurrentState = FSM.WANDER;
                        break;
                    }

                    stamina -= Time.deltaTime * 10;
                    if (stamina > 0)
                    {
                        if (isMoving)
                        {
                            if (Vector3.Distance(transform.position, destination) < 2 || MoveTime <= 0)
                            {
                                MoveTime = 2;
                                isMoving = false;
                            }
                        }
                        else
                        {

                            Quaternion prev = transform.rotation;
                            transform.LookAt(transform.position + FromWhere);
                            Quaternion angleOfDanger = transform.rotation;
                            transform.rotation = prev;
                            float result = angleOfDanger.eulerAngles.y + Random.Range(-45, 45);

                            Vector3 direction = new Vector3(Mathf.Sin(result * Mathf.Deg2Rad), 0, Mathf.Cos(result * Mathf.Deg2Rad));

                            destination = transform.position + direction * 5;
                            navMeshAgent.SetDestination(destination);
                            isMoving = true;
                        }
                        if (MoveTime > 0)
                            MoveTime -= Time.deltaTime;
                    }
                    else
                    {
                        stamina = 0;
                        CurrentState = FSM.WANDER;
                        isMoving = false;
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
                    if (deadTime <= 0 && PV.IsMine)
                        PhotonNetwork.Destroy(gameObject);
                    break;
                }
        }
    }

    override public void GetDamaged(int damage)
    {
        Health -= damage;
    }

    public void DamagedDirection(Vector3 direction)
    {
        FromWhere = transform.position - direction;
        hitTime = 2;
    }
}
