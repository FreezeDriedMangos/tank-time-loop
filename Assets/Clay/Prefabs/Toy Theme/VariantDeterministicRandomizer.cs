using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariantDeterministicRandomizer : MonoBehaviour
{
    private float weightsSum = -1;

    public GameObject[] personalAestheticVariants;
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

        if (personalAestheticVariants.Length != weights.Length) return;

        if (weightsSum == -1)
        {
            weightsSum = 0;
            foreach (float weight in weights) 
            {
                if (weight <= 0) continue;
                weightsSum += weight;
            }
        }

        for (int i = 0; i < personalAestheticVariants.Length; i++)
        {
            personalAestheticVariants[i].active = false;
        }

        float randVal = GetRandom();
        for (int i = 0; i < weights.Length; i++)
        {
            if (weights[i] <= 0) continue;

            randVal -= weights[i];
            if (randVal <= 0)
            {
                personalAestheticVariants[i].active = true;
                return;
            }
        }

        personalAestheticVariants[personalAestheticVariants.Length-1].active = true;
    }

    // returns a "random" number that's always the same for a given tile position
    float GetRandom()
    {
        //return Random.Range(0, weightsSum+0.01f);
        string posStr = GameState.levelName +":"+ this.transform.position.x + "," + this.transform.position.y + "," + this.transform.position.z;
        return Mathf.Abs(posStr.GetHashCode()/7f) % (weightsSum+0.1f);
    }

}
