using Assets.Scripts.Runtime.TileMatchingGame.Controller;
using Assets.Scripts.Runtime.TileMatchingGame.Model.Interfaces;
using UnityEngine;
using static Assets.Scripts.Runtime.TileMatchingGame.ScriptableObjects.Level;

namespace Assets.Scripts.Runtime.TileMatchingGame.Model
{
    public class MaxMovesGoal : IGoal
    {
        private readonly GameManager _gameManager;

        private int _maxMoves;
        private int _totalMoves;
        public GoalsEnum Goal => GoalsEnum.MaxMovesGoal;

        public MaxMovesGoal(GameManager gameManager, GoalSetup setupData)
        {
            _gameManager = gameManager;
            _gameManager.OnNextMove += UpdateProgress;

            _maxMoves = setupData.maxPoints;
            _totalMoves = 0;
        }

        public string GetDescription()
        {
            return $"Beat this level with less than {_maxMoves} moves!";
        }

        public string GetProgress()
        {
            return $"{Mathf.Max(0, _maxMoves - _totalMoves)} left!";
        }

        public bool HasFailedGoal()
        {
            return _totalMoves >= _maxMoves;
        }

        public bool IsGoalCompleted()
        {
            return _maxMoves >= _totalMoves;
        }

        public void UpdateProgress()
        {
            _totalMoves++;
        }

        public void Dispose()
        {
            _gameManager.OnNextMove -= UpdateProgress;
        }
    }
}