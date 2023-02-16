using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json.Linq;

public static class GameState
{

    //
    // Misc
    //

    public enum GameplayType {MENUS, LEVELS, VS, KING_OF_THE_HILL};
    public static GameplayType gameplayType = GameplayType.MENUS;


    //
    // Info for the "Menus" scene
    //

    public enum MenuState {NORMAL, GAME_OVER, WIN, TIE};
    public static MenuState menuState = MenuState.NORMAL;

    public static int[] winners;

    //
    // Level building
    //

    public static string[][] levelsToBuild;
    public static int currentLevel;
    public static int currentWorld;

    public static string levelName = null;
    public static string worldName = null;

    public static ThemeAssetPack levelTheme;

    // note: for arenas, levelsToBuild will have a single element and currentLevel will never increase


    //
    // Stuff for gameplay scripts
    //

    //public static int numPlayers = 1;

    // loadedLevel[floorNum, x, z] -> the contents of the level at tile (x, z) on floor number floorNum (starting at 0)
    public static string[,,] loadedLevel;

    public static Player[] players;

    public static int LevelHeight { get { if (loadedLevel == null) return -1; return loadedLevel.GetLength(2); } private set {} }
    public static int LevelWidth  { get { if (loadedLevel == null) return -1; return loadedLevel.GetLength(1); } private set {} }
    public static int LevelNumFloors  { get { if (loadedLevel == null) return -1; return loadedLevel.GetLength(0); } private set {} }
    
    public static GameObject[] playerTankPrefabs;
    public static bool[] playerIsComputer;
    public static bool[] playerJoined;

    public static float[] kingOfTheHillScores;

    //
    // Config
    //

    public static Config config;
    //public static JObject config;

    public static class ConfigUtility
    {
        public static TankConfig GetConfigForTankType(TankController.Type type)
        {
            if (config.tankConfigs == null) return config.defaultTankConfig;


            for (int i = 0; i < config.tankConfigs.Length; i++)
            {
                if (config.tankConfigs[i].type == type.ToString())
                    return config.tankConfigs[i];
            }

            return config.defaultTankConfig;
        }

        public static SFXConfig GetConfigForSFX(string sfx)
        {
            foreach (SFXConfig sfxc in config.sfxConfigs)
            {
                if (sfxc.name == sfx) 
                {
                    return sfxc;
                }
            }
            
            SFXConfig d = new SFXConfig();
            d.volume = 1;
            d.pitchMin = 1;
            d.pitchMax = 1;
            
            return d;
        }
    }
}