using UnityEngine;
using System.Collections;
using System;

public class NodeOld : IComparable
{
    //public float nodeTotalCost;
    //public float estimatedCost;
    
    
    public Vector3 position;
    public int gridX;
    public int gridY;

    public bool walkable;
    public bool bObstacle;
    public int gCost;
    public int hCost;

    public int fCost {
        get {
            return gCost + hCost;
        }
    }

    public NodeOld parent;

    public NodeOld()
    {
        bObstacle = false;
        parent = null;
    }
    public NodeOld(Vector3 pos)
    {
        bObstacle = false;
        parent = null;
        position = pos;
    }
    public NodeOld(bool _obstacle, Vector3 _postion, int _gridX, int _gridY)
    {
        bObstacle = _obstacle;
        position = _postion;
        gridX = _gridX;
        gridY = _gridY;
    }


    public void MarkAsObstacle()
    {
        bObstacle = true;
    }
    public int CompareTo(object obj)
    {
        NodeOld node = (NodeOld)obj;
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