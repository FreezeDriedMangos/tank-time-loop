using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this component assumes the camera is only rotated on the x axis
public class CameraSetter : MonoBehaviour
{
    Camera camera;
    private bool setup = false;

    private string[,,] lastLoadedLevel;
    private float lastAspect;

    public float widthBump    = 2;
    public float heightBump   = 2;
    public float zBump        = 0;//-0.5f;
    public float distanceBump = 2;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Setup();
    }

    // this turned out to all be some pretty simple trig, thanks to 
    // https://docs.unity3d.com/Manual/FrustumSizeAtDistance.html
    // which does all the heavy lifting. All I needed to do was
    // calculate the desired frustrum width/height at the lowest floor of
    // the level (based on the level width/height) and then that
    // link gave me the distance the camera needed to be from the
    // middle of the lowest floor
    void Setup()
    {
        if (camera == null) camera = GetComponent<Camera>();
        if (GameState.loadedLevel == null) return;
        if (GameState.loadedLevel == lastLoadedLevel && camera.aspect == lastAspect) return;

        SetupConfig();

        lastLoadedLevel = GameState.loadedLevel;
        lastAspect = camera.aspect;

        //if (setup) return;
        //setup = true;


        float levelWidth = GameState.LevelWidth; 
        float levelHeight = GameState.LevelHeight;

        float desiredVisibleWidth  = levelWidth+widthBump;
        float desiredVisibleHeight = levelHeight+heightBump;


        float desiredFrustrumWidth = desiredVisibleWidth;
        float desiredFrustrumHeight = desiredVisibleHeight * Mathf.Sin(Mathf.Deg2Rad * camera.transform.eulerAngles.x); // basically a vector projection, but with trig instead
        

        //https://docs.unity3d.com/Manual/FrustumSizeAtDistance.html
        // float distanceH = desiredFrustrumHeight                * 0.5 / Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        // float distanceW = (desiredFrustrumWidth/camera.aspect) * 0.5 / Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        // above made more efficient

        float distance = Mathf.Max(desiredFrustrumHeight, desiredFrustrumWidth/camera.aspect) * 0.5f / Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);

        int whichFloorToBaseLocationOn = 0;
        camera.transform.position = new Vector3(levelWidth/2f, whichFloorToBaseLocationOn, zBump-levelHeight/2f) - camera.transform.forward * (distance + distanceBump);
    }

    private bool configSetup = false;
    void SetupConfig()
    {
        if (configSetup) return;
        configSetup = true;

        widthBump    = GameState.config.cameraConfigForBaseGameplay.widthBump; //2;
        heightBump   = GameState.config.cameraConfigForBaseGameplay.heightBump; // 2;
        zBump        = GameState.config.cameraConfigForBaseGameplay.zBump; // 0;//-0.5f;
        distanceBump = GameState.config.cameraConfigForBaseGameplay.distanceBump; // 2;
    
    }
}
