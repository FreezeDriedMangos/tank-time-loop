using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaListPopulator : MonoBehaviour
{
    public FileLoader f = null;
    public ArenaSelectButton arenaSelectButtonPrefab;
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

        rt = GetComponent<RectTransform>();

        for (int i = 0; i < f.vsMode_arenas.Length; i++)
        {
            //Debug.Log("setting up button for " + f.vsMode_arenas[i]);
            GameObject g = GameObject.Instantiate(arenaSelectButtonPrefab.gameObject);
            ArenaSelectButton b = g.GetComponent<ArenaSelectButton>();

            g.GetComponent<RectTransform>().SetParent(rt);
            b.SetArena(f.vsMode_arenas[i]);
            b.SetLocation(i/numColums, i%numColums, numColums);
        }
    }
}
