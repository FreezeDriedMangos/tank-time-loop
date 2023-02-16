
using Newtonsoft.Json.Linq;
using System;
using UnityEngine;

[Serializable]
public class TankConfig
{
    //public TankController.Team team;
    //public TankController.Type type;

    public string team;
    public string type;

    
        // team = (TankController.Team) Enum.Parse(typeof(TankController.Team), team, true);
        // type = (TankController.Type) Enum.Parse(typeof(TankController.Type), type, true);

        // if (!Enum.IsDefined(typeof(TankController.Team), team)) Debug.LogError("In Config File: Invalid tank team \""+team+"\"");
        // if (!Enum.IsDefined(typeof(TankController.Type), type)) Debug.LogError("In Config File: Invalid tank type \""+team+"\"");
        
    
    public float turnSpeed;
    public float moveSpeed;

    
    public float fireShellCooldown_inSeconds;
    public float fireRocketCooldown_inSeconds;
    public float fireBombCooldown_inSeconds;

    public float freezeTimeAfterFireShell_inSeconds;
    public float freezeTimeAfterFireRocket_inSeconds;
    public float freezeTimeAfterFireBomb_inSeconds;

    public int shellCountLimit;
    public int rocketCountLimit;
    public int bombCountLimit;

    
    public float faceFrontBias_Degrees;
    public float turningRadius_inDegrees; 

    public bool leaveTracks;
    public float leaveTracksEveryXSeconds;
    public float leaveTracksEveryXUnitsDriven;

    public AIConfig aiConfig;
}
