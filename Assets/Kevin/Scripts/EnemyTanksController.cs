using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(TankTransform))]
[RequireComponent(typeof(Rigidbody))]
public class EnemyTanksController : MonoBehaviour
{   
    public bool shootsRockets = false;

    // public Camera cam;
    // public NavMeshAgent agent;
    // public WaypointsManager waypointsManager;
    // private FaceNavMeshController agentController;

    private float x;
    private float z;
    private string[,,] level;

    TankController c;
    TankController targetTank;
    private bool setup = false;

    //
    // =====================================================================
    //

    //
    // setup functions
    //

    private void Start() 
    {
        level = GameState.loadedLevel;
        InvokeRepeating("ChangePosition", 2.0f, 3.0f);
    }

    void ChangePosition()
    {
        Random random = new Random();
        //x = Random.Range(1f,  GameState.loadedLevel.GetLength(1) - 1);
        //z = Random.Range(-1f,  -GameState.loadedLevel.GetLength(2) + 1);

        int newX =  Random.Range(1,  level.GetLength(1) - 1);
        int newZ =  Random.Range(1,  level.GetLength(2) - 1);

        if (isOpenSpace(newX, newZ) == false)
        {
            int[] validPos = getValidPos(newX, newZ);

            while (validPos.Length < 2)
            {
                newX =  Random.Range(1,  level.GetLength(1) - 1);
                newZ =  Random.Range(1,  level.GetLength(2) - 1);

                validPos = getValidPos(newX, newZ);
            }

        }

        x = newX;
        z = newZ * -1;
    }

    bool isOpenSpace(int newX, int newZ)
    {
        if (level[0, newX, newZ] == "w" 
            || level[0, newX, newZ] == "b" 
            || level[0, newX, newZ] == "rl" 
            || level[0, newX, newZ] == "rr" 
            || level[0, newX, newZ] == "ru" 
            || level[0, newX, newZ] == "rd")
            return false;

        return true;
    }

    int [] getValidPos(int x, int z)
    {
        if (x + 2 < level.GetLength(1) && isOpenSpace(x, z))
            return new int [] {x + 2, z};

        if (x - 2 > 1 && isOpenSpace(x - 2, z))
            return new int[] {x - 2, z};

        if (z + 2 < level.GetLength(2) && isOpenSpace(x, z + 2))
            return new int[] {x, z + 2};

        if (z - 2 > 1 && isOpenSpace(x, z -2))
            return new int[] {x, z - 2};

        return new int[1];
    }

    public void Setup()
    {
        if (setup) return;
        setup = true;

        c = GetComponent<TankController>();
    }

    private TankController FindOpponent()
    {
        TankController[] tanks = GameObject.FindObjectsOfType<TankController>();

        foreach (TankController t in tanks)
            if (t.team != c.team && !t.destroyed)
                return t;
        
        return null;
    }

    //
    // Fixed Update
    //

    private void FixedUpdate() 
    {
        Setup();

        if (targetTank == null || targetTank.destroyed)
            targetTank = FindOpponent();

        if (targetTank != null)
        {
            c.AimAt(new Vector2(targetTank.transform.position.x, targetTank.transform.position.z));
            
            if (shootsRockets) c.FireRocket();
            else c.FireShell();
        }

        //Debug.Log(c.lastShellFireWasSuccess);

        c.MoveTo(new Vector2(x, z));
        



    }

    private void Update() {


        // if (!agent.pathPending)
        // {
        //     if (agent.remainingDistance <= agent.stoppingDistance)
        //     {
        //         if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
        //         {
        //             waypointsManager.FreeWaypoint(waypoint);
        //             waypoint = waypointsManager.AssignWaypoint();

        //             agentController = GetComponent<FaceNavMeshController>();
        //             agentController.moveAgent(waypoint);

        //         }
        //     }
        // }



        
    }
}