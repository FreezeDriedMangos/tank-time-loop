using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TankTransform))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(VCR))]
[RequireComponent(typeof(TankAppearance))]
//[RequireComponent(typeof(EnemyTanksController))]
public class TankController : MonoBehaviour
{   
    public static List<TankController> allTanks = new List<TankController>();

    public enum Team {NONE,    PLAYER_1, PLAYER_2,    PLAYER_3,       PLAYER_4,     ENEMY}
    public enum Type {GENERIC, PLAYER,   STILL_ENEMY, STANDARD_ENEMY, ROCKET_ENEMY, BOMB_ENEMY, FAST_ENEMY, BOMB_AND_STANDARD_ENEMY}

    // constants
    public const float MOVETO_LOCATION_TOLERANCE = 0.01f;
    public const float BODY_ROTATION_TOLERANCE = 0.01f;
    public const float HEAD_ROTATION_TOLERANCE = 0.01f;
    public const float MOVETO_SLOWDOWN_RADIUS = 0.5f;
    
    //
    // params - don't change during execution
    //
    [Header("misc")]
    [SerializeField] private Team _team;
                     public  Team team { get { return _team; } set { _team = value; }}
    [SerializeField] private Type _type;
                     public  Type type { get { return _type; } set { _type = value; }}
    

    [Header("movement")]
    [SerializeField] private float turnSpeed;
    [SerializeField] private float moveSpeed;

    [Header("shooting stuff")]
    [SerializeField] private float fireShellCooldown_inSeconds;
    [SerializeField] private float fireRocketCooldown_inSeconds;
    [SerializeField] private float fireBombCooldown_inSeconds;

    [SerializeField] private float freezeTimeAfterFireShell_inSeconds;
    [SerializeField] private float freezeTimeAfterFireRocket_inSeconds;
    [SerializeField] private float freezeTimeAfterFireBomb_inSeconds;

    [SerializeField] private int shellCountLimit;
    [SerializeField] private int rocketCountLimit;
    [SerializeField] private int bombCountLimit;

    [Header("more movement stuff")]
    [Range(0, 90f)] [SerializeField] private float faceFrontBias_Degrees;
    [Range(0, 90f)] [SerializeField] private float turningRadius_inDegrees; // if tt.BodyRotation is within turningRadius_inDegrees of its target rotation, the tank will start to move while still turning
    
    [Header("aesthetic")]
    [SerializeField] private bool leaveTracks;
    [SerializeField] private float leaveTracksEveryXSeconds;
    [SerializeField] private float leaveTracksEveryXUnitsDriven;
    [SerializeField] private GameObject tracksPrefab;
    [SerializeField] private GameObject fireShellMarker;
    [SerializeField] private GameObject fireRocketMarker;
    [SerializeField] private GameObject fireBombMarker;
    [SerializeField] private GameObject deathMarker;
    public static GameObject TracksMaster;

    // prefabs
    [Header("prefabs")]
    [SerializeField] private GameObject shellPrefab;
    [SerializeField] private GameObject rocketPrefab;
    [SerializeField] private GameObject bombPrefab;

    // sfx
    [Header("sounds")]
    [SerializeField] private AudioSource whirrSound = null;
    [SerializeField] private AudioSource moveSound = null;
    [SerializeField] private AudioSource turnSound = null;
    [SerializeField] private AudioSource destroySound = null;
    [SerializeField] private AudioSource fireSound = null;
    

    // Components
    public TankTransform tt {get; private set;}
    public TankAppearance ta {get; private set;}
    private Rigidbody rb;
    public VCR vcr;
    //private EnemyTanksController ec;
    public AIController aic {get; private set;}

    //
    // "return values"
    //

    public bool lastShellFireWasSuccess  {get; private set;}
    public bool lastRocketFireWasSuccess {get; private set;}
    public bool lastBombFireWasSuccess   {get; private set;}

    public bool reachedMoveTarget {get; private set;}

