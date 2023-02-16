using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class ButtonResponder : MonoBehaviour
{
    public enum MenuName { TitleScreen, LevelsMenu, VsMenu, SettingsMenu }
    
    public MusicController music;
    
    public GameObject mainMenu;
    public GameObject titleScreen;
    public GameObject levelsMenu;
    public GameObject vsMenu;
    public GameObject settingsMenu;
    public GameObject playerSetupMenu;

    public GameObject winScreen;
    public GameObject gameOverScreen;
    public GameObject tieScreen;


    public TextAsset temp_testLevel;
    public ThemeAssetPack temp_theme;
    public string temp_levelsPath;

    public PlayerSelectorManager psm;
    public PlayerEventHandler peh;

    void SavePlayers()
    {
        if (GameState.menuState != GameState.MenuState.NORMAL) return;

        Player[] players = GameObject.FindObjectsOfType<Player>();
        Array.Reverse(players);
        //GameState.players = players; // comment this out
        //GameState.numPlayers = players.Length; // get rid of this field in GameState

        //HI PLEASE READ THESE NOTES HERE :)

        // note: also when spawning players, use GameState.playerPrefabs[i] instead of local fields

        // replace all checks of GameState.numPlayers with checks of GameState.playerJoined[i]
        GameState.players = peh.GetPlayersJoined();
        GameState.playerJoined = peh.playerHasJoined;
        GameState.playerTankPrefabs = psm.GetSelected();
        GameState.playerIsComputer = psm.GetPlayersAreComputers();

        Debug.Log(string.Join<GameObject>(", ", GameState.playerTankPrefabs));

        foreach (Player p in players)
            DontDestroyOnLoad(p.gameObject);
    }

    public void StartLevelEditor()
    {
        SavePlayers();

        SceneManager.LoadScene("LevelEditor", LoadSceneMode.Single);
    }

    public void StartKotH()
    {
        SavePlayers();

        GameState.gameplayType = GameState.GameplayType.KING_OF_THE_HILL;

        GameState.kingOfTheHillScores = new float[4];

        SceneManager.LoadScene("BaseGameplayScene", LoadSceneMode.Single);
        SceneManager.LoadScene("1v1ModeControl", LoadSceneMode.Additive);
    }

    public void Start1v1()
    {
        //TextAsset[] stupidCSharpArrayInitialization = {temp_testLevel, null};
        //GameState.levelToLoad = stupidCSharpArrayInitialization;
        
        //GameState.levelTheme = temp_theme;
        //GameState.numPlayers = 2;

        SavePlayers();

        GameState.gameplayType = GameState.GameplayType.VS;

        SceneManager.LoadScene("BaseGameplayScene", LoadSceneMode.Single);
        SceneManager.LoadScene("1v1ModeControl", LoadSceneMode.Additive);
    }

    public void StartLevels()
    {
        //TextAsset[] stupidCSharpArrayInitialization = {temp_testLevel, null};
        //GameState.levelToLoad = stupidCSharpArrayInitialization;
        //GameState.levelTheme = temp_theme;
        //GameState.levelsPath = temp_levelsPath;
        
        SavePlayers();

        GameState.gameplayType = GameState.GameplayType.LEVELS;

        SceneManager.LoadScene("BaseGameplayScene", LoadSceneMode.Single);
        SceneManager.LoadScene("LevelsModeControl", LoadSceneMode.Additive);
    }

    public void StartLevelsFirstTime()
    {
        FileLoader l = GameObject.FindObjectOfType<FileLoader>();
        string worldPath = l.levelsMode_Worlds[GameState.config.levelsModeConfig.cheat_worldToStartOn-1];

        GameState.levelsToBuild = l.LoadLevelsFromWorld(worldPath);
        GameState.levelTheme = l.GetThemeAssetPackForWorld(worldPath);
        GameState.worldName = "name not yet implemented";

        GameState.currentWorld = GameState.config.levelsModeConfig.cheat_worldToStartOn-1;
        GameState.currentLevel = GameState.config.levelsModeConfig.cheat_levelToStartOn-1;

        StartLevels();
    }

    GameState.GameplayType gameplayType;

    public void OpenMenu(string menuName)
    {
        Debug.Log("opening " + menuName);

        mainMenu.SetActive(false);
        titleScreen.SetActive(false);
        levelsMenu.SetActive(false);
        vsMenu.SetActive(false);
        settingsMenu.SetActive(false);
        playerSetupMenu.SetActive(false);

        winScreen.SetActive(false);
        gameOverScreen.SetActive(false);
        tieScreen.SetActive(false);

        switch(menuName)
        {
            case "titleScreen":
                mainMenu.SetActive(true);
                titleScreen.SetActive(true);
                bool[] arr1 = {false, false, false, false, false, true, false, false, false, false};
                music.playClip = new List<bool>(arr1);
                break;
            case "settingsMenu":
                mainMenu.SetActive(true);
                settingsMenu.SetActive(true);
                bool[] arr2 = {false, false, false, false, false, true, false, true, false, false};
                music.playClip = new List<bool>(arr2);
                break;
            case "levelsMenu":
                mainMenu.SetActive(true);
                levelsMenu.SetActive(true);
                bool[] arr3 = {false, true, false, false, false, true, false, false, false, true};
                music.playClip = new List<bool>(arr3);
                break;
            case "1v1Menu":
                mainMenu.SetActive(true);
                vsMenu.SetActive(true);
                bool[] arr4 = {false, true, false, true, false, true, false, false, false, false};
                music.playClip = new List<bool>(arr4);
                break;
            case "setUpGame":
                if (gameplayType == GameState.GameplayType.MENUS)  { OpenMenu("titleScreen"); return; }
                if (gameplayType == GameState.GameplayType.VS)     { OpenMenu("1v1Menu");     return; }
                if (gameplayType == GameState.GameplayType.LEVELS) { OpenMenu("levelsMenu");  return; }
                break;
            case "playersMenu_1v1":
                gameplayType = GameState.GameplayType.VS;
                playerSetupMenu.SetActive(true);
                bool[] arr5 = {false, true, false, true, false, true, false, false, false, false};
                music.playClip = new List<bool>(arr5);
                break;
            case "playersMenu_levels":
                gameplayType = GameState.GameplayType.LEVELS;
                playerSetupMenu.SetActive(true);
                bool[] arr6 = {false, true, false, false, false, true, false, false, false, true};
                music.playClip = new List<bool>(arr6);
                break;
        }
    }
}
