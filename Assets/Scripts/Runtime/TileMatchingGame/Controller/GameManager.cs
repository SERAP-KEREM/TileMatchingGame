using Assets.Scripts.Runtime.TileMatchingGame.Controller.GameStates;
using Assets.Scripts.Runtime.TileMatchingGame.Controller.Interfaces;
using Assets.Scripts.Runtime.TileMatchingGame.Model;
using Assets.Scripts.Runtime.TileMatchingGame.Model.Interfaces;
using Assets.Scripts.Runtime.TileMatchingGame.Services.Interfaces;
using Assets.Scripts.Runtime.TileMatchingGame.View;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Runtime.TileMatchingGame.Controller
{
    public class GameManager
    {
        private IBoard _board;
        private IGameState _currentState;
        private IGameState _lastState;
        private IMatchFinder _matchFinder;
        private IBoardModifier _boardModifier;
        private IScoreManager _scoreManager;
        private ISoundManager _soundManager;
        private IGameState[] _gameStates;

        private readonly Dictionary<GameStateEnum, IGameState> _gameStatesDict = new Dictionary<GameStateEnum, IGameState>();

        public IMatchFinder MatchFinder { get => _matchFinder; }
        public GameStateEnum? CurrentState => _currentState?.State;
        public event Action OnNextMove;
        public event Action OnEndTurn;

        public GameManager(IBoard board, IMatchFinder matchFinder, IBoardModifier boardModifier, IScoreManager scoreManager, ISoundManager soundManager, Func<GameManager, IGameState[]> gameStateFactory)
        {
            _board = board;
            _matchFinder = matchFinder;
            _boardModifier = boardModifier;
            _scoreManager = scoreManager;
            _soundManager = soundManager;
            _gameStates = gameStateFactory(this);
            CreateGameStates();
        }

        private void CreateGameStates()
        {
            _gameStatesDict.Clear();
            foreach (var gameState in _gameStates) 
            {
                _gameStatesDict[gameState.State] = gameState;
            }
        }

        public void ChangeState(GameStateEnum newStateEnum)
        {
            if (newStateEnum == GameStateEnum.LastState)
            {
                IGameState returnState = _lastState ?? _gameStatesDict[GameStateEnum.Playing];
                if (returnState == null || returnState == _currentState)
                {
                    return;
                }

                _currentState?.Exit();
                _currentState = returnState;
                _currentState.Enter();
                return;
            }

            if (newStateEnum == _currentState?.State)
            {
                return;
            }

            if (!_gameStatesDict.TryGetValue(newStateEnum, out IGameState newState) || newState == null)
            {
                UnityEngine.Debug.LogWarning($"Game state '{newStateEnum}' is not registered.");
                return;
            }

            _lastState = _currentState;
            _currentState?.Exit();
            _currentState = newState;
            _currentState.Enter();
        }

        public void StartGame()
        {
            _boardModifier.RestartBoard();
            RefillBoard();
            ChangeState(GameStateEnum.Playing);
        }

        public void PrepareNewLevel()
        {
            _boardModifier.ClearActiveTiles();
        }

        public void ResetGame()
        {
            _scoreManager.ResetScore();
        }

        public void RefillBoard()
        {
            _boardModifier.FillEmptySpaces();
        }

        public void HandleTileClick(TileView tileView)
        {
            _currentState?.HandleTileClick(tileView.Tile);
        }

        public void OnMatchedTiles(List<Tile> matchedTiles)
        {
            _soundManager.PlaySound(AppConstants.TilePopSound);
            _scoreManager.AddScore(matchedTiles.Count);
            _boardModifier.RemoveTiles(matchedTiles);

            _boardModifier.UpdateTilesPosition();

            OnNextMove?.Invoke();

            RefillBoard();

            OnEndTurn?.Invoke();
        }

        public void OnPausePressed()
        {
            if (_currentState == null)
            {
                return;
            }

            if (_currentState.State == GameStateEnum.Goals)
            {
                ChangeState(GameStateEnum.LastState);
                return;
            }

            if (_currentState.State == GameStateEnum.Paused)
            {
                ChangeState(GameStateEnum.LastState);
                return;
            }

            if (_currentState.State == GameStateEnum.Playing)
            {
                ChangeState(GameStateEnum.Paused);
            }
        }
    }

    public enum GameStateEnum
    {
        Menu,
        Playing,
        Paused,
        GameOver,
        Victory,
        Goals,
        LastState
    }

}