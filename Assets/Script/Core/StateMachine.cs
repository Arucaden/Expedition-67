namespace Expedition67.Core
{
    public class StateMachine<T>
    {
        public BaseState<T> CurrentState { get; private set; }

        public void ChangeState(BaseState<T> newState)
        {
            CurrentState?.OnExit();

            CurrentState = newState;
            CurrentState.OnEnter();
        }

        public void OnUpdate()
        {
            CurrentState?.OnUpdate();
        }
    }
}
