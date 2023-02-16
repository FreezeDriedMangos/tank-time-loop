using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountDown : MonoBehaviour
{
    public float time_seconds = 1;
    private float time_remaining = 0;
    private bool haveActivatedNext = true;

    public CountDown nextNumber;
    private Text text;

    public void StartCountdown()
    {
        time_remaining = time_seconds;
        haveActivatedNext = false;

        if (text == null) text = GetComponent<Text>();
    }

    public void EndCountdown()
    {
        time_remaining = -1;
        haveActivatedNext = true;

        if (text == null) text = GetComponent<Text>();
        
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        if (nextNumber != null) nextNumber.EndCountdown();
    }

    public void CascadeSetTime(float seconds)
    {
        time_seconds = seconds;
        if (nextNumber != null) nextNumber.CascadeSetTime(seconds);
    }

    void Update()
    {

        if (time_remaining <= 0)
        {
            if (!haveActivatedNext && nextNumber != null) 
            {
                nextNumber.StartCountdown();
                haveActivatedNext = true;
            }
            
            return;
        }

        time_remaining -= Time.deltaTime;

        float alpha = time_remaining/time_seconds; 
        text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
    }
}
