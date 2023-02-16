using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuStartup : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject gameOverScreen;
    public GameObject winScreen;
    public GameObject tieScreen;

    public FileLoader fileLoader;
    public ButtonResponder buttonResponder;

    private void Awake() 
    {
        mainMenu.active = false;
        gameOverScreen.active = false;
        winScreen.active = false;
        
        switch(GameState.menuState)
        {
            case GameState.MenuState.WIN:       winScreen.active = true;      break;
            case GameState.MenuState.GAME_OVER: gameOverScreen.active = true; break;
            case GameState.MenuState.TIE:       tieScreen.active = true;      break;
            default:                            mainMenu.active = true;       break;
        }    

        if (GameState.gameplayType == GameState.GameplayType.LEVELS && GameState.menuState == GameState.MenuState.WIN)
        {
            LoadNextWorld();
        }

        GameState.menuState = GameState.MenuState.NORMAL;
    }

    void LoadNextWorld()
    {

        //GameState.menuState = GameState.MenuState.NORMAL;
        GameState.currentWorld++;
        GameState.currentLevel = 0;

        fileLoader.Setup();

        Debug.Log("auto loadign!");
        Debug.Log("num worlds: " + fileLoader.levelsMode_Worlds.Length);
        Debug.Log("next world, that's currently trying to load: " + GameState.currentWorld);

        if (GameState.currentWorld < fileLoader.levelsMode_Worlds.Length)
        {
            string worldPath = fileLoader.levelsMode_Worlds[GameState.currentWorld];

            string[] split = worldPath.Split('/');

            string worldName = split[split.Length-1];

            Debug.Log("Autoloading "  + worldPath);

            GameState.levelsToBuild = fileLoader.LoadLevelsFromWorld(worldPath);
            GameState.levelTheme = fileLoader.GetThemeAssetPackForWorld(worldPath);
            GameState.worldName = worldName;

            buttonResponder.StartLevels();
        }
    }
}
