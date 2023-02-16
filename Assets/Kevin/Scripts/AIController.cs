using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
 
using UnityEngine.UI;
 
public class AIController : MonoBehaviour
{
    public class TankInfo
    {
        public TankController tc;
        public bool isAlly;
 
        public bool hasLoS;
        public Vector2 losDir;
        public bool isBounceLoS;
        public float raycastDist;
        public bool isBombLoS;
 
        public float distance;
 
    }
 
    public class NullableRaycastHit
    {
        public RaycastHit hit;
 
        public NullableRaycastHit(RaycastHit h) { hit = h; }
    }
 
    public const float DODGE_DISTANCE = 0.25f;
    public const float PATROL_DISTANCE = 4; 
 
    public const bool SHOW_DEBUG = false;

    private bool setup;
 
    //
    // other components
    //
    private TankController tc;
 
 
    //
    // state info
    //
    FSM fsm;
    private List<TankInfo> tankInfo = new List<TankInfo>();
    private float currentTime = 0;
 
    public string currentState_DISPLAY_ONLY;
    public Text currentStateDisplay;
    public GameObject TextPrefab;
 
    //
    // configurable parameters
    //
    public bool understandsBouncing;
    public bool shootsRockets;
    public bool shootsBombsOnly;
    public bool shootsBombs;

    public float bombSightRangeMax;
    public float bombSightRangeMin;
 
 
    public int bounceIterationsPerDirection = 5;
 
 
    public float dodgeProbability = 0.9f;
    public float findGroupProbability = 0.1f;
 
 
    public float moveRandomlyToAimTimer;
    public float giveUpSearchTimer;
    public float stayInGroupTimer;
    public float pickNewBuddyTimer;
    public float idleTimer;
    public float patrollingTimer;
    public float aimToShootTimer;
    public float shootToMoveRandomlyTimer;
 
    public float closeEnoughToBecomeBuddiesDistance = 2f;
 
 
    public float bulletGonnaHitSightRadius = 2;
    public float recalcSearchTimer = 0.8f;
 
    //
    // My functions
    //
    void Start()
    {
        // temp, for debugging
        //Setup(null);
        //tc.Setup();
        //tc.ta.SetHologram(0);
    }
 
    public void ReSetup()
    {
        setup = false;
        Setup();
    }
 
    public void Setup()
    {
        if (setup) return;
        setup = true;
 
        currentTime = 0;
 
        tc = this.GetComponent<TankController>();
        // //RefreshTankInfo();
        // Debug.Log("initializing fsm");
        // fsm = new FSM(new State_NonCombat(), this);
        // Debug.Log("initialized fsm to " + fsm.GetStateName());
 
 
 
        // debug text
 
        if (SHOW_DEBUG)
        {
            GameObject g = GameObject.Instantiate(TextPrefab);//new GameObject();
            //currentStateDisplay = g.AddComponent<Text>();
            g.transform.parent = GameObject.FindObjectOfType<Canvas>().transform;
            currentStateDisplay = g.GetComponent<Text>();
            currentStateDisplay.transform.position = Camera.main.WorldToScreenPoint(this.transform.position);
            currentStateDisplay.text = "INITIALIZING";
        }
    }
 
    public void SetupConfig(AIConfig aiconf)
    {
        if (aiconf == null) return;
        understandsBouncing = aiconf.understandsBouncing;
        shootsRockets = aiconf.firesRockets;
        shootsBombsOnly = aiconf.shootsBombsOnly;
        shootsBombs = aiconf.shootsBombs;
        bombSightRangeMax = aiconf.bombSightRangeMax;
        bombSightRangeMin = aiconf.bombSightRangeMin;
 
        bounceIterationsPerDirection = aiconf.bounceIterationsPerDirection;
 
        dodgeProbability = aiconf.dodgeProbability;
        findGroupProbability = aiconf.findGroupProbability;
 
        moveRandomlyToAimTimer = aiconf.moveRandomlyToAimTimer;
        giveUpSearchTimer = aiconf.giveUpSearchTimer;
        stayInGroupTimer = aiconf.stayInGroupTimer;
        pickNewBuddyTimer = aiconf.pickNewBuddyTimer;
        idleTimer = aiconf.idleTimer;
        patrollingTimer = aiconf.patrollingTimer;
        recalcSearchTimer = aiconf.recalcSearchTimer;
        shootToMoveRandomlyTimer = 0.8f;
        aimToShootTimer = 0.8f;
 
        closeEnoughToBecomeBuddiesDistance = aiconf.closeEnoughToBecomeBuddiesDistance;
 
        bulletGonnaHitSightRadius = aiconf.bulletGonnaHitSightRadius;
 
    }
 
    void OnDestroy()
    {
        Destroy(currentStateDisplay);
    }
 
    string lastState = "";
    public string runningLog = "";
 
    public float updateChance = 0.3f;

