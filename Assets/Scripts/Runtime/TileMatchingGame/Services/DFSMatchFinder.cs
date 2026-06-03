using Assets.Scripts.Runtime.TileMatchingGame.Model;
using Assets.Scripts.Runtime.TileMatchingGame.Model.Interfaces;
using Assets.Scripts.Runtime.TileMatchingGame.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Profiling;

namespace Assets.Scripts.Runtime.TileMatchingGame.Services
{
    public class DFSMatchFinder : IMatchFinder
    {
        private readonly IBoard _board;
        private static readonly int[,] NeighborOffsets = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } };

        public DFSMatchFinder(IBoard board)
        {
            _board = board;
        }

        public List<Tile> FindMatches(Tile startTile)
        {
            var matchedTiles = new List<Tile>();
            if (startTile == null)
            {
                return matchedTiles;
            }

            var visitedTiles = new HashSet<Tile>();

            Profiler.BeginSample("DFSMatchFinder");

            DFSIterativeOptimized(startTile, visitedTiles, matchedTiles);

            //Span<int> resultBuffer = stackalloc int[_board.Width * _board.Height];
            //int matchCount = DFSIterativeWithSpan(startTile, resultBuffer);
            //for (int i = 0; i < matchCount; i++)
            //{
            //    int index = resultBuffer[i];
            //    Tile tile = GetTileFromIndex(index);
            //    if (tile != null)
            //    {
            //        matchedTiles.Add(tile);
            //    }
            //}

            Profiler.EndSample();

            return matchedTiles;
        }


        // Recursive DFS implementation.
        // It is more readable and easier to understand
        // but can lead to stack overflow for large grids
        private void DFSRecursive(Tile currentTile, HashSet<Tile> visitedTiles, List<Tile> matchedTiles)
        {
            visitedTiles.Add(currentTile);
            matchedTiles.Add(currentTile);

            foreach (Tile neighbor in GetNeighbors(currentTile))
            {
                if (!visitedTiles.Contains(neighbor) && neighbor.TileData.Color == currentTile.TileData.Color)
                {
                    DFSRecursive(neighbor, visitedTiles, matchedTiles);
                }
            }
        }

        // Iterative DFS implementation.
        // It uses a stack to avoid recursion
        // and can handle larger grids without stack overflow.
        // It is less readable than the recursive version.
        private void DFSIterative(Tile currentTile, HashSet<Tile> visitedTiles, List<Tile> matchedTiles)
        {
            var stack = new Stack<Tile>();
            stack.Push(currentTile);

            while (stack.Count > 0)
            {
                var tile = stack.Pop();

                if (!visitedTiles.Contains(tile))
                {
                    visitedTiles.Add(tile);
                    matchedTiles.Add(tile);

                    foreach (Tile neighbor in GetNeighbors(tile))
                    {
                        if (neighbor.TileData.Color == tile.TileData.Color)
                        {
                            stack.Push(neighbor);
                        }
                    }
                }
            }
        }


        // Optimized iterative DFS implementation.
        // It is different from the previous one
        // because it has circuit breaker logic to avoid unnecessary processing
        // and combines the Add() and Contains() HashSet methods to reduce the number of calls (last if).
        public void DFSIterativeOptimized(Tile currentTile, HashSet<Tile> visitedTiles, List<Tile> matchedTiles)
        {
            if (currentTile == null || visitedTiles == null || matchedTiles == null)
            {
                return;
            }

            var stack = new Stack<Tile>();

            if (visitedTiles.Add(currentTile))
            {
                stack.Push(currentTile);
                matchedTiles.Add(currentTile);
            }
            else
            {
                return;
            }

            while (stack.Count > 0)
            {
                var tile = stack.Pop();
                var currentColor = tile.TileData.Color;

                foreach (Tile neighbor in GetNeighbors(tile))
                {
                    if (neighbor.TileData.Color.Equals(currentColor) && visitedTiles.Add(neighbor))
                    {
                        matchedTiles.Add(neighbor);
                        stack.Push(neighbor);
                    }
                }
            }
        }

        // This method is an optimized version of the iterative DFS
        // that uses a Span<int> to store the stack
        // and a Span<bool> to track visited tiles.
        // The stack memory is used instead of the heap
        // to avoid garbage collection overhead.
        // It is faster and more memory efficient
        // but less readable than the previous versions.
        private int DFSIterativeWithSpan(Tile startTile, Span<int> resultsBuffer)
        {
            var totalTilesCount = _board.Width * _board.Height;

            Span<int> stack = stackalloc int[totalTilesCount];
            Span<bool> visited = stackalloc bool[totalTilesCount];

            int stackPointer = 0;
            int matchCount = 0;

            stack[stackPointer++] = GetTileIndex(startTile);

            while (stackPointer > 0)
            {
                int currentTileIndex = stack[--stackPointer];

                if (visited[currentTileIndex])
                {
                    continue;
                }

                visited[currentTileIndex] = true;

                if (matchCount < resultsBuffer.Length)
                {
                    resultsBuffer[matchCount++] = currentTileIndex;
                }
                else
                {
                    Debug.LogWarning("Warning: resultsBuffer is full!");
                }

                Tile currentTile = GetTileFromIndex(currentTileIndex);
                TileColor currentColor = currentTile.TileData.Color;

                foreach (Tile neighbor in GetNeighborsOptimized(currentTile))
                {
                    int neighborIndex = GetTileIndex(neighbor);

                    if (!visited[neighborIndex] && neighbor.TileData.Color.Equals(currentColor))
                    {
                        if (stackPointer < stack.Length)
                        {
                            stack[stackPointer++] = neighborIndex;
                        }
                        else
                        {
                            Debug.LogError("Error: Stack overflow!");
                        }
                    }
                }
            }

            return matchCount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetTileIndex(Tile tile)
        {
            return tile.Row * _board.Height + tile.Column;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Tile GetTileFromIndex(int index)
        {
            int row = index / _board.Height;
            int col = index % _board.Height;

            if (row < 0 || row >= _board.Height || col < 0 || col >= _board.Width)
            {
                return null;
            }

            return _board.GetTileAt(row, col);
        }

        // This method is used to get the neighbors of a tile.
        // It uses a nested loop to check the four possible directions (up, down, left, right).
        // It creates a new list each time it is called,
        // which can be inefficient for large grids.
        private IEnumerable<Tile> GetNeighbors(Tile tile)
        {
            var neighbors = new List<Tile>();
            int[,] offsets = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } };

            for (int i = 0; i < offsets.GetLength(0); i++)
            {
                int newRow = tile.Row + offsets[i, 0];
                int newColumn = tile.Column + offsets[i, 1];

                if (newRow >= 0 && newRow < _board.Height &&
                    newColumn >= 0 && newColumn < _board.Width)
                {
                    Tile neighbor = _board.GetTileAt(newRow, newColumn);
                    if (neighbor != null)
                    {
                        neighbors.Add(neighbor);
                    }
                }
            }

            return neighbors;
        }

        // This method avoids creating a new list and uses a yield return
        // to return neighbors one by one.
        // It also uses a static array for offsets to avoid
        // creating a new array each time.
        private IEnumerable<Tile> GetNeighborsOptimized(Tile tile)
        {
            if (tile == null) yield break;

            for (int i = 0; i < NeighborOffsets.GetLength(0); i++)
            {
                int newRow = tile.Row + NeighborOffsets[i, 0];
                int newColumn = tile.Column + NeighborOffsets[i, 1];

                if (newRow >= 0 && newRow < _board.Height &&
                    newColumn >= 0 && newColumn < _board.Width)
                {
                    Tile neighbor = _board.GetTileAt(newRow, newColumn);
                    if (neighbor != null)
                    {
                        yield return neighbor;
                    }
                }
            }
        }
    }
}