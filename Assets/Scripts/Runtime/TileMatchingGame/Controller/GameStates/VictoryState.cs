using Assets.Scripts.Runtime.TileMatchingGame.Controller.Interfaces;
using Assets.Scripts.Runtime.TileMatchingGame.Model;
using UnityEngine;

namespace Assets.Scripts.Runtime.TileMatchingGame.Controller.GameStates
{
    public class VictoryState : IGameState
    {

        private ISoundManager _soundManager;
        private RectTransform _victoryView;

        public GameStateEnum State => GameStateEnum.Victory;


        public VictoryState(RectTransform victoryView, ISoundManager soundManager)
        {
            _victoryView = victoryView;
            _soundManager = soundManager;
        }


        public void Enter()
        {
            _victoryView.gameObject.SetActive(true);
            _soundManager.PlaySound(AppConstants.VictorySound);
        }

        public void Exit()
        {
            _victoryView.gameObject.SetActive(false);
        }

        public void HandleTileClick(Tile tile)
        {

        }
    }
}