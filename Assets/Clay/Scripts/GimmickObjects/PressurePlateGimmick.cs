using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateGimmick : MonoBehaviour
{
    GimmickMeta gm;

    List<SwitchReactor> recievers = new List<SwitchReactor>();

    void Awake()
    {
        gm = GetComponent<GimmickMeta>();
        gm.Setup = Setup;
    }

    public void Setup()
    {
        foreach (GimmickMeta gm in GameObject.FindObjectsOfType<GimmickMeta>())
        {
            if (gm.metadata != this.gm.metadata) continue;

            SwitchReactor sr = gm.GetComponent<SwitchReactor>();

            if (sr == null) continue;

            recievers.Add(sr);
        }
    }

    private int numOn = 0;
    private List<TankController> ts = new List<TankController>();

    void OnTriggerEnter(Collider c)
    {
        TankController t = c.GetComponent<TankController>();
        if (t != null) { numOn ++; ts.Add(t); }
        if (numOn != 1) return; // someone's already been sitting on the pplate or some non-tank hit it

        foreach (SwitchReactor sr in recievers)
        {
            sr.SwitchEnabled();
        }
    }

    void OnTriggerExit(Collider c)
    {
        TankController t = c.GetComponent<TankController>();
        if (t != null) Exited(t);
    }

    void Exited(TankController t)
    {
        numOn--;

        if (t != null) ts.Remove(t);

        if (numOn > 0) return; // someone's still sitting on the pplate

        foreach (SwitchReactor sr in recievers)
        {
            if (sr == null) continue;
            Debug.Log("sr: " + sr);
            try { sr.SwitchDisabled(); } catch {} // this is so dumb that I need a try/catch. For some reason unity thought sr was null sometimes
        }
    }

    void FixedUpdate() 
    {
        List<TankController> temp = null;
        foreach (TankController t in ts)
        {
            if (!t.destroyed) continue;

            if (temp == null) temp = new List<TankController>();
            temp.Add(t);
        }   

        if (temp == null) return;

        foreach (TankController t in temp)
        {
            Exited(t);
        }
    }
}
