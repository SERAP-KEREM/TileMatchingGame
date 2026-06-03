using Assets.Scripts.Runtime.TileMatchingGame.Controller;
using Assets.Scripts.Runtime.TileMatchingGame.Controller.Interfaces;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Runtime.TileMatchingGame.View
{
    public class GameHUD : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _goalsSummaryText;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _gameOverRestartButton;
        [SerializeField] private Button _nextLevelButton;
        [SerializeField] private Button _goalsButton;

        private GameManager _gameManager;
        private IScoreManager _scoreManager;
        private IGoalManager _goalManager;
        private LevelManager _levelManager;

        public void Initialize(GameManager gameManager, IScoreManager scoreManager, IGoalManager goalManager, LevelManager levelManager)
        {
            _gameManager = gameManager;
            _scoreManager = scoreManager;
            _goalManager = goalManager;
            _levelManager = levelManager;
            _scoreManager.OnScoreChanged += OnScoreChanged;
            _gameManager.OnEndTurn += OnTurnEnded;

            if (_restartButton != null)
            {
                _restartButton.onClick.AddListener(RestartCurrentLevel);
            }

            if (_gameOverRestartButton != null)
            {
                _gameOverRestartButton.onClick.AddListener(RestartCurrentLevel);
            }

            if (_nextLevelButton != null)
            {
                _nextLevelButton.onClick.AddListener(LoadNextLevel);
            }

            if (_goalsButton != null)
            {
                _goalsButton.onClick.AddListener(OpenGoalsPanel);
            }

            UpdateScoreDisplay(_scoreManager.CurrentScore);
            SetGoalsDescription();
        }

        private void OnScoreChanged(int newScore)
        {
            UpdateScoreDisplay(newScore);
        }

        private void OnTurnEnded()
        {
            SetGoalsDescription();
        }

        public void SetLevelInfo(int levelNumber, string levelDisplayName)
        {
            if (_levelText == null)
            {
                return;
            }

            _levelText.text = FormatLevelHudLabel(levelNumber, levelDisplayName);
        }

        private static string FormatLevelHudLabel(int levelNumber, string levelDisplayName)
        {
            string label = string.IsNullOrWhiteSpace(levelDisplayName)
                ? $"Level {levelNumber}"
                : levelDisplayName.Trim();

            if (Regex.IsMatch(label, $@"^Level\s*{levelNumber}\b", RegexOptions.IgnoreCase))
            {
                return label;
            }

            if (label.Equals($"Level{levelNumber}", System.StringComparison.OrdinalIgnoreCase))
            {
                return $"Level {levelNumber}";
            }

            return $"Level {levelNumber}: {label}";
        }

        public void SetGoalsDescription()
        {
            if (_goalsSummaryText == null)
            {
                return;
            }

            _goalsSummaryText.text = GoalsPanelView.BuildGoalsSummaryText(_goalManager);
        }

        private void OpenGoalsPanel()
        {
            if (_gameManager == null)
            {
                return;
            }

            if (_gameManager.CurrentState == GameStateEnum.Goals)
            {
                _gameManager.ChangeState(GameStateEnum.LastState);
                return;
            }

            _gameManager.ChangeState(GameStateEnum.Goals);
        }

        private void OnDestroy()
        {
            if (_scoreManager != null)
            {
                _scoreManager.OnScoreChanged -= OnScoreChanged;
            }

            if (_gameManager != null)
            {
                _gameManager.OnEndTurn -= OnTurnEnded;
            }
        }

        private void RestartCurrentLevel()
        {
            _levelManager.RestartLevel();
        }

        private void LoadNextLevel()
        {
            _levelManager.LoadNextLevel();
        }

        private void UpdateScoreDisplay(int newScore)
        {
            if (_scoreText != null)
            {
                _scoreText.text = $"Score: {newScore}";
            }
        }
    }
}
