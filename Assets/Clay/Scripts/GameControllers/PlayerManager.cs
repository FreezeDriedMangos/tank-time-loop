using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int PlayerNum {get {return _PlayerNum;} set {_PlayerNum = value;}}
    [Range(1,4)] [SerializeField] private int _PlayerNum;

    [SerializeField] private GameObject playerPrefab;
    public List<GameObject> spawnPoints;
    public int tanksLeft {get; private set;}
    
    List<VCR> recordingVCRs;
    List<VCR> playbackVCRs;
    List<TankController> tanks = new List<TankController>();

    public int numSpawnPoints {get; private set;}


    private float freezeTimer;
    private float invincibleTimer;
    private bool freezeTimerRunning;
    private bool invincibleTimerRunning;
    private bool invincibleTimerValue;
    


    private bool setup;

    public void Setup(List<GameObject> spawnPoints)
    {
        if (setup) return;
        setup = true;

        if (!GameState.playerJoined[PlayerNum-1]) return;

        tanks = new List<TankController>();

        recordingVCRs = new List<VCR>();
        playbackVCRs  = new List<VCR>(); 

        this.spawnPoints = spawnPoints;
        this.numSpawnPoints = spawnPoints.Count;

        //spawnPoints = new List<GameObject>();

        // numSpawnPoints = 1;
        // for (int i = 0; i < numSpawnPoints; i++)
        // {
        //     GameObject g = GameObject.Find("p"+PlayerNum+"."+(i+1));
        //     Debug.Log("spawnpoint " + i + ": " + g);

        //     if (g == null) break;

        //     numSpawnPoints++;
        //     spawnPoints.Add(g);
        // }
        // numSpawnPoints--;
    }

    public void HookupIcons(PlayerIconController[][] icons)
    {
        for (int i = 0; i < icons[PlayerNum-1].Length && i < tanks.Count; i++)
        {
            // Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAA player " + (PlayerNum-1) + " tank " + i);
            // PlayerIconController pic = icons[PlayerNum-1][i];
            // tanks[i].vcr.onRogueNotify = () => { pic.SetHologram(); };
        }
    }

    public void UnSetup()
    {
        if (!setup) return;
        setup = false;
    }

    public void SetUpPlayer(int roundNumber)
    {
        SetUpPlayer(roundNumber, null);
    }

    // roundNumber is 1 indexed
    // PlayerNum is 1 indexed
    public void SetUpPlayer(int roundNumber, TankController.NotifyDelegate OnPlayerDestroyed)
    {
        if (!GameState.playerJoined[PlayerNum-1]) return;
        if (roundNumber-1 >= numSpawnPoints) return;

        GameObject p;
        TankController t;
        CursorController c;

        //Debug.Log("player idx " + (PlayerNum-1) + " " + GameState.playerTankPrefabs[PlayerNum-1]);
        p = GameObject.Instantiate(GameState.playerTankPrefabs[PlayerNum-1]);
        p.name = "tank p"+PlayerNum+"."+(roundNumber);

        p.transform.position = 
            spawnPoints[roundNumber-1].transform.position;

        t = p.GetComponent<TankController>();
        if (GameState.players[PlayerNum-1] != null)
        {
            if (GameState.players[PlayerNum-1].cursor != null) GameState.players[PlayerNum-1].cursor.gameObject.SetActive(false);
            GameState.players[PlayerNum-1].tank = t;
            c = GameState.players[PlayerNum-1].cursor = t.transform.Find("Player Cursor").GetComponent<CursorController>();
            c.Setup();
        }
        t.Setup();
        t.onDestroyedNotify = OnPlayerDestroyed;
        if(PlayerNum == 1) t.team = TankController.Team.PLAYER_1;
        if(PlayerNum == 2) t.team = TankController.Team.PLAYER_2;
        if(PlayerNum == 3) t.team = TankController.Team.PLAYER_3;
        if(PlayerNum == 4) t.team = TankController.Team.PLAYER_4;
        
        tanks.Add(t);
        recordingVCRs.Add(p.GetComponent<VCR>());
        
        tanksLeft = roundNumber;
    }

    // corresponds to PlayersManager.DestroyAllTanks
    public void DestroyPlayerTanks()
    {
        if (!GameState.playerJoined[PlayerNum-1]) return;
        if (tanks == null) return;

        foreach (TankController tank in tanks)
            if(tank != null)
                GameObject.Destroy(tank.gameObject);
    }

    public void SetInvincible(bool val)
    {
        foreach (TankController tank in tanks)
            tank.Invincible = val;
    }

    public void UnDestroy()
    {
        foreach (TankController tank in tanks)
            tank.SetDestroyed(false);
    }

    public void SetAppearance(int roundNumber)
    {
        if (!GameState.playerJoined[PlayerNum-1]) return;

        foreach (VCR v in playbackVCRs)
        {
            TankController t = v.GetComponent<TankController>();
            t.SetDestroyed(false);
            TankAppearance a = v.GetComponent<TankAppearance>();
            a.SetHologram(roundNumber);
        }
    }

    public void RecordingVCRs_to_PlaybackVCRs()
    {
        if (!GameState.playerJoined[PlayerNum-1]) return;

        RecordingVCRs_to_PlaybackVCRs(0);
    }

    public void RecordingVCRs_to_PlaybackVCRs(int roundNumber)
    {
        if (!GameState.playerJoined[PlayerNum-1]) return;

        foreach (VCR vcr in recordingVCRs)
        {
            vcr.SetState(VCR.VCRState.IDLE);

            TankAppearance appearance = vcr.GetComponent<TankAppearance>();
            if (appearance == null) continue;
            appearance.SetHologram(roundNumber);
        }

        playbackVCRs.AddRange(recordingVCRs);
        recordingVCRs.Clear();
    }

    public void ResetPlayer()
    {
        if (!GameState.playerJoined[PlayerNum-1]) return;

        foreach (VCR vcr in recordingVCRs)
            vcr.SetState(VCR.VCRState.IDLE);
        
        foreach (VCR vcr in playbackVCRs)
            vcr.SetState(VCR.VCRState.RESETING);
        foreach (TankController tank in tanks)
        {
            tank.SetDestroyed(false);
            tank.frozen = true;
        }
    }

    public void StartPlayer()
    {
        if (!GameState.playerJoined[PlayerNum-1]) return;

        Debug.Log("Starting player " + (PlayerNum));

        foreach (VCR vcr in recordingVCRs)
            vcr.SetState(VCR.VCRState.RECORDING);
        foreach (VCR vcr in playbackVCRs)
            vcr.SetState(VCR.VCRState.PLAYINGBACK);
            
        foreach (TankController tank in tanks)
            tank.frozen = false;
    }

    public void StartPlayerAfterTimer(float timeInSeconds)
    {
        freezeTimer = timeInSeconds;
        freezeTimerRunning = true;
    }
    public void SetInvincibleAfterTimer(bool v, float timeInSeconds)
    {
        invincibleTimer = timeInSeconds;
        invincibleTimerRunning = true;
        invincibleTimerValue = v;
    }

    void FixedUpdate() 
    {
        if (invincibleTimerRunning)
        {
            invincibleTimer -= Time.deltaTime;
            
            if (invincibleTimer <= 0)
            {
                invincibleTimerRunning = false;
                SetInvincible(invincibleTimerValue);
            }
        }    

        if (freezeTimerRunning)
        {
            freezeTimer -= Time.deltaTime;

            if (freezeTimer <= 0)
            {
                freezeTimerRunning = false;
                StartPlayer();
            }
        }
    }
    
}
