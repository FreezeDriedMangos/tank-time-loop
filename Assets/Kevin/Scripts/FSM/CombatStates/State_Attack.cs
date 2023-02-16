using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class State_Attack : IState
{
    public AIController myAIC;
    
    public FSM SetupSubFSM()
    {
        return new FSM(new State_Aim(), myAIC);
    }

    public void Enter() 
    {        
        Debug.Log("Entered State_Attack");
    } 
    public void Update() 
    {
        myAIC.AimAtTargetedTank();
    } 
    public void Exit() 
    {
        Debug.Log("Exited State_Attack");
    } 

    public AIController aic
    {
        get{ return myAIC; }
        set{ myAIC = value; } 
    }
    public IState ShouldExitTo() 
    {   
        if (myAIC.TooManyEnemiesWithLoSToMe())
            return new State_Flee();
        
        if (myAIC.BulletGonnaHitMe())
            return new State_Defense();

        return null; 
    }
}
