using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public Transform seeker;
    public List<Transform> target = new List<Transform>(); 
    NodeGrid grid;

    private void Awake()
    {
        grid = GetComponent<NodeGrid>();
    }

    private void Update()
    {
        //finds Seeker
        seeker = GameObject.FindGameObjectWithTag("Seeker").transform;

        //Makes a list of players
        target.Clear();
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Player"))
        {
            target.Add(obj.transform);
        }
        if (target.Count == 0)
            return;

        //finds closest player
        int closestPlayer = 0;
        float closestDistance = Vector3.Distance(seeker.position, target[0].transform.position);
        for (int i = 0; i < target.Count; i++)
        {
            //Debug.Log(target[i].name + " " + Vector3.Distance(seeker.position, target[i].transform.position));
            if (Vector3.Distance(seeker.position, target[i].transform.position) < closestDistance)
            {
                closestPlayer = i;
                closestDistance = Vector3.Distance(seeker.position, target[i].transform.position);
            }
        }
        FindPath(seeker.position, target[closestPlayer].position);
    }
    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        //Convert world positions into node positions
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        List<Node> openSet = new List<Node>();  //The list of nodes to be evaluated
        HashSet<Node> closedSet = new HashSet<Node>();//The list of nodes already evaluated
        openSet.Add(startNode); //Add start node to open list

        while (openSet.Count > 0) //while there are nodes to be evaluated
        {
            Node currentNode = openSet[0];

            //looks for the cheapest node in the open set
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }
            openSet.Remove(currentNode);//removes cheapest node from open set
            closedSet.Add(currentNode); //adds cheapest node to closed set

            if (currentNode == targetNode) //if we found our target
            {
                //retrace steps from target back to start
                RetracePath(startNode, targetNode);
                return;
            }

            //Looks through each of the current node's neighbours
            foreach (Node neighbour in grid.GetNeighbours(currentNode)){
                //check if it is walkable or already explored
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                    continue;

                int newMovCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);

                    neighbour.parent = currentNode;

                    if(!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX-nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY-nodeB.gridY);

        if (dstX > dstY)
            return 14*dstY + 10*(dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        grid.path = path;
    }
}
