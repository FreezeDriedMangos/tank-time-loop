using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    Transform aimLocation;
    public TankTransform tank;
    public float distToCamera;
    private RectTransform parentTransform;
    private static Vector3 UP = new Vector3(0,1,0);

    private bool setup = false;

    public Vector2 currentPos {get; private set;}

    private DottedLineController line;

    // void Update()
    // {
    //     Debug.Log("hi");
    //     Vector2 tankLoc = Camera.main.WorldToScreenPoint(tank.ShellSpawnLocation);
    //     line.SetStartAndEnd(tankLoc, currentPos);
    // }

    // Start is called before the first frame update
    void Start()
    {
        //Setup();
    }

    public void Setup()
    {
        if (setup) return;
        setup = true;

        //GameObject parent = new GameObject("Player Cursor");
        //this.transform.parent = parent.transform;


        aimLocation = new GameObject("aim location").transform;
        //aimLocation.parent = parent.transform;

        Canvas c = transform.Find("/Canvas").GetComponent<Canvas>();//GameObject.FindObjectsOfType<Canvas>()[0];
        transform.parent = c.transform;
        transform.position = currentPos = Vector3.zero;
        transform.eulerAngles = Vector3.zero;   
        GetComponent<RectTransform>().localPosition = Vector3.zero;

        currentPos = this.transform.position;

        parentTransform = transform.parent.GetComponent<RectTransform>();

        line = GetComponent<DottedLineController>();
        if (tank != null)
        {
            line.Setup();
            line.tank = tank.GetComponent<TankController>();
        }
    }

    public void MovePos(Vector2 deltaScreenPos)
    {
        currentPos += deltaScreenPos;

        currentPos = new Vector2(Mathf.Clamp(currentPos.x, 0, parentTransform.rect.width), Mathf.Clamp(currentPos.y, 0, parentTransform.rect.height));

        SetPos(currentPos);
    }

    public void SetPos(Vector2 screenPos)
    {
        currentPos = screenPos;

        // if (tank == null) 
        // {
        //     GameObject.Destroy(this.gameObject);
        //     return;
        // }

        // set the visual cursor location (me)
        this.transform.position = screenPos;

        // all the below code is for aiming the tank
        if (tank == null) return;

        // prepare for some radical geometry
        Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Camera.main.nearClipPlane));
        Vector3 cameraPos = Camera.main.transform.position;

        // derrived from intersection of line and plane
        Vector3 dir = pos - cameraPos;
        dir = dir.normalized;
        
        Vector3 pointOnPlane = tank.ShellSpawnLocation; // a point on the plane we want the cursor to be on
                                                        // since the plane we want is horizontal (it's normal is just UP)
                                                        // All that matters for pointOnPlane is the y value
        float d = Vector3.Dot(pointOnPlane - pos, UP) / Vector3.Dot(dir, UP);
        aimLocation.position = pos + dir*d;


        // update the line
        Vector2 tankLoc = Camera.main.WorldToScreenPoint(tank.ShellSpawnLocation);
        line.SetStartAndEnd(tankLoc, currentPos);
    }

    public Vector2 GetAimLocation()
    {
        return new Vector2(aimLocation.position.x, aimLocation.position.z);
    }
}
