using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using System;

public static class AStar {

    //Using this value can decide whether algoritmin should work more like dijkstra or greedy best first. If value is 1 this works like traditional A*.
    private const float heurasticMultiplier = 2f;


    /// <summary>
    /// Creates path from startPos to targetPos using A*.
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    public static Vector3[] FindPath(Node startNode, Node goalNode)
    {
        //How long will path founding take
        Stopwatch sw = new Stopwatch();
        sw.Start();


        Heap<Node> openSet = new Heap<Node>(Grid.instance.Maxsize);
        Heap<Node> closedSet = new Heap<Node>(Grid.instance.Maxsize);

        //Check if goal is inside collider
        //Collider2D[] colliders = Physics2D.OverlapCircleAll(targetNode.worldPosition, Grid.instance.nodeRadius, Grid.instance.unwalkableMask);
        if (/*colliders.Length > 0 || */goalNode.walkable == false || startNode.walkable == false)
        {
            UnityEngine.Debug.Log("Start or goal inside collider");
            return null;
        }


        openSet.Add(startNode);
        //For showing path counting 
        if (Grid.instance.showGrid)
        {
            Grid.openList.Add(startNode);
        }


        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            //For showing path counting 
            if (Grid.instance.showGrid)
            {
                Grid.closedList.Add(currentNode);
            }


            if (currentNode == goalNode)
            {
                //For testing path calculation. Can be removed from final version.
                sw.Stop();
                if (Grid.instance.showPathSearchDebug)
                {
                    Vector3[] path = RetracePath(startNode, goalNode);
                    int pathLenght = 0;
                    for (int i = 0; i < path.Length - 1; i++)
                    {
                        pathLenght += Mathf.RoundToInt(Vector3.Distance(path[i], path[i + 1]));
                    }
                    UnityEngine.Debug.Log("Time took to calculate path: " + sw.ElapsedMilliseconds + "ms. Number of nodes counted " + Grid.openList.Count + ". Path lenght: " + pathLenght);
                    Grid.pathFound = true;
                }

                return RetracePath(startNode, goalNode);
            }

            Node[] neighbours = Grid.instance.GetNeighbours(currentNode);

            for (int i = 0; i < neighbours.Length; i++)
            {
                Node neighbour = neighbours[i];

                //Calculate obstacles while creating path
                //CheckIfNodeIsObstacle(neighbour);

                if (neighbour == null || neighbour.walkable == false || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.movementPenalty;
                if (newCostToNeighbour < neighbour.gCost || openSet.Contains(neighbour) == false)
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = Mathf.RoundToInt(GetDistance(neighbour, goalNode) * heurasticMultiplier);
                    neighbour.parent = currentNode;

                    if (openSet.Contains(neighbour) == false)
                    {
                        openSet.Add(neighbour);

                        //For showing path counting 
                        if (Grid.instance.showGrid)
                        {
                            Grid.openList.Add(neighbour);
                        }
                    }
                    else {
                        openSet.UpdateItem(neighbour);
                    }
                }
            }
        }
        return null;
    }

    ///// <summary>
    ///// Creates path from startPos to targetPos using A*.
    ///// </summary>
    ///// <param name="startPos"></param>
    ///// <param name="targetPos"></param>
    ///// <returns></returns>
    //public static Vector3[] FindPath(Vector3 startPos, Vector3 targetPos)
    //{
    //    Stopwatch sw = new Stopwatch();
    //    sw.Start();

    //    Node startNode = Grid.instance.ClosestNodeFromWorldPoint(startPos);
    //    Node targetNode = Grid.instance.ClosestNodeFromWorldPoint(targetPos);
    //    Heap<Node> openSet = new Heap<Node>(Grid.instance.Maxsize);
    //    Heap<Node> closedSet = new Heap<Node>(Grid.instance.Maxsize);

    //    //Check if goal is inside collider
    //    //Collider2D[] colliders = Physics2D.OverlapCircleAll(targetNode.worldPosition, Grid.instance.nodeRadius, Grid.instance.unwalkableMask);
    //    if (/*colliders.Length > 0 || */targetNode.walkable == false || startNode.walkable == false)
    //    {
    //        UnityEngine.Debug.Log("Start or goal inside collider");
    //        return null;
    //    }

    //    openSet.Add(startNode);
    //    //For showing path counting 
    //    if (Grid.instance.showGrid) {
    //        Grid.openList.Add(startNode);
    //    }

    //    Grid.instance.GetNeighbours(startNode);

    //    while (openSet.Count > 0)
    //    {
    //        Node node = openSet.RemoveFirst();
    //        closedSet.Add(node);

    //        //For showing path counting 
    //        if (Grid.instance.showGrid) {
    //            Grid.closedList.Add(node);
    //        }


    //        if (node == targetNode) {
    //            //For testing path calculation. Can be removed from final version.
    //            sw.Stop();
    //            Vector3[] path = RetracePath(startNode, targetNode);
    //            int pathLenght = 0;
    //            for (int i = 0; i < path.Length - 1; i++)
    //            {
    //                pathLenght += Mathf.RoundToInt(Vector3.Distance(path[i], path[i + 1]));
    //            }
    //            UnityEngine.Debug.Log("Time took to calculate path: " + sw.ElapsedMilliseconds + "ms. Number of nodes counted " + Grid.openList.Count + ". Path lenght: " + pathLenght);
    //            Grid.pathFound = true;

    //            return RetracePath(startNode, targetNode);
    //        }

    //        Node[] neighbours = Grid.instance.GetNeighbours(node);
    //        Node neighbour;

    //        for (int i = 0; i < neighbours.Length; i++)
    //        {
    //            neighbour = neighbours[i];

    //            //Calculate obstacles while creating path
    //            //CheckIfNodeIsObstacle(neighbour);

    //            if (neighbour == null || !neighbour.walkable || closedSet.Contains(neighbour)) {
    //                continue;
    //            }

    //            int newCostToNeighbour = node.gCost + GetDistance(node, neighbour) + neighbour.movementPenalty;
    //            if (newCostToNeighbour < neighbour.gCost || openSet.Contains(neighbour) == false) {
    //                neighbour.gCost = newCostToNeighbour;
    //                neighbour.hCost = Mathf.RoundToInt(GetDistance(neighbour, targetNode) * heurasticMultiplier);
    //                neighbour.parent = node;

    //                if (!openSet.Contains(neighbour)) {
    //                    openSet.Add(neighbour);
    //                    //For showing path counting 
    //                    if (Grid.instance.showGrid) {
    //                        Grid.openList.Add(neighbour);
    //                    }
    //                }
    //                else {
    //                    openSet.UpdateItem(neighbour);
    //                }
    //            }
    //        }
    //    }
    //    return null;
    //}

    /// <summary>
    /// Creates path from startNode to targetNode
    /// </summary>
    /// <param name="startNode"></param>
    /// <param name="targetNode"></param>
    /// <returns></returns>
    public static Vector3[] RetracePath(Node startNode, Node targetNode)
    {
        List<Vector3> path = new List<Vector3>();
        Node currentNode = targetNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.worldPosition);
            currentNode = currentNode.parent;
        }
        Vector3[] waypoints = path.ToArray();
        Array.Reverse(waypoints);
        return waypoints;
    }

    /// <summary>
    ///Way to reduce number of nodes from path. Only adds nodes that have new direction.This is useful if you have grid based game, but more nodes is better for non grid based game so path looks smooter.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    /// 
    public static Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
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
    public static Vector3[] pathSmooter(Vector3[] path) {
        List<Vector3> waypoints = new List<Vector3>();
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
            bool cantSeeTarget = Physics2D.Linecast(path[currentNode], path[i], Grid.instance.unwalkableMask);
            if (cantSeeTarget)
            {
                waypoints.Add(path[i - 1]);
                currentNode = i - 1;

            }

        }
        waypoints.Add(path[path.Length-1]);
        return waypoints.ToArray();

    }
    /// <summary>
    /// Gets heuristic distance from nodeA to nodeB using Manhattan distance
    /// </summary>
    /// <param name="nodeA"></param>
    /// <param name="nodeB"></param>
    /// <returns></returns>
    static int GetDistance(Node nodeA, Node nodeB) {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }


}
