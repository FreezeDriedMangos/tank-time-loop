using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TankController))]
public class VCR : MonoBehaviour
{
    public enum VCRState {IDLE, RECORDING, PLAYINGBACK, RESETING}
    public VCRState state {get; private set;}

    private TankController tc;
    private TankTransform  tt;
    private TankAppearance ta;
    //private EnemyTanksController ec;

    //private bool ecWasEnabled = false;
    private bool aicWasEnabled = false;
    
    //bool                  startLocationSet;
    //Vector3               startLocation;
    TankTransformState    startingTransformState;
    Queue<RecordingFrame> recording;
    List<RecordingFrame>  recordingBackup;
    RecordingFrame        latest = new RecordingFrame();
        public Vector2 MoveInDirectionArgs  {get {return latest.MoveInDirectionArgs;}  set { latest.MoveInDirectionArgs  = value;   latest._MoveInDirectionArgs  = true; }}
        public float   MoveInDirection2Args {get {return latest.MoveInDirection2Args;} set { latest.MoveInDirection2Args = value;   latest._MoveInDirection2Args = true; }}
        public Vector2 MoveToArgs           {get {return latest.MoveToArgs;}           set { latest.MoveToArgs           = value;   latest._MoveToArgs           = true; }}
        public bool    StopMovingRisingEdge {get {return latest.StopMovingRisingEdge;} set { latest.StopMovingRisingEdge = value;   latest._StopMovingRisingEdge = true; }}
        public Vector2 AimAtArgs            {get {return latest.AimAtArgs;}            set { latest.AimAtArgs            = value;   latest._AimAtArgs            = true; }}
        public float   AimArgs              {get {return latest.AimArgs;}              set { latest.AimArgs              = value;   latest._AimArgs              = true; }}
        public bool    FireShellRisingEdge  {get {return latest.FireShellRisingEdge;}  set { latest.FireShellRisingEdge  = value;   latest._FireShellRisingEdge  = true; }}
        public bool    FireRocketRisingEdge {get {return latest.FireRocketRisingEdge;} set { latest.FireRocketRisingEdge = value;   latest._FireRocketRisingEdge = true; }}
        public bool    FireBombRisingEdge   {get {return latest.FireBombRisingEdge;}   set { latest.FireBombRisingEdge   = value;   latest._FireBombRisingEdge   = true; }}
        public bool    WasDestroyed         {get {return latest.WasDestroyed;}         set { latest.WasDestroyed   = value;         latest._WasDestroyed         = true; }}

    private bool HaveBeenDestroyed = false;

    public delegate void OnRogueNotifier();
    public OnRogueNotifier onRogueNotify;

    public void Setup() 
    {
        tc = GetComponent<TankController>();
        tt = GetComponent<TankTransform>();
        ta = GetComponent<TankAppearance>();
        //ec = GetComponent<EnemyTanksController>();

        tc.requestDestroy = TrySetRogue;
        //ecWasEnabled = ec.enabled;
        aicWasEnabled = tc.aic.enabled;

        recording = new Queue<RecordingFrame>();
        recordingBackup = new List<RecordingFrame>();
        latest = new RecordingFrame();
        state = VCRState.IDLE;

        startingTransformState = null;
    }

    public void SetState(VCRState s)
    {
        if (s == state) return;

        state = s;

        switch (state)
        {
            case VCRState.RECORDING:   RecordStartState();     ta.SetNatural();         break;
            case VCRState.PLAYINGBACK: ResetTank(); Prepare(); ta.SetHologram(0);       break;
            case VCRState.RESETING:    ResetTank(); Prepare(); SetState(VCRState.IDLE); break;
            default: SetState(VCRState.IDLE); break;
        }

        // Enable / disable other scripts that control the tank
        // This lets VCR be the only script giving the tank input on Playback mode
        if (state == VCRState.PLAYINGBACK) 
        {
            tc.VCRInputOnly = true;
        }
        else
        {
            tc.VCRInputOnly = false;
        }
    }

    private void Prepare()
    {
        HaveBeenDestroyed = false;

        if (recording.Count == recordingBackup.Count) return;

        recording.Clear();
        foreach (RecordingFrame r in recordingBackup)
            recording.Enqueue(r);
    }

    private void RecordStartState()
    {
        if (startingTransformState != null) return;

        startingTransformState = tt.GetCurrentState();
    }

    private void ResetTank()
    {
        tt.SetState(startingTransformState);
        tc.ResetTank();
        //ec.enabled = ecWasEnabled;
        tc.aic.enabled = aicWasEnabled;
        HaveBeenDestroyed = false;
    }

