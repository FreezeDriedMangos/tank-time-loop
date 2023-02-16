using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelsModeListPopulator : MonoBehaviour
{
    public FileLoader f = null;
    public LevelsSelectButton levelsSelectButtonPrefab;
    public int numColums = 3;

    private bool setup = false;
    private RectTransform rt = null;
    


    private void OnEnable() 
    {
        Setup();    
    }

    void Setup()
    {
        if (setup) return;
        setup = true;

        GameState.currentLevel = 0;

        rt = GetComponent<RectTransform>();

        for (int i = 0; i < f.levelsMode_Worlds.Length; i++)
        {
            GameObject g = GameObject.Instantiate(levelsSelectButtonPrefab.gameObject);
            LevelsSelectButton b = g.GetComponent<LevelsSelectButton>();

            g.GetComponent<RectTransform>().SetParent(rt);
            b.SetWorld(f.levelsMode_Worlds[i]);
            b.SetLocation(i/numColums, i%numColums, numColums);
            b.worldNum = i;
        }
    }
}
