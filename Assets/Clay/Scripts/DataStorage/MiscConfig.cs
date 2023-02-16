using Newtonsoft.Json.Linq;
using System;

[Serializable]
public class MiscConfig 
{
    public bool JoyconMotionDampening;

    public bool hologramsLeaveTracks;
    public bool tracksGetClearedBetweenRounds;

    public bool fireShellMarkers;
    public bool fireRocketMarkers;
    public bool fireBombMarkers;
    public bool deathMarkers;
    public bool bombExplosionMarkers;

    public float normalTracksOpacity;
    public float specialTracksOpacity;
    public float explosionMarkerOpacity;


    public float bombLaunchElevationAngle;
    public float bombLaunchPower;
    public float explosionDuration_seconds;
    public float explosionExpandTargetSize;
    public float explosionExpandSpeed;
    public float explosionTankDestructionTimer_seconds;

    public float bigger_explosionDuration_seconds;
    public float bigger_explosionExpandTargetSize;
    public float bigger_explosionExpandSpeed;
    
    public float occlusionOverlayOpacity;

    public float rogueOnPushDistance;
    public bool rogueOnEarlyDeath;

    public bool kothModeScoreBasedOnTotalScore;
}
