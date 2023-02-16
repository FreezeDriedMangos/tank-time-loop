using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalBoundaryMaker : MonoBehaviour
{
    public GameObject boundariesParent;

    public GameObject xp;
    public GameObject xn;
    public GameObject zp;
    public GameObject zn;
    
    public GameObject boundary_xp;
    public GameObject boundary_xn;
    public GameObject boundary_zp;
    public GameObject boundary_zn;

    private bool setup;
    void FixedUpdate() 
    {
        if (setup) return;
        setup = true;

        bool val = boundariesParent.active;
        boundariesParent.SetActive(true);

        xp.SetActive(boundary_xp.active);
        xn.SetActive(boundary_xn.active);
        zp.SetActive(boundary_zp.active);
        zn.SetActive(boundary_zn.active);

        boundariesParent.SetActive(val);
    }
}
