using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfinderController : MonoBehaviour
{
    FileLoader fileLoader;
    FloydWarshall fw;
    bool debug = true;
    // Start is called before the first frame update
    void Start()
    {
        fileLoader = GetComponent<FileLoader>();
        fw = GetComponent<FloydWarshall>();
        GetLoadedLevels();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GetLoadedLevels()
    {
        foreach (string world in fileLoader.levelsMode_Worlds)
        {
            string[][] levels = fileLoader.LoadLevelsFromWorld(world);

            if (debug)
                PrintMatrix(levels);

            foreach (string[] level in levels)
            { 
                fw.InitFloydWarshall(level);
            }
        }
    }

    void PrintMatrix(string[][] levels)
    {
        for (int i = 0; i < levels.GetLength(0); i++)
            {
                for (int j = 0; j < levels[i].Length; j++)
                {
                    Debug.Log(levels[i][j] + "\t");
                }
            }
    }
}
