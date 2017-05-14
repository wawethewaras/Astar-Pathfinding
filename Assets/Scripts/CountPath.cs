using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class CountPath : MonoBehaviour
{
    public Transform startPos, endPos;
    private Vector3[] pathArray;

    //Interval time between pathfinding
    public float intervalTime = 1.0f;

    IEnumerator currentPath;

    private Vector3 endPosition;

    public float movespeed;

    public bool autoCountPath;

    private bool readyToCountPath;


    void Start()
    {
        readyToCountPath = true;
        startPos = transform;
        FindPath();
    }
    void Update()
    {
        if (Physics2D.Linecast(startPos.position, endPos.position, Grid.instance.unwalkableMask))
        {
            UnityEngine.Debug.DrawLine(startPos.position, endPos.position, Color.red);
        }
        else {
            UnityEngine.Debug.DrawLine(startPos.position, endPos.position, Color.blue);
        }

        //Count path more often if target is near
        if (autoCountPath)
        {
            FindPath();
        }
        else {
            if (Input.GetButtonDown("Jump"))
            {
                FindPath();
            }
        }

    }

    public void FindPath()
    {
        if (readyToCountPath == false) {
            return;
        }

        if (startPos == null || endPos == null) {
            print("Missing start position or endposition");
            return;
        }


        if (endPos.position != endPosition) {
            Grid.openList.Clear();
            Grid.closedList.Clear();
            Grid.pathFound = false;
            endPosition = endPos.position;

            //PathRequestManager.RequestPath(startPos.position, endPos.position, OnPathFound);
            pathArray = AStar.FindPath(startPos.position, endPos.position);

            //Check if path found
            if (pathArray == null) {
                return;
            }
            OnPathFound();
        }

    }

    public void OnPathFound() {
        if (currentPath != null)
        {
            StopCoroutine(currentPath);

        }
        currentPath = movepath(pathArray);
        StartCoroutine(currentPath);
        StartCoroutine(PathCountDelay());
    }

    //Working on a path requester
    //public void OnPathFoundRequester(Vector3[] newPath, bool pathSuccessful)
    //{
    //    if (pathSuccessful) {
    //        pathArray = newPath;

    //        if (currentPath != null)
    //        {
    //            StopCoroutine(currentPath);
    //        }
    //        currentPath = movepath(pathArray);
    //        StartCoroutine(currentPath);
    //          StartCoroutine(PathCountDelay());
    //    }


    //}


    public IEnumerator movepath(Vector3[] pathArray) {
        if (pathArray == null) {
            yield return null;
        }
        for (int i = 0; i < pathArray.Length; i++) {
            while (/*(objStartCube.transform.position - pathArray[i].position).sqrMagnitude > nextWaypointDistance * nextWaypointDistance*/startPos.transform.position != pathArray[i]) {
                startPos.transform.position =  Vector3.MoveTowards(startPos.transform.position, pathArray[i], Time.deltaTime* movespeed);
                if (startPos.transform.position == pathArray[i]) {
                    //Debug.Log("Goal reaced");
                }
                yield return null;
            }
        }

        yield return null;
    }

    public IEnumerator PathCountDelay()
    {
        readyToCountPath = false;
        float counter = Random.Range(intervalTime + 0.1f, intervalTime + 0.25f);
        yield return new WaitForSeconds(counter);
        readyToCountPath = true;

    }

    public void OnDrawGizmos()
    {
        if (pathArray != null)
        {
            for (int i = 0; i < pathArray.Length-1; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(pathArray[i], Vector3.one);
                Gizmos.DrawLine(pathArray[i], pathArray[i+1]);
            }
        }
    }



}