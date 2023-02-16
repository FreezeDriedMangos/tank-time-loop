using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerJoinIcon : MonoBehaviour
{
    public Color activeColor;
    public Color disabledColor;

    public Image flashImage;

    private Image i;

    public float flashLength_seconds;
    private float flash_timer;

    public void SetColor(bool enabled)
    {
        if (enabled)
            SetEnabledColor();
        else
            SetDisabledColor();
    }

    public void SetDisabledColor()
    {
        Setup();
        i.color = disabledColor;
    }

    public void SetEnabledColor()
    {
        Setup();
        i.color = activeColor;
    }

    private bool setup = false;
    private void Setup()
    {
        if (setup) return;
        setup = true;

        i = GetComponent<Image>();
    }

    public void InputResponse()
    {
        Setup();

        flash_timer = flashLength_seconds;
    }

    void Update()
    {
        if (flash_timer <= 0) return;

        flash_timer -= Time.deltaTime;
        flashImage.color = new Color(flashImage.color.r, flashImage.color.g, flashImage.color.b, flash_timer / flashLength_seconds);
    }
}
