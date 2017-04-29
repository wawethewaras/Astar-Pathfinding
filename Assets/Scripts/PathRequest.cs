using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PathRequest : MonoBehaviour
{

    //Queue<PathRequester> pathRequestQueue = new Queue<PathRequester>();
    //PathRequester currentPathRequest;

    //static PathRequest instance;
    //TestCode pathfinding;

    //bool isProcessingPath;

    //void Awake()
    //{
    //    instance = this;
    //    pathfinding = GetComponent<TestCode>();
    //}

    //public static void RequestPath(Node pathStart, Node pathEnd, Action<ArrayList, bool> callback)
    //{
    //    PathRequester newRequest = new PathRequester(pathStart, pathEnd, callback);
    //    instance.pathRequestQueue.Enqueue(newRequest);
    //    instance.TryProcessNext();
    //}

    //void TryProcessNext()
    //{
    //    if (!isProcessingPath && pathRequestQueue.Count > 0)
    //    {
    //        currentPathRequest = pathRequestQueue.Dequeue();
    //        isProcessingPath = true;
    //        AStar.FindPath(pathfinding.startNode, pathfinding.goalNode);
    //    }
    //}

    //public void FinishedProcessingPath(ArrayList path, bool success)
    //{
    //    currentPathRequest.callback(path, success);
    //    isProcessingPath = false;
    //    TryProcessNext();
    //}

    //struct PathRequester
    //{
    //    public Node pathStart;
    //    public Node pathEnd;
    //    public Action<ArrayList, bool> callback;

    //    public PathRequester(Node _start, Node _end, Action<ArrayList, bool> _callback)
    //    {
    //        pathStart = _start;
    //        pathEnd = _end;
    //        callback = _callback;
    //    }

    //}
}