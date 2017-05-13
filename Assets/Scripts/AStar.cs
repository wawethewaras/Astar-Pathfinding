using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class AStar
{

    //For showing calculated path. Should be removed from final version.
    public static List<Node> openList = new List<Node>();
    public static List<Node> closedList = new List<Node>();
    public static bool pathFound;

    //Using this value can decide whether algoritmin should work more like dijkstra or greedy best first. If value is 1 this works like traditional astar
    private const float heurasticMultiplier = 2f;


    public static Vector3[] FindPath(Vector3 startPos, Vector3 targetPos)
    {
        openList.Clear();
        closedList.Clear();
        pathFound = false;


        Node startNode = Grid.instance.NodeFromWorldPoint(startPos);
        Node targetNode = Grid.instance.NodeFromWorldPoint(targetPos);
        Heap<Node> openSet = new Heap<Node>(Grid.instance.Maxsize);
        Heap<Node> closedSet = new Heap<Node>(Grid.instance.Maxsize);

        //Check if goal is inside collider
        //Collider2D[] colliders = Physics2D.OverlapCircleAll(targetNode.worldPosition, Grid.instance.nodeRadius, Grid.instance.unwalkableMask);
        if (/*colliders.Length > 0 || */targetNode.walkable == false)
        {
            Debug.Log("Goal inside collider");
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
        openList.Add(startNode);
        Grid.instance.GetNeighbours(startNode);

        while (openSet.Count > 0)
        {
            Node node = openSet.RemoveFirst();
            closedSet.Add(node);

            //For showing path counting 
            closedList.Add(node);

            if (node == targetNode)
            {
                pathFound = true;
                return RetracePath(startNode, targetNode);
            }
            Node[] neighbours = Grid.instance.GetNeighbours(node);
            Node neighbour;
            //foreach (Node neighbour in Grid.instance.GetNeighbours(node))
            for (int i = 0; i < neighbours.Length; i++)
            {
                neighbour = neighbours[i];
                if (neighbour == null) {
                    break;
                }
                //Calculate obstacles while creating path
                //CheckIfNodeIsObstacle(neighbour);

                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }
                //if (node.parent == neighbour) {
                //    continue;
                //}
                int newCostToNeighbour = node.gCost + GetDistance(node, neighbour) + neighbour.movementPenalty;
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    //Heurastics have high value so algoritmin works bit more like Greedy best first
                    neighbour.hCost = Mathf.RoundToInt(GetDistance(neighbour, targetNode)*heurasticMultiplier);
                    neighbour.parent = node;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                        //For showing path counting 
                        openList.Add(neighbour);
                    }
                    else {
                        openSet.UpdateItem(neighbour);
                    }


                }
            }
        }
        Debug.Log("Path not found");

        return null;
    }

    public static Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        Grid.instance.path = path;
        
        return  SimplifyPath(path);


    }

    public static Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if (directionNew != directionOld || i+1 == path.Count)
            {
                waypoints.Add(path[i].worldPosition);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }

    public static void CheckIfNodeIsObstacle(Node node) {
        ////Calculate obstacles while creating path
        Collider2D[] colliders = Physics2D.OverlapCircleAll(node.worldPosition, Grid.instance.nodeRadius, Grid.instance.unwalkableMask);
        if (colliders.Length > 0)
        {
            node.walkable = false;
        }
        else {
            node.walkable = true;
        }
    }

    static int GetDistance(Node nodeA, Node nodeB)
    {
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
