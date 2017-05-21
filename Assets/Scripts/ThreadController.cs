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
                    UnityEngine.Debug.Log("Could not locate a ThreadController object. \n You have to have exactly one ThreadController in the scene.");
                    UnityEngine.Debug.Break();
                }

            }
            return s_Instance;
        }
    }

    private static List<Action> functionsToRunInMainThread = new List<Action>();
    private static List<CountPath> pathRequests = new List<CountPath>();

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
            CountPath requester = pathRequests[0];
            pathRequests.RemoveAt(0);
            IEnumerator pathCount = CountPath(requester);
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

    public static void SearchPathRequest(CountPath requester) {
        pathRequests.Add(requester);

    }

    public static IEnumerator CountPath(CountPath requester) {
        Node start = Grid.instance.ClosestNodeFromWorldPoint(requester.startPos.position);
        Node end = Grid.instance.ClosestNodeFromWorldPoint(requester.endPos.position);

        StartThreadedFunction(() => { FindPath(start, end); });

        while (currentThread.IsAlive)
        {
            yield return null;
        }
        readyToTakeNewPath = true;

        requester.StartCoroutine(requester.PathCountDelay());
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



}
