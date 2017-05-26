using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class CountPath : MonoBehaviour, Pathfinding
{
    public Transform startPos, endPos;
    private Vector3[] pathArray;

    //Interval time between pathfinding
    public float intervalTime = 1.0f;

    IEnumerator currentPath;

    public Vector3 endPosition;

    public float movespeed;
    

    [HideInInspector]
    public bool readyToCountPath;

    public bool autoCountPath;
    public bool showPathSmoothing;

    void Start()
    {
        readyToCountPath = true;
        startPos = transform;
    }
    void Update()
    {
        //if (Physics2D.Linecast(startPos.position, endPos.position, Grid.instance.unwalkableMask))
        //{
        //    UnityEngine.Debug.DrawLine(startPos.position, endPos.position, Color.red);
        //}
        //else {
        //    UnityEngine.Debug.DrawLine(startPos.position, endPos.position, Color.blue);
        //}

        if (autoCountPath && readyToCountPath)
        {
            FindPath(startPos, endPos);
        }
        else {
            if (Input.GetButtonDown("Jump"))
            {
                FindPath(startPos, endPos);
            }
        }


    }

    public void FindPath(Transform startPos, Transform endPos) {
        if (startPos == null || endPos == null) {
            print("Missing start position or endposition");
            return;
        }
        if (endPos.position != endPosition) {
            endPosition = endPos.position;
            readyToCountPath = false;

            if (Grid.instance.useThreading)
            {
                ThreadController.SearchPathRequest(this, startPos.position, endPosition);

            }
            else {

                Node start = Grid.instance.ClosestNodeFromWorldPoint(startPos.position);
                Node end = Grid.instance.ClosestNodeFromWorldPoint(endPosition);
                Vector3[] newPath = AStar.FindPath(start, end);
                OnPathFound(newPath);
                StartCoroutine(PathCountDelay());
            }
        }
    }

    public void OnPathFound(Vector3[] newPath) {
        if (currentPath != null)
        {
            StopCoroutine(currentPath);

        }
        currentPath = movepath(newPath);
        pathArray = newPath;
        StartCoroutine(currentPath);

    }

    public IEnumerator movepath(Vector3[] pathArray) {
        if (pathArray == null) {
            yield break;
        }
        for (int i = 0; i < pathArray.Length; i++) {
            while (startPos.transform.position != pathArray[i]) {

                ////if (Physics2D.Linecast(startPos.transform.position, endPosition, Grid.instance.unwalkableMask) == false)
                ////{
                ////    UnityEngine.Debug.DrawLine(startPos.transform.position, endPosition, Color.black, 10);
                ////    break;
                ////}

                if (i < pathArray.Length - 1)
                {
                    bool cantSeeTarget = Physics2D.Linecast(startPos.transform.position, pathArray[i + 1], Grid.instance.unwalkableMask);
                    if (cantSeeTarget == false)
                    {
                        if (showPathSmoothing)
                        {
                            UnityEngine.Debug.DrawLine(startPos.transform.position, pathArray[i + 1], Color.black, 10);
                        }
                        i++;


                    }
                }
                else {
                    bool cantSeeTarget = Physics2D.Linecast(startPos.transform.position, endPos.position, Grid.instance.unwalkableMask);
                    if (cantSeeTarget == false)
                    {
                        if (showPathSmoothing)
                        {
                            UnityEngine.Debug.DrawLine(startPos.transform.position, endPos.position, Color.black, 10);

                        }
                        break;
                    }
                    
                }
                Vector2 target_pos = pathArray[i];
                Vector2 my_pos =  transform.position;
                target_pos.x = target_pos.x - my_pos.x;
                target_pos.y = target_pos.y - my_pos.y;
                float angle = Mathf.Atan2(target_pos.y, target_pos.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

                startPos.transform.position = Vector3.MoveTowards(startPos.transform.position, pathArray[i], Time.deltaTime * movespeed);
                //Vector3 direction = (pathArray[i] - startPos.transform.position).normalized * 100;
                //startPos.GetComponent<Rigidbody2D>().velocity = direction * Time.deltaTime * movespeed;

                yield return null;
            }
        }
        while (true) {
            Vector2 target_pos = endPos.position;
            Vector2 my_pos = transform.position;
            target_pos.x = target_pos.x - my_pos.x;
            target_pos.y = target_pos.y - my_pos.y;
            float angle = Mathf.Atan2(target_pos.y, target_pos.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            startPos.transform.position = Vector3.MoveTowards(startPos.transform.position, endPos.position, Time.deltaTime * movespeed);
            //Vector3 direction = (endPosition - startPos.transform.position).normalized * 100; ;
            //startPos.GetComponent<Rigidbody2D>().velocity = direction * Time.deltaTime * movespeed;
            yield return null;
        }
    }

    public IEnumerator PathCountDelay()
    {
        readyToCountPath = false;
        float counter = Random.Range(intervalTime + 0.1f, intervalTime + 0.15f);
        yield return new WaitForSeconds(counter);
        readyToCountPath = true;

    }

    //Draw path to gizmoz
    public void OnDrawGizmos()
    {
        if (pathArray != null)
        {
            for (int i = 0; i < pathArray.Length - 1; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(pathArray[i], Vector3.one);
                Gizmos.DrawLine(pathArray[i], pathArray[i + 1]);
            }
        }
    }



}