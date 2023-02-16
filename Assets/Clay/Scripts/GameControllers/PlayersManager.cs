using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// manages player tanks during gameplay
public class PlayersManager : MonoBehaviour
{
    public GameObject player1Prefab;
    public GameObject player2Prefab;
    public GameObject player3Prefab;
    public GameObject player4Prefab;

    public List<GameObject> p1Starts;
    public List<GameObject> p2Starts;
    public List<GameObject> p3Starts;
    public List<GameObject> p4Starts;
    
    public int p1sLeft {get; private set;}
    public int p2sLeft {get; private set;}
    public int p3sLeft {get; private set;}
    public int p4sLeft {get; private set;}
    
    List<VCR> recordingVCRs;
    List<VCR> playbackVCRs;
    List<TankController> tanks = new List<TankController>();
    List<List<TankController>> playerTanks = new List<List<TankController>>();

    private bool setup = false;

    public int numSpawnPoints = 5;


    public void DestroyAllTanks()
    {
        if (tanks == null) return;

        foreach (TankController tank in tanks)
            if(tank != null)
                GameObject.Destroy(tank.gameObject);
    }

    public void UnSetup()
    {
        if (!setup) return;
        setup = false;
    }

    public void Setup()
    {
        if (setup) return;
        setup = true;

        tanks = new List<TankController>(GameObject.FindObjectsOfType<TankController>());

        recordingVCRs = new List<VCR>();
        playbackVCRs  = new List<VCR>(); 

        LevelBuilder b = GameObject.FindObjectOfType<LevelBuilder>();
        if (b == null)
        {
            p1Starts = new List<GameObject>();
            p1Starts.Add(GameObject.Find("p1.1"));
        }

        else
        {
            p1Starts = b.spawnPoints[0]; //new List<GameObject>();  
            p2Starts = b.spawnPoints[1]; //new List<GameObject>();
            p3Starts = b.spawnPoints[2]; //new List<GameObject>();
            p4Starts = b.spawnPoints[3]; //new List<GameObject>();
        }

        // numSpawnPoints = Mathf.Min(p1Starts.Count, 
        //                  Mathf.Min(p2Starts.Count, 
        //                  Mathf.Min(p3Starts.Count, 
        //                            p4Starts.Count)));

        numSpawnPoints = 9999;
        if (GameState.playerJoined[0]) numSpawnPoints = Mathf.Min(numSpawnPoints, p1Starts.Count);
        if (GameState.playerJoined[1]) numSpawnPoints = Mathf.Min(numSpawnPoints, p2Starts.Count);
        if (GameState.playerJoined[2]) numSpawnPoints = Mathf.Min(numSpawnPoints, p3Starts.Count);
        if (GameState.playerJoined[3]) numSpawnPoints = Mathf.Min(numSpawnPoints, p4Starts.Count);
        if (numSpawnPoints == 9999) numSpawnPoints = 0;

        // numSpawnPoints = 1;
        // for (int i = 0; i < numSpawnPoints; i++)
        // {
        //     GameObject g1 = (GameObject.Find("p1."+(i+1)));
        //     GameObject g2 = (GameObject.Find("p2."+(i+1)));
        //     GameObject g3 = (GameObject.Find("p3."+(i+1)));
        //     GameObject g4 = (GameObject.Find("p4."+(i+1)));

        //     if (g1 == null || g2 == null || g3 == null || g4 == null) break;

        //     numSpawnPoints++;
        //     p1Starts.Add(g1);
        //     p2Starts.Add(g2);
        //     p3Starts.Add(g3);
        //     p4Starts.Add(g4);
        // }
        // numSpawnPoints -= 1;

        playerTanks = new List<List<TankController>>();
        playerTanks.Add(new List<TankController>());
        playerTanks.Add(new List<TankController>());
        playerTanks.Add(new List<TankController>());
        playerTanks.Add(new List<TankController>());
    }

    public void SetUpPlayers(int roundNumber)
    {
        SetUpPlayers(roundNumber, null);
    }

