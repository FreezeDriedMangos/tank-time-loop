using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST_Reflection : MonoBehaviour
{
    public Transform targ1;
    public Transform targ2;

    public bool xNormal;
    public bool forceDistance;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        Vector3 normal = new Vector3(0, 0, 1);
        if (xNormal) normal = new Vector3(1, 0, 0);

        Vector3 antiNormal = new Vector3(1, 0, 0);
        if (xNormal) antiNormal = new Vector3(0, 0, 1);

        Debug.DrawRay(transform.position, normal, Color.red);
        //Debug.DrawRay(transform.position, antiNormal, new Color(0.5f, 0, 0));

        Vector3 dir1 = targ1.position-transform.position;
        Vector3 ref1 = Vector3.Reflect(dir1, normal);
        Debug.DrawRay(transform.position, dir1, Color.green);
        Debug.DrawRay(transform.position, ref1, Color.green);


        // Vector3 dir2 = targ2.position-transform.position;
        // Vector3 ref2 = Vector3.Reflect(dir2, normal);
        // Debug.DrawRay(transform.position, dir2, Color.blue);
        // Debug.DrawRay(transform.position, ref2, Color.blue);
        
        //Vector3 avgPos = (targ2.position + targ1.position) / 2f;
        //Debug.DrawLine(transform.position, avgPos, Color.yellow);

        Vector3 newTarg = targ1.position;
        if (!xNormal)
        {
            // float dist = targ1.position.z-transform.position.z;

            // newTarg = targ1.position - new Vector3(0, 0, dist*2);

            // if (forceDistance)
            // {
            //     //targ2.z-targ1.z = d;
            //     float desiredDistance = targ2.position.z - targ1.position.z;

            //     this.transform.position = new Vector3(-targ2.position.z, this.transform.position.y, this.transform.position.z);
            // }


            // pretending this is all stuff done by targ1
            Vector3 target = targ2.position;
            Vector3 self = targ1.position;
            Vector3 mirror = this.transform.position;
            float tx = target.x;
            float sx = self.x; // "self" x
            
            float mx = mirror.x; // this would normally be incremented by the "can I hit this target" function

            Vector3 mirroredTarget = newTarg = new Vector3(target.x + 2*(mx-tx), target.y, target.z);
            // all I really need to do from here is raycast from self to mirroredTarget and see if (hit.point.x == mx && (hit.normal == new Vector3(1, 0, 0) || (-1, 0, 0) ))

            //some extra stuff
            float ty = target.y;
            float sy = self.y;
            float my = (mx-sx) * ( (ty-sy) / (mx-sx + tx-mx) );

            if (forceDistance)
            {
                this.transform.position = new Vector3(mx, this.transform.position.y, my);
            }

        }
        else
        {
            float dist = targ1.position.x-transform.position.x;

            newTarg = targ1.position - new Vector3(dist*2, 0, 0);
        }

        Debug.DrawLine(transform.position, newTarg, Color.yellow);

    }
}