    void FixedUpdate()
    {
        if (Random.value >= updateChance) return;

        if (tc == null) { tc = this.GetComponent<TankController>(); Setup(); }
        if (tc == null) return;
        if (tc.destroyed) return;
 
        if (fsm == null)
        {
            fsm = new FSM(new State_Combat(), this);
            //Debug.Log("initialized fsm to " + fsm.GetStateName());
            DEBUG_UpdateStateLog();
        }
 
        //OpponentInLoS();
        PathingUpdate();   
        RefreshTankInfo();
        fsm.UpdateStep();
 
        DEBUG_UpdateStateLog(); 
        currentTime += Time.deltaTime;
 
    }
 
    void DEBUG_UpdateStateLog()
    {
        
        if (!SHOW_DEBUG) return;

        currentState_DISPLAY_ONLY = fsm.GetStateName();
        currentStateDisplay.transform.position = Camera.main.WorldToScreenPoint(this.transform.position);
        currentStateDisplay.text = string.Join("\n" , currentState_DISPLAY_ONLY.Split('-'));
        if (false && currentState_DISPLAY_ONLY != lastState)
        {
            lastState = currentState_DISPLAY_ONLY;
            runningLog += "time " + (Mathf.Round(currentTime*100f)/100f) + ": " + lastState + "\n";
            Debug.Log("FSM STATES LOG (for current state " + lastState + "):\n"+runningLog);
        }
    }
 
    int counter = 0;
 
    void RefreshTankInfo()
    {
        tankInfo = new List<TankInfo>();
 
        if (counter++ < 5) return; // tanks can see THROUGH the level on the first (first few?) frames, no clue why, but this is a bandaid
 
        foreach (TankController other in TankController.allTanks)
        {
            if (other == null) continue;
            if (other == this.tc) continue;
            if (other.destroyed) 
            {
                if (targetedTank != null && other == targetedTank.tc) targetedTank = null; // don't seek after dead tanks
                continue; // don't make info for dead tanks.
            }
            //Debug.Log("not checking self");
 
            TankInfo i = new TankInfo();
            i.tc = other;
            i.isAlly = (other.team == this.tc.team);
 
            NullableRaycastHit hit = null;

            if(!shootsBombsOnly)
                hit = CalcDirectLoS(i);
            else
                i.hasLoS = false;

            // if (i.hasLoS) 
            // {
            //     Debug.DrawRay(i.tc.transform.position - new Vector3(0, 0, 1), new Vector3(0, 0, 2), Color.blue);
            //     Debug.DrawRay(i.tc.transform.position - new Vector3(1, 0, 0 ), new Vector3(2, 0, 0), Color.blue);
            //     //i.tc.SetDestroyed(true);
 
 
            //     Debug.DrawRay(this.tc.transform.position - new Vector3(0, 0, 1), new Vector3(0, 0, 2), Color.magenta);
            //     Debug.DrawRay(this.tc.transform.position - new Vector3(1, 0, 0 ), new Vector3(2, 0, 0), Color.magenta);
 
            //     //Debug.Break();
            // }
 
            if (hit != null && understandsBouncing)
            {
                CalcSingleBounceLoS(i, hit.hit);
            }
 
            if (Mathf.Abs(i.tc.transform.position.y - this.transform.position.y) > 0.2)  // we're on different floors
                i.distance = Mathf.Infinity;
            else 
                i.distance = Vector3.Distance(i.tc.transform.position, this.transform.position);
 

            if (shootsBombsOnly || (shootsBombs && !i.hasLoS)) 
            {
                // try to calculate a bomb los
                CalcBombLoS(i);
            }

 
            tankInfo.Add(i);
 
            // update our specially marked tanks
            if (buddy != null        && other == buddy.tc)        buddy = i;
            if (targetedTank != null && other == targetedTank.tc) targetedTank = i;
        }
    }
 
    void CalcBombLoS(TankInfo i)
    {
        Vector3 iPos = i.tc.tt == null ? i.tc.transform.position : i.tc.tt.ShellSpawnLocation;
        Vector3 myPos = this.tc.tt.ShellSpawnLocation;
        
        float floorDiff = i.tc.transform.position.y - this.transform.position.y;

        i.distance = i.raycastDist = (i.tc.transform.position - this.transform.position).magnitude + floorDiff; // if target is below me, shrink the distance a bit, if target is above me, raise the distance a bit (that's what +floorDiff is for)
        if (i.raycastDist > bombSightRangeMax) return;
        if (i.raycastDist < bombSightRangeMin) return;
        


        RaycastHit hit;
        int layerMask = LayerMask.GetMask("Arena Objects");
        bool hitCeiling = Physics.Raycast(myPos, new Vector3(0, 1, 0), out hit, 0.5f, layerMask);
 
        if (hitCeiling) return; // no bomb los, we'll hit the ceiling

        if (floorDiff > 0.2) // my target is on the floor above me
        {
            // check to make sure there's no walls between us on the next floor up
            Vector3 raycastDir = iPos-(myPos+new Vector3(0, floorDiff, 0)); // pretend we're on the same height
            float distToTarget = raycastDir.magnitude; // this is actually the distance the raycast will need to travel
            i.losDir = new Vector2(raycastDir.x, raycastDir.z);
           

            bool hitWallOnNextFloor = Physics.Raycast(myPos+new Vector3(0, floorDiff, 0), raycastDir, out hit, distToTarget, layerMask);
            if (hitWallOnNextFloor) return;
        }

        //if there's no wall above me or between us on the next floor up, and you're in range of bombSightRange

        Vector3 losDir = iPos-myPos;
        i.hasLoS = true;
        i.isBombLoS = true;
        //i.distance = Vector3.Distance(i.tc.transform.position, this.transform.position);
        i.losDir = new Vector2(losDir.x, losDir.z);
    }

