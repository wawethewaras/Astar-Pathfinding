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
                if (s_Instance == null) {
                    Debug.Log("Could not locate a GridManager object. \n You have to have exactly one GridManager in the scene.");
                    Debug.Break();
                }

            }
            return s_Instance;
        }
    }
    public Transform player;
    public LayerMask unwalkableMask;
    public TerrainType[] walkableRegions;
    public LayerMask walkableMask;
    Dictionary<int, int> walkableRegionsDictonary = new Dictionary<int, int>();

    public Vector2 gridWorldSize;
    public float nodeRadius;
    Node[,] grid;

    float nodeDiameter { get { return nodeRadius * 2; } }
    int gridSizeX, gridSizeY;

    public bool showGrid;
    public List<Node> path;

    public bool cutCorners;

    public int Maxsize {
        get {
            return gridSizeX * gridSizeY;
        }
    }

    void Awake()
    {
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        foreach (TerrainType region in walkableRegions) {
            walkableMask.value |= region.terrainMask.value;
            walkableRegionsDictonary.Add(Mathf.RoundToInt(Mathf.Log(region.terrainMask.value,2)),region.TerrainPenalty);
        }
        CreateGrid();
    }

    public void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                bool walkable = true;
                //print(worldPoint);
                int movementPenalty = 0;

                //Ray ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
                RaycastHit2D hit = Physics2D.Linecast(worldPoint, Vector2.one, walkableMask);

                if (Physics2D.Linecast(worldPoint, Vector2.one, walkableMask)) {
                    walkableRegionsDictonary.TryGetValue(hit.collider.gameObject.layer,out movementPenalty);
                }

                grid[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);
                ////Precalculate obstacles
                AStar.CheckIfNodeIsObstacle(grid[x, y]);
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;


                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    Node newNode = grid[checkX, checkY];
                    //Calculate obstacles while creating path
                    //AStar.CheckIfNodeIsObstacle(newNode);
                    //Prevent corner cutting
                    if (cutCorners == false && (grid[checkX, node.gridY].walkable == false || grid[node.gridX, checkY].walkable == false || grid[checkX, checkY].walkable == false))
                    {
                        continue;
                    }
                    else {
                        neighbours.Add(newNode);
                    }
                }
            }
        }

        return neighbours;
    }


    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float positionOfNodeInGridX = (worldPosition.x - transform.position.x);
        float positionOfNodeInGridY = (worldPosition.y - transform.position.y);
        float percentX = (positionOfNodeInGridX + gridWorldSize.x/2) / gridWorldSize.x;
        float percentY = (positionOfNodeInGridY + gridWorldSize.y/2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        ////If target node is inside collider return nearby node
        //if (grid[x, y].walkable == false)
        //{
        //    print("This happened");
        //    List<Node> neigours = GetNeighbours(grid[x, y]);
        //    foreach (Node n in neigours)
        //    {
        //        print(n.walkable);
        //        if (n.walkable)
        //        {
        //            return n;
        //        }
        //    }
        //}

        return grid[x, y];
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

                Gizmos.color = Color.blue;
                Gizmos.DrawCube(NodeFromWorldPoint(player.position).worldPosition, Vector3.one * (nodeDiameter - .1f));
            }
            if (AStar.pathFound) {
                //Shows nodes added to open list
                Gizmos.color = Color.yellow;
                for (int i = 0; i < AStar.openList.Count; i++)
                {
                    Gizmos.DrawCube(AStar.openList[i].worldPosition, Vector3.one * (nodeDiameter - .1f));

                }
                //Shows nodes added to closed list
                Gizmos.color = Color.red;
                for (int i = 0; i < AStar.closedList.Count; i++)
                {
                    Gizmos.DrawCube(AStar.closedList[i].worldPosition, Vector3.one * (nodeDiameter - .1f) * 0.3f);

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

    [System.Serializable]
    public class TerrainType {
        public LayerMask terrainMask;
        public int TerrainPenalty;
    }

}

[CustomEditor(typeof(Grid))]
public class ObjectBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Grid myScript = (Grid)target;
        if (GUILayout.Button("Build Object"))
        {
            myScript.CreateGrid();
        }
    }
}

