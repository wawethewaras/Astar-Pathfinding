using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;

public static class AStar
{



    //Using this value can decide whether algoritmin should work more like dijkstra or greedy best first. If value is 1 this works like traditional astar
    private const float heurasticMultiplier = 2f;

    //Ment to be used with pathrequester
    //public static void StartFindPath(Vector3 startPos, Vector3 targetPos, MonoBehaviour requester)
    //{
    //    requester.StartCoroutine(FindPathRequest(startPos, targetPos));
    //}

    //public static IEnumerator FindPathRequest(Vector3 startPos, Vector3 targetPos)
    //{
    //    Stopwatch sw = new Stopwatch();
    //    sw.Start();

    //    Node startNode = Grid.instance.NodeFromWorldPoint(startPos);
    //    Node targetNode = Grid.instance.PlayerNodeFromWorldPoint(targetPos);
    //    Heap<Node> openSet = new Heap<Node>(Grid.instance.Maxsize);
    //    Heap<Node> closedSet = new Heap<Node>(Grid.instance.Maxsize);
    //    bool pathSuccess = false;
    //    Vector3[] waypoints = new Vector3[0];
    //    //Check if goal is inside collider
    //    //Collider2D[] colliders = Physics2D.OverlapCircleAll(targetNode.worldPosition, Grid.instance.nodeRadius, Grid.instance.unwalkableMask);
    //    if (/*colliders.Length > 0 || */targetNode.walkable == false)
    //    {
    //        UnityEngine.Debug.Log("Goal inside collider");
    //        yield return null;
    //    }

    //    ////Check if can see target and is there need to calculate path
    //    //bool cantSeeTarget = Physics2D.Linecast(startPos, targetPos, Grid.instance.unwalkableMask);
    //    //if (cantSeeTarget == false)
    //    //{
    //    //    Debug.Log("Can see target");
    //    //    Vector3[] path = new Vector3[2];
    //    //    path[0] = startPos;
    //    //    path[1] = targetPos;
    //    //    return path;
    //    //}

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
    //        if (Grid.instance.showGrid)
    //        {
    //            Grid.closedList.Add(node);
    //        }


    //        if (node == targetNode)
    //        {
    //            sw.Stop();
    //            UnityEngine.Debug.Log("Time took to calculate path: " + sw.ElapsedMilliseconds + "ms. Number of nodes counted " + Grid.openList.Count);
    //            Grid.pathFound = true;
    //            pathSuccess = true;
    //            break;
    //        }
    //        Node[] neighbours = Grid.instance.GetNeighbours(node);
    //        Node neighbour;
    //        //foreach (Node neighbour in Grid.instance.GetNeighbours(node))
    //        for (int i = 0; i < neighbours.Length; i++)
    //        {
    //            neighbour = neighbours[i];
    //            if (neighbour == null) {
    //                continue;
    //            }
    //            //Calculate obstacles while creating path
    //            //CheckIfNodeIsObstacle(neighbour);

    //            if (!neighbour.walkable || closedSet.Contains(neighbour))
    //            {
    //                continue;
    //            }
    //            //if (node.parent == neighbour) {
    //            //    continue;
    //            //}
    //            int newCostToNeighbour = node.gCost + GetDistance(node, neighbour) + neighbour.movementPenalty;
    //            if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
    //            {
    //                neighbour.gCost = newCostToNeighbour;
    //                //Heurastics have high value so algoritmin works bit more like Greedy best first
    //                neighbour.hCost = Mathf.RoundToInt(GetDistance(neighbour, targetNode)*heurasticMultiplier);
    //                neighbour.parent = node;

    //                if (!openSet.Contains(neighbour))
    //                {
    //                    openSet.Add(neighbour);
    //                    //For showing path counting 
    //                    if (Grid.instance.showGrid)
    //                    {
    //                        Grid.openList.Add(neighbour);
    //                    }
    //                }
    //                else {
    //                    openSet.UpdateItem(neighbour);
    //                }


    //            }
    //        }
    //    }
    //    yield return null;

    //    if (pathSuccess)
    //    {
    //        waypoints = RetracePath(startNode, targetNode);
    //    }
    //    PathRequestManager.instance.FinishedProcessingPath(waypoints, pathSuccess);
    //}

    public static Vector3[] FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Node startNode = Grid.instance.PlayerNodeFromWorldPoint(startPos);
        Node targetNode = Grid.instance.PlayerNodeFromWorldPoint(targetPos);
        Heap<Node> openSet = new Heap<Node>(Grid.instance.Maxsize);
        Heap<Node> closedSet = new Heap<Node>(Grid.instance.Maxsize);

        //Check if goal is inside collider
        //Collider2D[] colliders = Physics2D.OverlapCircleAll(targetNode.worldPosition, Grid.instance.nodeRadius, Grid.instance.unwalkableMask);
        if (/*colliders.Length > 0 || */targetNode.walkable == false || startNode.walkable == false)
        {
            UnityEngine.Debug.Log("Start or goal inside collider");
            return null;
        }

