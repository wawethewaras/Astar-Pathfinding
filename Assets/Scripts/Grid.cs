using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class Grid : MonoBehaviour
{
    private static Grid s_Instance = null;
    public static Grid instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType(typeof(Grid))
                as Grid;
                if (s_Instance == null)
                    Debug.Log("Could not locate a GridManager object. \n You have to have exactly one GridManager in the scene.");
            }
            return s_Instance;
        }
    }
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    Node[,] grid;

    float nodeDiameter { get { return nodeRadius * 2; } }
    int gridSizeX, gridSizeZ;

    public bool showGrid;
    public List<Node> path;



    void Awake()
    {
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeZ = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    public void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeZ];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (z * nodeDiameter + nodeRadius);
                Collider2D[] colliders = Physics2D.OverlapCircleAll(worldPoint, nodeRadius, unwalkableMask);
                bool walkable = true;
                if (colliders.Length > 0) {
                    walkable = false;
                }
                grid[x, z] = new Node(walkable, worldPoint, x, z);
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                if (x == 0 && z == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkZ = node.gridY + z;

                if (checkX >= 0 && checkX < gridSizeX && checkZ >= 0 && checkZ < gridSizeZ)
                {
                    neighbours.Add(grid[checkX, checkZ]);
                }
            }
        }

        return neighbours;
    }


    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentZ = (worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentZ = Mathf.Clamp01(percentZ);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int z = Mathf.RoundToInt((gridSizeZ - 1) * percentZ);
        return grid[x, z];
    }


    void OnDrawGizmos()
    {
        if (showGrid) { 
            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));

            if (grid != null)
            {
                foreach (Node n in grid)
                {
                    Gizmos.color = (n.walkable) ? Color.white : Color.red;
                    if (path != null)
                        if (path.Contains(n))
                            Gizmos.color = Color.black;
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
                }
            }
            if (AStar.pathFound) {
                //Shows nodes added to open list
                Gizmos.color = Color.yellow;
                for (int i = 0; i < AStar.openList.Count; i++)
                {
                    Gizmos.DrawCube(AStar.openList[i].worldPosition, Vector3.one);

                }
                //Shows nodes added to closed list
                Gizmos.color = Color.red;
                for (int i = 0; i < AStar.closedList.Count; i++)
                {
                    Gizmos.DrawCube(AStar.closedList[i].worldPosition, Vector3.one * 0.3f);

                }

                //Draws line from node to it's parent
                Gizmos.color = Color.green;
                for (int i = 0; i < AStar.closedList.Count; i++)
                {
                    if (AStar.closedList[i].parent != null)
                    {
                        Gizmos.DrawLine(AStar.closedList[i].worldPosition, AStar.closedList[i].parent.worldPosition);
                    }

                }
            }


        }
    }
}

