

public class FSM
{
    AIController aic;
    IState currentState;
    FSM currentStatesSubFSM;

    // calling new FSM() with a start state that
    // defines a subFSM will first call Enter on its state
    // then Enter on the subFSM's state
    public FSM(IState startState, AIController aic) 
    {
        this.aic = aic;         
        ChangeState(startState); 
    } 

    void ChangeState(IState newState) 
    {
        this.ExitState();    

        currentState = newState; 

        currentState.aic = this.aic;    
        currentState.Enter();        
        currentStatesSubFSM = currentState.SetupSubFSM();         
    } 

    void ExitState() 
    {
        if (currentState != null)
            currentState.Exit();

        if (currentStatesSubFSM != null)
            currentStatesSubFSM.ExitState();
    }

    public void UpdateStep() 
    {         
        if (currentState == null) return;         

        IState newState = currentState.ShouldExitTo(); 
        
        if(newState == null)         
        {                 
            currentState.Update();                 
            if (currentStatesSubFSM != null)                            
                currentStatesSubFSM.UpdateStep();         
        }         
        else         
        {                 
            ChangeState(newState);         
        } 
    }

    public string GetStateName()
    {
        string s = currentState.GetType().Name;//UnityEditor.ObjectNames.GetClassName((UnityEngine.Object)currentState);

        if (currentStatesSubFSM != null) s += " -> " + currentStatesSubFSM.GetStateName();

        return s;
    }
}