    NullableRaycastHit CalcDirectLoS(TankInfo i)
    {
        if (this.tc == null) Debug.LogError("tc was null");
 
        Vector3 myPos = this.tc.tt.ShellSpawnLocation;
        Vector3 iPos = i.tc.tt == null ? i.tc.transform.position : i.tc.tt.ShellSpawnLocation;
 
        // we're on different floors
        i.hasLoS = false;
        if (Mathf.Abs(iPos.y - myPos.y) > 0.2) return null;
 
        Vector3 raycastDir = iPos-myPos;
        float distToTarget = raycastDir.magnitude;
        i.losDir = new Vector2(raycastDir.x, raycastDir.z);
        i.raycastDist = distToTarget;
 
        RaycastHit hit;
        int layerMask = LayerMask.GetMask("Arena Objects");
        bool didHit = Physics.Raycast(myPos, raycastDir, out hit, distToTarget, layerMask);
 
        if (didHit)
        {
            Debug.DrawRay(myPos, raycastDir, Color.red);
 
            Debug.DrawRay(myPos, raycastDir.normalized * hit.distance, Color.yellow);
            i.hasLoS = false;
            return new NullableRaycastHit(hit);
        }
        else
        {
            Debug.DrawRay(myPos, raycastDir, Color.green);
            i.isBounceLoS = false;
            i.hasLoS = true;
 
            // Debug.DrawRay(i.tc.transform.position - new Vector3(0, 0, 1), new Vector3(0, 0, 2), Color.blue);
            // Debug.DrawRay(i.tc.transform.position - new Vector3(1, 0, 0 ), new Vector3(2, 0, 0), Color.blue);
            // //i.tc.SetDestroyed(true);
 
 
            // Debug.DrawRay(this.tc.transform.position - new Vector3(0, 0, 1), new Vector3(0, 0, 2), Color.magenta);
            // Debug.DrawRay(this.tc.transform.position - new Vector3(1, 0, 0 ), new Vector3(2, 0, 0), Color.magenta);
 
            //Debug.Log("hey, los true @ time " + currentTime + " for target " + i.tc.transform.position + " from tank " + this.tc.transform.position);
            //Debug.Break();
 
            return null;
        }
    }
 
    bool CalcSingleBounceLoS(TankInfo i, RaycastHit straightLineHit)
    {
        Vector3 myPos = this.tc.tt.ShellSpawnLocation;
        Vector3 iPos = i.tc.tt.ShellSpawnLocation;
 
        i.hasLoS = false;
 
        // we're on different floors
        if (Mathf.Abs(iPos.y - myPos.y) > 0.2) return false;
 
        //Vector3 raycastDir = iPos-myPos;
 
        //RaycastHit h;
        //RaycastBounce(straightLineHit, raycastDir, out h, iPos, LayerMask.GetMask("Arena Objects"));
 
 
 
        // bounces for walls facing x direction
        for (int j = 1; j <= bounceIterationsPerDirection; j++)
        {
            bool success;
 
            success = CheckForXMirror(i, j, iPos, myPos);
            if(success) break;
            success = CheckForXMirror(i, -j, iPos, myPos);
            if(success) break;
            success = CheckForZMirror(i, j, iPos, myPos);
            if(success) break;
            success = CheckForZMirror(i, -j, iPos, myPos);
            if(success) break;
        }
 
 
 
        // while()
        // {
        //     collider.gameObject.transform.position
        // }
 
        return false;
    }
 
