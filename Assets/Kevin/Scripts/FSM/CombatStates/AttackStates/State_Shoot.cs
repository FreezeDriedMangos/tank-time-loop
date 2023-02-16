using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Shoot : IState
{
    public AIController myAIC;
    private float startTime;
    
    public FSM SetupSubFSM()
    {
        return null;
    }

    public void Enter() 
    {
        Debug.Log("Entered State_Shoot");
        startTime = myAIC.CurrentTime();
        myAIC.Shoot();
    } 
    public void Update() {} 
    public void Exit() 
    {
        Debug.Log("Exited State_Shoot");
    } 

    public AIController aic
    {
        get{ return myAIC; }
        set{ myAIC = value; } 
    }
    public IState ShouldExitTo() 
    {   
        if (myAIC.TransitionTimerUp("shoot to moveRandomly", startTime))
            return new State_Move_Randomly(); 

        return null;
    }
}
