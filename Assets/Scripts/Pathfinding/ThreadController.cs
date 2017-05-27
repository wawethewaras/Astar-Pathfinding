using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

[RequireComponent(typeof(Grid))]
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
                    UnityEngine.Debug.Log("Could not locate a ThreadController object. \n You have to have exactly one ThreadController in the scene.");
                    UnityEngine.Debug.Break();
                }

            }
            return s_Instance;
        }
    }

    private static List<Action> functionsToRunInMainThread = new List<Action>();
    private static List<PathRequest> pathRequests = new List<PathRequest>();

    private static Thread currentThread;
    private static Vector3[] currentPath;

    private static bool readyToTakeNewPath = true;


    void Update() {

        while (functionsToRunInMainThread.Count > 0) {
            Action function = functionsToRunInMainThread[0];
            functionsToRunInMainThread.RemoveAt(0);
            function();

        }

        while (pathRequests.Count > 0 && readyToTakeNewPath)
        {
            readyToTakeNewPath = false;
            PathRequest requester = pathRequests[0];
            pathRequests.RemoveAt(0);
            IEnumerator pathCount = CountPath(requester.requester, requester.startPosition, requester.endPosition);
            StartCoroutine(pathCount);
        }
    }

    /// <summary>
    /// Run function on other thread.
    /// </summary>
    /// <param name="function"></param>
    public static void StartThreadedFunction(Action function)
    {
        currentThread = new Thread(new ThreadStart(function));
        currentThread.Start();
    }

    /// <summary>
    /// Que function to run it on main thread.
    /// </summary>
    /// <param name="function"></param>
    public static void QueFunctionToMainThread(Action function)
    {
        functionsToRunInMainThread.Add(function);
    }

    public static void SearchPathRequest(pathfinding.Pathfinding requester, Vector3 startPos, Vector3 endPos) {
        //For showing path counting process. Resets grid.
        Grid.openList.Clear();
        Grid.closedList.Clear();
        Grid.pathFound = false;

        PathRequest request = new PathRequest(requester, startPos, endPos);
        pathRequests.Add(request);

    }

    public static IEnumerator CountPath(pathfinding.Pathfinding requester, Vector3 startPos, Vector3 endPos) {
        Node start = Grid.instance.ClosestNodeFromWorldPoint(startPos);
        Node end = Grid.instance.ClosestNodeFromWorldPoint(endPos);

        StartThreadedFunction(() => { FindPath(start, end); });

        while (currentThread.IsAlive)
        {
            yield return null;
        }
        readyToTakeNewPath = true;

        instance.StartCoroutine(requester.PathCountDelay());
        requester.OnPathFound(currentPath);
    }



    static void FindPath(Node startPos, Node targetPos) {
        Vector3[] path = AStar.FindPath(startPos, targetPos);
        Action calculationFinished = () =>
        {
            currentPath = path;
        };
        QueFunctionToMainThread(calculationFinished);
    }

    void OnDisable()
    {
        if (currentThread != null) {
            currentThread.Abort();
        }

    }

}

struct PathRequest{
    public Vector3 startPosition;
    public Vector3 endPosition;
    public pathfinding.Pathfinding requester;

    public PathRequest(pathfinding.Pathfinding _requester, Vector3 _startPosition, Vector3 _endPosition) {
        startPosition = _startPosition;
        endPosition = _endPosition;
        requester = _requester;
    }
}