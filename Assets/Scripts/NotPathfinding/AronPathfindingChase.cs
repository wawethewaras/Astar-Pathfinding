using UnityEngine;
using Pathfinding;
using System.Collections;

public class AronPathfindingChase : MonoBehaviour
{
    //The point to move to
    //public Transform target;

    private Seeker seeker;

    //The calculated path
    public Path path;

    //The max distance from the AI to a waypoint for it to continue to the next waypoint
    public float nextWaypointDistance;

    //The waypoint we are currently moving towards
    private int currentWaypoint = 0;


    private bool countNewPath;

    public Transform target;
    public float moveSpeed;
    public bool autoCountPath;

    public void Start()
    {
        countNewPath = false;
        seeker = GetComponent<Seeker>();

        //Start a new path to the targetPosition, return the result to the OnPathComplete function
        seeker.StartPath(transform.position, target.position, OnPathComplete);

    }

    //void Update()
    //{
    //    if (countNewPath) {
    //        return;
    //    }
    //    if (autoCountPath && target != null)
    //    {
    //        ChaseTarget(moveSpeed,target);
    //    }
    //    else {
    //        if (Input.GetMouseButtonDown(0))
    //        {
    //            ChaseTarget(moveSpeed,target);
    //        }
    //    }
    //}

    public void OnPathComplete(Path p)
    {
        //Debug.Log("Yay, we got a path back. Did it have an error? " + p.error);
        if (!p.error)
        {
            path = p;
            //Reset the waypoint counter
            currentWaypoint = 0;
        }
    }


    public void ChaseTarget(float speed, Transform target)
    {
        if (!countNewPath)
        {

            StartCoroutine(WaitBeforeCountingNextPath(target));
            CountNewPath(target);
        }
        if (path == null)
        {
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            //Debug.Log("End Of Path Reached");
            return;
        }

        //Direction to the next waypoint
        Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        dir *= speed * Time.deltaTime;
        gameObject.transform.Translate(dir);

        //Check if we are close enough to the next waypoint
        //If we are, proceed to follow the next waypoint
        if (Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance)
        {
            currentWaypoint++;
            return;
        }
    }


    public Vector3 GetCurrentWaypoint()
    {
        if (path == null || currentWaypoint >= path.vectorPath.Count)
        {
            print("Error: No path");
            return Vector3.zero;
        }
        return path.vectorPath[currentWaypoint];
    }

    IEnumerator WaitBeforeCountingNextPath(Transform target)
    {
        float delay = Random.Range(0.3f, 0.6f);
        countNewPath = true;
        yield return new WaitForSeconds(delay);
        //seeker.StartPath(transform.position, new Vector3(target.position.x, target.position.y, 0), OnPathComplete);
        countNewPath = false;
    }

    public void CountNewPath(Transform target)
    {
        seeker.StartPath(transform.position, new Vector3(target.position.x, target.position.y, 0), OnPathComplete);
    }

}