using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BuildImmediately : MonoBehaviour
{
    LevelBuilder b;
    TextAsset old1;
    TextAsset old2;

    // Start is called before the first frame update
    void Start()
    {
        b = GetComponent<LevelBuilder>();     
    }

    // Update is called once per frame
    void Update()
    {
        if (b == null) b = GetComponent<LevelBuilder>(); 
        
        if (b.csvFloor1 == old1 && b.csvFloor2 == old2) return;

        old1 = b.csvFloor1;
        old2 = b.csvFloor2;
        b.BuildLevel();
    }
}
