using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Battle
{
    // TODO: using linq is slow, if there are performance issues, factor it out
    /// <summary>
    /// A BattleGrid maintains a spatial relationship between a series of GridSquares using a 2D array. 
    /// </summary>
    public class BattleGrid
    {
        public GridSquare[,] squares;

        /// <summary>
        /// Size, in squares, of this BattleGrid.
        /// </summary>
        public Vector2Int Size => new Vector2Int(squares.GetLength(0), squares.GetLength(1));

        /// <summary>
        /// Width, in squares, of this BattleGrid.
        /// </summary>
        public int Width => squares.GetLength(0);

        internal List<GridSquare> GetSquareList()
        {
            List<GridSquare> squareList = new List<GridSquare>();

            foreach (GridSquare square in squares)
            {
                squareList.Add(square);
            }

            return squareList;
        }

        /// <summary>
        /// Height, in squares, of this BattleGrid.
        /// </summary>
        public int Height => squares.GetLength(1);

        public BattleGrid(int width, int height)
        {
            squares = new GridSquare[width, height];
        }

        public GridSquare this[int x, int y]
        {
            get => squares[x, y];
            set
            {
                squares[x, y] = value;
                value.BindToGrid(this, x, y);
            }
        }

        public void DestroyImmediate()
        {
            foreach(GridSquare square in squares)
            {
                if(square) GameObject.DestroyImmediate(square.gameObject);
            }
        }

        /// <summary>
        /// Gets the cardinal direction pointing roughly from 'from' and pointing towards 'to.'
        /// A bias can be optionally provided to break ties.
        /// </summary>
        /// <param name="from">The starting point for the direction.</param>
        /// <param name="to">The endingpoint for the direction.</param>
        /// <param name="bias">Bias value used to break ties. Defaults to GridDirection.East.</param>
        /// <returns>The rough direction that does from 'from' to 'to.'</returns>
        public GridDirection GetDirectionFromTo(GridSquare from, GridSquare to, GridDirection bias = GridDirection.East)
        {
            if (!Contains(from))
            {
                throw new Exception("GetDirectionFromTo Error: argument 'from' is not contained by the grid.");
            }

            if(!Contains(to))
            {
                throw new Exception("GetDirectionFromTo Error: argument 'to' is not contained by the grid.");
            }

            if(from == to)
            {
                Debug.LogWarning("Warning: GetDirectionFromTo arguments 'from' and 'to' are the same square, returning 'bias' argument value.");
                return bias;
            }

            int horizontalDelta = to.X - from.X;
            int verticalDelta = to.Y - from.Y;

            bool useHorizontal = Math.Abs(horizontalDelta) > Math.Abs(verticalDelta);

            if(Math.Abs(horizontalDelta) == Math.Abs(verticalDelta))
            {
                useHorizontal = bias == GridDirection.East || bias == GridDirection.West;
            }

            GridDirection result;

            if(useHorizontal)
            {
                if(horizontalDelta > 0)
                {
                    result = GridDirection.East;
                }
                else
                {
                    result = GridDirection.West;
                }
            }
            else
            {
                if (verticalDelta > 0)
                {
                    result = GridDirection.North;
                }
                else
                {
                    result = GridDirection.South;
                }
            }

            return result;
        }

        /// <summary>
        /// Searches all GridSquares for entities and collects them in a List.
        /// </summary>
        /// <returns>A List of all Entities stored in the GridSquares of this Grid.</returns>
        public List<Entity> FindEntities()
        {
            List<Entity> entities = new List<Entity>();

            foreach(GridSquare square in squares)
            {
                entities.AddRange(square.Entities);
            }

            return entities;
        }

        /// <summary>
        /// Uses A* to find the shortest unblocked path between start and goal.
        /// </summary>
        /// <param name="start">The starting GridSquare.</param>
        /// <param name="goal">The destination GridSquare.</param>
        /// <returns>A list with the path to get from start to goal, or null if no path was found.</returns>
        public List<GridSquare> CalculatePath(GridSquare start, GridSquare goal)
        {
            if (!Contains(start))
            {
                Debug.LogError($"Failed to calculate BattleGrid path, GridSquare from: {start} is not on the grid.");
                return null;
            }

            if (!Contains(goal))
            {
                Debug.LogError($"Failed to calculate BattleGrid path, GridSquare from: {goal} is not on the grid.");
                return null;
            }

            // instantiate all nodes
            List<PathNode> pathNodes = new List<PathNode>();

            foreach(GridSquare square in squares)
            {
                if(square.IsPassable() || square == start || square == goal) pathNodes.Add(new PathNode(square, goal));
            }

            pathNodes.ForEach(node => node.CalculateNeighbors(pathNodes));

            // create the open set
            List<PathNode> openSet = new List<PathNode>();

            // configure start node
            PathNode startNode = pathNodes.Find(node => node.gridSquare == start);

            openSet.Add(startNode);
            startNode.gScore = 0;
            startNode.fScore = Heuristic(start, goal);

            while (openSet.Count > 0)
            {
                PathNode current = openSet.Min();
                if(current.gridSquare == goal)
                {
                    List<PathNode> nodePath = new List<PathNode>();
                    nodePath.Add(current);

                    PathNode previous = current.previous;

                    while (previous != null)
                    {
                        nodePath.Add(previous);
                        previous = previous.previous;
                    }

                    List<GridSquare> finalPath = new List<GridSquare>();

                    for(int i = nodePath.Count - 1; i >= 0; i--)
                    {
                        finalPath.Add(nodePath[i].gridSquare);
                    }

                    return finalPath;
                }

                openSet.Remove(current);

                foreach(PathNode neighbor in current.neighbors)
                {
                    if(current.gScore + 1 < neighbor.gScore)
                    {
                        neighbor.previous = current;
                        neighbor.gScore = current.gScore + 1;
                        neighbor.fScore = neighbor.gScore + Heuristic(neighbor.gridSquare, goal);
                        if(!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                        }
                    }
                }
            }

            Debug.LogWarning("no path found");
            return null;
        }

        private class PathNode : IComparable<PathNode>
        {
            public PathNode previous;
            public List<PathNode> neighbors;
            public GridSquare gridSquare;

            /// <summary>
            /// Cost of the cheapest path from start to this node currently known 
            /// </summary>
            public int gScore;

            /// <summary>
            /// Estimated cost of path that runs from start, passes through this node, and reaches the goal
            /// </summary>
            public int fScore;

            public PathNode(GridSquare current, GridSquare goal)
            {
                gridSquare = current;
                previous = null;
                gScore = int.MaxValue;
                fScore = int.MaxValue;
            }

            public int CompareTo(PathNode other)
            {
                return fScore - other.fScore;
            }

            //TODO: Hack with bad performance, target later if there are issues
            public void CalculateNeighbors(List<PathNode> pathNodes)
            {
                neighbors = new List<PathNode>();
                gridSquare.GetNeighbors().ForEach(gridSquare =>
                {
                    PathNode found = pathNodes.Find(node => node.gridSquare == gridSquare);
                    if(found != null)
                    {
                        neighbors.Add(found);
                    }
                });
            }
        }


        private static int Heuristic(GridSquare start, GridSquare goal)
        {
            return Mathf.Abs(start.X - goal.X) + (start.Y - goal.Y);
        }

        /// <summary>
        /// Checks if the given GridSquare is somewhere in this BattleGrid.
        /// </summary>
        /// <param name="testSquare">The GridSquare to check for.</param>
        /// <returns>True if the GridSquare is found, false otherwise.</returns>
        public bool Contains(GridSquare testSquare)
        {
            foreach (GridSquare square in squares)
            { 
                if (square == testSquare)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks whether the given position is within the bounds of this BattleGrid.
        /// </summary>
        /// <param name="position">The position to check.</param>
        /// <returns>True if the given position is in bounds, false otherwise.</returns>
        public bool HasPosition(Vector2Int position)
        {
            return HasPosition(position.x, position.y);
        }

        /// <summary>
        /// Checks whether the given position is within the bounds of this BattleGrid.
        /// </summary>
        /// <param name="x">X component of the position to check.</param>
        /// <param name="y">Y component of the position to check.</param>
        /// <returns>True if the given position is in bounds, false otherwise.</returns>
        public bool HasPosition(int x, int y)
        {
            return x >= 0 && x < squares.GetLength(0) && y >= 0 && y < squares.GetLength(1);
        }
    }
}
