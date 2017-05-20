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

    private List<Action> functionsToRunInMainThread = new List<Action>();
    private List<CountPath> pathRequests = new List<CountPath>();

    private Thread currentThread;
    private Vector3[] currentPath;

    private bool readyToTakeNewPath = true;

    //void Awake() {
    //    //readyToTakePath = true;
    //    //functionsToRunInMainThread = new List<Action>();
    //    //pathRequests = new List<CountPath>();
    //    //Node start = Grid.instance.ClosestNodeFromWorldPoint(startPos.transform.position);
    //    //Node end = Grid.instance.ClosestNodeFromWorldPoint(endPos.transform.position);
    //    //StartThreadedFunction(() => { slow(start, end); });
    //}

    void Update()
    {

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



    public void SearchPathRequest(CountPath counter) {
        pathRequests.Add(counter);

        //StartCoroutine(counting(counter));


    }


    public IEnumerator CountPath(CountPath requester) {
        requester.endPosition = requester.endPos.position;
        Node start = Grid.instance.ClosestNodeFromWorldPoint(requester.startPos.position);
        Node end = Grid.instance.ClosestNodeFromWorldPoint(requester.endPos.position);
        StartThreadedFunction(() => { FindPath(start, end); });
        requester.readyToCountPath = false;
        while (currentThread.IsAlive)
        {
            yield return null;
        }

        StartCoroutine(requester.PathCountDelay());
        readyToTakeNewPath = true;
        requester.OnPathFound(currentPath);
    }

    public void StartThreadedFunction(Action function) {
        currentThread = new Thread(new ThreadStart( function));
        currentThread.Start();
    }

    public void QueFunctionToMainThread(Action function) {
        functionsToRunInMainThread.Add(function);
    }

    void FindPath(Node startPos, Node targetPos) {
        Vector3[] path = AStar.FindPath(startPos, targetPos);
        Action aFunction = () =>
        {
            currentPath = path;
        };
        QueFunctionToMainThread(aFunction);
    }



}
