using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

namespace Astar2DPathFinding.Mika {

    [RequireComponent(typeof(Grid))]
    public class ThreadController : Singleton<ThreadController> {

        private static List<Action> functionsToRunInMainThread = new List<Action>();
        private static List<PathRequest> pathRequests = new List<PathRequest>();

        private static Thread currentThread;
        private static Vector2[] currentPath;

        private static bool readyToTakeNewPath = true;


        void Update() {

            while (functionsToRunInMainThread.Count > 0) {
                Action function = functionsToRunInMainThread[0];
                functionsToRunInMainThread.RemoveAt(0);
                function();

            }

            while (pathRequests.Count > 0 && readyToTakeNewPath) {
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
        public static void StartThreadedFunction(Action function) {
            currentThread = new Thread(new ThreadStart(function));
            currentThread.Start();
        }

        /// <summary>
        /// Que function to run it on main thread.
        /// </summary>
        /// <param name="function"></param>
        public static void QueFunctionToMainThread(Action function) {
            functionsToRunInMainThread.Add(function);
        }

        public static void SearchPathRequest(Pathfinding requester, Vector2 startPos, Vector2 endPos) {
            if (PathfindingGrid.Instance.useThreading) {
                PathRequest request = new PathRequest(requester, startPos, endPos);
                pathRequests.Add(request);
            }
            else {
                Node start = PathfindingGrid.Instance.ClosestNodeFromWorldPoint(startPos);
                Node end = PathfindingGrid.Instance.ClosestNodeFromWorldPoint(endPos);
                Vector2[] newPath = AStar.FindPath(start, end);
                requester.OnPathFound(newPath);
            }

        }

        private static IEnumerator CountPath(Pathfinding requester, Vector2 startPos, Vector2 endPos) {
            Node start = PathfindingGrid.Instance.ClosestNodeFromWorldPoint(startPos);
            Node end = PathfindingGrid.Instance.ClosestNodeFromWorldPoint(endPos);

            StartThreadedFunction(() => { FindPath(start, end); });

            while (currentThread.IsAlive) {
                yield return null;
            }
            readyToTakeNewPath = true;

            requester.OnPathFound(currentPath);
        }



        private static void FindPath(Node startPos, Node targetPos) {
            Vector2[] path = AStar.FindPath(startPos, targetPos);
            Action calculationFinished = () => {
                currentPath = path;
            };
            QueFunctionToMainThread(calculationFinished);
        }

        private void OnDisable() {
            if (currentThread != null) {
                currentThread.Abort();
            }

        }

    }

    struct PathRequest {
        public Vector2 startPosition;
        public Vector2 endPosition;
        public Pathfinding requester;

        public PathRequest(Pathfinding _requester, Vector2 _startPosition, Vector2 _endPosition) {
            startPosition = _startPosition;
            endPosition = _endPosition;
            requester = _requester;
        }
    }
}