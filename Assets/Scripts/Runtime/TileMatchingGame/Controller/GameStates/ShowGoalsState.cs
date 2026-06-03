using Assets.Scripts.Runtime.TileMatchingGame.Controller.Interfaces;
using Assets.Scripts.Runtime.TileMatchingGame.Model;
using Assets.Scripts.Runtime.TileMatchingGame.View;

namespace Assets.Scripts.Runtime.TileMatchingGame.Controller.GameStates
{
    public class ShowGoalsState : IGameState
    {
        private readonly GoalsPanelView _goalsPanel;
        private readonly IGoalManager _goalManager;

        public GameStateEnum State => GameStateEnum.Goals;

        public ShowGoalsState(GoalsPanelView goalsPanel, IGoalManager goalManager)
        {
            _goalsPanel = goalsPanel;
            _goalManager = goalManager;
        }

        public void Enter()
        {
            if (_goalsPanel == null)
            {
                UnityEngine.Debug.LogWarning("GoalsPanelView is not assigned. Goals button will do nothing visible.");
                return;
            }

            _goalsPanel.Show(_goalManager);
        }

        public void Exit()
        {
            _goalsPanel?.Hide();
        }

        public void HandleTileClick(Tile tile)
        {
        }
    }
}