    bool CheckForXMirror(TankInfo i, int mirrorDist, Vector3 iPos, Vector3 myPos)
    {
        //
        // First, pick a distance for a wall that we'll assume is there
        //
        float mx = Mathf.Floor(this.transform.position.x)+mirrorDist;
        Vector3 mirrorTarget = new Vector3(iPos.x+2*(mx-iPos.x), iPos.y, iPos.z); // here's where we'd think the target is looking at a mirror
        Vector3 raycastDir = mirrorTarget - myPos;
 
        //Debug.DrawRay(new Vector3(mx, iPos.y, iPos.z-5), new Vector3(0, 0, 10));
        //Debug.DrawRay(mirrorTarget, new Vector3(0, 0, 1), Color.blue);
 
        RaycastHit bouncePoint;
        float dist = Vector3.Distance(myPos, mirrorTarget);
        bool hit = Physics.Raycast(myPos, raycastDir, out bouncePoint, dist, LayerMask.GetMask("Arena Objects"));
 
        if(!hit) return false; // expected mirror wasn't here
 
        //Debug.Log("bounce point " + bouncePoint.point.x + " mx " + mx);
 
        // if the wall was there, then check the reflection
        if (Mathf.Abs(bouncePoint.point.x - mx) < 0.01) 
        {
            Debug.DrawLine(myPos, mirrorTarget, Color.green);
 
            RaycastHit finalHit;
            bool failed = RaycastBounce(bouncePoint, raycastDir, out finalHit, iPos);
 
            if (!failed)
            {
                i.hasLoS = true;
                i.isBounceLoS = true;
                i.losDir = new Vector2(raycastDir.x, raycastDir.z);
                i.raycastDist = dist;
                return true;
            }
        }
 
        return false;
    }
 
    bool CheckForZMirror(TankInfo i, int mirrorDist, Vector3 iPos, Vector3 myPos)
    {
        float mz = Mathf.Floor(this.transform.position.z)+mirrorDist;
        Vector3 mirrorTarget = new Vector3(iPos.x, iPos.y, iPos.z+2*(mz-iPos.z));
        Vector3 raycastDir = mirrorTarget - myPos;
 
        //Debug.DrawRay(new Vector3(mx, iPos.y, iPos.z-5), new Vector3(0, 0, 10));
        //Debug.DrawRay(mirrorTarget, new Vector3(0, 0, 1), Color.blue);
 
        RaycastHit bouncePoint;
        float dist = Vector3.Distance(myPos, mirrorTarget);
        bool hit = Physics.Raycast(myPos, raycastDir, out bouncePoint, dist, LayerMask.GetMask("Arena Objects"));
 
        if (!hit) return false;
 
        //Debug.Log("bounce point " + bouncePoint.point.x + " mx " + mx);
 
        if (Mathf.Abs(bouncePoint.point.z - mz) < 0.01) 
        {
            Debug.DrawLine(myPos, mirrorTarget, Color.green);
 
            RaycastHit finalHit;
            bool failed = RaycastBounce(bouncePoint, raycastDir, out finalHit, iPos);
 
            if (!failed)
            {
                i.hasLoS = true;
                i.isBounceLoS = true;
                i.losDir = new Vector2(raycastDir.x, raycastDir.z);
                i.raycastDist = dist;
                return true;
            }
        }
 
        return false;
    }
 
    bool RaycastBounce(RaycastHit h, Vector3 originalRaycastDir, out RaycastHit outH, Vector3 target)
    {
        return RaycastBounce(h, originalRaycastDir, out outH, target, LayerMask.GetMask("Arena Objects"));
    }
 
    bool RaycastBounce(RaycastHit h, Vector3 originalRaycastDir, out RaycastHit outH, Vector3 target, int layerMask)
    {
        Vector3 newDir = Vector3.Reflect(originalRaycastDir, h.normal);
 
        float distToTarget = Vector3.Distance(h.point, target);
 
        bool didHit = Physics.Raycast(h.point, newDir, out outH, distToTarget, layerMask);
 
        // debugging
        Debug.DrawRay(h.point, newDir, Color.green);
        // bool didHitTank = Physics.Raycast(h.point, newDir, out outH, distToTarget, LayerMask.GetMask("Arena Objects", "Tanks"));
        // if (!didHitTank || didHit)
        // {
        //     Debug.DrawRay(h.point, newDir, Color.red);
        // }
        // else
        //     Debug.DrawRay(h.point, newDir, Color.green);
        // end debugging
 
        return didHit;
    }
 
 
    int pathPointIdx = 0;
    bool moveToNext = true;
    Vector3 nextPathingTarget;
    Vector2 nextMoveToTarget;

    bool movingOutOfTheWay = false;
    float movingOutOfTheWayTimer = 0;
    float movingOutOfTheWayMaxTime = 2;

    const float RAYCAST_START_OUSIDE_TANK = 0.25f;
 
