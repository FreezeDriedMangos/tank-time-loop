using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixMusic : MonoBehaviour
{
    public MusicController music;

    // Start is called before the first frame update
    void Start()
    {
        bool[] arr1 = {false, false, false, false, false, true, false, false, false, false, false};
        music.playClip = new List<bool>(arr1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
