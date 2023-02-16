using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDestructionNotifier : MonoBehaviour
{
    public delegate void NotifyDelegate();
    public NotifyDelegate notify;

    private void OnDestroy() 
    {
        notify();
    }
}
