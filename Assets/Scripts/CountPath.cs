using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class CountPath : MonoBehaviour
{
    public Transform startPos, endPos;
    public Node startNode { get; set; }
    public Node goalNode { get; set; }
    public List<Node> pathArray;

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
        pathArray = new List<Node>();
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
            endPosition = endPos.position;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            pathArray = AStar.FindPath(startPos.position, endPos.position);
            sw.Stop();

            if (pathArray == null) {
                return;
            }
            print("Time took to calculate path: " + sw.ElapsedMilliseconds + "ms. Number of nodes counted " + AStar.openList.Count + ". Path lenght: " + pathArray.Count);
            //Debug.Log("New path calculated");
            OnPathFound();
        }


    }

    public void OnPathFound()
    {
        if (currentPath != null)
        {
            StopCoroutine(currentPath);
        }
        currentPath = movepath(pathArray);
        StartCoroutine(currentPath);

    }


    public IEnumerator movepath(List<Node> pathArray) {
        int security = 0;
        int i = 0;
        if (pathArray == null || pathArray.Count <= 1)
        {
            yield return null;
        }
        for (int j = 0; j < pathArray.Count; j++) {
            if (i == 0 && pathArray.Count > 1)
            {
                i = 1;
            }

            else {
                i = j;
            }
            while (/*(objStartCube.transform.position - pathArray[i].position).sqrMagnitude > nextWaypointDistance * nextWaypointDistance*/startPos.transform.position != pathArray[i].worldPosition) {
                startPos.transform.position =  Vector3.MoveTowards(startPos.transform.position, pathArray[i].worldPosition, Time.deltaTime* movespeed);
                if (startPos.transform.position == pathArray[i].worldPosition) {

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