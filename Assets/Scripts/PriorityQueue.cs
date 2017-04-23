using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue
{
    public static List<Vector3> openList = new List<Vector3>();
    public static List<Vector3> closedList = new List<Vector3>();

    public List<Node> nodes = new List<Node>();
    public int Length
    {
        get { return this.nodes.Count; }
    }
    public bool Contains(Node node)
    {
        return this.nodes.Contains(node);
    }
    public Node First()
    {
        if (this.nodes.Count > 0)
        {
            return (Node)this.nodes[0];
        }
        return null;
    }
    public void Push(Node node)
    {
        this.nodes.Add(node);
        //Debug.Log(node.position);
        
        this.nodes.Sort();
    }
    public void Remove(Node node)
    {
        this.nodes.Remove(node);
        //Ensure the list is sorted
        this.nodes.Sort();
    }

}
