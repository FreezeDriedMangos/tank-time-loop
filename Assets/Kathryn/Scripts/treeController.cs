using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class treeController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter (Collision col) {
        if(col.gameObject.GetComponent<BulletController>() == null) {
            return;
        }

        col.gameObject.GetComponent<BulletController>().DestroyShell();
        return;
    }
}
