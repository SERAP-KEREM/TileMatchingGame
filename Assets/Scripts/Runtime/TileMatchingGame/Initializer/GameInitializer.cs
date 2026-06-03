using Assets.Scripts.Runtime.TileMatchingGame.Controller;
using Assets.Scripts.Runtime.TileMatchingGame.Controller.GameStates;
using Assets.Scripts.Runtime.TileMatchingGame.Controller.Interfaces;
using Assets.Scripts.Runtime.TileMatchingGame.Model;
using Assets.Scripts.Runtime.TileMatchingGame.Model.Interfaces;
using Assets.Scripts.Runtime.TileMatchingGame.ScriptableObjects;
using Assets.Scripts.Runtime.TileMatchingGame.Services;
using Assets.Scripts.Runtime.TileMatchingGame.Services.Interfaces;
using Assets.Scripts.Runtime.TileMatchingGame.View;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Runtime.TileMatchingGame.Initializer
{
    public class GameInitializer : MonoBehaviour
    {
        [SerializeField] private TileFlyweight[] tileFlyweights; 
        [SerializeField] private Level[] levelData;
        [SerializeField] private RectTransform _levelButtonParent;
        [SerializeField] private Button _levelButtonPrefab;
        [SerializeField] private GameHUD _gameHudView;
        [SerializeField] private GameObject _tilePrefab;
        [SerializeField] private Transform _tilesParent;
        [SerializeField] private RectTransform _boardFrameTransform;
        [SerializeField] private RectTransform _startScreenView;
        [SerializeField] private RectTransform _pauseView;
        [SerializeField] private RectTransform _victoryView;
        [SerializeField] private RectTransform _gameOverView;
        [SerializeField] private RectTransform _goalsView;

        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _sfxSource;

        private IBoard _board;
        private IGoalManager _goalManager;
        private GameplayController _gameplayController;
        private TileViewPool _tileViewPool;
        private LevelManager _levelManager;
        private LevelButtonFactory _levelFactory;
        private GoalsPanelView _goalsPanel;
        private GameManager _gameManager;

        void Awake()
        {
            //Registering interfaces
            _board = new Board();
            ITileFactory tileFactory = new TileFactory(tileFlyweights);
            IMatchFinder matchFinder = new DFSMatchFinder(_board);
            IScoreManager scoreManager = new ScoreManager();
            ISoundManager soundManager = new SoundManager(_musicSource, _sfxSource);

            //Instanciating
            CanvasAdapter canvasAdapter = new CanvasAdapter(_board, Camera.main, _boardFrameTransform);
            _tileViewPool = new TileViewPool(_board, _tilePrefab, _tilesParent, canvasAdapter);
            BoardFiller boardFiller = new BoardFiller(_board, tileFactory, _tileViewPool, canvasAdapter, soundManager);
            BoardModifier boardModifier = new BoardModifier(_board, boardFiller, _tileViewPool);
            _goalsPanel = GetOrCreateGoalsPanel();
            _gameManager = new GameManager(_board, matchFinder, boardModifier, scoreManager, soundManager,
                InitializeGameStates(soundManager, scoreManager)
                );

            if (_goalsPanel != null)
            {
                _goalsPanel.OnCloseRequested += CloseGoalsPanel;
            }

            _levelFactory = new LevelButtonFactory(_levelManager, _levelButtonPrefab, _levelButtonParent);
            _gameplayController = new GameplayController(_gameManager, Camera.main);
            _gameHudView.Initialize(_gameManager, scoreManager, _goalManager, _levelManager);
            _gameManager.OnEndTurn += RefreshGoalsUi;
        }

        private void OnDestroy()
        {
            if (_gameManager != null)
            {
                _gameManager.OnEndTurn -= RefreshGoalsUi;
            }
        }

        private void RefreshGoalsUi()
        {
            if (_gameManager.CurrentState == GameStateEnum.Goals && _goalsPanel != null)
            {
                _goalsPanel.Refresh(_goalManager);
            }
        }

        private void CloseGoalsPanel()
        {
            if (_gameManager.CurrentState == GameStateEnum.Goals)
            {
                _gameManager.ChangeState(GameStateEnum.LastState);
            }
        }

        private GoalsPanelView GetOrCreateGoalsPanel()
        {
            if (_goalsView == null)
            {
                Debug.LogError("Goals View is not assigned on GameInitializer.");
                return null;
            }

            GoalsPanelView panel = _goalsView.GetComponent<GoalsPanelView>();
            if (panel == null)
            {
                panel = _goalsView.gameObject.AddComponent<GoalsPanelView>();
            }

            return panel;
        }

        private Func<GameManager, IGameState[]> InitializeGameStates(ISoundManager soundManager, IScoreManager scoreManager)
        {
            return gm =>
            {
                _goalManager = new GoalManager(gm, scoreManager, _board);
                _levelManager = new LevelManager(gm, _goalManager, _board, levelData.ToList());
                return new IGameState[]
                                { new PlayingState(gm, _startScreenView, _gameHudView, _levelManager, soundManager), new PauseState(_pauseView),
                    new VictoryState(_victoryView, soundManager), new GameOverState(_gameOverView,
                    soundManager), new ShowGoalsState(_goalsPanel, _goalManager) };
            };
        }

        private void Start()
        {
            HideOverlayPanels();

            for (int i = 0; i < levelData.Length; i++)
            {
                _levelFactory.CreateButton(levelData[i], i);
            }

            _tileViewPool.SetBoard(_board);
        }

        private void HideOverlayPanels()
        {
            if (_goalsPanel != null)
            {
                _goalsPanel.Hide();
            }

            if (_pauseView != null)
            {
                _pauseView.gameObject.SetActive(false);
            }

            if (_victoryView != null)
            {
                _victoryView.gameObject.SetActive(false);
            }

            if (_gameOverView != null)
            {
                _gameOverView.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            _gameplayController.ObserveClickHandler();
        }
    }
}