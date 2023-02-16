using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VsGameController : MonoBehaviour
{
    public enum VsGameState 
    {
        PLAYING,
        RESETING,
        GAME_ENDED,
        END_ROUND
    }

    public float roundLength_seconds;
    public float countdownWhenXSecondsLeftInRound = 10;

    public float roundCooldown_seconds;
    public float roundEndLength_seconds;
    public int numRounds;

    VsGameState state = VsGameState.RESETING; 
    public float stateChangeTimer = 0;

    private bool stateChangeRisingEdge;

    public int roundNumber;// {get; private set;}

    PlayersManager pm;
    LevelBuilder   levelBuilder;

    public bool[][] musicSettingsPerRound;
    MusicController music;


    public int[] playersLeft;
    private int playerCount;
    
    [Header("UI Elements")]
    public Canvas canvas;
    public GameObject roundOverText;

    public CountDown roundStartCountDown;
    public CountDown roundEndCountDown;
    public bool startedCountDown;

    public RectTransform p1Icon;
    public RectTransform p2Icon;
    public RectTransform p3Icon;
    public RectTransform p4Icon;
    
    private GameObject[] p1Icons;
    private GameObject[] p2Icons;
    private GameObject[] p3Icons;
    private GameObject[] p4Icons;

    public Color deadTankColor;
    
    public Text roundNumberText;

    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    void Setup() 
    {
        playerCount = 0;
        for (int i = 0; i < 4; i++)
        {
            if (GameState.playerJoined[i]) playerCount++;
        }

        bool f = false;
        bool t = true;

        bool[] round0 = new bool[]{f, f, f, f, f, t, f, t, f, f};
        bool[] round1 = new bool[]{f, f, t, f, f, t, f, t, f, f};
        bool[] round2 = new bool[]{f, t, f, f, t, t, f, t, f, f};
        bool[] round3 = new bool[]{f, t, f, t, t, t, f, t, f, f};
        bool[] round4 = new bool[]{f, t, f, t, t, f, t, t, t, f};
        musicSettingsPerRound = new bool[][] {round0, round1, round2, round3, round4};

        levelBuilder = GameObject.FindObjectsOfType<LevelBuilder>()[0];
        levelBuilder.BuildLevel();

        //vcrs = GameObject.FindObjectsOfType<VCR>();
        pm = GetComponent<PlayersManager>();
        pm.numSpawnPoints = numRounds;
        pm.Setup();

        music = GameObject.FindObjectsOfType<MusicController>()[0];

        roundNumber = 0;

        SetUpConfig();
        roundStartCountDown.CascadeSetTime(roundCooldown_seconds/3f);
        roundEndCountDown.CascadeSetTime(countdownWhenXSecondsLeftInRound/10f);
        
        numRounds = Mathf.Max(numRounds, pm.numSpawnPoints);

        SetUpNewState();
    }

    void SetUpConfig()
    {
        if (GameState.config == null || GameState.config.vsModeConfig == null)
        {
            Debug.LogError("Config was null!");
            return;
        }

        roundLength_seconds = GameState.config.vsModeConfig.roundLength_seconds;
        roundCooldown_seconds = GameState.config.vsModeConfig.roundCooldown_seconds;
        roundEndLength_seconds = GameState.config.vsModeConfig.roundEndLength_seconds;
        numRounds = GameState.config.vsModeConfig.maxNumRounds;
        countdownWhenXSecondsLeftInRound = GameState.config.vsModeConfig.countdownWhenXSecondsLeftInRound;
    }

    void FixedUpdate() 
    {
        StateChanges();
    }

    void StateChanges()
    {
        stateChangeTimer -= Time.deltaTime;

        bool stateChanged = TryStateChange();

        if (stateChanged) SetUpNewState();
        else StateUpdate();
    }

    private void StateUpdate()
    {
        if (state == VsGameState.PLAYING)
        {
            // start the time remaining countdown
            if (stateChangeTimer <= 10 && !startedCountDown)
            {
                startedCountDown = true;
                roundEndCountDown.StartCountdown();
            }
        }
    }

    private bool TryStateChange()
    {
        if (state == VsGameState.PLAYING)
        {
            // playing transitions to END_ROUND on one of two conditions:

            // playing transitions to END_ROUND after a timer
            if (stateChangeTimer <= 0)
            {
                state = VsGameState.END_ROUND;
                return true; 
            }

            // playing transitions to END_ROUND if only one team is left
            int numTeamsGT0 = 0;
            for (int i = 0; i < playersLeft.Length; i++)
            {
                if (playersLeft[i] > 0) numTeamsGT0++;
            }

            if (numTeamsGT0 > 1) return false;
            if (numTeamsGT0 == 1 && playerCount == 1) return false;

            

            if (GameState.gameplayType == GameState.GameplayType.KING_OF_THE_HILL) //return false; // IF THINGS ARE BROKEN, THIS LINE IS NEW 
            {
                // instead of just ending the round, add however much time is left on the timer to their score
                // this simulates the player just sitting on a capture point for the rest of the round.
                int i = 0;
                for (i = 0; i < playersLeft.Length; i++)
                {
                    if (playersLeft[i] > 0) break;
                }

                if (i < 4)
                {
                    GameState.kingOfTheHillScores[i] += stateChangeTimer;
                }
            }

            // change the state

            state = VsGameState.END_ROUND;
            return true;
        }

        if (state == VsGameState.END_ROUND)
        {
            // resetting transitions to RESETING after a timer
            if (stateChangeTimer > 0) return false;

            roundOverText.active = false;

            // change the state
            state = VsGameState.RESETING;
            return true;
        }

        if (state == VsGameState.RESETING)
        {
            // resetting transitions to PLAYING after a timer
            if (stateChangeTimer > 0) return false;

            // change the state
            state = VsGameState.PLAYING;
            return true;
        }

        return false;
    }

    private void SetUpTankIcons()
    {
        if (p1Icons != null)
            foreach(GameObject g in p1Icons)
                GameObject.Destroy(g);
        if (p2Icons != null)
            foreach(GameObject g in p2Icons)
                GameObject.Destroy(g);
        if (p3Icons != null)
            foreach(GameObject g in p3Icons)
                GameObject.Destroy(g);
        if (p4Icons != null)
            foreach(GameObject g in p4Icons)
                GameObject.Destroy(g);

        p1Icons = new GameObject[roundNumber];
        p2Icons = new GameObject[roundNumber];
        p3Icons = new GameObject[roundNumber];
        p4Icons = new GameObject[roundNumber];
        
        for(int i = 0; i < roundNumber; i++)
        {
            GameObject g;
            RectTransform rt;

            if (GameState.playerJoined[0])
            {
                g = GameObject.Instantiate(p1Icon.gameObject);
                rt = g.GetComponent<RectTransform>();
                rt.SetParent(canvas.transform);
                //rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, i*rt.rect.width);
                rt.anchoredPosition = p1Icon.anchoredPosition + new Vector2(i*rt.rect.width, 0);
                p1Icons[i] = g;

                if(i != roundNumber-1) g.GetComponent<PlayerIconController>().SetHologram();
            }


            if (GameState.playerJoined[1])
            {
                g = GameObject.Instantiate(p2Icon.gameObject);
                rt = g.GetComponent<RectTransform>();
                rt.SetParent(canvas.transform);
                //rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, i*rt.rect.width);
                rt.anchoredPosition = p2Icon.anchoredPosition + new Vector2(-i*rt.rect.width, 0);
                p2Icons[i] = g;

                if(i != roundNumber-1) g.GetComponent<PlayerIconController>().SetHologram();
            }

            
            if (GameState.playerJoined[2])
            {
                g = GameObject.Instantiate(p3Icon.gameObject);
                rt = g.GetComponent<RectTransform>();
                rt.SetParent(canvas.transform);
                //rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, i*rt.rect.width);
                rt.anchoredPosition = p3Icon.anchoredPosition + new Vector2(i*rt.rect.width, 0);
                p3Icons[i] = g;
                
                if(i != roundNumber-1) g.GetComponent<PlayerIconController>().SetHologram();
            }

            
            if (GameState.playerJoined[3])
            {
                g = GameObject.Instantiate(p4Icon.gameObject);
                rt = g.GetComponent<RectTransform>();
                rt.SetParent(canvas.transform);
                //rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, i*rt.rect.width);
                rt.anchoredPosition = p4Icon.anchoredPosition + new Vector2(-i*rt.rect.width, 0);
                p4Icons[i] = g;
                
                if(i != roundNumber-1) g.GetComponent<PlayerIconController>().SetHologram();
            }
        }
    }

    private void SetUpNewState()
    {

        if (state == VsGameState.RESETING)
        {
            roundNumber++;

            if (roundNumber > numRounds)
            {   
                // if we're resetting into a round past the last one, the game's over
                List<int> winners = new List<int>();
                if (playersLeft[0] > 0) winners.Add(1);
                if (playersLeft.Length >= 2 && playersLeft[1] > 0) winners.Add(2);
                if (playersLeft.Length >= 3 && playersLeft[2] > 0) winners.Add(3);
                if (playersLeft.Length >= 4 && playersLeft[3] > 0) winners.Add(4);
                
                if (winners.Count == 0)
                {
                    winners.Add(1);
                    if (playersLeft.Length >= 2) winners.Add(2);
                    if (playersLeft.Length >= 3) winners.Add(3);
                    if (playersLeft.Length >= 4) winners.Add(4);
                }

                Debug.Log("Changing scene!");
                GameState.winners = winners.ToArray();
                GameState.menuState = winners.Count == 1? GameState.MenuState.WIN : GameState.MenuState.TIE;
                
                if (GameState.gameplayType == GameState.GameplayType.KING_OF_THE_HILL)
                {
                    GameState.menuState = GameState.MenuState.WIN;
                    float maxTime = 0;
                    winners.Clear();
                    for(int i = 0; i < 4; i++)
                    {
                        if (GameState.kingOfTheHillScores[i] > maxTime)
                        {
                            maxTime = GameState.kingOfTheHillScores[i];
                            winners.Clear();
                        }

                        if (GameState.kingOfTheHillScores[i] == maxTime)
                        {
                            winners.Add(i);
                        }
                    }

                    GameState.winners = winners.ToArray();
                }

                Debug.Log(GameState.menuState);
                SceneManager.LoadScene("Menus");
                Debug.Log("Changed scene!");
            }

            roundNumberText.text = roundNumber + "";

            // set up the timer, how long this state lasts
            stateChangeTimer = roundCooldown_seconds;

            // give the player a countdown
            roundStartCountDown.StartCountdown();

            // clear all projectiles
            DestroyShellsAndBombs();

            // clear tank tracks
            if (GameState.config.miscConfig.tracksGetClearedBetweenRounds)
            {
                TankController.ClearTracks();
            }
            

            // sometimes tanks still look destroyed in this mode
            foreach (TankController t in GameObject.FindObjectsOfType<TankController>())
                t.SetDestroyed(false);

            // set all VCRs that were recording to be VCRs that will play back (ie, make holograms for next round)
            pm.RecordingVCRs_to_PlaybackVCRs();

            // reset the level
            levelBuilder.ResetEnemiesAndGimmicks();

            // foreach (TankController t in GameObject.FindObjectsOfType<TankController>())
            //     if (t.team == TankController.Team.ENEMY)
            //         t.frozen = true;
            
            
            // put recordings back at their start, and spawn new player tanks
            pm.SetUpPlayers(roundNumber, PlayerWasDestroyed);
            
            // fix the tanks' appearances (eg some that should be holograms are rogue, some are destroyed, etc)
            pm.SetAppearances(roundNumber);

            // play the new music tracks
            music.playClip = new List<bool>(roundNumber > musicSettingsPerRound.Length? musicSettingsPerRound[musicSettingsPerRound.Length-1] : musicSettingsPerRound[roundNumber-1]);

            // tell VCRs to get ready for next round, and freeze tanks 
            pm.ResetPlayers();

            // set up the icons that show which tanks are still alive
            SetUpTankIcons();
            pm.HookupIcons(p1Icons, p2Icons, p3Icons, p4Icons);

            // reset our count of how many players are alive on each team
            playersLeft = new int[4];
            for (int i = 0; i < 4; i++)
            {
                if (!GameState.playerJoined[i]) continue;
                playersLeft[i] = roundNumber;
            }


            // reset any king of the hill points
            foreach (KingOfTheHillPoint kothpoint in GameObject.FindObjectsOfType<KingOfTheHillPoint>())
            {
                kothpoint.Reset();
            }
        }


        if (state == VsGameState.END_ROUND)
        {
            // FREEZE EVERYTHING, or set time.deltaTime = 0
            stateChangeTimer = roundEndLength_seconds;

            roundEndCountDown.EndCountdown();
            roundOverText.active = true;

            foreach (TankController tank in GameObject.FindObjectsOfType<TankController>())
            {
                tank.frozen = true;
            }

            foreach (BulletController bullet in GameObject.FindObjectsOfType<BulletController>())
            {
                //bullet.speed = 0;
                bullet.GetComponent<Rigidbody>().velocity = new Vector3();
                bullet.enabled = false;
            }

            foreach (BombController bomb in GameObject.FindObjectsOfType<BombController>())
            {
                //bomb.speed = 0;
                Rigidbody rb = bomb.GetComponent<Rigidbody>();
                rb.velocity = new Vector3();
                rb.useGravity = false;
                bomb.enabled = false;
            }

            foreach (ExplosionController explosion in GameObject.FindObjectsOfType<ExplosionController>())
            {
                explosion.enabled = false;
                explosion.GetComponent<Collider>().enabled = false;
            }

            
        }

        if (state == VsGameState.PLAYING)
        {
            startedCountDown = false;

            // set up the timer, how long this state lasts
            stateChangeTimer = roundLength_seconds;

            // this is pretty easy
            pm.StartPlayers();

            // foreach (TankController t in GameObject.FindObjectsOfType<TankController>())
            //     if (t.team == TankController.Team.ENEMY)
            //         t.frozen = false;
        }
    }

    public void PlayerWasDestroyed(TankController t)
    {
        switch (t.team)
        {
            case TankController.Team.PLAYER_1: this.playersLeft[0]--; break;
            case TankController.Team.PLAYER_2: this.playersLeft[1]--; break;
            case TankController.Team.PLAYER_3: this.playersLeft[2]--; break;
            case TankController.Team.PLAYER_4: this.playersLeft[3]--; break;
        }

        string id = t.gameObject.name.Split(' ')[1]; // eg: p1.1
        int tankNum = int.Parse(id.Split('.')[1]);

        switch (t.team)
        {
            case TankController.Team.PLAYER_1: p1Icons[tankNum-1].GetComponent<Image>().color = deadTankColor; break;
            case TankController.Team.PLAYER_2: p2Icons[tankNum-1].GetComponent<Image>().color = deadTankColor; break;
            case TankController.Team.PLAYER_3: p3Icons[tankNum-1].GetComponent<Image>().color = deadTankColor; break;
            case TankController.Team.PLAYER_4: p4Icons[tankNum-1].GetComponent<Image>().color = deadTankColor; break;
        }
    }
    
    void DestroyShellsAndBombs()
    {
        BulletController[] shells = GameObject.FindObjectsOfType<BulletController>();
        foreach (BulletController g in shells)
            GameObject.Destroy(g.gameObject);

        BombController[] bombs = GameObject.FindObjectsOfType<BombController>();
        foreach (BombController g in bombs)
            GameObject.Destroy(g.gameObject);

        ExplosionController[] explosions = GameObject.FindObjectsOfType<ExplosionController>();
        foreach (ExplosionController g in explosions)
            GameObject.Destroy(g.gameObject);
    }
}
