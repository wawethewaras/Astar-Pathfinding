using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[System.Serializable]
public class TerrainType
{
    public LayerMask terrainMask;
    public int terrainPenalty;
}

public class Grid : MonoBehaviour
{
    private static Grid s_Instance = null;
    public static Grid instance {
        get {
            if (s_Instance == null) {
                s_Instance = FindObjectOfType(typeof(Grid)) as Grid;
                if (s_Instance == null) {
                    Debug.Log("Could not locate a GridManager object. \n You have to have exactly one GridManager in the scene.");
                    Debug.Break();
                }
            }
            return s_Instance;
        }
    }
    [Header("GRID")]
    public Vector2 gridWorldSize;
    public float nodeRadius;
    private Node[,] grid;

    public float nodeDiameter { get { return nodeRadius * 2; } }
    private int gridSizeX, gridSizeY;

    [Header("LAYERS")]
    public LayerMask unwalkableMask;
    public TerrainType[] walkableRegions;
    LayerMask walkableMask;
    Dictionary<int, int> walkableRegionsDictionary = new Dictionary<int, int>();

    [Header("Advanced")]
    public bool showGrid;
    public bool cutCorners;


    //This is for showing calculated path. Can be removed from final version
    public Transform player;
    //For showing calculated path. Should be removed from final version.
    public static List<Node> openList = new List<Node>();
    public static List<Node> closedList = new List<Node>();
    public static bool pathFound;


    public int Maxsize {
        get {
            return gridSizeX * gridSizeY;
        }
    }

    void Awake()
    {
        //Adding walkable regions to dictonary
        foreach (TerrainType region in walkableRegions)
        {
            walkableMask.value |= region.terrainMask.value;
            walkableRegionsDictionary.Add((int)Mathf.Log(region.terrainMask.value, 2), region.terrainPenalty);

        }

        CreateGrid();

    }

    public void CreateGrid() {
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;
        for (int x = 0; x < gridSizeX; x++) {
            for (int y = 0; y < gridSizeY; y++) {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                bool walkable = (Physics2D.OverlapCircle(worldPoint, nodeRadius, unwalkableMask) == null);

                int movementPenalty = 0;
                int newPenalty = 0;

                Collider2D[] hit = Physics2D.OverlapCircleAll(worldPoint,nodeRadius, walkableMask);
                if (hit.Length > 0) {
                    for (int i = 0; i < hit.Length; i++) {
                        walkableRegionsDictionary.TryGetValue(hit[i].gameObject.layer, out newPenalty);

                        //Return terrain with highest movement penalty
                        if (newPenalty > movementPenalty) {
                            movementPenalty = newPenalty;

                        }
                    }
                }

                grid[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);

            }
        }
    }

    public Node[] GetNeighbours(Node node) {
        Node[] neighbours = new Node[8];
        int index = 0;

        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {

                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
                    Node newNode = grid[checkX, checkY];
                    if (node.parent == newNode) {
                        continue;
                    }
                    //Calculate obstacles while creating path
                    //AStar.CheckIfNodeIsObstacle(newNode);

                    //Prevent corner cutting
                    if (cutCorners == false && (grid[checkX, checkY].walkable == false || grid[checkX, node.gridY].walkable == false || grid[node.gridX, checkY].walkable == false)) {
                        continue;
                    }
                    else {
                        neighbours[index] = newNode;
                        index++;
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
        return grid[x, y];
    }



    public Node PlayerNodeFromWorldPoint(Vector3 worldPosition)
    {
        float positionOfNodeInGridX = (worldPosition.x - transform.position.x);
        float positionOfNodeInGridY = (worldPosition.y - transform.position.y);
        float percentX = (positionOfNodeInGridX + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (positionOfNodeInGridY + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        //If target node is inside collider return nearby node
        if (grid[x, y].walkable == false)
        {
            //Node[] neighbours 
            Node neighbour = FindWalkableInRadius(x, y, 1);
            if (neighbour != null)
            {
                return neighbour;
            }
        }

        return grid[x, y];
    }

    Node FindWalkableInRadius(int centreX, int centreY, int radius)
    {
        
        for (int i = -radius; i <= radius; i++) {
            int verticalSearchX = i + centreX;
            int horizontalSearchY = i + centreY;

            // top
            if (InBounds(verticalSearchX, centreY + radius))
            {
                if (grid[verticalSearchX, centreY + radius].walkable)
                {
                    return grid[verticalSearchX, centreY + radius];
                }
            }

            // bottom
            if (InBounds(verticalSearchX, centreY - radius))
            {
                if (grid[verticalSearchX, centreY - radius].walkable)
                {
                    return grid[verticalSearchX, centreY - radius];
                }
            }
            // right
            if (InBounds(centreY + radius, horizontalSearchY))
            {
                if (grid[centreX + radius, horizontalSearchY].walkable)
                {
                    return grid[centreX + radius, horizontalSearchY];
                }
            }

            // left
            if (InBounds(centreY - radius, horizontalSearchY))
            {
                if (grid[centreX - radius, horizontalSearchY].walkable)
                {
                    return grid[centreX - radius, horizontalSearchY];
                }
            }

        }
        radius++;
        if (radius > 10) { return null; }
        return FindWalkableInRadius(centreX, centreY, radius);

    }
    bool InBounds(int x, int y)
    {
        return x >= 0 && x < gridSizeX && y >= 0 && y < gridSizeY;
    }

    public static void CheckIfNodeIsObstacle(Node node)
    {
        ////Calculate obstacles while creating path
        Collider2D[] colliders = Physics2D.OverlapCircleAll(node.worldPosition, Grid.instance.nodeRadius, Grid.instance.unwalkableMask);
        if (colliders.Length > 0)
        {
            node.walkable = false;
        }
        else {
            node.walkable = true;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));

        if (showGrid) { 

            if (grid != null)
            {
                foreach (Node n in grid)
                {
                    Gizmos.color = (n.walkable) ? Color.white : Color.red;
                    //if (path != null)
                    //    if (path.Contains(n))
                    //        Gizmos.color = Color.black;
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
                }

                Gizmos.color = Color.blue;
                Gizmos.DrawCube(NodeFromWorldPoint(player.position).worldPosition, Vector3.one * (nodeDiameter - .1f));
            }
            if (pathFound) {
                //Shows nodes added to open list
                Gizmos.color = Color.yellow;
                for (int i = 0; i < openList.Count; i++)
                {
                    Gizmos.DrawSphere(openList[i].worldPosition, (nodeRadius - .1f));

                }
                //Shows nodes added to closed list
                Gizmos.color = Color.red;
                for (int i = 0; i < closedList.Count; i++)
                {
                    Gizmos.DrawCube(closedList[i].worldPosition, Vector3.one * (nodeDiameter - .1f) * 0.3f);

                }

                //Draws line from node to it's parent
                Gizmos.color = Color.green;
                for (int i = 0; i < closedList.Count; i++)
                {
                    if (closedList[i].parent != null)
                    {
                        Gizmos.DrawLine(closedList[i].worldPosition, closedList[i].parent.worldPosition);
                    }

                }

            }

        }
    }



}

[CustomEditor(typeof(Grid))]
public class ObjectBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Grid myScript = (Grid)target;
        if (GUILayout.Button("Create grid"))
        {
            myScript.CreateGrid();
        }
    }
}

