using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class TankAI : Enemy
{
    protected List<GameObject> DetectedPlayers = new List<GameObject>();

    enum FSM { IDLE, PATROL, ATTACK, DEAD };
    public enum ENEMY_TYPE { SCIENTIST, ARMOURED_SCIENTIST, TANK };

    int MaxHealth = 100;
    public float MSpd = 2;
    float IdleTime;
    float MoveTime;
    float RotatingTurret = 0;
    bool RotateLeft = false;
    bool isMoving = false;
    bool GoingMonument = false;
    NavMeshAgent navMeshAgent;
    List<Vector3> Waypoints;
    int currentWaypoint;
    Vector3 destination;

    RocketLauncher gun;

    protected PhotonView PV;

    FSM CurrentState;

    public ENEMY_TYPE EnemyType;

    // Start is called before the first frame update
    public virtual void Awake()
    {
        MaxHealth = 1500;
        float dist = 100;

        Waypoints = new List<Vector3>();
        Vector3 temp;


        temp = new Vector3(transform.position.x + dist, 5, transform.position.z);
        Waypoints.Add(temp);


        temp = new Vector3(gameObject.transform.position.x + dist, 5, transform.position.z + dist);
        Waypoints.Add(temp);

        temp = new Vector3(transform.position.x, 5, transform.position.z + dist);
        Waypoints.Add(temp);

        temp = new Vector3(transform.position.x - dist, 5, transform.position.z + dist);
        Waypoints.Add(temp);

        temp = new Vector3(transform.position.x - dist, 5, transform.position.z);
        Waypoints.Add(temp);

        temp = new Vector3(transform.position.x - dist, 5, transform.position.z - dist);
        Waypoints.Add(temp);

        temp = new Vector3(transform.position.x, 5, transform.position.z - dist);
        Waypoints.Add(temp);

        temp = new Vector3(transform.position.x + dist, 5, transform.position.z - dist);
        Waypoints.Add(temp);


        currentWaypoint = 0;

        Health       = MaxHealth;
        IdleTime     = 0;
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        TargetPlayer = null;
        deadTime = 2;
        navMeshAgent.speed = MSpd;
        PV = GetComponent<PhotonView>();
        gun = gameObject.GetComponentInChildren<RocketLauncher>();
        gun.Init();
        gun.SetInfiniteAmmo(true);
        gun.SetTimeBetweenShots(0.33f);
        gun.SetCanFire(true);
        //Debug.Log("Awake: " + gun.GetInfiniteAmmo());
    }

    // Update is called once per frame
    public virtual void Update()
    {
        gun.SetInfiniteAmmo(true);
       //Debug.Log("Update: " + gun.GetInfiniteAmmo());
        

        if (CurrentState != FSM.ATTACK && TargetPlayer != null && !GoingMonument)
        {
            CurrentState = FSM.ATTACK;
            navMeshAgent.Stop();
        }
        if (Health <= 0)
        {
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
                        CurrentState = FSM.PATROL;
                        MoveTime = 180;
                    }
                    break;
                }
            case FSM.PATROL: // Moving around an area
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
                        destination = Waypoints[currentWaypoint];
                        navMeshAgent.SetDestination(destination);
                        Debug.Log(destination);
                        if (++currentWaypoint >= Waypoints.Count)
                            currentWaypoint = 0;
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
                        navMeshAgent.Resume();
                        break;
                    }
                    Attack();
                    break;
                }
            case FSM.DEAD: // Dead
                {
                    if (dead)
                    {
                        deadTime -= Time.deltaTime;
                    }
                    if (deadTime <= 0 && PV.IsMine)
                    {
                        // Harvestable = true;
                        PhotonNetwork.Destroy(gameObject);
                    }
                    break;
                }
            default:
                {
                    break;
                }
        }
        if (!dead)
        {
            if (CurrentState == FSM.ATTACK)
            {
                GameObject rotatingBody = gameObject.transform.Find("TurretBody").gameObject;
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
            else if (RotatingTurret > 0)
            {
                float result = 0;
                GameObject rotatingBody = rotatingBody = gameObject.transform.Find("TurretBody").gameObject;
                Quaternion PrevAngle = rotatingBody.transform.rotation;
                float turnSpd = 30.0f;
                if (RotateLeft)
                {
                    result = PrevAngle.eulerAngles.y - Time.deltaTime * turnSpd;
                    if (result < -180)
                        result += 360;
                }
                else
                {
                    result = PrevAngle.eulerAngles.y + Time.deltaTime * turnSpd;
                    if (result < 180)
                        result -= 360;
                }
                Vector3 direction = new Vector3(Mathf.Sin(result * Mathf.Deg2Rad), 0, Mathf.Cos(result * Mathf.Deg2Rad));

                rotatingBody.transform.LookAt(direction + rotatingBody.transform.position);
                RotatingTurret -= Time.deltaTime;
            }
            else
            {
                RotatingTurret = Random.Range(1, 5);
                if (Random.Range(0, 2) == 1)
                    RotateLeft = true;
                else
                    RotateLeft = false;
            }
        }
    }

    void Attack()
    {
        gun.Discharge();
    }

    [PunRPC]
    void TankFoundPlayer(int ID)
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
    void TankLostPlayer(int ID)
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
