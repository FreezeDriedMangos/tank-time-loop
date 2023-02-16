using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TankTransform : MonoBehaviour
{
    //  private bool setup = false;

    public bool facingForwards;
    [SerializeField] private Transform gunTip  = null;
    [SerializeField] private Transform head    = null;
    [SerializeField] private Transform leveler = null;
    //[SerializeField] private Transform body;

    public Vector3 LeveledEulerAngles
    {
        get { return leveler.eulerAngles; }
        set {}
    }

    public Vector3 ShellSpawnLocation 
    { 
        get { return gunTip.position; } 
        private set {}
    }

    public Quaternion ShellSpawnRotation
    { 
        get { return gunTip.rotation; } 
        private set {}
    }

    public Vector3 BombSpawnLocation 
    { 
        get { return head.position; } 
        private set {}
    }

    public float HeadRotation 
    { 
        get { return head.localEulerAngles.y + BodyRotation; } 
        set { head.localEulerAngles = new Vector3(0, value-BodyRotation, 0); }
    }

    public float BodyRotation
    {
        //get { return body.eulerAngles.y; } 
        //set { body.eulerAngles = new Vector3(0, value, 0); }
        get { return transform.eulerAngles.y; } 
        set 
        { 
            float h = HeadRotation;
            //HeadRotation -= value; // make sure that turning the body doesn't affect the head
            transform.eulerAngles = new Vector3
            (
                transform.eulerAngles.x, 
                value, 
                transform.eulerAngles.z
            ); 

            HeadRotation = h;
        }
    }

    public Vector3 Forward 
    {
        get
        {
            Vector3 forward = transform.forward;
            forward = new Vector3(forward.z, forward.y, -forward.x);

            if(facingForwards) return forward;
            else return -forward;
        }
    }

    public void LevelTank()
    {
        // transform.eulerAngles = new Vector3 
        // (
        //     leveler.eulerAngles.x,
        //     transform.eulerAngles.y,
        //     leveler.eulerAngles.z
        // );

        // transform.eulerAngles = new Vector3 
        // (
        //     ClampAngle(transform.eulerAngles.x, -45, 45),
        //     transform.eulerAngles.y,
        //     ClampAngle(transform.eulerAngles.z, -45, 45)
        // );
    }

    // float ClampAngle(float a, float min, float max)
    // {
    //     a = a % 360f; // a is now on (-360, 360)
    //     a += 360f;    // a is now on (360, 2*360)
    //     a = a % 360f; // a is now on [0, 360)

    //     if (a < min) return min + 0.5f;
    //     if (a > max) return max - 0.5f;
    //     return a;
    // }

    public TankTransformState GetCurrentState()
    {
        TankTransformState s = new TankTransformState();
        // s.levelerPos = leveler.position;
        // s.levelerRot = leveler.eulerAngles;
        // s.levelerVel = leveler.GetComponent<Rigidbody>().velocity;
        // s.levelerAngularVel = leveler.GetComponent<Rigidbody>().angularVelocity;

        s.tankPos = transform.position;
        s.tankRot = transform.eulerAngles;

        s.HeadRotation = HeadRotation;
        s.BodyRotation = BodyRotation;

        s.facingForwards = facingForwards;

        return s;
    }

    public void SetState(TankTransformState s)
    {
        // leveler.position    = s.levelerPos;
        // leveler.eulerAngles = s.levelerRot;
        // leveler.GetComponent<Rigidbody>().velocity = s.levelerVel;
        // leveler.GetComponent<Rigidbody>().angularVelocity = s.levelerAngularVel;

        if (s == null) return;

        transform.position = s.tankPos;
        transform.eulerAngles = s.tankRot;

        HeadRotation = s.HeadRotation;
        BodyRotation = s.BodyRotation;

        facingForwards = s.facingForwards;
    }
    
    // public void Setup()
    // {
    //     if (setup) return;
    //     setup = true;

    //     gunTip = transform.Find("Head/Gun_Body/Gun_Tip");
    //     head = transform.Find("Head");
    //     Debug.Log(head);
    //     //body = transform.Find("Body");
    // }

    // void Start() { Setup(); }
    // void Awake() { Setup(); }
}
