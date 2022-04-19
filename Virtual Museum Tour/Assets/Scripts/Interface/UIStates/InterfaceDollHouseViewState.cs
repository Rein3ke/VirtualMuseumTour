using Events;
using State;

namespace Interface.UIStates
{
	public class InterfaceDollHouseViewState : IState
	{
		private UIManager UIManager { get; }

		public InterfaceDollHouseViewState(UIManager uiManager)
		{
			UIManager = uiManager;
		}

		public void Enter()
		{
			EventManager.StartListening(EventType.EventDollHouseView, OnDollHouseView);
		}

		public void Tick()
		{
			
		}

		public void FixedTick()
		{
			
		}

		public void Exit()
		{
			EventManager.StopListening(EventType.EventDollHouseView, OnDollHouseView);
		}
		
		private void OnDollHouseView(EventParam eventParam)
		{
			if (!eventParam.EventBoolean)
			{
				UIManager.ChangeState(UIManager.EmptyState);
			}
		}
	}
}