using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Patrolling : IState
{
    private float time; 
    public AIController myAIC;
    public FSM SetupSubFSM()
    {
        return new FSM(new State_Pick_Target(), aic);
    }

    public void Enter() 
    {
        time = aic.CurrentTime(); 
    }
    public void Update() {} 
    public void Exit() 
    {
        myAIC.StopPathfinding();
    } 

    public AIController aic
    {
        get{ return myAIC; }
        set{ myAIC = value; } 
    }

    public IState ShouldExitTo() 
    {     
        // if (myAIC.TransitionProbability("patrolling to findGroup") && 
        //     myAIC.FriendlyTankNearbyWithLineOfSight())         
        //     return new State_Form_Group();     
        if (myAIC.TransitionTimerUp("Patrolling to Idle", time))          
            return new State_Idle();   
    
        return null; 
    }
}
