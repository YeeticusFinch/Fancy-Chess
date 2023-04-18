using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Board : MonoBehaviour
{
    public int pieceSet = 0;

    public string gameModeName = "Fancy Chess";
    public string description = "Fancy description";

    public int xAxis = 0;
    public int yAxis = 1;
    public int zAxis = -1;
    public int xxAxis = -1;
    public int yyAxis = -1;
    public int zzAxis = -1;

    public LayerMask mask;
    public List<int> size = new List<int>();
    public List<int> dimensionsToMirror = new List<int>(); // when setting up the pieces on the board, mirror dimensions of these numbers
    [HideInInspector]
    public int dimensions = 2;
    //public Vector3 size = new Vector3(8, 1, 8); // Depricated
    public GameObject boardSquare;
    public Color colorA;
    public Color colorB;
    public bool swapBlackAndWhite = false;

    [HideInInspector]
    public Dictionary<string, Tile> squares = new Dictionary<string, Tile>();

    [HideInInspector]
    public List<List<int>> forwards = null;
    [HideInInspector]
    public List<List<int>> laterals = null;
    [HideInInspector]
    public List<List<int>> diagonals = null;

    public int groundLevel = 0;

    public bool bowling = false;
    public bool canabalism = false;
    public bool winOnVIPKill = true;

    public int whiteTurnsPerRound = 1;
    public int blackTurnsPerRound = 1;

    public bool whiteTurn = true;
    public bool canMove = true;

    public bool saveToJson = false;

    //public GameMaster gameMaster;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init()
    {
        GameMaster.bowling = bowling;
        GameMaster.canabalism = canabalism;
        InitBoard();
        InitDirections();
    }

    public ChessPiece PieceAt(List<int> loc)
    {
        string str = CarlMath.ListAsString(loc);
        if (!squares.ContainsKey(str) || squares[str] == null)
            return null;
        return squares[str].piece;
    }

    public void UpdatePieceLocations()
    {
        GameObject[] pieces = GameObject.FindGameObjectsWithTag("Piece");
        foreach (Tile square in squares.Values)
        {
            square.piece = null;
        }
        foreach (GameObject piece in pieces)
        {
            RaycastHit hit;
            if (Physics.Raycast(piece.transform.position, -Vector3.up, out hit, 5, mask))
            {
                //Debug.Log(hit.transform.gameObject.name);
                if (hit.transform.gameObject.GetComponent<ChessPiece>() != null)
                {
                    //GameMaster.instance.board.squares[CarlMath.ListAsString(hit.transform.gameObject.GetComponent<ChessPiece>().location)].Click();
                    piece.GetComponent<ChessPiece>().location = hit.transform.gameObject.GetComponent<ChessPiece>().location;
                }
                else if (hit.transform.gameObject.GetComponent<Tile>() != null)
                {
                    //hit.transform.gameObject.GetComponent<Tile>().Click();
                    piece.GetComponent<ChessPiece>().location = hit.transform.gameObject.GetComponent<Tile>().GetLocation();
                    hit.transform.gameObject.GetComponent<Tile>().piece = piece.GetComponent<ChessPiece>();
                }
            }
        }
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
        GameMaster.displayLoc = new List<int>(startIter);
        SpawnSquares(0, startIter);
        StartCoroutine(HideOtherDimensions());


        GameMaster.displayDims = new int[] { xAxis, yAxis, zAxis, xxAxis, yyAxis, zzAxis };
        GameMaster.instance.cam.GetComponent<MainCamera>().selectors[0].GetComponent<TextMesh>().text = "∇ [" + (xAxis == -1 ? "nil" : "" + (xAxis+1)) + "]";
        GameMaster.instance.cam.GetComponent<MainCamera>().selectors[1].GetComponent<TextMesh>().text = "∇ [" + (yAxis == -1 ? "nil" : "" + (yAxis + 1)) + "]";
        GameMaster.instance.cam.GetComponent<MainCamera>().selectors[2].GetComponent<TextMesh>().text = "∇ [" + (zAxis == -1 ? "nil" : "" + (zAxis + 1)) + "]";
        GameMaster.instance.cam.GetComponent<MainCamera>().selectors[3].GetComponent<TextMesh>().text = "∇ [" + (xxAxis == -1 ? "nil" : "" + (xxAxis + 1)) + "]";
        GameMaster.instance.cam.GetComponent<MainCamera>().selectors[4].GetComponent<TextMesh>().text = "∇ [" + (yyAxis == -1 ? "nil" : "" + (yyAxis + 1)) + "]";
        GameMaster.instance.cam.GetComponent<MainCamera>().selectors[5].GetComponent<TextMesh>().text = "∇ [" + (zzAxis == -1 ? "nil" : "" + (zzAxis + 1)) + "]";
        GameMaster.instance.board.UpdateDimensions();
    }

    IEnumerator HideOtherDimensions()
    {
        yield return new WaitForSeconds(0.3f);
        UpdateDimensions();
    }

    public void UpdateSquares()
    {
        foreach (Tile square in squares.Values)
        {
            bool hide = false;
            for (int i = 0; i < dimensions; i++)
            {
                if (GameMaster.displayDims[0] != i && GameMaster.displayDims[1] != i && GameMaster.displayDims[2] != i && GameMaster.displayDims[3] != i && GameMaster.displayDims[4] != i && GameMaster.displayDims[5] != i && square.GetLocation()[i] != GameMaster.displayLoc[i])
                    hide = true;
                

            }
        }
    }

    public float GetMinY()
    {
        float result = -69420;
        foreach (Tile square in squares.Values)
        {
            if (result == -69420 || square.transform.position.y < result)
                result = square.transform.position.y;
        }
        return result;
    }

    void SpawnSquares(int index, List<int> iter)
    {
        if (index >= dimensions) 
        {
            int dimX = GameMaster.displayDims[0];
            int dimY = GameMaster.displayDims[1];
            int dimZ = GameMaster.displayDims[2];
            int dimXX = GameMaster.displayDims[3];
            int dimYY = GameMaster.displayDims[4];
            int dimZZ = GameMaster.displayDims[5];
            GameObject newTile = Instantiate(boardSquare, new Vector3(dimensions > 0 ? iter[dimX]-size[dimX]/2 + 0.5f : 0, dimensions > 2 ? 1.5f*(iter[dimZ]*2-size[dimZ]) : 0, dimensions > 1 ? iter[dimY]-size[dimY]/2 + 0.5f : 0), Quaternion.identity);
            newTile.transform.position += new Vector3(dimensions > 3 ? (GameMaster.spaceBetweenX * size[dimX] * (iter[dimXX]-size[dimXX]/2 + 0.5f)) : 0, dimensions > 5 ? (GameMaster.spaceBetweenZ * size[dimZ]*(iter[dimZZ] - size[dimZZ] / 2 + 0.5f)) : 0, dimensions > 4 ? (GameMaster.spaceBetweenY * size[dimY] * (iter[dimYY] - size[dimYY] / 2 + 0.5f)) : 0);
            int evens = 0;
            bool hide = false;
            for (int i = 0; i < iter.Count; i++) {
                evens += iter[i] % 2 == 0 ? 1 : 0;
                if (GameMaster.displayDims[0] != i && GameMaster.displayDims[1] != i && GameMaster.displayDims[2] != i && GameMaster.displayDims[3] != i && GameMaster.displayDims[4] != i && GameMaster.displayDims[5] != i)
                    hide = true;
            }
            if (evens % 2 != 0)
            {
                newTile.GetComponent<Tile>().SetColor(colorA);
            }
            else
            {
                newTile.GetComponent<Tile>().SetColor(colorB);
            }
            newTile.GetComponent<Tile>().SetLocation(new List<int>(iter));
            newTile.GetComponent<Tile>().board = this;
            //if (hide) ; // hide shit (maybe this should be done after the piece gets placed
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

    public void UpdateDimensions()
    {
        int dimX = GameMaster.displayDims[0];
        int dimY = GameMaster.displayDims[1];
        int dimZ = GameMaster.displayDims[2];
        int dimXX = GameMaster.displayDims[3];
        int dimYY = GameMaster.displayDims[4];
        int dimZZ = GameMaster.displayDims[5];
        foreach (Tile square in squares.Values)
        {
            float xPos = (dimX == -1 ? 0 : square.GetLocation()[dimX] - size[dimX] / 2 + 0.5f) + (dimXX == -1 ? 0 : size[dimX] * GameMaster.spaceBetweenX * square.GetLocation()[dimXX]);
            float yPos = (dimY == -1 ? 0 : square.GetLocation()[dimY] - size[dimY] / 2 + 0.5f) + (dimYY == -1 ? 0 : size[dimY] * GameMaster.spaceBetweenY * square.GetLocation()[dimYY]);
            float zPos = (dimZ == -1 ? 0 : 1.5f * (square.GetLocation()[dimZ] * 2 - size[dimZ]) + (dimZZ == -1 ? 0 : size[dimZ] * 1.5f * GameMaster.spaceBetweenZ * square.GetLocation()[dimZZ]));
            square.console += "\n\n---------\nxPos = " + xPos + ", yPos = " + yPos + ", zPos = " + zPos;
            bool skip = false;
            for (int i = 0; i < dimensions; i++)
            {
                if (!GameMaster.displayDims.Contains(i) && square.GetLocation()[i] != GameMaster.displayLoc[i])
                    skip = true;
                //else if (GameMaster.displayDims.Contains(i))
                //    square.console += "\ndisplayDims contains " + i;
                //else
                //    square.console += "\n" + square.GetLocation()[i] + " = " + GameMaster.displayLoc[i];
            }
            if (skip)
            {
                //square.console += "SKIPPING!";
                if (square.piece != null)
                {
                    //square.piece.gameObject.SetActive(false);
                    square.piece.GetComponent<MeshRenderer>().enabled = false;
                    square.piece.GetComponent<BoxCollider>().enabled = false;
                    square.piece.GetComponent<Rigidbody>().useGravity = false;
                    square.piece.gameObject.layer = 2;
                }
                if (GameMaster.bowling)
                {
                    GameObject[] pieces = GameObject.FindGameObjectsWithTag("Piece");
                    foreach (GameObject piece in pieces)
                    {
                        if (CarlMath.equals(piece.GetComponent<ChessPiece>().location, square.GetLocation()))
                        {
                            piece.GetComponent<MeshRenderer>().enabled = false;
                            piece.GetComponent<BoxCollider>().enabled = false;
                            piece.GetComponent<Rigidbody>().useGravity = false;
                            piece.gameObject.layer = 2;
                        }
                    }
                }
                //square.gameObject.SetActive(false);
                square.GetComponent<MeshRenderer>().enabled = false;
                square.GetComponent<BoxCollider>().enabled = false;
                square.gameObject.layer = 2;
            } else
            {
                //square.console += "UNDO SKIP";
                //square.gameObject.SetActive(true);
                square.GetComponent<MeshRenderer>().enabled = true;
                square.GetComponent<BoxCollider>().enabled = true;
                square.gameObject.layer = 0;
                if (square.piece != null)
                {
                    //square.piece.gameObject.SetActive(true);
                    square.piece.GetComponent<MeshRenderer>().enabled = true;
                    square.piece.GetComponent<BoxCollider>().enabled = true;
                    if (GameMaster.bowling) square.piece.GetComponent<Rigidbody>().useGravity = true;
                    square.piece.gameObject.layer = 0;
                }
                if (GameMaster.bowling)
                {
                    GameObject[] pieces = GameObject.FindGameObjectsWithTag("Piece");
                    foreach (GameObject piece in pieces)
                    {
                        if (CarlMath.equals(piece.GetComponent<ChessPiece>().location, square.GetLocation()))
                        {
                            piece.GetComponent<MeshRenderer>().enabled = true;
                            piece.GetComponent<BoxCollider>().enabled = true;
                            if (GameMaster.bowling) piece.GetComponent<Rigidbody>().useGravity = true;
                            piece.gameObject.layer = 0;
                        }
                    }
                }
            }
            square.transform.localEulerAngles = Vector3.zero;
            square.transform.position = new Vector3(xPos, zPos, yPos);
            if (dimZZ != -1) square.transform.RotateAround(new Vector3(size[dimX] / 2, size[dimZ] / 2, size[dimY] / 2), Vector3.up, 20 * square.GetLocation()[dimZZ]);
            if (square.piece != null)
                square.piece.transform.position = square.transform.position;
        }
    }

    public Tile[] GetBestMove()
    {
        Debug.Log("Getting best move");
        Tile[] result = { null, null };
        int p = 0;

        foreach (Tile t in squares.Values)
        {
            if (t.piece != null && t.piece.white == whiteTurn)
            {
                foreach (Tile square in squares.Values)
                {
                    if (!square.gameObject.Equals(gameObject))
                        square.ClearColor();
                }
                Tile newT = t.piece.GetBestMove();
                if (newT != null)
                {
                    int newP = newT.piece != null ? (newT.piece.points * (newT.piece.white != t.piece.white ? 1 : (GameMaster.bowling ? 0 : -1))) : 0;
                    Debug.Log("newP = " + newP);
                    if (result[0] == null || newP > p || (newP == p && Random.Range(0, Mathf.Max(1, squares.Count / 2)) == 0))
                    {
                        result[1] = newT;
                        result[0] = t;
                        p = newP;
                    }
                }
            }
        }

        return result;
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
       if (saveToJson)
       {
            saveToJson = false;
            string json = JsonUtility.ToJson(this);
            FileIO.WriteString(json, "Assets" + System.IO.Path.DirectorySeparatorChar + "GameModes" + System.IO.Path.DirectorySeparatorChar + gameModeName.Replace(" ", "_") + ".json");
            Debug.Log("Printing to " + gameModeName.Replace(" ", "_") + ".json");
       }
    }

    public void loadFromFile(string path)
    {
        Board o = new Board();
        JsonUtility.FromJsonOverwrite(FileIO.ReadString(path), o);
        gameModeName = o.gameModeName;
        xAxis = o.xAxis;
        yAxis = o.yAxis;
        zAxis = o.zAxis;
        xxAxis = o.xxAxis;
        yyAxis = o.yyAxis;
        zzAxis = o.zzAxis;
        mask = o.mask;
        size = o.size;
        dimensionsToMirror = o.dimensionsToMirror;
        colorA = o.colorA;
        colorB = o.colorB;
        swapBlackAndWhite = o.swapBlackAndWhite;
        groundLevel = o.groundLevel;
        bowling = o.bowling;
        canabalism = o.canabalism;
        winOnVIPKill = o.winOnVIPKill;
        whiteTurnsPerRound = o.whiteTurnsPerRound;
        blackTurnsPerRound = o.blackTurnsPerRound;
        whiteTurn = o.whiteTurn;
        description = o.description;
        pieceSet = o.pieceSet;
    }
}
