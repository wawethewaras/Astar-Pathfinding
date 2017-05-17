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

    public float nextWaypointDistance;

    void Start()
    {
        readyToCountPath = true;
        startPos = transform;
        FindPath(startPos.position, endPos.position);
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
            FindPath(startPos.position, endPos.position);
        }
        else {
            if (Input.GetButtonDown("Jump"))
            {
                FindPath(startPos.position, endPos.position);
            }
        }

    }

    public void FindPath(Vector3 start, Vector3 goal)
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
            pathArray = AStar.FindPath(start, goal);
            //Check if path found
            if (pathArray == null) {
                print("Path not Found");
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
            while (startPos.transform.position != pathArray[i])
            {
                //while ((startPos.transform.position - pathArray[i]).sqrMagnitude > nextWaypointDistance && startPos.transform.position != pathArray[pathArray.Length - 1]) {

                //if (Physics2D.Linecast(startPos.transform.position, endPos.position, Grid.instance.unwalkableMask) == false)
                //{
                //    UnityEngine.Debug.DrawLine(startPos.transform.position, endPos.position, Color.black, 10);
                //    break;


                //}

                if (i < pathArray.Length - 2)
                {
                    bool cantSeeTarget = Physics2D.Linecast(startPos.transform.position, pathArray[i + 1], Grid.instance.unwalkableMask);
                    if (cantSeeTarget == false)
                    {
                        UnityEngine.Debug.DrawLine(startPos.transform.position, pathArray[i + 1], Color.black, 10);
                        i++;


                    }
                }
                else {
                    bool cantSeeTarget = Physics2D.Linecast(startPos.transform.position, endPos.position, Grid.instance.unwalkableMask);
                    if (cantSeeTarget == false)
                    {
                        UnityEngine.Debug.DrawLine(startPos.transform.position, endPos.position, Color.black, 10);
                        break;
                    }
                    
                }
                Vector2 mouse_pos = pathArray[i];
                Vector2 object_pos =  transform.position;
                mouse_pos.x = mouse_pos.x - object_pos.x;
                mouse_pos.y = mouse_pos.y - object_pos.y;
                float angle = Mathf.Atan2(mouse_pos.y, mouse_pos.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
                startPos.transform.position = Vector3.MoveTowards(startPos.transform.position, pathArray[i], Time.deltaTime * movespeed);
                //Vector3 direction = (pathArray[i] - startPos.transform.position).normalized * 100;
                //startPos.GetComponent<Rigidbody2D>().velocity = direction * Time.deltaTime * movespeed;


                ////If end node reached let's just move torward target position
                //if (startPos.transform.position == pathArray[pathArray.Length-1]) {
                //    pathArray[pathArray.Length - 1] = endPosition;

                //    //This is to prevent overload. Might not be nessesary
                //    if (endPosition != endPos.position) {
                //        break;
                //    }
                //}
                yield return null;
            }
        }
        while (true) {
            Vector2 mouse_pos = endPosition;
            Vector2 object_pos = transform.position;
            mouse_pos.x = mouse_pos.x - object_pos.x;
            mouse_pos.y = mouse_pos.y - object_pos.y;
            float angle = Mathf.Atan2(mouse_pos.y, mouse_pos.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            startPos.transform.position = Vector3.MoveTowards(startPos.transform.position, endPosition, Time.deltaTime * movespeed);
            //Vector3 direction = (endPosition - startPos.transform.position).normalized * 100; ;
            //startPos.GetComponent<Rigidbody2D>().velocity = direction * Time.deltaTime * movespeed;
            yield return null;
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

    //public void OnDrawGizmos()
    //{
    //    if (pathArray != null)
    //    {
    //        for (int i = 0; i < pathArray.Length-1; i++)
    //        {
    //            Gizmos.color = Color.black;
    //            Gizmos.DrawCube(pathArray[i], Vector3.one);
    //            Gizmos.DrawLine(pathArray[i], pathArray[i+1]);
    //        }
    //    }
    //}



}