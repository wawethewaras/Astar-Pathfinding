using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace Astar2DPathFinding.Mika { 
    public static class AStar {


        //Max nodes to count. This will prevent counting the whole grid if target is unreachable.
        private const int closedListMaxCount =  100000;

        /// <summary>
        /// Creates path from startPos to targetPos using A*.
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="targetPos"></param>
        /// <returns></returns>
        public static Vector2[] FindPath(Node startNode, Node goalNode)
        {
            //How long will path founding take
            Stopwatch sw = new Stopwatch();
            //For showing path counting process. Resets grid.

            if (PathfindingGrid.Instance.showPathSearchDebug) {
                            sw.Start();

                PathfindingGrid.openList.Clear();
                PathfindingGrid.closedList.Clear();
                PathfindingGrid.pathFound = false;
            }

            PathfindingGrid.Instance.ResetNodes();
            int nodeCount = 0;

            Heap<Node> openSet = new Heap<Node>(PathfindingGrid.Maxsize);

            if (goalNode.walkable == NodeType.obstacle || startNode.walkable == NodeType.obstacle)
            {
                UnityEngine.Debug.Log("Start or goal inside collider.");
                return null;
            }


            openSet.Add(startNode);
            //For showing path counting 
            if (PathfindingGrid.Instance.showGrid)
            {
                PathfindingGrid.openList.Add(startNode);
            }
            nodeCount++;

            //Node[] neighbours;
            Node neighbour;
            Node currentNode;

            while (openSet.Count > 0) {
                currentNode = openSet.RemoveFirst();
                currentNode.inClosedList = true;

                //For showing path counting 
                if (PathfindingGrid.Instance.showGrid)
                {
                    PathfindingGrid.closedList.Add(currentNode);
                }


                if (currentNode == goalNode) {
                    //For testing path calculation. Can be removed from final version.
                    sw.Stop();
                    if (PathfindingGrid.Instance.showPathSearchDebug) {
                        Vector2[] path = RetracePath(startNode, goalNode);
                        UnityEngine.Debug.Log("<color=Blue>Path found! </color> Time took to calculate path: " + sw.Elapsed + "ms. Number of nodes counted " + nodeCount + ". Path lenght: " + path.Length + ". Heurastics: " + PathfindingGrid.Instance.heuristicMethod);
                        PathfindingGrid.pathFound = true;
                    }

                    return RetracePath(startNode, goalNode);
                }

                //if (openSet.Count > closedListMaxCount) {
                //    return null;
                //}
                //UnityEngine.Debug.Log("Neigg" + currentNode.neighbours[0].gridX);
                //PathfindingGrid.Instance.GetNeighbours(currentNode);

                for (int i = 0; i < currentNode.neighbours.Length; i++) {
                    neighbour = currentNode.neighbours[i];

                    //Calculate obstacles while creating path
                    //CheckIfNodeIsObstacle(neighbour);

                    if (neighbour == null || neighbour.walkable == NodeType.obstacle || neighbour.inClosedList) {
                        continue;
                    }

                    int newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.movementPenalty;
                    if (newCostToNeighbour < neighbour.gCost || neighbour.inOpenSet == false) {
                        neighbour.gCost = newCostToNeighbour;
                        neighbour.hCost = Mathf.RoundToInt(GetDistance(neighbour, goalNode) * PathfindingGrid.Instance.heuristicMultiplier);
                        neighbour.parent = currentNode;

                        if (neighbour.inOpenSet == false) {
                            openSet.Add(neighbour);
                            neighbour.inOpenSet = true;

                            //For showing path counting 
                            if (PathfindingGrid.Instance.showGrid) {
                                PathfindingGrid.openList.Add(neighbour);
                            }
                            nodeCount++;
                        }
                        else {
                            openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }
            if (PathfindingGrid.Instance.showPathSearchDebug) {
                sw.Stop();
                UnityEngine.Debug.Log("<color=red>Path not found! </color> Time took to calculate path: " + sw.ElapsedMilliseconds + "ms.");
            }
            return null;
        }

        /// <summary>
        /// Creates path from startNode to targetNode
        /// </summary>
        /// <param name="startNode"></param>
        /// <param name="targetNode"></param>
        /// <returns></returns>
        public static Vector2[] RetracePath(Node startNode, Node targetNode) {
            List<Vector2> path = new List<Vector2>();
            Node currentNode = targetNode;

            while (currentNode != startNode) {
                path.Add(currentNode.worldPosition);
                currentNode = currentNode.parent;
            }

            Vector2[] waypoints = new Vector2[path.Count];

            int pathLength = 0;
            for (int i = path.Count - 1; i >= 0; i--) {
                waypoints[pathLength] = path[i];
                pathLength++;
            }

            //Vector2[] waypoints = path.ToArray();
            //Array.Reverse(waypoints);
            return waypoints;
        }

        /// <summary>
        ///Way to reduce number of nodes from path. Only adds nodes that have new direction.This is useful if you have grid based game, but more nodes is better for non grid based game so path looks smooter.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// 
        public static Vector2[] SimplifyPath(List<Node> path)
        {
            List<Vector2> waypoints = new List<Vector2>();
            Vector2 directionOld = Vector2.zero;

            for (int i = 1; i < path.Count; i++)
            {
                Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
                if (directionNew != directionOld)
                {
                    waypoints.Add(path[i].worldPosition);
                }
                directionOld = directionNew;
            }
            waypoints.Add(path[path.Count - 1].worldPosition);
            return waypoints.ToArray();
        }

        /// <summary>
        /// Reduces number of nodes from path. Only adds nodes that are blocked by obstacle. This is useful if you want to have short path, but you can create smoother looking path using dynamic collider check.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Vector2[] pathSmooter(Vector2[] path) {
            List<Vector2> waypoints = new List<Vector2>();
            int currentNode = 0;
            waypoints.Add(path[0]);

            int security = 0;
            for (int i = 1; i < path.Length; i++)
            {
                security++;
                if (security >= 1000) {
                    UnityEngine.Debug.LogError("Crash");
                    break;
                }
                bool cantSeeTarget = Physics2D.Linecast(path[currentNode], path[i], PathfindingGrid.Instance.unwalkableMask);
                if (cantSeeTarget)
                {
                    waypoints.Add(path[i - 1]);
                    currentNode = i - 1;

                }

            }
            waypoints.Add(path[path.Length-1]);
            return waypoints.ToArray();

        }

        private static int GetDistance(Node nodeA, Node nodeB)
        {
            if (PathfindingGrid.Instance.heuristicMethod == PathfindingGrid.Heuristics.VectorMagnitude)
            {
                return GetDistance2(nodeA, nodeB);
            }
            else if (PathfindingGrid.Instance.heuristicMethod == PathfindingGrid.Heuristics.Euclidean)
            {
                return GetDistance3(nodeA, nodeB);
            }
            else {
                return GetDistance1(nodeA, nodeB);
            }
        }

        /// <summary>
        /// Gets heuristic distance from nodeA to nodeB using Manhattan distance
        /// </summary>
        /// <param name="nodeA"></param>
        /// <param name="nodeB"></param>
        /// <returns></returns>
        private static int GetDistance1(Node nodeA, Node nodeB) {
            int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
            int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);
        }

        /// <summary>
        /// Gets heuristic distance from nodeA to nodeB using basic distance
        /// </summary>
        /// <param name="nodeA"></param>
        /// <param name="nodeB"></param>
        /// <returns></returns>
        private static int GetDistance2(Node nodeA, Node nodeB) {
            int distance = (int)((nodeA.worldPosition - nodeB.worldPosition).magnitude);
            return distance;
        }

        /// <summary>
        /// Gets heuristic distance from nodeA to nodeB using Euclidean distance
        /// </summary>
        /// <param name="nodeA"></param>
        /// <param name="nodeB"></param>
        /// <returns></returns>
        private static int GetDistance3(Node nodeA, Node nodeB)
        {
            int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
            int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

            return 10 * (int)Mathf.Sqrt(dstX*dstX + dstY * dstY);
        }
    }
}