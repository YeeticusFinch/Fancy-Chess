using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private List<int> location;
    public ChessPiece piece;
    public GameObject underside;
    public Board board;
    public TextMesh label;
    public ChessPiece enPassant = null;

    public static bool deselect = false;
    private static bool deselecting = false;

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
        GetComponent<Renderer>().material.SetColor("_Color", c);
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
        if (deselect && !GetComponent<Renderer>().material.GetColor("_EmissionColor").Equals(Color.black))
        {
            //ClearColor();
            if (!deselecting)
                StartCoroutine(Dedeselect());
        }
        if (GameMaster.debug)
        {
            if (piece != null)
            {
                label.text = (piece.white ? "W " : "B ") + piece.type;
            }
            else
            {
                label.text = "";
            }
            if (ChessPiece.enPassantTile == this)
            {
                label.text += " ept";
            }
            if (enPassant != null)
            {
                label.text += " ep";
            }
            label.transform.eulerAngles = new Vector3(90, GameMaster.instance.cam.transform.eulerAngles.y, 0);
        } else if (!GameMaster.debug && label.text != null && label.text.Length > 0)
        {
            label.text = "";
        }
    }
    
    IEnumerator Dedeselect()
    {
        deselecting = true;
        yield return new WaitForSeconds(0.1f);
        deselect = false;
        deselecting = false;
    }

    public void Click()
    {
        Click(this.piece);
        ChessPiece.enPassantTile = null;
    }

    public void Click(ChessPiece piece) // TODO: figure out why the piece location doesn't lign up when a piece is clicked
    {
        //if (GetComponent<MeshRenderer>().enabled == false)
        //    return;
        //deselect = true;
        foreach (Tile square in board.squares.Values)
        {
            if (!square.gameObject.Equals(gameObject))
                square.ClearColor();
        }
        GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        clicked = Time.time + 0.1f;
        if (GameMaster.rachel && piece != null)
        {
            piece.Die();
            GameMaster.NoRachel();
            return;
        }
        if (GetComponent<Renderer>().material.GetColor("_EmissionColor").Equals(Color.green) && GameMaster.selectedPiece != null) // If true, move the piece over
        {
            //TO DO: check if this move puts you in check
            //Debug.Log("Moving the Piece!");
            List<int> oldLoc = GameMaster.selectedPiece.location;
            string str = CarlMath.ListAsString(oldLoc);
            board.squares[str].piece = null;
            piece = GameMaster.selectedPiece;
            piece.location = location;
            if (ChessPiece.enPassantTile != null && ChessPiece.enPassantTile == this && piece != null && piece.type.Equals("pawn"))
            {
                board.squares[CarlMath.ListAsString(CarlMath.halfway(oldLoc, location))].enPassant = piece;
                piece.enPassant = board.squares[CarlMath.ListAsString(CarlMath.halfway(oldLoc, location))];
            }
            else if (piece != null && piece.enPassant != null)
            {
                piece.enPassant.enPassant = null;
                piece.enPassant = null;
            }
            StartCoroutine(MoveTo(piece.gameObject, transform.position + (GameMaster.bowling ? Vector3.up * 0.5f : Vector3.zero), 0.2f, piece));
            piece.hasMoved = true;
            if (!GameMaster.debug)
            {
                GameMaster.usedTurns++;
                if (GameMaster.availableTurns() <= GameMaster.usedTurns)
                {
                    GameMaster.instance.board.whiteTurn = !piece.white;
                    GameMaster.instance.board.canMove = false;
                    GameMaster.usedTurns = 0;
                }
            }
            //piece.transform.position = transform.position;
            //GetComponent<Renderer>().material.SetColor("_Color", color);
            //ClearColor();
            //clicked = 0;
            //deselect = true;
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

    public string console = "";

    IEnumerator MoveTo(GameObject item, Vector3 pos, float seconds, ChessPiece piece)
    {
        //item.SetActive(true);
        //foreach (Collider col in ghost)
        //    col.isTrigger = true;
        item.GetComponent<ChessPiece>().movingTo = pos;
        //piece.GetComponent<Rigidbody>().useGravity = false;
        piece.GetComponent<Rigidbody>().freezeRotation = true;
        //piece.GetComponent<Rigidbody>().isKinematic = true;
        item.GetComponent<MeshRenderer>().enabled = true;
        item.GetComponent<BoxCollider>().enabled = true;
        if (GameMaster.bowling) item.GetComponent<Rigidbody>().useGravity = true;
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
        if (GameMaster.bowling)
        {
            yield return new WaitForSecondsRealtime(0.5f);
            board.UpdatePieceLocations();
        } else
        {
            if (this.piece != null)
                this.piece.Die();
            if (enPassant != null && (enPassant.white != piece.white || GameMaster.canabalism))
            {
                enPassant.Die();
                enPassant = null;
            }
            this.piece = piece;
        }
    }

    IEnumerator AddToBoard()
    {
        yield return new WaitForSeconds(0.1f);
        GameMaster.instance.board.squares.Add(CarlMath.ListAsString(location), this);
    }
}
