namespace Expedition67.Core
{
    public abstract class BaseState<T>
    {
        protected T owner;

        public abstract void OnEnter();
        public abstract void OnUpdate();
        public abstract void OnExit();
    }
}
