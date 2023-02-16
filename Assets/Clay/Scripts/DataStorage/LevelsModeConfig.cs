
using Newtonsoft.Json.Linq;
using System;

[Serializable]
public class LevelsModeConfig 
{
    public float preLevelTime_seconds;
    public float bannerExitAnimationLength_seconds;
    public float levelPreviewTime_seconds;
    public float gameOverGraceTime_seconds;
    public float levelEndFreezeFrameTime_seconds;
    public int maxNumLives;

    public float respawnFreezeTime_seconds;
    public float respawnInvincibilityTime_seconds;

    public int cheat_worldToStartOn;
    public int cheat_levelToStartOn;
}
