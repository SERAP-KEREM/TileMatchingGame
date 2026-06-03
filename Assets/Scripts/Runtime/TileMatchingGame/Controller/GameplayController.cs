using Assets.Scripts.Runtime.TileMatchingGame.View;
using UnityEngine;

namespace Assets.Scripts.Runtime.TileMatchingGame.Controller
{
    public class GameplayController
    {
        private readonly GameManager _gameManager;
        private readonly Camera _camera;

        public GameplayController(GameManager gameManager, Camera camera)
        {
            _gameManager = gameManager;
            _camera = camera;
        }

        public void ObserveClickHandler()
        {
            DetectMouseInput();
            DetectTouchInput();
        }

        private void DetectMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 worldPoint = _camera.ScreenToWorldPoint(Input.mousePosition);
                ProcessInput(worldPoint);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _gameManager.OnPausePressed();
            }
        }

        private void DetectTouchInput()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    Vector2 worldPoint = _camera.ScreenToWorldPoint(touch.position);
                    ProcessInput(worldPoint);
                }
            }
        }

        private void ProcessInput(Vector2 worldPoint)
        {
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject.TryGetComponent(out TileView tileView))
            {
                HandleTileClick(tileView);
            }
        }

        public void HandleTileClick(TileView tileView)
        {
            _gameManager.HandleTileClick(tileView);
        }
    }
}
