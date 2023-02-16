using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SliderPlayerPrefs : MonoBehaviour
{
    Slider s;
    public string prefName;

    private void Awake() 
    {
        if (s == null) s = GetComponent<Slider>();
        s.value = PlayerPrefs.GetFloat(prefName);
    }

    public void SetValue()
    {
        if (s == null) s = GetComponent<Slider>();

        PlayerPrefs.SetFloat(prefName, s.value);
    }
}
