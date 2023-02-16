using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class iceCubeController : MonoBehaviour
{
    private float speed = 0.5f;
    private float distance = 1.5f;
    private Vector3 velocity;
    private bool isMoving = false;
    private string direction;
    private Vector3 originalPosition;

    public Dictionary<string, bool> canMoveInDir = new Dictionary<string, bool>();


    public iceCubeColliderController col;
    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
        
    }

    
    void FixedUpdate()
    {
        //if a child gets collided with, move in opposite direction
        if(isMoving == true) {
            
            transform.position += velocity*Time.deltaTime;
            Vector3 newPosition = transform.position;
            float compX = Mathf.Abs(newPosition.x - originalPosition.x);
            float compZ = Mathf.Abs(newPosition.z - originalPosition.z);
            if(compX < distance && compZ < distance) {
                //keep Moving
            } else {
                // velocity = new Vector3(0,0,0);
                //originalPosition = newPosition;
                //isMoving = false;
                StopMoving();
            }

            
            
        }

    }

    void OnCollisionEnter(Collision collision) {
        // Collider c = collision.collider;
        // if (c.GetComponent<BulletController>() == null) {
        //     return;
        // }
        // // destroying the bullet so that it doesnt bounce and trigger another ice cube
        // c.GetComponent<BulletController>().DestroyShell();

        // List<GameObject> children = new List<GameObject>();
        // foreach (Transform child in transform) {
        //     children.Add(child.gameObject);
        // }
        // foreach (GameObject child in children) {
        //     if(child.GetComponent<iceCubeColliderController>() != null) {
        //         if(child.GetComponent<iceCubeColliderController>().colFlag == true) {
        //             isMoving = true;
        //             if(child.name == "collider_pos_x") {
        //                 // move obj in neg x direction
        //                 moveObj("negx");
        //             } else if(child.name == "collider_neg_x") {
        //                 //move obj in pos x direction
        //                 moveObj("posx");
        //             } else if(child.name == "collider_pos_z") {
        //                 //move obj in neg z direction
        //                 moveObj("negz");
        //             } else if(child.name == "collider_neg_z") {
        //                 //move obj in pos z direction
        //                 moveObj("posz");
        //             }
        //         }
        //     }
        // }
        // return;
    }

    public void StopMoving() {
        isMoving = false;
        velocity = Vector3.zero;

        try
        {
            NavigationBaker b = GameObject.FindObjectOfType<NavigationBaker>();
            b.RebuildNavMeshSoon();
        }
        catch (System.Exception) {}
    }

    public void moveObj(string dir) {
        //send out a raycast??? to check if there is something in the way?????
        //if its clear, move cube

        if (!canMoveInDir[dir]) return;
        if (isMoving) return;

        if(dir == "negx") {
            Debug.Log("Moving in negative x direction");
            velocity = new Vector3(-speed, 0, 0);

        } else if(dir == "posx") {
            Debug.Log("Moving in positive x direction");
            velocity = new Vector3(speed, 0, 0);
        } else if(dir == "negz") {
            Debug.Log("Moving in negative z direction");
            velocity = new Vector3(0, 0, -speed);
        } else if(dir == "posz") {
            Debug.Log("Moving in positive z direction");
            velocity = new Vector3(0, 0, speed);
        } else {
            Debug.Log("WRONG DIReCtion");
            return;
        }

        isMoving = true;
        originalPosition = new Vector3(Mathf.Floor(transform.position.x), Mathf.Floor(transform.position.y), Mathf.Floor(transform.position.z));


        return;
    }
}
