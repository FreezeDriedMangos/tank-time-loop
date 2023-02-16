using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Aim : IState
{
    public AIController myAIC;
    private float startTime;
    public FSM SetupSubFSM()
    {
        return null;
    }

    public void Enter() 
    {
        Debug.Log("Entered State_Aim");
        startTime = myAIC.CurrentTime();
    } 
    public void Update() 
    {
        myAIC.AimAtTargetedTank();
    } 
    public void Exit() 
    {
        Debug.Log("Exited State_Aim");
    } 

    public AIController aic
    {
        get{ return myAIC; }
        set{ myAIC = value; } 
    }

    public IState ShouldExitTo() 
    {     
        if (myAIC.FriendlyInLineOfFire())
            return new State_Reposition();
        
        if (myAIC.OpponentInLoS() && myAIC.TransitionTimerUp("aim to shoot", startTime))
            return new State_Shoot();

        return null; 
    }
}
