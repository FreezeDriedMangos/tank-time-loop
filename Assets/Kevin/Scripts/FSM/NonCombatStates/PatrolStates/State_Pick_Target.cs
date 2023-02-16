
public class State_Pick_Target : IState
{
    public AIController myAIC;
    
    public FSM SetupSubFSM()
    {
        return null;
    }

    public void Enter() 
    {
        myAIC.PickPatrolPathfindTarget();
    } 
    public void Update() {} 
    public void Exit() {} 

    public AIController aic
    {
        get{ return myAIC; }
        set{ myAIC = value; } 
    }
    public IState ShouldExitTo() 
    {     
        return new State_Move_To_Target(); 
    }
}
