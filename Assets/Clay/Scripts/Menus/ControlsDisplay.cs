using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlsDisplay : MonoBehaviour
{
    [SerializeField] private Text text;



    public void SetDisplay(string inputMethod)
    {
        string moveString = "";
        string aimString = "";
        string shootString = "";
        string rocketString = "";
        string bombString = "";
        
        Debug.Log(inputMethod);

        if (inputMethod.StartsWith("Joycon"))
        {
            moveString = "Stick";
            aimString = "Motion, recenter with home/capture";
            shootString = "ZR / ZL";
            rocketString = "R / L";
            bombString = "SR / SL / DPad Down";
        }

        if (inputMethod.Equals("Mouse and Keyboard"))
        {
            moveString = "WSAD";
            aimString = "Mouse";
            shootString = "Left Click";
            rocketString = "Right Click";
            bombString = "Spacebar / Middle Click";
        }
            
            
        if (inputMethod.Equals("PS4 Dualshock"))
        {
            moveString = "Left Stick";
            aimString = "Right Stick / Triggers";
            shootString = "Triangle";
            rocketString = "Square";
            bombString = "Circle";
        }
        
        if (inputMethod.StartsWith("XBox") || inputMethod.StartsWith("Xbox") || inputMethod.StartsWith("X Box"))
        {
            moveString = "Left Stick";
            aimString = "Right Stick / Bumpers";
            shootString = "Y / Right Trigger";
            rocketString = "B";
            bombString = "A / Left Trigger";
        }

        text.text = "Move:    " + moveString + "\n" +
                    "Aim:     " + aimString  + "\n" +
                    "Shoot:   " + shootString  + "\n" +
                    "Rocket:  " + rocketString  + "\n" +
                    "Bomb:    " + bombString;
                    
    }
}
