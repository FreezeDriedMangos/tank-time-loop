using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerIconController : MonoBehaviour
{
    public Color ActiveColor;
    public Color HologramColor;
    public Color DeadColor;
    public Color DisabledColor;

    private Image img;

    private bool setup;
    void Setup()
    {
        if (setup) return;
        setup = true;

        img = GetComponent<Image>();
    }

    public void SetActive()
    {
        Setup();

        img.color = ActiveColor;
    }

    public void SetDisabled()
    {
        Setup();

        img.color = DisabledColor;
    }

    public void SetHologram()
    {
        Setup();

        img.color = HologramColor;
    }

    public void SetDead()
    {
        Setup();

        img.color = DeadColor;
    }

    public void SetRogue()
    {
        transform.eulerAngles = new Vector3(0, 0, 180);
    }
}
