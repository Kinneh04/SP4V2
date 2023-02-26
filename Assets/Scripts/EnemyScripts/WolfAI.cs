using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class WolfAI : Enemy
{
    enum FSM { IDLE, WANDER, ATTACK, DEAD };

    List<GameObject> DetectedPlayers = new List<GameObject>();
    List<Enemy> DetectedPrey = new List<Enemy>();

    int MaxHealth = 100;
    public float MSpd = 2;
    float IdleTime;
    float MoveTime;
    bool isMoving = false;
    public bool change = false;
    NavMeshAgent navMeshAgent;
    Vector3 destination;

    public Enemy Prey;

    float BiteCD = 0;

    float PounceCD = 0;
    bool Pouncing = false;

    GameObject Target;

    FSM CurrentState;

    // Start is called before the first frame update
    public virtual void Awake()
    {
        MaxHealth = 100;
        Health = MaxHealth;
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        navMeshAgent.speed = MSpd;
        IdleTime = 2;
        MoveTime = 2;
        deadTime = 1;
        CurrentState = FSM.IDLE;
        PV = GetComponent<PhotonView>();
    }
    private void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (!dead)
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
        }
        if (Health <= 0 && !dead)
        {
            audioManager.GetComponent<PhotonView>().RPC("MultiplayerPlay3DAudio", RpcTarget.All, (int)AudioManager.AudioID.Wolf, 1.0f, transform.position);
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
                        change = false;
                        if (TargetPlayer != null && Prey != null)
                        {
                            if (Vector3.Distance(transform.position, TargetPlayer.transform.position) < Vector3.Distance(transform.position, Prey.transform.position))
                            {
                                Target = TargetPlayer;
                            }
                            else
                            {
                                Target = Prey.gameObject;
                            }
                        }
                        else if (TargetPlayer != null)
                        {
                            Target = TargetPlayer;
                        }
                        else if (Prey != null)
                        {
                            Target = Prey.gameObject;
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
                    float distance = Vector3.Distance(transform.position, Target.transform.position);
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
                            //Debug.Log(destination);
                            destination = Target.transform.position;
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
                            destination = (Target.transform.position - transform.position) * 5 + transform.position;

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
                    if (deadTime <= 0 && PV.IsMine)
                    {
                        Harvestable = true;
                        // PhotonNetwork.Destroy(gameObject);
                    }
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
        else if (Target == Prey.gameObject)
        {
            if (BiteCD <= 0)
            {
                Enemy enemy = Prey.GetComponent<Enemy>();
                enemy.GetDamaged(10);
                BiteCD = 2;
                if (enemy.dead)
                {
                    Prey = null;
                    change = true;
                }
            }
        }
    }

    [PunRPC]
    void WolfFoundPlayer(int ID)
    {
        GameObject other = PhotonView.Find(ID).gameObject;
        DetectedPlayers.Add(other);
        // Targeting
        WolfAI wolf = GetComponent<WolfAI>();
        if (wolf.TargetPlayer == null)
        {
            wolf.TargetPlayer = other;
            wolf.change = true;
        }
        else if (Vector3.Distance(wolf.transform.position, wolf.TargetPlayer.transform.position) > Vector3.Distance(wolf.transform.position, other.transform.position))
        {
            wolf.TargetPlayer = other;
            wolf.change = true;
        }
    }

    [PunRPC]
    void WolfFoundPrey(int ID)
    {
        Enemy other = PhotonView.Find(ID).gameObject.GetComponent<Enemy>();
        if (other.dead)
            return;
        DetectedPrey.Add(other);
        WolfAI wolf = GetComponent<WolfAI>();
        if (wolf.Prey == null)
        {
            wolf.Prey = other;
            wolf.change = true;
        }
        else if (Vector3.Distance(wolf.transform.position, wolf.Prey.transform.position) > Vector3.Distance(wolf.transform.position, other.transform.position))
        {
            wolf.Prey = other;
            wolf.change = true;
        }
    }

    [PunRPC]
    void WolfLostPlayer(int ID)
    {
        GameObject other = PhotonView.Find(ID).gameObject;
        bool TargetLeft = false;
        WolfAI wolf = GetComponent<WolfAI>();
        if (wolf.TargetPlayer == other)
        {
            TargetLeft = true;
            wolf.TargetPlayer = null;
            wolf.change = true;
            DetectedPlayers.Remove(other);
        }
        if (TargetLeft)
        {
            for (int i = 0; i < DetectedPlayers.Count; i++)
            {
                if (wolf.TargetPlayer == null)
                {
                    wolf.TargetPlayer = DetectedPlayers[0];
                }
                else if (Vector3.Distance(wolf.transform.position, wolf.TargetPlayer.transform.position) > Vector3.Distance(wolf.transform.position, DetectedPlayers[i].transform.position))
                {
                    wolf.TargetPlayer = DetectedPlayers[i];
                }
            }
        }
    }

    [PunRPC]
    void WolfLostPrey(int ID)
    {
        Enemy other = PhotonView.Find(ID).gameObject.GetComponent<Enemy>();
        if (other.dead)
            return;
        bool PreyLeft = false;
        WolfAI wolf = gameObject.GetComponentInParent<WolfAI>();
        if (wolf.Prey == other)
        {
            PreyLeft = true;
            wolf.Prey = null;
            wolf.change = true;
            DetectedPrey.Remove(other);
        }
        if (PreyLeft)
        {
            for (int i = 0; i < DetectedPrey.Count; i++)
            {
                if (wolf.Prey == null)
                {
                    wolf.Prey = DetectedPrey[0];
                }
                else if (Vector3.Distance(wolf.transform.position, wolf.Prey.transform.position) > Vector3.Distance(wolf.transform.position, DetectedPrey[i].transform.position))
                {
                    wolf.Prey = DetectedPrey[i];
                }
            }
        }
    }
}
