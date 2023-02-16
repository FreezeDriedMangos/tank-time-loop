using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DottedLineController : MonoBehaviour
{
    private Vector2 start;
    private Vector2 end;
    
    List<GameObject> dots = new List<GameObject>();

    public GameObject dotPrefab;

    public int numDots;

    public TankController tank;

    public void Setup()
    {
        Delete();

        RectTransform t = GetComponent<RectTransform>();

        for (int i = 0; i < numDots; i++)
        {
            GameObject g = GameObject.Instantiate(dotPrefab);
            g.name = "dot " + i;
            g.GetComponent<RectTransform>().SetParent(t, true);

            dots.Add(g);
        }
    }

    public void Delete() 
    {
        foreach (GameObject dot in dots)
        {
            GameObject.Destroy(dot);
        }

        dots.Clear();
    }

    public void SetStartAndEnd(Vector2 start, Vector2 end)
    {
        Vector2 dir = end - start;
        dir = dir.normalized;

        float dist = Vector2.Distance(start, end);
        float distUnit = dist / dots.Count;

        for (int i = 0; i < dots.Count; i++)
        {
            dots[i].transform.position = start + dir*i*distUnit;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // if the tank gets destroyed, never render a line again
        if (tank != null && tank.destroyed)
        {
            Delete();
            this.enabled = false;
        }
    }
}
