using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class TestCode : MonoBehaviour
{
    public Transform startPos, endPos;
    public Node startNode { get; set; }
    public Node goalNode { get; set; }
    public List<Node> pathArray;
    //public GameObject objStartCube, objEndCube;
    private float elapsedTime = 0.0f;
    //Interval time between pathfinding
    public float intervalTime = 1.0f;

    public List<Node> openList;

    IEnumerator currentPath;

    Vector3 endPostion;
    float distanceBetweenPoints;

    public float movespeed;
    public bool autoCountPath;

    public bool showCalculatedObstacles;

    public float nextWaypointDistance;


    void Start()
    {
        //objStartCube = GameObject.FindGameObjectWithTag("Start");
        //objEndCube = GameObject.FindGameObjectWithTag("End");
        pathArray = new List<Node>();
        
        FindPath();
    }
    void Update()
    {

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
        PriorityQueue.closedList.Clear();
        PriorityQueue.openList.Clear();

        startPos = startPos.transform;
        endPos = endPos.transform;
        //startNode = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex(startPos.position)));
        //goalNode = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex(endPos.position)));
        if (endPos.position != endPostion) {
            endPostion = endPos.position;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            pathArray = AStar.FindPath(startPos.position, endPos.position);
            sw.Stop();
            print("Time took to calculate path: " + sw.ElapsedMilliseconds + "ms");
            //Debug.Log(pathArray[0].position);

            if (pathArray == null) {
                return;
            }
            //Debug.Log("New path calculated");
            if (currentPath != null)
            {
                StopCoroutine(currentPath);
            }
            currentPath = movepath(pathArray);
            StartCoroutine(currentPath);
        }


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
            //if (pathArray.Count > 1)
            //{
            //    print(AStar.GetDistance(pathArray[i] ,pathArray[i+1]));
            //}
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

    //void OnDrawGizmos()
    //{
    //    if (pathArray == null)
    //        return;
    //    if (pathArray.Count > 0)
    //    {
    //        int index = 1;
    //        foreach (SebNode node in pathArray)
    //        {
    //            if (index < pathArray.Count)
    //            {
    //                SebNode nextNode = (SebNode)pathArray[index];
    //                UnityEngine.Debug.DrawLine(node.worldPosition, nextNode.worldPosition,
    //                Color.green);
    //                index++;
    //            }
    //        }
    //    }
    //    if (showCalculatedObstacles)
    //    {
    //        Gizmos.color = Color.yellow;
    //        for (int i = 0; i < SebPathfinding.openSet.Count; i++)
    //        {
    //            Gizmos.DrawCube(SebPathfinding.openSet[i].worldPosition, Vector3.one);

    //        }
    //        Gizmos.color = Color.red;
    //        for (int i = 0; i < SebPathfinding.closedSet.Count; i++)
    //        {
    //            Gizmos.DrawCube(SebPathfinding.closedSet[i].worldPosition, Vector3.one * 0.3f);

    //        }
    //    }


    //}
}