        ////Check if can see target and is there need to calculate path
        //bool cantSeeTarget = Physics2D.Linecast(startPos, targetPos, Grid.instance.unwalkableMask);
        //if (cantSeeTarget == false)
        //{
        //    Debug.Log("Can see target");
        //    Vector3[] path = new Vector3[2];
        //    path[0] = startPos;
        //    path[1] = targetPos;
        //    return path;
        //}

        openSet.Add(startNode);
        //For showing path counting 
        if (Grid.instance.showGrid) {
            Grid.openList.Add(startNode);
        }

        Grid.instance.GetNeighbours(startNode);

        while (openSet.Count > 0)
        {
            Node node = openSet.RemoveFirst();
            closedSet.Add(node);

            //For showing path counting 
            if (Grid.instance.showGrid) {
                Grid.closedList.Add(node);
            }


            if (node == targetNode) {
                //For testing path calculation. Can be removed from final version.
                sw.Stop();
                Vector3[] path = RetracePath(startNode, targetNode);
                //int pathLenght = 0;
                //for (int i = 0; i < path.Length - 1; i++)
                //{
                //    pathLenght += Mathf.RoundToInt(Vector3.Distance(path[i], path[i + 1]));
                //}
                //UnityEngine.Debug.Log("Time took to calculate path: " + sw.ElapsedMilliseconds + "ms. Number of nodes counted " + Grid.openList.Count + ". Path lenght: " + pathLenght);
                Grid.pathFound = true;

                return RetracePath(startNode, targetNode);
            }

            Node[] neighbours = Grid.instance.GetNeighbours(node);
            Node neighbour;
            //foreach (Node neighbour in Grid.instance.GetNeighbours(node))
            for (int i = 0; i < neighbours.Length; i++)
            {
                neighbour = neighbours[i];

                //Calculate obstacles while creating path
                //CheckIfNodeIsObstacle(neighbour);

                if (neighbour == null || !neighbour.walkable || closedSet.Contains(neighbour)) {
                    continue;
                }

                int newCostToNeighbour = node.gCost + GetDistance(node, neighbour) + neighbour.movementPenalty;
                if (newCostToNeighbour < neighbour.gCost || openSet.Contains(neighbour) == false) {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = Mathf.RoundToInt(GetDistance(neighbour, targetNode) * heurasticMultiplier);
                    neighbour.parent = node;

                    if (!openSet.Contains(neighbour)) {
                        openSet.Add(neighbour);
                        //For showing path counting 
                        if (Grid.instance.showGrid) {
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

    public static Vector3[] RetracePath(Node startNode, Node targetNode)
    {
        if (startNode == targetNode) {
            Vector3[] shortPath = new Vector3[1];
            shortPath[0] = targetNode.worldPosition;
            return shortPath;
        }
        List<Vector3> path = new List<Vector3>();
        Node currentNode = targetNode;
        while (currentNode != startNode)
        {
            path.Add(currentNode.worldPosition);
            currentNode = currentNode.parent;
        }
        //path.Reverse();
        Vector3[] waypoints = path.ToArray();
        Array.Reverse(waypoints);
        return waypoints;


    }
    public static Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
                waypoints.Add(path[i].worldPosition);

        }
        waypoints.Add(path[path.Count - 1].worldPosition);
        return waypoints.ToArray();
    }

    //public static Vector3[] SimplifyPath(List<Node> path)
    //{
    //    List<Vector3> waypoints = new List<Vector3>();
    //    Vector2 directionOld = Vector2.zero;

    //    for (int i = 1; i < path.Count; i++)
    //    {
    //        Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
    //        if (directionNew != directionOld)
    //        {
    //            waypoints.Add(path[i].worldPosition);
    //        }
    //        directionOld = directionNew;
    //    }
    //    waypoints.Add(path[path.Count - 1].worldPosition);
    //    return pathSmooter(waypoints.ToArray());
    //}

    public static Vector3[] pathSmooter(Vector3[] path) {
        List<Vector3> waypoints = new List<Vector3>();
        int currentNode = 0;
        waypoints.Add(path[0]);

        int security = 0;
        for (int i = 1; i < path.Length; i++)
        {
            security++;
            if (security >= 100) {
                UnityEngine.Debug.LogError("Crash");
                break;
            }
            //bool cantSeeTarget = (Physics2D.CircleCastAll(path[currentNode], Grid.instance.nodeRadius, path[i], Vector3.Distance(path[currentNode], path[i]),Grid.instance.unwalkableMask)).Length > 0;
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

    static int GetDistance(Node nodeA, Node nodeB) {
        int dstX = Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    public static int Abs(int value)
    {
        if (value >= 0)
            return value;
        return -value;
    }




}
