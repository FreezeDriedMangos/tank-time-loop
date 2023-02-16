using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OcclusionSillhouetteController : MonoBehaviour
{
    public Material mat;
    public Camera occludablesOnlyCamera;
    public Camera occludersOnlyCamera;
    public Camera occludersAndOccludablesCamera;

    public Shader occlusionSillhouetteHelper;

    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    RenderTexture makeCameraRenderTex(Camera camera) 
    {
        if (camera.targetTexture != null) return camera.targetTexture;
     
        RenderTexture r = new RenderTexture(Screen.width, Screen.height, 24);
        r.Create();
        camera.targetTexture = r;

        return r;
    }

    void Setup() 
    {
        mat.SetTexture("_ObjectsOnly",     makeCameraRenderTex(occludablesOnlyCamera));
        mat.SetTexture("_CoverOnly",       makeCameraRenderTex(occludersOnlyCamera));
        mat.SetTexture("_ObjectsAndCover", makeCameraRenderTex(occludersAndOccludablesCamera));
        
        //occludablesOnlyCamera.        SetReplacementShader(occlusionSillhouetteHelper, "OcclusionSilhouette");
        //occludersOnlyCamera.          SetReplacementShader(occlusionSillhouetteHelper, "OcclusionSilhouette");
        //occludersAndOccludablesCamera.SetReplacementShader(occlusionSillhouetteHelper, "OcclusionSilhouette");
        
        RectTransform prt = this.transform.parent.GetComponent<RectTransform>();
        this.GetComponent<RectTransform>().sizeDelta = new Vector2(prt.rect.width, prt.rect.height);
        
        SetupConfig();
    }

    void SetupConfig()
    {
        if (GameState.config == null || GameState.config.miscConfig == null) return;


        Color c = new Color(0, 0, 0, GameState.config.miscConfig.occlusionOverlayOpacity);
        mat.SetColor("_Color", c);
    }
}
