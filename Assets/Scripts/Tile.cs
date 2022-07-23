using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private List<int> location;
    public ChessPiece piece;
    public GameObject underside;

    Color color;
    float clicked = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void SetLocation(List<int> loc)
    {
        location = loc;
        StartCoroutine(AddToBoard());
    }

    public List<int> GetLocation()
    {
        return location;
    }

    public void SetColor(Color c)
    {
        color = c;
        GetComponent<Renderer>().material.SetColor("_Color", color);
        GetComponent<Tile>().underside.GetComponent<Renderer>().material.SetColor("_Color", color / 2);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > clicked)
        {
            GetComponent<Renderer>().material.SetColor("_Color", color);
        }
    }

    public void Click()
    {
        GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        clicked = Time.time + 0.1f;
    }

    IEnumerator AddToBoard()
    {
        yield return new WaitForSeconds(0.1f);
        GameMaster.instance.board.squares.Add(CarlMath.ListAsString(location), this);
    }
}
