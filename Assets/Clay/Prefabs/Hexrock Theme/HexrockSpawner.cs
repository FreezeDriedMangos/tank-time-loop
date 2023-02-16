using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class HexrockSpawner : MonoBehaviour
{

    List<GameObject> hexrocks = new List<GameObject>();
    public GameObject hexrockPrefab;
    //public float hexrockWidth;
    //public float hexrockDepth;
    public float hexrockSize; // measurement from center to a point

    public float minHexrockHeight = 0.9f;
    public float maxHexrockHeight = 1f;

    public bool generateHexrocks = true;

    public Material[] materialVariants;
    

    // Start is called before the first frame update
    void Start()
    {
        SpawnHexrocks();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (generateHexrocks)
        {
            generateHexrocks = false;
            SpawnHexrocks();
        }
    }


    public void SpawnHexrocks()
    {
        //hexrockWidth = 2.0f * hexrockSize;
        //hexrockHeight = (float)Mathf.Sqrt(3.0) * hexrockSize;

        foreach(GameObject g in hexrocks)
            DestroyImmediate(g);
        
        hexrocks.Clear();

        //
        // hexrocks will appear at (w*5/4 + k*(6/4)*w, h/2 + 2K*h) and (w/2 + k*w*3/2, Kh)
        // for all integer values of k and K
        //

        // minimum x is (x-0.5) 
        // maximum x is (x+0.5) 
        // minimum for class 1 hexagons
        // x-0.5 <= w*5/4 + k*(6/4)*w
        // x-0.5 - w*5/4 <= k*(6/4)*w
        // (x-0.5 - w*5/4)/((6/4)*w) <= k
        //
        // minimum for class 1 hexagons
        // (x+0.5 - w*5/4)/((6/4)*w) >= k
        //
        // so for class 1 hexagons, k ranges from ceiling((x-0.5 - w/2)/((6/4)*w)) to floor((x+0.5 - w/2)/((6/4)*w))
        //
        // y values
        // y-0.5 <= h/2 + 2K*h
        // y-0.5 - h/2 <= 2K*h
        // (y-0.5 - h/2)/(2h) <= K
        //
        // (y+0.5 - h/2)/(2h) >= K
        // so for class 1 hexagons, K ranges from ceiling((y-0.5 - h/2)/(2h)) to floor((y+0.5 - h/2)/(2h))

        float x = this.transform.position.x;
        float z = this.transform.position.z;
        float w = 2.0f * hexrockSize; //hexrockWidth;
        float h = (float)Mathf.Sqrt(3.0f) * hexrockSize; //hexrockHeight;

        float class1minI = (float)(  (  x-0.5 - w*5.0/4.0  ) / (  (6.0/4.0)*w  )  );
        float class1maxI = (float)(  (  x+0.5 - w*5.0/4.0  ) / (  (6.0/4.0)*w  )  );
        
        float class1minJ = (float)(  ( z-0.5 - h/2.0 )/( h )  );
        float class1maxJ = (float)(  ( z+0.5 - h/2.0 )/( h )  );

        for(int i = (int)Mathf.Ceil (class1minI); i <= class1maxI; i++)
        {
            x = (float)(w*5.0/4.0 + i*(6.0/4.0)*w);

            for(int j = (int)Mathf.Ceil (class1minJ); j <= class1maxJ; j++)
            {
                z = (float)(h/2.0 + j*h); // problem with this, or with class1minJ

                GameObject g = Instantiate(hexrockPrefab);
                g.transform.position = new Vector3(x, this.transform.position.y, z);
                g.transform.parent = this.transform;
                g.transform.localScale = new Vector3(1, Random.Range(minHexrockHeight, maxHexrockHeight), 1);
                hexrocks.Add(g);

                SelectColor(g, 1, i, j);
            }
        }

        // minimum x for class 2 hexagons
        //
        // x-0.5 <= w/2 + i*w*3/2
        // (x-0.5 - w/2) / (w*3/2) <= i
        //
        // maximum x for class 2 hexagons
        // (x+0.5 - w/2) / (w*3/2) >= i
        //
        // minimum y
        // y-0.5 <= jh 
        // (y-0.5)/h <= j
        //
        // (y+0.5)/h >= j 
        x = this.transform.position.x;
        z = this.transform.position.z;

        float class2minI = (float)(  (x-0.5 - w/2.0) / (w*3.0/2.0)  );
        float class2maxI = (float)(  (x+0.5 - w/2.0) / (w*3.0/2.0)  );
        
        float class2minJ = (float)(  (z-0.5)/h  );
        float class2maxJ = (float)(  (z+0.5)/h  );

        for(int i =  (int)Mathf.Ceil (class2minI); i <= class2maxI; i++)
        {
            x = (float)(w/2.0 + i*w*3.0/2.0);

            for(int j =  (int)Mathf.Ceil (class2minJ); j <= class2maxJ; j++)
            {
                z = (float)(j*h);

                GameObject g = Instantiate(hexrockPrefab);
                g.transform.position = new Vector3(x, this.transform.position.y, z);
                g.transform.parent = this.transform;
                g.transform.localScale = new Vector3(1, Random.Range(minHexrockHeight, maxHexrockHeight), 1);
                hexrocks.Add(g);

                SelectColor(g, 2, i, j);
            }
        }
    }

    void SelectColor(GameObject g, int classNum, int i, int j)
    {
        // give the hexrock a "random" color according to its i,j position
        string posStr = GameState.levelName +": hexrock class " + classNum + " ("+ i + ", " + j+")";
        int matIdx = (int)Mathf.Abs(posStr.GetHashCode()/7f) % (materialVariants.Length);
        g.transform.Find("HexagonCylinder").GetComponent<MeshRenderer>().material = materialVariants[matIdx];
    }
}
