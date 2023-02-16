using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimmickMeta : MonoBehaviour
{
    public string metadata;

    public delegate void SetupDelegate();
    public SetupDelegate Setup;

    private bool hasSetup;

    void FixedUpdate() 
    {
        if (!hasSetup && Setup != null)
        {
            hasSetup = true;
            Setup();
        }
    }
}
