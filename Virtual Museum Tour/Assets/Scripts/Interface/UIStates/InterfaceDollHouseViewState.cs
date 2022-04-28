using Events;
using State;

namespace Interface.UIStates
{
	/// <summary>
	/// This class (state) describes the behaviour when the DollHouseView is active.
	/// </summary>
	public class InterfaceDollHouseViewState : IState
	{
		#region Properties

		/// <summary>
		/// Stores a reference to the UIManager to have access to its switch state method.
		/// </summary>
		private UIManager UIManager { get; }

		#endregion

		#region Constructor

		/// <summary>
		/// Sets the current reference to the UIManager.
		/// </summary>
		/// <param name="uiManager">Current active UIManager.</param>
		public InterfaceDollHouseViewState(UIManager uiManager)
		{
			UIManager = uiManager;
		}

		#endregion

		#region IState Implementation

		/// <summary>
		/// Subscribes to the DollHouseView event to change to switch to the empty state when triggered.
		/// </summary>
		public void Enter()
		{
			EventManager.StartListening(EventType.EventDollHouseView, ChangeToEmptyState);
		}

		/// <summary>
		/// Update method. Gets called every frame but is not used in this state.
		/// </summary>
		public void Tick()
		{
			// Do nothing
		}

		/// <summary>
		/// Unsubscribes from the DollHouseView event.
		/// </summary>
		public void Exit()
		{
			EventManager.StopListening(EventType.EventDollHouseView, ChangeToEmptyState);
		}

		#endregion

		#region State Change Methods

		/// <summary>
		/// Orders the UIManager to switch to the EmptyState.
		/// </summary>
		/// <param name="eventParam">Boolean, if the DollHouseView is closed (via EventParam).</param>
		private void ChangeToEmptyState(EventParam eventParam)
		{
			if (!eventParam.EventBoolean)
			{
				UIManager.ChangeState(UIManager.EmptyState);
			}
		}

		#endregion
	}
}