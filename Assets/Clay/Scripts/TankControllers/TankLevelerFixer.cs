using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankLevelerFixer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate() {
        transform.localPosition = new Vector3(0, transform.localPosition.y, 0);
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0, transform.localEulerAngles.z);
    }
}
