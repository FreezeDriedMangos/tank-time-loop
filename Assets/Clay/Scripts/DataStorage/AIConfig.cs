
using Newtonsoft.Json.Linq;
using System;

[Serializable]
public class AIConfig
{
    public bool firesRockets;

    
    public bool understandsBouncing;

    

    public int bounceIterationsPerDirection; // = 5;
    

    public float dodgeProbability; // = 0.9f;
    public float findGroupProbability; // = 0.1f;

    
    public float moveRandomlyToAimTimer;
    public float giveUpSearchTimer;
    public float stayInGroupTimer;
    public float pickNewBuddyTimer;
    public float idleTimer;
    public float patrollingTimer;
    public float recalcSearchTimer;

    
    public float closeEnoughToBecomeBuddiesDistance; // = 2f;

    public float bulletGonnaHitSightRadius;


    public float bombSightRangeMax;
    public float bombSightRangeMin;
    public bool shootsBombs;
    public bool shootsBombsOnly;

}
