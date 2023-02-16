
using Newtonsoft.Json.Linq;
using System;

[Serializable]
public class Config
{

    //
    // JSON
    //

    public CameraConfig cameraConfigForBaseGameplay;
    
    public LevelBuildingConfig levelBuildingConfig;

    public GimmicksConfig gimmicksConfig;
    
    public MiscConfig miscConfig;

    public VsModeConfig vsModeConfig;
    public LevelsModeConfig levelsModeConfig;

    public SFXConfig[] sfxConfigs;

    public TankConfig defaultTankConfig;

    public TankConfig[] tankConfigs;


    //
    // JSON
    //

}
