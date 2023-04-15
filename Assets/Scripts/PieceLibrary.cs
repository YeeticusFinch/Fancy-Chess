using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceLibrary : MonoBehaviour
{
    public ChessPiece[] pieces;
    public int index = 0;
    public Tile square;

    // Start is called before the first frame update
    void Start()
    {
        foreach (ChessPiece p in pieces)
        {
            p.transform.localPosition = Vector3.zero;
            p.gameObject.GetComponent<Rigidbody>().useGravity = false;
            p.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
        pieces[0].gameObject.GetComponent<MeshRenderer>().enabled = true;
    }

    public void next(int add)
    {
        pieces[index].GetComponent<MeshRenderer>().enabled = false;
        index += add;
        index %= pieces.Length;
        if (index < 0) index = pieces.Length - index;
        pieces[index].GetComponent<MeshRenderer>().enabled = true;
    }

    public ChessPiece getPiece()
    {
        return pieces[index];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