    void PathingUpdate() 
    {
        if (path == null || path.corners == null || path.corners.Length <= 0 || path.corners.Length <= pathPointIdx) return;
 
        movingOutOfTheWayTimer -= Time.deltaTime;

        // set up the next moveto target
        if (moveToNext)
        {
            moveToNext = false;
            pathPointIdx++;
 
            if (pathPointIdx >= path.corners.Length) return;
 
            nextPathingTarget = path.corners[pathPointIdx];
            nextMoveToTarget = new Vector2(nextPathingTarget.x, nextPathingTarget.z);
            tc.MoveTo(nextMoveToTarget);
            tc.AimAt(nextMoveToTarget);
        }

        Debug.DrawLine(transform.position, nextPathingTarget, Color.cyan);

        // if there's a tank in my way, move out of its way.
        RaycastHit hit;
        Vector3 raycastDir = nextPathingTarget-transform.position;
        // i'm adding (raycastDir.normalized*0.25f) to the start position here because when the tank did the raycast, it was hitting itself and thinking it was another tank
        if (!movingOutOfTheWay && Physics.Raycast(transform.position + raycastDir.normalized*RAYCAST_START_OUSIDE_TANK, raycastDir, out hit, 0.35f, LayerMask.GetMask("Tanks")))
        {
            Debug.Log("THERES SOMEONE IN MY WAYYYYYY");
            Reposition(new Vector2(raycastDir.x, raycastDir.z));
            movingOutOfTheWay = true;
            movingOutOfTheWayTimer = movingOutOfTheWayMaxTime;

            Debug.DrawRay(transform.position, raycastDir.normalized*0.3f, Color.blue);
            //Debug.Break();
        }

        // check if we're either done moving out of the way or we've hit our next pathfinding target
        if (!movingOutOfTheWay)
        {
            float dist = Vector3.Distance(nextPathingTarget, transform.position);
            if (tc.reachedMoveTarget || dist <= 0.1)
            {
                moveToNext = true;
            }
        }
        else
        {
            if (DoneRepositioning() || movingOutOfTheWayTimer <= 0)
            {
                movingOutOfTheWay = false;
                tc.MoveTo(nextMoveToTarget);
                tc.AimAt(nextMoveToTarget); 
            }
        }
    }
 
    // fuction modified from Selzier and Valkyr_x via http://answers.unity.com/answers/1426690/view.html
    Vector3 RandomNavMeshPoint(Vector3 center, float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        center += randomDirection;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        // returns false if there's no point on the navmesh within 1.4 units of the input position
        if (NavMesh.SamplePosition(randomDirection, out hit, 1.4f, NavMesh.AllAreas)) 
        {
            return hit.position;            
        }
        else
        {
            // problem
            Debug.LogWarning("RandomNavMeshPoint generated invalid location");
            return center;
        }
    }
 
    Vector3 RandomNavMeshPointInsideLevel()
    {
        NavMeshHit hit;
        Vector3 random = new Vector3(Random.Range(0f, (float)GameState.LevelWidth), Random.Range(0f, -(float)GameState.LevelHeight));
 
        // returns false if there's no point on the navmesh within 1 unit of the input position
        if (NavMesh.SamplePosition(random, out hit, 1.4f, NavMesh.AllAreas)) 
        {
            return hit.position;            
        }
        else
        {
            // problem
            Debug.LogWarning("RandomNavMeshPointInsideLevel generated invald location");
            return transform.position;
        }
    }
 
    //
    // FSM Helper functions
    //
 
    public float CurrentTime()
    {
        return currentTime;
    }
 
    //
    // FSM Condition functions
    //
 
    public bool ReachedPathfindTarget()
    {
        if (path == null || path.corners == null) return true;
        return pathPointIdx >= path.corners.Length;
    }
 
    public bool TransitionProbability(string transitionName)
    {
        float val = Random.value;
        switch (transitionName)
        {
            case "defense to dodge": return val < dodgeProbability;
            case "patrolling to findGroup": return val < findGroupProbability;
        }
 
        Debug.LogError(transitionName + " is not a valid probability transition name.");
 
        return false;
    }
 
    public bool TransitionTimerUp(string transitionName, float startTime)
    {
        switch(transitionName)
        {
            case "moveRandomly to aim": return currentTime-startTime >= moveRandomlyToAimTimer;
            case "searching to nonCombat": return currentTime-startTime >= giveUpSearchTimer;
            case "formGroup to idle": return currentTime-startTime >= stayInGroupTimer;
            case "pathfindToBuddy to pickBuddy": return currentTime-startTime >= pickNewBuddyTimer;
            case "idle to patrolling": return currentTime-startTime >= idleTimer;
            case "Patrolling to Idle": return currentTime-startTime >= patrollingTimer;
            case "restart search": return currentTime-startTime >= recalcSearchTimer;
            case "shoot to moveRandomly": return currentTime-startTime >= shootToMoveRandomlyTimer;
            case "aim to shoot": return currentTime-startTime >= aimToShootTimer;
        }
 
        Debug.LogError(transitionName + " is not a valid timer transition name.");
        return false;
    }
 
 
    public bool FriendlyTankNearbyWithLineOfSight()
    {
        if (true) return false;
 
        foreach(TankInfo i in tankInfo)
        {
            if (i.hasLoS && i.isAlly && i.distance <= closeEnoughToBecomeBuddiesDistance)
            {
                if (!ValidBuddy(i)) continue;
                return true;
            }
        }
 
        return false;
    }
 
