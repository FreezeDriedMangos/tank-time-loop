using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StupidRampFixer : MonoBehaviour
{
    List<TankController> tanks = new List<TankController>();
    static float bump = 0.001f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate() 
    {
        foreach (TankController t in tanks)
        {
            if (t.moving == false) continue;
            t.transform.position = new Vector3(t.transform.position.x, t.transform.position.y+bump, t.transform.position.z);
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        TankController t = other.gameObject.GetComponent<TankController>();
        
        if (t == null) return;

        tanks.Add(t);
    }

    private void OnTriggerExit(Collider other) 
    {
        TankController t = other.gameObject.GetComponent<TankController>();

        if (t == null) return;

        tanks.Remove(t);
    }
}
