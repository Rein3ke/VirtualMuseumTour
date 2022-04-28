namespace State
{
    /// <summary>
    /// Interface for state inheriting classes (e.g. NavigationUserInterfaceState).
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// Enter method for the state.
        /// </summary>
        void Enter();

        /// <summary>
        /// Similar to Unity's Update() method. Gets called every frame.
        /// </summary>
        void Tick();

        /// <summary>
        /// Exit method for the state.
        /// </summary>
        void Exit();
    }
}