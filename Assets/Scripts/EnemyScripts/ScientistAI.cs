using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class ScientistAI : Enemy
{
    enum FSM { IDLE, PATROL, WANDER, ATTACK, DEAD };
    public enum ENEMY_TYPE { SCIENTIST, ARMOURED_SCIENTIST, TANK };


    protected List<GameObject> DetectedPlayers = new List<GameObject>();

    int MaxHealth = 100;
    public float MSpd = 2;
    float IdleTime;
    float MoveTime;
    bool StructureFound = false;
    bool isMoving = false;
    bool GoingMonument = false;
    private NavMeshAgent navMeshAgent;
    Vector3 destination;
    float TimebetweenShots;
    FSM CurrentState;

    public WeaponInfo gun;

    public ENEMY_TYPE EnemyType;

    // Start is called before the first frame update
    public virtual void Awake()
    {
        CurrentState = FSM.IDLE;
        if (EnemyType == ENEMY_TYPE.SCIENTIST)
            MaxHealth = 150;
        else if (EnemyType == ENEMY_TYPE.ARMOURED_SCIENTIST)
            MaxHealth = 400;

        Health = MaxHealth;
        IdleTime = 0;
        deadTime = 1;
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        TargetPlayer = null;
        navMeshAgent.speed = MSpd;
        gun.Init();
        gun.SetInfiniteAmmo(true);
        gun.SetTimeBetweenShots(0.33f);
        gun.SetCanFire(true);
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
            gun.SetInfiniteAmmo(true);

            if (Structure != null && !StructureFound)
            {
                StructureFound = true;
            }
            if (CurrentState != FSM.ATTACK && TargetPlayer != null && !GoingMonument)
            {
                CurrentState = FSM.ATTACK;
            }
        }
        if (Health <= 0 && !dead)
        {
            audioManager.GetComponent<PhotonView>().RPC("MultiplayerPlay3DAudio", RpcTarget.All, (int)AudioManager.AudioID.Scientist, 1.0f, transform.position); 
            CurrentState = FSM.DEAD;
            dead = true;
            navMeshAgent.Stop();
        }
        switch (CurrentState)
        {
            case FSM.IDLE: // Doesnt do anything
                {
                    if (IdleTime > 0)
                    {
                        IdleTime -= Time.deltaTime;
                    }
                    else
                    {
                        if (StructureFound)
                        {
                            CurrentState = FSM.PATROL;
                            MoveTime = 2;
                        }
                        else
                        {
                            CurrentState = FSM.WANDER;
                            MoveTime = 2;
                        }
                    }
                    break;
                }
            case FSM.PATROL: // Moving around an area
                {
                    if (isMoving)
                    {
                        if (GoingMonument)
                        {
                            if (navMeshAgent.remainingDistance < 5)
                            {
                                isMoving = false;
                                GoingMonument = false;
                                IdleTime = Random.Range(1, 5);
                                CurrentState = FSM.IDLE;
                                break;
                            }
                            break;
                        }
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
                        // Need to limit to within the bounds of the linked structure
                        float x = Random.Range(-5, 5);
                        float z = Random.Range(-5, 5);
                        destination = Structure.transform.position;
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
            case FSM.WANDER: // Moving anywhere
                {
                    if (Structure != null)
                    {
                        StructureFound = true;
                        isMoving = false;
                        CurrentState = FSM.PATROL;
                        MoveTime = 0;
                        break;
                    }
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
                        destination = gameObject.transform.position;
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
            case FSM.ATTACK: // Chasing and attacking the player
                {
                    if (TargetPlayer == null)
                    {
                        isMoving = false;
                        IdleTime = Random.Range(1, 5);
                        CurrentState = FSM.IDLE;
                        break;
                    }
                    if (StructureFound && Structure != null)
                    {
                        if (Vector3.Distance(gameObject.transform.position, Structure.transform.position) >= 25)
                        {
                            isMoving = true;
                            GoingMonument = true;
                            CurrentState = FSM.PATROL;
                            break;
                        }
                    }
                    if (isMoving)
                    {
                        if (Vector3.Distance(gameObject.transform.position, destination) < 1)
                        {
                            isMoving = false;
                        }
                    }
                    else
                    {
                        if (Vector3.Distance(gameObject.transform.position, TargetPlayer.transform.position) >= 5)
                        {
                            Vector3 ADistance = (TargetPlayer.transform.position - gameObject.transform.position) / 2;
                            destination = gameObject.transform.position + ADistance.normalized * (Vector3.Distance(gameObject.transform.position, TargetPlayer.transform.position) - 5);
                            navMeshAgent.SetDestination(destination);
                            isMoving = true;
                        }
                        else if (Vector3.Distance(gameObject.transform.position, TargetPlayer.transform.position) < 4)
                        {
                            Vector3 ADistance = (TargetPlayer.transform.position - gameObject.transform.position) / 2;
                            destination = gameObject.transform.position + ADistance.normalized * (Vector3.Distance(gameObject.transform.position, TargetPlayer.transform.position) - 5);
                            navMeshAgent.SetDestination(destination);
                            isMoving = true;
                        }
                    }
                    if (Vector3.Distance(gameObject.transform.position, TargetPlayer.transform.position) <= 10)
                    {
                        Attack();
                    }
                    if (MoveTime <= 0)
                    {
                        isMoving = false;
                        MoveTime = 2;
                    }
                    else
                        MoveTime -= Time.deltaTime;
                    break;
                }
            case FSM.DEAD: // Dead
                {
                    if (dead)
                    {
                        Vector3 direction = Vector3.RotateTowards(transform.forward, Vector3.up, Mathf.Deg2Rad * (1 - deadTime) * 90, 0);
                        transform.LookAt(direction + transform.position);
                        deadTime -= Time.deltaTime;
                    }
                    if (deadTime <= 0 && PV.IsMine)
                    {
                        PhotonNetwork.Instantiate("ScientistBox", transform.position, Quaternion.identity);
                        PhotonNetwork.Destroy(gameObject);
                    }
                    break;
                }
        }
        if (CurrentState == FSM.ATTACK)
        {
            GameObject rotatingBody = gameObject;
            float result = 0;
            Quaternion PrevAngle = rotatingBody.transform.rotation;

            rotatingBody.transform.LookAt(new Vector3(TargetPlayer.transform.position.x, rotatingBody.transform.position.y, TargetPlayer.transform.position.z));
            Quaternion DesiredAngle = rotatingBody.transform.rotation;

            rotatingBody.transform.rotation = PrevAngle;

            float difference = DesiredAngle.eulerAngles.y - PrevAngle.eulerAngles.y;
            float turnSpd = 45.0f;
            if (difference > 0 && difference <= 180)
            {
                result = PrevAngle.eulerAngles.y + Time.deltaTime * turnSpd;
                if (result > DesiredAngle.eulerAngles.y)
                    result = DesiredAngle.eulerAngles.y;
            }
            else if (difference < 0 && difference >= -180)
            {
                result = PrevAngle.eulerAngles.y - Time.deltaTime * turnSpd;
                if (result < DesiredAngle.eulerAngles.y)
                    result = DesiredAngle.eulerAngles.y;
            }
            else if (difference < -180)
            {
                result = PrevAngle.eulerAngles.y + Time.deltaTime * turnSpd;
                if (result > 360)
                    result -= 360;
            }
            else if (difference > 180)
            {
                result = PrevAngle.eulerAngles.y - Time.deltaTime * turnSpd;
                if (result < 0)
                    result += 360;
            }
            else
            {
                result = PrevAngle.eulerAngles.y;
            }
            Vector3 direction = new Vector3(Mathf.Sin(result * Mathf.Deg2Rad), 0, Mathf.Cos(result * Mathf.Deg2Rad));

            rotatingBody.transform.LookAt(direction + rotatingBody.transform.position);
        }
    }

    void Attack()
    {
        gun.NonPlayerDischarge(PV);
    }

    [PunRPC]
    void FoundPlayer(int ID)
    {
        GameObject other = PhotonView.Find(ID).gameObject;
        DetectedPlayers.Add(other);
        // Targeting
        Enemy enemy = GetComponent<Enemy>();
        if (enemy.TargetPlayer == null)
        {
            enemy.TargetPlayer = other;
        }
        else if (Vector3.Distance(enemy.transform.position, enemy.TargetPlayer.transform.position) > Vector3.Distance(enemy.transform.position, other.transform.position))
        {
            enemy.TargetPlayer = other;
        }
    }

    [PunRPC]
    void FoundMonument(int ID)
    {
        GameObject other = PhotonView.Find(ID).gameObject;
        Enemy enemy = GetComponent<Enemy>();
        enemy.Structure = other;
    }

    [PunRPC]
    void LostPlayer(int ID)
    {
        GameObject other = PhotonView.Find(ID).gameObject;
        bool TargetLeft = false;
        Enemy enemy = GetComponent<Enemy>();
        if (enemy.TargetPlayer == other)
        {
            TargetLeft = true;
            enemy.TargetPlayer = null;
            DetectedPlayers.Remove(other);
        }
        if (TargetLeft)
        {
            for (int i = 0; i < DetectedPlayers.Count; i++)
            {
                if (enemy.TargetPlayer == null)
                {
                    enemy.TargetPlayer = DetectedPlayers[0];
                }
                else if (Vector3.Distance(enemy.transform.position, enemy.TargetPlayer.transform.position) > Vector3.Distance(enemy.transform.position, DetectedPlayers[i].transform.position))
                {
                    enemy.TargetPlayer = DetectedPlayers[i];
                }
            }
        }
    }

}
