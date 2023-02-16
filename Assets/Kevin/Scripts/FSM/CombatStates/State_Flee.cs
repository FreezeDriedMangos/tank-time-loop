using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Flee : IState
{
    public AIController myAIC;
    
    public FSM SetupSubFSM()
    {
        return null;
    }

    public void Enter() 
    {
        Debug.Log("Entered State_Flee");
        myAIC.PathfindToSafeAlly();
    } 
    public void Update() {} 
    public void Exit() 
    {
        Debug.Log("Exited State_Flee");
    } 

    public AIController aic
    {
        get{ return myAIC; }
        set{ myAIC = value; } 
    }
    public IState ShouldExitTo() 
    {     
        if (myAIC.BulletGonnaHitMe())
            return new State_Defense();
            
        return null; 
    }
}
