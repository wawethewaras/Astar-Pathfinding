using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;


public class PathRequestManager : MonoBehaviour {

    private static PathRequestManager s_Instance = null;
    public static PathRequestManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType(typeof(PathRequestManager))
                as PathRequestManager;
                if (s_Instance == null)
                {
                    Debug.Log("Could not locate a PathRequestManager object. \n You have to have exactly one PathRequestManager in the scene.");
                    Debug.Break();
                }

            }
            return s_Instance;
        }
    }

    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;


    bool isProcessingPath;



    //public static void myThreadCount(CountPath finder)
    //{
    //    Thread myThread = new Thread(finder.FindPath);
    //    //ThreadStart myThread = delegate
    //    //{
    //    //    finder.FindPath();

    //    //};
    //    myThread.Start();
    //}

    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    void TryProcessNext()
    {
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            //AStar.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd, this);
        }
    }

    public void FinishedProcessingPath(Vector3[] path, bool success)
    {
        currentPathRequest.callback(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;

        public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback)
        {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
        }

    }
}
