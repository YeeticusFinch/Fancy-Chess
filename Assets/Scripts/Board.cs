using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Vector3 size = new Vector3(8, 1, 8);
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