    public TankInfo buddy;
    public const float buddyTooCloseDistance = 2;//0.8f;
 
    public bool MyBuddyPickedMeAsABuddyAndWereNearby()
    {
        return ValidBuddy(buddy);
    }
 
    public bool ValidBuddy(TankInfo buddy)
    {
        if (buddy == null) return false;
        //if (buddy.tc.aic.buddy == null) return false;
 
 
        Debug.Log(buddy.distance);
        if (/*buddy.tc.aic.buddy.tc == this.tc &&*/ /*Vector3.Distance(buddy.tc.transform.position, this.transform.position)*/ buddy.distance <= buddyTooCloseDistance)
        {
            Debug.Log("Too close!");
            return true;
        }
 
        return false;
    }
 
    public TankInfo targetedTank;
 
    public bool OpponentInLoS()
    {
        //Debug.Log("check");
        if (tankInfo == null) return false;
        if (tankInfo.Count == 0) return false;
 
        foreach(TankInfo i in tankInfo)
        {
            if (i.hasLoS && !i.isAlly)
            {
                targetedTank = i;
                //Debug.Log("targeted tank position: " + i.tc.transform.position + " @ " + currentTime);
 
                // Debug.DrawRay(i.tc.transform.position - new Vector3(0, 0, 1), new Vector3(0, 0, 2), Color.cyan);
                // Debug.DrawRay(i.tc.transform.position - new Vector3(1, 0, 0 ), new Vector3(2, 0, 0), Color.cyan);
                // //i.tc.SetDestroyed(true);
 
 
                // Debug.DrawRay(this.tc.transform.position - new Vector3(0, 0, 1), new Vector3(0, 0, 2), Color.red);
                // Debug.DrawRay(this.tc.transform.position - new Vector3(1, 0, 0 ), new Vector3(2, 0, 0), Color.red);
 
                //Debug.Break();
                return true;
            }
        }
 
        return false;
    }
 
    public int NumEnemiesWithLoSToMe()
    {
        int n = 0;
        foreach(TankInfo i in tankInfo)
        {
            if (i.hasLoS && !i.isAlly)
            {
                n++;
            }
        }
 
        return n;
    }

    public bool CanMove()
    {
        return tc.canMove;
    }
 
    public bool LostTargetLOS()
    {
        return targetedTank == null || !targetedTank.hasLoS;
    }
 
    public int enemiesWithLoSCountForFear = 5;
 
    public bool TooManyEnemiesWithLoSToMe()
    {
        if (NumEnemiesWithLoSToMe() >= enemiesWithLoSCountForFear) return true;
        return false;
    }
 
    public bool FriendlyInLineOfFire()
    {
        if (targetedTank == null) return false;
        // raycast from me to bounce point, if it hits a tank and that tank is an ally return true
        // raycast from bounce point to target tank, if it hits a tank and that tank is an ally return true
 
        Vector3 raycastDir = targetedTank.losDir;
        Vector3 myPos = this.transform.position;
        RaycastHit hit;
        float dist = targetedTank.raycastDist;
        bool didHit = Physics.Raycast(myPos + raycastDir.normalized * RAYCAST_START_OUSIDE_TANK, raycastDir, out hit, dist, LayerMask.GetMask("Tanks", "Arena Objects"));
 
        if (hit.transform == null) return false; // hit nothing???
 
        TankController c = hit.transform.GetComponent<TankController>();
 
        if (c != null)
            if (c.team == this.tc.team)
                return true;
            else
                return false;
 
        if (targetedTank.isBounceLoS)
        {
            RaycastHit hit2;
            didHit = RaycastBounce(hit, raycastDir, out hit2, targetedTank.tc.transform.position, LayerMask.GetMask("Tanks", "Arena Objects"));
 
            c = hit.transform.GetComponent<TankController>();
            if (c != null) 
                if (c.team == this.tc.team)
                    return true;
                else
                    return false;
        }
 
        return false;
    }
 
 
    public BulletController bulletThatsGonnaHitMe;
    public bool BulletGonnaHitMe()
    {
        //bullet gonna hit me
        // for bulletcontroller b in bulletcontroller.allbullets
        // if raycast from bullet along bullet.rb.velocity mask tanks and arena objects
        // if the hit object is me
        // bulletThatsGonnaHitMe = b;
        // return true
        // else
        // if understand bouncing redo with bounce raycast
 
        Vector3 myPos = this.transform.position;
 
        foreach (BulletController bullet in BulletController.allBullets)
        {
            Vector3 bPos = bullet.transform.position; 
            float dist = Vector3.Distance(bPos, myPos);
            if (dist > bulletGonnaHitSightRadius) return false;

            Debug.DrawLine(myPos, bPos, Color.cyan);
 
            //RaycastHit hit;
            //Vector3 raycastDir = bullet.transform.position - myPos;
            //bool didHit = Physics.Raycast(myPos, raycastDir, out hit, dist, LayerMask.GetMask("Tanks", "Arena Objects"));
 
            //if (didHit) continue; // this bullet is blocked by something
 
            RaycastHit hit;
            //Vector3 raycastDir = bPos - myPos;
            bool didHit = Physics.Raycast(bPos, bullet.rb.velocity, out hit, dist, LayerMask.GetMask("Tank Shells", "Tanks", "Arena Objects"));
 
            Debug.DrawRay(bPos, bullet.rb.velocity*10, Color.magenta);

            // if (didHit)
            // {
            //     Debug.Log("about to hit: " + hit.transform.gameObject.name);
            //     Debug.Log("my name " + this.gameObject.name);
            //     Debug.DrawRay(bPos, bullet.rb.velocity*10, Color.magenta);
            //     Debug.Log("hits");
            //     Debug.Break();
            // }

            if (didHit && (hit.transform.gameObject == this.gameObject || (hit.transform.parent != null && hit.transform.parent.gameObject == this.gameObject)))
            {
                // it will hit me :O
                bulletThatsGonnaHitMe = bullet;
                return true;
            }
        }
 
        return false;
    }
 
