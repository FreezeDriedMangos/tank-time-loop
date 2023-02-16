using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class NavigationBaker : MonoBehaviour
{
    public NavMeshSurface surface;

    public float soon_inSeconds = 1; // the lower this is, the sooner the navmesh will be updated after a level event
    public float soonEnoughThreshold_inSeconds = 0.1f; // the higher this is, the fewer times a navmesh will need to be updated
                                                       // however, if it's >= soon_inSeconds it will more or less break stuff

                                                       // any events that happen within soonEnoughThreshold_inSeconds of eachother
                                                       // will only cause the navmesh to be rebuilt once
    public List<float> timers = new List<float>(); 

    // Start is called before the first frame update
    void Start()
    {
        surface.BuildNavMesh();
    }

    public void RebuildNavMesh()
    {
        surface.BuildNavMesh();
    }

    public void RebuildNavMeshSoon()
    {
        timers.Add(soon_inSeconds);
    }

    public void FixedUpdate()
    {
        if (timers.Count > 0)
        {
            List<int> removeAtIdxs = new List<int>();
            bool rebuild = false;

            for(int i = 0; i < timers.Count; i++)
            {
                timers[i] -= Time.deltaTime;
                if (timers[i] <= 0)
                {
                    rebuild = true;
                    removeAtIdxs.Add(i);
                }
            }

            // if no timers were finished, there's nothing more to do
            if (!rebuild) return;

            // if we're rebuilding, check to see if any timers are close enough to zero to eliminate them
            if (rebuild)
                for(int i = 0; i < timers.Count; i++)
                    if (timers[i] <= soonEnoughThreshold_inSeconds)
                        removeAtIdxs.Add(i);
            
            // removeAtIdxs needs to be in strictly decreasing order
            // with no duplicates
            removeAtIdxs = removeAtIdxs.Distinct().ToList();
            removeAtIdxs.Sort();
            removeAtIdxs.Reverse();

            // these timers are done, remove them
            foreach (int idx in removeAtIdxs)
            {
                timers.RemoveAt(idx);
            }

            if (rebuild) RebuildNavMesh();
        }
    }
}
