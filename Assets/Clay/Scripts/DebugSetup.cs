using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSetup : MonoBehaviour
{
    PlayersManager pm;
    public int numPlayers = 1;

    private int numPlayersIn = 0;

    private bool setup = false;

    public GameObject playerTankPrefab;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlayerJoin()
    {
        numPlayersIn++;

        if (numPlayersIn == numPlayers)
            Setup();
    }

    public void Setup()
    {
        if (setup) return;
        setup = true;

        //GameState.config = new Config(Newtonsoft.Json.Linq.JObject.Parse(DEFAULT_CONFIG));
        
        GameState.config = JsonUtility.FromJson<Config>(DEFAULT_CONFIG);
        
        //GameState.config = new Config();
        //GameState.config.miscConfig = new MiscConfig();
        //GameState.config.sfxConfigs = new SFXConfig[0];

        pm = GetComponent<PlayersManager>();

        GameState.players = GameObject.FindObjectsOfType<Player>();
        GameState.playerTankPrefabs = new GameObject[4]{playerTankPrefab, playerTankPrefab, playerTankPrefab, playerTankPrefab};
        GameState.playerJoined = new bool[]{true, false, false, false};
        //GameState.numPlayers = numPlayers;
        pm.Setup();
        pm.SetUpPlayers(1);
    
        Debug.Log("HIIIIIIIIIIIIIIIIII" + GameObject.Find("p1."+(0+1)));
    }

    public const string DEFAULT_CONFIG = "{\"cameraConfigForBaseGameplay\":{\"widthBump\":2,\"heightBump\":2,\"zBump\":0,\"distanceBump\":2},\"levelBuildingConfig\":{\"makeBoundaries\":false},\"gimmicksConfig\":{\"elevator_raiseSpeed\":0.2},\"miscConfig\":{\"hologramsLeaveTracks\":false,\"tracksGetClearedBetweenRounds\":false,\"fireShellMarkers\":true,\"fireRocketMarkers\":true,\"fireBombMarkers\":true,\"deathMarkers\":true,\"bombExplosionMarkers\":true,\"normalTracksOpacity\":0.35,\"specialTracksOpacity\":0.35,\"explosionMarkerOpacity\":0.5,\"explosionDuration_seconds\":1,\"explosionExpandTargetSize\":2,\"explosionExpandSpeed\":0.5,\"bigger_explosionDuration_seconds\":1,\"bigger_explosionExpandTargetSize\":3,\"bigger_explosionExpandSpeed\":0.5,\"occlusionOverlayOpacity\":0.203125,\"rogueOnPushDistance\":0.75,\"rogueOnEarlyDeath\":false},\"vsModeConfig\":{\"roundLength_seconds\":50,\"roundCooldown_seconds\":3,\"roundEndLength_seconds\":2,\"maxNumRounds\":5,\"countdownWhenXSecondsLeftInRound\":10},\"levelsModeConfig\":{\"preLevelTime_seconds\":1.5,\"bannerExitAnimationLength_seconds\":0.25,\"levelPreviewTime_seconds\":1.5,\"gameOverGraceTime_seconds\":10,\"levelEndFreezeFrameTime_seconds\":1,\"maxNumLives\":10,\"respawnFreezeTime_seconds\":0.5,\"respawnInvincibilityTime_seconds\":0.75,\"cheat_worldToStartOn\":1,\"cheat_levelToStartOn\":1},\"sfxConfigs\":[{\"name\":\"tank_whirrSound\",\"volume\":1,\"pitchMin\":0.8,\"pitchMax\":1.3},{\"name\":\"tank_moveSound\",\"volume\":1,\"pitchMin\":0.8,\"pitchMax\":1.3},{\"name\":\"tank_turnSound\",\"volume\":1,\"pitchMin\":0.8,\"pitchMax\":1.3},{\"name\":\"tank_destroySound\",\"volume\":1,\"pitchMin\":0.8,\"pitchMax\":1.3},{\"name\":\"tank_fireSound\",\"volume\":1,\"pitchMin\":0.8,\"pitchMax\":1.3},{\"name\":\"shell_bounceSound\",\"volume\":1,\"pitchMin\":0.8,\"pitchMax\":1.3},{\"name\":\"shell_destroySound\",\"volume\":1,\"pitchMin\":0.8,\"pitchMax\":1.3},{\"name\":\"rocket_bounceSound\",\"volume\":1,\"pitchMin\":0.8,\"pitchMax\":1.3},{\"name\":\"rocket_destroySound\",\"volume\":1,\"pitchMin\":0.8,\"pitchMax\":1.3},{\"name\":\"bomb_explodeSound\",\"volume\":1,\"pitchMin\":0.8,\"pitchMax\":1.3}],\"defaultTankConfig\":{\"team\":\"NONE\",\"type\":\"GENERIC\",\"turnSpeed\":90,\"moveSpeed\":80,\"fireShellCooldown_inSeconds\":1,\"fireRocketCooldown_inSeconds\":5,\"fireBombCooldown_inSeconds\":2,\"freezeTimeAfterFireShell_inSeconds\":0.25,\"freezeTimeAfterFireRocket_inSeconds\":0.5,\"freezeTimeAfterFireBomb_inSeconds\":0.5,\"shellCountLimit\":10,\"rocketCountLimit\":10,\"bombCountLimit\":15,\"faceFrontBias_Degrees\":0,\"turningRadius_inDegrees\":0,\"aiConfig\":{\"firesRockets\":false,\"understandsBouncing\":false,\"bounceIterationsPerDirection\":0,\"dodgeProbability\":0.9,\"findGroupProbability\":0.2,\"moveRandomlyToAimTimer\":1,\"giveUpSearchTimer\":1,\"stayInGroupTimer\":7.5,\"pickNewBuddyTimer\":3.5,\"idleTimer\":1.5,\"patrollingTimer\":10,\"recalcSearchTimer\":0.8,\"closeEnoughToBecomeBuddiesDistance\":3,\"bulletGonnaHitSightRadius\":3},\"leaveTracks\":true,\"leaveTracksEveryXSeconds\":0.1,\"leaveTracksEveryXUnitsDriven\":10},\"tankConfigs\":[{\"team\":\"NONE\",\"type\":\"PLAYER\",\"turnSpeed\":140,\"moveSpeed\":120,\"fireShellCooldown_inSeconds\":0.25,\"fireRocketCooldown_inSeconds\":0,\"fireBombCooldown_inSeconds\":1,\"freezeTimeAfterFireShell_inSeconds\":0.25,\"freezeTimeAfterFireRocket_inSeconds\":0.5,\"freezeTimeAfterFireBomb_inSeconds\":0.5,\"shellCountLimit\":5,\"rocketCountLimit\":1,\"bombCountLimit\":1,\"faceFrontBias_Degrees\":60,\"turningRadius_inDegrees\":20,\"aiConfig\":{\"firesRockets\":false,\"understandsBouncing\":true,\"bounceIterationsPerDirection\":5,\"dodgeProbability\":0.9,\"findGroupProbability\":0.2,\"moveRandomlyToAimTimer\":1,\"giveUpSearchTimer\":3,\"stayInGroupTimer\":1,\"pickNewBuddyTimer\":1,\"idleTimer\":1,\"patrollingTimer\":10,\"recalcSearchTimer\":0.8,\"closeEnoughToBecomeBuddiesDistance\":1,\"bulletGonnaHitSightRadius\":3},\"leaveTracks\":true,\"leaveTracksEveryXSeconds\":0.1,\"leaveTracksEveryXUnitsDriven\":10},{\"team\":\"NONE\",\"type\":\"STILL_ENEMY\",\"turnSpeed\":0,\"moveSpeed\":0,\"fireShellCooldown_inSeconds\":5,\"fireRocketCooldown_inSeconds\":5,\"fireBombCooldown_inSeconds\":2,\"freezeTimeAfterFireShell_inSeconds\":0.25,\"freezeTimeAfterFireRocket_inSeconds\":0.5,\"freezeTimeAfterFireBomb_inSeconds\":0.5,\"shellCountLimit\":3,\"rocketCountLimit\":1,\"bombCountLimit\":3,\"faceFrontBias_Degrees\":0,\"turningRadius_inDegrees\":0,\"aiConfig\":{\"firesRockets\":false,\"understandsBouncing\":true,\"bounceIterationsPerDirection\":5,\"dodgeProbability\":0,\"findGroupProbability\":0,\"moveRandomlyToAimTimer\":0,\"giveUpSearchTimer\":0,\"stayInGroupTimer\":0,\"pickNewBuddyTimer\":0,\"idleTimer\":0.1,\"patrollingTimer\":1,\"recalcSearchTimer\":0.8,\"closeEnoughToBecomeBuddiesDistance\":0,\"bulletGonnaHitSightRadius\":3},\"leaveTracks\":true,\"leaveTracksEveryXSeconds\":0.1,\"leaveTracksEveryXUnitsDriven\":10},{\"team\":\"NONE\",\"type\":\"STANDARD_ENEMY\",\"turnSpeed\":90,\"moveSpeed\":80,\"fireShellCooldown_inSeconds\":5,\"fireRocketCooldown_inSeconds\":5,\"fireBombCooldown_inSeconds\":2,\"freezeTimeAfterFireShell_inSeconds\":0.25,\"freezeTimeAfterFireRocket_inSeconds\":0.5,\"freezeTimeAfterFireBomb_inSeconds\":0.5,\"shellCountLimit\":3,\"rocketCountLimit\":1,\"bombCountLimit\":3,\"faceFrontBias_Degrees\":0,\"turningRadius_inDegrees\":0,\"aiConfig\":{\"firesRockets\":false,\"understandsBouncing\":false,\"bounceIterationsPerDirection\":0,\"dodgeProbability\":0.9,\"findGroupProbability\":0.2,\"moveRandomlyToAimTimer\":1,\"giveUpSearchTimer\":1,\"stayInGroupTimer\":7.5,\"pickNewBuddyTimer\":3.5,\"idleTimer\":1.5,\"patrollingTimer\":10,\"recalcSearchTimer\":0.8,\"closeEnoughToBecomeBuddiesDistance\":3,\"bulletGonnaHitSightRadius\":3},\"leaveTracks\":true,\"leaveTracksEveryXSeconds\":0.1,\"leaveTracksEveryXUnitsDriven\":10},{\"team\":\"NONE\",\"type\":\"ROCKET_ENEMY\",\"turnSpeed\":90,\"moveSpeed\":80,\"fireShellCooldown_inSeconds\":5,\"fireRocketCooldown_inSeconds\":5,\"fireBombCooldown_inSeconds\":2,\"freezeTimeAfterFireShell_inSeconds\":0.25,\"freezeTimeAfterFireRocket_inSeconds\":0.5,\"freezeTimeAfterFireBomb_inSeconds\":0.5,\"shellCountLimit\":3,\"rocketCountLimit\":1,\"bombCountLimit\":3,\"faceFrontBias_Degrees\":0,\"turningRadius_inDegrees\":0,\"aiConfig\":{\"firesRockets\":true,\"understandsBouncing\":false,\"bounceIterationsPerDirection\":0,\"dodgeProbability\":0.2,\"findGroupProbability\":0.1,\"moveRandomlyToAimTimer\":1,\"giveUpSearchTimer\":2,\"stayInGroupTimer\":3,\"pickNewBuddyTimer\":1.5,\"idleTimer\":1,\"patrollingTimer\":15,\"recalcSearchTimer\":0.8,\"closeEnoughToBecomeBuddiesDistance\":3,\"bulletGonnaHitSightRadius\":3},\"leaveTracks\":true,\"leaveTracksEveryXSeconds\":0.1,\"leaveTracksEveryXUnitsDriven\":10},{\"team\":\"NONE\",\"type\":\"BOMB_ENEMY\",\"turnSpeed\":0,\"moveSpeed\":0,\"fireShellCooldown_inSeconds\":5,\"fireRocketCooldown_inSeconds\":5,\"fireBombCooldown_inSeconds\":2,\"freezeTimeAfterFireShell_inSeconds\":0.25,\"freezeTimeAfterFireRocket_inSeconds\":0.5,\"freezeTimeAfterFireBomb_inSeconds\":0.5,\"shellCountLimit\":3,\"rocketCountLimit\":1,\"bombCountLimit\":3,\"faceFrontBias_Degrees\":0,\"turningRadius_inDegrees\":0,\"aiConfig\":{\"firesRockets\":false,\"understandsBouncing\":false,\"bounceIterationsPerDirection\":0,\"dodgeProbability\":0.9,\"findGroupProbability\":0.2,\"moveRandomlyToAimTimer\":1,\"giveUpSearchTimer\":1,\"stayInGroupTimer\":7.5,\"pickNewBuddyTimer\":3.5,\"idleTimer\":1.5,\"patrollingTimer\":10,\"recalcSearchTimer\":0.8,\"closeEnoughToBecomeBuddiesDistance\":3,\"bulletGonnaHitSightRadius\":3},\"leaveTracks\":true,\"leaveTracksEveryXSeconds\":0.1,\"leaveTracksEveryXUnitsDriven\":10},{\"team\":\"NONE\",\"type\":\"FAST_ENEMY\",\"turnSpeed\":150,\"moveSpeed\":130,\"fireShellCooldown_inSeconds\":5,\"fireRocketCooldown_inSeconds\":5,\"fireBombCooldown_inSeconds\":2,\"freezeTimeAfterFireShell_inSeconds\":0.25,\"freezeTimeAfterFireRocket_inSeconds\":0.5,\"freezeTimeAfterFireBomb_inSeconds\":0.5,\"shellCountLimit\":3,\"rocketCountLimit\":1,\"bombCountLimit\":3,\"faceFrontBias_Degrees\":0,\"turningRadius_inDegrees\":0,\"aiConfig\":{\"firesRockets\":false,\"understandsBouncing\":true,\"bounceIterationsPerDirection\":2,\"dodgeProbability\":0.9,\"findGroupProbability\":0.05,\"moveRandomlyToAimTimer\":1,\"giveUpSearchTimer\":1,\"stayInGroupTimer\":3,\"pickNewBuddyTimer\":0.1,\"idleTimer\":2,\"patrollingTimer\":10,\"recalcSearchTimer\":0.8,\"closeEnoughToBecomeBuddiesDistance\":5,\"bulletGonnaHitSightRadius\":3},\"leaveTracks\":true,\"leaveTracksEveryXSeconds\":0.1,\"leaveTracksEveryXUnitsDriven\":10}]}";
}
