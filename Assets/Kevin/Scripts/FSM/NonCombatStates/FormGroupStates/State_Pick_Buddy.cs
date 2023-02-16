
public class State_Pick_Buddy : IState
{
    public AIController myAIC;
    
    public FSM SetupSubFSM()
    {
        return null;
    }

    public void Enter() 
    {
        myAIC.PickBuddy();
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
        return new State_Move_To_Buddy(); 
    }
}
