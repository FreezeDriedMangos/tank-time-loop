using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class destructionController : MonoBehaviour
{
    public GameObject remains;

    public delegate void Notifier();
    public Notifier OnBlowUp;

    private void OnTriggerEnter (Collider col) {
        
        if (col.GetComponent<ExplosionController>() == null) {
            return;
        }
        
        GameObject g = GameObject.Instantiate(remains);
        g.transform.position = remains.transform.position + this.transform.position;

        try
        {
            if (GameState.config.levelBuildingConfig.makeBoundaries)
            {
                Transform boundaries = this.transform.parent.Find("InsideBoundaries");
                boundaries.gameObject.SetActive(true);
                boundaries.transform.Find("X+ Boundary").gameObject.SetActive(true);
                boundaries.transform.Find("X- Boundary").gameObject.SetActive(true);
                boundaries.transform.Find("Z+ Boundary").gameObject.SetActive(true);
                boundaries.transform.Find("Z- Boundary").gameObject.SetActive(true);
            }
        }
        catch (Exception) {}

        if (OnBlowUp != null) OnBlowUp(); 
        Destroy(gameObject);

        try
        {
            NavigationBaker b = GameObject.FindObjectOfType<NavigationBaker>();
            b.RebuildNavMeshSoon();
        }
        catch (Exception) {}
    }
}
