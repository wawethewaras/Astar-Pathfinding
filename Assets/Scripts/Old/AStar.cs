using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AStarOld
{

    public static List<NodeOld> FindPath(Vector3 startPos, Vector3 endPos)
    {
        NodeOld start = new NodeOld(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex(startPos)));
        NodeOld goal = new NodeOld(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex(endPos)));

        PriorityQueue openList = new PriorityQueue();
        PriorityQueue closedList = new PriorityQueue();

        openList.Push(start);

        //start.nodeTotalCost = 0.0f;

        start.gCost = 0;
        //start.estimatedCost = HeuristicEstimateCost(start, goal);
        start.hCost = HeuristicEstimateCost(start, goal);
        ////For showing calculated path
        PriorityQueue.openList.Add(start.position);



        NodeOld node = null;
        while (openList.Length != 0)
        {
            node = openList.First();
            //Check if the current node is the goal node
            if (node.position == goal.position)
            {
                Debug.Log("Path found! Lenght: " + node.fCost);
                return CalculatePath(node);
            }
            //Create an ArrayList to store the neighboring nodes
            List<NodeOld> neighbours = new List<NodeOld>();
            GridManager.instance.GetNeighbours(node, neighbours);
            for (int i = 0; i < neighbours.Count; i++)
            {
                NodeOld neighbourNode = neighbours[i];
                if (!closedList.Contains(neighbourNode))
                {
                    int hcost = GetDistance(node, neighbourNode);
                    int gcost = node.fCost + hcost;
                    int neighbourHcost = HeuristicEstimateCost(neighbourNode, goal);
                    neighbourNode.gCost = gcost;
                    neighbourNode.parent = node;
                    neighbourNode.hCost = neighbourHcost;
                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Push(neighbourNode);
                        ////For showing calculated path
                        PriorityQueue.openList.Add(neighbourNode.position);

                    }
                }
            }
            //Push the current node to the closed list
            closedList.Push(node);

            ////For showing calculated path
            PriorityQueue.closedList.Add(node.position);

            //and remove it from openList
            openList.Remove(node);
        }
        if (node.position != goal.position)
        {
            Debug.LogError("Goal Not Found");
            Debug.Break();
            return null;
        }

        return CalculatePath(node);
    }
    private static List<NodeOld> CalculatePath(NodeOld node)
    {
        List<NodeOld> list = new List<NodeOld>();
        while (node != null)
        {
            list.Add(node);
            node = node.parent;
        }
        list.Reverse();
        return list;
    }

    private static int HeuristicEstimateCost(NodeOld curNode, NodeOld goalNode)
    {
        Vector3 vecCost = (curNode.position - goalNode.position);
        return Mathf.RoundToInt(vecCost.magnitude * 10);
    }

    public static int GetDistance(NodeOld curNode, NodeOld goalNode)
    {
        Vector3 vecCost = (curNode.position - goalNode.position);
        //int dstX = Mathf.Abs(curNode.gridX - goalNode.gridX);
        //int dstY = Mathf.Abs(curNode.gridY - goalNode.gridY);
        //float multiplier = 1;
        //if (Mathf.Abs(dstX) + Mathf.Abs(dstY) == 2) {
        //    multiplier = 1.4f;
        //}
        return Mathf.RoundToInt(vecCost.magnitude * 10);
    }
}