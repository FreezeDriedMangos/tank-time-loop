using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TEST_NavMesh : MonoBehaviour
{
    public Transform p1;
    public Transform p2;

    public NavMeshPath path;
    private float elapsed = 0.0f;

    public bool updatePath;


    // Start is called before the first frame update
    void Start()
    {
        path = new NavMeshPath();
        elapsed = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (updatePath)
        {
            updatePath = false;

            NavMesh.CalculatePath(p1.position, p2.position, NavMesh.AllAreas, path);
        }

        // // Update the way to the goal every second.
        // elapsed += Time.deltaTime;
        // if (elapsed > 1.0f)
        // {
        //     elapsed -= 1.0f;
        //     NavMesh.CalculatePath(p1.position, p2.position, NavMesh.AllAreas, path);
        // }

        for (int i = 0; i < path.corners.Length - 1; i++)
            Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);

    }

    void OnDrawGizmos() 
    {
        if (path == null || path.corners == null) return;

        for (int i = 0; i < path.corners.Length; i++)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(path.corners[i], 0.1f);
        }
    }
}