    //
    // State-related stuff
    //
    private bool setup = false;

    // state information - followers
    private Vector2 moveTarget;
    private Vector3 moveDirection;

    // state information - leaders
    private bool  stopOnTarget;
    private float targetBodyRotation;
    private float targetHeadRotation;

    private float remainingFreezeTime_inSeconds;

    private float remainingShellCooldown_inSeconds;
    private float remainingRocketCooldown_inSeconds;
    private float remainingBombCooldown_inSeconds;

    public int numExistingShells  { get; private set; }
    public int numExistingRockets { get; private set; }
    public int numExistingBombs   { get; private set; }

    private bool firingShell = false;
    private bool firingRocket = false;
    private bool firingBomb = false;


    private float leaveTracksTimer = 0;

    
    // state 
    public  bool frozen  { get{ return _frozen || remainingFreezeTime_inSeconds > 0; }  set { _frozen = value; frozenExceptForAiming = value; } }
    private bool _frozen = false;

    public bool  moving { get { return _moving; } private set {_moving = value; moveSound.mute = !value;}}
    private bool _moving = false;

    public bool  turning { get { return _turning; } private set {_turning = value; turnSound.mute = !value;}}
    private bool _turning = false;

    public bool destroyed { get {return _destroyed;} set { SetDestroyed(value); }}
    private bool _destroyed = false;

    public bool VCRInputOnly { get {return _VCRInputOnly;} set {_VCRInputOnly = value;}}
    private bool _VCRInputOnly = false;

    public bool GoneRogue { get {return _GoneRogue;} set {_GoneRogue = value;}}
    private bool _GoneRogue = false;

    public bool IsHologram { get { return VCRInputOnly || GoneRogue; } }

    public bool Invincible { get { return _Invincible; } set { _Invincible = value; }}
    private bool _Invincible = false;

    public bool frozenExceptForAiming { get { return _frozenExceptForAiming; } set {_frozenExceptForAiming = value;} }
    private bool _frozenExceptForAiming = false;

    public bool canMove { get { return !frozenExceptForAiming && !frozen && turnSpeed > 0 && moveSpeed > 0; } } 

    // notifiers
    public delegate void NotifyDelegate(TankController t);
    public NotifyDelegate onDestroyedNotify { get { return onDestroyedNotifyList[0]; }  set {onDestroyedNotifyList.Add(value);} }
    private List<NotifyDelegate> onDestroyedNotifyList = new List<NotifyDelegate>();

    // permission requests
    public delegate bool RequestDelegate(TankController t);
    public RequestDelegate requestDestroy; 

    //
    // =====================================================================
    //

    //
    // setup functions
    //

    void Awake()
    {
        allTanks.Add(this);
        SetVolume();
    }

    private void Start() 
    {
        Setup();
    }

    public void Setup()
    {
        if(setup) return;
        setup = true;

        // grab components
        tt = GetComponent<TankTransform>();
        ta = GetComponent<TankAppearance>();
        rb = GetComponent<Rigidbody>();
        vcr = GetComponent<VCR>();
        //ec = GetComponent<EnemyTanksController>();
        aic = GetComponent<AIController>();

        // read from config file
        if (GameState.config != null) SetParameters();

        // set my state
        turning = false;
        moving = false;

        // set up components
        ta.Setup();
        ta.SetNatural();

        ResetTank();

        vcr.Setup();

        if (aic != null) aic.Setup();
    }

