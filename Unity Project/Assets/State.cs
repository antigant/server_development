public abstract class State
{
    protected string stateID;

    public State(string stateID)
    {
        this.stateID = stateID;
    }

    public string StateID { get { return stateID; } }
    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}