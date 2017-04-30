using UnityEngine;
using System.Collections;
using System;

public class Node : IComparable
{
    //public float nodeTotalCost;
    //public float estimatedCost;
    
    
    public Vector3 position;
    public int gridX;
    public int gridY;

    public bool bObstacle;
    public int gCost;
    public int hCost;

    public int fCost {
        get {
            return gCost + hCost;
        }
    }

    public Node parent;

    public Node()
    {
        bObstacle = false;
        parent = null;
    }
    public Node(Vector3 pos)
    {
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
        if (fCost < node.fCost)
            return -1;
        //Positive value means object comes after this in the sort
        //order.
        if (fCost > node.fCost) return 1;
        return 0;
    }
}