    private void SetParameters()
    {
        TankConfig configData = GameState.ConfigUtility.GetConfigForTankType(this.type);

        if (configData == null) return;

        //this.team = configData.team;
        //this.type = configData.type;

        
        this.turnSpeed = configData.turnSpeed;
        this.moveSpeed = configData.moveSpeed;

        
        this.fireShellCooldown_inSeconds = configData.fireShellCooldown_inSeconds;
        this.fireRocketCooldown_inSeconds = configData.fireRocketCooldown_inSeconds;
        this.fireBombCooldown_inSeconds = configData.fireBombCooldown_inSeconds;

        this.freezeTimeAfterFireShell_inSeconds = configData.freezeTimeAfterFireShell_inSeconds;
        this.freezeTimeAfterFireRocket_inSeconds = configData.freezeTimeAfterFireRocket_inSeconds;
        this.freezeTimeAfterFireBomb_inSeconds = configData.freezeTimeAfterFireBomb_inSeconds;

        this.shellCountLimit = configData.shellCountLimit;
        this.rocketCountLimit = configData.rocketCountLimit;
        this.bombCountLimit = configData.bombCountLimit;

        
        this.faceFrontBias_Degrees = configData.faceFrontBias_Degrees;
        this.turningRadius_inDegrees = configData.turningRadius_inDegrees;


        this.leaveTracks = configData.leaveTracks;
        this.leaveTracksEveryXSeconds = configData.leaveTracksEveryXSeconds;
        this.leaveTracksEveryXUnitsDriven = configData.leaveTracksEveryXUnitsDriven;


        // set AI parameters
        //ec.shootsRockets = configData.aiConfig.firesRockets;

        
        if (aic != null) aic.SetupConfig(configData.aiConfig);
    }

    void OnDestroy()
    {
        allTanks.Remove(this);
    }

    //
    // Fixed Update
    //

    private void FixedUpdate() 
    {
        if (vcr != null) vcr.UpdateStep();

        if (transform.position.y < -5) SetDestroyed(true);

        this.UpdateStep();
    }

    //
    // Update
    //

    private void Update() 
    {
        SetVolume();
    }

    //
    // Etc
    //

    public void SetVolume()
    {
        if (GameState.config == null) return;

        float volume = PlayerPrefs.GetFloat("SFXVolume");
        
        SFXConfig c1 = GameState.ConfigUtility.GetConfigForSFX("tank_whirrSound");
        SFXConfig c2 = GameState.ConfigUtility.GetConfigForSFX("tank_moveSound");
        SFXConfig c3 = GameState.ConfigUtility.GetConfigForSFX("tank_turnSound");
        SFXConfig c4 = GameState.ConfigUtility.GetConfigForSFX("tank_destroySound");
        SFXConfig c5 = GameState.ConfigUtility.GetConfigForSFX("tank_fireSound");   

        whirrSound.volume   = volume * c1.volume;
        moveSound.volume    = volume * c2.volume;
        turnSound.volume    = volume * c3.volume;
        destroySound.volume = volume * c4.volume;
        fireSound.volume    = volume * c5.volume;

        whirrSound.pitch   = Random.Range(c1.pitchMin, c1.pitchMax);
        moveSound. pitch   = Random.Range(c2.pitchMin, c2.pitchMax);
        turnSound. pitch   = Random.Range(c3.pitchMin, c3.pitchMax);
        destroySound.pitch = Random.Range(c4.pitchMin, c4.pitchMax);
        fireSound.pitch    = Random.Range(c5.pitchMin, c5.pitchMax);

        whirrSound.Play();
    }

    public void ResetTank()
    {
        // state
        frozen = false;
        moving = false;
        turning = false;

        // state information - leaders
        stopOnTarget = false;
        targetBodyRotation = tt.BodyRotation;
        targetHeadRotation = tt.HeadRotation;

        remainingFreezeTime_inSeconds = 0;

        remainingShellCooldown_inSeconds  = 0;//fireShellCooldown_inSeconds  + (float)Random.Range(0, 0.1f);
        remainingRocketCooldown_inSeconds = 0;//fireRocketCooldown_inSeconds + (float)Random.Range(0, 0.1f);
        remainingBombCooldown_inSeconds   = 0;//fireBombCooldown_inSeconds   + (float)Random.Range(0, 0.1f);

        numExistingShells = 0;
        numExistingRockets = 0;
        numExistingBombs = 0;

        rb.velocity        = new Vector3(0, 0, 0);
        rb.angularVelocity = new Vector3(0, 0, 0);

        SetDestroyed(false);
    }

