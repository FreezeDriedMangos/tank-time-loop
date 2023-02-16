using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Idle : IState
{
    public AIController myAIC;
    private float startTime;

    public FSM SetupSubFSM()
    {
        return null;
    }

    public void Enter() 
    {
        startTime = myAIC.CurrentTime();
        myAIC.StartIdle();
    } 
    public void Update() {} 
    public void Exit() {} 

    public AIController aic
    {
        get{ return myAIC; }
        set{ myAIC = value; } 
    }
    public IState ShouldExitTo() 
    {   
        if (myAIC.TransitionTimerUp("idle to patrolling", startTime))
            return new State_Patrolling();
        return null; 
    }
}
