using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class AStar
{

    //For showing calculated path. Should be removed from final version.
    public static List<Node> openList = new List<Node>();
    public static List<Node> closedList = new List<Node>();
    public static bool pathFound;



    public static List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        openList.Clear();
        closedList.Clear();
        pathFound = false;


        Node startNode = Grid.instance.NodeFromWorldPoint(startPos);
        Node targetNode = Grid.instance.NodeFromWorldPoint(targetPos);
        Heap<Node> openSet = new Heap<Node>(Grid.instance.Maxsize);
        List<Node> closedSet = new List<Node>();



        //Check if goal is inside collider
        Collider2D[] colliders = Physics2D.OverlapCircleAll(targetNode.worldPosition, Grid.instance.nodeRadius, Grid.instance.unwalkableMask);
        if (colliders.Length > 0 || targetNode.walkable == false)
        {
            Debug.Log("Goal inside collider");
            return null;
        }

        //Check if can see target and is there need to calculate path
        bool cantSeeTarget = Physics2D.Linecast(startPos, targetPos, Grid.instance.unwalkableMask);
        if (cantSeeTarget == false)
        {
            Debug.Log("Can see target");
            List<Node> path = new List<Node>(2);
            path.Add(startNode);
            path.Add(targetNode);
            return path;
        }

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

            foreach (Node neighbour in Grid.instance.GetNeighbours(node))
            {
                //Calculate obstacles while creating path
                CheckIfNodeIsObstacle(neighbour);

                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
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

    public static List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        float pathLength = 0;
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            pathLength++;

            currentNode = currentNode.parent;
        }
        path.Reverse();
        Debug.Log("Path lenght: " + pathLength);
        Grid.instance.path = path;
        return path;


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
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}
