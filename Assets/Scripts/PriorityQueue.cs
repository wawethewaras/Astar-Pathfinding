using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue
{
    public static List<Vector3> openList = new List<Vector3>();
    public static List<Vector3> closedList = new List<Vector3>();

    public List<NodeOld> nodes = new List<NodeOld>();
    public int Length
    {
        get { return this.nodes.Count; }
    }
    public bool Contains(NodeOld node)
    {
        return this.nodes.Contains(node);
    }
    public NodeOld First()
    {
        if (this.nodes.Count > 0)
        {
            return (NodeOld)this.nodes[0];
        }
        return null;
    }
    public void Push(NodeOld node)
    {
        this.nodes.Add(node);
        //Debug.Log(node.position);
        
        this.nodes.Sort();
    }
    public void Remove(NodeOld node)
    {
        this.nodes.Remove(node);
        //Ensure the list is sorted
        //this.nodes.Sort();
    }

}