    public void UpdateStep()
    {
        switch (state)
        {
            case VCRState.RECORDING:   RecordStep();   break;
            case VCRState.PLAYINGBACK: PlaybackStep(); break;
        }
    }

    private void RecordStep()
    {
        // if any of these changed, record them
        // this.StopMovingRisingEdge
        // this.MoveToArgs
        // this.MoveInDirectionArgs
        // this.AimAtArgs
        // this.FireShellRisingEdge
        // this.FireRocketRisingEdge
        // this.FireBombRisingEdge

        latest.position = this.transform.position;
        recording.Enqueue(latest);
        recordingBackup.Add(latest);

        // and then null them all out
        latest = new RecordingFrame();
    }

    private void PlaybackStep()
    {
        if (recording.Count <= 0) 
        {
            GoRogue();
            return;
        }

        RecordingFrame curr = recording.Dequeue(); 

        // maybe do each of these steps, depending on the recording data:
        // call StopMoving, MoveTo, or MoveInDirection
        // call AimAt or Aim
        // call FireShell, FireRocket, or FireBomb

        if (curr._MoveInDirectionArgs  )   tc.MoveInDirection (curr.MoveInDirectionArgs  , true);
        if (curr._MoveInDirection2Args )   tc.MoveInDirection (curr.MoveInDirection2Args , true);
        if (curr._MoveToArgs           )   tc.MoveTo          (curr.MoveToArgs           , true);
        if (curr._StopMovingRisingEdge )   tc.StopMoving      (                            true);
        if (curr._AimAtArgs            )   tc.AimAt           (curr.AimAtArgs            , true);
        if (curr._AimArgs              )   tc.Aim             (curr.AimArgs              , true);
        if (curr._FireShellRisingEdge  )   tc.FireShell       (true);
        if (curr._FireRocketRisingEdge )   tc.FireRocket      (true);
        if (curr._FireBombRisingEdge   )   tc.FireBomb        (true);
        if (curr._WasDestroyed         )   HaveBeenDestroyed = true;     

        if (Vector3.Distance(curr.position, this.transform.position) > GameState.config.miscConfig.rogueOnPushDistance) GoRogue();       
    }

    // returns whether or not the tank should be destroyed
    // also hijacks this system to set self to rogue
    public bool TrySetRogue(TankController tc)
    {
        if (!GameState.config.miscConfig.rogueOnEarlyDeath) return true;

        if (state == VCRState.PLAYINGBACK)
        {
            // if I was originally destroyed on this frame or next, destroy me again
            if (HaveBeenDestroyed || recording.Peek()._WasDestroyed || tc.destroyed) return true;

            // if we're in the playback state (ie, the tank is a recording)
            GoRogue();
            return false;
        }
        else
        {
            // if we're not a recording
            return true;
        }
    }

    private void GoRogue()
    {
        tc.VCRInputOnly = false;
        tc.GoneRogue = true;
        ta.SetRogue(0);

        //ec.enabled = true;
        tc.aic.enabled = true;
        SetState(VCRState.IDLE);
        if (onRogueNotify != null) onRogueNotify();
    }
}

class RecordingFrame
{
    public Vector2 MoveInDirectionArgs;
    public float   MoveInDirection2Args;
    public Vector2 MoveToArgs;
    public bool    StopMovingRisingEdge;
    public Vector2 AimAtArgs;
    public float   AimArgs;
    public bool    FireShellRisingEdge;
    public bool    FireRocketRisingEdge;
    public bool    FireBombRisingEdge;

    public bool    WasDestroyed;

    public Vector3 position;

    // since there's no nullable Boolean, Vector2, or Float types in C#, the below variables record whether their corresponding variables are "null" or not
    public bool _MoveInDirectionArgs  = false;
    public bool _MoveInDirection2Args = false;
    public bool _MoveToArgs           = false;
    public bool _StopMovingRisingEdge = false;
    public bool _AimAtArgs            = false;
    public bool _AimArgs              = false;
    public bool _FireShellRisingEdge  = false;
    public bool _FireRocketRisingEdge = false;
    public bool _FireBombRisingEdge   = false;

    public bool _WasDestroyed         = false;


    public void Clear()
    {
        _MoveInDirectionArgs  = false;
        _MoveInDirection2Args = false;
        _MoveToArgs           = false;
        _StopMovingRisingEdge = false;
        _AimAtArgs            = false;
        _AimArgs              = false;
        _FireShellRisingEdge  = false;
        _FireRocketRisingEdge = false;
        _FireBombRisingEdge   = false;
        _WasDestroyed         = false;
    }
}