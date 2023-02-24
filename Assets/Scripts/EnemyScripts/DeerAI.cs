using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class DeerAI : Enemy
{
    List<GameObject> DetectedPredator = new List<GameObject>();
    enum FSM { IDLE, WANDER, RUN, DEAD };

    int MaxHealth = 100;
    public float MSpd = 2;
    float IdleTime;
    float MoveTime;
    float stamina = 100;
    bool isMoving = false;
    bool isRunning = false;
    float runTime;
    public bool change = false;
    NavMeshAgent navMeshAgent;
    Vector3 destination;
    Vector3 FromWhere;
    int enemyType;
    float hitTime;

    protected PhotonView PV;

    public GameObject Predator;

    FSM CurrentState;
    // Start is called before the first frame update
    public virtual void Awake()
    {
        MaxHealth = 100;
        Health = MaxHealth;
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        navMeshAgent.speed = MSpd;
        IdleTime = 0;
        MoveTime = 0;
        hitTime = 0;
        deadTime = 1;
        CurrentState = FSM.IDLE;
        enemyType = 0;
        PV = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (change)
        {
            if (TargetPlayer != null && Predator != null)
            {
                if (Vector3.Distance(transform.position, TargetPlayer.transform.position) < Vector3.Distance(transform.position, Predator.transform.position))
                {
                    FromWhere = TargetPlayer.transform.position;
                    enemyType = 1;
                }
                else
                {
                    FromWhere = Predator.transform.position;
                    enemyType = 2;
                }
            }
            else if (TargetPlayer != null)
            {
                FromWhere = TargetPlayer.transform.position;
                enemyType = 1;
            }
            else if (Predator != null)
            {
                FromWhere = Predator.transform.position;
                enemyType = 2;
            }
            change = false;
            isRunning = true;
            runTime = 2;
            navMeshAgent.speed = MSpd * 2.0f;
        }
        if (hitTime > 0)
        {
            hitTime -= Time.deltaTime;
            if (!isRunning)
            {
                isRunning = true;
                runTime = 2;
                navMeshAgent.speed = MSpd * 2.0f;
            }
            else if (TargetPlayer != null || Predator != null && hitTime > 0)
                change = true;
        }
        else if (TargetPlayer == null && Predator == null && isRunning)
        {
            navMeshAgent.speed = MSpd;
            enemyType = 0;
            if (runTime > 0)
                runTime -= Time.deltaTime;
            else
            {
                isRunning = false;
                isMoving = false;
                CurrentState = FSM.WANDER;
            }
        }
        if (enemyType == 1)
        {
            FromWhere = TargetPlayer.transform.position;
        }
        else if (enemyType == 2)
        {
            FromWhere = Predator.transform.position;
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
                    if (stamina < 100)
                        stamina += Time.deltaTime * 10;
                    else
                        stamina = 100;
                    if (IdleTime > 0)
                        IdleTime -= Time.deltaTime;
                    else
                    {
                        CurrentState = FSM.WANDER;
                        MoveTime = 0.5f;
                    }
                    break;
                }
            case FSM.WANDER:
                {
                    if (stamina < 100)
                        stamina += Time.deltaTime * 5;
                    else
                        stamina = 100;
                    if (isRunning)
                    {
                        if (stamina >= 100)
                        {
                            stamina = 100;
                            CurrentState = FSM.RUN;
                            isMoving = false;
                            MoveTime = 0.25f;
                            break;
                        }
                        if (isMoving)
                        {
                            if (Vector3.Distance(transform.position, destination) < 1 || MoveTime <= 0)
                            {
                                MoveTime = 0.25f;
                                isMoving = false;
                            }
                        }
                        else
                        {
                            Quaternion prev = transform.rotation;
                            transform.LookAt(transform.position + FromWhere);
                            Quaternion angleOfDanger = transform.rotation;
                            transform.rotation = prev;
                            float result = angleOfDanger.eulerAngles.y + Random.Range(-30, 30);

                            Vector3 direction = new Vector3(Mathf.Sin(result * Mathf.Deg2Rad), 0, Mathf.Cos(result * Mathf.Deg2Rad));

                            destination = transform.position + direction * 10;
                            navMeshAgent.SetDestination(destination);
                            isMoving = true;
                        }
                        if (MoveTime <= 0)
                        {
                            isMoving = false;
                            MoveTime = 0.25f;
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
                                IdleTime = Random.Range(0, 1);
                                CurrentState = FSM.IDLE;
                                break;
                            }
                        }
                        else
                        {
                            Quaternion prev = transform.rotation;
                            transform.LookAt(transform.position + transform.forward);
                            Quaternion angleOfDanger = transform.rotation;
                            transform.rotation = prev;
                            float result = angleOfDanger.eulerAngles.y + Random.Range(-30, 30);

                            Vector3 direction = new Vector3(Mathf.Sin(result * Mathf.Deg2Rad), 0, Mathf.Cos(result * Mathf.Deg2Rad));

                            destination = transform.position + direction * 10;
                            navMeshAgent.SetDestination(destination);
                            isMoving = true;
                            MoveTime = 0.5f;
                        }
                        if (MoveTime <= 0)
                        {
                            isMoving = false;
                            IdleTime = Random.Range(0, 1);
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
                                MoveTime = 0.25f;
                                isMoving = false;
                            }
                        }
                        else
                        {
                            Quaternion prev = transform.rotation;
                            transform.LookAt(transform.position + FromWhere);
                            Quaternion angleOfDanger = transform.rotation;
                            transform.rotation = prev;
                            float result = angleOfDanger.eulerAngles.y + Random.Range(-30, 30);

                            Vector3 direction = new Vector3(Mathf.Sin(result * Mathf.Deg2Rad), 0, Mathf.Cos(result * Mathf.Deg2Rad));

                            destination = transform.position + direction * 10;
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
                    {
                        Harvestable = true;
                        // PhotonNetwork.Destroy(gameObject);
                    }
                    break;
                }
        }
    }

    public void DamagedDirection(Vector3 direction)
    {
        FromWhere = transform.position - direction;
        hitTime = 2;
    }

    [PunRPC]
    void DeerFoundPlayer(int ID)
    {
        GameObject other = PhotonView.Find(ID).gameObject;
        DetectedPredator.Add(other);
        // Targeting
        DeerAI deer = GetComponent<DeerAI>();
        if (deer.TargetPlayer == null)
        {
            deer.TargetPlayer = other;
            deer.change = true;
        }
        else if (Vector3.Distance(deer.transform.position, deer.TargetPlayer.transform.position) > Vector3.Distance(deer.transform.position, other.transform.position))
        {
            deer.TargetPlayer = other;
            deer.change = true;
        }
    }

    [PunRPC]
    void DeerFoundWolf(int ID)
    {
        GameObject other = PhotonView.Find(ID).gameObject;
        DetectedPredator.Add(other);
        DeerAI deer = GetComponent<DeerAI>();
        if (deer.Predator == null)
        {
            deer.Predator = other;
            deer.change = true;
        }
        else if (Vector3.Distance(deer.transform.position, deer.Predator.transform.position) > Vector3.Distance(deer.transform.position, other.transform.position))
        {
            deer.Predator = other;
            deer.change = true;
        }
    }

    [PunRPC]
    void DeerLostPlayer(int ID)
    {
        GameObject other = PhotonView.Find(ID).gameObject;
        bool TargetLeft = false;
        DeerAI deer = GetComponent<DeerAI>();
        if (deer.TargetPlayer == other)
        {
            TargetLeft = true;
            deer.TargetPlayer = null;
            deer.change = true;
            DetectedPredator.Remove(other);
        }
        if (TargetLeft)
        {
            for (int i = 0; i < DetectedPredator.Count; i++)
            {
                if (deer.TargetPlayer == null)
                {
                    deer.TargetPlayer = DetectedPredator[0];
                }
                else if (Vector3.Distance(deer.transform.position, deer.TargetPlayer.transform.position) > Vector3.Distance(deer.transform.position, DetectedPredator[i].transform.position))
                {
                    deer.TargetPlayer = DetectedPredator[i];
                }
            }
        }
    }

    [PunRPC]
    void DeerLostWolf(int ID)
    {
        GameObject other = PhotonView.Find(ID).gameObject;
        bool PredatorLeft = false;
        DeerAI deer = GetComponent<DeerAI>();
        if (deer.Predator == other)
        {
            PredatorLeft = true;
            deer.Predator = null;
            deer.change = true;
            DetectedPredator.Remove(other);
        }
        if (PredatorLeft)
        {
            for (int i = 0; i < DetectedPredator.Count; i++)
            {
                if (deer.Predator == null)
                {
                    deer.Predator = DetectedPredator[0];
                }
                else if (Vector3.Distance(deer.transform.position, deer.Predator.transform.position) > Vector3.Distance(deer.transform.position, DetectedPredator[i].transform.position))
                {
                    deer.Predator = DetectedPredator[i];
                }
            }
        }
    }
}
