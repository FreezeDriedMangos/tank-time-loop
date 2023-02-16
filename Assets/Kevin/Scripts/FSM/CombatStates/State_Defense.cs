using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Defense : IState
{
    public AIController myAIC;
    
    public FSM SetupSubFSM()
    {
        if (myAIC.TransitionProbability("defense to dodge"))
            return new FSM(new State_Dodge(), myAIC);
        
        return new FSM(new State_Parry(), myAIC);
    }

    public void Enter() 
    {
        Debug.Log("Entered State_Defense");
    } 
    public void Update() {} 
    public void Exit() 
    {
        Debug.Log("Exited State_Defense");
    } 

    public AIController aic
    {
        get{ return myAIC; }
        set{ myAIC = value; } 
    }
    public IState ShouldExitTo() 
    {     
        return new State_Attack(); 
    }
}
