using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using System.Diagnostics;

public class ThreadController : MonoBehaviour {

    private static ThreadController s_Instance = null;
    public static ThreadController instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType(typeof(ThreadController))
                as ThreadController;
                if (s_Instance == null)
                {
                    UnityEngine.Debug.Log("Could not locate a PathRequestManager object. \n You have to have exactly one PathRequestManager in the scene.");
                    UnityEngine.Debug.Break();
                }

            }
            return s_Instance;
        }
    }

    List<Action> functionsToRunInMainThread;

    Thread mainThread;
    public Transform startPos, endPos;

    void Start() {
        mainThread = System.Threading.Thread.CurrentThread;

        functionsToRunInMainThread = new List<Action>();
        int rand = UnityEngine.Random.Range(0, 10);
        Node start = Grid.instance.ClosestNodeFromWorldPoint(startPos.transform.position);
        Node end = Grid.instance.ClosestNodeFromWorldPoint(endPos.transform.position);
        StartThreadedFunction(() => { slow(start, end); });
    }

    void Update()
    {
        while (functionsToRunInMainThread.Count > 0) {
            Action someFunc = functionsToRunInMainThread[0];
            functionsToRunInMainThread.RemoveAt(0);
            someFunc();
            UnityEngine.Debug.Log("Current thread is main: " + mainThread.Equals(System.Threading.Thread.CurrentThread));
        }
    }

    public void StartThreadedFunction(Action function) {
        Thread t = new Thread(new ThreadStart( function));
        t.Start();
    }

    public void QuesMainThread(Action function) {
        functionsToRunInMainThread.Add(function);
    }

    void slow(Node startPos, Node targetPos) {
        UnityEngine.Debug.Log("Current thread is main: " + mainThread.Equals(System.Threading.Thread.CurrentThread));
        Vector3[] path = FindPath(startPos, targetPos);
        Action aFunction = () =>
        {
            paths = path;
        };
        QuesMainThread(aFunction);
    }

    public Vector3[] paths;

    public static Vector3[] FindPath(Node startNode, Node targetNode)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Heap<Node> openSet = new Heap<Node>(Grid.instance.Maxsize);
        Heap<Node> closedSet = new Heap<Node>(Grid.instance.Maxsize);

        //Check if goal is inside collider
        //Collider2D[] colliders = Physics2D.OverlapCircleAll(targetNode.worldPosition, Grid.instance.nodeRadius, Grid.instance.unwalkableMask);
        if (/*colliders.Length > 0 || */targetNode.walkable == false || startNode.walkable == false)
        {
            UnityEngine.Debug.Log("Start or goal inside collider");
            return null;
        }

        openSet.Add(startNode);
        //For showing path counting 
        if (Grid.instance.showGrid)
        {
            Grid.openList.Add(startNode);
        }

        Grid.instance.GetNeighbours(startNode);

        while (openSet.Count > 0)
        {
            Node node = openSet.RemoveFirst();
            closedSet.Add(node);

            //For showing path counting 
            if (Grid.instance.showGrid)
            {
                Grid.closedList.Add(node);
            }


            if (node == targetNode)
            {
                //For testing path calculation. Can be removed from final version.
                sw.Stop();
                Vector3[] path = RetracePath(startNode, targetNode);
                int pathLenght = 0;
                for (int i = 0; i < path.Length - 1; i++)
                {
                    pathLenght += Mathf.RoundToInt(Vector3.Distance(path[i], path[i + 1]));
                }
                UnityEngine.Debug.Log("Time took to calculate path: " + sw.ElapsedMilliseconds + "ms. Number of nodes counted " + Grid.openList.Count + ". Path lenght: " + pathLenght);
                Grid.pathFound = true;

                return RetracePath(startNode, targetNode);
            }

            Node[] neighbours = Grid.instance.GetNeighbours(node);
            Node neighbour;

            for (int i = 0; i < neighbours.Length; i++)
            {
                neighbour = neighbours[i];

                //Calculate obstacles while creating path
                //CheckIfNodeIsObstacle(neighbour);

                if (neighbour == null || !neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newCostToNeighbour = node.gCost + GetDistance(node, neighbour) + neighbour.movementPenalty;
                if (newCostToNeighbour < neighbour.gCost || openSet.Contains(neighbour) == false)
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = Mathf.RoundToInt(GetDistance(neighbour, targetNode) * 2f);
                    neighbour.parent = node;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                        //For showing path counting 
                        if (Grid.instance.showGrid)
                        {
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

    /// <summary>
    /// Creates path from startNode to targetNode
    /// </summary>
    /// <param name="startNode"></param>
    /// <param name="targetNode"></param>
    /// <returns></returns>
    public static Vector3[] RetracePath(Node startNode, Node targetNode)
    {
        if (startNode == targetNode)
        {
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
