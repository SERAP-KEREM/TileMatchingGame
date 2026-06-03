using Assets.Scripts.Runtime.TileMatchingGame.Model;
using System.Collections.Generic;

namespace Assets.Scripts.Runtime.TileMatchingGame.Services.Interfaces
{
    public interface IBoardModifier
    {
        public void RemoveTiles(List<Tile> tilesToRemove);
        public void UpdateTilesPosition();
        public void FillEmptySpaces();
        void ClearActiveTiles();
        void RestartBoard();
    }
}