using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankCrusher : MonoBehaviour
{
    public bool activated;

    void OnTriggerEnter(Collider other) 
    {
        // if tank, set destroyed
        // if bullet, destroy
        // if bomb, nothing   

        if (!activated) return;

        TankController t = other.GetComponent<TankController>();
        BulletController b = other.GetComponent<BulletController>();

        if (t != null) t.SetDestroyed(true);
        if (b != null) Destroy(b.gameObject); 
    }
}