    public bool CanParry()
    {
        if (shootsBombsOnly) return false;

        if (shootsRockets)
            return tc.CanFireRocket();
        else
            return tc.CanFireShell();
    }
 
    public bool CanShoot()
    {
        if (shootsBombsOnly) return tc.CanFireBomb();
        if (shootsBombs && targetedTank != null && targetedTank.isBombLoS) tc.CanFireBomb();

        if (shootsRockets)
            return tc.CanFireRocket();
        else
            return tc.CanFireShell();
    }
 
    private bool dodgeDirection;
    public bool CanDodge()
    {
        if (bulletThatsGonnaHitMe == null) return false;
 
        Vector3 myPos = this.transform.position;
        Vector3 bulletVelocityNorm = bulletThatsGonnaHitMe.rb.velocity.normalized;
        Vector3 dodgeOption1 = new Vector3(bulletVelocityNorm.z, 0, -bulletVelocityNorm.x);
        Vector3 dodgeOption2 = new Vector3(-bulletVelocityNorm.z, 0, bulletVelocityNorm.x);
 
        // check to see if dodgeOption1 works by raycasting 0.25 in that direction
        // otherwise do dodgeOption2
 
        RaycastHit hit;
        bool didHit = Physics.Raycast(myPos, dodgeOption1, out hit, DODGE_DISTANCE, LayerMask.GetMask("Arena Objects"));
        if (!didHit) 
        {
            dodgeDirection = true;
            return true;
        }
 
        didHit = Physics.Raycast(myPos, dodgeOption2, out hit, DODGE_DISTANCE, LayerMask.GetMask("Arena Objects"));
        if (!didHit) 
        {
            dodgeDirection = false;
            return true;
        }
 
 
        return false;
    }
 
    public bool DoneRepositioning()
    {
        return tc.reachedMoveTarget;
    }
 
    //
    // FSM Endpoint functions
    //
 
    public NavMeshPath path;
 
    public void Shoot()
    {
        if (targetedTank != null && targetedTank.isBombLoS && shootsBombs) tc.FireBomb();
        else if (shootsBombsOnly) tc.FireBomb();
        else if (shootsRockets) tc.FireRocket();
        else tc.FireShell();
    }
 
    public void PathTo(Vector3 target)
    {
        //Debug.Log("target: " + target);
        path = new NavMeshPath();
        NavMesh.CalculatePath(this.transform.position, target - new Vector3(0, 0.01f, 0), NavMesh.AllAreas, path);
 
        for (int i = 0; i < path.corners.Length - 1; i++)
            Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.magenta);
 