    public void SetDestroyed(bool val) 
    {
        if (val == destroyed) return;
        if (val && Invincible) return;

        if (requestDestroy != null) if (val && !requestDestroy(this)) return;

        _destroyed = val;
        if (vcr != null) vcr.WasDestroyed = val;

        if (val) frozen = true;

        if (val) ta.SetDestroyed(false);
        if (!val) ta.SetNatural();

        transform.Find("BustedTank").gameObject.SetActive(val);
        transform.Find("TankRenderers").gameObject.SetActive(!val);
        transform.Find("TankHitbox").gameObject.SetActive(!val);
        //transform.Find("Leveler").gameObject.SetActive(!val);
        whirrSound.mute = val;

        Collider[] colliders = GetComponents<Collider>();
        foreach (Collider c in colliders)
            c.isTrigger = val;
        // rb.enabled = val;
        rb.isKinematic = val;

        if (val) destroySound.Play();

        if (val)
        {
            if (!(!GameState.config.miscConfig.hologramsLeaveTracks && IsHologram) && GameState.config.miscConfig.deathMarkers)
            {
                SpawnTrack(deathMarker, false);
            }
        }

        if (val) if (aic != null) aic.ReSetup();

        if (val)
        {
            foreach(NotifyDelegate d in onDestroyedNotifyList)
                d(this);
        }
        //if (onDestroyedNotify != null && val)
        //    onDestroyedNotify(this);
    }

    // ===============================================
    //
    // Input Functions:
    //
    // MoveInDirection(Vector2)
    // MoveInDirection(float)
    // MoveTo(Vector2)
    // StopMoving()
    //
    // AimAt()
    // Aim()
    //
    // FireShell()   -- after next fixed update, success is stored in lastShellFireWasSuccess
    // FireRocket()  -- after next fixed update, success is stored in lastRocketFireWasSuccess
    // FireBomb()    -- after next fixed update, success is stored in lastBombFireWasSuccess
    //
    // ===============================================

    //
    // Movement
    //

    public void MoveInDirection(Vector2 dir)    { MoveInDirection(dir, false);      }
    public void MoveInDirection(float dirAngle) { MoveInDirection(dirAngle, false); }
    public void MoveTo(Vector2 loc)             { MoveTo(loc, false); }
    public void StopMoving()                    { StopMoving(false);  }


    public void MoveInDirection(Vector2 dir, bool isVCRInput)
    {
        if (!isVCRInput && VCRInputOnly) return;

        if (dir == Vector2.zero)
        {
            StopMoving();
            return;
        }
        if (vcr != null) vcr.MoveInDirectionArgs = dir;


        dir = dir.normalized;

        moveDirection = new Vector3(dir.x, 0, dir.y);
        SetTargetRotation(Mathf.Rad2Deg * -Mathf.Atan2(dir.y, dir.x));
        moveTarget = new Vector2();
        stopOnTarget = false;
    }

    public void MoveInDirection(float dirAngle, bool isVCRInput)
    {
        if (!isVCRInput && VCRInputOnly) return;

        if (vcr != null) vcr.MoveInDirection2Args = dirAngle;

        dirAngle *= Mathf.Deg2Rad;
        Vector2 dir = new Vector2(Mathf.Cos(dirAngle), Mathf.Sin(dirAngle));

        moveDirection = new Vector3(dir.x, 0, dir.y);
        SetTargetRotation(dirAngle);
        moveTarget = new Vector2();
        stopOnTarget = false;
    }

    public void MoveTo(Vector2 loc, bool isVCRInput)
    {
        if (!isVCRInput && VCRInputOnly) return;
        
        Vector2 pos2 = new Vector2(transform.position.x, transform.position.z);
        if (Vector2.Distance(pos2, loc) < MOVETO_LOCATION_TOLERANCE * moveSpeed/80f) return;

        if (vcr != null) vcr.MoveToArgs = loc;

        reachedMoveTarget = false;

        // Vector2 pos2 = new Vector2(transform.position.x, transform.position.z);
        MoveInDirection(loc - pos2);
        moveTarget = loc;
        stopOnTarget = true;
    }

