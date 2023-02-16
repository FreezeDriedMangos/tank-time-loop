using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Form_Group : IState
{
    public AIController myAIC;
    
    public FSM SetupSubFSM()
    {
        return new FSM(new State_Pick_Buddy(), myAIC);
    }

    public void Enter() {} 
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
        if (myAIC.MyBuddyPickedMeAsABuddyAndWereNearby())
        {
            Debug.Log("We're nearby!");
            return new State_Idle();
        }

        return null; 
    }
}
