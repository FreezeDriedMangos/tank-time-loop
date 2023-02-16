using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_NonCombat : IState
{
    public AIController myAIC;
    
    public FSM SetupSubFSM()
    {
        return new FSM(new State_Patrolling(), myAIC);
    }

    public void Enter() {} 
    public void Update() {} 
    public void Exit() {} 

    public AIController aic
    {
        get{ return myAIC; }
        set{ myAIC = value; } 
    }
    public IState ShouldExitTo() 
    {     
        if (myAIC.OpponentInLoS())
            return new State_Combat();

        return null; 
    }
}