    public void StopMoving(bool isVCRInput)
    {
        if (!isVCRInput && VCRInputOnly) return;
        
        if (vcr != null) vcr.StopMovingRisingEdge = true;

        moveDirection = Vector3.zero;
        moving = false;
        turning = false;
    }

    //
    // Aiming
    //

    public void AimAt(Vector2 loc)  { AimAt(loc, false); }
    public void Aim(float dir)      { Aim(dir, false);   }


    public void AimAt(Vector2 loc, bool isVCRInput)
    {
        if (!isVCRInput && VCRInputOnly) return;
        
        if (vcr != null) vcr.AimAtArgs = loc;

        if (frozen) return;
        Aim(Mathf.Rad2Deg * -Mathf.Atan2(loc.y-transform.position.z, loc.x-transform.position.x));
        // aiming = true;
    }

    public void Aim(float dir, bool isVCRInput)
    {
        if (!isVCRInput && VCRInputOnly) return;
        
        if (vcr != null) vcr.AimArgs = dir;

        //if (dir == targetHeadRotation) return;

        //targetHeadRotation = dir;
        //aiming = true;

        if (frozen) return;
        //tt.HeadRotation = dir;

        targetHeadRotation = dir;
    }
    
    //
    // Firing
    //

    public void FireShell()  { FireShell(false);  }
    public void FireRocket() { FireRocket(false); }
    public void FireBomb()   { FireBomb(false);   }
    

    public void FireShell(bool isVCRInput)
    {
        if (!isVCRInput && VCRInputOnly) return;
        
        lastShellFireWasSuccess = false;

        if (vcr != null) vcr.FireShellRisingEdge = true;
        firingShell = true;
    }

    public void FireRocket(bool isVCRInput)
    {
        if (!isVCRInput && VCRInputOnly) return;
        
        lastRocketFireWasSuccess = false;

        if (vcr != null) vcr.FireRocketRisingEdge = true;
        firingRocket = true;
    }

    public void FireBomb(bool isVCRInput)
    {
        if (!isVCRInput && VCRInputOnly) return;
        
        lastBombFireWasSuccess = false;
        if (vcr != null) vcr.FireBombRisingEdge = true;
        firingBomb = true;
    }

    public bool CanFireShell()
    {
        if (frozen) return false;
        if (numExistingShells >= shellCountLimit) return false;  
        if (remainingShellCooldown_inSeconds > 0) return false;
        return true;
    }

    public bool CanFireRocket()
    {
        if (frozen) return false;
        if (numExistingRockets >= rocketCountLimit) return false; 
        if (remainingRocketCooldown_inSeconds > 0) return false;

        return true;
    }

    public bool CanFireBomb()
    {
        if (frozen) return false;
        if (numExistingBombs >= bombCountLimit) return false; 
        if (remainingBombCooldown_inSeconds > 0) return false;

        return true;
    }

    // =======================================
    //
    // Private helper functions
    //
    // =======================================

    private bool SpawnShell() 
    {
        if (!CanFireShell()) return false;

        if (!(!GameState.config.miscConfig.hologramsLeaveTracks && IsHologram) && GameState.config.miscConfig.fireShellMarkers)
        {
            SpawnTrack(fireShellMarker);
        }

        // spawn a shell and set its location and facing direction properly, based on the tank's head
        GameObject shell = GameObject.Instantiate(shellPrefab);
        shell.transform.rotation = tt.ShellSpawnRotation;
        shell.transform.position = tt.ShellSpawnLocation;

        // register the shell to this tank's team
        shell.GetComponent<BulletController>().firedBy = team;
        
        // notify the BulletController script
        shell.GetComponent<BulletController>().Setup();

        // add a listener for when the shell despawns / is GameObject.Destroy()'d
        shell.AddComponent<ObjectDestructionNotifier>();
        shell.GetComponent<ObjectDestructionNotifier>().notify = ShellDidDespawn;

        // update the shell count for this shell type
        numExistingShells++;

        // make the tank pause after shooting
        remainingFreezeTime_inSeconds = freezeTimeAfterFireShell_inSeconds;
        remainingShellCooldown_inSeconds = fireShellCooldown_inSeconds;

        // play the sound
        fireSound.pitch = Random.Range(0.8f, 1.3f);
        fireSound.Play();

        return true;
    }

