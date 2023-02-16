using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Move_Randomly : IState
{
    public AIController myAIC;
    private float startTime;
    public FSM SetupSubFSM()
    {
        return null;
    }

    public void Enter() 
    {
        Debug.Log("Entered State_Move_Randomly");
        startTime = myAIC.CurrentTime();
        myAIC.MoveRandomly();
    } 
    public void Update() {} 
    public void Exit() 
    {
        Debug.Log("Exited State_Move_Randomly");
        myAIC.StopPathfinding();
    } 

    public AIController aic
    {
        get{ return myAIC; }
        set{ myAIC = value; } 
    }
    public IState ShouldExitTo() 
    {     
        if (myAIC.TransitionTimerUp("moveRandomly to aim", startTime))
            return new State_Aim();
        return null; 
    }
}
