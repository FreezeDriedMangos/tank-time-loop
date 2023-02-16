using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchReactor : MonoBehaviour
{
    public delegate void SwitchToggle();

    public SwitchToggle SwitchEnabled;
    public SwitchToggle SwitchDisabled;
}
