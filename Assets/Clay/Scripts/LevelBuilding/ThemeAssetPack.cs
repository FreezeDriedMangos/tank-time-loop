using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThemeAssetPack : MonoBehaviour
{
    public static Dictionary<string, ThemeAssetPack> masterList;

    public GameObject floor;
    public GameObject[] walls;
    public GameObject[] archs;
    public GameObject[] ramps;
    public GameObject[] breakableWalls;
    public GameObject[] halfWalls;
    public GameObject[] transparentWalls;

    public GameObject[] gimmickObjects;


    public GameObject[] p1Spawns;
    public GameObject[] p2Spawns;
    public GameObject[] p3Spawns;
    public GameObject[] p4Spawns;
    
    [Header("Generic things, mostly should be the same for every asset pack.")]
    public GameObject[] enemyPrefabs;

    
    public GameObject kingOfTheHillCapturePoint;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
