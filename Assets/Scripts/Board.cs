using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public List<int> size = new List<int>();
    public int dimensions = 2;
    //public Vector3 size = new Vector3(8, 1, 8); // Depricated
    public GameObject boardSquare;
    public Color colorA;
    public Color colorB;

    // Start is called before the first frame update
    void Start()
    {
        InitBoard();
    }

    void InitBoard()
    {
        List<int> startPos = new List<int>();
        for (int i = 0; i < dimensions; i++)
            startPos[i] = -size[i]/2;         // Always keep the center on 0,0   or 0,0,0      or 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
        
    }

    void SpawnSquares(int index, List<int> iter)
    {
        if (index >= dimensions) 
        {
            GameObject newTile = Instantiate(boardSquare, new Vector3(iter[0]-size.x/2, iter[1]*2-size.y, iter[2]-size.z/2), Quaternion.identity);
            int evens = 0;
            for (int i = 0; i < iter.size(); i++)
                evens += iter[i]%2 == 0 ? 1 : 0;
            if (evens <= 1)
                newTile.GetComponent<Renderer>().material.SetColor("_Color", colorA);
            else
                newTile.GetComponent<Renderer>().material.SetColor("_Color", colorB);
            newTile.GetComponent<Tile>.position = new List<int>(iter);
            return;
        }
        for (int i = 0; i < size[index]; i++)
        {
            iter[index] = i;
            SpawnSquares(index + 1, new List<int>(iter))
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
