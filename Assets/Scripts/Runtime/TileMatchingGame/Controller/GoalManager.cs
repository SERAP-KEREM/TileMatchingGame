using Assets.Scripts.Runtime.TileMatchingGame.Controller.Interfaces;
using Assets.Scripts.Runtime.TileMatchingGame.Model;
using Assets.Scripts.Runtime.TileMatchingGame.Model.Interfaces;
using Assets.Scripts.Runtime.TileMatchingGame.ScriptableObjects;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Runtime.TileMatchingGame.Controller
{
    public class GoalManager : IGoalManager
    {
        private readonly IScoreManager _scoreManager;
        private readonly IBoard _board;
        private readonly GameManager _gameManager;

        private readonly List<IGoal> _activeLevelGoals = new();

        public IReadOnlyList<IGoal> CurrentLevelGoals => _activeLevelGoals;

        public GoalManager(GameManager gameManager, IScoreManager scoreManager, IBoard board)
        {
            _gameManager = gameManager;
            _scoreManager = scoreManager;
            _board = board;

            _gameManager.OnEndTurn += () => OnEndTurnBehavior();
        }

        public void SetupLevelGoals(Level.GoalSetup[] goalsSetup)
        {
            ResetGoals();

            if (goalsSetup == null) return;

            foreach (var setup in goalsSetup)
            {
                var goalInstance = GetGoalObject(setup);
                if (goalInstance != null)
                {
                    _activeLevelGoals.Add(goalInstance);
                }
                else
                {
                    Debug.LogWarning($"Goal type {setup.goalEnum} is not supported.");
                }
            }
        }

        private IGoal GetGoalObject(Level.GoalSetup goalSetup)
        {
            return goalSetup.goalEnum switch
            {
                GoalsEnum.TotalPointsGoal => new CollectTilesPointsGoal(_scoreManager, goalSetup),
                GoalsEnum.ColorTilesGoal => new CollectColorTilesGoal(_board, goalSetup),
                GoalsEnum.MaxMovesGoal => new MaxMovesGoal(_gameManager, goalSetup),
                _ => null,
            };
        }

        private void ResetGoals()
        {
            foreach (var goal in _activeLevelGoals)
            {
                goal.Dispose();
            }

            _activeLevelGoals.Clear();
        }

        public bool CheckForVictory()
        {
            if (!_activeLevelGoals.Any()) return false;
            return _activeLevelGoals.All(goal => goal.IsGoalCompleted());
        }

        public bool CheckForGameOver()
        {
            if (!_activeLevelGoals.Any()) return false;
            return _activeLevelGoals.Any(goal => goal.HasFailedGoal());
        }

        private void OnEndTurnBehavior()
        {
            if (CheckForVictory())
            {
                _gameManager.ChangeState(GameStateEnum.Victory);
            }
            else if (CheckForGameOver())
            {
                _gameManager.ChangeState(GameStateEnum.GameOver);
            }
        }

        public void Dispose()
        {
            ResetGoals();
            _gameManager.OnEndTurn -= OnEndTurnBehavior;
        }
    }
}