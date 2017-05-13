using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class CountPath : MonoBehaviour
{
    public Transform startPos, endPos;
    public Node startNode { get; set; }
    public Node goalNode { get; set; }
    //public List<Node> pathArray;
    private Vector3[] pathArray;

    private float elapsedTime = 0.0f;
    //Interval time between pathfinding
    public float intervalTime = 1.0f;

    public List<Node> openList;

    IEnumerator currentPath;

    Vector3 endPosition;

    public float movespeed;
    public bool autoCountPath;

    public bool showCalculatedObstacles;



    void Start()
    {
        //pathArray = new List<Node>();
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
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= intervalTime)
            {
                elapsedTime = 0.0f;
                FindPath();
            }
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

        if (startPos == null || endPos == null) {
            print("Missing start position or endposition");
            return;
        }


        if (endPos.position != endPosition) {
            Grid.openList.Clear();
            Grid.closedList.Clear();
            Grid.pathFound = false;
            endPosition = endPos.position;

            PathRequestManager.RequestPath(startPos.position, endPos.position, OnPathFound);


            if (pathArray == null) {
                return;
            }
            int pathLenght = 0;
            for (int i = 0; i < pathArray.Length -1;i++) {
                pathLenght += Mathf.RoundToInt( Vector3.Distance(pathArray[i], pathArray[i+1]));
            }
            //print("Time took to calculate path: " + sw.ElapsedMilliseconds + "ms. Number of nodes counted " + Grid.openList.Count + ". Path lenght: " + pathLenght);
            //Debug.Log("New path calculated");
            //OnPathFound();
        }


    }


    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful) {
            pathArray = newPath;

            if (currentPath != null)
            {
                StopCoroutine(currentPath);
            }
            currentPath = movepath(pathArray);
            StartCoroutine(currentPath);
        }


    }


    public IEnumerator movepath(Vector3[] pathArray) {
        int security = 0;
        int i = 0;
        if (pathArray == null || pathArray.Length <= 1)
        {
            yield return null;
        }
        for (int j = 0; j < pathArray.Length; j++) {
            if (i == 0 && pathArray.Length > 1)
            {
                i = 1;
            }

            else {
                i = j;
            }

            while (/*(objStartCube.transform.position - pathArray[i].position).sqrMagnitude > nextWaypointDistance * nextWaypointDistance*/startPos.transform.position != pathArray[i]) {
                startPos.transform.position =  Vector3.MoveTowards(startPos.transform.position, pathArray[i], Time.deltaTime* movespeed);
                if (startPos.transform.position == pathArray[i]) {

                    //Debug.Log("Goal reaced");
                }
                yield return null;
                //if (security > 2000) {                    
                //    Debug.Break();
                //    break;
                //}
                security++;
            }
        }

        yield return null;
    }

}