    private bool SpawnRocket()
    {
        if (!CanFireRocket()) return false;

        if (!(!GameState.config.miscConfig.hologramsLeaveTracks && IsHologram) && GameState.config.miscConfig.fireRocketMarkers)
        {
            SpawnTrack(fireRocketMarker);
        }

        // spawn a shell and set its location and facing direction properly, based on the tank's head
        GameObject shell = GameObject.Instantiate(rocketPrefab);
        shell.transform.rotation = tt.ShellSpawnRotation;
        shell.transform.position = tt.ShellSpawnLocation;

        // register the shell to this tank's team
        shell.GetComponent<BulletController>().firedBy = team;

        // notify the RocketController script
        shell.GetComponent<BulletController>().Setup();

        // add a listener for when the shell despawns / is GameObject.Destroy()'d
        shell.AddComponent<ObjectDestructionNotifier>();
        shell.GetComponent<ObjectDestructionNotifier>().notify = RocketDidDespawn;

        // update the shell count for this shell type
        numExistingRockets++;

        // make the tank pause after shooting
        remainingFreezeTime_inSeconds = freezeTimeAfterFireShell_inSeconds;
        remainingRocketCooldown_inSeconds = fireRocketCooldown_inSeconds;

        // play the sound
        fireSound.pitch = Random.Range(1f, 1.5f);
        fireSound.Play();

        return true;
    }

    private bool SpawnBomb()
    {
        if (!CanFireBomb()) return false;

        if (!(!GameState.config.miscConfig.hologramsLeaveTracks && IsHologram) && GameState.config.miscConfig.fireBombMarkers)
        {
            SpawnTrack(fireBombMarker);
        }

        // spawn a shell and set its location and facing direction properly, based on the tank's head
        GameObject shell = GameObject.Instantiate(bombPrefab);
        shell.transform.rotation = tt.ShellSpawnRotation;
        shell.transform.position = tt.ShellSpawnLocation;
        shell.GetComponent<BombController>().direction = tt.ShellSpawnRotation.eulerAngles.y;

        // register the shell to this tank's team
        shell.GetComponent<BombController>().firedBy = team;

        // notify the BombController script
        shell.GetComponent<BombController>().Setup();

        // add a listener for when the shell despawns / is GameObject.Destroy()'d
        shell.AddComponent<ObjectDestructionNotifier>();
        shell.GetComponent<ObjectDestructionNotifier>().notify = BombDidDespawn;

        // update the shell count for this shell type
        numExistingBombs++;

        // make the tank pause after shooting
        remainingFreezeTime_inSeconds = freezeTimeAfterFireShell_inSeconds;
        remainingBombCooldown_inSeconds = fireBombCooldown_inSeconds;

        // play the sound
        fireSound.pitch = Random.Range(0.5f, 0.8f);
        fireSound.Play();

        return true;
    }

