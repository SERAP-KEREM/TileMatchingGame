using Assets.Scripts.Runtime.TileMatchingGame.Model.Interfaces;
using System;

namespace Assets.Scripts.Runtime.TileMatchingGame.Model
{
    public class Board : IBoard
    {
        private Tile[,] _tiles;
        public int Width { get; set; } 
        public int Height { get; set; }

        public event Action OnBoardUpdate;
        public event Action<Tile> OnTileRemoved;
        public event Action<Tile> OnTileFalling;

        public Tile GetTileAt(int row, int column)
        {
            if (_tiles == null || Height <= 0 || Width <= 0)
            {
                return null;
            }

            if (row < 0 || row >= Height || column < 0 || column >= Width)
            {
                return null;
            }

            if (row >= _tiles.GetLength(0) || column >= _tiles.GetLength(1))
            {
                return null;
            }

            return _tiles[row, column];
        }

        public void SetTileAt(int row, int column, Tile newTile)
        {
            if (row < 0 || row >= Height || column < 0 || column >= Width)
            {
                return;
            }

            if (_tiles == null || row >= _tiles.GetLength(0) || column >= _tiles.GetLength(1))
            {
                return;
            }

            _tiles[row, column] = newTile;
        }

        public void RemoveTileAt(int row, int column, bool isFalling = false)
        {
            if (_tiles == null || row < 0 || row >= _tiles.GetLength(0) || column < 0 || column >= _tiles.GetLength(1))
            {
                return;
            }

            if (row >= 0 && row < Height && column >= 0 && column < Width)
            {
                var removedTile = _tiles[row, column];
                if (!isFalling)
                {
                   OnTileRemoved?.Invoke(removedTile);
                }
                _tiles[row, column] = null;
            }

        }

        public void ResetBoard()
        {
            _tiles = new Tile[Height, Width];
            OnBoardUpdate?.Invoke();
        }

        public void FallTile(int row, int col, int rowAbove, Tile tileAbove)
        {
            SetTileAt(row, col, tileAbove);
            tileAbove.UpdatePosition(row, col);
            RemoveTileAt(rowAbove, col, true);
            OnTileFalling?.Invoke(tileAbove);
        }
    }
}