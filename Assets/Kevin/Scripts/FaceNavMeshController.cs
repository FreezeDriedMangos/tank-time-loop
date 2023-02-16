using System;
using UnityEngine;
using UnityEngine.AI;

public class FaceNavMeshController : MonoBehaviour {

    NavMeshAgent agent;
    Rigidbody rb;

	// Use this for initialization
	void Start () {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        // if (!agent.isStopped)
        // {
        //     var targetPosition = agent.pathEndPosition;
        //     var targetPoint = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        //     var _direction = (targetPoint - transform.position).normalized;
        //     var _lookRotation = Quaternion.LookRotation(_direction);

        //     float angle = Quaternion.Angle(transform.rotation, _lookRotation);

        //     if (angle > 20)
        //         agent.isStopped = true;

        //     else
        //         agent.isStopped = false;

        //     transform.rotation = Quaternion.RotateTowards(transform.rotation, _lookRotation, 130f * Time.deltaTime);
        // }
	}

    public void moveAgent(int[] waypoint)
    {
        
            var targetPosition = new Vector3(waypoint[0], transform.position.y, waypoint[1]);
            var targetPoint = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
            var _direction = (targetPoint - transform.position).normalized;
           
            var _lookRotation = Quaternion.LookRotation(_direction);

            float angle = Quaternion.Angle(transform.rotation, _lookRotation);


            if (angle < 20) 
            {
                agent.isStopped = false;
                agent.SetDestination(new Vector3(waypoint[0], transform.position.y, waypoint[1]));
            }

            else
                agent.isStopped = true;


            transform.rotation = Quaternion.RotateTowards(transform.rotation, _lookRotation, 135f * Time.deltaTime);
        
    }
}