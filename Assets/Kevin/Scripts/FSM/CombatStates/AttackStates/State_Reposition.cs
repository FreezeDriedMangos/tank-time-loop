using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Reposition : IState
{
    public AIController myAIC;
    
    public FSM SetupSubFSM()
    {
        return null;
    }

    public void Enter() 
    {
        Debug.Log("Entered State_Reposition");
        myAIC.Reposition();
    } 
    public void Update() {} 
    public void Exit() 
    {
        Debug.Log("Exited State_Reposition");
        myAIC.StopPathfinding();
    } 

    public AIController aic
    {
        get{ return myAIC; }
        set{ myAIC = value; } 
    }
    public IState ShouldExitTo() 
    {     
        if (!myAIC.FriendlyInLineOfFire())
            return new State_Aim();
        return null; 
    }
}
