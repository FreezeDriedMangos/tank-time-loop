using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LevelTitleSlide : MonoBehaviour
{

    public GameObject[] parts;
    public Text text;

    private float timer;
    public float exitAnimationLength_seconds;

    public float totalTime_seconds;

    public float partsSpeedScaling = 0.05f;
    

    public void SetDisplay()
    {
        text.text = "World " + (GameState.currentWorld+1) +  " - Level " + GameState.currentLevel;

        foreach (GameObject g in parts)
        {
            g.transform.position = new Vector3(Screen.width/2f, Screen.height/2f, 0);
        }

        timer = totalTime_seconds;
    }

    void Update()
    {
        if (timer <= 0) return;

        timer -= Time.deltaTime;

        if (timer <= exitAnimationLength_seconds)
        {
            // need to move each component by its width or more in (totalTime_seconds*(1-percentTimeStill)) seconds

            float i = 0;
            foreach (GameObject g in parts)
            {
                float a = exitAnimationLength_seconds * (i+1) / (float)parts.Length;

                g.transform.position += Time.deltaTime * new Vector3(Screen.width / (a), 0, 0);
                i++;
            }
        }
    }
}
