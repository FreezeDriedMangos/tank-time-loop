
public class State_Parry : IState
{
    public AIController myAIC;
    
    public FSM SetupSubFSM()
    {
        return null;
    }

    public void Enter() 
    {
        myAIC.ParryBullet();
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
        return null; 
    }
}