    public void SetUpPlayers(int roundNumber, TankController.NotifyDelegate OnPlayerDestroyed)
    {
        Setup();

        if (roundNumber > numSpawnPoints) return;

        GameObject p;
        TankController t;
        CursorController c;

        // ======================
        // Player 1
        // ======================
        if (GameState.playerJoined[0])
        {
            p = GameObject.Instantiate(GameState.playerTankPrefabs[0]);
            p.name = "tank p1."+(roundNumber);
            p.transform.position = p1Starts[roundNumber-1].transform.position;

            
            t = p.GetComponent<TankController>();
            if (GameState.players[0] != null)
            {
                if (GameState.players[0].cursor != null) GameState.players[0].cursor.gameObject.SetActive(false);
                GameState.players[0].tank = t;
                c = GameState.players[0].cursor = t.transform.Find("Player Cursor").GetComponent<CursorController>();
                c.Setup();
            }
            t.Setup();
            t.onDestroyedNotify = OnPlayerDestroyed;
            t.team = TankController.Team.PLAYER_1;

            tanks.Add(t);
            playerTanks[0].Add(t);
            recordingVCRs.Add(p.GetComponent<VCR>());
            
            p1sLeft = roundNumber;
        }

        // ======================
        // Player 2
        // ======================

        if (GameState.playerJoined[1])
        {
            p = GameObject.Instantiate(GameState.playerTankPrefabs[1]);
            p.name = "tank p2."+(roundNumber);
            p.transform.position = p2Starts[roundNumber-1].transform.position;

            t = p.GetComponent<TankController>();
            if (GameState.players[1] != null)
            {
                if (GameState.players[1].cursor != null) GameState.players[1].cursor.gameObject.SetActive(false);
                GameState.players[1].tank = t;
                c = GameState.players[1].cursor = t.transform.Find("Player Cursor").GetComponent<CursorController>();
                c.Setup();
            }
            t.Setup();
            t.onDestroyedNotify = OnPlayerDestroyed;
            t.team = TankController.Team.PLAYER_2;

            tanks.Add(t);
            playerTanks[1].Add(t);
            recordingVCRs.Add(p.GetComponent<VCR>());

            p2sLeft = roundNumber;
        }


        // ======================
        // Player 3
        // ======================

        
        if (GameState.playerJoined[2])
        {
            p = GameObject.Instantiate(GameState.playerTankPrefabs[2]);
            p.name = "tank p3."+(roundNumber);
            p.transform.position = p3Starts[roundNumber-1].transform.position;

            
            t = p.GetComponent<TankController>();
            if (GameState.players[2] != null)
            {
                if (GameState.players[2].cursor != null) GameState.players[2].cursor.gameObject.SetActive(false);
                GameState.players[2].tank = t;
                c = GameState.players[2].cursor = t.transform.Find("Player Cursor").GetComponent<CursorController>();
                c.Setup();
            }
            t.Setup();
            t.onDestroyedNotify = OnPlayerDestroyed;
            t.team = TankController.Team.PLAYER_3;

            tanks.Add(t);
            playerTanks[2].Add(t);
            recordingVCRs.Add(p.GetComponent<VCR>());

            p3sLeft = roundNumber;
        }

        // ======================
        // Player 4
        // ======================

        if (GameState.playerJoined[3])
        {
            p = GameObject.Instantiate(GameState.playerTankPrefabs[3]);
            p.name = "tank p4."+(roundNumber);
            p.transform.position = p4Starts[roundNumber-1].transform.position;

            
            t = p.GetComponent<TankController>();
            if (GameState.players[3] != null)
            {
                if (GameState.players[3].cursor != null) GameState.players[3].cursor.gameObject.SetActive(false);
                GameState.players[3].tank = t;
                c = GameState.players[3].cursor = t.transform.Find("Player Cursor").GetComponent<CursorController>();
                c.Setup();
            }
            t.Setup();
            t.onDestroyedNotify = OnPlayerDestroyed;
            t.team = TankController.Team.PLAYER_4;

            tanks.Add(t);
            playerTanks[3].Add(t);
            recordingVCRs.Add(p.GetComponent<VCR>());
            
            p4sLeft = roundNumber;
        }
    }

    // public void P1Destroyed() { p1sLeft--; }
    // public void P2Destroyed() { p2sLeft--; }
    // public void P3Destroyed() { p3sLeft--; }
    // public void P4Destroyed() { p4sLeft--; }
    

    public void SetAppearances(int roundNumber)
    {
        foreach (VCR v in playbackVCRs)
        {
            TankController t = v.GetComponent<TankController>();
            t.SetDestroyed(false);
            TankAppearance a = v.GetComponent<TankAppearance>();
            a.SetHologram(roundNumber);
        }
    }

    public void HookupIcons(GameObject[] p1icons, GameObject[] p2icons, GameObject[] p3icons, GameObject[] p4icons)
    {
        GameObject[][] icons = new GameObject[][] {p1icons, p2icons, p3icons, p4icons};

        foreach(TankController tc in tanks)
        {
            Debug.Log("ICON LINKING FOR *"+tc.gameObject.name+"*");
            try
            {
                //string name = tc.gameObject.name;
                //if (tc.gameObject.name[0+5] != 'p') continue;
                
                //Debug.Log(tc.gameObject.name.Substring(1+5,2+5));

                int pNum = int.Parse(""+tc.gameObject.name[1+5]);//2+5));
                int rNum = int.Parse(tc.gameObject.name.Substring(3+5));

                Debug.Log("setting up icon " + pNum + " " + rNum);
                tc.vcr.onRogueNotify = () => { Debug.Log("set icon rogue " + pNum + " " + rNum); icons[pNum-1][rNum-1].GetComponent<PlayerIconController>().SetRogue(); };
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
            }
        }

        // for (int i = 0; i < 4; i++)
        // {
        //     GameObject[] thisIcons = icons[i];
        //     for (int j = 0; j < icons.Length; j++)
        //     {
        //         playerTanks[i][j].vcr.onRogueNotify = () => { icons[i][j].GetComponent<PlayerIconController>().SetRogue(); };
        //     }
        // }
    }

    // public int CheckForWinner()
    // {
    //     if (p1sLeft == 0 && p2sLeft == 0 && p3sLeft == 0) return 4;
    //     if (p1sLeft == 0 && p2sLeft == 0 && p4sLeft == 0) return 3;
    //     if (p1sLeft == 0 && p4sLeft == 0 && p3sLeft == 0) return 2;
    //     if (p4sLeft == 0 && p2sLeft == 0 && p3sLeft == 0) return 1;
    //     return -1;
    // }

    public void RecordingVCRs_to_PlaybackVCRs()
    {
        RecordingVCRs_to_PlaybackVCRs(0);
    }

    public void RecordingVCRs_to_PlaybackVCRs(int roundNumber)
    {
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

    public void ResetPlayers()
    {
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

    public void StartPlayers()
    {
        foreach (VCR vcr in recordingVCRs)
            vcr.SetState(VCR.VCRState.RECORDING);
        foreach (VCR vcr in playbackVCRs)
            vcr.SetState(VCR.VCRState.PLAYINGBACK);
            
        foreach (TankController tank in tanks)
            tank.frozen = false;
    }
}
