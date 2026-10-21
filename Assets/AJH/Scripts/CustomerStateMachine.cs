using System.Collections.Generic;

namespace AJH.Scripts
{
    public class CustomerStateMachine
    {
        AbstractState currentState;
        
        Dictionary<State,AbstractState> states = new();

        public CustomerStateMachine(Customers customers)
        {
            states.Add(State.Idle, new Idle(this, customers));
            states.Add(State.Come, new Come(this, customers));
            states.Add(State.Talking, new Talking(this, customers));
            states.Add(State.Pay, new Pay(this, customers));
            states.Add(State.Back, new Back(this, customers));
            states.Add(State.Done, new Done(this, customers));
            
            ChangeState(State.Idle);
        }

        public void ChangeState(State newState)
        {
            currentState?.Exit();
            currentState = states[newState];
            currentState?.Enter();
        }

        public void UpdateState()
        {
            currentState?.StateUpdate();
        }
    }

    public enum State
    {
        Idle,
        Come,
        Talking,
        Pay,
        Back,
        Done
    }
}