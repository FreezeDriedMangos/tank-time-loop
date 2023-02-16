public interface IState
{
    AIController aic {get; set;}
    FSM SetupSubFSM();
    void Enter(); 
    void Update(); 
    void Exit(); 
    IState ShouldExitTo();
}