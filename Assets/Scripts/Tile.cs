using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private List<int> location;
    public ChessPiece piece;
    public GameObject underside;
    public Board board;

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

    public void TempColor(Color c)
    {
        //Debug.Log("Setting temp color");
        //transform.localScale *= 0.5f;
        GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
        GetComponent<Renderer>().material.SetColor("_EmissionColor", c);
        GetComponent<Renderer>().material.SetColor("_Color", color/2);
        //DynamicGI.UpdateEnvironment();
    }

    public void ClearColor()
    {
        GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
        GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.black);
        GetComponent<Renderer>().material.SetColor("_Color", color);
        //DynamicGI.UpdateEnvironment();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameMaster.gameRunning) return;
        if (Time.time > clicked && clicked != 0)
        {
            GetComponent<Renderer>().material.SetColor("_Color", color);
            clicked = 0;
        }
        if (Input.GetButtonDown("Fire1"))
        {
            ClearColor();
        }
    }

    public void Click()
    {
        GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        clicked = Time.time + 0.1f;
        if (GetComponent<Renderer>().material.GetColor("_EmissionColor").Equals(Color.green) && GameMaster.selectedPiece != null) // If true, move the piece over
        {
            //TO DO: check if this move puts you in check
            Debug.Log("Moving the Piece!");
            List<int> oldLoc = GameMaster.selectedPiece.location;
            string str = CarlMath.ListAsString(oldLoc);
            board.squares[str].piece = null;
            piece = GameMaster.selectedPiece;
            piece.location = location;
            StartCoroutine(MoveTo(piece.gameObject, transform.position + Vector3.up * 0.5f, 0.2f));
            piece.hasMoved = true;
            GameMaster.instance.board.whiteTurn = !piece.white;
            GameMaster.instance.board.canMove = false;
            //piece.transform.position = transform.position;
        }
        if (piece != null && board.whiteTurn == piece.white)
        {
            Debug.Log("Clicked on " + piece.type);
            GameMaster.selectedPiece = piece;
            List<List<int>> moves = piece.GetMovableLocations();
            TempColor(Color.yellow);
            if (moves != null && moves.Count > 0)
            {
                Debug.Log("Found " + moves.Count + " possible moves");
                foreach (List<int> pos in moves)
                {
                    string str = CarlMath.ListAsString(pos);
                    if (pos != null && board.squares.ContainsKey(str) && board.squares[str] != null)
                    {
                        board.squares[str].TempColor(Color.yellow);
                    }
                }
            }
            else Debug.Log("This piece cannot move");
        }
    }
    
    IEnumerator MoveTo(GameObject item, Vector3 pos, float seconds)
    {
        //foreach (Collider col in ghost)
        //    col.isTrigger = true;
        item.GetComponent<ChessPiece>().movingTo = pos;
        //piece.GetComponent<Rigidbody>().useGravity = false;
        piece.GetComponent<Rigidbody>().freezeRotation = true;
        //piece.GetComponent<Rigidbody>().isKinematic = true;
        float dist = (pos - item.transform.position).magnitude;
        float step = (pos - item.transform.position).magnitude / (seconds * 100);
        while ((pos - item.transform.position).magnitude > 0.03f && item.GetComponent<ChessPiece>().movingTo == pos && !(board.canMove && board.whiteTurn != item.GetComponent<ChessPiece>().white))
        {
            if ((pos - item.transform.position).magnitude < 0.1f)
            {
                item.transform.position = pos;
                item.GetComponent<Rigidbody>().velocity = Vector3.zero;
                break;
            }
            else if ((pos - item.transform.position).magnitude < 0.5f)
                item.GetComponent<Rigidbody>().velocity = (pos - item.transform.position) / seconds;
            else
                item.GetComponent<Rigidbody>().velocity = (pos - item.transform.position).normalized * dist / seconds;
            //item.transform.position += (pos - item.transform.position).normalized * Mathf.Min(step, (pos - item.transform.position).magnitude);
            yield return new WaitForSecondsRealtime(0.01f);
        }
        //foreach (Collider col in ghost)
        //    col.isTrigger = false;
        item.GetComponent<Rigidbody>().velocity = Vector3.zero;
        piece.GetComponent<Rigidbody>().freezeRotation = false;
        //piece.GetComponent<Rigidbody>().isKinematic = false;
        //piece.GetComponent<Rigidbody>().useGravity = true;
        item.GetComponent<ChessPiece>().movingTo = Vector3.zero;
    }

    IEnumerator AddToBoard()
    {
        yield return new WaitForSeconds(0.1f);
        GameMaster.instance.board.squares.Add(CarlMath.ListAsString(location), this);
    }
}