        pathPointIdx = 0;
        moveToNext = true;
    }
 
    public void PickBuddy()
    {
        foreach(TankInfo i in tankInfo)
        {
            if (i.hasLoS && i.isAlly && i.distance <= closeEnoughToBecomeBuddiesDistance)
            {
                if (!ValidBuddy(i)) continue;
                buddy = i;
                return;
            }
        }
 
        buddy = null;
    }
 
    public void PathfindToBuddy()
    {
        if (buddy == null) return;
        PathTo(buddy.tc.transform.position);
    }
 
    public void StartIdle()
    {
        tc.StopMoving();
        StopPathfinding();
    }
 
    private Vector3 patrolTarget;
    public void PickPatrolPathfindTarget()
    {
        if (GameState.LevelWidth == 0) patrolTarget = RandomNavMeshPoint(this.transform.position, PATROL_DISTANCE);
        else patrolTarget = RandomNavMeshPointInsideLevel();
    }
 
    public void MoveToPatrolTarget()
    {
        PathTo(patrolTarget);
    }
 
    public void StopPathfinding()
    {
        tc.StopMoving();
        path = null;
    }
 
    public void PathfindToTargetedTank()
    {
        if (targetedTank == null) return;
        PathTo(targetedTank.tc.transform.position);
    }
 
    public void ParryBullet()
    {
        if (bulletThatsGonnaHitMe == null) return;
 
        Vector2 bulletPos2D = new Vector2(bulletThatsGonnaHitMe.transform.position.x, bulletThatsGonnaHitMe.transform.position.z);
        tc.AimAt(bulletPos2D);
 
        Shoot();
    }
 
    public void DodgeBullet()
    {
        if (bulletThatsGonnaHitMe == null) return;
        Vector3 bulletVelocityNorm = bulletThatsGonnaHitMe.rb.velocity.normalized;
        Vector2 dodgeOption1 = new Vector2(bulletVelocityNorm.z, -bulletVelocityNorm.x);
        Vector2 dodgeOption2 = new Vector2(-bulletVelocityNorm.z, bulletVelocityNorm.x);
 
        // check to see if dodgeOption1 works by raycasting 0.25 in that direction
        // otherwise do dodgeOption2
 
        Vector2 dodgeDir = dodgeOption2;
        if (dodgeDirection) dodgeDir = dodgeOption1;
 
        StopPathfinding();
        Vector2 thisPos2d = new Vector2(this.transform.position.z, this.transform.position.z);
        tc.MoveTo(thisPos2d + dodgeDir*0.25f);
    }
 
    public void AimAtTargetedTank()
    {
        if (targetedTank == null) return;
        // Vector2 myPos = new Vector2(this.transform.position.x, this.transform.position.z);
        //tc.AimAt(myPos + targetedTank.losDir);

        Debug.DrawRay(transform.position, targetedTank.losDir.normalized, Color.blue);
        Vector2 aimTarg = targetedTank.losDir + new Vector2(this.transform.position.x, this.transform.position.z);
        tc.AimAt(aimTarg);
    }
 
    public void MoveRandomly()
    {
        Vector3 target = RandomNavMeshPoint(this.transform.position, 1);
        PathTo(target);
    }
 
    public void Reposition()
    {
        if (targetedTank == null) 
        {
            float facingRad = Mathf.Deg2Rad * tc.tt.HeadRotation;
            Reposition(new Vector2(Mathf.Cos(facingRad), Mathf.Sin(facingRad)));
        }
        else
        {
            Reposition(targetedTank.losDir.normalized);
        }
    }

    public void Reposition(Vector2 relativeTo)
    {
        // do something similar to dodge
        // but perpendicular to aim direction
 
        Vector3 myPos = this.transform.position;
        Vector2 aimDirNorm = relativeTo;
        Vector3 dodgeOption1 = new Vector3(aimDirNorm.y, 0, -aimDirNorm.x);
        Vector3 dodgeOption2 = new Vector3(-aimDirNorm.y, 0, aimDirNorm.x);
 
        // check to see if dodgeOption1 works by raycasting 0.25 in that direction
        // otherwise do dodgeOption2
 
        RaycastHit hit;
        bool didHit = Physics.Raycast(myPos, dodgeOption1, out hit, DODGE_DISTANCE, LayerMask.GetMask("Arena Objects"));
        if (!didHit) 
        {
            Vector2 pos = new Vector2(myPos.x, myPos.z) + DODGE_DISTANCE * new Vector2(dodgeOption1.x, dodgeOption1.z);
            tc.MoveTo(pos);
            return;
        }
 
        didHit = Physics.Raycast(myPos, dodgeOption2, out hit, DODGE_DISTANCE, LayerMask.GetMask("Arena Objects"));
        if (!didHit) 
        {
            Vector2 pos = new Vector2(myPos.x, myPos.z) + DODGE_DISTANCE * new Vector2(dodgeOption2.x, dodgeOption2.z);
            tc.MoveTo(pos);
            return;
        }
    }
 
    // only used for PathfindToSafeAlly
    struct SortTanksByDistance
    {
        public TankController tankController;
        public float dist;
    }
 
    public void PathfindToSafeAlly()
    {
 
        // sort all allies by distance to me
        List<SortTanksByDistance> allies = new List<SortTanksByDistance>();
        foreach(TankInfo i in tankInfo)
        {
            if (!i.isAlly) continue;
            SortTanksByDistance st = new SortTanksByDistance();
            st.tankController = i.tc;
            st.dist = Vector3.Distance(this.transform.position, i.tc.transform.position);
            allies.Add(st);
        }
 
        allies.Sort(delegate(SortTanksByDistance c1, SortTanksByDistance c2) { return c1.dist.CompareTo(c2.dist); });
 
        // find the farthest ally I can path to and path to them
        for (int i = allies.Count-1; i >= 0; i--)
        {
            PathTo(allies[i].tankController.transform.position);
            if (!ReachedPathfindTarget()) return;
        }
    }
}