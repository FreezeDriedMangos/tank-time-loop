using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

using UnityEngine;

// https://stackoverflow.com/a/36021470/9643841
public class FileLoader : MonoBehaviour
{
    static string THEME_FILE_NAME = "theme.txt";

    public string LevelsModeRootPath;
    public string VsModeRootPath;
    public string ConfigPath;
    
    public string[] levelsMode_Worlds;
    public string[] vsMode_arenas;

    public bool setup {get; private set;}

    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }


    public void Setup()
    {
        if (setup) return;
        setup = true;

        DontDestroyOnLoad(this.gameObject);

        if(LevelsModeRootPath.StartsWith("Assets"))
            LevelsModeRootPath = LevelsModeRootPath.Substring(6);
        if(VsModeRootPath.StartsWith("Assets"))
            VsModeRootPath = VsModeRootPath.Substring(6);
        if(ConfigPath.StartsWith("Assets"))
            ConfigPath = ConfigPath.Substring(6);
        

        DirectoryInfo levelDirectoryPath = new DirectoryInfo(Application.dataPath+LevelsModeRootPath);
        //Debug.Log(Application.dataPath+LevelsModeRootPath);

        //FileInfo[] files = levelDirectoryPath.GetFiles("*.*", SearchOption.TopDirectoryOnly); //SearchOption.AllDirectories

        // foreach (FileInfo file in files) {
        //     Debug.Log(file.Name);
        //     Debug.Log(file.Extension);
        // }

        levelsMode_Worlds = Directory.GetDirectories(Application.dataPath+LevelsModeRootPath);
        vsMode_arenas     = Directory.GetDirectories(Application.dataPath+VsModeRootPath);

        // get all folders in LevelsModeRootPath
            // pass folder path to LevelsModeController once a theme is selected
        // get all folders in VsModeRootPath


        // read in config data
        
        string configString = File.ReadAllText(Application.dataPath+ConfigPath);
        Debug.Log("Config Data read: " + configString);


        // old stuff
        // var converter = new StringEnumConverter();
        // Config data = JsonConvert.DeserializeObject<Config>(configString, converter);
        // GameState.config = data;



        // Config template = new Config();
        // template.defaultTankConfig = new TankConfig();
        // template.defaultTankConfig.type = TankController.Type.GENERIC;
        // template.tankConfigs = new TankConfig[6];
        // for(int i = 0; i < 6; i++)
        //     template.tankConfigs[i] = new TankConfig();
        // template.tankConfigs[0].type = TankController.Type.PLAYER;
        // template.tankConfigs[1].type = TankController.Type.STILL_ENEMY;
        // template.tankConfigs[2].type = TankController.Type.STANDARD_ENEMY; 
        // template.tankConfigs[3].type = TankController.Type.ROCKET_ENEMY; 
        // template.tankConfigs[4].type = TankController.Type.BOMB_ENEMY; 
        // template.tankConfigs[5].type = TankController.Type.FAST_ENEMY;
        

        // // Debug.Log(GameState.config);
        // Debug.Log(JsonConvert.SerializeObject(template, converter));
        // Debug.Log(JsonConvert.SerializeObject(GameState.config, converter));



        //JObject jo = JObject.Parse(configString);
        //GameState.config = new Config(jo);

        GameState.config = JsonUtility.FromJson<Config>(configString);
        Debug.Log("samples of read config data: \nroguepushdistance: " + GameState.config.miscConfig.rogueOnPushDistance + " default tank ai search give up timer: " + GameState.config.defaultTankConfig.aiConfig.giveUpSearchTimer);
        Debug.Log("Read data back to json" + JsonUtility.ToJson(GameState.config));
    }


    public ThemeAssetPack GetThemeAssetPackForArena(int arenaIdx)
    {
        return GetThemeAssetPackForArena(vsMode_arenas[arenaIdx]);
    }

    public ThemeAssetPack GetThemeAssetPackForArena(string arenaPath)
    {
        string themeName = File.ReadAllText(arenaPath+"/"+THEME_FILE_NAME, Encoding.UTF8).Replace("\n", "").Replace("\r", "");
        
        //Debug.Log(arenaPath + " -> " + themeName);
        return ThemeAssetPack.masterList[themeName];
    }


    public ThemeAssetPack GetThemeAssetPackForWorld(int worldIdx)
    {
        return GetThemeAssetPackForWorld(levelsMode_Worlds[worldIdx]);
    }

    public ThemeAssetPack GetThemeAssetPackForWorld(string worldPath)
    {
        // find theme.txt
        // read the contents
        // return ThemeAssetPack.masterList[contents];

        string themeName = File.ReadAllText(worldPath+"/"+THEME_FILE_NAME, Encoding.UTF8).Replace("\n", "").Replace("\r", "");

        return ThemeAssetPack.masterList[themeName];
    }

    public string[] LoadArena(int arenaIdx)
    {
        return LoadArena(vsMode_arenas[arenaIdx]);
    }

    public string[] LoadArena(string arenaPath)
    {
        return ReadAllCSVs(arenaPath);
    }

    // returns an array of levels, where each level is
    // [csvForFloor1, csvForFloor2, ...]
    public string[][] LoadLevelsFromWorld(int worldIdx)
    {
        return LoadLevelsFromWorld(levelsMode_Worlds[worldIdx]);
    }

    public string[][] LoadLevelsFromWorld(string worldPath)
    {
        string[] levels = Directory.GetDirectories(worldPath);

        string[][] retval = new string[levels.Length][];

        for (int k = 0; k < levels.Length; k++)
        {
            string levelPath = levels[k];
            retval[k] = ReadAllCSVs(levelPath);
        }

        return retval;
    }

    private string[] ReadAllCSVs(string dirPath)
    {
        DirectoryInfo levelDirectory = new DirectoryInfo(dirPath);
        FileInfo[] files = levelDirectory.GetFiles("*.csv", SearchOption.TopDirectoryOnly);

        int numFiles = 0;
        for (int i = 0; i < numFiles; i++)
        {
            if (files[i].Extension == ".meta" || files[i].Extension == ".csv.meta") continue;
            numFiles++;
        }

        string[] CSVs = new string[files.Length];
        for (int i = 0, j = 0; i < files.Length; i++)
        {
            //Debug.Log(files[i].Name);
            if (files[i].Extension == ".meta" || files[i].Extension == ".csv.meta") continue;

            CSVs[j] = File.ReadAllText(dirPath+"/"+files[i].Name, Encoding.UTF8);
            //Debug.Log(CSVs[j]);

            j++;
        }

        return CSVs;
    }
}
