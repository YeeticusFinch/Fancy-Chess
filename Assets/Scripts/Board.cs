using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public List<int> size = new List<int>();
    public List<int> dimensionsToMirror = new List<int>(); // when setting up the pieces on the board, mirror dimensions of these numbers
    [HideInInspector]
    public int dimensions = 2;
    //public Vector3 size = new Vector3(8, 1, 8); // Depricated
    public GameObject boardSquare;
    public Color colorA;
    public Color colorB;

    [HideInInspector]
    public Dictionary<string, Tile> squares = new Dictionary<string, Tile>();

    [HideInInspector]
    public List<List<int>> forwards = null;
    [HideInInspector]
    public List<List<int>> laterals = null;
    [HideInInspector]
    public List<List<int>> diagonals = null;

    //public GameMaster gameMaster;

    // Start is called before the first frame update
    void Start()
    {
        InitBoard();
        InitDirections();
    }

    void InitBoard()
    {
        dimensions = size.Count;
        //List<int> startPos = new List<int>();
        //for (int i = 0; i < dimensions; i++)
        //    startPos.Add(-size[i]/2);         // Always keep the center on 0,0   or 0,0,0      or 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
        List<int> startIter = new List<int>();
        for (int i = 0; i < dimensions; i++)
            startIter.Add(0);
        SpawnSquares(0, startIter);
    }

    void SpawnSquares(int index, List<int> iter)
    {
        if (index >= dimensions) 
        {
            GameObject newTile = Instantiate(boardSquare, new Vector3(dimensions > 0 ? iter[0]-size[0]/2 + 0.5f : 0, dimensions > 2 ? 1.5f*(iter[2]*2-size[2]) : 0, dimensions > 1 ? iter[1]-size[1]/2 + 0.5f : 0), Quaternion.identity);
            int evens = 0;
            for (int i = 0; i < iter.Count; i++)
                evens += iter[i]%2 == 0 ? 1 : 0;
            if (evens % 2 != 0)
            {
                newTile.GetComponent<Tile>().SetColor(colorA);
            }
            else
            {
                newTile.GetComponent<Tile>().SetColor(colorB);
            }
            newTile.GetComponent<Tile>().SetLocation(new List<int>(iter));
            return;
        }
        for (int i = 0; i < size[index]; i++)
        {
            iter[index] = i;
            SpawnSquares(index + 1, new List<int>(iter));
        }
    }

    void InitDirections() // sets up unit vectors for each movement direction (yes this can change based on board configurations)
    {
        forwards = new List<List<int>>(); // May not need this one actually
        laterals = new List<List<int>>();
        diagonals = new List<List<int>>();

        for (int i = 0; i < dimensions; i++)
        {
            laterals.Add(new List<int>());   // Each "lateral" direction is a unit vector parallel to an axis
            for (int j = 0; j < i; j++)
                laterals[laterals.Count - 1].Add(0);
            laterals[laterals.Count - 1].Add(1);

            laterals.Add(new List<int>());   // Each "lateral" direction is a unit vector parallel to an axis
            for (int j = 0; j < i; j++)
                laterals[laterals.Count - 1].Add(0);
            laterals[laterals.Count - 1].Add(-1);

            // Each "diagonal" direction is a unit vector in two axes
            for (int j = 0; j < i; j++)
            {
                diagonals.Add(new List<int>());
                for (int k = j; k < i; k++)
                    laterals[laterals.Count - 1].Add(0);
            }
            laterals[laterals.Count - 1].Add(-1);

            if (dimensionsToMirror.Contains(i))
            {
                forwards.Add(new List<int>());    // "forwards" is any axis that is mirrored
                for (int j = 0; j < i; j++)
                    forwards[forwards.Count - 1].Add(0);
                forwards[forwards.Count - 1].Add(1);
            }
        }
    }


    /*
    void InitBoard()
    {
        Vector3 startPos = new Vector3(-size.x/2, -size.y, -size.z/2);
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                for (int k = 0; k < size.z; k++)
                {
                    GameObject newTile = Instantiate(boardSquare, startPos+(new Vector3(i, j*2, k)), Quaternion.identity);
                    if ((i%2 == 0 && j%2 != 0 && k%2 != 0) || (i % 2 != 0 && j % 2 == 0 && k % 2 != 0) || (i % 2 != 0 && j % 2 != 0 && k%2 == 0) || (i % 2 == 0 && j % 2 == 0 && k % 2 == 0))
                        newTile.GetComponent<Renderer>().material.SetColor("_Color", colorA);
                    else
                        newTile.GetComponent<Renderer>().material.SetColor("_Color", colorB);
                }
            }
        }
    }*/

    // Update is called once per frame
    void Update()
    {
        
    }
}
