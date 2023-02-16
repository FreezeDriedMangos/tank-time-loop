using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorGimmick : MonoBehaviour
{
    //[SerializeField] private Rigidbody rb;
    [SerializeField] private float raiseSpeed;
    [SerializeField] private float maxHeight;

    [SerializeField] private TankCrusher crusher;

    private NavigationBaker nav;
    
    public bool dropping = false;
    public bool rising = false;

    public bool inverted = false;


    

    public float gravity = 9.8f;
    [SerializeField] private GameObject elevatorBlock;
    Vector3 velocity;


    void Awake()
    {
        //if (rb == null) return;

        //rb.transform.localPosition = new Vector3(0, 0, 0);
        SwitchReactor sr = GetComponent<SwitchReactor>();

        if (inverted)
        {
            sr.SwitchEnabled = DropElevator;
            sr.SwitchDisabled = RaiseElevator;
            RaiseElevator();
        }
        else
        {
            sr.SwitchEnabled = RaiseElevator;
            sr.SwitchDisabled = DropElevator;
        }

        if (GameState.config != null && GameState.config.gimmicksConfig != null)
        {
            raiseSpeed = GameState.config.gimmicksConfig.elevator_raiseSpeed;
        }


        nav = GameObject.FindObjectOfType<NavigationBaker>();

    }

    public void RaiseElevator()
    {
        //if (rb == null) return;
        
        Debug.Log("rising!");

        crusher.activated = false;
        //rb.useGravity = false;
        //rb.velocity = new Vector3(0, raiseSpeed, 0);

        velocity = new Vector3(0, raiseSpeed, 0);

        dropping = false;
        rising = true;
    }

    public void DropElevator()
    {
        //if (rb == null) return;

        Debug.Log("falling!");

        crusher.activated = true;
        //rb.useGravity = true;
        velocity = new Vector3();

        dropping = true;
        rising = false;

        if (nav != null) nav.RebuildNavMeshSoon();
    }


    public void FixedUpdate()
    {
        //if (rb == null) return;

        // if (!dropping && !rising && rb.velocity != Vector3.zero)
        // {
        //     //rb.velocity = new Vector3();
        //     //rb.useGravity = false;
        //     rb.transform.localPosition = new Vector3(0, 0, 0);
        // }

        if (dropping)
        {
            velocity += new Vector3(0, -1, 0) * gravity*Time.deltaTime;
            elevatorBlock.transform.position += velocity*Time.deltaTime;
        }
        if (rising)
        {
            elevatorBlock.transform.position += velocity*Time.deltaTime;
        }

        if (!dropping && elevatorBlock.transform.localPosition.y > maxHeight)
        {
            if (nav != null) nav.RebuildNavMeshSoon();
            //rb.velocity = new Vector3();
            //rb.transform.localPosition = new Vector3(0, maxHeight, 0);
            elevatorBlock.transform.localPosition = new Vector3(0, maxHeight, 0);
            rising = false;
        }

        if (dropping && elevatorBlock.transform.localPosition.y <= 0.01)
        {
            crusher.activated = false;
            //rb.useGravity = false;
            //rb.velocity = new Vector3();
            //rb.transform.localPosition = new Vector3(0, 0, 0);
            elevatorBlock.transform.localPosition = new Vector3(0, 0, 0);
            dropping = false;
        }
    }
}
