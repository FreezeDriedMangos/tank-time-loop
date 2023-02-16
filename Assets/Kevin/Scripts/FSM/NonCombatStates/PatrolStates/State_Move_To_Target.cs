
public class State_Move_To_Target : IState
{
    public AIController myAIC;
    
    public FSM SetupSubFSM()
    {
        return null;
    }

    public void Enter() 
    {
        myAIC.MoveToPatrolTarget();
    } 
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
        if (myAIC.ReachedPathfindTarget())
            return new State_Pick_Target(); 
        
        return null;
    }
}
