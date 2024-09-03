using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool walkable;
    public Vector3 worldPosition;

    //position in the node grid
    public int gridX; 
    public int gridY;

    public int gCost; //distance from starting node
    public int hCost; //distant from end node
    public Node parent;

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }

    //Calculates fCost on the fly
    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
}