    private void SetTargetRotation(float angle)
    {
        // find out whether desiredAngle or (desiredAngle+180) is closer to current body rotation
        // then set targetBodyRotation equal to the winner 

        float flip = angle + 180f;
        float flipDiff = Mathf.Abs(Mathf.DeltaAngle(tt.BodyRotation, flip));
        float angleDiff = Mathf.Abs(Mathf.DeltaAngle(tt.BodyRotation, angle));

        if (angleDiff <= BODY_ROTATION_TOLERANCE) { tt.BodyRotation = angle; moving = true; turning = false; tt.facingForwards = true;  return; }
        if (flipDiff  <= BODY_ROTATION_TOLERANCE) { tt.BodyRotation = flip;  moving = true; turning = false; tt.facingForwards = false; return; }

        if (angleDiff < flipDiff+faceFrontBias_Degrees)
        {
            targetBodyRotation = angle;
            tt.facingForwards = true;
        }
        else 
        {
            targetBodyRotation = flip;
            tt.facingForwards = false;
        }

        turning = true;
        moving = false;
    }

    // =======================================
    //
    // UpdateStep
    // the tank itself only updates in this function
    // everything else only sets variables that say what to do next time this function happens
    //
    // =======================================

    private void UpdateStep()
    {
        // I don't think this does anything anymore, but I'm afraid to remove it
        // it's a relic from the time we had a separate box collider/rigidbody keeping the tank level
        tt.LevelTank();

        // track cooldowns
        if (remainingShellCooldown_inSeconds >= 0)  remainingShellCooldown_inSeconds  -= Time.deltaTime;
        if (remainingRocketCooldown_inSeconds >= 0) remainingRocketCooldown_inSeconds -= Time.deltaTime;
        if (remainingBombCooldown_inSeconds >= 0)   remainingBombCooldown_inSeconds   -= Time.deltaTime;
        
        // if tank can't move, update the timer, eat the player's input, end the function
        if (frozen)
        {
            remainingFreezeTime_inSeconds -= Time.deltaTime;

            firingShell  = false;
            firingRocket = false;
            firingBomb   = false;

            return;
        }

        // aim
        tt.HeadRotation = targetHeadRotation;

        if (frozenExceptForAiming)
        {
            firingShell  = false;
            firingRocket = false;
            firingBomb   = false;

            return;
        }

        // fire a shell if we're supposed to
        if (firingShell)  { lastShellFireWasSuccess  = SpawnShell();  firingShell  = false; }
        if (firingRocket) { lastRocketFireWasSuccess = SpawnRocket(); firingRocket = false; }
        if (firingBomb)   { lastBombFireWasSuccess   = SpawnBomb();   firingBomb   = false; }

        if (stopOnTarget && !reachedMoveTarget)
        {
            // if we haven't finished MoveTo yet, update our direction in case we get pushed off course
            
            // calculate new direction
            // if different from current direction, call MoveTo again
            // if opposite from current direction, target reached

            Vector2 pos2 = new Vector2(transform.position.x, transform.position.z);
            Vector2 newDir = (moveTarget - pos2).normalized;
            Vector2 moveDir2 = new Vector2(moveDirection.x, moveDirection.z).normalized;

            if (newDir == -moveDir2) reachedMoveTarget = true;
            else if (newDir != moveDir2) MoveTo(moveTarget);
        }

        // record the move direction for this frame
        // we do this instead of just using moveDirection directly because we 
        // might need to change the currentMoveDirection if this is a "move before
        // the turning is done" frame. In which case we need a different move direction
        Vector3 currentMoveDir = moveDirection;

        // handle turning in all its complexities
        if(turning)
        {
            // how far we have left to complete the turn
            float deltaAngleLeft = Mathf.Abs(Mathf.DeltaAngle(tt.BodyRotation, targetBodyRotation));

            if (deltaAngleLeft <= BODY_ROTATION_TOLERANCE)
            {
                // if the turn is complete,
                turning = false;
                moving = true;
                tt.BodyRotation = targetBodyRotation;
            }
            else
            {
                // turn the tank a little bit
                float diff = Mathf.Abs(Mathf.DeltaAngle(targetBodyRotation, tt.BodyRotation));
                tt.BodyRotation = Mathf.LerpAngle(tt.BodyRotation, targetBodyRotation, Time.deltaTime*turnSpeed / diff);
            }

            // handle moving while finishing a turn
            if (deltaAngleLeft < turningRadius_inDegrees)
            {
                moving = true;
                if (tt.facingForwards)
                    currentMoveDir = new Vector3(Mathf.Cos(Mathf.Deg2Rad*tt.BodyRotation), 0, -Mathf.Sin(Mathf.Deg2Rad*tt.BodyRotation));
                else
                    currentMoveDir = new Vector3(-Mathf.Cos(Mathf.Deg2Rad*tt.BodyRotation), 0, Mathf.Sin(Mathf.Deg2Rad*tt.BodyRotation));
            }
        }

        if (moving)
        {
            float speed = moveSpeed * Time.deltaTime;

            Vector2 pos2 = new Vector2(transform.position.x, transform.position.z);
            float distToTarg = Vector2.Distance(pos2, moveTarget);
            if (stopOnTarget && distToTarg <= MOVETO_SLOWDOWN_RADIUS)
            {
                // if we're close enough to the target that we want to slow down, start slowing down
                speed *= distToTarg / MOVETO_SLOWDOWN_RADIUS;
            }

            // various methods for actually moving the tank
                // works, but requires rb.rotation to be frozen, and so the player's always horizontal, even on slopes
                // good speed: 80
                // lies. this method actually works really well even without rb.rotation frozen
                
                Vector3 vel = currentMoveDir * speed;
                rb.velocity = new Vector3(vel.x, rb.velocity.y+0.1f, vel.z);
                // rb.velocity = new Vector3(vel.x, rb.velocity.y+0.1f, vel.z)
                //             + new Vector3(0, speed*tt.Forward.y, 0);

                //rb.velocity = tt.Forward * speed + new Vector3(0,0.1f,0);



                // works, but a bit jittery.
                // good speed: 1.2
                //rb.MovePosition(transform.position + currentMoveDir * moveSpeed * Time.deltaTime);

                // doesn't really work.
                //transform.position += Time.deltaTime * moveSpeed * currentMoveDir;
                
                // doesn't really work
                //rb.AddForce(currentMoveDir * moveSpeed * Time.fixedDeltaTime, ForceMode.Impulse);


            // check to see if MoveTo() has finally completed
            if ((stopOnTarget && reachedMoveTarget) || (stopOnTarget && distToTarg < MOVETO_LOCATION_TOLERANCE))
            {
                reachedMoveTarget = true;
                transform.position = new Vector3(moveTarget.x, transform.position.y, moveTarget.y);
                moving = false;
                stopOnTarget = false;
            }


            // leave tracks
            if (leaveTracks && !(!GameState.config.miscConfig.hologramsLeaveTracks && IsHologram)) 
            {
                //leaveTracksTimer -= Time.deltaTime;
                leaveTracksTimer -= speed;

                if (leaveTracksTimer <= 0)
                {
                    leaveTracksTimer = leaveTracksEveryXUnitsDriven;

                    SpawnTrack(tracksPrefab);
                }
            }
        }
    }

    private void SpawnTrack(GameObject tracksPrefab)
    {
        SpawnTrack(tracksPrefab, true);
    }
    

    private void SpawnTrack(GameObject tracksPrefab, bool rotateToFacing)
    {
        if (TracksMaster == null)
        {
            TracksMaster = new GameObject("TracksMaster");
        }

        GameObject tracks = GameObject.Instantiate(tracksPrefab);
        tracks.transform.parent = TracksMaster.transform;
        tracks.transform.position = this.transform.position + new Vector3(0, 0.01f, 0); // the extra y is to prevent z fighting
        if (rotateToFacing) tracks.transform.eulerAngles = new Vector3(0, tt.BodyRotation, 0);
    }
    
    //
    // Firing delegates
    //

    public void ShellDidDespawn()  { numExistingShells--;  }
    public void RocketDidDespawn() { numExistingRockets--; }
    public void BombDidDespawn()   { numExistingBombs--;   }

    //
    // Static functions
    //

    public static void ClearTracks()
    {
        GameObject.Destroy(TracksMaster);
    }
}
