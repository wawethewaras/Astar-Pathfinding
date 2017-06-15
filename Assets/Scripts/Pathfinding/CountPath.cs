using UnityEngine;
using System.Collections;
using pathfinding;

public class CountPath : MonoBehaviour, pathfinding.Pathfinding
{
    private Transform startPos;
    private Vector3 endPos;
    private Vector3[] pathArray;



    private IEnumerator currentPath;
    private Vector3 endPosition;
    private bool readyToCountPath = true;


    public float movespeed;

    //Interval time between pathfinding
    public float intervalTime = 1.0f;
    public bool showPathSmoothing;
    public bool usePathSmooting;

    public void FindPath(Transform _seeker, Vector3 _endPos) {
        if (!readyToCountPath) {
            return;
        }

        else if (_seeker == null) {
            Debug.LogError("Missing seeker!",this);
            return;
        }

        startPos = _seeker;
        endPos = _endPos;
        Grid grid = Grid.instance;

        //Basic raycast if can move directly to end target
        bool cantSeeTarget = Physics2D.Linecast(_seeker.transform.position, _endPos, grid.unwalkableMask);
        if (cantSeeTarget == false)
        {
            Vector3[] newPath = new Vector3[1];
            newPath[0] = _endPos;
            OnPathFound(newPath);
            StartCoroutine(PathCountDelay());
            return;
        }

        if (_endPos != endPosition) {
            endPosition = _endPos;
            readyToCountPath = false;

            ThreadController.SearchPathRequest(this, _seeker.position, endPosition, grid);
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

                if (usePathSmooting && i < pathArray.Length - 1)
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
                    bool cantSeeTarget = Physics2D.Linecast(startPos.transform.position, endPos, Grid.instance.unwalkableMask);
                    if (cantSeeTarget == false)
                    {
                        if (showPathSmoothing)
                        {
                            UnityEngine.Debug.DrawLine(startPos.transform.position, endPos, Color.black, 10);

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
            Vector2 target_pos = endPos;
            Vector2 my_pos = transform.position;
            target_pos.x = target_pos.x - my_pos.x;
            target_pos.y = target_pos.y - my_pos.y;
            float angle = Mathf.Atan2(target_pos.y, target_pos.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            startPos.transform.position = Vector3.MoveTowards(startPos.transform.position, endPos, Time.deltaTime * movespeed);
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