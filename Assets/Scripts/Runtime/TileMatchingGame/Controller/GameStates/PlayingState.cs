using Assets.Scripts.Runtime.TileMatchingGame.Controller;
using Assets.Scripts.Runtime.TileMatchingGame.Controller.Interfaces;
using Assets.Scripts.Runtime.TileMatchingGame.Model;
using Assets.Scripts.Runtime.TileMatchingGame.View;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Runtime.TileMatchingGame.Controller.GameStates
{
    public class PlayingState : IGameState
    {
        private GameManager _gameManager;
        private RectTransform _startScreenView;
        private GameHUD _gameHud;
        private LevelManager _levelManager;
        private ISoundManager _soundManager;

        public GameStateEnum State => GameStateEnum.Playing;

        public PlayingState(GameManager gameManager, RectTransform startScreenView, GameHUD gameHud, LevelManager levelManager, ISoundManager soundManager)
        {
            _gameManager = gameManager;
            _startScreenView = startScreenView;
            _gameHud = gameHud;
            _levelManager = levelManager;
            _soundManager = soundManager;
        }


        public void Enter()
        {
            _startScreenView.gameObject.SetActive(false);
            _gameHud.SetLevelInfo(_levelManager.CurrentLevelNumber, _levelManager.CurrentLevelDisplayName);
            _gameHud.SetGoalsDescription();
            _soundManager.PlayMusic(AppConstants.RetroArcadeMusic);
        }

        public void Exit()
        {
            _soundManager.StopMusic();
        }

        public void HandleTileClick(Tile tile)
        {
            
            List<Tile> matchedTiles = _gameManager.MatchFinder.FindMatches(tile);
            if (matchedTiles.Count >= AppConstants.MinimumMatchSize)
            {
                _gameManager.OnMatchedTiles(matchedTiles);
            }
        }
    }
}