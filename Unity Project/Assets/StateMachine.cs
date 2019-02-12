using System.Collections;
using System.Collections.Generic;

public class StateMachine
{
    IDictionary<string, State> statesMap = new Dictionary<string, State>();
    State currState = null, nextState = null;

    public string CurrentState { get { return currState.StateID; } }

    public void AddState(State newState)
    {
        if (statesMap.ContainsKey(newState.StateID))
            return;

        if (currState == null)
            currState = nextState = newState;

        statesMap.Add(newState.StateID, newState);
    }

    public void SetNextState(string nextStateID)
    {
        if (!statesMap.ContainsKey(nextStateID))
            return;

        nextState = statesMap[nextStateID];
    }

    // Update is called once per frame
    public void Update()
    {
        if (currState != nextState)
        {
            currState.Exit();
            currState = nextState;
            currState.Enter();
        }

        currState.Update();
    }
}
