using Assets.Scripts.Runtime.TileMatchingGame.Model;
using Assets.Scripts.Runtime.TileMatchingGame.Model.Interfaces;
using Assets.Scripts.Runtime.TileMatchingGame.Services.Interfaces;
using System.Collections.Generic;

namespace Assets.Scripts.Runtime.TileMatchingGame.Services
{
    public class BoardModifier : IBoardModifier
    {
        private readonly IBoard _board;
        private readonly BoardFiller _boardFiller;
        private readonly TileViewPool _poolViewPool;

        public BoardModifier(IBoard board, BoardFiller boardFiller, TileViewPool tileViewPool)
        {
            _board = board;
            _boardFiller = boardFiller;
            _poolViewPool = tileViewPool;
        }

        public void RemoveTiles(List<Tile> tilesToRemove)
        {
            foreach (Tile tile in tilesToRemove)
            {
                if (tile != null)
                {
                    _board.RemoveTileAt(tile.Row, tile.Column, false);
                }
            }
        }

        public void UpdateTilesPosition()
        {
            for (int col = 0; col < _board.Width; col++)
            {
                for (int row = 0; row < _board.Height; row++)
                {
                    if (_board.GetTileAt(row, col) == null)
                    {
                        for (int rowAbove = row + 1; rowAbove < _board.Height; rowAbove++)
                        {
                            Tile tileAbove = _board.GetTileAt(rowAbove, col);
                            if (tileAbove != null)
                            {
                                _board.FallTile(row, col, rowAbove, tileAbove);
                                break;
                            }
                        }
                    }
                }
            }
        }

        public void FillEmptySpaces()
        {
            _boardFiller.FillEmptySpaces();
        }

        public void ClearActiveTiles()
        {
            _boardFiller.StopFill();
            _poolViewPool.ReleaseAllTileViews();
        }

        public void RestartBoard()
        {
            ClearActiveTiles();
            _board.ResetBoard();
            _poolViewPool.PrePopulate(20);
        }
    }
}
