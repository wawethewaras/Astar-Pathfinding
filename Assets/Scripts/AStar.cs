using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class AStar
{

    public static List<Node> openList = new List<Node>();
    public static List<Node> closedList = new List<Node>();
    public static bool pathFound;
    public static int numberOfnodes;



    public static List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        openList.Clear();
        closedList.Clear();
        pathFound = false;

        //RaycastHit2D hit = Physics2D.Raycast(startPos, targetPos, Grid.instance.unwalkableMask);
        //if (hit.collider != null) {
        //    Debug.Log("Hit?");
        //}

        Node startNode = Grid.instance.NodeFromWorldPoint(startPos);
        Node targetNode = Grid.instance.NodeFromWorldPoint(targetPos);

        List<Node> openSet = new List<Node>();
        List<Node> closedSet = new List<Node>();
        openSet.Add(startNode);
        //For counting path
        openList.Add(startNode);


        while (openSet.Count > 0)
        {
            Node node = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost)
                {
                    if (openSet[i].hCost < node.hCost)
                        node = openSet[i];
                }
            }

            openSet.Remove(node);
            closedSet.Add(node);
            //For counting path
            closedList.Add(node);
            numberOfnodes++;

            if (node == targetNode)
            {
                pathFound = true;
                return RetracePath(startNode, targetNode);
            }

            foreach (Node neighbour in Grid.instance.GetNeighbours(node))
            {
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

                    if (!openSet.Contains(neighbour)) {
                        openSet.Add(neighbour);
                        //For counting path
                        openList.Add(neighbour);
                        numberOfnodes++;
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

    static int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}
