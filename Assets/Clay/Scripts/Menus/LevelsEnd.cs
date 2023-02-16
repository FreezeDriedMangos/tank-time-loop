using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelsEnd : MonoBehaviour
{
    public Text text;

    // Start is called before the first frame update
    void Start()
    {
        text.text = "You made it to World " + (GameState.currentWorld+1) + " - Level " + GameState.currentLevel;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
