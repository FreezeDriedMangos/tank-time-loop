using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST_SimplePathfind : MonoBehaviour
{
    public TankController tc;

    //public Transform target;
    public TEST_NavMesh pathing;

    public bool moveToNext = true;
    public int idx = 0;

    Vector3 target;
    Vector2 tankTarget;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (pathing.path.corners == null || pathing.path.corners.Length <= 0 || pathing.path.corners.Length <= idx) return;

        if (moveToNext)
        {
            moveToNext = false;
            Debug.Log("AAAAAAAAAAAAA");
            target = pathing.path.corners[idx++];
            tankTarget = new Vector2(target.x, target.z);
            tc.MoveTo(tankTarget);
            Debug.Log("BBBBBBBBBB");
        }

        float dist = Vector3.Distance(target, transform.position);
        if (tc.reachedMoveTarget || dist <= 0.1)
        {
            moveToNext = true;
        }
    }

    void OnDrawGizmos()
    {

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(target, 0.12f);
    }
}
