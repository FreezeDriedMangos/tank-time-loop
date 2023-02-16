using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelsGameController : MonoBehaviour
{
    public enum LevelsGameState 
    {
        PRE_LEVEL,
        LEVEL_PREVIEW,
        PLAYING,
        LEVEL_OVER,
        POTENTIAL_GAME_OVER
    }

    LevelsGameState state = LevelsGameState.PRE_LEVEL; 
    public float stateChangeTimer = 0;

    public float preLevelTime_seconds;
    public float bannerExitAnimationLength_seconds;
        public bool haveExitedBanner;
    public float levelPreviewTime_seconds;
        public bool gameOver = false;
    public float gameOverGraceTime_seconds;
        public bool allPlayersOutOfLives = false;
    public float levelEndTime_seconds;
    public float respawnFreezeTime_seconds;
    public float respawnInvincibilityTime_seconds;

    public int maxNumLives;


    List<PlayerManager> pms;
    LevelBuilder   levelBuilder;
    MusicController music;

    public List<int> playerLivesUsed;
    public List<int> playerTotalLives;
    public int enemiesLeft;
    public int totalNumEnemies;
    
    
    [Header("UI Elements")]
    public Canvas canvas;
    public GameObject victoryText;
    public Text stateText;

    public CountDown levelStartCountDown;
    public CountDown gameOverCountdown;
    
    public LevelTitleSlide titleCard;

    public Text totalNumEnemeiesText;
    public Text numEnemeiesLeftText;

    public Text worldNumText;
    public Text levelNumText;
    
    [Header("Player Icons")]

    public PlayerIconController p1Icon;
    public PlayerIconController p2Icon;
    public PlayerIconController p3Icon;
    public PlayerIconController p4Icon;
    
    private PlayerIconController[] p1Icons;
    private PlayerIconController[] p2Icons;
    private PlayerIconController[] p3Icons;
    private PlayerIconController[] p4Icons;
    private PlayerIconController[][] allIcons;
    
    

    private bool setup = false;

    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    void Setup() 
    {
        if (setup) return;
        setup = true;
        
        levelBuilder = FindObjectOfType<LevelBuilder>();
        pms = new List<PlayerManager>(GetComponents<PlayerManager>());
        music = FindObjectOfType<MusicController>();

        worldNumText.text = (GameState.currentWorld+1)+"";

        SetUpConfig();

        SetUpNewState();

        SetUpIcons();
    }

    void SetUpConfig()
    {
        if (GameState.config == null || GameState.config.levelsModeConfig == null) return;

        preLevelTime_seconds = GameState.config.levelsModeConfig.preLevelTime_seconds;
        bannerExitAnimationLength_seconds = GameState.config.levelsModeConfig.bannerExitAnimationLength_seconds;
        levelPreviewTime_seconds = GameState.config.levelsModeConfig.levelPreviewTime_seconds;
        levelEndTime_seconds = GameState.config.levelsModeConfig.levelEndFreezeFrameTime_seconds;
        gameOverGraceTime_seconds = GameState.config.levelsModeConfig.gameOverGraceTime_seconds;
        maxNumLives = GameState.config.levelsModeConfig.maxNumLives;

        respawnFreezeTime_seconds = GameState.config.levelsModeConfig.respawnFreezeTime_seconds;

        respawnInvincibilityTime_seconds = GameState.config.levelsModeConfig.respawnInvincibilityTime_seconds;        

        levelStartCountDown.CascadeSetTime(levelPreviewTime_seconds/3f);
        gameOverCountdown.CascadeSetTime(gameOverGraceTime_seconds/10f);

        titleCard.totalTime_seconds = preLevelTime_seconds;
        titleCard.exitAnimationLength_seconds = bannerExitAnimationLength_seconds;
        
    }

    void SetUpIcons()
    {
        if (p1Icons != null)
            foreach(PlayerIconController g in p1Icons)
                GameObject.Destroy(g.gameObject);
        if (p2Icons != null)
            foreach(PlayerIconController g in p2Icons)
                GameObject.Destroy(g.gameObject);
        if (p3Icons != null)
            foreach(PlayerIconController g in p3Icons)
                GameObject.Destroy(g.gameObject);
        if (p4Icons != null)
            foreach(PlayerIconController g in p4Icons)
                GameObject.Destroy(g.gameObject);

        p1Icons = new PlayerIconController[playerTotalLives[0]];
        p2Icons = new PlayerIconController[playerTotalLives[1]];
        p3Icons = new PlayerIconController[playerTotalLives[2]];
        p4Icons = new PlayerIconController[playerTotalLives[3]];
        

        PlayerIconController[][] iconLists = new PlayerIconController[4][] {p1Icons, p2Icons, p3Icons, p4Icons};
        PlayerIconController[] iconPrefabs = new PlayerIconController[4] {p1Icon, p2Icon, p3Icon, p4Icon};

        for (int j = 0; j < 4; j++)
        {
            for(int i = 0; i < playerTotalLives[j]; i++)
            {
                GameObject g;
                RectTransform rt;

                g = GameObject.Instantiate(iconPrefabs[j].gameObject);
                rt = g.GetComponent<RectTransform>();
                Vector2 ap = rt.anchoredPosition;
                rt.SetParent(canvas.transform);
                //rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, i*rt.rect.width);
                Debug.Log("player num " + j + " icon anchored pos " + rt.position);
                rt.anchoredPosition = ap + new Vector2( (j%2==1? -1 : 1) * i*rt.rect.width, 0);

                iconLists[j][i] = g.GetComponent<PlayerIconController>();
                iconLists[j][i].SetDisabled();
            }
        }

        allIcons = iconLists;

        if (p1Icons.Length > 0) p1Icons[0].SetActive();
        if (p2Icons.Length > 0) p2Icons[0].SetActive();
        if (p3Icons.Length > 0) p3Icons[0].SetActive();
        if (p4Icons.Length > 0) p4Icons[0].SetActive();
    }

    void FixedUpdate() 
    {
        StateChanges();
    }

    void StateChanges()
    {
        if (stateChangeTimer > 0) stateChangeTimer -= Time.deltaTime;

        bool stateChanged = TryStateChange();

        if (stateChanged) SetUpNewState();
        else StateUpdate();
    }

    private void SetUpNewState()
    {
        if (stateText!= null) stateText.text = state.ToString();

        if (state == LevelsGameState.PRE_LEVEL)
        {
            stateChangeTimer = preLevelTime_seconds;

            //
            // clean up from previous states
            //

            music.RestartMusic();

            victoryText.SetActive(false);
            allPlayersOutOfLives = false;

            TankController.ClearTracks();

            // reset music
            // IMPLEMENT ME

            //
            // set up this state
            //

            if (GameState.currentLevel >= GameState.levelsToBuild.Length)
            {
                Debug.Log("hiiiiiiiiiiiiiiiiiiiiiiooooooooooooooooo");
                GameState.menuState = GameState.MenuState.WIN;
                ReturnToMainMenu();
                return;
            }
            
            // destroy remaining projectiles
            DestroyShellsAndBombs();

            // destroy players and prepare pms for new level
            foreach (PlayerManager pm in pms)
            {
                pm.DestroyPlayerTanks();
                pm.UnSetup();
            }

            // PlayerManager kept finding spawnpoints that were part of the level that had been destroyed, so this function basically erases their names so they can't be found. ... that's kinda messed up now that I've said it
            ObliterateSpawnPoints();

            // build level
            levelBuilder.BuildLevel();


            // increment level counter
            GameState.currentLevel++;
            levelNumText.text = GameState.currentLevel+"";

            // show banner
            titleCard.SetDisplay();

            // freeze enemies
            foreach (TankController t in GameObject.FindObjectsOfType<TankController>())
            {
                if (t.team == TankController.Team.ENEMY)
                {
                    t.Setup(); // setup will unfreeze a tank, and the tank controller will call setup on its own unless I call it here
                    t.frozenExceptForAiming = true;
                }
            }

            // set up players and pms
            playerTotalLives = new List<int>();
            playerLivesUsed = new List<int>();
            
            for (int i = 0; i < pms.Count; i++)
            {
                PlayerManager pm = pms[i];
                // NOTE: IN THE PLAYER WAS DESTROYED FUNCTION, CHECK IF ALL PLAYERS ARE OUT OF LIVES, AND IF SO
                // SET BOOLEAN allPlayersOutOfLives 
                pm.Setup(levelBuilder.spawnPoints[i]);
                pm.SetUpPlayer(1, PlayerWasDestroyed);
                pm.ResetPlayer();

                playerLivesUsed.Add(0);
                playerTotalLives.Add(Mathf.Min(pm.numSpawnPoints, maxNumLives));
            }

            SetUpIcons();

            // SETUP ICONS WITH PMSes
            // foreach (PlayerManager pm in pms)
            // {
            //     pm.HookupIcons(allIcons);
            // }


            // connect enemy deaths to the counter
            // note: for a similar reason to above with ObliterateSpawnPoints, the below for loop includes current and past level enemies
            foreach (TankController tc in GameObject.FindObjectsOfType<TankController>())
            {
                if (tc.team != TankController.Team.ENEMY) continue;

                tc.onDestroyedNotify = EnemyTankWasDestroyed;
            }
            totalNumEnemies = enemiesLeft = levelBuilder.GetNumEnemies();
            totalNumEnemeiesText.text = numEnemeiesLeftText.text = totalNumEnemies+"";
        }
        if (state == LevelsGameState.LEVEL_PREVIEW)
        {
            stateChangeTimer = levelPreviewTime_seconds;

            levelStartCountDown.StartCountdown();
        }
        if (state == LevelsGameState.PLAYING)
        {
            // start players and enemy tanks

            foreach (PlayerManager pm in pms)
            {
                pm.StartPlayer();
            }

            foreach (TankController t in GameObject.FindObjectsOfType<TankController>())
            {
                if (t.team == TankController.Team.ENEMY && !t.destroyed)
                    t.frozen = false;
            }
        }
        if (state == LevelsGameState.LEVEL_OVER)
        {
            victoryText.SetActive(true);
            gameOverCountdown.EndCountdown();

            FreezeEverything();

            stateChangeTimer = levelEndTime_seconds;
        }
        if (state == LevelsGameState.POTENTIAL_GAME_OVER)
        {
            stateChangeTimer = gameOverGraceTime_seconds;

            gameOverCountdown.StartCountdown();
        }
    }

    private void StateUpdate()
    {
        if (state == LevelsGameState.PRE_LEVEL)
        {

        }
        if (state == LevelsGameState.LEVEL_PREVIEW)
        {

        }
        if (state == LevelsGameState.PLAYING)
        {

        }
        if (state == LevelsGameState.LEVEL_OVER)
        {

        }
        if (state == LevelsGameState.POTENTIAL_GAME_OVER)
        {

        }
    }

    private bool TryStateChange()
    {
        if (state == LevelsGameState.PRE_LEVEL)
        {
            if (stateChangeTimer > 0) return false;

            state = LevelsGameState.LEVEL_PREVIEW;
            return true;
        }
        if (state == LevelsGameState.LEVEL_PREVIEW)
        {
            if (stateChangeTimer > 0) return false;

            state = LevelsGameState.PLAYING;
            return true;
        }
        if (state == LevelsGameState.PLAYING)
        {
            // if all enemies are defeated, next level
            if (enemiesLeft <= 0)
            {
                state = LevelsGameState.LEVEL_OVER;
                gameOver = false;
                return true;
            }

            // if all players have lost all lives, got to POTENTIAL_GAME_OVER
            if (allPlayersOutOfLives)
            {
                state = LevelsGameState.POTENTIAL_GAME_OVER;
                return true;
            }

            return false;
        }
        if (state == LevelsGameState.LEVEL_OVER)
        {
            if (stateChangeTimer > 0) return false;

            if (gameOver) GameOver();

            // advance to the next level
            state = LevelsGameState.PRE_LEVEL;
            return true;
        }
        if (state == LevelsGameState.POTENTIAL_GAME_OVER)
        {
            // if we're in this state and all enemies die, the player still wins, and advances to next level
            if (enemiesLeft <= 0)
            {
                state = LevelsGameState.LEVEL_OVER;
                return true;
            }

            // don't give the player a game over until the timer's up
            if (stateChangeTimer > 0) return false;

            GameOver();
            return true;
        }

        return false;
    }

    private void ObliterateSpawnPoints()
    {
        for (int PlayerNum = 1; PlayerNum <= 4; PlayerNum++)
        {
            int numSpawnPoints = 1;
            for (int i = 0; i < numSpawnPoints; i++)
            {
                GameObject g = GameObject.Find("p"+PlayerNum+"."+(i+1));
                Debug.Log("spawnpoint " + i + ": " + g);

                if (g == null) break;

                numSpawnPoints++;
                g.name = "gone forever";
            }
        }
    }

    private void GameOver()
    {
        GameState.menuState = GameState.MenuState.GAME_OVER;
        ReturnToMainMenu();
    }

    private void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Menus");
    }

    public void PlayerWasDestroyed(TankController t)
    {
        // if this is being called because the gameobject itself was destroyed, then forget it
        if (t == null || t.gameObject == null) return;

        // we don't care about hologram tanks right now
        // soon tho we'll want to update the icons
        // IMPLEMENT MEEE
        // if (t.IsHologram) return;

        string id = t.gameObject.name.Split(' ')[1]; // eg: p1.1
        int tankNum = int.Parse(id.Split('.')[1]);
        int teamNum = -1;

        switch (t.team)
        {
            case TankController.Team.PLAYER_1: teamNum = 0; break;
            case TankController.Team.PLAYER_2: teamNum = 1; break;
            case TankController.Team.PLAYER_3: teamNum = 2; break;
            case TankController.Team.PLAYER_4: teamNum = 3; break;
        }

        if (teamNum == -1)
        {
            Debug.LogError("PlayerWasDestroyed found an invalid team number / a tank with an invalid team");
            return;
        }


        allIcons[teamNum][tankNum-1].SetDead();

        if (t.IsHologram) return; // all we wanted to do is update the icons


        playerLivesUsed[teamNum]++;

        if (playerLivesUsed[teamNum] >= playerTotalLives[teamNum])
        {
            // sucks, you're dead

            allPlayersOutOfLives = true;
            for (int i = 0; i < playerLivesUsed.Count; i++)
            {
                allPlayersOutOfLives = allPlayersOutOfLives && (playerLivesUsed[i] >= playerTotalLives[i]);
            }
        }
        else
        {
            pms[teamNum].UnDestroy();

            pms[teamNum].RecordingVCRs_to_PlaybackVCRs();

            pms[teamNum].SetUpPlayer(playerLivesUsed[teamNum]+1, PlayerWasDestroyed);
            pms[teamNum].SetAppearance(playerLivesUsed[teamNum]+1);
            pms[teamNum].ResetPlayer();
            pms[teamNum].SetInvincible(true);

            // short timer

            pms[teamNum].StartPlayerAfterTimer(respawnFreezeTime_seconds);
            pms[teamNum].SetInvincibleAfterTimer(false, respawnInvincibilityTime_seconds);
            

            // set up icons

            for (int i = 0; i < playerLivesUsed[teamNum]; i++)
            {
                allIcons[teamNum][i].SetHologram();
            }
            allIcons[teamNum][playerLivesUsed[teamNum]].SetActive();

            //pms[teamNum].HookupIcons(allIcons);
        }
    }

    public void EnemyTankWasDestroyed(TankController tc)
    {
        enemiesLeft--;
        numEnemeiesLeftText.text = enemiesLeft+"";
    }

    void FreezeEverything()
    {
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

