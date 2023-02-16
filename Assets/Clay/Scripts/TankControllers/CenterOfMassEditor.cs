using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CenterOfMassEditor : MonoBehaviour
{
    private static Vector3 DOWN = new Vector3(0, -1, 0);

    public Vector3 com;
    private Rigidbody rb;

    public float extraGravity = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (com != rb.centerOfMass)
            rb.centerOfMass = com;
    }

    void FixedUpdate()
    {
        rb.velocity += DOWN*extraGravity;
    }
}
