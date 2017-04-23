using UnityEngine;
using System.Collections;
using System;

public class Node : IComparable
{
    public float nodeTotalCost;
    public float estimatedCost;
    public bool bObstacle;
    public Node parent;
    public Vector3 position;
    public Node()
    {
        estimatedCost = 0.0f;
        nodeTotalCost = 1.0f;
        bObstacle = false;
        parent = null;
    }
    public Node(Vector3 pos)
    {
        estimatedCost = 0.0f;
        nodeTotalCost = 1.0f;
        bObstacle = false;
        parent = null;
        position = pos;
    }
    public void MarkAsObstacle()
    {
        bObstacle = true;
    }
    public int CompareTo(object obj)
    {
        Node node = (Node)obj;
        //Negative value means object comes before this in the sort
        //order.
        if (estimatedCost < node.estimatedCost)
            return -1;
        //Positive value means object comes after this in the sort
        //order.
        if (estimatedCost > node.estimatedCost) return 1;
        return 0;
    }
}