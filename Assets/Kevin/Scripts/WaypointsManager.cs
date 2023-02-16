using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointsManager : MonoBehaviour
{
	Queue<int[]> availableWaypoints;

    // Start is called before the first frame update
    void Start()
    {
    	availableWaypoints = new Queue<int[]>();
        availableWaypoints.Enqueue(new int[] {1, 1});
        availableWaypoints.Enqueue(new int[] {3, 3});
        availableWaypoints.Enqueue(new int[] {-3, -3});
        availableWaypoints.Enqueue(new int[] {-2, -2});
        availableWaypoints.Enqueue(new int[] {2, 2});
        availableWaypoints.Enqueue(new int[] {2, -3});
        availableWaypoints.Enqueue(new int[] {-3, 2});
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FreeWaypoint(int[] waypoint)
    {
    	availableWaypoints.Enqueue(waypoint);
    }

    public int[] AssignWaypoint()
    {
    	return availableWaypoints.Dequeue();
    }
}
