using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    Node[,] grid;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public LayerMask unwalkableMask;
    public List<Node> path; //list of nodes in path

    float nodeDiameter;
    int gridSizeX;
    int gridSizeY;

    private void Start()
    {
        nodeDiameter = 2 * nodeRadius;

        //Calculates how many nodes fit in the grid
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];

        //Takes the center coordinates and moves the point to the left and bottom (forward in 3D is up in the grid)
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        //Runs through the grid and checks if the nodes are walkable or not
        for(int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeX; y++)
            {
                //Moves through each point a node will occupy
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius,unwalkableMask));//Physics.CheckSphere returns true if there is a collision with an unwalkable
                grid[x, y] = new Node(walkable,worldPoint,x,y); //Creates a Node on the grid containing its position and walkable bool
            }
        }
    }

    //Finds the node closest to a coordinate
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x; //How far along the x axis the point is
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y; //How far along the y axis the point is

        //Clamps between 0 and 1
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        //applies the percentage to the number of nodes in the direction, then rounds it to the nearest node
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    //Makes a list of neighbors around a node (variable size!)
    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        //looks at the 3x3 grid centered on a node
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue; //skips the center node
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                //Checks if the node is inside the overall grid
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX,checkY]);
                }
            }
        }
        return neighbours;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y)); //the 2D Y axis is aligned with the 3D Z axis

        if(grid != null)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = (n.walkable) ? Color.white : Color.red; //if the node is walkable, color is white, else, color is red
                if (path.Contains(n))
                    Gizmos.color = Color.black;
                Gizmos.DrawCube(n.worldPosition, Vector3.one*(nodeDiameter-.1f));
            }
        }
    }
}
