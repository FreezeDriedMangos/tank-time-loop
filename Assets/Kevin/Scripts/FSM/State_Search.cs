using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Search : IState
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
        myAIC.OpponentInLoS();
    } 
    public void Update() 
    {
        if (myAIC.ReachedPathfindTarget())
            myAIC.PathfindToTargetedTank();
    } 
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
        if (myAIC.TransitionTimerUp("restart search", startTime))
            return new State_Search();

        if (myAIC.OpponentInLoS())
            return new State_Combat();
        return null; 
    }
}
