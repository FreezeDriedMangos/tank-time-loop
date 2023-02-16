
public class State_Move_To_Buddy : IState
{
    public AIController myAIC;
    private float startTime;
    public FSM SetupSubFSM()
    {
        return null;
    }

    public void Enter() 
    {
        startTime = myAIC.CurrentTime();
        myAIC.PathfindToBuddy();
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
        if (myAIC.TransitionTimerUp("pathfindToBuddy to pickBuddy", startTime))
            return new State_Pick_Buddy();

        return null; 
    }
}
