﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialDeterministicRandomizer : MonoBehaviour
{
    private float weightsSum = -1;

    public string randomizerID;

    public Material[] materialVariants;
    public float[] weights;
    
    private bool setup = false;

    void Start() 
    {
        Setup();    
    }

    void Setup()
    {
        if (setup) return;
        setup = true;

        if (materialVariants.Length != weights.Length) return;

        if (weightsSum == -1)
        {
            weightsSum = 0;
            foreach (float weight in weights) 
            {
                if (weight <= 0) continue;
                weightsSum += weight;
            }
        }

        float randVal = GetRandom();
        for (int i = 0; i < weights.Length; i++)
        {
            if (weights[i] <= 0) continue;

            randVal -= weights[i];
            if (randVal <= 0)
            {
                GetComponent<MeshRenderer>().material = materialVariants[i];
                //personalAestheticVariants[i].active = true;
                return;
            }
        }

        GetComponent<MeshRenderer>().material = materialVariants[materialVariants.Length-1];
    }

    // returns a "random" number that's always the same for a given tile position
    float GetRandom()
    {
        //return Random.Range(0, weightsSum+0.01f);
        string posStr = GameState.levelName +":"+ this.transform.position.x + "," + this.transform.position.y + "," + this.transform.position.z;
        return Mathf.Abs(posStr.GetHashCode()/7f) % (weightsSum+0.1f);
    }

}
