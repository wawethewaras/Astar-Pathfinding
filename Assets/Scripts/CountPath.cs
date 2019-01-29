using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Astar2DPathFinding.Mika {

    public class CountPath : MonoBehaviour, Pathfinding {
        private Transform startPos;
        private Vector2 endPos;
        private Vector2[] pathArray;


        private IEnumerator currentPath;
        private Vector2 endPosition;
        private bool readyToCountPath = true;


        public float movespeed;

        //Interval time between pathfinding
        [SerializeField]
        private float intervalTime = 1.0f;
        [SerializeField]
        private bool usePathSmooting;
        [SerializeField]
        private bool showPathSmoothing;

        public async void FindPath(Transform _seeker, Vector2 _endPos) {
            if (!readyToCountPath) {
                return;
            }

            else if (_seeker == null) {
                UnityEngine.Debug.LogError("Missing seeker!", this);
                return;
            }

            startPos = _seeker;
            endPos = _endPos;
            //Basic raycast if can move directly to end target

            //bool cantSeeTarget = Physics2D.Linecast(_seeker.transform.position, _endPos, grid.unwalkableMask);
            //if (cantSeeTarget == false)
            //{
            //    Vector2[] newPath = new Vector2[1];
            //    newPath[0] = _endPos;
            //    OnPathFound(newPath);
            //    sw.Stop();
            //    print("Time took to find path: " + sw.ElapsedMilliseconds);
            //    StartCoroutine(PathCountDelay());
            //    return;
            //}

            if (_endPos != endPosition) {
                endPosition = _endPos;
                await SearchPathRequest(this, _seeker.position, endPosition);

            }
        }
        //This has not been tested
        public void StopMovement() {
            if (currentPath != null) {
                StopCoroutine(currentPath);
            }

        }

        public void OnPathFound(Vector2[] newPath) {
            if (currentPath != null) {
                StopCoroutine(currentPath);

            }
            currentPath = movepath(newPath);
            pathArray = newPath;
            StartCoroutine(currentPath);

        }

        public IEnumerator movepath(Vector2[] pathArray) {
            if (pathArray == null) {
                yield break;
            }
            for (int i = 0; i < pathArray.Length; i++) {
                while ((Vector2)startPos.transform.position != pathArray[i]) {

                    ////if (Physics2D.Linecast(startPos.transform.position, endPosition, Grid.instance.unwalkableMask) == false)
                    ////{
                    ////    UnityEngine.Debug.DrawLine(startPos.transform.position, endPosition, Color.black, 10);
                    ////    break;
                    ////}

                    if (usePathSmooting && i < pathArray.Length - 1) {
                        bool cantSeeTarget = Physics2D.Linecast(startPos.transform.position, pathArray[i + 1], PathfindingGrid.Instance.unwalkableMask);
                        if (cantSeeTarget == false) {
                            if (showPathSmoothing) {
                                UnityEngine.Debug.DrawLine(startPos.transform.position, pathArray[i + 1], Color.white, 10);
                            }
                            i++;


                        }
                    }
                    else {
                        bool cantSeeTarget = Physics2D.Linecast(startPos.transform.position, endPos, PathfindingGrid.Instance.unwalkableMask);
                        if (cantSeeTarget == false) {
                            if (showPathSmoothing) {
                                UnityEngine.Debug.DrawLine(startPos.transform.position, endPos, Color.white, 10);

                            }
                            break;
                        }

                    }
                    Vector2 target_pos = pathArray[i];
                    Vector2 my_pos = transform.position;
                    target_pos.x = target_pos.x - my_pos.x;
                    target_pos.y = target_pos.y - my_pos.y;
                    float angle = Mathf.Atan2(target_pos.y, target_pos.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

                    startPos.transform.position = Vector2.MoveTowards(startPos.transform.position, pathArray[i], Time.deltaTime * movespeed);
                    //Vector2 direction = (pathArray[i] - startPos.transform.position).normalized * 100;
                    //startPos.GetComponent<Rigidbody2D>().velocity = direction * Time.deltaTime * movespeed;

                    yield return null;
                }
            }
            //while (true) {
            //    Vector2 target_pos = endPos;
            //    Vector2 my_pos = transform.position;
            //    target_pos.x = target_pos.x - my_pos.x;
            //    target_pos.y = target_pos.y - my_pos.y;
            //    float angle = Mathf.Atan2(target_pos.y, target_pos.x) * Mathf.Rad2Deg;
            //    transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            //    startPos.transform.position = Vector2.MoveTowards(startPos.transform.position, endPos, Time.deltaTime * movespeed);
            //    //Vector2 direction = (endPosition - startPos.transform.position).normalized * 100; ;
            //    //startPos.GetComponent<Rigidbody2D>().velocity = direction * Time.deltaTime * movespeed;
            //    yield return null;
            //}
        }

        private static Task<Vector3[]> SearchPathRequest(Pathfinding requester, Vector2 startPos, Vector2 endPos) {
            var taskCompletionSource = new TaskCompletionSource<Vector3[]>();

            Node start = PathfindingGrid.Instance.NodeFromWorldPoint(startPos);
            Node end = PathfindingGrid.Instance.ClosestNodeFromWorldPoint(endPos, start.gridAreaID);
            Vector2[] newPath = null;
            newPath = AStar.FindPath(start, end);
            requester.OnPathFound(newPath);

            return taskCompletionSource.Task;

        }

        public IEnumerator PathCountDelay() {
            readyToCountPath = false;
            float counter = Random.Range(intervalTime + 0.1f, intervalTime + 0.15f);
            yield return new WaitForSeconds(counter);
            readyToCountPath = true;

        }

        //Draw path to gizmoz
        public void OnDrawGizmos() {
            if (pathArray != null) {
                for (int i = 0; i < pathArray.Length - 1; i++) {
                    Gizmos.color = Color.black;
                    Gizmos.DrawCube(pathArray[i], Vector2.one);
                    Gizmos.DrawLine(pathArray[i], pathArray[i + 1]);
                }
            }
        }



    }
}