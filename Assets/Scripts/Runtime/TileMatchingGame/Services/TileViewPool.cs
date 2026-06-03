using Assets.Scripts.Runtime.TileMatchingGame.Model;
using Assets.Scripts.Runtime.TileMatchingGame.Model.Interfaces;
using Assets.Scripts.Runtime.TileMatchingGame.View;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Runtime.TileMatchingGame.Services
{
    public class TileViewPool : IDisposable
    {
        private readonly GameObject _tilePrefab;
        private readonly Transform _parent;
        private readonly CanvasAdapter _canvasAdapter;

        private readonly Stack<TileView> _pool = new Stack<TileView>();
        private readonly Dictionary<Tile, TileView> _tileViewMap = new Dictionary<Tile, TileView>();

        private IBoard _board;
        private bool _hasPrePopulated;

        public TileViewPool(IBoard board, GameObject tilePrefab, Transform parent, CanvasAdapter canvasAdapter)
        {
            _board = board;
            _tilePrefab = tilePrefab;
            _parent = parent;
            _canvasAdapter = canvasAdapter;
        }

        public void SetBoard(IBoard board)
        {
            _board.OnTileRemoved += HandleTileRemoved;
            _board.OnTileFalling += HandleTileFalling;
        }

        public void PrePopulate(int initialSize)
        {
            if (_hasPrePopulated)
            {
                return;
            }

            for (int i = 0; i < initialSize; i++)
            {
                TileView tileView = InstantiateTile(i);
                tileView.gameObject.SetActive(false);
                _pool.Push(tileView);
            }

            _hasPrePopulated = true;
        }

        public void ReleaseAllTileViews()
        {
            foreach (TileView tileView in _tileViewMap.Values.ToList())
            {
                if (tileView != null)
                {
                    ReturnTileView(tileView);
                }
            }

            _tileViewMap.Clear();

            for (int i = 0; i < _parent.childCount; i++)
            {
                Transform child = _parent.GetChild(i);
                if (child.TryGetComponent(out TileView tileView))
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        public TileView GetTileView(Tile tile)
        {
            TileView tileView;
            if (_pool.Count > 0)
            {
                tileView = _pool.Pop();
                tileView.gameObject.SetActive(true);
                _tileViewMap[tile] = tileView;
            }
            else
            {
                tileView = InstantiateTile(tile.Id);
                _tileViewMap[tile] = tileView;
            }

            tileView.transform.position = _canvasAdapter.GetTileViewPosition(_board.Height + _board.Height, tile.Column);
            tileView.transform.localScale = _canvasAdapter.GetTileViewScale(tileView);
            return tileView;
        }

        public void ReturnTileView(TileView tileView)
        {
            if (tileView == null)
            {
                return;
            }

            tileView.gameObject.SetActive(false);
            _pool.Push(tileView);
        }

        private void HandleTileRemoved(Tile tile)
        {
            if (_tileViewMap.TryGetValue(tile, out TileView tileView))
            {
                _tileViewMap.Remove(tile);
                ReturnTileView(tileView);
            }
        }

        private void HandleTileFalling(Tile tile)
        {
            if (_tileViewMap.TryGetValue(tile, out TileView tileView))
            {
                var newPosition = _canvasAdapter.GetTileViewPosition(tileView);
                tileView.OnTilePositionUpdated(newPosition);
            }
        }

        private TileView InstantiateTile(int i)
        {
            GameObject instance = UnityEngine.Object.Instantiate(_tilePrefab, _parent);
            instance.name = $"Tile-{i}";
            return instance.GetComponent<TileView>();
        }

        void IDisposable.Dispose()
        {
            if (_board != null)
            {
                _board.OnTileRemoved -= HandleTileRemoved;
                _board.OnTileFalling -= HandleTileFalling;
            }
        }
    }
}
