using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<BulletController>().Setup();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
