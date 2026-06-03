using Assets.Scripts.Runtime.TileMatchingGame.Model.Interfaces;
using static Assets.Scripts.Runtime.TileMatchingGame.ScriptableObjects.Level;

namespace Assets.Scripts.Runtime.TileMatchingGame.Model
{
    public class CollectColorTilesGoal : IGoal<Tile>
    {
        private readonly IBoard _board;
        private readonly TileColor _tileColor;
        private readonly int _amountToReach;
        private int _currentAmount;
        public GoalsEnum Goal => GoalsEnum.ColorTilesGoal;

        public CollectColorTilesGoal(IBoard board, GoalSetup setupData) 
        {
            _board = board;
            _board.OnTileRemoved += UpdateProgress;

            _tileColor = setupData.tileColor;
            _amountToReach = setupData.tileQuantity;
            _currentAmount = 0;
        }

        public string GetDescription()
        {

            return $"You must get:\n {_tileColor}: {_amountToReach} tiles";
        }

        public string GetProgress()
        {
            return $"{_currentAmount}/{_amountToReach}\n";
        }

        public bool HasFailedGoal()
        {
            return false;
        }

        public bool IsGoalCompleted()
        {
            return _currentAmount >= _amountToReach;
        }

        public void UpdateProgress(Tile progressData)
        {
            if (_tileColor == progressData.TileData.Color)
            {
                _currentAmount++;
            }
        }

        public void Dispose()
        {
            _board.OnTileRemoved -= UpdateProgress;
        }
    }
}