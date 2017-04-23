using UnityEngine;
using System.Collections;
public class TestCode : MonoBehaviour
{
    private Transform startPos, endPos;
    public Node startNode { get; set; }
    public Node goalNode { get; set; }
    public ArrayList pathArray;
    public GameObject objStartCube, objEndCube;
    private float elapsedTime = 0.0f;
    //Interval time between pathfinding
    public float intervalTime = 1.0f;

    public ArrayList openList;
    void Start()
    {
        objStartCube = GameObject.FindGameObjectWithTag("Start");
        objEndCube = GameObject.FindGameObjectWithTag("End");
        pathArray = new ArrayList();
        FindPath();
    }
    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= intervalTime)
        {
            elapsedTime = 0.0f;
            FindPath();
        }
    }

    public void FindPath()
    {
        PriorityQueue.closedList.Clear();
        PriorityQueue.openList.Clear();
        startPos = objStartCube.transform;
        endPos = objEndCube.transform;
        startNode = new Node(GridManager.instance.GetGridCellCenter(
        GridManager.instance.GetGridIndex(startPos.position)));
        goalNode = new Node(GridManager.instance.GetGridCellCenter(
        GridManager.instance.GetGridIndex(endPos.position)));
        pathArray = AStar.FindPath(startNode, goalNode);
        
    }
    void OnDrawGizmos()
    {
        if (pathArray == null)
            return;
        if (pathArray.Count > 0)
        {
            int index = 1;
            foreach (Node node in pathArray)
            {
                if (index < pathArray.Count)
                {
                    Node nextNode = (Node)pathArray[index];
                    Debug.DrawLine(node.position, nextNode.position,
                    Color.green);
                    index++;
                }
            }
        }
        Gizmos.color = Color.yellow;
        for (int i = 0; i < PriorityQueue.openList.Count;i++)
        {
            Gizmos.DrawCube(PriorityQueue.openList[i], Vector3.one);

        }
        Gizmos.color = Color.red;
        for (int i = 0; i < PriorityQueue.closedList.Count; i++)
        {
            Gizmos.DrawCube(PriorityQueue.closedList[i], Vector3.one*0.3f);

        }

    }


}